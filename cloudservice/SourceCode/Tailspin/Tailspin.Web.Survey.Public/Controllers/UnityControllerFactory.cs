namespace Tailspin.Web.Survey.Public.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Practices.Unity;

    public class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly IUnityContainer container;

        public UnityControllerFactory(IUnityContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return this.container.Resolve(controllerType, new DependencyOverride<RequestContext>(requestContext)) as IController;
        }
    }
}