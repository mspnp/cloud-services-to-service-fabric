using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Tailspin.SurveyManagementService.Client.Models;

namespace Tailspin.SurveyManagementService.Client
{
    public interface ISurveyManagementService : IService
    {
        Task<SurveyInformation> PublishSurveyAsync(Survey survey);
        Task<ICollection<SurveyInformation>> ListSurveysAsync();
        Task<Survey> GetSurveyAsync(string slugName);
        Task<ICollection<SurveyInformation>> GetLatestSurveysAsync();
    }
}
