namespace Tailspin.SurveyManagementService.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tailspin.Shared.Models.Client;

    public interface ISurveyManagementService
    {
        Task<ICollection<SurveyInformation>> GetLatestSurveysAsync();
        Task<Survey> GetSurveyAsync(string slugName);
        Task<ICollection<SurveyInformation>> ListSurveysAsync();
        Task<SurveyInformation> PublishSurveyAsync(Survey survey);
    }
}
