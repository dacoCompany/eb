using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.eBado.Models.Account
{
    public class LanguagesModel
    {
        public string[] SelectedLanguages { get; set; }

        public IEnumerable<SelectListItem> AllLanguages { get; set; }
    }
}