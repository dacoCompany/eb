using Infrastructure.Common;
using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Common.Models;
using Infrastructure.Common.Models.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Web.eBado.IoC;
using Web.eBado.Models.Account;
using Web.eBado.Models.Company;
using Web.eBado.Models.Shared;
using WebAPIFactory.Caching.Core;

namespace Web.eBado.Helpers
{
    public class SharedHelper
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICache httpCache;
        private const int encryptConstant = 5168;
        private const int multiplyContstant = 42;
        private readonly Uri locationBaseUri = new Uri("http://freegeoip.net/xml/");
        private const string baseMapUrl = "https://www.google.com/maps/search/?api=1&query=";

        public SharedHelper(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            httpCache = NinjectResolver.GetInstance<ICache>();
        }

        public Countries GetUserCountry()
        {
            var countryCookie = HttpContext.Current.Request.Cookies["country"];
            if (countryCookie == null)
            {
                var currentCountry = GetCountryByIP();
                countryCookie = new HttpCookie("country", currentCountry.ToString()) { HttpOnly = true };

                HttpContext.Current.Response.AppendCookie(countryCookie);
            }
            return (Countries)Enum.Parse(typeof(Countries), countryCookie.Value);
        }


        public IEnumerable<SelectListItem> GetCategoriesToListItem()
        {
            
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem();
            var allCategories = new List<SelectListItem>();
            foreach (var category in cachedCategories)
            {
                allCategories.Add(new SelectListItem { Text = category.CategoryName, Value = category.CategoryName });
                allCategories.AddRange(category.SubCategories.Select(subCategory => new SelectListItem { Text = subCategory.SubCategoryName, Value = subCategory.SubCategoryName }));
            }
            return allCategories.AsEnumerable();
        }

        public IEnumerable<SelectListItem> GetMainCategoriesToListItem()
        {
            
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem();

            var allCategories = cachedCategories.Select(category => new SelectListItem { Value = category.CategoryName, Text = category.CategoryName });

            return allCategories.AsEnumerable();
        }

        public IEnumerable<AllCategoriesModel> GetCategoriesWithSubCategories()
        {
            
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem();

            return cachedCategories.Select(category => new AllCategoriesModel
            {
                Category = category.CategoryName,
                SubCategories = category.SubCategories.Select(subCat => subCat.SubCategoryName)
            });
        }

        public List<CachedAllCategoriesModel> SetCategoriesCacheToListItem(List<CachedAllCategoriesModel> cachedCategories)
        {
            var cacheSettings = new CacheSettings("cacheDurationKey", "cacheExpirationKey");

            cachedCategories = unitOfWork.CategoryRepository.FindAll().Select(category => new CachedAllCategoriesModel
            {
                Id = category.Id,
                CategoryName = category.Name,
                SubCategories = category.SubCategories.Select(sc => new SubCategoryModel
                {
                    Id = sc.Id,
                    SubCategoryName = sc.Name
                })
            }).ToList();


            httpCache.Insert(CacheKeys.CategoryKey, cachedCategories, null, cacheSettings);
            return cachedCategories;
        }

        public IEnumerable<CachedLocationsModel> GetCachedLocations()
        {
            
            var cachedLocations = httpCache.GetData<List<CachedLocationsModel>>(CacheKeys.LocationKey);

            if (cachedLocations == null)
            {
                cachedLocations = SetLocationsToCache(cachedLocations);
            }

            return cachedLocations;
        }

        public int GetLocationByPostalCode(string postalCode)
        {
            var locations = GetCachedLocations();

            return locations.FirstOrDefault(x => x.PostalCode.Equals(postalCode)
                   || x.PostalCode.Replace(" ", "").Equals(postalCode.Replace(" ", ""))
                   || x.City.StartsWith(postalCode, StringComparison.OrdinalIgnoreCase)
                   || x.CityAlias.StartsWith(postalCode, StringComparison.OrdinalIgnoreCase)
                   || x.DistrictAlias.StartsWith(postalCode, StringComparison.OrdinalIgnoreCase)).Id;
        }

