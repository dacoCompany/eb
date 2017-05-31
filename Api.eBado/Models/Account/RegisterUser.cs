using System.ComponentModel.DataAnnotations;

namespace Api.eBado.Models.Account
{
    public class RegisterUser : RegisterBase
    {
        [Display(Name = "PostalCode")]
        public string PostalCode { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Street")]
        public string Street { get; set; }

        [Display(Name = "StreetNumber")]
        public string StreetNumber { get; set; }
    }
}