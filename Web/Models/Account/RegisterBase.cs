using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class RegisterBase
    {
        public RegisterBase()
        {
        }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public string SelectedSubCategory{ get; set; }
    }
}