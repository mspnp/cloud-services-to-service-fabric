namespace Tailspin.Web.Survey.Shared
{
    public static class AzureConstants
    {
        public static class BlobContainers
        {
            public const string SurveyAnswers = "tempsurveyanswers";
            public const string SurveyAnswersSummaries = "surveyanswerssummaries";
            public const string SurveyAnswersLists = "surveyanswerslists";
            public const string Tenants = "tenants";
            public const string Logos = "logos";
        }

        public static class Queues
        {
            public const string SurveyAnswerStoredStandard = "surveyanswerstoredstandard";
            public const string SurveyAnswerStoredPremium = "surveyanswerstoredpremium";
            public const string SurveyTransferRequest = "surveytransfer";
        }

        public static class Tables
        {
            public const string Surveys = "Surveys";
            public const string Questions = "Questions";

            public const string SurveyExtensions = "SurveyExtensions";
        }

        public static class LogsConfiguration
        {
            public const int ScheduledTransferMinutes = 5;
            public const string DefaultStorageConfigSetting = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        }
    }
}
