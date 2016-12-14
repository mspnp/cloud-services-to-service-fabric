namespace Tailspin.Web.Tests.Area.Survey
{
    using System.Linq;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey;

    [TestClass]
    public class SurveyRegistrationFixture
    {
        //[TestMethod]
        //public void RegisterMySurveysRoute()
        //{
        //    var routeBuilder = new RouteBuilder(null);

        //    AppRoutes.RegisterRoutes(routeBuilder);

        //    var route = routeBuilder.Routes.SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "Index", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisterNewSurveyRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/newsurvey", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "New", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisterAnalyzeSurveyRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/{surveySlug}/analyze", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "Analyze", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisterBrowseSurveyResponsesRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/{surveySlug}/analyze/browse/{answerId}", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "BrowseResponses", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["answerId"] as string, string.Empty, System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisterExportSurveyResponsesRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/{surveySlug}/analyze/export", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "ExportResponses", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisteDeleteSurveyRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/{surveySlug}/delete", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "Delete", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisteNewQuestionRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/newquestion", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "NewQuestion", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisteAddQuestionRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/newquestion/add", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "AddQuestion", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}

        //[TestMethod]
        //public void RegisteCancelNewQuestionRoute()
        //{
        //    var routes = new RouteCollection();
        //    var registrationArea = new SurveyAreaRegistration();
        //    var areaRegistrationContext = new AreaRegistrationContext(registrationArea.AreaName, routes);

        //    registrationArea.RegisterArea(areaRegistrationContext);

        //    var route = routes.Cast<Route>().SingleOrDefault(r =>
        //            string.Equals(r.Url, "survey/{tenantId}/newquestion/cancel", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["controller"] as string, "Surveys", System.StringComparison.OrdinalIgnoreCase) &&
        //            string.Equals(r.Defaults["action"] as string, "CancelNewQuestion", System.StringComparison.OrdinalIgnoreCase));
        //    Assert.IsNotNull(route);
        //}
    }
}
