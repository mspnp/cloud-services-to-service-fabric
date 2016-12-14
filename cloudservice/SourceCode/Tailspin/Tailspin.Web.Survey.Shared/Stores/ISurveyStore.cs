namespace Tailspin.Web.Survey.Shared.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Extensibility;
    using Tailspin.Web.Survey.Shared.Models;

    public interface ISurveyStore
    {
        Task InitializeAsync();

        string GetStorageKeyFor(Survey survey);

        Task SaveSurveyAsync(Survey survey);
        Task DeleteSurveyByTenantAndSlugNameAsync(string tenant, string slugName);
        Task<Survey> GetSurveyByTenantAndSlugNameAsync(string tenant, string slugName, bool getQuestions);
        Task<IEnumerable<Survey>> GetSurveysByTenantAsync(string tenant);
        Task<IEnumerable<Survey>> GetRecentSurveysAsync();

        Task SaveSurveyExtensionAsync(Survey survey, IModelExtension extension);
        Task DeleteSurveyExtensionByTenantAndSlugNameAsync(string tenant, string slugName);
        Task<IModelExtension> GetSurveyExtensionByTenantAndSlugNameAsync(string tenant, string slugName, Type extensionType);
        Task<IEnumerable<IModelExtension>> GetSurveyExtensionsByTenantAsync(string tenant, Type extensionType);
    }
}