namespace Tailspin.Web.Survey.Shared.Stores
{
    using AzureStorage;
    using Tailspin.Web.Survey.Shared.Models;

    public interface ISurveyAnswerContainerFactory
    {
        IAzureBlobContainer<SurveyAnswer> Create(string tenant, string surveySlug);
    }
}