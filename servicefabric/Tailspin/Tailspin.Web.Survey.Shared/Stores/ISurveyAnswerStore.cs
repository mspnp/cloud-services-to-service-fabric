namespace Tailspin.Web.Survey.Shared.Stores
{
    using System.Collections.Generic;
    using Models;
    using System.Threading.Tasks;

    public interface ISurveyAnswerStore
    {
        Task InitializeAsync();
        Task SaveSurveyAnswerAsync(SurveyAnswer surveyAnswer);
        Task<SurveyAnswer> GetSurveyAnswerAsync(string tenant, string slugName, string surveyAnswerId);
        Task<string> GetFirstSurveyAnswerIdAsync(string tenant, string slugName);
        Task AppendSurveyAnswerIdToAnswersListAsync(string tenant, string slugName, string surveyAnswerId);
        Task<SurveyAnswerBrowsingContext> GetSurveyAnswerBrowsingContextAsync(string tenant, string slugName, string answerId);
        Task<IEnumerable<string>> GetSurveyAnswerIdsAsync(string tenant, string slugName);
        Task DeleteSurveyAnswersAsync(string tenant, string slugName);
    }
}