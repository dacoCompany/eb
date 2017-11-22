using Infrastructure.Common.DB;
using System.Web.Mvc;

namespace Web.eBado.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        // GET: Company
        public ActionResult Index()
        {
            return View();
        }

        [Route("AllCompanies")]
        [AllowAnonymous]
        public ActionResult AllCompanies()
        {
            return View();
        }
    }
}