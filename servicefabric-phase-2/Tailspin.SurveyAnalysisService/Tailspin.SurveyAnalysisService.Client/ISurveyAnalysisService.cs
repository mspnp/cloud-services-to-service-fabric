namespace Tailspin.SurveyAnalysisService.Client
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Tailspin.SurveyAnalysisService.Client.Models;

    public interface ISurveyAnalysisService : IService
    {
        Task MergeSurveyAnswerToAnalysisAsync(SurveyAnswer surveyAnswer);

        Task<SurveyAnswersSummary> GetSurveyAnswersSummaryAsync(string slugName);
    }
}
