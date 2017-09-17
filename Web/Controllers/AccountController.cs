using Infrastructure.Common.DB;
using Infrastructure.Common.Validations;
using System.Web.Mvc;
using Web.eBado.Helpers;
using Web.eBado.IoC;
using Web.eBado.Models.Account;
using Web.eBado.Models.MvcExtensions;
using Web.eBado.Models.Shared;
using Web.eBado.Validators;
using WebAPIFactory.Configuration.Core;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;

namespace Web.eBado.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        AccountHelper accountHelper;
        public AccountController()
        {
            this.accountHelper = new AccountHelper();
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
            model.CompanyModel.CompanyLocation = accountHelper.GetCountryByID();
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
        public ActionResult AccountGallery(FilesViewModel model)
        {
            model = new FileUploadController(new Configuration(), new UnitOfWork()).Show();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult EditAccountGallery()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult BatchAccountGallery()
        {
            var model = new BatchGalleryModel();
            return View(model);
        }

        #endregion

        #region HTTP POST

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
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
        #endregion
    }
}