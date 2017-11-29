using Infrastructure.Common.DB;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web.eBado.Helpers;
using Web.eBado.Models.Account;
using Web.eBado.Models.Company;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    [RoutePrefix("Company")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        CompanyHelper companyHelper;
        SharedHelper sharedHelper;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            companyHelper = new CompanyHelper(unitOfWork);
            sharedHelper = new SharedHelper(unitOfWork);
        }

        [Route("AllCompanies")]
        [AllowAnonymous]
        public ActionResult AllCompanies(string selectedCategory, CompanySearchModel model)
        {
            model = companyHelper.GetAllCompanies(model, unitOfWork);

            if (model.SearchParameters.SelectedCategory == null)
            {
                model.SearchParameters.SelectedCategory = companyHelper.GetCategoryBySelectedItem(selectedCategory);
            }
            model.AllMainCategories = sharedHelper.GetMainCategoriesToListItem();
            model.AllCategories = sharedHelper.GetCategoriesWithSubCategories();
            return View(model);
        }
    }
}