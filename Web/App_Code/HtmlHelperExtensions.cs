using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Web.eBado.App_Code
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ValidationMessagesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression); ;
            var modelState = htmlHelper.ViewData.ModelState;

            // If we have multiple (server-side) validation errors, collect and present them.
            if (modelState.ContainsKey(propertyName) && modelState[propertyName].Errors.Count > 1)
            {
                var msgs = new StringBuilder();
                foreach (ModelError error in modelState[propertyName].Errors)
                {
                    msgs.AppendLine(error.ErrorMessage);
                }

                // Return standard ValidationMessageFor, overriding the message with our concatenated list of messages.
                return htmlHelper.ValidationMessageFor(expression, msgs.ToString(), htmlAttributes as IDictionary<string, object> ?? htmlAttributes);
            }

            // Revert to default behaviour.
            return htmlHelper.ValidationMessageFor(expression, null, htmlAttributes as IDictionary<string, object> ?? htmlAttributes);
        }
    }
}