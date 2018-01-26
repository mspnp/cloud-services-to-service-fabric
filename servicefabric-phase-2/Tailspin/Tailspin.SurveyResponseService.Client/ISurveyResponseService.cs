using System;
using System.Threading.Tasks;
using Tailspin.Shared.Models.Client;

namespace Tailspin.SurveyResponseService.Client
{
    public interface ISurveyResponseService
    {
        Task SaveSurveyResponseAsync(SurveyAnswer surveyAnswer);
    }
}
