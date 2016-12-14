namespace Tailspin.Web.Survey.Shared.QueueMessages
{
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public class SurveyAnswerStoredMessage : AzureQueueMessage
    {
        public string SurveyAnswerBlobId { get; set; }

        public string TenantId { get; set; }

        public string SurveySlugName { get; set; }

        public bool AppendedToAnswers { get; set; }
    }
}