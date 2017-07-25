using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Web.eBado.Helpers;
using Web.eBado.Models.Shared;
using WebAPIFactory.Configuration.Core;

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
        private const string StorageConnectionStringKey = "StorageConnectionString";
        private readonly IConfiguration configuration;

        public FileUploadController(IConfiguration configuration)
        {
            this.configuration = configuration;
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
        public void UploadFileAzure()
        {
            var container = GetAzureBlobContainer();

            var files = MapAttachmentsFromRequest();

            foreach (var file in files)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("photos/" + file.Name);
                blockBlob.UploadFromByteArray(file.Content, 0, file.Content.Length);
                file.Url = blockBlob.Uri.ToString();

                string fileThumb = Path.GetFileNameWithoutExtension(file.Name) + "_thumbImg.jpg";
                using (MemoryStream stream = new MemoryStream(file.Content))
                {
                    var thumbnail = new WebImage(stream).Resize(130, 100, true, true);
                    thumbnail.FileName = fileThumb;
                    //blockBlob.UploadFromByteArray(thumbnail); // get the byte data from the image, upload to blob
                }
            }


        }

        private CloudBlobContainer GetAzureBlobContainer()
        {
            // get connection string
            var storageAccount = CloudStorageAccount.Parse(configuration.GetValueByKey(StorageConnectionStringKey));

            // create blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // retrieve container reference
            CloudBlobContainer container = blobClient.GetContainerReference("webgallery");

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

            foreach (HttpPostedFileBase httpPostedFile in currentRequest.Files.AllKeys.Select(key => currentRequest.Files[key]).Where(httpPostedFile => httpPostedFile != null))
            {
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

    }
}