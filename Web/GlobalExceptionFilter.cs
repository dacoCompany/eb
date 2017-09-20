using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;

namespace Web.eBado
{
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        /// <summary>Called when an exception occurs.</summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null) return;

            var exception = filterContext.Exception;

            EntlibLogger.LogError(GetType().Name, $"Service not available\r\n{exception.Message}", DiagnosticsLogging.Create("Global", "ExceptionFilter"), exception);

            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Service not available.");
            filterContext.ExceptionHandled = true;
        }
    }
}