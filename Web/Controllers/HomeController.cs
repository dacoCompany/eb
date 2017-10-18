using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Common.Models;
using Newtonsoft.Json;
using Infrastructure.Configuration;
using System.Configuration;
using System.Net;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult SetLanguage(string language)
        {
            var supportedLang = ConfigurationManager.AppSettings.Get(ConfigurationKeys.SupportedLanguagesKey);
            if (supportedLang.Contains(language))
            {
                if (!(language == Request.Cookies["lang"].Value))
                {
                    CultureInfo ci = new CultureInfo(language);
                    Thread.CurrentThread.CurrentCulture = ci;
                    Thread.CurrentThread.CurrentUICulture = ci;
                    var requestCookie = Request.Cookies["lang"];
                    requestCookie.Value = language;
                    Response.SetCookie(requestCookie);
                    return new HttpStatusCodeResult(HttpStatusCode.Redirect);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult Index3()
        {
            return View();
        }

        [HttpGet]
        public string Index4()
        {
            return "Welcome";
        }
    }
}