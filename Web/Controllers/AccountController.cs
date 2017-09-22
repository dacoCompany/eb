using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.DB;
using Infrastructure.Common.Validations;
using System.Web.Mvc;
using AutoMapper;
using eBado.BusinessObjects;
using eBado.Entities;
using Web.eBado.Helpers;
using Web.eBado.IoC;
using Web.eBado.Models.Account;
using Web.eBado.Models.MvcExtensions;
using Web.eBado.Models.Shared;
using Web.eBado.Validators;
using WebAPIFactory.Configuration.Core;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;
using System.Linq;
using System.Data.Entity;
using System.Web.Security;

namespace Web.eBado.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        AccountHelper accountHelper;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFilesBusinessObjects fileBo;

        public AccountController(IConfiguration configuration, IUnitOfWork unitOfWork, IFilesBusinessObjects fileBo)
        {
            this.accountHelper = new AccountHelper();
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.fileBo = fileBo;
        }

        #region HTTP GET

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterUser()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterCompany()
        {
            RegisterCompanyModel model = new RegisterCompanyModel();
            accountHelper.InitializeAllCategories(model);
            model.CompanyModel.CompanyLocation = accountHelper.GetCountryByIP();
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserAccountSettings()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangeSettings()
        {
            ChangeSettingsModel model = new ChangeSettingsModel();
            model.Title = "Ing.";
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AccountGallery(string batchId)
        {
            var model = new AttachmentGalleryModel();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AttachmentEntity, AttachmentModel>();
                cfg.CreateMap<AttachmentGalleryEntity, AttachmentGalleryModel>();
            });

            var entities = fileBo.GetBatchFiles(batchId);
            model = Mapper.Map<AttachmentGalleryModel>(entities);
            
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult EditAccountGallery(string batchId)
        {
            var model = new AttachmentGalleryModel();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AttachmentEntity, AttachmentModel>();
                cfg.CreateMap<AttachmentGalleryEntity, AttachmentGalleryModel>();
            });

            var entities = fileBo.GetBatchFiles(batchId);
            model = Mapper.Map<AttachmentGalleryModel>(entities);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult BatchAccountGallery()
        {
            try
            {
                var model = new BatchGalleryModel();
                var entities = fileBo.GetBatches(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<BatchModel, BatchEntity>().ReverseMap();
                });

                var fileEntities = Mapper.Map<Collection<BatchModel>>(entities);
                model.Batch = fileEntities;
                return View(model);
            }
            catch (Exception e)
            {
                EntlibLogger.LogError("Account", "FileUpload", e.Message, DiagnosticsLogging.Create("Controller", "Account"), e);
                throw;
            }

        }

        #endregion

        #region HTTP POST

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            var validationResult = new ValidationResultCollection();
            AccountValidator.ValidateUserLogin(unitOfWork, validationResult, model);
            ModelState.AddModelErrors(validationResult);

            if (ModelState.IsValid)
            {
                var userDetail = unitOfWork.UserDetailsRepository.FindWhere(ud => ud.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                    .Include(ud => ud.UserRole.UserRole2UserPermission.Select(ur => ur.UserPermission))
                    .Include(ud => ud.CompanyDetails2UserDetails.Select(cd => cd.CompanyDetail))
                    .Include(ud => ud.CompanyDetails2UserDetails.Select(cd => cd.CompanyRole.CompanyRole2CompanyPermission
                      .Select(cr => cr.CompanyPermission))).FirstOrDefault();

                var userRole = userDetail.UserRole;

                SessionModel session = new SessionModel()
                {
                    Id = userDetail.Id,
                    Email = userDetail.Email,
                    Name = userDetail.DisplayName,
                    UserRole = userRole.Name,
                    UserPermissions = userRole.UserRole2UserPermission.Select(ur => ur.UserPermission.Name),
                    HasCompany = userDetail.CompanyDetails2UserDetails.Any(cd => cd.IsActive == true)
                };
                foreach (var company in userDetail.CompanyDetails2UserDetails)
                {
                    var companyDetail = company.CompanyDetail;
                    var companyRole = company.CompanyRole;
                    CompanySessionModel companySession = new CompanySessionModel()
                    {
                        Id = companyDetail.Id,
                        Name = companyDetail.Name,
                        CompanyRole = companyRole.Name,
                        CompanyPermissions = companyRole.CompanyRole2CompanyPermission.Select(cr => cr.CompanyPermission.Name)
                    };
                    session.Companies.Add(companySession);
                }

                FormsAuthentication.SetAuthCookie(session.Email, false);
                Session["User"] = session;

            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(RegisterUserModel model)
        {
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {
                EntlibLogger.LogError("Account", "Register", $"Registration attempt with e-mail address: {model.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                EntlibLogger.LogInfo("Account", "Register", $"Registration attempt with e-mail address: {model.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                EntlibLogger.LogWarning("Account", "Register", $"Registration attempt with e-mail address: {model.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                EntlibLogger.LogVerbose("Account", "Register", $"Registration attempt with e-mail address: {model.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                var validationResult = new ValidationResultCollection();
                AccountValidator.ValidateUserRegistration(uow, validationResult, model);
                if (AccountHelper.IsValidCaptcha())
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            accountHelper.RegisterUser(model, uow);
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCompany(RegisterCompanyModel model)
        {
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {
                var validationResult = new ValidationResultCollection();
                AccountValidator.ValidateUserRegistration(uow, validationResult, model.UserModel);
                if (validationResult.Count > 0)
                {
                    ModelState.AddModelErrors(validationResult);
                }

                if (AccountHelper.IsValidCaptcha())
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            accountHelper.RegisterCompany(model, uow);
                        }
                        catch
                        {
                            accountHelper.InitializeAllCategories(model);
                        }
                    }
                    else
                    {
                        accountHelper.InitializeAllCategories(model);
                    }
                }
            }

            return View(model);
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            using (var uow = new UnitOfWork())
            {
                //if (!accountHelper.CheckIfEmailExist(model.Email, uow))
                //{
                //    ModelState.AddModelError("Email", "Email does not exist!");
                //    return View(model);
                //}
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSettings(ChangeSettingsModel model)
        {
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BatchAccountGallery(BatchGalleryModel model)
        {
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBatch(BatchGalleryModel model)
        {
            string batchUniqueId = fileBo.CreateBatch(model.Name, model.Description);
            EntlibLogger.LogInfo("File", "Create Batch", $"Created new batch with id: {batchUniqueId}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Account" });
            return RedirectToAction("EditAccountGallery", new { batchId = batchUniqueId });
        }
        #endregion
    }
}