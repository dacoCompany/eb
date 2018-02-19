using Infrastructure.Common.DB;
using MvcThrottle;
using System.Web.Mvc;
using Web.eBado.Helpers;
using Web.eBado.Models.Company;
using Web.eBado.Models.Shared;
using WebAPIFactory.Caching.Core;

namespace Web.eBado.Controllers
{
    [EnableThrottling]
    [RoutePrefix("Company")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        CompanyHelper companyHelper;
        SharedHelper sharedHelper;

        public CompanyController(IUnitOfWork unitOfWork, ICache httpCache)
        {
            this.unitOfWork = unitOfWork;
            companyHelper = new CompanyHelper(unitOfWork, httpCache);
            sharedHelper = new SharedHelper(unitOfWork, httpCache);
        }

        [Route("AllCompanies")]
        [AllowAnonymous]
        public ActionResult AllCompanies(CompanySearchModel model)
        {
            var session = Session["User"] as SessionModel;
            model = model ?? new CompanySearchModel();
            if (model.SelectedCategory != null)
            {
                model.SelectedMainCategory = model.SelectedCategory;
            }
            model = companyHelper.GetAllCompanies(model, unitOfWork, session);
            model = companyHelper.InitializeCompanyData(session, model, unitOfWork);

            return View(model);
        }

        [Route("CompanyDetail")]
        [AllowAnonymous]
        public ActionResult CompanyDetail(string id, CompanyDetailModel model)
        {
            var session = Session["User"] as SessionModel;
            model = model ?? new CompanyDetailModel();
            int companyId = sharedHelper.DecryptId(id);

            model = companyHelper.GetCompanyDetail(model, unitOfWork, companyId, session);
            return View(model);
        }
    }
}