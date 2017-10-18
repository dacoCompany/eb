using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Web.eBado.Models.Account;

namespace Web.eBado.Helpers
{
    public class AccountHelper
    {
        #region Constants
        const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        private Uri locationBaseUri = new Uri("http://freegeoip.net/xml/");
        #endregion

        #region Public Methods
        public Countries GetCountryByIP()
        {
            string ip = HttpContext.Current.Request.UserHostAddress;
            var url = new Uri(locationBaseUri, ip);
            XmlDocument doc = new XmlDocument();
            doc.Load(url.ToString());
            XmlNodeList nodeLstCity = doc.GetElementsByTagName("CountryName");
            string countryName = nodeLstCity[0].InnerText;
            if (!string.IsNullOrEmpty(countryName))
            {
                return (Countries)System.Enum.Parse(typeof(Countries), countryName);
            }
            else
            {
                return Countries.Select;
            }
        }

        public static bool IsValidCaptcha()
        {

            var secret = ConfigurationManager.AppSettings.Get(ConfigurationKeys.ReCaptchaSecretKey);
            var req =
                (HttpWebRequest)WebRequest.Create(string.Format(ConfigurationManager.AppSettings.Get(ConfigurationKeys.ReCaptchaUri),
                    secret, HttpContext.Current.Request.Form["g-recaptcha-response"]));

            using (var wResponse = req.GetResponse())
            {

                using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                {
                    string responseFromServer = readStream.ReadToEnd();
                    if (!responseFromServer.Contains("\"success\": false"))
                        return true;
                }
            }

            return false;

        }

        public void Registration(RegistrationModel model, IUnitOfWork uow, bool createCompany = false)
        {
            var userRole = uow.UserRoleRepository.FirstOrDefault(r => r.Name == UserRole.StandardUser.ToString());
            int locationId = int.Parse(model.UserModel.PostalCode);
            var location = uow.LocationRepository.FirstOrDefault(l => l.Id == locationId);
            string salt = GenerateSalt();

            var userDetails = new UserDetailDbo
            {
                Email = model.UserModel.Email,
                Password = EncodePassword(model.UserModel.Password, salt),
                Salt = salt,
                Title = model.UserModel.Title,
                FirstName = model.UserModel.FirstName,
                Surname = model.UserModel.Surname,
                PhoneNumber = model.UserModel.PhoneNumber,
                AdditionalPhoneNumber = string.IsNullOrEmpty(model.UserModel.AdditionalPhoneNumber) ? null : model.UserModel.AdditionalPhoneNumber,
                DisplayName = $"{model.UserModel.FirstName} {model.UserModel.Surname}",
                UserRole = userRole
            };

            userDetails.Addresses.Add(new AddressDbo
            {
                Street = model.UserModel.Street,
                Number = model.UserModel.StreetNumber,
                IsBillingAddress = true,
                LocationId = location.Id
            });

            uow.UserDetailsRepository.Add(userDetails);

            if (createCompany)
            {
                CreateCompany(uow, model.CompanyModel, userDetails.Id);
            }

            uow.Commit();
        }

        public void CreateCompany(IUnitOfWork uow, CompanyModel model, int userId)
        {
            var companyTypeId = uow.CompanyTypeRepository.FirstOrDefault(ct => ct.Name == model.CompanyType.ToString()).Id;
            var companyRoleId = uow.CompanyRoleRepository.FirstOrDefault(cr => cr.Name == CompanyRole.Owner.ToString()).Id;
            int companyLocationId = int.Parse(model.CompanyPostalCode);
            var companyLocation = uow.LocationRepository.FirstOrDefault(l => l.Id == companyLocationId);
            var categoriesIds = uow.SubCategoryRepository.FindWhere(a => a.Name.Equals(model.Categories.SelectedCategories));

            var companyDetails = new CompanyDetailDbo
            {
                Name = string.IsNullOrEmpty(model.CompanyName) ? "test company" : model.CompanyName,
                PhoneNumber = model.CompanyPhoneNumber,
                AdditionalPhoneNumber = model.CompanyAdditionalPhoneNumber,
                Ico = model.CompanyIco,
                Dic = model.CompanyDic,
                CompanyTypeId = companyTypeId,
            };
            companyDetails.Addresses.Add(new AddressDbo
            {
                Street = model.CompanyStreet,
                Number = model.CompanyStreetNumber,
                IsBillingAddress = true,
                LocationId = companyLocation.Id
            });
            companyDetails.CompanyDetails2UserDetails.Add(new CompanyDetails2UserDetailsDbo
            {
                CompanyRoleId = companyRoleId,
                UserDetailsId = userId,
            });

            companyDetails.SubCategory2CompanyDetails.Add(new SubCategory2CompanyDetailsDbo
            {
            });

            uow.CompanyDetailsRepository.Add(companyDetails);
        }

        public void InitializeAllCategories(CompanyModel model)
        {
            model.Categories.AllCategories = GetAllCategories();
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

        #endregion

        #region Private Methods
        private static string GenerateSalt()
        {
            int length = 10;

            var randNum = new Random();
            var chars = new char[length];
            var allowedCharCount = allowedChars.Length;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        private IEnumerable<SelectListItem> GetAllCategories()
        {
            List<SelectListItem> allCars = new List<SelectListItem>();
            //Add a few cars to make a list of cars
            for (var i = 0; i < 50; i++)
            {
                allCars.Add(new SelectListItem { Value = $"Tag Text{i}", Text = $"tag Text{i}" });

            }

            return allCars.AsEnumerable();
        }

        #endregion
    }
}