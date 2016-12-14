namespace Tailspin.Web.Survey.Shared.Stores
{
    using Models;

    public interface ISurveySqlStore
    {
        void SaveSurvey(string connectionString, SurveyData surveyData);

        void Reset(string connectionString, string tenant, string slugName);
    }
}