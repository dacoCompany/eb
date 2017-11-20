using Infrastructure.Common.Enums;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Resources;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

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

        public CompanyType CompanyType { get; set; }

        public Countries CompanyLocation { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 100, RangeBoundaryType.Inclusive, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        public string CompanyName { get; set; }

        [DataType(DataType.MultilineText)]
        public string CompanyDescription { get; set; }

        [IgnoreNulls(Ruleset = "RegisterCompany")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Inclusive, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        public string CompanyEmail { get; set; }

        public int? CompanyIco { get; set; }

        public int? CompanyDic { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Inclusive, MessageTemplate = "Too long (max. 10 characters)", Ruleset = "RegisterCompany")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        [DataType(DataType.PhoneNumber)]
        public string CompanyPhoneNumber { get; set; }

        [IgnoreNulls(Ruleset = "RegisterCompany")]
        [StringLengthValidator(10, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Inclusive, MessageTemplate = "Must be exactly 10 characters", Ruleset = "RegisterCompany")]
        public string CompanyAdditionalPhoneNumber { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyStreetNumber { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterCompany")]
        public string CompanyPostalCode { get; set; }

        [ObjectValidator("RegisterCompany", Ruleset = "RegisterCompany")]
        public CategoriesModel Categories { get; set; }

        public LanguagesModel Languages { get; set; }

        public string ProfileUrl { get; set; }

        /// <summary>
        /// Validation of whether the company address is filled.
        /// </summary>
        /// <param name="results">Results of the validation.</param>
        [SelfValidation(Ruleset = "RegisterCompany")]
        public void ValidatePasswordMatch(ValidationResults results)
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
        }

    }
}