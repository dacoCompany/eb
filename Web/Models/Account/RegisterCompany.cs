using Infrastructure.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class RegisterCompany : RegisterUser
    {
        [Required(ErrorMessage = "Required field!")]
        public AccountType CompanyType { get; set; }

        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string Ico { get; set; }

        [Required(ErrorMessage = "Required field!")]
        [DataType(DataType.PhoneNumber)]
        public string CompanyPhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string CompanyAdditionalPhoneNumber { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string CompanyStreet { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string CompanyStreetNumber { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string CompanyPostalCode { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public ICollection<CategoriesModel> Categories { get; set; }
    }
}