using System.Web.Mvc;
using Web.eBado.Models.Account;

namespace Web.eBado.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(RegisterUser model)
        {
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterPartTime()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPartTime(RegisterPartTime model)
        {
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterSelfEmployed()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSelfEmployed(RegisterSelfEmployed model)
        {
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterCompany()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCompany(RegisterCompany model)
        {
            return View(model);
        }
    }
}