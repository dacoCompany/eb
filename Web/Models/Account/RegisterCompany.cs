using Infrastructure.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class RegisterCompany : RegisterUser
    {
        public CompanyType CompanyType { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string CompanyName { get; set; }

        public string Ico { get; set; }

        [Required(ErrorMessage = "Required field!")]
        [DataType(DataType.PhoneNumber)]
        public int? CompanyPhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        public int? CompanyAdditionalPhoneNumber { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyStreetNumber { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string CompanyPostalCode { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public ICollection<string> Categories { get; set; }
    }
}