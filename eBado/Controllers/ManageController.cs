using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}