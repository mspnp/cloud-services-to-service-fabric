using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClientModels = Tailspin.Shared.Models.Client;
using ApiModels = Tailspin.Shared.Models.Api;
using Tailspin.SurveyAnswerService.Models;
using Tailspin.SurveyAnalysisService.Client;

namespace Tailspin.SurveyAnswerService.Controllers
{
    [Route("api/[controller]")]
    public class SurveyAnswersController : Controller
    {
        private static string SurveyAnswersListContainerName = "surveyanswerslist";

        AzureBlobContainerFactory<ApiModels.SurveyAnswer> surveyAnswerContainerFactory;
        AzureBlobContainerFactory<List<string>> surveyAnswerListContainerFactory;
        ISurveyAnalysisService surveyAnalysisService;

        public SurveyAnswersController(
            AzureBlobContainerFactory<ApiModels.SurveyAnswer> surveyAnswerContainerFactory,
            AzureBlobContainerFactory<List<string>> surveyAnswerListContainerFactory,
            ISurveyAnalysisService surveyAnalysisService)
        {
            this.surveyAnswerContainerFactory = surveyAnswerContainerFactory;
            this.surveyAnswerListContainerFactory = surveyAnswerListContainerFactory;
            this.surveyAnalysisService = surveyAnalysisService;
        }

        // GET api/surveyanswer
        [HttpGet]
        public async Task<ClientModels.SurveyAnswerBrowsingContext> GetSurveyAnswerBrowsingContextAsync(string slugName, string answerId)
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
                var browsingContext = new ClientModels.SurveyAnswerBrowsingContext();
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
                throw;
            }
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
