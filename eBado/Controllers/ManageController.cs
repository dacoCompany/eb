using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        [HttpPost]
        public JsonResult GetPostalCodes(string prefix)
        {
            List<PostalCodeModel> ObjList = new List<PostalCodeModel>()
            {

                new PostalCodeModel {Id=1,PostalCode="040 01 - Kosice - Barca" },
                new PostalCodeModel {Id=2,PostalCode="040 02" },
                new PostalCodeModel {Id=3,PostalCode="040 03" },
                new PostalCodeModel {Id=4,PostalCode="040 04" },
                new PostalCodeModel {Id=5,PostalCode="040 05" },
                new PostalCodeModel {Id=6,PostalCode="040 06" },
                new PostalCodeModel {Id=7,PostalCode="040 07" }

        };
            var postalCode = ObjList.Where(p => p.PostalCode.StartsWith(prefix)).Select(s => new
            {
                val = s.Id,
                label = s.PostalCode
            }).ToList();
            return Json(postalCode, JsonRequestBehavior.AllowGet);
        }
    }
}