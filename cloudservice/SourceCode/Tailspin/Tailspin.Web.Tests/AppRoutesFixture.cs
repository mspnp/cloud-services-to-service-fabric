namespace Tailspin.Web.Tests
{
    using System;
    using System.Linq;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppRoutesFixture
    {
        [TestMethod]
        public void RegisterOnBoardingRoute()
        {
            var routes = new RouteCollection();

            AppRoutes.RegisterRoutes(routes);

            var route = routes.Cast<Route>().SingleOrDefault(r =>
                    string.Equals(r.Url, string.Empty, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["controller"] as string, "OnBoarding", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["action"] as string, "Index", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(route);
        }


        [TestMethod]
        public void RegisterAuthenticationRoute()
        {
            var routes = new RouteCollection();

            AppRoutes.RegisterRoutes(routes);

            var route = routes.Cast<Route>().SingleOrDefault(r =>
                    string.Equals(r.Url, "Account/{action}", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["controller"] as string, "Authentication", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(route);
        }

        [TestMethod]
        public void RegisterMyAccountRoute()
        {
            var routes = new RouteCollection();

            AppRoutes.RegisterRoutes(routes);

            var route = routes.Cast<Route>().SingleOrDefault(r =>
                    string.Equals(r.Url, "{tenantId}/MyAccount", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["controller"] as string, "Account", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["action"] as string, "Index", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(route);
        }

        [TestMethod]
        public void RegisterUploadLogoRoute()
        {
            var routes = new RouteCollection();

            AppRoutes.RegisterRoutes(routes);

            var route = routes.Cast<Route>().SingleOrDefault(r =>
                    string.Equals(r.Url, "{tenantId}/MyAccount/UploadLogo", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["controller"] as string, "Account", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["action"] as string, "UploadLogo", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(route);
        }
    }
}
