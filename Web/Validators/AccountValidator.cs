using Infrastructure.Common.DB;
using Infrastructure.Common.Validations;
using System;
using Web.eBado.Helpers;
using Web.eBado.Models.Account;

namespace Web.eBado.Validators
{
    public static class AccountValidator
    {
        #region Method specific validations

        public static void ValidateUserRegistration(IUnitOfWork uow, ValidationResultCollection collection, UserModel model)
        {
            ValidateEmailNotExist(uow, collection, model.Email, nameof(model.Email));
            ValidatePasswordEquality(uow, collection, model.Password, model.RepeatPassword, nameof(model.RepeatPassword));
        }

        public static void ValidateUserLogin(IUnitOfWork uow, ValidationResultCollection collection, LoginModel model)
        {
            ValidateUserCredentials(uow, collection, model);
        }

        public static void ValidateCompanyRegistration(IUnitOfWork uow, ValidationResultCollection validationResult, CompanyModel model)
        {

        }

        #endregion

        #region Attribute specific validations

        private static void ValidateEmailNotExist(IUnitOfWork uow, ValidationResultCollection collection, string emailAddress, string parameterName)
        {
            var userDetails = uow.UserDetailsRepository.FirstOrDefault(ud => ud.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (userDetails != null)
            {
                ValidationHelpers.AddValidationResult(collection, nameof(userDetails.Email), ValidationErrors.EmailAlreadyExist);
            }
        }

        private static void ValidatePasswordEquality(IUnitOfWork uow, ValidationResultCollection collection, string password, string repeatPassword, string parameterName)
        {
            if (!password.Equals(repeatPassword))
            {
                ValidationHelpers.AddValidationResult(collection, parameterName, ValidationErrors.PasswordsAreNotEqual);
            }
        }

        private static void ValidateUserCredentials(IUnitOfWork uow, ValidationResultCollection collection, LoginModel model)
        {
            var userDetails = uow.UserDetailsRepository.FirstOrDefault(ud => ud.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));
            if (userDetails != null)
            {
                var encodedPws = AccountHelper.EncodePassword(model.Password, userDetails.Salt);
                bool isSamePsw = uow.UserDetailsRepository.AnyActive(ud => ud.Password.Equals(encodedPws));
                if (!isSamePsw)
                {
                    ValidationHelpers.AddValidationResult(collection, nameof(LoginModel.ErrorMessage), ValidationErrors.WrongLogin);
                }
            }
            else
            {
                ValidationHelpers.AddValidationResult(collection, nameof(LoginModel.ErrorMessage), ValidationErrors.WrongLogin);
            }
        }

        #endregion
    }
}