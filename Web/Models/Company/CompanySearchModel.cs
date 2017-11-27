using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;

namespace Web.eBado.Models.Company
{
    public class CompanySearchModel
    {
        public CompanySearchModel()
        {
            SearchParameters = new SearchParametersModel();
        }
        public IEnumerable<SelectListItem> AllMainCategories { get; set; }
        public IEnumerable<AllCategoriesModel> AllCategories { get; set; }
        public SearchParametersModel SearchParameters { get; set; }
        public ICollection<CompanyModel> CompanyModel { get; set; }
    }
}