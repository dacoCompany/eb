using Infrastructure.Common.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.eBado.IoC;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    public class ManageController : Controller
    {
        public JsonResult GetSubCategories(string id)
        {
            List<SelectListItem> subCategories = new List<SelectListItem>();

            subCategories.Add(new SelectListItem { Text = "", Value = "0" });
            subCategories.Add(new SelectListItem { Text = "Daco1", Value = "1" });
            subCategories.Add(new SelectListItem { Text = "Daco2", Value = "2" });

            return Json(new SelectList(subCategories, "Value", "Text"));
        }

        [HttpGet]
        public JsonResult GetPostalCodes(string prefix)
        {
            var location = new object();
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {
                location = uow.LocationRepository.FindWhere(x => x.PostalCode.StartsWith(prefix)
                    || x.PostalCode.Replace(" ", "").StartsWith(prefix.Replace(" ", ""))).Take(10).AsEnumerable()
                    .Select(loc => new
                    {
                        val = loc.Id,
                        label = $"{loc.PostalCode}-{loc.City}"
                    }).ToList();
            }

            return Json(location, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void GetCategories()
        {
            List<string> categoriesList = new List<string>();
           for(var i=0; i<10; i++)
            {
                var text = $"myText{i}";
                categoriesList.Add(text);
            }
            
           TempData["Categories"] = categoriesList;
        }

    }
}