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
using Web.eBado.Models.Company;
using Web.eBado.Models.Shared;
using WebAPIFactory.Caching.Core;

namespace Web.eBado.Helpers
{
    public class SharedHelper
    {
        private readonly IUnitOfWork unitOfWork;
        private const int encryptConstant = 5168;
        private const int multiplyContstant = 42;
        private readonly Uri locationBaseUri = new Uri("http://freegeoip.net/xml/");
        public SharedHelper(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
            var cache = NinjectResolver.GetInstance<ICache>();
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem(cache);
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
            var cache = NinjectResolver.GetInstance<ICache>();
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem(cache);

            var allCategories = cachedCategories.Select(category => new SelectListItem { Value = category.CategoryName, Text = category.CategoryName });

            return allCategories.AsEnumerable();
        }

        public IEnumerable<AllCategoriesModel> GetCategoriesWithSubCategories()
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem(cache);

            return cachedCategories.Select(category => new AllCategoriesModel
            {
                Category = category.CategoryName,
                SubCategories = category.SubCategories.Select(subCat => subCat.SubCategoryName)
            });
        }

        public List<CachedAllCategoriesModel> SetCategoriesCacheToListItem(List<CachedAllCategoriesModel> cachedCategories, ICache cache)
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


            cache.Insert(CacheKeys.CategoryKey, cachedCategories, null, cacheSettings);
            return cachedCategories;
        }

        public IEnumerable<CachedLocationsModel> GetCachedLocations()
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            var cachedLocations = cache.GetData<List<CachedLocationsModel>>(CacheKeys.LocationKey);

            if (cachedLocations == null)
            {
                cachedLocations = SetLocationsToCache(cachedLocations, cache);
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

        private List<CachedAllCategoriesModel> GetCachedCategoriesInListItem(ICache cache)
        {
            var cachedCategories = cache.GetData<List<CachedAllCategoriesModel>>(CacheKeys.CategoryKey);

            if (cachedCategories == null)
            {
                cachedCategories = SetCategoriesCacheToListItem(cachedCategories, cache);
            }

            return cachedCategories;
        }

      

        private List<CachedLocationsModel> SetLocationsToCache(List<CachedLocationsModel> cachedLocations, ICache cache)
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


            cache.Insert(CacheKeys.LanguageKey, cachedLocations, null, cacheSettings);
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