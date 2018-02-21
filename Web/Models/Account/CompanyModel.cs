using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Resources;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.eBado.Models.Account
{
    [HasSelfValidation]
    public class CompanyModel
    {
        public CompanyModel()
        {
            Categories = new CategoriesModel();
            Languages = new LanguagesModel();
        }

        public int Id { get; set; }

        public string CompanyId { get; set; }

        public CompanyType CompanyType { get; set; }

        public Countries CompanyLocation { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 100, RangeBoundaryType.Inclusive, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        public string CompanyName { get; set; }

        [DataType(DataType.MultilineText)]
        public string CompanyDescription { get; set; }

        [IgnoreNulls(Ruleset = "RegisterCompany")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Inclusive, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        public string CompanyEmail { get; set; }

        [RangeValidator(1, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        public int CompanyIco { get; set; }

        public int? CompanyDic { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Inclusive, MessageTemplate = "Too long (max. 10 characters)", Ruleset = "RegisterCompany")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        [DataType(DataType.PhoneNumber)]
        public string CompanyPhoneNumber { get; set; }

        public string CompanyAdditionalPhoneNumber { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyStreetNumber { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        public string CompanyPostalCode { get; set; }

        public string CompanyCity { get; set; }

        [ObjectValidator("RegisterCompany", Ruleset = "RegisterCompany")]
        [ObjectValidator("RegisterContractor", Ruleset = "RegisterContractor")]
        public CategoriesModel Categories { get; private set; }

        public LanguagesModel Languages { get; private set; }

        public IEnumerable<string> AllSelectedCategories { get; set; }

        public string ProfileUrl { get; set; }

        public DateTime DateRegistered { get; set; }

        /// <summary>
        /// Validation of whether the company address is filled.
        /// </summary>
        /// <param name="results">Results of the validation.</param>
        [SelfValidation(Ruleset = "RegisterCompany")]
        public void ValidateCompaneAddress(ValidationResults results)
        {
            if (results == null)
                return;

            if (CompanyType != CompanyType.Company)
            {
                if (!string.IsNullOrEmpty(CompanyStreet))
                {
                    results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resources.RequiredField, this, nameof(CompanyStreet), null, null));
                }

                if (!string.IsNullOrEmpty(CompanyStreetNumber))
                {
                    results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resources.RequiredField, this, nameof(CompanyStreetNumber), null, null));
                }
            }

            if (CompanyType != CompanyType.PartTime)
            {
                using (var uow = DependencyResolver.Current.GetService<IUnitOfWork>())
                {
                    var company = uow.CompanyDetailsRepository.FindFirstOrDefault(cd => cd.Ico == CompanyIco);

                    if (company != null)
                    {
                        results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult("Company already exists.", this, nameof(CompanyIco), null, null));
                    }
                }
            }
        }

    }
}