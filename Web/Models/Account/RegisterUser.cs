using Infrastructure.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.eBado.Models.Shared;

namespace Web.eBado.Models.Account
{
    public class RegisterUser
    {
        #region User informations

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        [Required(ErrorMessage = "Required field!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string RepeatPassword { get; set; }

        [DataType(DataType.PhoneNumber)]
        public int? PhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        public int? AdditionalPhoneNumber { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string PostalCode { get; set; }

        #endregion

        #region Company informations

        public CompanyType CompanyType { get; set; }

        public string CompanyName { get; set; }

        public string Ico { get; set; }

        [DataType(DataType.PhoneNumber)]
        public int? CompanyPhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        public int? CompanyAdditionalPhoneNumber { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyStreetNumber { get; set; }

        public string CompanyPostalCode { get; set; }

        public ICollection<CategoriesModel> Categories { get; set; }

        #endregion

    }
}