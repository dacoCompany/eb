using System;
using Infrastructure.Common.DB;
using Infrastructure.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.eBado.Helpers;
using Web.eBado.IoC;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    [RoutePrefix("Manage")]
    public class ManageController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        SessionHelper sessionHelper;

        public ManageController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            sessionHelper = new SessionHelper();
        }

        [AllowAnonymous]
        [Route("SetLanguage")]
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

        [System.Web.Http.Authorize]
        [Route("SetAccount")]
        public async Task<ActionResult> SetAccount(string accountName)
        {
            var currentSession = Session["User"] as SessionModel;
            bool isUserAccount = currentSession.Name == accountName;
            SessionModel newSession = null;

            if (isUserAccount)
            {
                newSession = sessionHelper.SetUserSession(currentSession.Id, unitOfWork);

                int userRoleId = 0;

                using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
                {
                    userRoleId = uow.UserDetailsRepository.FindById(newSession.Id).UserRoleId;
                }

                bool result = await GetToken(userRoleId, 0);

                if (!result)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }

            }
            else
            {
                newSession = sessionHelper.SetCompanySession(accountName, currentSession, unitOfWork);

                int companyRoleId = 0;

                using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
                {
                    string companyRole = newSession.Companies.First(c => c.IsActive).CompanyRole;
                    companyRoleId = uow.CompanyRoleRepository.FindFirstOrDefault(cr => cr.Name == companyRole).Id;
                }

                bool result = await GetToken(0, companyRoleId);

                if (!result)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }

            }
            Session.Remove("User");
            Session["User"] = newSession;
            return new HttpStatusCodeResult(HttpStatusCode.Redirect);
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("GetPostalCodes")]
        public JsonResult GetPostalCodes(string prefix)
        {
            var location = new object();
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {

                location = uow.LocationRepository.FindWhere(x => x.PostalCode.StartsWith(prefix)
                    || x.PostalCode.Replace(" ", "").StartsWith(prefix.Replace(" ", ""))
                    || x.City.StartsWith(prefix)).Take(10).AsEnumerable()
                    .Select(loc => new
                    {
                        val = loc.Id,
                        label = $"{loc.PostalCode} - {loc.District} - {loc.City}"
                    }).ToList();
            }

            return Json(location, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetCategories")]
        public void GetCategories()
        {
            List<string> categoriesList = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                var text = $"myText{i}";
                categoriesList.Add(text);
            }

            // TempData["Categories"] = categoriesList;
            ViewBag.MultiselectCountry = new MultiSelectList(categoriesList);
        }

        private static string RemoveDiacritics(string city)
        {
            var normalizedString = city.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private async Task<bool> GetToken(int userRoleId = 0, int companyRoleId = 0)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:52708/");

            var response = await client.GetAsync($"api/OAuth/GetLoginToken?appId=123&userRoleId={userRoleId}&companyRoleId={companyRoleId}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var authCookie = new HttpCookie("tokenCookie", content) { HttpOnly = true };
                HttpContext.Response.AppendCookie(authCookie);

                return true;
            }

            return false;
        }
    }
}