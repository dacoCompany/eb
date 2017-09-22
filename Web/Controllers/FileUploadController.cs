using System;
using Infrastructure.Common.DB;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        public JsonResult GetFileList()
        {
            //var list = filesHelper.GetFileList();
            return Json(JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteFile(string file)
        {
            bool deleted = filesBo.DeleteFile(file);

            return deleted ? Json("Deleted", JsonRequestBehavior.AllowGet) : Json("Error", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteFileAzure(string fileName)
        {
            bool deleted = filesBo.DeleteFile(fileName);

            return deleted ? Json("Deleted", JsonRequestBehavior.AllowGet) : Json("Error", JsonRequestBehavior.AllowGet);
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