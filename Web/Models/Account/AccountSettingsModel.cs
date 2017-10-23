using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class AccountSettingsModel
    {
        public bool IsFacebookLogin { get; set; }
        public ChangePasswordModel PasswordModel { get; set; }
        public UserModel UserModel { get; set; }
        public CompanyModel CompanyModel { get; set; }
        public SearchSettingsModel SearchModel { get; set; }
        public LanguageModel LanguageModel { get; set; }
        public NotificationModel NotificationModel { get; set; }
    }
}