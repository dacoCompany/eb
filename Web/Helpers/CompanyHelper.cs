using GeoCoordinatePortable;
using Infrastructure.Common;
using Infrastructure.Common.DB;
using Infrastructure.Common.Models;
using PagedList;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Web.eBado.Models.Account;
using Web.eBado.Models.Company;
using Web.eBado.Models.Shared;
using WebAPIFactory.Caching.Core;

namespace Web.eBado.Helpers
{
    public class CompanyHelper
    {
        private const double MilesToMetersConstant = 1609.344;
        SharedHelper sharedHelper;
        public CompanyHelper(IUnitOfWork unitOfWork, ICache httpCache)
        {
            sharedHelper = new SharedHelper(unitOfWork, httpCache);
        }

        public CompanySearchModel GetAllCompanies(CompanySearchModel model, IUnitOfWork unitOfWork, SessionModel session, int? page = null)
        {
            var timer = Stopwatch.StartNew();

            var postalCodeList = GetRelatedPostalCodes(model);

            timer.Stop();
            Debug.WriteLine(timer.ElapsedMilliseconds);

            timer.Restart();
            var companyDetails = unitOfWork.CompanyDetailsRepository.FindAll()
                .WhereIf(!string.IsNullOrEmpty(model.Name), search => search.Name.Contains(model.Name))
                .WhereIf(!string.IsNullOrEmpty(model.SelectedMainCategory), search => search.Category2CompanyDetails.Select(c => c.Category.Name).Contains(model.SelectedMainCategory))
                .WhereIf(!string.IsNullOrEmpty(model.SelectedSubCategory), search => search.SubCategory2CompanyDetails.Select(sc => sc.SubCategory.Name).Contains(model.SelectedSubCategory))
                .WhereIf(postalCodeList.Any(), search => postalCodeList.Intersect(search.Addresses.Select(a => a.PostalCode)).Any())
                .Select(company => new CompanyModel
                {
                    Id = company.Id,
                    CompanyId = company.EncryptedId,
                    CompanyDescription = company.Description,
                    CompanyName = company.Name,
                    CompanyPostalCode = company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.PostalCode,
                    CompanyCity = !(string.IsNullOrEmpty(company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.District))
                    ? company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.District
                    : company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value).Location.City,
                    //AllSelectedCategories = company.Category2CompanyDetails.Where(c => c.IsActive).Select(c => c.Category.Name)
                    //.Concat(company.SubCategory2CompanyDetails.Where(s => s.IsActive).Select(s => s.SubCategory.Name)),
                    ProfileUrl = company.ProfilePictureUrl,
                    DateRegistered = company.DateCreated.Value
                });

            model.CompanyModel = companyDetails.OrderBy(cd => cd.Id).ToPagedList(model.Page ?? 1, 10);
            timer.Stop();
            Debug.WriteLine(timer.ElapsedMilliseconds);
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
                CompanyIco = company.Ico.Value,
                CompanyName = company.Name,
                CompanyPhoneNumber = company.PhoneNumber,
                CompanyPostalCode = companyLocation.PostalCode,
                CompanyStreet = companyAddress.Street,
                CompanyStreetNumber = companyAddress.Number,
                CompanyType = sharedHelper.GetCompanyType(company.CompanyType.Name),
                ProfileUrl = company.ProfilePictureUrl,
                DateRegistered = company.DateCreated.Value
            };

            var batches = company.BatchAttachments.WhereActive();
            foreach (var batch in batches)
            {
                var batchModel = new AllCompanyAttachmentsModel
                {
                    BatchName = batch.Name,
                    BatchDescription = batch.Description,
                };

                foreach (var attachment in batch.Attachments.WhereActive())
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
            model.Languages = company.CompanyDetails2Languages.WhereActive().Select(cd => sharedHelper.GetFormattedLanguage(cd.Language)).ToList();
            //model.Categories = company.Category2CompanyDetails.WhereActive().Select(c => c.Category.Name)
            //        .Concat(company.SubCategory2CompanyDetails.WhereActive().Select(s => s.SubCategory.Name)).ToList();
            if (session != null)
            {
                model.CustomerEmail = session.Email;
            }
            model.MapUrl = sharedHelper.GenerateMapUrl(company.Addresses.FirstOrDefault(a => a.IsBillingAddress.Value));

            return model;
        }

        public CompanySearchModel InitializeCompanyData(SessionModel session, CompanySearchModel model)
        {
            model.DefaultRadius = model.Radius != 0 ? model.Radius : sharedHelper.GetDefaultRadius(session);
            model.AllMainCategories = sharedHelper.GetMainCategoriesToListItem();
            model.AllCategories = sharedHelper.GetCategoriesWithSubCategories();

            return model;
        }

        public CompanySearchModel InitializeAccountSettings(SessionModel session, CompanySearchModel model, IUnitOfWork unitOfWork)
        {
            if (session != null)
            {
                var companySesion = session.Companies.FirstOrDefault(c => c.IsActive);
                if (session.IsActive)
                {
                    model = sharedHelper.GetUserSettings(unitOfWork, model, session.Id);
                }
                else if (companySesion != null)
                {
                    model = sharedHelper.GetCompanySettings(unitOfWork, model, companySesion.Id);
                }
            }
            else
            {
                model = sharedHelper.GetDefaultCountry(model);
            }

            return model;
        }

        private List<string> GetRelatedPostalCodes(CompanySearchModel model)
        {
            List<string> postalCodeList = new List<string>();
            var cachedLocations = sharedHelper.GetCachedLocations();
            var relatedLocations = GetRelatedLocations(model, cachedLocations);
            var locationDbo = sharedHelper.GetLocationByPostalCode(model.PostalCode, relatedLocations);
            var countryCodes = sharedHelper.GetCountryShortCode(model);
            var locationsBySelectedCountries = relatedLocations.Where(location => countryCodes.Contains(location.Country));
            if (locationDbo != null)
            {
                foreach (var location in locationsBySelectedCountries)
                {
                    var sCoord = new GeoCoordinate((double)locationDbo.Lat, (double)locationDbo.Lon);
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

        private IEnumerable<CachedLocationsModel> GetRelatedLocations(CompanySearchModel model, IEnumerable<CachedLocationsModel> cachedLocations)
        {
            var relatedLocations = new Collection<CachedLocationsModel>();

            if (model.SearchInSK)
            {
                relatedLocations.AddRange(cachedLocations.Where(cl => cl.Country == Constants.SlovakiaShortCode));
            }
            else if (model.SearchInHU)
            {
                relatedLocations.AddRange(cachedLocations.Where(cl => cl.Country == Constants.HungaryShortCode));
            }
            else if (model.SearchInCZ)
            {
                relatedLocations.AddRange(cachedLocations.Where(cl => cl.Country == Constants.CzechiaShortCode));
            }

            return relatedLocations;
        }
    }
}