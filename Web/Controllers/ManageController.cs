using Infrastructure.Common.DB;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Web.eBado.IoC;

namespace Web.eBado.Controllers
{
    public class ManageController : Controller
    {
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