using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tailspin.Shared;
using Tailspin.Shared.Models.Client;

namespace Tailspin.SurveyResponseService.Client
{
    public class SurveyResponseService : ISurveyResponseService
    {
        private static readonly HttpClient httpClient;
        private static readonly string defaultPartitionKey;
        private static readonly string defaultPartitionKind;
        
        static SurveyResponseService()
        {
            httpClient = new HttpClient {
                BaseAddress = new Uri(
                    "http://localhost:19081/Tailspin/Tailspin.SurveyResponseService/")
            };
            defaultPartitionKey = "1";
            defaultPartitionKind = "Int64Range";
        }

        public async Task SaveSurveyResponseAsync(SurveyAnswer surveyAnswer)
        {
            var jsonSurveyAnswer = JsonConvert.SerializeObject(surveyAnswer);

            HttpResponseMessage response = await httpClient.PostAsync($"api/surveyresponses?PartitionKey={defaultPartitionKey}&PartitionKind={defaultPartitionKind}", new StringContent(jsonSurveyAnswer, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
