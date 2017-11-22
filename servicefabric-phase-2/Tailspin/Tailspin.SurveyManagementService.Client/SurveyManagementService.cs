namespace Tailspin.SurveyManagementService.Client
{
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Tailspin.Shared;
    using Tailspin.Shared.Models.Client;

    public class SurveyManagementService : ISurveyManagementService
    {
        private static readonly HttpClient httpClient;

        static SurveyManagementService()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(
                    "http://localhost:19081/Tailspin/SurveyManagementService/")
            };
        }

        public async Task<ICollection<SurveyInformation>> GetLatestSurveysAsync()
        {
            ICollection<SurveyInformation> latestSurveys = null;

            HttpResponseMessage response = await httpClient.GetAsync($"api/surveys/latest");
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            latestSurveys = JsonConvert.DeserializeObject<ICollection<SurveyInformation>>(content);

            return latestSurveys;
        }

        public async Task<Survey> GetSurveyAsync(string slugName)
        {
            Survey survey = null;

            HttpResponseMessage response = await httpClient.GetAsync($"api/surveys/{slugName}");
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            survey = JsonConvert.DeserializeObject<Survey>(content);

            return survey;
        }

        public async Task<ICollection<SurveyInformation>> ListSurveysAsync()
        {
            ICollection<SurveyInformation> surveys = null;

            HttpResponseMessage response = await httpClient.GetAsync($"api/surveys");
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            surveys = JsonConvert.DeserializeObject<ICollection<SurveyInformation>>(content);

            return surveys;
        }

        public async Task<SurveyInformation> PublishSurveyAsync(Survey survey)
        {
            SurveyInformation surveyInformation = null;

            var jsonSurvey = JsonConvert.SerializeObject(survey);

            HttpResponseMessage response = await httpClient.PostAsync($"api/surveys", new StringContent(jsonSurvey, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            surveyInformation = JsonConvert.DeserializeObject<SurveyInformation>(content);

            return surveyInformation;
        }
    }
}
