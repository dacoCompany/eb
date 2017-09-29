using System;
using Infrastructure.Common.DB;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using AutoMapper;
using eBado.BusinessObjects;
using eBado.Entities;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Web.eBado.Helpers;
using Web.eBado.Models.Shared;
using WebAPIFactory.Configuration.Core;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;

namespace Web.eBado.Controllers
{
    public class FileUploadController : Controller
    {
        FilesHelper filesHelper;
        
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFilesBusinessObjects filesBo;

        public FileUploadController(IConfiguration configuration, IUnitOfWork unitOfWork, IFilesBusinessObjects filesBo)
        {
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.filesBo = filesBo;
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
        public JsonResult Upload(string batchId)
        {
            try
            {
                var files = MapAttachmentsFromRequest();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<FileModel, FileEntity>();
                });

                var fileEntities = Mapper.Map<ICollection<FileEntity>>(files);

                int uploadedCount = filesBo.UploadFiles(fileEntities, batchId);

                return files.Count == uploadedCount ? Json("Success", JsonRequestBehavior.AllowGet) : Json("Some files are not supported.", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                EntlibLogger.LogError("File", "Upload", e.Message, DiagnosticsLogging.Create("Controller", "File"), e);
                throw;
            }
        }

        [HttpPost]
        public JsonResult Upload2()
        {
            var resultList = new List<ViewDataUploadFilesResult>();

            var CurrentContext = HttpContext;

            var filesHelper = new FilesHelper(null, null, null, null, null);
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

        public JsonResult GetFileList(string batchId)
        {
            var model = new AttachmentGalleryModel();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AttachmentEntity, AttachmentModel>();
                cfg.CreateMap<AttachmentGalleryEntity, AttachmentGalleryModel>();
            });

            var entities = filesBo.GetBatchFiles(batchId);
            model = Mapper.Map<AttachmentGalleryModel>(entities);
            return new JsonNetResult(model.Attachments);
        }

        [HttpPost]
        public ActionResult DeleteFiles(string batchId, ICollection<string> file)
        {
            bool deleted = filesBo.DeleteFiles(file, batchId);

            return deleted ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Some files cannot be deleted. Please try again later.");
        }

        [HttpPost]
        public JsonResult DeleteBatch(string batchId)
        {
            bool deleted = true;

            return deleted ? Json("Deleted", JsonRequestBehavior.AllowGet) : Json("Error", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteVideo(string batchId, string name)
        {
            bool deleted = true;

            return deleted ? Json("Deleted", JsonRequestBehavior.AllowGet) : Json("Error", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteFileAzure(string fileName)
        {
            bool deleted = filesBo.DeleteFile(fileName);

            return deleted ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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
                        Content = byteFile,
                        Size = byteFile.Length
                    });
                }
            }

            return fileCollection;
        }
    }
}