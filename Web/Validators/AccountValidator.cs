using Infrastructure.Common.DB;
using Infrastructure.Common.Validations;
using System;
using Web.eBado.Models.Account;

namespace Web.eBado.Validators
{
    public static class AccountValidator
    {
        #region Method specific validation

        public static void ValidateUserRegistration(IUnitOfWork uow, ValidationResultCollection collection, RegisterUser model)
        {
            ValidateEmailExist(uow, collection, model.Email);
        }

        #endregion

        #region Attribute specific validation

        public static void ValidateEmailExist(IUnitOfWork uow, ValidationResultCollection collection, string emailAddress)
        {
            var userDetails = uow.UserDetailsRepository.FirstOrDefault(ud => ud.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));

            if (userDetails != null)
            {
                ValidationHelpers.AddValidationResult(collection, nameof(userDetails.Email), ValidationErrors.EmailAlreadyExist);
            }
        }

        #endregion
    }
}