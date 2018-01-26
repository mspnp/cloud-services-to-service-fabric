namespace Tailspin.SurveyAnswerService.Client
{
    using System.Threading.Tasks;
    using Tailspin.Shared.Models.Client;

    public interface ISurveyAnswerService
    {
        Task<SurveyAnswerBrowsingContext> GetSurveyAnswerBrowsingContextAsync(string slugName, string answerId);
    }
}
