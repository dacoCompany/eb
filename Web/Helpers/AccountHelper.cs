using eBado.BusinessObjects;
using eBado.Entities;
using Infrastructure.Common;
using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Common.Models;
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
using Web.eBado.IoC;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;
using WebAPIFactory.Caching.Core;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;

namespace Web.eBado.Helpers
{
    public class AccountHelper
    {
        #region Constants

        const string AllowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

        private readonly IUnitOfWork unitOfWork;
        private readonly IFilesBusinessObjects fileBo;
        SharedHelper sharedHelper;

        #endregion

        #region Constructor

        public AccountHelper(IUnitOfWork unitOfWork, IFilesBusinessObjects fileBo)
        {
            this.unitOfWork = unitOfWork;
            this.fileBo = fileBo;
            sharedHelper = new SharedHelper(unitOfWork);
        }

        #endregion

        #region Public Methods

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

        public void RegisterContractor(IUnitOfWork uow, RegistrationModel model)
        {
            var userModel = model.UserModel;
            var companyModel = model.CompanyModel;
            try
            {
                var userRole = uow.UserRoleRepository.FindFirstOrDefault(r => r.Name == UserRole.User.ToString());
                string salt = GenerateSalt();
                var userDetails = new UserDetailDbo
                {
                    Email = userModel.Email,
                    Password = EncodePassword(userModel.Password, salt),
                    Salt = salt,
                    PhoneNumber = userModel.PhoneNumber,
                    DisplayName = sharedHelper.GetUserDisplayName(userModel),
                    UserRole = userRole,
                    IsValidated = true
                };

                userDetails.UserSetting = new UserSettingDbo
                {
                    SearchInCZ = true,
                    SearchInSK = true,
                    SearchInHU = true,
                    SearchRadius = 30,
                    NotifyCommentOnContribution = true,
                    NotifyCommentOnAccount = true
                };

                uow.UserDetailsRepository.Add(userDetails);

                var companyTypeId = uow.CompanyTypeRepository.FindFirstOrDefault(ct => ct.Name == companyModel.CompanyType.ToString()).Id;
                var companyRoleId = uow.CompanyRoleRepository.FindFirstOrDefault(cr => cr.Name == CompanyRole.Owner.ToString()).Id;
                var categoriesIds = uow.SubCategoryRepository.FindWhere(a => a.Name.Equals(companyModel.Categories.SelectedCategories));

                var companyDetails = new CompanyDetailDbo
                {
                    CompanyTypeId = companyTypeId,
                    Name = sharedHelper.GetUserDisplayName(userModel)
                };

                companyDetails.CompanyDetails2UserDetails.Add(new CompanyDetails2UserDetailsDbo
                {
                    CompanyRoleId = companyRoleId,
                    UserDetail = userDetails,
                    EnableNotification = true
                });

                var selectedCategories = companyModel.Categories.SelectedCategories.ToList();
                if (selectedCategories.Any())
                {
                    SetSelectedCategories(selectedCategories, companyDetails);
                }

                uow.CompanyDetailsRepository.Add(companyDetails);
                uow.Commit();

                EntlibLogger.LogInfo("Account", "Register", $"Successful registration with e-mail address: {userModel.Email}", new DiagnosticsLogging { DiagnosticsArea = "Helper", DiagnosticsCategory = "Register" });

            }
            catch (Exception ex)
            {
                EntlibLogger.LogError("Account", "Register", $"Failed registration with e-mail address: {userModel.Email}", new DiagnosticsLogging { DiagnosticsArea = "Helper", DiagnosticsCategory = "Register" }, ex);
                throw;
            }
        }

