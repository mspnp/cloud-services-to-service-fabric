using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Tailspin.Web.Survey.Public
{

    public static class AppRoutes
    {
        public static void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                "Home",
                string.Empty,
                new { controller = "Surveys", action = "Index" });

            routes.MapRoute(
                "ViewSurvey",
                "survey/{surveySlug}",
                new { controller = "Surveys", action = "Display" });

            routes.MapRoute(
                "ThankYouForFillingTheSurvey",
                "survey/{surveySlug}/thankyou",
                new { controller = "Surveys", action = "ThankYou" });

            routes.MapRoute(
                "ErrorResponse",
                "home/error",
                new { controller = "Home", action = "Error" });
            }
    }
}
