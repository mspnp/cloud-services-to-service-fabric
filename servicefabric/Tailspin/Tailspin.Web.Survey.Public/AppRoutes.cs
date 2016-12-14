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
                "survey/{tenantId}/{surveySlug}",
                new { controller = "Surveys", action = "Display" });

            routes.MapRoute(
                "ThankYouForFillingTheSurvey",
                "survey/{tenantId}/{surveySlug}/thankyou",
                new { controller = "Surveys", action = "ThankYou" });
        }
    }
}
