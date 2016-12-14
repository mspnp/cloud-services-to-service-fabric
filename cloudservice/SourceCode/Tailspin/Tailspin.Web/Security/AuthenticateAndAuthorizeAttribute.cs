using System.Web.Mvc;

namespace Tailspin.Web.Security
{

    // Modifies the [Authorize] to return a 403 (Forbidden) in the case where the request
    // is authenticated but not authorized.

    // This is needed to make role-based authorization work correctly, otherwise unauthorized
    // requests will always redirect to the IDP, even if the user is already signed in.

    // Note - This is a temporary workaround, ASP.NET Core has the correct behavior.

    public class AuthenticateAndAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult(403);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}