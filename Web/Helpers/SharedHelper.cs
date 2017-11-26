using Infrastructure.Common;
using Infrastructure.Common.DB;
using Infrastructure.Common.Models;
using Infrastructure.Common.Models.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.eBado.IoC;
using Web.eBado.Models.Shared;
using WebAPIFactory.Caching.Core;

namespace Web.eBado.Helpers
{
    public class SharedHelper
    {
        private readonly IUnitOfWork unitOfWork;
        public SharedHelper(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IEnumerable<SelectListItem> GetCategoriesToListItem()
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem(cache);

            var allCategories = cachedCategories.Select(category => new SelectListItem { Value = category.Name, Text = category.Name });

            return allCategories.AsEnumerable();
        }

        public IEnumerable<SelectListItem> GetMainCategoriesToListItem()
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            List<CachedAllCategoriesModel> cachedCategories = GetCachedCategoriesInListItem(cache);

            var allCategories = cachedCategories.Where(c=>c.IsMain).Select(category => new SelectListItem { Value = category.Name, Text = category.Name });

            return allCategories.AsEnumerable();
        }

        public IEnumerable<AllCategoriesModel> GetCategoriesWithSubCategories()
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            List<CachedCategoriesWithSubCategoriesModel> cachedCategories = GetCachedCategoriesWithSubCategories(cache);

            var allCategories = cachedCategories.Select(category => new AllCategoriesModel {
                Category=category.Category,
                SubCategories=category.SubCategories
            });

            return allCategories;
        }

        public List<CachedAllCategoriesModel> SetCategoriesCacheToListItem(List<CachedAllCategoriesModel> cachedCategories, ICache cache)
        {
            var cacheSettings = new CacheSettings("cacheDurationKey", "cacheExpirationKey");

            var categoryList = unitOfWork.CategoryRepository.FindAll().Select(category => new CachedAllCategoriesModel
            {
                Id = category.Id,
                Name = category.Name,
                IsMain = true
            }).ToList();

            var subCategoryList = unitOfWork.SubCategoryRepository.FindAll().Select(subCategory => new CachedAllCategoriesModel
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                IsMain = false
            }).ToList();

            cachedCategories = new List<CachedAllCategoriesModel>();
            cachedCategories.AddRange(categoryList.Concat(subCategoryList));

            cache.Insert(CacheKeys.CategoryListItemKey, cachedCategories, null, cacheSettings);
            return cachedCategories;
        }

        public List<CachedCategoriesWithSubCategoriesModel> SetCategoriesWithSubCategoriesCache(List<CachedCategoriesWithSubCategoriesModel> cachedCategories, ICache cache)
        {
            var cacheSettings = new CacheSettings("cacheDurationKey", "cacheExpirationKey");

            cachedCategories = unitOfWork.CategoryRepository.FindAll().Select(category => new CachedCategoriesWithSubCategoriesModel
            {
               Category = category.Name,
               SubCategories = category.SubCategories.Select(sc=>sc.Name)
            }).ToList();

            cache.Insert(CacheKeys.CategoryKey, cachedCategories, null, cacheSettings);
            return cachedCategories;
        }

        private List<CachedAllCategoriesModel> GetCachedCategoriesInListItem(ICache cache)
        {
            var cachedCategories = cache.GetData<List<CachedAllCategoriesModel>>(CacheKeys.CategoryListItemKey);

            if (cachedCategories == null)
            {
                cachedCategories = SetCategoriesCacheToListItem(cachedCategories, cache);
            }

            return cachedCategories;
        }

        private List<CachedCategoriesWithSubCategoriesModel> GetCachedCategoriesWithSubCategories(ICache cache)
        {
            var cachedCategories = cache.GetData<List<CachedCategoriesWithSubCategoriesModel>>(CacheKeys.CategoryKey);

            if (cachedCategories == null)
            {
                cachedCategories = SetCategoriesWithSubCategoriesCache(cachedCategories, cache);
            }

            return cachedCategories;
        }

    }
}