using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class UserAccountSettingsModel
    {
        public ChangePasswordModel PasswordModel { get; set; }
        public ChangeSettingsModel SettingsModel { get; set; }
    }
}