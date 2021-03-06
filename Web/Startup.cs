﻿using BackgroundProcessing.Hangfire;
using Infrastructure.IoC;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Owin;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(Web.eBado.Startup))]

namespace Web.eBado
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            //

            var container = IocContainer.GetContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DatabaseFactory.ClearDatabaseProviderFactory();
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            Logger.SetLogWriter(new LogWriterFactory().Create());

            var hangfireConfiguration = new HangfireConfiguration(container);
            app.UseHangfireJobServer(hangfireConfiguration.ServerOptions);
            app.UseHangfireJobDashboard("/hangfire", hangfireConfiguration.DashboardOptions);

            string issuer = "http://ebadoauthorization.azurewebsites.net/";
            string audience = "eBado";
            string secret = "VGhpcyBpcyBhIHRva2VuIHNlY3JldCBmb3IgZWJhZG8gYXV0aGVudGljYXRpb24u";

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret),
                }
            });

            //SetLanguageCookie();
        }

        private void SetLanguageCookie()
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
