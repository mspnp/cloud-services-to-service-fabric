namespace Tailspin.Web.Survey.Shared.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using AzureStorage;
    using QueueMessages;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Models;
    using System.Threading.Tasks;

    public class SurveyAnswerStore : ISurveyAnswerStore
    {
        private readonly ITenantStore tenantStore;
        private readonly ISurveyAnswerContainerFactory surveyAnswerContainerFactory;
        private readonly IAzureQueue<SurveyAnswerStoredMessage> standardSurveyAnswerStoredQueue;
        private readonly IAzureQueue<SurveyAnswerStoredMessage> premiumSurveyAnswerStoredQueue;
        private readonly IAzureBlobContainer<List<string>> surveyAnswerIdsListContainer;

        public SurveyAnswerStore(
            ITenantStore tenantStore, 
            ISurveyAnswerContainerFactory surveyAnswerContainerFactory,
            IAzureQueue<SurveyAnswerStoredMessage> standardSurveyAnswerStoredQueue, 
            IAzureQueue<SurveyAnswerStoredMessage> premiumSurveyAnswerStoredQueue, 
            IAzureBlobContainer<List<string>> surveyAnswerIdsListContainer)
        {
            this.tenantStore = tenantStore;
            this.surveyAnswerContainerFactory = surveyAnswerContainerFactory;
            this.standardSurveyAnswerStoredQueue = standardSurveyAnswerStoredQueue;
            this.premiumSurveyAnswerStoredQueue = premiumSurveyAnswerStoredQueue;
            this.surveyAnswerIdsListContainer = surveyAnswerIdsListContainer;
        }
        
        public async Task InitializeAsync()
        {
            await this.surveyAnswerIdsListContainer.EnsureExistsAsync().ConfigureAwait(false);
            await this.premiumSurveyAnswerStoredQueue.EnsureExistsAsync().ConfigureAwait(false);
            await this.standardSurveyAnswerStoredQueue.EnsureExistsAsync().ConfigureAwait(false);
        }

        public async Task SaveSurveyAnswerAsync(SurveyAnswer surveyAnswer)
        {
            var tenant = await this.tenantStore.GetTenantAsync(surveyAnswer.TenantId).ConfigureAwait(false);
            if (tenant != null)
            {
                var surveyAnswerBlobContainer = this.surveyAnswerContainerFactory.Create(surveyAnswer.TenantId, surveyAnswer.SlugName);
                await surveyAnswerBlobContainer.EnsureExistsAsync().ConfigureAwait(false);

                surveyAnswer.CreatedOn = DateTime.UtcNow;
                var blobId = surveyAnswer.CreatedOn.GetFormatedTicks();
                await surveyAnswerBlobContainer.SaveAsync(blobId, surveyAnswer).ConfigureAwait(false);

                var queue = SubscriptionKind.Premium.Equals(tenant.SubscriptionKind)
                    ? this.premiumSurveyAnswerStoredQueue
                    : this.standardSurveyAnswerStoredQueue;

                 await queue.AddMessageAsync(new SurveyAnswerStoredMessage
                    {
                        SurveyAnswerBlobId = blobId,
                        TenantId = surveyAnswer.TenantId,
                        SurveySlugName = surveyAnswer.SlugName
                    }).ConfigureAwait(false);
            }
        }

        public async Task<SurveyAnswer> GetSurveyAnswerAsync(string tenant, string slugName, string surveyAnswerId)
        {
            var surveyBlobContainer = this.surveyAnswerContainerFactory.Create(tenant, slugName);
            await surveyBlobContainer.EnsureExistsAsync().ConfigureAwait(false);
            return await surveyBlobContainer.GetAsync(surveyAnswerId).ConfigureAwait(false);
        }

        public async Task<string> GetFirstSurveyAnswerIdAsync(string tenant, string slugName)
        {
            string id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", tenant, slugName);
            var answerIdList = await this.surveyAnswerIdsListContainer.GetAsync(id).ConfigureAwait(false);

            if (answerIdList != null)
            {
                return answerIdList[0];
            }

            return string.Empty;
        }

        public async Task AppendSurveyAnswerIdToAnswersListAsync(string tenant, string slugName, string surveyAnswerId)
        {
            string id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", tenant, slugName);
            var answerIdList = await this.surveyAnswerIdsListContainer.GetAsync(id).ConfigureAwait(false) ?? new List<string>(1);
            answerIdList.Add(surveyAnswerId);
            await this.surveyAnswerIdsListContainer.SaveAsync(id, answerIdList).ConfigureAwait(false);
        }

        public async Task<SurveyAnswerBrowsingContext> GetSurveyAnswerBrowsingContextAsync(string tenant, string slugName, string answerId)
        {
            string id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", tenant, slugName);
            var answerIdsList = await this.surveyAnswerIdsListContainer.GetAsync(id).ConfigureAwait(false);

            string previousId = null;
            string nextId = null;
            if (answerIdsList != null)
            {
                var currentAnswerIndex = answerIdsList.FindIndex(i => i == answerId);

                if (currentAnswerIndex - 1 >= 0)
                {
                    previousId = answerIdsList[currentAnswerIndex - 1];
                }

                if (currentAnswerIndex + 1 <= answerIdsList.Count - 1)
                {
                    nextId = answerIdsList[currentAnswerIndex + 1];
                }
            }

            return new SurveyAnswerBrowsingContext
                       {
                           PreviousId = previousId,
                           NextId = nextId
                       };
        }

        public async Task<IEnumerable<string>> GetSurveyAnswerIdsAsync(string tenant, string slugName)
        {
            string id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", tenant, slugName);
            return await this.surveyAnswerIdsListContainer.GetAsync(id).ConfigureAwait(false);
        }

        public async Task DeleteSurveyAnswersAsync(string tenant, string slugName)
        {
            var surveyBlobContainer = this.surveyAnswerContainerFactory.Create(tenant, slugName);
            await surveyBlobContainer.DeleteContainerAsync().ConfigureAwait(false);

            string id = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", tenant, slugName);
            await this.surveyAnswerIdsListContainer.DeleteAsync(id).ConfigureAwait(false);
        }
    }
}