using Infrastructure.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Web.eBado.Models.Account;

namespace Web.eBado.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompanyRequiredAttribute : ValidationAttribute
    {
        private ICollection<string> memberNames;

        public CompanyRequiredAttribute()
        {
            memberNames = new Collection<string>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = value as CompanyModel;

            switch (model.CompanyType)
            {
                case CompanyType.PartTime:
                    return PartTimeValidation(model);
                case CompanyType.SelfEmployed:
                    return SelfEmployerValidation(model);
                case CompanyType.Company:
                    return CompanyValidation(model);
                default:
                    return ValidationResult.Success;
            }
        }

        private ValidationResult PartTimeValidation(CompanyModel model)
        {
            if (model.Categories.SelectedCategories == null)
            {
                memberNames.Add(nameof(model.Categories));
            }

            if (memberNames.Any())
            {
                return new ValidationResult(ErrorMessageString, memberNames);
            }

            return ValidationResult.Success;
        }

        private ValidationResult SelfEmployerValidation(CompanyModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName))
            {
                memberNames.Add(nameof(model.CompanyName));
            }

            if (model.CompanyIco == null)
            {
                memberNames.Add(nameof(model.CompanyIco));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyPhoneNumber))
            {
                memberNames.Add(nameof(model.CompanyPhoneNumber));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyStreet))
            {
                memberNames.Add(nameof(model.CompanyStreet));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyStreetNumber))
            {
                memberNames.Add(nameof(model.CompanyStreetNumber));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyPostalCode))
            {
                memberNames.Add(nameof(model.CompanyPostalCode));
            }

            if (model.Categories.SelectedCategories == null)
            {
                memberNames.Add(nameof(model.Categories));
            }

            if (memberNames.Any())
            {
                return new ValidationResult(ErrorMessageString, memberNames);
            }

            return ValidationResult.Success;
        }

        private ValidationResult CompanyValidation(CompanyModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName))
            {
                memberNames.Add(nameof(model.CompanyName));
            }

            if (model.CompanyIco == null)
            {
                memberNames.Add(nameof(model.CompanyIco));
            }

            if (model.CompanyDic == null)
            {
                memberNames.Add(nameof(model.CompanyDic));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyPhoneNumber))
            {
                memberNames.Add(nameof(model.CompanyPhoneNumber));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyStreet))
            {
                memberNames.Add(nameof(model.CompanyStreet));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyStreetNumber))
            {
                memberNames.Add(nameof(model.CompanyStreetNumber));
            }

            if (string.IsNullOrWhiteSpace(model.CompanyPostalCode))
            {
                memberNames.Add(nameof(model.CompanyPostalCode));
            }

            if (model.Categories.SelectedCategories == null)
            {
                memberNames.Add(nameof(model.Categories));
            }

            if (memberNames.Any())
            {
                return new ValidationResult(ErrorMessageString, memberNames);
            }

            return ValidationResult.Success;
        }
    }
}