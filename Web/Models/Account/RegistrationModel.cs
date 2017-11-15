using System;
using Infrastructure.Resources;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
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

        [ObjectValidator("RegisterUser", Ruleset = "RegisterUser")]
        public UserModel UserModel { get; private set; }

        [CompanyRequired(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public CompanyModel CompanyModel { get; private set; }

    }
}