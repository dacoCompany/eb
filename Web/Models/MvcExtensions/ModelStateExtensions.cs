using Infrastructure.Common.Validations;
using System.Web.Mvc;

namespace Web.eBado.Models.MvcExtensions
{
    public static class ModelStateExtensions
    {
        public static void AddModelErrors(this ModelStateDictionary modelState, ValidationResultCollection collection)
        {
            if (collection.Count > 0)
            {
                foreach (var validationResult in collection)
                {
                    modelState.AddModelError(validationResult.Name, validationResult.Description);
                }
            }
        }
    }
}