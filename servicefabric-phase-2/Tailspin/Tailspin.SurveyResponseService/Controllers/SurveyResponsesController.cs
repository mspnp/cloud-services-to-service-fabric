using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClientModels = Tailspin.Shared.Models.Client;
using ApiModels = Tailspin.Shared.Models.Api;
using Tailspin.SurveyAnalysisService.Client;
using Tailspin.SurveyResponseService.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data;

namespace Tailspin.SurveyResponseService.Controllers
{
    [Route("api/[controller]")]
    public class SurveyResponsesController : Controller
    {
        private static string SurveyAnswersListContainerName = "surveyanswerslist";

        AzureBlobContainerFactory<ApiModels.SurveyAnswer> surveyAnswerContainerFactory;
        AzureBlobContainerFactory<List<string>> surveyAnswerListContainerFactory;
        ISurveyAnalysisService surveyAnalysisService;

        private readonly IReliableStateManager _stateManager;

        public SurveyResponsesController(
            AzureBlobContainerFactory<ApiModels.SurveyAnswer> surveyAnswerContainerFactory,
            AzureBlobContainerFactory<List<string>> surveyAnswerListContainerFactory,
            ISurveyAnalysisService surveyAnalysisService,
            IReliableStateManager stateManager)
        {
            this.surveyAnswerContainerFactory = surveyAnswerContainerFactory;
            this.surveyAnswerListContainerFactory = surveyAnswerListContainerFactory;
            this.surveyAnalysisService = surveyAnalysisService;
            _stateManager = stateManager;
        }

        // POST api/surveyresponses
        [HttpPost]
        public async Task SaveSurveyResponseAsync([FromBody]ClientModels.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var queue = await _stateManager.GetOrAddAsync<IReliableConcurrentQueue<ClientModels.SurveyAnswer>>("surveyQueue");

                    if (surveyAnswer.Id == null)
                    {
                        surveyAnswer.Id = Guid.NewGuid().ToString();
                    }

                    using (var tx = _stateManager.CreateTransaction())
                    {
                        await queue.EnqueueAsync(tx, surveyAnswer);
                        await tx.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.ServiceRequestFailed(ex.ToString());
                    throw;
                }
            }
        }
    }
}
