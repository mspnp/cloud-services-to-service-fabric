using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Tailspin.Web.Shared.Helpers;
using Newtonsoft.Json;
using Tailspin.SurveyAnalysisService.Models;
using ClientModels = Tailspin.Shared.Models.Client;

namespace Tailspin.SurveyAnalysisService.Controllers
{
    [Route("api/[controller]")]
    public class AnalysisController : Controller
    {
        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ServiceFabricConfiguration.GetConfigurationSettingValue("ConnectionStrings", "RedisCacheConnectionString", "YourRedisCacheConnectionString");

            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        [HttpPost]
        public async Task MergeSurveyAnswerToAnalysisAsync([FromBody]ClientModels.SurveyAnswer surveyAnswer)
        {
            try
            {
                var surveyAnswersSummaryCache = Connection.GetDatabase();
                var success = false;

                do
                {
                    var result = await surveyAnswersSummaryCache.StringGetAsync(surveyAnswer.SlugName);
                    var isNew = result.IsNullOrEmpty;
                    var transaction = surveyAnswersSummaryCache.CreateTransaction();

                    SurveyAnswersSummary surveyAnswersSummary = null;

                    if (isNew)
                    {
                        transaction.AddCondition(Condition.KeyNotExists(surveyAnswer.SlugName));
                        surveyAnswersSummary = new SurveyAnswersSummary(surveyAnswer.SlugName);
                    }
                    else
                    {
                        surveyAnswersSummary = JsonConvert.DeserializeObject<SurveyAnswersSummary>(result);
                        transaction.AddCondition(Condition.StringEqual(surveyAnswer.SlugName, result));
                    }

                    ServiceEventSource.Current.Message("Slug name:{0}|Total answers:{1}", surveyAnswersSummary.SlugName,
                        surveyAnswersSummary.TotalAnswers);

                    // Add and merge the new answer to new or existing summary
                    surveyAnswersSummary.AddNewAnswer(surveyAnswer.ToSurveyAnswer());

                    transaction.StringSetAsync(surveyAnswer.SlugName,
                            JsonConvert.SerializeObject(surveyAnswersSummary));

                    //This is a simple implementation of optimistic concurrency.
                    //If transaction fails, another user must have edited the same survey.
                    //If so, try to process this survey answer again.

                    //Another approach is to store each survey answer option as a separate hash in Redis and simply increment
                    //the hash value. This technique will not cause collisions with other threads but will require a redesign of
                    //how survey answer summaries are stored in redis. This approach is left to the reader to implement.

                    success = await transaction.ExecuteAsync();
                } while (!success);

            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceRequestFailed(ex.ToString());
                throw;
            }
        }

        [HttpGet]
        [Route("Summaries/{slugName}")]
        public async Task<ClientModels.SurveyAnswersSummary> GetSurveyAnswersSummaryAsync(string slugName)
        {
            try
            {
                var surveyAnswersSummaryCache = Connection.GetDatabase();

                // Look for slug name in the survey answers summary cache
                var surveyAnswersSummaryInStore = await surveyAnswersSummaryCache.StringGetAsync(slugName);

                if (!surveyAnswersSummaryInStore.IsNullOrEmpty)
                {
                    var returnData =
                        JsonConvert.DeserializeObject<SurveyAnswersSummary>(surveyAnswersSummaryInStore)
                            .ToSurveyAnswersSummary();

                    return returnData;
                }

                return null;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceRequestFailed(ex.ToString());
                throw;
            }
        }
    }
}
