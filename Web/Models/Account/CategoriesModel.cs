using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.eBado.Models.Account
{
    public class CategoriesModel
    {
        [Required]
        public string[] SelectedCategories { get; set; }
        public IEnumerable<SelectListItem> AllCategories { get; set; }
    }
}