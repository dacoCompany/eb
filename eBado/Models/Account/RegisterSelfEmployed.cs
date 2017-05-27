using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class RegisterSelfEmployed : RegisterBase
    {
        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "PostalCode")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "StreetNumber")]
        public string StreetNumber { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Ico")]
        public string Ico { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Dic")]
        public string Dic { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }
    }
}