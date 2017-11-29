using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Web.eBado.Models.Account;
using Web.eBado.Models.Company;
using Web.eBado.Models.Shared;

namespace Web.eBado.Helpers
{
    public class CompanyHelper
    {
        SharedHelper sharedHelper;
        public CompanyHelper(IUnitOfWork unitOfWork)
        {
            sharedHelper = new SharedHelper(unitOfWork);
        }
        public string GetCategoryBySelectedItem(string selectedItem)
        {
            if (selectedItem == null)
            {
                return null;
            }
            var allCategories = sharedHelper.GetCategoriesWithSubCategories();
            var category = allCategories.FirstOrDefault(ac => ac.Category.Contains(selectedItem));
            if (category != null)
            {
                return selectedItem;
            }
            else
            {
                category = allCategories.FirstOrDefault(ac => ac.SubCategories.Contains(selectedItem));
                return category.Category;
            }
        }

        public CompanySearchModel GetAllCompanies(CompanySearchModel model ,IUnitOfWork unitOfWork)
        {
            var companyDetailsDbo = unitOfWork.CompanyDetailsRepository.FindAll()
                .Include(cd => cd.Category2CompanyDetails)
                .Include(cd => cd.SubCategory2CompanyDetails);

            foreach (var company in companyDetailsDbo)
            {
                var companyLocation = company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location;
                var allCategories = company.Category2CompanyDetails.Select(c => c.Category.Name);
                var allSubCategories = company.SubCategory2CompanyDetails.Select(s => s.SubCategory.Name);
                var companyDetail = new CompanyModel
                {
                    CompanyId = sharedHelper.EncryptId(company.Id),
                    CompanyDescription = company.Description,
                    CompanyName = company.Name,
                    CompanyPostalCode = companyLocation?.PostalCode,
                    CompanyCity = companyLocation?.District,
                    AllSelectedCategories = allCategories.Concat(allSubCategories),
                    ProfileUrl = company.ProfilePictureUrl
                };
                model.CompanyModel.Add(companyDetail);
            }
           model.SearchParameters = GetDefaultCountry(model);
            return model;
        }

        private SearchParametersModel GetDefaultCountry(CompanySearchModel model)
        {
            var currentCountry = sharedHelper.GetUserCountry();
            var searchParameters = model.SearchParameters;
            switch (currentCountry)
            {
                case Countries.Czechia:
                    searchParameters.SearchInCZ = true;
                    break;
                case Countries.Hungary:
                    searchParameters.SearchInHU = true;
                    break;
                case Countries.Slovakia:
                    searchParameters.SearchInSK = true;
                    break;
            }
            return searchParameters;
        }
    }
}