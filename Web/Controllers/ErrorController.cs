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
            return View();
        }
    }
}