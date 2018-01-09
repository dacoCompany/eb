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
using Web.eBado.Models.Shared;

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
                    AllSelectedCategories = company.Category2CompanyDetails.Where(c => c.IsActive).Select(c => c.Category.Name)
                    .Concat(company.SubCategory2CompanyDetails.Where(s => s.IsActive).Select(s => s.SubCategory.Name)),
                    ProfileUrl = company.ProfilePictureUrl
                });

            model.CompanyModel = companyDetails.ToList().ToPagedList(model.Page ?? 1, 5);
            return model;
        }

        public CompanyDetailModel GetCompanyDetail(CompanyDetailModel model, IUnitOfWork unitOfWork, int companyId, SessionModel session)
        {
            var company = unitOfWork.CompanyDetailsRepository.FindById(companyId);
            var companyAddress = company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value);
            var companyLocation = companyAddress.Location;
            int imageCount = 0;
            int videoCount = 0;
            model.CompanyModel = new CompanyModel
            {
                CompanyCity = string.IsNullOrEmpty(companyLocation.District) ? companyLocation.City : companyLocation.District,
                CompanyAdditionalPhoneNumber = company.AdditionalPhoneNumber,
                CompanyDescription = company.Description,
                CompanyDic = company.Dic,
                CompanyEmail = company.Email,
                CompanyIco = company.Ico,
                CompanyName = company.Name,
                CompanyPhoneNumber = company.PhoneNumber,
                CompanyPostalCode = companyLocation.PostalCode,
                CompanyStreet = companyAddress.Street,
                CompanyStreetNumber = companyAddress.Number,
                CompanyType = sharedHelper.GetCompanyType(company.CompanyType.Name),
                ProfileUrl = company.ProfilePictureUrl
            };

            var batches = company.BatchAttachments.Where(ba => ba.IsActive);
            foreach (var batch in batches)
            {
                var batchModel = new AllCompanyAttachmentsModel
                {
                    BatchName = batch.Name,
                    BatchDescription = batch.Description,
                };
                foreach (var attachment in batch.Attachments.Where(a => a.IsActive))
                {
                    var attachmentModel = new AttachmentModel
                    {
                        AttachmentType = attachment.FileType,
                        Name = attachment.Name,
                        ThumbnailUrl = attachment.ThumbnailUrl,
                        Url = attachment.OriginalUrl
                    };
                    if (attachment.FileType == "video")
                    {
                        videoCount++;
                    }
                    else
                    {
                        imageCount++;
                    }
                    batchModel.Attachment.Add(attachmentModel);
                }
                model.Attachments.Add(batchModel);
            }
            model.ImagesCount = imageCount;
            model.VideosCount = videoCount;
            model.Languages = company.CompanyDetails2Languages.Where(cd => cd.IsActive).Select(cd => sharedHelper.GetFormattedLanguage(cd.Language)).ToList();
            model.Categories = company.Category2CompanyDetails.Where(c => c.IsActive).Select(c => c.Category.Name)
                    .Concat(company.SubCategory2CompanyDetails.Where(s => s.IsActive).Select(s => s.SubCategory.Name)).ToList();
            if (session != null)
            {
                model.CustomerEmail = session.Email;
            }
            model.MapUrl = sharedHelper.GenerateMapUrl(company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value));

            return model;
        }

        public CompanySearchModel InitializeCompanyData(SessionModel session, CompanySearchModel model)
        {
            model.DefaultRadius = sharedHelper.GetDefaultRadius(session);
            model.AllMainCategories = sharedHelper.GetMainCategoriesToListItem();
            model.AllCategories = sharedHelper.GetCategoriesWithSubCategories();
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