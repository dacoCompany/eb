using Infrastructure.Resources;
using Web.eBado.Validators;

namespace Web.eBado.Models.Account
{
    public class RegisterCompanyModel
    {
        public RegisterCompanyModel()
        {
            UserModel = new RegisterUserModel();
            CompanyModel = new CompanyModel();
        }

        public RegisterUserModel UserModel { get; private set; }

        [CompanyRequired(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public CompanyModel CompanyModel { get; private set; }

    }
}