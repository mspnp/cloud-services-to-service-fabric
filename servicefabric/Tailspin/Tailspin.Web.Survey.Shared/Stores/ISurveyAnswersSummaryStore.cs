namespace Tailspin.Web.Survey.Shared.Stores
{
    using Models;
    using System.Threading.Tasks;

    public interface ISurveyAnswersSummaryStore
    {
        Task InitializeAsync();
        Task<SurveyAnswersSummary> GetSurveyAnswersSummaryAsync(string tenant, string slugName);        
        Task DeleteSurveyAnswersSummaryAsync(string tenant, string slugName);
        Task SaveSurveyAnswersSummaryAsync(SurveyAnswersSummary surveyAnswersSummary);
        Task MergeSurveyAnswersSummaryAsync(SurveyAnswersSummary partialSurveyAnswersSummary);
    }
}
