using Infrastructure.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.eBado.Models.Shared;

namespace Web.eBado.Models.Account
{
    public class RegisterUser
    {        
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
    }
}