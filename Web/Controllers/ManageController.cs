using Infrastructure.Common.DB;
using Infrastructure.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Web.eBado.Helpers;
using Web.eBado.IoC;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    public class ManageController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        SessionHelper sessionHelper;

        public ManageController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            sessionHelper = new SessionHelper();
        }
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

        public ActionResult SetAccount(string accountName)
        {
            var currentSession = Session["User"] as SessionModel;
            bool isUserAccount = currentSession.Name == accountName;
            SessionModel newSession = null;

            if (isUserAccount)
            {
                newSession = sessionHelper.SetUserSession(currentSession.Id, unitOfWork);

            }
            else
            {
                newSession = sessionHelper.SetCompanySession(accountName, currentSession, unitOfWork);

            }
            Session.Remove("User");
            Session["User"] = newSession;
            return new HttpStatusCodeResult(HttpStatusCode.Redirect);
        }

       

        [HttpGet]
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
    }
}