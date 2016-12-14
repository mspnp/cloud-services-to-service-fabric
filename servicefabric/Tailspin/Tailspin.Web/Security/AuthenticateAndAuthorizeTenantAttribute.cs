using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Tailspin.Web.Security
{
    // Authorizes the user if the user's tenant ID matches the tenant given in the route data.
    public class AuthenticateAndAuthorizeTenantAttribute : AuthorizeAttribute
    {
        public override bool Match(object obj)
        {
            var httpContext = (HttpContext)obj;

            //TODO FIX
            var routeTenantId = "TODO FIX";//(string)httpContext...RouteData.Values["tenantId"];

            if (String.IsNullOrEmpty(routeTenantId))
            {
                return false;
            }

            if (base.Match(httpContext))
            {
                var principal = httpContext.User.Identity as ClaimsIdentity;
                var tenantId = principal.GetTenantIdValue();
                return tenantId.Equals(routeTenantId, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return false;
            }
        }
    }
}