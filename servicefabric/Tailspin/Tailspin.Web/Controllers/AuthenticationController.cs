using Microsoft.AspNetCore.Mvc;

namespace Tailspin.Web.Controllers
{
    // This controller is usually called "AccountController" but that's already used.
    public class AuthenticationController : Controller
    {
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            //if (!Request..IsAuthenticated) GET GUIDANCE FROM MIKE
            //{
            //    this.HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            //}
        }
        public void SignOut()
        {
            // Send an OpenID Connect sign-out request.
            //HttpContext.GetOwinContext().Authentication.SignOut(
                //OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

    }
}