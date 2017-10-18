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
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult RegisterUser()
        {
            RegistrationModel model = new RegistrationModel();

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterCompany()
        {
            RegistrationModel model = new RegistrationModel();
            accountHelper.InitializeAllCategories(model.CompanyModel);
            model.CompanyModel.CompanyLocation = accountHelper.GetCountryByIP();

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangeSettings()
        {
            var currentUrl = Request.Url.ToString();
            if (UserNotAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = currentUrl });
            }
            ChangeSettingsModel model = new ChangeSettingsModel();
            model.Title = "Ing.";
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult MyAdvertisements()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult EditAccountGallery(string batchId)
        {
            var currentUrl = Request.Url.ToString();
            if (UserNotAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = currentUrl });
            }
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
            var currentUrl = Request.Url.ToString();
            if (UserNotAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = currentUrl });
            }
            try
            {
                var model = new BatchGalleryModel();
                int companyId = GetActiveCompany();
                var entities = fileBo.GetBatches(companyId);

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
            ModelState.AddValidationErrors(validationResult);

            if (ModelState.IsValid)
            {
                var userDetail = unitOfWork.UserDetailsRepository.FindWhere(ud => ud.Email.ToLower().Equals(model.Email.ToLower()))
                    .Include(ud => ud.UserRole.UserRole2UserPermission.Select(ur => ur.UserPermission))
                    .Include(ud => ud.CompanyDetails2UserDetails.Select(cd => cd.CompanyDetail))
                    .Include(ud => ud.CompanyDetails2UserDetails.Select(cd => cd.CompanyRole.CompanyRole2CompanyPermission
                      .Select(cr => cr.CompanyPermission))).FirstOrDefault();

                var userRole = userDetail.UserRole;

                SessionModel session = new SessionModel()
                {
                    Id = userDetail.Id,
                    IsActive = userDetail.IsActive,
                    Email = userDetail.Email,
                    Name = userDetail.DisplayName,
                    UserRole = userRole.Name,
                    UserPermissions = userRole.UserRole2UserPermission.Select(ur => ur.UserPermission.Name),
                    HasCompany = userDetail.CompanyDetails2UserDetails.Any(cd => cd.IsActive == true)
                };
                foreach (var company in userDetail.CompanyDetails2UserDetails.Where(cd => cd.IsActive))
                {
                    var companyDetail = company.CompanyDetail;
                    var companyRole = company.CompanyRole;
                    CompanySessionModel companySession = new CompanySessionModel()
                    {
                        Id = companyDetail.Id,
                        IsActive = companyDetail.IsActive,
                        Name = companyDetail.Name,
                        CompanyRole = companyRole.Name,
                        CompanyPermissions = companyRole.CompanyRole2CompanyPermission.Select(cr => cr.CompanyPermission.Name)
                    };
                    session.Companies.Add(companySession);
                }

                FormsAuthentication.SetAuthCookie(session.Email, true);
                Session["User"] = session;

                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(RegistrationModel model)
        {
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {
                //EntlibLogger.LogError("Account", "Register", $"Registration attempt with e-mail address: {model.UserModel.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                //EntlibLogger.LogInfo("Account", "Register", $"Registration attempt with e-mail address: {model.UserModel.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                //EntlibLogger.LogWarning("Account", "Register", $"Registration attempt with e-mail address: {model.UserModel.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                //EntlibLogger.LogVerbose("Account", "Register", $"Registration attempt with e-mail address: {model.UserModel.Email}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Register" });
                bool isRegistrationWithCompany = model.CompanyModel.CompanyType != 0;

                if (!ModelState.IsValid)
                {
                    if (isRegistrationWithCompany)
                    {
                        accountHelper.InitializeAllCategories(model.CompanyModel);
                        return View("RegisterCompany", model);
                    }
                    else
                    {
                        return View("RegisterUser", model);
                    }
                }

                var validationResult = new ValidationResultCollection();

                AccountValidator.ValidateUserRegistration(uow, validationResult, model.UserModel);
                if (isRegistrationWithCompany)
                {
                    AccountValidator.ValidateCompanyRegistration(uow, validationResult, model.CompanyModel);
                }

                if (validationResult.Any())
                {
                    ModelState.AddValidationErrors(validationResult);
                }

                if (AccountHelper.IsValidCaptcha())
                {
                    if (ModelState.IsValid)
                    {
                        accountHelper.Registration(model, uow, isRegistrationWithCompany);
                    }
                    else
                    {
                        if (isRegistrationWithCompany)
                        {
                            accountHelper.InitializeAllCategories(model.CompanyModel);
                            return View("RegisterCompany", model);
                        }
                        else
                        {
                            return View("RegisterUser", model);
                        }
                    }
                }
            }

            return RedirectToAction("Login", "Account");
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
            int companyId = GetActiveCompany();
            string batchUniqueId = fileBo.CreateBatch(model.Name, model.Description, companyId);
            EntlibLogger.LogInfo("File", "Create Batch", $"Created new batch with id: {batchUniqueId}", new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Account" });
            return RedirectToAction("EditAccountGallery", new { batchId = batchUniqueId });
        }
        #endregion

        #region Private methods

        private bool UserNotAuthenticated()
        {
            var session = (SessionModel)Session["User"];
            return !Request.IsAuthenticated || session == null ? true : false;
        }

        private int GetActiveCompany()
        {
            var session = Session["User"] as SessionModel;
            return session.Companies.First(c => c.IsActive).Id;
        }

        #endregion
    }
}