using System.Web.Mvc;

namespace Web.eBado.Controllers
{
    [RoutePrefix("Home")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("~/")]
        [Route("Index")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Index2")]
        [AllowAnonymous]
        public ActionResult Index2()
        {
            return View();
        }

        [Route("Index3")]
        [AllowAnonymous]
        public ActionResult Index3()
        {
            return View();
        }

        [HttpGet]
        [Route("Index4")]
        [AllowAnonymous]
        public string Index4()
        {
            return "Welcome";
        }
    }
}