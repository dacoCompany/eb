using System.Web.Mvc;

namespace Web.eBado.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Dependency resolving example
            // using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            // {
            //     var location = uow.LocationRepository.FindAll();
            // }

            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult Index3()
        {
            return View();
        }
    }
}