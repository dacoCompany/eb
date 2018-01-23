using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using ByteSizeLib;
using eBado.Entities;
using Infrastructure.Common.DB;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
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
        private readonly DiagnosticsLogging diagnosticLogConstant;
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
            diagnosticLogConstant = DiagnosticsLogging.Create("BusinessObject", "File");
        }

        public int UploadFiles(IEnumerable<FileEntity> files, string batchId, int companyId)
        {
            try
            {
                var container = GetAzureBlobContainer();

                var batch = unitOfWork.BatchAttachmentRepository.FindFirstOrDefault(ba => ba.GuId == batchId && ba.CompanyDetailsId == companyId);
                int fileCount = 0;

                foreach (var file in files)
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference($"photos/{batchId}/{file.Name}");
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
                            CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference($"photos/{batchId}/{fileThumb}");
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
                            CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference($"photos/{batchId}/{fileThumb}");
                            blockBlobThumb.UploadFromByteArray(thumb, 0, thumb.Length);
                            file.ThumbnailUrl = blockBlobThumb.Uri.ToString();
                        }
                    }

                    var attachment = new AttachmentDbo
                    {
                        OriginalUrl = file.Url,
                        ThumbnailUrl = file.ThumbnailUrl,
                        Name = file.Name,
                        Size = file.Size,
                        FileType = "file"
                    };
                    batch.Attachments.Add(attachment);
                    ++fileCount;
                }

                unitOfWork.Commit();

                EntlibLogger.LogVerbose("File", "Upload", $"Uploaded {fileCount} of {files.Count()} files to batch {batchId}.", diagnosticLogConstant);
                return fileCount;

            }
            catch (Exception ex)
            {
                EntlibLogger.LogError("File", "Upload", $"Upload failed. - {ex.Message}", diagnosticLogConstant, ex);
                throw;
            }
        }

        public string UploadProfilePicture(FileEntity file, string userId, string companyId)
        {
            try
            {
                if (!supportedImageTypes.Contains(file.ContentType))
                    throw new InvalidDataException("File type not supported.");

                var container = GetAzureBlobContainer();

                // TODO: add check if profile picture exists, then delete old

                string fileThumb128;
                string fileThumb256;
                string fileThumb512;
                UserDetailDbo user = null;
                CompanyDetailDbo company = null;

                if (!string.IsNullOrEmpty(userId))
                {
                    fileThumb128 = $"{userId}_128.jpg";
                    fileThumb256 = $"{userId}_256.jpg";
                    fileThumb512 = $"{userId}_512.jpg";
                    int userIdDecr = DecryptId(userId);
                    user = unitOfWork.UserDetailsRepository.FindById(userIdDecr);
                }
                else if (!string.IsNullOrEmpty(companyId))
                {
                    fileThumb128 = $"{companyId}_128.jpg";
                    fileThumb256 = $"{companyId}_256.jpg";
                    fileThumb512 = $"{companyId}_512.jpg";
                    int companyIdDecr = DecryptId(companyId);
                    company = unitOfWork.CompanyDetailsRepository.FindById(companyIdDecr);
                }
                else
                {
                    throw new Exception();
                }

                using (MemoryStream stream = new MemoryStream(file.Content))
                {
                    var thumbnail128 = new WebImage(stream).Resize(128, 128, true, true).Crop(1, 1);
                    thumbnail128.FileName = fileThumb128;
                    byte[] thumb128 = thumbnail128.GetBytes("image/jpeg");
                    CloudBlockBlob blockBlobThumb128 = container.GetBlockBlobReference($"profiles/{fileThumb128}");
                    blockBlobThumb128.UploadFromByteArray(thumb128, 0, thumb128.Length);
                    fileThumb128 = blockBlobThumb128.Uri.ToString();
                }

                using (MemoryStream stream = new MemoryStream(file.Content))
                {
                    var thumbnail256 = new WebImage(stream).Resize(256, 256, true, true).Crop(1, 1);
                    thumbnail256.FileName = fileThumb256;
                    byte[] thumb256 = thumbnail256.GetBytes("image/jpeg");
                    CloudBlockBlob blockBlobThumb256 = container.GetBlockBlobReference($"profiles/{fileThumb256}");
                    blockBlobThumb256.UploadFromByteArray(thumb256, 0, thumb256.Length);
                    fileThumb256 = blockBlobThumb256.Uri.ToString();
                }

                using (MemoryStream stream = new MemoryStream(file.Content))
                {
                    var thumbnail512 = new WebImage(stream).Resize(512, 512, true, true).Crop(1, 1);
                    thumbnail512.FileName = fileThumb512;
                    byte[] thumb512 = thumbnail512.GetBytes("image/jpeg");
                    CloudBlockBlob blockBlobThumb512 = container.GetBlockBlobReference($"profiles/{fileThumb512}");
                    blockBlobThumb512.UploadFromByteArray(thumb512, 0, thumb512.Length);
                    fileThumb512 = blockBlobThumb512.Uri.ToString();
                }

                if (user != null)
                {
                    user.ProfilePictureUrlSmall = fileThumb128;
                    user.ProfilePictureUrlMedium = fileThumb256;
                    user.ProfilePictureUrl = fileThumb512;
                }
                else if (company != null)
                {
                    company.ProfilePictureUrlSmall = fileThumb128;
                    company.ProfilePictureUrlMedium = fileThumb256;
                    company.ProfilePictureUrl = fileThumb512;
                }

                unitOfWork.Commit();
                EntlibLogger.LogVerbose("File", "UploadProfilePicture", $"Uploaded profile picture for user {userId}.", diagnosticLogConstant);

                return fileThumb512;
            }
            catch (Exception ex)
            {
                EntlibLogger.LogError("File", "UploadProfilePicture", $"Upload failed. - {ex.Message}", diagnosticLogConstant, ex);
                throw;
            }
        }

        public int DecryptId(string id)
        {
            int encryptConstant = 5168;
            int multiplyContstant = 42;

            var decryptedId = Convert.ToInt32(new String(id.Where(Char.IsDigit).ToArray()));
            return (decryptedId / multiplyContstant) - encryptConstant;
        }

        public bool UploadVideo(string url, string batchId, int companyId)
        {
            string videoId = HttpUtility.ParseQueryString(url).Get(0);

            if (string.IsNullOrEmpty(videoId))
            {
                return false;
            }

            string embedUrl = $"https://www.youtube.com/embed/{videoId}?rel=0";

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://www.youtube.com/");
            var response = client.GetAsync($"oembed?url=http://www.youtube.com/watch?v={videoId}&format=json").Result;

            Dictionary<string, string> values = new Dictionary<string, string>();
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;

                values = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            }
            else
            {
                return false;
            }

            try
            {
                var batch = unitOfWork.BatchAttachmentRepository.FindFirstOrDefault(ba => ba.GuId == batchId && ba.CompanyDetailsId == companyId);

                var videoAttachment = new AttachmentDbo
                {
                    BatchAttId = batch.Id,
                    Name = values["title"],
                    FileType = "video",
                    ThumbnailUrl = values["thumbnail_url"],
                    Size = 0,
                    OriginalUrl = embedUrl
                };

                batch.Attachments.Add(videoAttachment);

                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                EntlibLogger.LogError("File", "Video Upload", $"Video upload failed. - {ex.Message}", diagnosticLogConstant, ex);
                throw;
            }
        }

        public bool DeleteVideo(string batchId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            try
            {
                var batch = unitOfWork.BatchAttachmentRepository.FindFirstOrDefault(ba => ba.GuId == batchId);
                var attachment = unitOfWork.AttachmentRepository.FindFirstOrDefault(a => a.Name == name && a.BatchAttId == batch.Id);

                if (attachment != null)
                {
                    attachment.IsActive = false;
                }
                else
                {
                    return false;
                }

                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                EntlibLogger.LogError("File", "Delete Video", $"Video delete failed. - {ex.Message}", diagnosticLogConstant, ex);
                throw;
            }
        }

        public bool DeleteFile(string fileName)
        {
            var container = GetAzureBlobContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("photos/" + fileName);
            bool result = blockBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

            string thumbName = Path.GetFileNameWithoutExtension(fileName) + "_thumbImg.jpg";
            CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference("photos/" + thumbName);
            bool resultThumb = blockBlobThumb.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

            var attachment = unitOfWork.AttachmentRepository.FindFirstOrDefault(a => a.OriginalUrl.Contains(fileName));
            bool dboDeleted = false;

            if (attachment != null)
            {
                attachment.IsActive = false;
                unitOfWork.Commit();
                dboDeleted = true;
            }

            return result && resultThumb && dboDeleted;
        }

        public bool DeleteFiles(IEnumerable<string> files, string batchId)
        {
            var container = GetAzureBlobContainer();
            bool deletedAll = true;
            int deletedCount = 0;

            var batchDbo = unitOfWork.BatchAttachmentRepository.FindFirstOrDefault(ba => ba.GuId.Equals(batchId));

            foreach (string fileName in files)
            {
                string correctName = fileName;

                if (fileName.StartsWith("btn"))
                {
                    correctName = fileName.Substring(3);
                }

                CloudBlockBlob blockBlob = container.GetBlockBlobReference($"photos/{batchId}/{correctName}");
                bool result = blockBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

                string thumbName = Path.GetFileNameWithoutExtension(correctName) + "_thumbImg.jpg";
                CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference($"photos/{batchId}/{thumbName}");
                bool resultThumb = blockBlobThumb.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);

                var attachment = unitOfWork.AttachmentRepository.FindFirstOrDefault(a => a.OriginalUrl.Contains(correctName) && a.BatchAttId == batchDbo.Id);
                bool dboDeleted = false;

                if (attachment != null)
                {
                    attachment.IsActive = false;
                    dboDeleted = true;
                }

                if (!(result && resultThumb && dboDeleted))
                {
                    EntlibLogger.LogWarning("File", "Delete", $"File '{correctName}' in batch {batchId} cannot be deleted.", diagnosticLogConstant);
                    deletedAll = false;
                }
                else
                {
                    ++deletedCount;
                }
            }

            unitOfWork.Commit();
            batchDbo.ThumbnailUrl = batchDbo.Attachments.FirstActive()?.ThumbnailUrl;
            unitOfWork.Commit();
            if (deletedAll)
                EntlibLogger.LogVerbose("File", "Delete", $"Deleted {files.Count()} files.", diagnosticLogConstant);
            else
                EntlibLogger.LogInfo("File", "Delete", $"Deleted {deletedCount} of {files.Count()} files.", diagnosticLogConstant);

            return deletedAll;
        }

        public bool DeleteBatch(string batchId)
        {
            bool deletedBatch = false;

            try
            {
                var container = GetAzureBlobContainer();
                foreach (IListBlobItem blob in container.GetDirectoryReference(batchId).ListBlobs(true))
                {
                    if (blob.GetType() == typeof(CloudBlob) || blob.GetType().BaseType == typeof(CloudBlob))
                    {
                        ((CloudBlob)blob).DeleteIfExists();
                    }
                }

                var batch = unitOfWork.BatchAttachmentRepository.FindFirstOrDefault(ba => ba.GuId == batchId);
                batch.IsActive = false;

                batch.Attachments.ForEach(b => b.IsActive = false);

                unitOfWork.Commit();

                deletedBatch = true;
            }
            catch (Exception ex)
            {
                EntlibLogger.LogError("File", "Delete Batch", $"Batch {batchId} cannot be deleted. {ex.Message}", diagnosticLogConstant, ex);
                deletedBatch = false;
            }

            return deletedBatch;
        }

        public AttachmentGalleryEntity GetBatchFiles(string batchId)
        {
            if (string.IsNullOrEmpty(batchId))
            {
                throw new ArgumentNullException(nameof(batchId));
            }

            var batchDbo = unitOfWork.BatchAttachmentRepository.FindWhere(ba => ba.GuId == batchId).FirstOrDefault();

            if (batchDbo == null)
            {
                throw new ArgumentException("Invalid batch identifier.", nameof(batchId));
            }

            var result = new AttachmentGalleryEntity
            {
                Name = batchDbo.Name,
                Description = batchDbo.Description,
                Guid = batchDbo.GuId
            };

            foreach (var dbo in batchDbo.Attachments.WhereActive())
            {
                result.Attachments.Add(new AttachmentEntity
                {
                    Name = dbo.Name,
                    Size = ByteSize.FromBytes(dbo.Size).KiloBytes.ToString("N2"),
                    ThumbnailUrl = dbo.ThumbnailUrl,
                    Url = dbo.OriginalUrl,
                    Batch = batchDbo.GuId,
                    AttachmentType = dbo.FileType
                });
            }

            return result;
        }

        public string CreateBatch(string name, string description, int companyId)
        {
            var guid = Guid.NewGuid();

            var batchDbo = new BatchAttachmentDbo { Name = name, GuId = guid.ToString(), Description = description, CompanyDetailsId = companyId };

            unitOfWork.BatchAttachmentRepository.Add(batchDbo);
            unitOfWork.Commit();

            EntlibLogger.LogInfo("File", "Create Batch", $"Created new batch with name '{name}', id: {guid.ToString()}", diagnosticLogConstant);

            return guid.ToString();
        }

        public ICollection<BatchEntity> GetBatches(int companyId)
        {
            if (companyId <= 0)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var companyDbo = unitOfWork.CompanyDetailsRepository.FindWhere(cd => cd.Id == companyId).Include(cd => cd.BatchAttachments).FirstOrDefault();

            if (companyDbo == null)
            {
                throw new ArgumentException("Invalid company identifier.", nameof(companyId));
            }

            var resposne = new Collection<BatchEntity>();

            foreach (var batch in companyDbo.BatchAttachments)
            {
                resposne.Add(new BatchEntity
                {
                    Id = batch.Id,
                    Name = batch.Name,
                    Guid = batch.GuId,
                    Description = batch.Description.Length > 100 ? batch.Description.Substring(0, 100) : batch.Description,
                    AttachmentsCount = batch.Attachments.CountActive(),
                    BaseThumbUrl = batch.Attachments.CountActive() > 0 ? batch.Attachments.FirstActive().ThumbnailUrl : null
                });
            }

            return resposne;
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
