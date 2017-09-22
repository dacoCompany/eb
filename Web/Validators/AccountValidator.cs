using Infrastructure.Common.DB;
using Infrastructure.Common.Validations;
using System;
using Web.eBado.Helpers;
using Web.eBado.Models.Account;

namespace Web.eBado.Validators
{
    public static class AccountValidator
    {
        #region Method specific validation

        public static void ValidateUserRegistration(IUnitOfWork uow, ValidationResultCollection collection, RegisterUserModel model)
        {
            ValidateEmailNotExist(uow, collection, model.Email);
        }

        public static void ValidateUserLogin(IUnitOfWork uow, ValidationResultCollection collection, LoginModel model)
        {
            ValidateUserExist(uow, collection, model);
        }

        #endregion

        #region Attribute specific validation

        public static void ValidateEmailNotExist(IUnitOfWork uow, ValidationResultCollection collection, string emailAddress)
        {
            var userDetails = uow.UserDetailsRepository.FirstOrDefault(ud => ud.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (userDetails != null)
            {
                ValidationHelpers.AddValidationResult(collection, nameof(userDetails.Email), ValidationErrors.EmailAlreadyExist);
            }
        }

        public static void ValidateUserExist(IUnitOfWork uow, ValidationResultCollection collection, LoginModel model)
        {
            var userDetails = uow.UserDetailsRepository.FirstOrDefault(ud => ud.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));
            if (userDetails != null)
            {
                var encodedPws = AccountHelper.EncodePassword(model.Password, userDetails.Salt);
                bool isSamePsw = uow.UserDetailsRepository.AnyActive(ud => ud.Password == encodedPws);
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