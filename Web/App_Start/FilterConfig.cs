using MvcThrottle;
using System.Web.Mvc;

namespace Web.eBado
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalExceptionFilter());
            filters.Add(new EbadoThrottleFilter
            {
                Policy = new ThrottlePolicy(1, 10, 2000)
                {
                    IpThrottling = true,
                    EndpointThrottling = true,
                    EndpointType = EndpointThrottlingType.ControllerAndAction,
                    StackBlockedRequests = true
                },
                Repository = new CacheRepository(),
                Logger = new EbadoThrottleLogger()

            });
        }
    }
}
