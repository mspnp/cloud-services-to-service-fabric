namespace Tailspin.SurveyAnalysisService.Client
{
    using System.Threading.Tasks;
    using Tailspin.Shared.Models.Client;

    public interface ISurveyAnalysisService 
    {
        Task MergeSurveyAnswerToAnalysisAsync(SurveyAnswer surveyAnswer);

        Task<SurveyAnswersSummary> GetSurveyAnswersSummaryAsync(string slugName);
    }
}
