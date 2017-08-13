using System.ComponentModel.DataAnnotations;
using Infrastructure.Resources;

namespace Web.eBado.Models.Account
{
    public class RegisterUser
    { 
        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public string RepeatPassword { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string AdditionalPhoneNumber { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string PostalCode { get; set; }
    }
}