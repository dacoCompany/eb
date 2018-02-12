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
using System.Web.Security;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.IO;
using Infrastructure.Common.Enums;
using Infrastructure.Common;

namespace Web.eBado.Controllers
{
    [RoutePrefix("Account")]
    public class AccountController : Controller
    {
        private readonly AccountHelper accountHelper;
        private readonly SessionHelper sessionHelper;
        private readonly SharedHelper sharedHelper;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFilesBusinessObjects fileBo;
        private readonly DiagnosticsLogging diagnosticLogConstant;

        public AccountController(IConfiguration configuration, IUnitOfWork unitOfWork, IFilesBusinessObjects fileBo)
        {
            accountHelper = new AccountHelper(unitOfWork, fileBo);
            sessionHelper = new SessionHelper(unitOfWork);
            sharedHelper = new SharedHelper(unitOfWork);
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.fileBo = fileBo;
            diagnosticLogConstant = new DiagnosticsLogging { DiagnosticsArea = "Controller", DiagnosticsCategory = "Account" };
        }

        #region HTTP GET

        [AllowAnonymous]
        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("Logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [Route("RegisterUser")]
        public ActionResult RegisterUser()
        {
            RegistrationModel model = new RegistrationModel();

            return View(model);
        }

        [AllowAnonymous]
        [Route("RegisterCompany")]
        public ActionResult RegisterCompany()
        {
            RegistrationModel model = new RegistrationModel();
            accountHelper.InitializeData(model.CompanyModel, unitOfWork);

            return View(model);
        }

        [AllowAnonymous]
        [Route("ForgotPassword")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [EbadoMvcAuthorization]
        [Route("ChangeSettings")]
        [NoClientCache]
        public ActionResult ChangeSettings()
        {
            if (IsUserAuthnticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
            }
            AccountSettingsModel model = new AccountSettingsModel();
            var session = Session["User"] as SessionModel;
            try
            {
                if (session.IsActive)
                {
                    model = accountHelper.GetUserSettings(unitOfWork, session);
                }
                else
                {
                    model = accountHelper.GetCompanySettings(unitOfWork, session);
                }
            }
            catch (Exception e)
            {
                EntlibLogger.LogError("Account", "ChangeSettings-GET", e.Message, diagnosticLogConstant, e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }


            accountHelper.InitializeData(model.CompanyModel, unitOfWork);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult MyAdvertisements()
        {
            return View();
        }

        [EbadoMvcAuthorization(Roles = "AddGallery,RemoveGallery,AddAttachments,RemoveAttachments")]
        [Route("EditAccountGallery")]
        public ActionResult EditAccountGallery(string batchId)
        {
            if (IsUserAuthnticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
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

        [EbadoMvcAuthorization(Roles = "AddGallery,RemoveGallery,AddAttachments,RemoveAttachments")]
        [Route("BatchAccountGallery")]
        public ActionResult BatchAccountGallery()
        {
            if (IsUserAuthnticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
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
        [Route("Login")]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            EntlibLogger.LogVerbose("Account", "Login", $"Login attempt with e-mail address: {model.Email}", diagnosticLogConstant);
            var validationResult = new ValidationResultCollection();
            AccountValidator.ValidateUserLogin(unitOfWork, validationResult, model);
            ModelState.AddValidationErrors(validationResult);

            if (ModelState.IsValid)
            {
                var userDetail = unitOfWork.UserDetailsRepository.FindFirstOrDefault(ud => ud.Email.ToLower().Equals(model.Email.ToLower()));
                var session = sessionHelper.SetUserSession(userDetail.Id);

                string language = userDetail.UserSetting.Language;
                sharedHelper.GetCultureInfo(language ?? Constants.EnglishCultureInfo);

                //FormsAuthentication.SetAuthCookie(session.Email, true);
                Session["User"] = session;

                var client = new HttpClient();
                string authServerBaseUri = configuration.GetValueByKey("AuthServerBaseUri");
                client.BaseAddress = new Uri(authServerBaseUri);

                var response = await client.GetAsync($"api/OAuth/GetLoginToken?appId=123&userRoleId={userDetail.UserRoleId}&companyRoleId=0");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var authCookie = new HttpCookie("tokenCookie", content.Replace("\"", string.Empty)) { HttpOnly = true };
                    HttpContext.Response.AppendCookie(authCookie);
                    HttpContext.Response.AppendHeader("Authorization", $"Bearer {authCookie}");
                }
                else
                {
                    string content = await response.Content.ReadAsStringAsync();
                    model.ErrorMessage = "Authentication failed";
                    EntlibLogger.LogInfo("Account", "Login", $"Failed login with e-mail address: {model.Email}. Authentication token cannot be issued. \t {response.StatusCode} {response.ReasonPhrase}\r\n{content}\r\n{client.BaseAddress}\r\n{userDetail.Id}\t{userDetail.UserRoleId}", diagnosticLogConstant);
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }

                if (returnUrl != null)
                {
                    EntlibLogger.LogVerbose("Account", "Login", $"Successful login with e-mail address: {model.Email}", diagnosticLogConstant);
                    model.ReturnUrl = returnUrl;
                    model.ShouldRedirect = true;
                    return View(model);
                }
                else
                {
                    EntlibLogger.LogVerbose("Account", "Login", $"Successful login with e-mail address: {model.Email}", diagnosticLogConstant);
                    model.ReturnUrl = "~/Home/Index";
                    model.ShouldRedirect = true;
                    return View(model);
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
        [Route("RegisterUser")]
        public ActionResult RegisterUser(RegistrationModel model)
        {
            EntlibLogger.LogVerbose("Account", "Register", $"Registration attempt with e-mail address: {model.UserModel.Email}", diagnosticLogConstant);

            var entlibValidationResult = Validation.Validate(model, "RegisterUser");

            if (!entlibValidationResult.IsValid)
            {
                this.ModelState.AddValidationErrors(entlibValidationResult);

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

            return RedirectToAction("ChangeSettings", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("RegisterCompany")]
        public ActionResult RegisterCompany(RegistrationModel model)
        {
            EntlibLogger.LogVerbose("Account", "Register", $"Registration attempt (user & company) with e-mail address: {model.UserModel.Email}", diagnosticLogConstant);

            var ruleSets = new List<string> { RuleSets.User };
            if (model.CompanyModel.CompanyType == CompanyType.PartTime)
            {
                ruleSets.Add(RuleSets.Contractor);
            }
            else
            {
                ruleSets.Add(RuleSets.Company);
            }

            var entlibValidationResult = Validation.Validate(model, ruleSets.ToArray());

            if (!entlibValidationResult.IsValid)
            {
                if (!ModelState.ContainsKey("CompanyModel.Categories.SelectedCategories"))
                {
                    this.ModelState.Add(new KeyValuePair<string, ModelState>(
                        "CompanyModel.Categories.SelectedCategories", new ModelState
                        {
                            Value = new ValueProviderResult(model.CompanyModel.Categories.SelectedCategories,
                                model.CompanyModel.Categories.SelectedCategories?.ToString(),
                                CultureInfo.CurrentCulture)
                        }));
                }
                this.ModelState.AddValidationErrors(entlibValidationResult);
                accountHelper.InitializeData(model.CompanyModel, unitOfWork);
                return View("RegisterCompany", model);
            }

            var validationResult = new ValidationResultCollection();

            AccountValidator.ValidateUserRegistration(unitOfWork, validationResult, model.UserModel);
            AccountValidator.ValidateCompanyRegistration(unitOfWork, validationResult, model.CompanyModel);
            if (validationResult.Any())
            {
                ModelState.AddValidationErrors(validationResult);
                accountHelper.InitializeData(model.CompanyModel, unitOfWork);
                return View("RegisterCompany", model);
            }

            if (AccountHelper.IsValidCaptcha())
            {
                if (model.CompanyModel.CompanyType == CompanyType.PartTime)
                {
                    accountHelper.RegisterContractor(unitOfWork, model);
                }
                else
                {
                    var userDetail = accountHelper.RegisterUser(unitOfWork, model.UserModel);
                    accountHelper.RegisterCompany(unitOfWork, model.CompanyModel, userDetail);
                }
            }

            return RedirectToAction("ChangeSettings", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("RegisterCompanySolo")]
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
        [Route("ForgotPassword")]
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
        [ValidateAntiForgeryToken]
        [EbadoMvcAuthorization(Roles = "ChangeSettings, ChangeAccountSettings, Read, Write")]
        [Route("ChangeSettings")]
        [NoClientCache]
        public ActionResult ChangeSettings(AccountSettingsModel model)
        {
            if (IsUserAuthnticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
            }

            var session = Session["User"] as SessionModel;
            ChangePasswordModel passwordModel = model.PasswordModel;
            bool changePsw = false;

            if (passwordModel.OldPassword != null && passwordModel.NewPassword != null)
            {
                var validationResult = new ValidationResultCollection();
                AccountValidator.ValidateChangeSettings(unitOfWork, validationResult, passwordModel, session.Id);
                ModelState.AddValidationErrors(validationResult);
                changePsw = true;
            }

            // TODO: entlib validation before modelState check
            //if (ModelState.IsValid)
            //{

            try
            {
                if (session.IsActive)
                {
                    model = accountHelper.UpdateUserSettings(unitOfWork, model, changePsw, session);
                }
                else
                {
                    model = accountHelper.UpdateCompanySettings(unitOfWork, model, session);
                }
            }
            catch (Exception e)
            {
                EntlibLogger.LogError("Account", "ChangeSettings-POST", e.Message, diagnosticLogConstant, e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            //}
            accountHelper.InitializeData(model.CompanyModel, unitOfWork);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("BatchAccountGallery")]
        [EbadoMvcAuthorization(Roles = "AddGallery,RemoveGallery,AddAttachments,RemoveAttachments")]
        public ActionResult BatchAccountGallery(BatchGalleryModel model)
        {
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("CreateBatch")]
        [EbadoMvcAuthorization(Roles = "AddGallery")]
        public ActionResult CreateBatch(BatchGalleryModel model)
        {
            int? companyId = GetActiveCompany();

            if (companyId == null)
            {
                return new HttpUnauthorizedResult();
            }

            var entlibValidationResult = Validation.Validate(model, "CreateBatch");

            if (!entlibValidationResult.IsValid)
            {
                var entities = fileBo.GetBatches(companyId.Value);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<BatchModel, BatchEntity>().ReverseMap();
                });

                var fileEntities = Mapper.Map<Collection<BatchModel>>(entities);
                model.Batch = fileEntities;
                model.HasError = true;

                this.ModelState.AddValidationErrors(entlibValidationResult);
                return View("BatchAccountGallery", model);
            }

            model.HasError = false;
            string batchUniqueId = fileBo.CreateBatch(model.Name, model.Description, companyId.Value);
            EntlibLogger.LogInfo("File", "Create Batch", $"Created new batch with id: {batchUniqueId}", diagnosticLogConstant);
            return RedirectToAction("EditAccountGallery", new { batchId = batchUniqueId });
        }
        #endregion

        #region Private methods

        private bool IsUserAuthnticated()
        {
            var session = (SessionModel)Session["User"];
            return session == null;
        }

        private int? GetActiveCompany()
        {
            var session = Session["User"] as SessionModel;
            return session.Companies.FirstOrDefault(c => c.IsActive)?.Id;
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

        #endregion
    }
}