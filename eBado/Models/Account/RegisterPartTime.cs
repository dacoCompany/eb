using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class RegisterPartTime : RegisterBase
    {
        [Required(ErrorMessage = "Povinne pole")]
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

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }
    }
}