        public UserDetailDbo RegisterUser(IUnitOfWork uow, UserModel model, bool canCommit = false)
        {
            try
            {
                var userRole = uow.UserRoleRepository.FindFirstOrDefault(r => r.Name == UserRole.User.ToString());
                string salt = GenerateSalt();
                var userDetails = new UserDetailDbo
                {
                    Email = model.Email,
                    Password = EncodePassword(model.Password, salt),
                    Salt = salt,
                    PhoneNumber = model.PhoneNumber,
                    DisplayName = sharedHelper.GetUserDisplayName(model),
                    UserRole = userRole,
                    IsValidated = true
                };

                userDetails.UserSetting = new UserSettingDbo
                {
                    SearchInCZ = true,
                    SearchInSK = true,
                    SearchInHU = true,
                    SearchRadius = 30,
                    NotifyCommentOnContribution = true,
                    NotifyCommentOnAccount = true
                };

                uow.UserDetailsRepository.Add(userDetails);

                if (canCommit)
                {
                    uow.Commit();
                    userDetails.EncryptedId = sharedHelper.EncryptId(userDetails.Id, EncryptType.U);
                    uow.Commit();
                }

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
            int postalCodeId = sharedHelper.GetLocationByPostalCode(model.CompanyPostalCode);

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
                UserDetail = userDetail,
                EnableNotification = true
            });

            var selectedCategories = model.Categories.SelectedCategories.ToList();
            if (selectedCategories.Any())
            {
                SetSelectedCategories(selectedCategories, companyDetails);
            }

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
            companyDetails.EncryptedId = sharedHelper.EncryptId(companyDetails.Id, EncryptType.C);

            if (string.IsNullOrEmpty(userDetail.EncryptedId))
                userDetail.EncryptedId = sharedHelper.EncryptId(userDetail.Id, EncryptType.U);

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

            if (model.ProfilePicture != null)
            {
                //TODO: upload image to blob and save to db
                if (model.ProfilePicture.ContentLength != 0)
                {
                    FileEntity picture = null;
                    using (BinaryReader reader = new BinaryReader(model.ProfilePicture.InputStream))
                    {
                        var byteFile = reader.ReadBytes(model.ProfilePicture.ContentLength);
                        picture = new FileEntity
                        {
                            Name = new FileInfo(model.ProfilePicture.FileName).Name,
                            ContentType = model.ProfilePicture.ContentType,
                            Content = byteFile,
                            Size = byteFile.Length
                        };
                    }
                    model.UserModel.ProfileUrl = fileBo.UploadProfilePicture(picture, sharedHelper.EncryptId(session.Id, EncryptType.U), null);
                }
            }
            else
            {
                userModel.ProfileUrl = userDetails.ProfilePictureUrl;
            }
            
            if (changePsw)
            {
                userDetails.Password = EncodePassword(passwordModel.NewPassword, userDetails.Salt);
            }

            userDetails.AdditionalPhoneNumber = userModel.AdditionalPhoneNumber;
            userDetails.FirstName = userModel.FirstName;
            userDetails.PhoneNumber = userModel.PhoneNumber;
            userDetails.Surname = userModel.Surname;
            userDetails.Title = userModel.Title;

            if (userDetails.Addresses.Any())
            {
                var address = userDetails.Addresses.FirstOrDefault(ad => ad.IsBillingAddress.Value);
                address.Street = userModel.Street;
                address.Number = userModel.StreetNumber;
                int locationId = sharedHelper.GetLocationByPostalCode(userModel.PostalCode);
                address.LocationId = locationId;
            }
            var userSettings = userDetails.UserSetting;
            userSettings.SearchInCZ = searchModel.SearchInCZ;
            userSettings.SearchInHU = searchModel.SearchInHU;
            userSettings.SearchInSK = searchModel.SearchInSK;
            userSettings.SearchRadius = searchModel.SearchRadius;
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

            var companyDetails = unitOfWork.CompanyDetailsRepository.FindById(companyId);

            CompanyModel companyModel = model.CompanyModel;
            SearchSettingsModel searchModel = model.SearchModel;
            NotificationModel notificationModel = model.NotificationModel;

