﻿using Infrastructure.Resources;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Web.eBado.Models.Account
{
    [HasSelfValidation]
    public class UserModel
    {
        [IgnoreNulls(Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 20, RangeBoundaryType.Inclusive, MessageTemplate = "Too long (max. 20 characters)", Ruleset = "RegisterUser")]
        public string Title { get; set; }

        [IgnoreNulls(Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Inclusive, MessageTemplate = "Too long (max. 50 characters)", Ruleset = "RegisterUser")]
        public string FirstName { get; set; }

        [IgnoreNulls(Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Inclusive, MessageTemplate = "Too long (max. 50 characters)", Ruleset = "RegisterUser")]
        public string Surname { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 100, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 100, RangeBoundaryType.Inclusive, MessageTemplate = "Too long (max. 100 characters)", Ruleset = "RegisterUser")]
        [EmailAddress]
        public string Email { get; set; }

        [StringLengthValidator(8, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Ignore, MessageTemplate = "Must be at least 8 characters", Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterUser")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLengthValidator(8, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Ignore, MessageTemplate = "Must be at least 8 characters", Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterUser")]
        [DataType(DataType.Password)]
        public string RepeatPassword { get; set; }

        [IgnoreNulls(Ruleset = "RegisterUser")]
        [StringLengthValidator(10, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Inclusive, MessageTemplate = "Must be exactly 10 characters", Ruleset = "RegisterUser")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [IgnoreNulls(Ruleset = "RegisterUser")]
        [StringLengthValidator(10, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Inclusive, MessageTemplate = "Must be exactly 10 characters", Ruleset = "RegisterUser")]
        [DataType(DataType.PhoneNumber)]
        public string AdditionalPhoneNumber { get; set; }

        [IgnoreNulls(Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 100, RangeBoundaryType.Ignore, MessageTemplate = "Too long (max. 100 characters)", Ruleset = "RegisterUser")]
        public string Street { get; set; }

        [IgnoreNulls(Ruleset = "RegisterUser")]
        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 10, RangeBoundaryType.Ignore, MessageTemplate = "Too long (max. 10 characters)", Ruleset = "RegisterUser")]
        public string StreetNumber { get; set; }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 50, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = "RequiredField", Ruleset = "RegisterUser")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Validation of whether the entered passwords match.
        /// </summary>
        /// <param name="results">Results of the validation.</param>
        [SelfValidation(Ruleset = "RegisterUser")]
        public void ValidatePasswordMatch(ValidationResults results)
        {
            if (results == null)
                return;

            if (Password != RepeatPassword)
            {
                results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult("Passwords must match.", this, nameof(Password), null, null));
                results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult("Passwords must match.", this, nameof(RepeatPassword), null, null));
            }
        }
    }
}