using System;
using System.Collections.ObjectModel;
using Infrastructure.Common.DB;
using Infrastructure.Common.Validations;
using System.Web.Mvc;
using AutoMapper;
using eBado.BusinessObjects;
using eBado.Entities;
using Web.eBado.Helpers;
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
using System.Collections.Generic;

namespace Web.eBado.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        AccountHelper accountHelper;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFilesBusinessObjects fileBo;
        private readonly DiagnosticsLogging diagnosticLogConstant;

        public AccountController(IConfiguration configuration, IUnitOfWork unitOfWork, IFilesBusinessObjects fileBo)
        {
            this.accountHelper = new AccountHelper();
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.fileBo = fileBo;
            diagnosticLogConstant = new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Account" };
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
            var model = new AccountSettingsModel();
            model.UserModel = new UserModel
            {
                PostalCode = "123456"
            };
            model.CompanyModel = new CompanyModel
            {
                CompanyPostalCode = "987654",
                Categories = new CategoriesModel
                {
                    SelectedCategories = new string[] {"daco1","daco2"}.ToArray()
                }
            };
            var daco = new SelectListItem            {
                Value = "1",
                Text = "daco"
            };
            var daco2 = new SelectListItem
            {
                Value = "2",
                Text = "daco2"
            };
            Collection<SelectListItem> dacoIne = new Collection<SelectListItem>();
            var daco3 = new UserRoleModel {
                UserID = 1,
                SelectedRoleId=1,
                UserEmail="mail@email.com"
            };
            var daco4 = new UserRoleModel
            {
                UserID = 2,
                SelectedRoleId = 1,
                UserEmail = "mail2@email.com"
            };
            List<UserRoleModel> userRoles = new List<UserRoleModel>();
            userRoles.Add(daco3);
            userRoles.Add(daco4);
            dacoIne.Add(daco);
            dacoIne.Add(daco2);
            model.EditMembersAndRolesModel = new EditMembersAndRolesModel
            {
                AllRoles = dacoIne,
                SelectedRoleId = 2,
                UserRoles = userRoles
            };

            accountHelper.InitializeAllCategories(model.CompanyModel);
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
                int? companyId = GetActiveCompany();

                if (companyId == null)
                {
                    return new HttpUnauthorizedResult();
                }

                var entities = fileBo.GetBatches(companyId.Value);

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
                EntlibLogger.LogError("Account", "BatchGallery", e.Message, diagnosticLogConstant, e);
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
            EntlibLogger.LogVerbose("Account", "Login", $"Login attempt with e-mail address: {model.Email}", diagnosticLogConstant);
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
                    IsActive = true,
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
                        IsActive = false,
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
                    EntlibLogger.LogVerbose("Account", "Login", $"Successful login with e-mail address: {model.Email}", diagnosticLogConstant);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                EntlibLogger.LogInfo("Account", "Login", $"Failed login with e-mail address: {model.Email}", diagnosticLogConstant);
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(RegistrationModel model)
        {
            EntlibLogger.LogVerbose("Account", "Register", $"Registration attempt with e-mail address: {model.UserModel.Email}", diagnosticLogConstant);

            if (!ModelState.IsValid)
            {
                return View("RegisterUser", model);
            }

            var validationResult = new ValidationResultCollection();

            AccountValidator.ValidateUserRegistration(unitOfWork, validationResult, model.UserModel);
            if (validationResult.Any())
            {
                ModelState.AddValidationErrors(validationResult);
            }

            if (AccountHelper.IsValidCaptcha())
            {
                if (ModelState.IsValid)
                {
                    accountHelper.RegisterUser(unitOfWork, model.UserModel, true);
                }
                else
                {
                    return View("RegisterUser", model);
                }
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCompany(RegistrationModel model)
        {
            EntlibLogger.LogVerbose("Account", "Register", $"Registration attempt (user & company) with e-mail address: {model.UserModel.Email}", diagnosticLogConstant);

            if (!ModelState.IsValid)
            {
                accountHelper.InitializeAllCategories(model.CompanyModel);
                return View("RegisterCompany", model);
            }

            var validationResult = new ValidationResultCollection();

            AccountValidator.ValidateUserRegistration(unitOfWork, validationResult, model.UserModel);
            AccountValidator.ValidateCompanyRegistration(unitOfWork, validationResult, model.CompanyModel);
            if (validationResult.Any())
            {
                ModelState.AddValidationErrors(validationResult);
                accountHelper.InitializeAllCategories(model.CompanyModel);
                return View("RegisterCompany", model);
            }

            if (AccountHelper.IsValidCaptcha())
            {
                var userDetail = accountHelper.RegisterUser(unitOfWork, model.UserModel);
                accountHelper.RegisterCompany(unitOfWork, model.CompanyModel, userDetail);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCompanySolo(CompanyModel model)
        {
            var session = Session["User"] as SessionModel;
            EntlibLogger.LogVerbose("Account", "Register", $"Registration attempt (company) with e-mail address: {session.Email}", diagnosticLogConstant);

            if (!ModelState.IsValid)
            {
                //TODO: Create RegisterCompanySolo view
                ////return View("RegisterCompany", model);
            }

            var validationResult = new ValidationResultCollection();

            AccountValidator.ValidateCompanyRegistration(unitOfWork, validationResult, model);
            if (validationResult.Any())
            {
                ModelState.AddValidationErrors(validationResult);
            }

            if (AccountHelper.IsValidCaptcha())
            {
                if (ModelState.IsValid)
                {
                    var userDetail = unitOfWork.UserDetailsRepository.FindById(session.Id);
                    accountHelper.RegisterCompany(unitOfWork, model, userDetail);
                }
                else
                {
                    //TODO: Create RegisterCompanySolo view
                    ////return View("RegisterCompany", model);
                }
            }

            return RedirectToAction("ChangeSettings", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            EntlibLogger.LogInfo("Account", "Forgot Password", $"Password reset requested for e-mail address: {model.Email}", diagnosticLogConstant);
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
        public ActionResult ChangeSettings(AccountSettingsModel model)
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
            int? companyId = GetActiveCompany();

            if (companyId == null)
            {
                return new HttpUnauthorizedResult();
            }

            string batchUniqueId = fileBo.CreateBatch(model.Name, model.Description, companyId.Value);
            EntlibLogger.LogInfo("File", "Create Batch", $"Created new batch with id: {batchUniqueId}", diagnosticLogConstant);
            return RedirectToAction("EditAccountGallery", new { batchId = batchUniqueId });
        }
        #endregion

        #region Private methods

        private bool UserNotAuthenticated()
        {
            var session = (SessionModel)Session["User"];
            return !Request.IsAuthenticated || session == null ? true : false;
        }

        private int? GetActiveCompany()
        {
            var session = Session["User"] as SessionModel;
            return session.Companies.First(c => c.IsActive)?.Id;
        }

        #endregion
    }
}