namespace Tailspin.SurveyAnswerService.Client
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Tailspin.SurveyAnswerService.Client.Models;

    public interface ISurveyAnswerService : IService
    {
        Task SaveSurveyAnswerAsync(SurveyAnswer surveyAnswer);
        Task<SurveyAnswerBrowsingContext> GetSurveyAnswerBrowsingContextAsync(string slugName, string answerId);
    }
}
