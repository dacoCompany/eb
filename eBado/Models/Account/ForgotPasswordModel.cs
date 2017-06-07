using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Povinne pole")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}