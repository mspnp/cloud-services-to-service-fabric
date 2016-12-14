namespace Tailspin.Web.Survey.Shared.Stores
{
    using System.Globalization;
    using Models;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using System.Threading.Tasks;

    public class SurveyAnswersSummaryStore : ISurveyAnswersSummaryStore
    {
        private readonly IAzureBlobContainer<SurveyAnswersSummary> surveyAnswersSummaryBlobContainer;

        public SurveyAnswersSummaryStore(IAzureBlobContainer<SurveyAnswersSummary> surveyAnswersSummaryBlobContainer)
        {
            this.surveyAnswersSummaryBlobContainer = surveyAnswersSummaryBlobContainer;
        }

        public async Task InitializeAsync()
        {
            await this.surveyAnswersSummaryBlobContainer.EnsureExistsAsync().ConfigureAwait(false);
        }

        public async Task<SurveyAnswersSummary> GetSurveyAnswersSummaryAsync(string tenant, string slugName)
        {
            var id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", tenant, slugName);
            return await this.surveyAnswersSummaryBlobContainer.GetAsync(id).ConfigureAwait(false);
        }

        public async Task SaveSurveyAnswersSummaryAsync(SurveyAnswersSummary surveyAnswersSummary)
        {
            var id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", surveyAnswersSummary.Tenant, surveyAnswersSummary.SlugName);
            await this.surveyAnswersSummaryBlobContainer.SaveAsync(id, surveyAnswersSummary).ConfigureAwait(false);
        }

        public async Task MergeSurveyAnswersSummaryAsync(SurveyAnswersSummary partialSurveyAnswersSummary)
        {
            var id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", partialSurveyAnswersSummary.Tenant, partialSurveyAnswersSummary.SlugName);
            var surveyAnswersSummaryInStore = await this.surveyAnswersSummaryBlobContainer.GetAsync(id).ConfigureAwait(false);
            partialSurveyAnswersSummary.MergeWith(surveyAnswersSummaryInStore);
            await this.surveyAnswersSummaryBlobContainer.SaveAsync(id, partialSurveyAnswersSummary).ConfigureAwait(false);
        }

        public async Task DeleteSurveyAnswersSummaryAsync(string tenant, string slugName)
        {
            var id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", tenant, slugName);
            await this.surveyAnswersSummaryBlobContainer.DeleteAsync(id).ConfigureAwait(false);
        }
    }
}