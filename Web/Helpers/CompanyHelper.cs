using GeoCoordinatePortable;
using Infrastructure.Common;
using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Web.eBado.Models.Account;
using Web.eBado.Models.Company;


namespace Web.eBado.Helpers
{
    public class CompanyHelper
    {
        private const double MilesToMetersConstant = 1609.344;
        SharedHelper sharedHelper;
        public CompanyHelper(IUnitOfWork unitOfWork)
        {
            sharedHelper = new SharedHelper(unitOfWork);
        }

        public CompanySearchModel GetAllCompanies(CompanySearchModel model, IUnitOfWork unitOfWork, int? page = null)
        {
            model = sharedHelper.GetDefaultCountry(model);
            var postalCodeList = GetRelatedPostalCodes(model);

            var companyDetails = unitOfWork.CompanyDetailsRepository.FindAll()
                .WhereIf(!string.IsNullOrEmpty(model.Name), search => search.Name.Contains(model.Name))
                .WhereIf(!string.IsNullOrEmpty(model.SelectedMainCategory), search => search.Category2CompanyDetails.Select(c => c.Category.Name).Contains(model.SelectedMainCategory))
                .WhereIf(!string.IsNullOrEmpty(model.SelectedSubCategory), search => search.SubCategory2CompanyDetails.Select(sc => sc.SubCategory.Name).Contains(model.SelectedSubCategory))
                .Where(search => postalCodeList.Contains(search.Addresses.FirstOrDefault(a => a.IsBillingAddress == true).Location.PostalCode))
                .Select(company => new CompanyModel
                {
                    CompanyId = company.EncryptedId,
                    CompanyDescription = company.Description,
                    CompanyName = company.Name,
                    CompanyPostalCode = company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.PostalCode,
                    CompanyCity = !(string.IsNullOrEmpty(company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.District))
                    ? company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.District
                    : company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.City,
                    AllSelectedCategories = company.Category2CompanyDetails.Select(c => c.Category.Name)
                    .Concat(company.SubCategory2CompanyDetails.Select(s => s.SubCategory.Name)),
                    ProfileUrl = company.ProfilePictureUrl
                });

            model.CompanyModel = companyDetails.ToList().ToPagedList(model.Page ?? 1, 5);
            return model;
        }

        private List<string> GetRelatedPostalCodes(CompanySearchModel model)
        {
            List<string> postalCodeList = new List<string>();
            var cachedLocations = sharedHelper.GetCachedLocations();
            int? locationId = null;
            if (model.PostalCode != null)
            {
                locationId = sharedHelper.GetLocationByPostalCode(model.PostalCode);
            }
            var countryCodes = sharedHelper.GetCountryShortCode(model);
            var currentLocation = cachedLocations.FirstOrDefault(location => location.Id == locationId);
            var locationsBySelectedCountries = cachedLocations.Where(location => countryCodes.Contains(location.Country));
            if (currentLocation != null)
            {
                foreach (var location in locationsBySelectedCountries)
                {
                    var sCoord = new GeoCoordinate((double)currentLocation.Lat, (double)currentLocation.Lon);
                    var eCoord = new GeoCoordinate((double)location.Lat, (double)location.Lon);

                    var result = (sCoord.GetDistanceTo(eCoord)) / MilesToMetersConstant;
                    if (result < model.Radius)
                    {
                        postalCodeList.Add(location.PostalCode);
                    }
                }
            }
            else
            {
                postalCodeList.AddRange(locationsBySelectedCountries.Select(location => location.PostalCode));
            }
            return postalCodeList;
        }       
    }
}