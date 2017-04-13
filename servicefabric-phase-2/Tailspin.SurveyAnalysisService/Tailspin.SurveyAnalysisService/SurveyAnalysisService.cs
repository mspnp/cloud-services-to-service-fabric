namespace Tailspin.SurveyAnalysisService
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using Tailspin.SurveyAnalysisService.Models;
    using Tailspin.SurveyAnalysisService.Client;
    using System.Linq;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public sealed class SurveyAnalysisService : StatelessService, ISurveyAnalysisService
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

        public SurveyAnalysisService(StatelessServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context => this.CreateServiceRemotingListener(context)) };
        }

        public async Task MergeSurveyAnswerToAnalysisAsync(Client.Models.SurveyAnswer surveyAnswer)
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
                throw new SurveyAnalysisServiceException();
            }
        }

        public async Task<Client.Models.SurveyAnswersSummary> GetSurveyAnswersSummaryAsync(string slugName)
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
                throw new SurveyAnalysisServiceException();
            }
        }
    }
}

