namespace Tailspin.SurveyAnswerService.Client
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

    public class SurveyAnswerService : ISurveyAnswerService
    {
        private static readonly HttpClient httpClient;

        static SurveyAnswerService()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(
                    "http://localhost:19081/Tailspin/SurveyAnswerService/")
            };
        }

        public async Task<SurveyAnswerBrowsingContext> GetSurveyAnswerBrowsingContextAsync(string slugName, string answerId)
        {
            SurveyAnswerBrowsingContext browsingContext = null;

            HttpResponseMessage response = await httpClient.GetAsync($"api/surveyanswers?slugName={slugName}&answerId={answerId}");
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            browsingContext = JsonConvert.DeserializeObject<SurveyAnswerBrowsingContext>(content);

            return browsingContext;
        }
    }
}
