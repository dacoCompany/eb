using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;
using JwtSecurityToken = System.IdentityModel.Tokens.JwtSecurityToken;

namespace Web.eBado
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class EbadoMvcAuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly char[] splitParameter = new char[1]
        {
            ','
        };
        private readonly object typeId = new object();
        private string[] rolesSplit = new string[0];
        private string[] usersSplit = new string[0];
        private string roles;
        private string users;

        /// <summary>Gets or sets the user roles that are authorized to access the controller or action method.</summary>
        /// <returns>The user roles that are authorized to access the controller or action method.</returns>
        public string Roles
        {
            get
            {
                return this.roles ?? string.Empty;
            }
            set
            {
                this.roles = value;
                this.rolesSplit = SplitString(value);
            }
        }

        /// <summary>Gets the unique identifier for this attribute.</summary>
        /// <returns>The unique identifier for this attribute.</returns>
        public override object TypeId
        {
            get
            {
                return this.typeId;
            }
        }

        /// <summary>Gets or sets the users that are authorized to access the controller or action method.</summary>
        /// <returns>The users that are authorized to access the controller or action method.</returns>
        public string Users
        {
            get
            {
                return this.users ?? string.Empty;
            }
            set
            {
                this.users = value;
                this.usersSplit = SplitString(value);
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                return;

            var authHeader = httpContext.Request.Cookies["tokenCookie"];
            var url = filterContext.HttpContext.Request.Url;
            string loginUrl = $"~/Account/Login?returnUrl={HttpUtility.UrlEncode(url.ToString())}";

            if (authHeader == null)
            {
                EntlibLogger.LogError("Attribute", "Authorize", "Cookie 'tokenCookie' is missing from the request.", DiagnosticsLogging.Create("Attribute", "Authorization"));
                filterContext.Result = new RedirectResult(loginUrl, false);
                return;
            }

            if (string.IsNullOrEmpty(authHeader.Value))
            {
                EntlibLogger.LogError("Attribute", "Authorize", "Cookie 'tokenCookie' does not have a value.", DiagnosticsLogging.Create("Attribute", "Authorization"));
                filterContext.Result = new RedirectResult(loginUrl, false);
                return;
            }

            string headerToken = authHeader.Value.Replace("\"", string.Empty);

            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(headerToken))
            {
                var token = handler.ReadToken(headerToken) as JwtSecurityToken;

                if (token.ValidTo <= DateTime.Now)
                {
                    // token expired, redirect to login
                    filterContext.Result = new RedirectResult(loginUrl, false);
                    return;
                }

                if (rolesSplit.Length == 0)
                {
                    return;
                }

                var claims = token.Claims.Where(t => t.Type == "role").Select(t => new { t.Value });

                bool rolesMatch = claims.Any(c => rolesSplit.Any(r => r == c.Value));

                if (!rolesMatch)
                {
                    // return 401 unauthorized
                    filterContext.Result = (ActionResult)new HttpUnauthorizedResult();
                }
            }
            else
            {
                EntlibLogger.LogError("Attribute", "Authorize", "Invalid JWT token string", DiagnosticsLogging.Create("Attribute", "Authorization"));
                filterContext.Result = new RedirectResult(loginUrl, false);
            }
        }

        internal string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
                return new string[0];
            return ((IEnumerable<string>)original.Split(this.splitParameter)).Select(piece => new
            {
                piece = piece,
                trimmed = piece.Trim()
            }).Where(param0 => !string.IsNullOrEmpty(param0.trimmed)).Select(param0 => param0.trimmed).ToArray<string>();
        }
    }
}