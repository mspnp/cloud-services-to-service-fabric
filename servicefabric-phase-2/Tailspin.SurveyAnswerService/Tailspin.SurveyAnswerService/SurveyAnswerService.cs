namespace Tailspin.SurveyAnswerService
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyAnswerService.Client.Models;
    using Tailspin.SurveyAnswerService.Models;
    using Tailspin.SurveyAnalysisService.Client;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public sealed class SurveyAnswerService : StatelessService, ISurveyAnswerService
    {
        private static string SurveyAnswersListContainerName = "surveyanswerslist";

        AzureBlobContainerFactory<Models.SurveyAnswer> surveyAnswerContainerFactory;
        AzureBlobContainerFactory<List<string>> surveyAnswerListContainerFactory;
        ISurveyAnalysisService surveyAnalysisService;

        public SurveyAnswerService(StatelessServiceContext context,
            AzureBlobContainerFactory<Models.SurveyAnswer> surveyAnswerContainerFactory,
            AzureBlobContainerFactory<List<string>> surveyAnswerListContainerFactory,
            ISurveyAnalysisService surveyAnalysisService)
            : base(context)
        {
            this.surveyAnswerContainerFactory = surveyAnswerContainerFactory;
            this.surveyAnswerListContainerFactory = surveyAnswerListContainerFactory;
            this.surveyAnalysisService = surveyAnalysisService;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context => this.CreateServiceRemotingListener(context)) };
        }

        public async Task SaveSurveyAnswerAsync(Client.Models.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            try
            {
                var model = surveyAnswer.ToSurveyAnswer();
                model.CreatedOn = DateTime.UtcNow;
                var container = surveyAnswerContainerFactory($"{model.SlugName}-answers");
                await container.EnsureExistsAsync();
                var surveyId = $"{model.CreatedOn.Ticks:D19}";
                await container.SaveAsync(surveyId, model);
                await AppendSurveyAnswerIdToSurveyAnswerListAsync(model.SlugName, surveyId);

                await surveyAnalysisService.MergeSurveyAnswerToAnalysisAsync(model.ToAnalysisServiceSurveyAnswer());
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceRequestFailed(ex.ToString());
                throw new SurveyAnswerServiceException();
            }
        }

        public async Task<SurveyAnswerBrowsingContext> GetSurveyAnswerBrowsingContextAsync(string slugName, string answerId)
        {
            if (string.IsNullOrWhiteSpace(slugName))
            {
                throw new ArgumentException($"{nameof(slugName)} cannot be null, empty, or only whitespace");
            }

            try
            {
                var containerName = $"{slugName}-answers";
                var container = surveyAnswerContainerFactory(containerName);
                var surveyAnswerList = await GetSurveyAnswerListAsync(slugName);
                var browsingContext = new SurveyAnswerBrowsingContext();
                if (surveyAnswerList.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(answerId))
                    {
                        answerId = surveyAnswerList[0];
                    }

                    if (await container.ExistsAsync())
                    {
                        var previousAnswerId = surveyAnswerList
                            .TakeWhile(s => string.Compare(s, answerId) != 0)
                            .LastOrDefault();
                        var nextAnswerId = surveyAnswerList
                            .SkipWhile(s => string.Compare(s, answerId) != 0)
                            .Skip(1)
                            .FirstOrDefault();
                        var surveyAnswer = await container.GetAsync(answerId);

                        browsingContext.NextAnswerId = nextAnswerId;
                        browsingContext.PreviousAnswerId = previousAnswerId;
                        browsingContext.SurveyAnswer = surveyAnswer?.ToSurveyAnswer();
                    }
                }

                return browsingContext;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceRequestFailed(ex.ToString());
                throw new SurveyAnswerServiceException();
            }
        }

        private async Task AppendSurveyAnswerIdToSurveyAnswerListAsync(string slugName, string surveyAnswerId)
        {
            if (string.IsNullOrWhiteSpace(slugName))
            {
                throw new ArgumentException($"{nameof(slugName)} cannot be null, empty, or only whitespace");
            }

            if (string.IsNullOrWhiteSpace(surveyAnswerId))
            {
                throw new ArgumentException($"{nameof(surveyAnswerId)} cannot be null, empty, or only whitespace");
            }

            var answerListContainer = surveyAnswerListContainerFactory(SurveyAnswersListContainerName);
            await answerListContainer.EnsureExistsAsync();
            var answerIdList = await answerListContainer.GetAsync(slugName)
                .ConfigureAwait(false) ?? new List<string>();
            answerIdList.Add(surveyAnswerId);
            await answerListContainer.SaveAsync(slugName, answerIdList)
                .ConfigureAwait(false);
        }

        private async Task<List<string>> GetSurveyAnswerListAsync(string slugName)
        {
            if (string.IsNullOrWhiteSpace(slugName))
            {
                throw new ArgumentException($"{nameof(slugName)} cannot be null, empty, or only whitespace");
            }

            var answerListContainer = surveyAnswerListContainerFactory(SurveyAnswersListContainerName);
            var answerIdList = await answerListContainer.GetAsync(slugName)
                .ConfigureAwait(false) ?? new List<string>();
            return answerIdList;
        }
    }
}