        public int GetDefaultRadius(SessionModel session)
        {
            int defaultId = Constants.DefaultRadius;
            if (session != null)
            {
                int? userId = null;
                int? companyId = session.Companies.FirstOrDefault(c => c.IsActive)?.Id;
                userId = session.IsActive ? session.Id : userId;
                if (userId != null)
                {
                    defaultId = unitOfWork.UserDetailsRepository.FindById(userId.Value).UserSetting.SearchRadius ?? defaultId;
                }
                else
                {
                    defaultId = unitOfWork.CompanyDetailsRepository.FindById(companyId.Value).CompanySetting.SearchRadius ?? defaultId;
                }
            }
            return defaultId;
        }

        public string GetUserDisplayName(UserModel model)
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

        public CompanySearchModel GetDefaultCountry(CompanySearchModel model)
        {
            var currentCountry = GetUserCountry();
            switch (currentCountry)
            {
                case Countries.Czechia:
                    model.SearchInCZ = true;
                    break;
                case Countries.Hungary:
                    model.SearchInHU = true;
                    break;
                case Countries.Slovakia:
                    model.SearchInSK = true;
                    break;
            }
            return model;
        }

        public List<string> GetCountryShortCode(CompanySearchModel model)
        {
            var countryShortCodes = new List<string>();
            if (model.SearchInSK)
            {
                countryShortCodes.Add(Constants.SlovakiaShortCode);
            }
            else if (model.SearchInCZ)
            {
                countryShortCodes.Add(Constants.CzechiaShortCode);
            }
            else if (model.SearchInHU)
            {
                countryShortCodes.Add(Constants.HungaryShortCode);
            }
            return countryShortCodes;
        }

        public CompanyType GetCompanyType(string companyType)
        {
            return (CompanyType)System.Enum.Parse(typeof(CompanyType), companyType);
        }

        public string EncryptId(int id)
        {
            var calculatedId = (encryptConstant + id) * multiplyContstant;
            return $"E{calculatedId}C";
        }
      
        public int DecryptId(string id)
        {
            var decryptedId = Convert.ToInt32(new String(id.Where(Char.IsDigit).ToArray()));
            return (decryptedId / multiplyContstant) - encryptConstant;
        }

        public string GetFormattedLanguage(LanguageDbo language)
        {
            return $"({language.Code}) {language.Name}";
        }

        public string GenerateMapUrl(AddressDbo address)
        {
            if(address == null)
            {
                return "";
            }
            var location = address.Location;
            var city = !string.IsNullOrEmpty(location.District) ? location.District : location.City;
            return $"{baseMapUrl}{address.Street}+{address.Number},+{location.PostalCode.Replace(" ", "+")}+{city}";
        }
        private List<CachedAllCategoriesModel> GetCachedCategoriesInListItem()
        {
            var cachedCategories = httpCache.GetData<List<CachedAllCategoriesModel>>(CacheKeys.CategoryKey);

            if (cachedCategories == null)
            {
                cachedCategories = SetCategoriesCacheToListItem(cachedCategories);
            }

            return cachedCategories;
        }

      

        private List<CachedLocationsModel> SetLocationsToCache(List<CachedLocationsModel> cachedLocations)
        {
            var cacheSettings = new CacheSettings("cacheDurationKey", "cacheExpirationKey");

            cachedLocations = unitOfWork.LocationRepository.FindAll().Select(location => new CachedLocationsModel
            {
                Id = location.Id,
                City = location.City,
                CityAlias = location.CityAlias,
                Country = location.Country,
                County = location.County,
                District = location.District,
                DistrictAlias = location.DistrictAlias,
                Lat = location.Lat.Value,
                Lon = location.Lon.Value,
                PostalCode = location.PostalCode
            }).ToList();


            httpCache.Insert(CacheKeys.LanguageKey, cachedLocations, null, cacheSettings);
            return cachedLocations;
        }

        private Countries GetCountryByIP()
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

    }
}