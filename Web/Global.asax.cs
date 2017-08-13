using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.eBado.IoC;

namespace Web.eBado
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DependencyResolver.SetResolver(new NinjectResolver());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_AcquireRequestState(object sender, EventArgs e)
        //{
        //    string requestLang = (string)HttpContext.Current.Request.RequestContext.RouteData.Values["lang"];

        //    CultureInfo ci;

        //    if (string.IsNullOrEmpty(requestLang))
        //    {
        //        string langName = "en-US";
        //        ci = new CultureInfo(langName);
        //    }
        //    else
        //    {
        //        ci = new CultureInfo(requestLang);
        //    }

        //    Thread.CurrentThread.CurrentUICulture = ci;
        //    Thread.CurrentThread.CurrentCulture = ci;
        //}

    }
}
