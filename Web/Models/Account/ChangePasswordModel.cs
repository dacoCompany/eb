using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class ChangePasswordModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string RepeatNewPassword { get; set; }
    }
}