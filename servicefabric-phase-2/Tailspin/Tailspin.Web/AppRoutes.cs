namespace Tailspin.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;

    public static class AppRoutes
    {
        public static void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                "MySurveys",
                string.Empty,
                new { controller = "Surveys", action = "Index" });

            routes.MapRoute(
                "NewSurvey",
                "survey/newsurvey",
                new { controller = "Surveys", action = "New" });

            routes.MapRoute(
                "NewQuestion",
                "survey/newquestion",
                new { controller = "Surveys", action = "NewQuestion" });

            routes.MapRoute(
                "AddQuestion",
                "survey/newquestion/add",
                new { controller = "Surveys", action = "AddQuestion" });

            routes.MapRoute(
                "CancelNewQuestion",
                "survey/newquestion/cancel",
                new { controller = "Surveys", action = "CancelNewQuestion" });

            routes.MapRoute(
                "AnalyzeSurvey",
                "survey/{surveySlug}/analyze",
                new { controller = "Surveys", action = "Analyze" });

            routes.MapRoute(
                "BrowseResponses",
                "survey/{surveySlug}/analyze/browse/{answerId}",
                new { controller = "Surveys", action = "BrowseResponses", answerId = string.Empty });

            routes.MapRoute(
                "ExportResponses",
                "survey/{surveySlug}/analyze/export",
                new { controller = "Surveys", action = "ExportResponses" });

            routes.MapRoute(
                "DeleteSurvey",
                "survey/{surveySlug}/delete",
                new { controller = "Surveys", action = "Delete" });

            routes.MapRoute(
                "ErrorResponse",
                "home/error",
                new { controller = "Home", action = "Error" });
        }
    }
}
