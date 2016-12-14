using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Tailspin.Web.Security
{
    // Authorizes the user if the user's tenant ID matches the tenant given in the route data.
    public class AuthenticateAndAuthorizeTenantAttribute : AuthenticateAndAuthorizeAttribute
    {
        public string TenantId { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (String.IsNullOrEmpty(TenantId))
            {
                return false;
            }

            if (base.AuthorizeCore(httpContext))
            {
                var principal = httpContext.User.Identity as ClaimsIdentity;
                var tenantId = principal.GetTenantIdValue();
                return tenantId.Equals(this.TenantId, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            this.TenantId = (string)filterContext.RouteData.Values["tenantId"];
            base.OnAuthorization(filterContext);
        }
    }
}