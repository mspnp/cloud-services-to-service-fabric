namespace Tailspin.Web.Survey.Public.Tests
{
    using System.Linq;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey.Public;

    [TestClass]
    public class AppRoutesFixture
    {
        [TestMethod]
        public void RegisterDisplaySurveyRoute()
        {
            var routes = new RouteCollection();

            AppRoutes.RegisterRoutes(routes);

            var route = routes.Cast<Route>().SingleOrDefault(r =>
                    string.Equals(r.Url, "survey/{tenantId}/{surveySlug}", System.StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["action"] as string, "Display", System.StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(route);
        }

        [TestMethod]
        public void RegisteThankYouForFillingSurveyRoute()
        {
            var routes = new RouteCollection();

            AppRoutes.RegisterRoutes(routes);

            var route = routes.Cast<Route>().SingleOrDefault(r =>
                    string.Equals(r.Url, "survey/{tenantId}/{surveySlug}/thankyou", System.StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["action"] as string, "ThankYou", System.StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(route);
        }

        [TestMethod]
        public void RegisteHomeRoute()
        {
            var routes = new RouteCollection();

            AppRoutes.RegisterRoutes(routes);

            var route = routes.Cast<Route>().SingleOrDefault(r =>
                    string.Equals(r.Url, string.Empty, System.StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Defaults["action"] as string, "Index", System.StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(route);
        }
    }
}
