namespace Tailspin.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;

    public static class AppRoutes
    {
        public static void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                "OnBoarding",
                string.Empty,
                new { controller = "OnBoarding", action = "Index" });

            routes.MapRoute(
                "Management",
                "Management",
                new { controller = "Management", action = "Index" });

            routes.MapRoute(
                "Management-New",
                "Management/new",
                new { controller = "Management", action = "New" });

            routes.MapRoute(                
                "Management-Detail",
                "Management/{tenantId}",
                new { controller = "Management", action = "Detail" });

            routes.MapRoute(
               "JoinTenant",
               "Join",
               new { controller = "OnBoarding", action = "Join" });

            routes.MapRoute(
                "MyAccount",
                "{tenantId}/MyAccount",
                new { controller = "Account", action = "Index" });

            routes.MapRoute(
                "UploadLogo",
                "{tenantId}/MyAccount/UploadLogo",
                new { controller = "Account", action = "UploadLogo" });

            routes.MapRoute(
                "Authentication",
                "Account/{action}",
                new { controller = "Authentication" });

            //Survey routes
            routes.MapRoute(
                "MySurveys",
                "survey/{tenantId}",
                new { controller = "Surveys", action = "Index" });

            routes.MapRoute(
                "NewSurvey",
                "survey/{tenantId}/newsurvey",
                new { controller = "Surveys", action = "New" });

            routes.MapRoute(
                "NewQuestion",
                "survey/{tenantId}/newquestion",
                new { controller = "Surveys", action = "NewQuestion" });

            routes.MapRoute(
                "AddQuestion",
                "survey/{tenantId}/newquestion/add",
                new { controller = "Surveys", action = "AddQuestion" });

            routes.MapRoute(
                "CancelNewQuestion",
                "survey/{tenantId}/newquestion/cancel",
                new { controller = "Surveys", action = "CancelNewQuestion" });

            routes.MapRoute(
                "AnalyzeSurvey",
                "survey/{tenantId}/{surveySlug}/analyze",
                new { controller = "Surveys", action = "Analyze" });

            routes.MapRoute(
                "BrowseResponses",
                "survey/{tenantId}/{surveySlug}/analyze/browse/{answerId}",
                new { controller = "Surveys", action = "BrowseResponses", answerId = string.Empty });

            routes.MapRoute(
                "ExportResponses",
                "survey/{tenantId}/{surveySlug}/analyze/export",
                new { controller = "Surveys", action = "ExportResponses" });

            routes.MapRoute(
                "DeleteSurvey",
                "survey/{tenantId}/{surveySlug}/delete",
                new { controller = "Surveys", action = "Delete" });
        }
    }
}
