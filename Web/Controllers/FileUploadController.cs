using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using Infrastructure.Common.DB;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Web.eBado.Helpers;
using Web.eBado.Models.Shared;
using WebAPIFactory.Configuration.Core;
using System.Drawing;

namespace Web.eBado.Controllers
{
    public class FileUploadController : Controller
    {
        FilesHelper filesHelper;
        string serverMapPath = "~/Files/somefiles/";
        private string StorageRoot
        {
            get { return Path.Combine(HostingEnvironment.MapPath(serverMapPath)); }
        }
        private string UrlBase = "/Files/somefiles/";
        string DeleteURL = "/FileUpload/DeleteFile/?file=";
        string DeleteType = "GET";
        private readonly HashSet<string> supportedImageTypes = new HashSet<string> { "image/jpg", "image/jpeg", "image/png", "image/png", "image/bmp", "image/tiff", "image/tif" };
        private readonly HashSet<string> supportedFileTypes = new HashSet<string>
        {
            "application/msword",
            "application/vnd.ms-word.document.macroEnabled.12",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.ms-excel.sheet.macroEnabled.12",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-powerpoint",
            "application/vnd.ms-powerpoint.presentation.macroenabled.12",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.ms-powerpoint.slideshow.macroenabled.12",
            "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
            "application/pdf",
            "application/x-zip-compressed",
            "application/x-7z-compressed"
        };
        private const string StorageConnectionStringKey = "StorageConnectionString";
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;

        public FileUploadController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            filesHelper = new FilesHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, serverMapPath);
        }

        public FilesViewModel Show()
        {
            JsonFiles ListOfFiles = filesHelper.GetFileList();
            var model = new FilesViewModel()
            {
                Files = ListOfFiles.files
            };

            return model;
        }

        [HttpPost]
        public JsonResult Upload()
        {
            var resultList = new List<ViewDataUploadFilesResult>();

            var CurrentContext = HttpContext;

            filesHelper.UploadAndShowResults(CurrentContext, resultList);
            JsonFiles files = new JsonFiles(resultList);

            bool isEmpty = !resultList.Any();
            if (isEmpty)
            {
                return Json("Error ");
            }
            else
            {
                return Json(files);
            }
        }
        public JsonResult GetFileList()
        {
            var list = filesHelper.GetFileList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteFile(string file)
        {
            filesHelper.DeleteFile(file);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadFileAzure()
        {
            var container = GetAzureBlobContainer();

            var files = MapAttachmentsFromRequest();

            var batch = new BatchAttachmentDbo
            {
                Description = "Some batch",
                Name = "TestBatch",
                UserAccountId = 1
            };

            foreach (var file in files)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("photos/" + file.Name);
                blockBlob.UploadFromByteArray(file.Content, 0, file.Content.Length);
                file.Url = blockBlob.Uri.ToString();

                if (!supportedFileTypes.Contains(file.ContentType))
                    continue;

                string fileThumb = Path.GetFileNameWithoutExtension(file.Name) + "_thumbImg.jpg";
                if (supportedImageTypes.Contains(file.ContentType))
                {
                    using (MemoryStream stream = new MemoryStream(file.Content))
                    {
                        var thumbnail = new WebImage(stream).Resize(130, 100, true, true);
                        thumbnail.FileName = fileThumb;
                        byte[] thumb = thumbnail.GetBytes("image/jpeg");
                        CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference("photos/" + fileThumb);
                        blockBlobThumb.UploadFromByteArray(thumb, 0, thumb.Length);
                        file.ThumbnailUrl = blockBlobThumb.Uri.ToString();
                    }
                }
                else
                {
                    // get thumbnail for non-image file
                    string extension = GetExtensionFromMimeType(file.ContentType);
                    string path = HostingEnvironment.MapPath($@"~/Content/Free-file-icons/48px/{extension}.png");
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        byte[] thumb = new byte[1048576];
                        stream.ReadAsync(thumb, 0, (int)stream.Length);
                        CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference("photos/" + fileThumb);
                        blockBlobThumb.UploadFromByteArray(thumb, 0, thumb.Length);
                        file.ThumbnailUrl = blockBlobThumb.Uri.ToString();
                    }
                }

                var attachment = new AttachmentDbo
                {
                    OriginalUrl = file.Url,
                    ThumbnailUrl = file.ThumbnailUrl
                };
                batch.Attachments.Add(attachment);

            }
            
            unitOfWork.BatchAttachmentRepository.Add(batch);
            unitOfWork.Commit();

            return files.Count == batch.Attachments.Count ? Json("Success", JsonRequestBehavior.AllowGet) : Json("Some files are not supported.", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteFileAzure(string fileName)
        {
            var container = GetAzureBlobContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("photos/" + fileName);
            bool result = blockBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

            string thumbName = Path.GetFileNameWithoutExtension(fileName) + "_thumbImg.jpg";
            CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference("photos/" + thumbName);
            bool resultThumb = blockBlobThumb.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

            var attachment = unitOfWork.AttachmentRepository.FirstOrDefault(a => a.OriginalUrl.Contains(fileName));
            bool dboDeleted = false;

            if (attachment != null)
            {
                attachment.IsActive = false;
                unitOfWork.Commit();
                dboDeleted = true;
            }


            return result && resultThumb && dboDeleted ? Json("Deleted", JsonRequestBehavior.AllowGet) : Json("Error", JsonRequestBehavior.AllowGet);
        }

        private CloudBlobContainer GetAzureBlobContainer()
        {
            // get connection string
            var storageAccount = CloudStorageAccount.Parse(configuration.GetValueByKey(StorageConnectionStringKey));

            // create blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // retrieve container reference
            CloudBlobContainer container = blobClient.GetContainerReference("ebadogallery");

            // create container if doesn't exist
            container.CreateIfNotExists();

            // set container permissions
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });

            return container;
        }

        private ICollection<FileModel> MapAttachmentsFromRequest()
        {
            HttpRequestBase currentRequest = HttpContext.Request;
            ICollection<FileModel> fileCollection = new Collection<FileModel>();

            if (currentRequest.Files.Count <= 0)
                return fileCollection;

            foreach (string file in currentRequest.Files)
            {
                var httpPostedFile = currentRequest.Files[file] as HttpPostedFileBase;
                if (httpPostedFile.ContentLength == 0)
                    continue;

                using (BinaryReader reader = new BinaryReader(httpPostedFile.InputStream))
                {
                    var byteFile = reader.ReadBytes(httpPostedFile.ContentLength);
                    fileCollection.Add(new FileModel
                    {
                        Name = new FileInfo(httpPostedFile.FileName).Name,
                        ContentType = httpPostedFile.ContentType,
                        Content = byteFile
                    });
                }
            }

            return fileCollection;
        }

        private string GetExtensionFromMimeType(string mimeType)
        {
            switch (mimeType)
            {
                case "application/msword":
                case "application/vnd.ms-word.document.macroEnabled.12":
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    return "doc";
                case "application/vnd.ms-excel":
                case "application/vnd.ms-excel.sheet.macroEnabled.12":
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    return "xls";
                case "application/vnd.ms-powerpoint":
                case "application/vnd.ms-powerpoint.presentation.macroenabled.12":
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                case "application/vnd.ms-powerpoint.slideshow.macroenabled.12":
                case "application/vnd.openxmlformats-officedocument.presentationml.slideshow":
                    return "ppt";
                case "application/pdf":
                    return "pdf";
                case "application/x-zip-compressed":
                case "application/x-7z-compressed":
                    return "zip";
                default:
                    return "_blank";
            }
        }

    }
}