using PagedList;
using System.Collections.Generic;
using System.Web.Mvc;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;

namespace Web.eBado.Models.Company
{
    public class CompanySearchModel
    {
        public int? Page { get; set; }
        public string SelectedMainCategory { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedSubCategory { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public int Radius { get; set; }
        public int DefaultRadius { get; set; }
        public bool SearchInSK { get; set; }
        public bool SearchInCZ { get; set; }
        public bool SearchInHU { get; set; }
        public IEnumerable<SelectListItem> AllMainCategories { get; set; }
        public IEnumerable<AllCategoriesModel> AllCategories { get; set; }
        public IPagedList<CompanyModel> CompanyModel { get; set; }        
    }
}