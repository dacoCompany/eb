using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        [Route("Error500")]
        [AllowAnonymous]
        public ActionResult Error500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}