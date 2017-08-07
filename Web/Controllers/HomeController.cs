using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Common.Models;
using Newtonsoft.Json;

namespace Web.eBado.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var baseUri = new Uri("http://localhost:50198/");
            var client = new HttpClient();
            client.BaseAddress = baseUri;
            var response = await client.GetAsync(new Uri(baseUri, "api/SKRegister/GetCompanyDetailsById?id=36168165"));

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<CompanyDetailsModel>(response.Content.ReadAsStringAsync().Result);
            }

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
    }
}