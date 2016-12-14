namespace Tailspin.Web.Survey.Public
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Practices.Unity;
    using Tailspin.Web.Survey.Public.Controllers;

    public class MvcApplication : System.Web.HttpApplication
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Microsoft.DisposeObjectsBeforeLosingScope", Justification = "This container is used in the controller factory and cannot be disposed.")]
        protected void Application_Start()
        {
            var container = new UnityContainer();
            ContainerBootstraper.RegisterTypes(container, false);
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(container));

            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            AppRoutes.RegisterRoutes(RouteTable.Routes);
        }
    }
}