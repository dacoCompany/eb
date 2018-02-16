using MvcThrottle;
using System.Web.Mvc;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    [EnableThrottling]
    [RoutePrefix("Home")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("~/")]
        [Route("Index")]
        [AllowAnonymous]
        public ActionResult Index(SiteAreaBaseModel<SiteItemModel> model)
        {
            model = model ?? new SiteAreaBaseModel<SiteItemModel>();
            return View(model);
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

        [HttpGet]
        [Route("RedirectToLogin")]
        [AllowAnonymous]
        public ActionResult RedirectToLogin(string returnUrl)
        {
            var model = new RedirectModel { RedirectUrl = returnUrl };
            return View(model);
        }
    }
}