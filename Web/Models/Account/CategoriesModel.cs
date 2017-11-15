using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.eBado.Models.Account
{
    public class CategoriesModel
    {
        public string[] SelectedCategories { get; set; }
        public IEnumerable<SelectListItem> AllCategories { get; set; }
    }
}