using System.Web.Mvc;

namespace Web.eBado.Controllers
{
    [RoutePrefix("Error")]
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [Route("Error401")]
        [AllowAnonymous]
        public ActionResult Error401()
        {
            Response.StatusCode = 401;
            return View();
        }

        [Route("Error404")]
        [AllowAnonymous]
        public ActionResult Error404()
        {
            Response.StatusCode = 404;
            return View();
        }

        [Route("Error429")]
        [AllowAnonymous]
        public ActionResult Error429()
        {
            Response.StatusCode = 429;
            return View();
        }

        [Route("Error500")]
        [AllowAnonymous]
        public ActionResult Error500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}