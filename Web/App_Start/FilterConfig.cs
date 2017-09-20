using System.Web;
using System.Web.Mvc;

namespace Web.eBado
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
