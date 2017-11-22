using System.Collections.Generic;
using System.Web.Mvc;
using Infrastructure.Resources;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Web.eBado.Models.Account
{
    [HasSelfValidation]
    public class CategoriesModel
    {
        public CategoriesModel()
        {
            SelectedCategories = null;
        }

        public string[] SelectedCategories { get; set; }

        public IEnumerable<SelectListItem> AllCategories { get; set; }

        /// <summary>
        /// Validation of whether the company address is filled.
        /// </summary>
        /// <param name="results">Results of the validation.</param>
        [SelfValidation(Ruleset = "RegisterCompany")]
        public void ValidatePasswordMatch(ValidationResults results)
        {
            if (results == null)
                return;

            if (SelectedCategories == null || SelectedCategories.Length == 0)
            {
                results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult("Please select at least one category", this, $"CompanyModel.Categories.{nameof(SelectedCategories)}", null, null));
                results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resources.RequiredField, this, $"CompanyModel.Categories.{nameof(SelectedCategories)}", null, null));
            }
        }
    }
}