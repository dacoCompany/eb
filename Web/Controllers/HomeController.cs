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
            var baseUri = new Uri("http://localhost:50198/");
            var client = new HttpClient();
            client.BaseAddress = baseUri;
            //var response = await client.GetAsync(new Uri(baseUri, "api/SKRegister/GetCompanyDetailsById?id=36168165"));

            //if (response.IsSuccessStatusCode)
            //{
            //    var result = JsonConvert.DeserializeObject<CompanyDetailsModel>(response.Content.ReadAsStringAsync().Result);
            //}

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
            string lang = (string)this.ControllerContext.RouteData.Values["lang"];
            return $"Lang: {lang}\t UI Culture: {Thread.CurrentThread.CurrentUICulture.Name}\t Culture: {Thread.CurrentThread.CurrentCulture.Name}";
        }
    }
}