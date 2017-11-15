using Infrastructure.Common.Validations;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace Web.eBado.Models.MvcExtensions
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Adds the model errors from validation result collection. This methods adds new key-value pairs into ModelStateDictionary
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="collection">The collection.</param>
        public static void AddModelErrors(this ModelStateDictionary modelState, ValidationResultCollection collection)
        {
            foreach (var validationResult in collection)
            {
                modelState.AddModelError(validationResult.ParameterName, validationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Adds the validation errors from validation result collection. This adds errors to existing key-value pairs inside ModelStateDictionary
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="collection">The collection.</param>
        public static void AddValidationErrors(this ModelStateDictionary modelState, ValidationResultCollection collection)
        {
            foreach (var validationResult in collection)
            {
                var keyValuePair = modelState.FirstOrDefault(item => item.Key.EndsWith(validationResult.ParameterName));
                keyValuePair.Value.Errors.Add(validationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Adds the validation errors from validation result collection. This adds errors to existing key-value pairs inside ModelStateDictionary
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="collection">The collection.</param>
        public static void AddValidationErrors(this ModelStateDictionary modelState, ValidationResults collection)
        {
            foreach (var validationResult in collection)
            {
                var keyValuePair = modelState.FirstOrDefault(item => item.Key.EndsWith(validationResult.Key));
                keyValuePair.Value.Errors.Add(validationResult.Message);
            }
        }
    }
}