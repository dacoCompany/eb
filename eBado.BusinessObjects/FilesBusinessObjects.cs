using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Hosting;
using eBado.Entities;
using Infrastructure.Common.DB;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using WebAPIFactory.Configuration.Core;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;

namespace eBado.BusinessObjects
{
    public class FilesBusinessObjects : IFilesBusinessObjects
    {
        private const string StorageConnectionStringKey = "StorageConnectionString";

        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
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

        public FilesBusinessObjects(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        public int UploadFiles(IEnumerable<FileEntity> files, string batchId)
        {
            var container = GetAzureBlobContainer();

            var batch = unitOfWork.BatchAttachmentRepository.FirstOrDefault(ba => ba.Name == batchId);
            int fileCount = 0;
            foreach (var file in files)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("photos/" + file.Name);
                blockBlob.UploadFromByteArray(file.Content, 0, file.Content.Length);
                file.Url = blockBlob.Uri.ToString();

                if (!supportedFileTypes.Contains(file.ContentType) && !supportedImageTypes.Contains(file.ContentType))
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
                ++fileCount;

            }

            unitOfWork.Commit();
            return fileCount;
        }

        public bool DeleteFile(string fileName)
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

            return result && resultThumb && dboDeleted;
        }

        public IEnumerable<FileEntity> GetBatchFiles(int batchId)
        {
            var files = new Collection<FileEntity>();
            return files;
            var batch = unitOfWork.BatchAttachmentRepository.FindById(batchId);

            if (batch.Attachments == null || !batch.Attachments.Any())
                return files;

            foreach (var attachment in batch.Attachments)
            {
                files.Add(new FileEntity { Name = attachment.OriginalUrl.Split('/').Last(), ThumbnailUrl = attachment.ThumbnailUrl, Url = attachment.OriginalUrl });
            }

            return files;
        }

        public string CreateBatch(string name, string description)
        {
            var guid = Guid.NewGuid();

            var batchDbo = new BatchAttachmentDbo { Name = guid.ToString(), Description = description, CompanyDetailsId = 1 };

            unitOfWork.BatchAttachmentRepository.Add(batchDbo);
            unitOfWork.Commit();

            EntlibLogger.LogInfo("File", "Create Batch", $"Created new batch with id: {guid.ToString()}", new DiagnosticsLogging());

            return guid.ToString();
        }

        private CloudBlobContainer GetAzureBlobContainer()
        {
            try
            {
                // get connection string
                var valueByKey = configuration.GetValueByKey(StorageConnectionStringKey);
                var storageAccount = CloudStorageAccount.Parse(valueByKey);

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
            catch (Exception e)
            {
                EntlibLogger.LogError("FileBO", "Error", e.Message, new DiagnosticsLogging(), e);
                throw;
            }

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
