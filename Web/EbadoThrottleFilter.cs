using Infrastructure.Resources;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.eBado
{
    public class EbadoThrottleFilter : MvcThrottle.ThrottlingFilter
    {
        public EbadoThrottleFilter() : base()
        {
            QuotaExceededMessage = "Too many requests.";
        }

        protected override ActionResult QuotaExceededResult(RequestContext context, string message, HttpStatusCode responseCode, string requestId)
        {
            var rateLimitedView = new ViewResult
            {
                ViewName = "Error429"
            };
            rateLimitedView.ViewData["Message"] = Resources.TooManyRequestsMessage;

            return rateLimitedView;
        }
    }
}