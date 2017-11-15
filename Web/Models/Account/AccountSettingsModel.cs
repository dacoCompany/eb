using System.Collections.Generic;

namespace Web.eBado.Models.Account
{
    public class AccountSettingsModel
    {
        public AccountSettingsModel()
        {
            PasswordModel = new ChangePasswordModel();
            UserModel = new UserModel();
            CompanyModel = new CompanyModel();
            SearchModel = new SearchSettingsModel();
            NotificationModel = new NotificationModel();
            EditMembersAndRolesModel = new EditMembersAndRolesModel();
        }

        public bool IsFacebookLogin { get; set; }
        public IEnumerable<string> CurrentCategories { get; set; }        
        public IEnumerable<CurrentLanguagesModel> CurrentLanguages { get; set; }        
        public ChangePasswordModel PasswordModel { get; set; }
        public UserModel UserModel { get; set; }
        public CompanyModel CompanyModel { get; set; }
        public SearchSettingsModel SearchModel { get; set; }
        public NotificationModel NotificationModel { get; set; }
        public EditMembersAndRolesModel EditMembersAndRolesModel { get; set; }
    }
}