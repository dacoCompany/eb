using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
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
        public ActionResult RegisterPartTime()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterSelfEmployed()
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
            model = new FileUploadController().Show();
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
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPartTime(RegisterPartTime model)
        {
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSelfEmployed(RegisterSelfEmployed model)
        {
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCompany(RegisterCompany model)
        {
            // GetCategories();
            if (ModelState.IsValid)
            {
                string password = GeneratePassword();
                string salt = GenerateSalt();

                using (var uow = new UnitOfWork())
                {
                    if (CheckIfEmailExist(model.Email, uow))
                    {
                        ModelState.AddModelError("Email", "Email is not unique!");
                        return View(model);
                    }

                    if (!model.SelectedSubCategory.Any())
                    {
                        ModelState.AddModelError("SubCategory", "Subcategory!");
                        return View(model);
                    }

                    var accountLocation = uow.LocationRepository.FindWhere(lr => lr.PostalCode == model.PostalCode).FirstOrDefault();

                    var addressModel = new AddressDbo
                    {
                        Street = model.Street,
                        Number = model.StreetNumber,
                        LocationId = accountLocation.Id,
                        IsBillingAddress = true
                    };

                    var accountModel = new UserAccountDbo
                    {
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Title = model.Title,
                        FirstName = model.FirstName,
                        Surname = model.Surname,
                        UniqueName = model.AccountName,
                        Ico = model.Ico,
                        Dic = model.Dic,
                        SubCatId = Convert.ToInt32(model.SelectedSubCategory),
                        Password = EncodePassword(password, salt),
                        Salt = salt,
                        UserRoleId = (int)AccountType.Company
                    };
                    accountModel.Addresses.Add(addressModel);

                    uow.Commit();
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

        #endregion

        #region Private methods

        private static string GeneratePassword()
        {
            int length = 15;
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

        private static string GenerateSalt()
        {
            int length = 10;
            const string allowedChars = "abcdefghABCDEFGH0123456789";
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
            var userEmail = uow.UserAccountRepository.FindWhere(ua => ua.Email == email).FirstOrDefault();
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