            if (model.ProfilePicture != null)
            {
                //TODO: upload image to blob and save to db
                if (model.ProfilePicture.ContentLength != 0)
                {
                    FileEntity picture = null;
                    using (BinaryReader reader = new BinaryReader(model.ProfilePicture.InputStream))
                    {
                        var byteFile = reader.ReadBytes(model.ProfilePicture.ContentLength);
                        picture = new FileEntity
                        {
                            Name = new FileInfo(model.ProfilePicture.FileName).Name,
                            ContentType = model.ProfilePicture.ContentType,
                            Content = byteFile,
                            Size = byteFile.Length
                        };
                    }
                    model.CompanyModel.ProfileUrl = fileBo.UploadProfilePicture(picture, null, sharedHelper.EncryptId(session.Companies.First(c => c.IsActive).Id, EncryptType.C));
                }
            }
            else
            {
                companyModel.ProfileUrl = companyDetails.ProfilePictureUrl;
            }

            

            companyDetails.AdditionalPhoneNumber = companyModel.CompanyAdditionalPhoneNumber;
            companyDetails.Email = companyModel.CompanyEmail;
            companyDetails.PhoneNumber = companyModel.CompanyPhoneNumber;
            companyDetails.Description = companyModel.CompanyDescription;

            var address = companyDetails.Addresses.First(ad => ad.IsBillingAddress.Value);
            address.Street = companyModel.CompanyStreet;
            address.Number = companyModel.CompanyStreetNumber;

            int locationId = sharedHelper.GetLocationByPostalCode(companyModel.CompanyPostalCode);
            address.LocationId = locationId;
            var companySettings = companyDetails.CompanySetting;
            companySettings.SearchInCZ = searchModel.SearchInCZ;
            companySettings.SearchInHU = searchModel.SearchInHU;
            companySettings.SearchInSK = searchModel.SearchInSK;
            companySettings.SearchRadius = searchModel.SearchRadius;
            companySettings.NotifyCommentOnAccount = notificationModel.NotifyCommentOnAccount;
            companySettings.NotifyCommentOnContribution = notificationModel.NotifyCommentOnContribution;
            companySettings.NotifyAllMember = notificationModel.NotifyAllMember;

            SetMembersNotification(notificationModel, companyDetails);

            var selectedCategories = model.CompanyModel.Categories.SelectedCategories;
            if (selectedCategories != null)
            {
                SetSelectedCategories(selectedCategories?.ToList(), companyDetails);
            }

            var selectedLanguages = model.CompanyModel.Languages.SelectedLanguages;

            if (selectedLanguages != null)
            {
                SetSelectedLanguages(unitOfWork, selectedLanguages?.ToList(), companyDetails);
                Array.Clear(selectedLanguages, 0, selectedLanguages.Length);
            }

            model.EditMembersAndRolesModel = new EditMembersAndRolesModel

            {
                AllRoles = GetAllRoles(unitOfWork, companyId),
                UserRoles = GetUserRoles(companyDetails),
                Permissions = new CompanyPermissionsModel()
            };
            model.CurrentCategories = GetCurrentCategories(companyDetails, selectedCategories?.ToList());
            if (selectedCategories != null)
            {
                Array.Clear(selectedCategories, 0, selectedCategories.Length);
            }

            unitOfWork.Commit();

