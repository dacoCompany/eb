using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Common.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Web.eBado.IoC;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;
using Web.eBado.Validators;
using WebAPIFactory.Configuration.Core;

namespace Web.eBado.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Constants

        const string AllowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

        #endregion

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
            // GetCategories();
            return View();
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
        public ActionResult RegisterUser(RegisterUser model)
        {
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {
                var validationResult = new ValidationResultCollection();
                AccountValidator.ValidateUserRegistration(uow, validationResult, model);

                if (ModelState.IsValid)
                {
                    var userRole = uow.UserRoleRepository.FirstOrDefault(r => r.Code == UserRole.StandardUser.ToString());


                    var userDetails = new UserDetailsDbo
                    {
                        Email = model.Email,
                        Password = model.Password,
                        Salt = GenerateSalt(),
                        Title = model.Title,
                        FirstName = model.FirstName,
                        Surname = model.Surname,
                        PhoneNumber = model.PhoneNumber,
                        AdditionalPhoneNumber = model.AdditionalPhoneNumber,
                        DisplayName = string.Format("{0} {1}", model.FirstName, model.Surname),
                        UserRoleId = userRole.Id,
                    };
                    userDetails.Addresses.Add(new AddressDbo
                    {
                        Street = model.Street,
                        Number = model.StreetNumber,
                        IsBillingAddress = true,
                    });
                }
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCompany(RegisterCompany model)
        {
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {
                var validationResult = new ValidationResultCollection();
                AccountValidator.ValidateUserRegistration(uow, validationResult, model);

                if (ModelState.IsValid)
                {
                    var userRole = uow.UserRoleRepository.FirstOrDefault(r => r.Code == UserRole.StandardUser.ToString());
                    var companyType = uow.CompanyTypeRepository.FirstOrDefault(ct => ct.Code == model.CompanyType.ToString());


                    var userDetails = new UserDetailsDbo
                    {
                        Email = model.Email,
                        Password = model.Password,
                        Salt = GenerateSalt(),
                        Title = model.Title,
                        FirstName = model.FirstName,
                        Surname = model.Surname,
                        PhoneNumber = model.PhoneNumber,
                        AdditionalPhoneNumber = model.AdditionalPhoneNumber,
                        DisplayName = string.Format("{0} {1}", model.FirstName, model.Surname),
                        UserRoleId = userRole.Id,
                    };
                    userDetails.Addresses.Add(new AddressDbo
                    {
                        Street = model.Street,
                        Number = model.StreetNumber,
                        IsBillingAddress = true,
                    });

                    var companyDetails = new CompanyDetailsDbo
                    {
                        Name = model.CompanyName,
                        // PhoneNumber = model.CompanyPhoneNumber,
                        // AdditionalPhoneNumber = model.CompanyAdditionalPhoneNumber,
                        CompanyTypeId = companyType.Id,
                    };
                    companyDetails.Addresses.Add(new AddressDbo
                    {
                        Street = model.CompanyStreet,
                        Number = model.CompanyStreetNumber,
                        IsBillingAddress = true,
                    });
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
                if (!CheckIfEmailExist(model.Email, uow))
                {
                    ModelState.AddModelError("Email", "Email does not exist!");
                    return View(model);
                }
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

        [AllowAnonymous]
        public ActionResult BatchAccountGallery()
        {
            var model = new List<BatchGalleryModel>();
            return View(model);
        }

        #endregion

        #region Private methods

        private static string GenerateSalt()
        {
            int length = 10;
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var randNum = new Random();
            var chars = new char[length];
            var allowedCharCount = allowedChars.Length;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
        public static string EncodePassword(string pass, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);

            return Convert.ToBase64String(inArray);
        }

        private bool CheckIfEmailExist(string email, UnitOfWork uow)
        {
            var userEmail = uow.UserDetailsRepository.FindWhere(ua => ua.Email == email).FirstOrDefault();
            return userEmail == null ? true : false;
        }

        private void GetCategories()
        {
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            using (var uow = new UnitOfWork())
            {
                var categories = uow.CategoryRepository.FindAll();
                categoriesList.Add(new SelectListItem { Text = "---Select---", Value = "0" });
                foreach (var category in categories.ToList())
                {
                    categoriesList.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
                }
            };
            ViewData["categories"] = categoriesList;
        }

        #endregion
    }
}