using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Web.eBado.IoC;

namespace Web.eBado
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //DependencyResolver.SetResolver(new NinjectResolver());
            //AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            //DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var langCookie = HttpContext.Current.Request.Cookies["lang"];

            CultureInfo ci;

            // Get the language from cookie value, if not present(first call to page), set it based on browser language
            if (langCookie != null)
            {
                ci = GetCultureInfo(langCookie.Value);
            }
            else
            {
                string[] requestLang = HttpContext.Current.Request.UserLanguages;

                if (requestLang == null || !requestLang.Any())
                {
                    string langName = "en-US";
                    ci = new CultureInfo(langName);
                }
                else
                {
                    string lang = requestLang.First();
                    ci = GetCultureInfo(lang);
                }

                langCookie = new HttpCookie("lang", ci.Name) { HttpOnly = true };

                HttpContext.Current.Response.AppendCookie(langCookie);
            }


            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
        }

        private CultureInfo GetCultureInfo(string lang)
        {
            CultureInfo ci;
            try
            {
                ci = new CultureInfo(lang);
            }
            catch (CultureNotFoundException exception)
            {
                Console.WriteLine(exception);
                string langName = "en-US";
                ci = new CultureInfo(langName);
            }
            return ci;
        }
    }
}
