namespace Tailspin.SurveyAnalysisService.Client
{
    using System;
    using System.Fabric;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Newtonsoft.Json;
    using Tailspin.Shared;
    using Tailspin.Shared.Models.Client;

    public class SurveyAnalysisService : ISurveyAnalysisService
    {
        private static readonly HttpClient httpClient;

        static SurveyAnalysisService()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(
                    "http://localhost:19081/Tailspin/SurveyAnalysisService/")
            };
        }

        public async Task<SurveyAnswersSummary> GetSurveyAnswersSummaryAsync(string slugName)
        {
            SurveyAnswersSummary summary = null;

            HttpResponseMessage response = await httpClient.GetAsync($"api/Analysis/Summaries/{slugName}");
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            summary = JsonConvert.DeserializeObject<SurveyAnswersSummary>(content);

            return summary;
        }

        public async Task MergeSurveyAnswerToAnalysisAsync(SurveyAnswer surveyAnswer)
        {
            var jsonSurveyAnswer = JsonConvert.SerializeObject(surveyAnswer);

            HttpResponseMessage response = await httpClient.PostAsync($"api/Analysis", new StringContent(jsonSurveyAnswer, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
