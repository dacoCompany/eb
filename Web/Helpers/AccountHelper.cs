using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Common.Validations;
using Infrastructure.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;
using Web.eBado.Validators;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;

namespace Web.eBado.Helpers
{
    public class AccountHelper
    {
        #region Constants

        const string AllowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Uri locationBaseUri = new Uri("http://freegeoip.net/xml/");

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
                return (Countries)Enum.Parse(typeof(Countries), countryName);
            }

            return Countries.Select;
        }

        public static bool IsValidCaptcha()
        {
            var secret = ConfigurationManager.AppSettings.Get(ConfigurationKeys.ReCaptchaSecretKey);
            var req = (HttpWebRequest)WebRequest.Create(string.Format(ConfigurationManager.AppSettings.Get(ConfigurationKeys.ReCaptchaUri),
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

        public UserDetailDbo RegisterUser(IUnitOfWork uow, UserModel model, bool canCommit = false)
        {
            try
            {
                var userRole = uow.UserRoleRepository.FindFirstOrDefault(r => r.Name == UserRole.User.ToString());
                string salt = GenerateSalt();
                int postalCodeId = GetLocation(model.PostalCode, uow);
                var userDetails = new UserDetailDbo
                {
                    Email = model.Email,
                    Password = EncodePassword(model.Password, salt),
                    Salt = salt,
                    Title = model.Title,
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    PhoneNumber = model.PhoneNumber,
                    AdditionalPhoneNumber = string.IsNullOrEmpty(model.AdditionalPhoneNumber) ? null : model.AdditionalPhoneNumber,
                    DisplayName = GetUserDisplayName(model),
                    UserRole = userRole
                };

                userDetails.Addresses.Add(new AddressDbo
                {
                    Street = model.Street,
                    Number = model.StreetNumber,
                    IsBillingAddress = true,
                    LocationId = postalCodeId
                });

                userDetails.UserSetting = new UserSettingDbo
                {
                    Language = Thread.CurrentThread.CurrentCulture.Name,
                    SearchInCZ = true,
                    SearchInSK = true,
                    SearchInHU = true,
                    SearchRadius = 30,
                    NotifyCommentOnContribution = true,
                    NotifyCommentOnAccount = true
                };

                uow.UserDetailsRepository.Add(userDetails);

                if (canCommit)
                    uow.Commit();

                EntlibLogger.LogInfo("Account", "Register", $"Successful registration with e-mail address: {model.Email}", new DiagnosticsLogging { DiagnosticsArea = "Helper", DiagnosticsCategory = "Register" });

                return userDetails;
            }
            catch (Exception ex)
            {
                EntlibLogger.LogError("Account", "Register", $"Failed registration with e-mail address: {model.Email}", new DiagnosticsLogging { DiagnosticsArea = "Helper", DiagnosticsCategory = "Register" }, ex);
                throw;
            }
        }

        public void RegisterCompany(IUnitOfWork uow, CompanyModel model, UserDetailDbo userDetail)
        {
            var companyTypeId = uow.CompanyTypeRepository.FindFirstOrDefault(ct => ct.Name == model.CompanyType.ToString()).Id;
            var companyRoleId = uow.CompanyRoleRepository.FindFirstOrDefault(cr => cr.Name == CompanyRole.Owner.ToString()).Id;
            var categoriesIds = uow.SubCategoryRepository.FindWhere(a => a.Name.Equals(model.Categories.SelectedCategories));
            int postalCodeId = GetLocation(model.CompanyPostalCode, uow);

            var companyDetails = new CompanyDetailDbo
            {
                Name = string.IsNullOrEmpty(model.CompanyName) ? "test company" : model.CompanyName,
                PhoneNumber = model.CompanyPhoneNumber,
                AdditionalPhoneNumber = model.CompanyAdditionalPhoneNumber,
                Ico = model.CompanyIco,
                Dic = model.CompanyDic,
                CompanyTypeId = companyTypeId,
                Email = model.CompanyEmail
            };
            companyDetails.Addresses.Add(new AddressDbo
            {
                Street = model.CompanyStreet,
                Number = model.CompanyStreetNumber,
                IsBillingAddress = true,
                LocationId = postalCodeId
            });
            companyDetails.CompanyDetails2UserDetails.Add(new CompanyDetails2UserDetailsDbo
            {
                CompanyRoleId = companyRoleId,
                UserDetail = userDetail
            });

            companyDetails.SubCategory2CompanyDetails.Add(new SubCategory2CompanyDetailsDbo
            {
            });

            companyDetails.CompanySetting = new CompanySettingDbo
            {
                NotifyCommentOnAccount = true,
                NotifyCommentOnContribution = true,
                NotifyAllMember = true,
                SearchInHU = true,
                SearchInSK = true,
                SearchInCZ = true,
                SearchRadius = 100
            };

            uow.CompanyDetailsRepository.Add(companyDetails);

            uow.Commit();
        }

        public AccountSettingsModel UpdateUserSettings(IUnitOfWork unitOfWork, AccountSettingsModel model, bool changePsw, SessionModel session)
        {
            if (model.UserModel.Email == null)
            {
                model = GetUserSettings(unitOfWork, session);
            }

            UserModel userModel = model.UserModel;
            SearchSettingsModel searchModel = model.SearchModel;
            NotificationModel notificationModel = model.NotificationModel;
            ChangePasswordModel passwordModel = model.PasswordModel;

            var userDetails = unitOfWork.UserDetailsRepository.FindById(session.Id);
            if (changePsw)
            {
                userDetails.Password = EncodePassword(passwordModel.NewPassword, userDetails.Salt);
            }

            userDetails.AdditionalPhoneNumber = userModel.AdditionalPhoneNumber;
            userDetails.FirstName = userModel.FirstName;
            userDetails.PhoneNumber = userModel.PhoneNumber;
            userDetails.Surname = userModel.Surname;
            userDetails.Title = userModel.Title;

            var address = userDetails.Addresses.FirstOrDefault(ad => ad.IsBillingAddress.Value);
            address.Street = userModel.Street;
            address.Number = userModel.StreetNumber;
            int locationId = GetLocation(userModel.PostalCode, unitOfWork);
            address.LocationId = locationId;

            var userSettings = userDetails.UserSetting;
            userSettings.SearchInCZ = searchModel.SearchInCZ;
            userSettings.SearchInHU = searchModel.SearchInHU;
            userSettings.SearchInSK = searchModel.SearchInSK;
            userSettings.SearchRadius = searchModel.SearchRadius;
            userSettings.Language = model.SelectedLanguage;
            userSettings.NotifyCommentOnAccount = notificationModel.NotifyCommentOnAccount;
            userSettings.NotifyCommentOnContribution = notificationModel.NotifyCommentOnContribution;

            unitOfWork.Commit();
            return (model);
        }

        public AccountSettingsModel UpdateCompanySettings(IUnitOfWork unitOfWork, AccountSettingsModel model, SessionModel session)
        {
            var companyId = session.Companies.FirstOrDefault(c => c.IsActive).Id;

            if (model.CompanyModel.CompanyName == null)
            {
                model = GetCompanySettings(unitOfWork, session);
            }

            CompanyModel companyModel = model.CompanyModel;
            SearchSettingsModel searchModel = model.SearchModel;
            NotificationModel notificationModel = model.NotificationModel;

            var companyDetails = unitOfWork.CompanyDetailsRepository.FindById(companyId);

            companyDetails.AdditionalPhoneNumber = companyModel.CompanyAdditionalPhoneNumber;
            companyDetails.Email = companyModel.CompanyEmail;
            companyDetails.PhoneNumber = companyModel.CompanyPhoneNumber;

            var address = companyDetails.Addresses.First(ad => ad.IsBillingAddress.Value);
            address.Street = companyModel.CompanyStreet;
            address.Number = companyModel.CompanyStreetNumber;

            int locationId = GetLocation(companyModel.CompanyPostalCode, unitOfWork);
            address.LocationId = locationId;
            var companySettings = companyDetails.CompanySetting;
            companySettings.SearchInCZ = searchModel.SearchInCZ;
            companySettings.SearchInHU = searchModel.SearchInHU;
            companySettings.SearchInSK = searchModel.SearchInSK;
            companySettings.SearchRadius = searchModel.SearchRadius;
            companySettings.Language = model.SelectedLanguage;
            companySettings.NotifyCommentOnAccount = notificationModel.NotifyCommentOnAccount;
            companySettings.NotifyCommentOnContribution = notificationModel.NotifyCommentOnContribution;
            companySettings.NotifyAllMember = notificationModel.NotifyAllMember;

            var memberRoles = companyDetails.CompanyDetails2UserDetails
                .Where(cd => cd.CompanyRole.Name != CompanyRole.Owner.ToString() && cd.IsActive)
                .Select(cd => new UserRoleModel
                {
                    UserEmail = cd.UserDetail.Email,
                    SelectedRoleId = cd.CompanyRoleId.ToString()
                }).ToList();

            model.EditMembersAndRolesModel = new EditMembersAndRolesModel

            {
                AllRoles = GetAllRoles(unitOfWork, companyId),
                UserRoles = memberRoles,
                Permissions = new CompanyPermissionsModel()
            };

            unitOfWork.Commit();
            return (model);
        }

        public AccountSettingsModel GetUserSettings(IUnitOfWork uow, SessionModel session)
        {
            AccountSettingsModel model = new AccountSettingsModel();

            int userId = session.Id;
            UserDetailDbo userDetails = uow.UserDetailsRepository.FindFirstOrDefault(ud => ud.Id == userId);
            AddressDbo address = userDetails.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value);
            UserSettingDbo userSettings = userDetails.UserSetting;
            model.UserModel = new UserModel
            {
                AdditionalPhoneNumber = userDetails.AdditionalPhoneNumber,
                Email = userDetails.Email,
                FirstName = userDetails.FirstName,
                PhoneNumber = userDetails.PhoneNumber,
                PostalCode = address.Location.PostalCode,
                Street = address.Street,
                StreetNumber = address.Number,
                Surname = userDetails.Surname,
                Title = userDetails.Title
            };
            model.SearchModel = new SearchSettingsModel
            {
                SearchInCZ = userSettings.SearchInCZ,
                SearchInHU = userSettings.SearchInHU,
                SearchInSK = userSettings.SearchInSK,
                SearchRadius = userSettings.SearchRadius.Value
            };
            model.SelectedLanguage = userSettings.Language;
            model.EditMembersAndRolesModel.AllRoles = GetDefaultRoles();

            return model;
        }
        public AccountSettingsModel GetCompanySettings(IUnitOfWork uow, SessionModel session)
        {
            AccountSettingsModel model = new AccountSettingsModel();
            int companyId = session.Companies.FirstOrDefault(c => c.IsActive).Id;
            CompanyDetailDbo companyDetails = uow.CompanyDetailsRepository.FindFirstOrDefault(cd => cd.Id == companyId);
            AddressDbo address = companyDetails.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value);
            CompanySettingDbo companySettings = companyDetails.CompanySetting;
            model.CompanyModel = new CompanyModel
            {
                CompanyAdditionalPhoneNumber = companyDetails.AdditionalPhoneNumber,
                CompanyDic = companyDetails.Dic,
                CompanyEmail = companyDetails.Email,
                CompanyIco = companyDetails.Ico,
                CompanyName = companyDetails.Name,
                CompanyPhoneNumber = companyDetails.PhoneNumber,
                CompanyStreet = address.Street,
                CompanyStreetNumber = address.Number,
                CompanyPostalCode = address.Location.PostalCode,
            };

            model.CurrentCategories = companyDetails.Category2CompanyDetails.Select(c => c.Category.Name);
            var memberRoles = companyDetails.CompanyDetails2UserDetails
                .Where(cd => cd.CompanyRole.Name != CompanyRole.Owner.ToString() && cd.IsActive)
                .Select(cd => new UserRoleModel
                {
                    UserEmail = cd.UserDetail.Email,
                    SelectedRoleId = cd.CompanyRoleId.ToString()
                }).ToList();

            model.EditMembersAndRolesModel = new EditMembersAndRolesModel

            {
                AllRoles = GetAllRoles(uow, companyId),
                UserRoles = memberRoles,
                Permissions = new CompanyPermissionsModel()
            };
            model.NotificationModel = new NotificationModel
            {
                NotifyCommentOnAccount = companySettings.NotifyCommentOnAccount,
                NotifyCommentOnContribution = companySettings.NotifyCommentOnContribution,
                NotifyAllMember = companySettings.NotifyAllMember
            };
            model.SearchModel = new SearchSettingsModel
            {
                SearchInCZ = companySettings.SearchInCZ,
                SearchInHU = companySettings.SearchInHU,
                SearchInSK = companySettings.SearchInSK,
                SearchRadius = companySettings.SearchRadius.Value
            };
            model.SelectedLanguage = companySettings.Language;

            return model;
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
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = AllowedChars[Convert.ToInt32((AllowedChars.Length) * randNum.NextDouble())];
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

        private string GetUserDisplayName(UserModel model)
        {
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.Surname))
            {
                int index = model.Email.LastIndexOf("@");

                if (index > 0)
                {
                    return model.Email.Substring(0, index);
                }

                throw new Exception("Invalid email format.");
            }
            else
            {
                return $"{model.FirstName} {model.Surname}";
            }
        }

        private int GetLocation(string postalCode, IUnitOfWork uow)
        {
            return uow.LocationRepository.FindFirstOrDefault(x => x.PostalCode.Equals(postalCode)
                   || x.PostalCode.Replace(" ", "").Equals(postalCode.Replace(" ", ""))
                   || x.City.Contains(postalCode)).Id;
        }

        private IEnumerable<SelectListItem> GetAllRoles(IUnitOfWork uow, int companyId)
        {
            List<SelectListItem> allRoles = new List<SelectListItem>();
            var companyRoles = uow.CompanyRoleRepository
                .FindWhere(cr => cr.Name != CompanyRole.Owner.ToString() || cr.CreatedByCompId == companyId)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
            allRoles.AddRange(companyRoles);

            return allRoles;
        }

        private IEnumerable<SelectListItem> GetDefaultRoles()
        {
            List<SelectListItem> roles = new List<SelectListItem>();

            roles.Add(new SelectListItem { Value = "", Text = "" });
            return roles;
        }
        #endregion
    }
}
