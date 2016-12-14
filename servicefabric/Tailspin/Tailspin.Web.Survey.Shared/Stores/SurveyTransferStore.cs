namespace Tailspin.Web.Survey.Shared.Stores
{
    using AzureStorage;
    using QueueMessages;
    using System.Threading.Tasks;

    public class SurveyTransferStore : ISurveyTransferStore
    {
        private readonly IAzureQueue<SurveyTransferMessage> surveyTransferQueue;

        public SurveyTransferStore(IAzureQueue<SurveyTransferMessage> surveyTransferQueue)
        {
            this.surveyTransferQueue = surveyTransferQueue;
        }

        public async Task InitializeAsync()
        {
            await this.surveyTransferQueue.EnsureExistsAsync().ConfigureAwait(false);
        }

        public async Task TransferAsync(string tenant, string slugName)
        {
            await this.surveyTransferQueue.AddMessageAsync(new SurveyTransferMessage { Tenant = tenant, SlugName = slugName }).ConfigureAwait(false);
        }
    }
}