            model.CurrentLanguages = GetCurrentLanguages(companyDetails, unitOfWork);
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
                PostalCode = address?.Location?.PostalCode,
                Street = address?.Street,
                StreetNumber = address?.Number,
                Surname = userDetails?.Surname,
                Title = userDetails?.Title,
                ProfileUrl = userDetails.ProfilePictureUrl
            };
            model.SearchModel = new SearchSettingsModel
            {
                SearchInCZ = userSettings.SearchInCZ,
                SearchInHU = userSettings.SearchInHU,
                SearchInSK = userSettings.SearchInSK,
                SearchRadius = userSettings.SearchRadius.Value
            };
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
                CompanyDescription = companyDetails.Description,
                ProfileUrl = companyDetails.ProfilePictureUrl
            };

            model.EditMembersAndRolesModel = new EditMembersAndRolesModel

            {
                AllRoles = GetAllRoles(uow, companyId),
                UserRoles = GetUserRoles(companyDetails),
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
            model.CurrentCategories = GetCurrentCategories(companyDetails);
            model.CurrentLanguages = GetCurrentLanguages(companyDetails, uow);

            return model;
        }

        public void InitializeData(CompanyModel model, IUnitOfWork unitOfWork)
        {
            model.Categories.AllCategories = sharedHelper.GetCategoriesToListItem();
            model.Languages.AllLanguages = GetAllLanguages(unitOfWork);
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

        private IEnumerable<SelectListItem> GetAllLanguages(IUnitOfWork unitOfWork)
        {
            List<SelectListItem> allLanguages = new List<SelectListItem>();
            var cache = NinjectResolver.GetInstance<ICache>();
            var cachedLanguages = cache.GetData<List<CachedLanguagesModel>>(CacheKeys.LanguageKey);

            if (cachedLanguages == null)
            {
                cachedLanguages = SetLanguagesCache(cachedLanguages, cache, unitOfWork);
            }

            var languages = cachedLanguages.Select(language => new SelectListItem { Value = language.Code, Text = $"({language.Code}) {language.LanguageName}" });

            allLanguages.AddRange(languages);
            return allLanguages.AsEnumerable();
        }

        private IEnumerable<SelectListItem> GetAllRoles(IUnitOfWork uow, int companyId)
        {
            List<SelectListItem> allRoles = new List<SelectListItem>();
            var companyRoles = uow.CompanyRoleRepository
                .FindWhere(cr => cr.Name != CompanyRole.Owner.ToString() || cr.CreatedByCompId == companyId)
                .Select(x => new SelectListItem
                {
                    Value = x.Name,
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

        private static void SetMembersNotification(NotificationModel notificationModel, CompanyDetailDbo companyDetails)
        {
            if (notificationModel.NotifyAllMember)
            {
                foreach (var user2company in companyDetails.CompanyDetails2UserDetails.Where(cd => !cd.EnableNotification))
                {
                    user2company.EnableNotification = true;
                    companyDetails.CompanyDetails2UserDetails.Add(user2company);
                }
            }
            else
            {
                foreach (var user2company in companyDetails.CompanyDetails2UserDetails.Where(cd => cd.CompanyRole.Name != CompanyRole.Owner.ToString()))
                {
                    user2company.EnableNotification = false;
                    companyDetails.CompanyDetails2UserDetails.Add(user2company);
                }
            }
        }

        private List<UserRoleModel> GetUserRoles(CompanyDetailDbo companyDetails)
        {
            return companyDetails.CompanyDetails2UserDetails
            .Where(cd => cd.CompanyRole.Name != CompanyRole.Owner.ToString() && cd.IsActive)
            .Select(cd => new UserRoleModel
            {
                UserEmail = cd.UserDetail.Email,
                SelectedRoleId = cd.CompanyRole.Name
            }).ToList();
        }

        private IEnumerable<string> GetCurrentCategories(CompanyDetailDbo companyDetails, List<string> selectedCategories = null)
        {
            var allCategories = new List<string>();
            var categories = companyDetails.Category2CompanyDetails.Where(c => c.IsActive).Select(c => c.Category.Name);
            var subCategories = companyDetails.SubCategory2CompanyDetails.Where(c => c.IsActive).Select(c => c.SubCategory.Name);
            allCategories.AddRange(categories.Concat(subCategories));
            if (selectedCategories != null)
            {
                allCategories.AddRange(selectedCategories);
            }
            return allCategories;
        }

        private IEnumerable<CurrentLanguagesModel> GetCurrentLanguages(CompanyDetailDbo companyDetails, IUnitOfWork unitOfWork)
        {
            return unitOfWork.CompanyDetails2LanguagesRepository.FindWhere(c => c.CompanyDetailsId == companyDetails.Id).Select(c => new CurrentLanguagesModel
            {
                Code = c.Language.Code,
                LanguageName = c.Language.Name
            });
        }


        private void SetSelectedCategories(List<string> selectedCategories, CompanyDetailDbo companyDetails)
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            var cachedCategories = cache.GetData<List<CachedAllCategoriesModel>>(CacheKeys.CategoryKey);

            if (cachedCategories == null)
            {
                cachedCategories = sharedHelper.SetCategoriesCacheToListItem(cachedCategories);
            }

            var categoryIds = cachedCategories.Where(c => selectedCategories.Contains(c.CategoryName)).Select(c => c.Id);
            var subCategoryIds = cachedCategories.Where(c => selectedCategories.Contains(c.SubCategories.SelectMany(s => s.SubCategoryName))).Select(c => c.Id);
            var currentCategoriesId = companyDetails.Category2CompanyDetails.Where(c => c.IsActive).Select(c => c.CategoryId);
            var currentSubCategoriesId = companyDetails.SubCategory2CompanyDetails.Where(c => c.IsActive).Select(c => c.SubCategoryId);

            foreach (var categoryId in categoryIds)
            {
                if (!currentCategoriesId.Contains(categoryId))
                {
                    var category2company = new Category2CompanyDetailsDbo
                    {
                        CategoryId = categoryId,
                        CompanyDetailsId = companyDetails.Id
                    };
                    companyDetails.Category2CompanyDetails.Add(category2company);
                }
            }

            foreach (var subCategoryId in subCategoryIds)
            {
                if (!currentSubCategoriesId.Contains(subCategoryId))
                {
                    var subCategory2company = new SubCategory2CompanyDetailsDbo
                    {
                        SubCategoryId = subCategoryId,
                        CompanyDetailsId = companyDetails.Id
                    };
                    companyDetails.SubCategory2CompanyDetails.Add(subCategory2company);
                }
            }
        }

        private void SetSelectedLanguages(IUnitOfWork uow, List<string> selecteLanguages, CompanyDetailDbo companyDetails)
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            var cachedLanguages = cache.GetData<List<CachedLanguagesModel>>(CacheKeys.LanguageKey);

            if (cachedLanguages == null)
            {
                cachedLanguages = SetLanguagesCache(cachedLanguages, cache, uow);
            }

            var alLanguagesIds = cachedLanguages.Where(c => selecteLanguages.Contains(c.Code)).Select(c => c.Id);
            var currentLanguagesId = companyDetails.CompanyDetails2Languages.Where(c => c.IsActive).Select(c => c.LanguageId);

            foreach (var languageId in alLanguagesIds)
            {
                if (!currentLanguagesId.Contains(languageId))
                {
                    var company2language = new CompanyDetails2LanguagesDbo
                    {
                        LanguageId = languageId,
                        CompanyDetailsId = companyDetails.Id
                    };
                    companyDetails.CompanyDetails2Languages.Add(company2language);
                }
            }
        }

        private List<CachedLanguagesModel> SetLanguagesCache(List<CachedLanguagesModel> cachedLanguages, ICache cache, IUnitOfWork unitOfWork)
        {
            var cacheSettings = new CacheSettings("cacheDurationKey", "cacheExpirationKey");
            cachedLanguages = new List<CachedLanguagesModel>();

            var languageList = unitOfWork.LanguageRepository.FindAll().Select(language => new CachedLanguagesModel
            {
                Id = language.Id,
                Code = language.Code,
                LanguageName = language.Name
            }).ToList();

            cachedLanguages.AddRange(languageList);

            cache.Insert(CacheKeys.LanguageKey, cachedLanguages, null, cacheSettings);
            return cachedLanguages;
        }
        #endregion
    }
}
