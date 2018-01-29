using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; }

        public bool ShouldRedirect { get; set; }
    }
}