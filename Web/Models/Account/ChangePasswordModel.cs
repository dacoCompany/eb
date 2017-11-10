using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class ChangePasswordModel
    {
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string RepeatNewPassword { get; set; }
    }
}