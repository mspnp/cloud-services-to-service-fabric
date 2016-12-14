namespace Tailspin.Web.Areas.Survey
{
    using System.Web.Mvc;

    public class SurveyAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Survey";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MySurveys",
                "survey/{tenantId}",
                new { controller = "Surveys", action = "Index" });

            context.MapRoute(
                "NewSurvey",
                "survey/{tenantId}/newsurvey",
                new { controller = "Surveys", action = "New" });

            context.MapRoute(
                "NewQuestion",
                "survey/{tenantId}/newquestion",
                new { controller = "Surveys", action = "NewQuestion" });

            context.MapRoute(
                "AddQuestion",
                "survey/{tenantId}/newquestion/add",
                new { controller = "Surveys", action = "AddQuestion" });

            context.MapRoute(
                "CancelNewQuestion",
                "survey/{tenantId}/newquestion/cancel",
                new { controller = "Surveys", action = "CancelNewQuestion" });

            context.MapRoute(
                "AnalyzeSurvey",
                "survey/{tenantId}/{surveySlug}/analyze",
                new { controller = "Surveys", action = "Analyze" });

            context.MapRoute(
                "BrowseResponses",
                "survey/{tenantId}/{surveySlug}/analyze/browse/{answerId}",
                new { controller = "Surveys", action = "BrowseResponses", answerId = string.Empty });

            context.MapRoute(
                "ExportResponses",
                "survey/{tenantId}/{surveySlug}/analyze/export",
                new { controller = "Surveys", action = "ExportResponses" });

            context.MapRoute(
                "DeleteSurvey",
                "survey/{tenantId}/{surveySlug}/delete",
                new { controller = "Surveys", action = "Delete" });
        }
    }
}
