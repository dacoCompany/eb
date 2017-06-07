using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Povinne pole")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Povinne pole")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string RepeatNewPassword { get; set; }
    }
}