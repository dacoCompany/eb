using Infrastructure.Resources;
using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class UserModel
    {
        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

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