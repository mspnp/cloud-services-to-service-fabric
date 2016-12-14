namespace Tailspin.Web.Survey.Shared.QueueMessages
{
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public class SurveyTransferMessage : AzureQueueMessage
    {
        public string Tenant { get; set; }
        public string SlugName { get; set; }
    }
}