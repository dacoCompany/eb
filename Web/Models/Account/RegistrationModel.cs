using Infrastructure.Resources;
using Web.eBado.Validators;

namespace Web.eBado.Models.Account
{
    public class RegistrationModel
    {
        public RegistrationModel()
        {
            UserModel = new UserModel();
            CompanyModel = new CompanyModel();
        }

        public UserModel UserModel { get; private set; }

        [CompanyRequired(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public CompanyModel CompanyModel { get; private set; }

    }
}