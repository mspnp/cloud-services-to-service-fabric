namespace Tailspin.Web
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Practices.Unity;
    using Tailspin.Web.Controllers;
    using System.Web.Helpers;
    using System.IdentityModel.Claims;

    public class MvcApplication : System.Web.HttpApplication
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Microsoft.DisposeObjectsBeforeLosingScope", Justification = "This container is used in the controller factory and cannot be disposed.")]
        protected void Application_Start()
        {
            var container = new UnityContainer();
            ContainerBootstraper.RegisterTypes(container, false);

            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            AreaRegistration.RegisterAllAreas();
            AppRoutes.RegisterRoutes(RouteTable.Routes);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }

        private void Application_Error(object sender, System.EventArgs e)
        {
            System.Exception ex = Server.GetLastError();

            if (ex is System.Web.HttpRequestValidationException)
            {
            }
        }
    }
}