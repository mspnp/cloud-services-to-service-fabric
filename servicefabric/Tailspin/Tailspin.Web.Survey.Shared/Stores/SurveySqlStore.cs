namespace Tailspin.Web.Survey.Shared.Stores
{
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using Models;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Stores.AzureSql;

    public class SurveySqlStore : AzureSqlWithRetryPolicy, ISurveySqlStore
    {
        public void SaveSurvey(string connectionString, SurveyData surveyData)
        {
            this.ConnectionRetryPolicy.ExecuteAction(() =>
            {
                using (var dataContext = new SurveySqlDataContext(connectionString))
                {
                    dataContext.SurveyDatas.InsertOnSubmit(surveyData);
                    try
                    {
                        this.CommandRetryPolicy.ExecuteAction(() => dataContext.SubmitChanges());
                    }
                    catch (SqlException ex)
                    {
                        TraceHelper.TraceError(ex.TraceInformation());
                        throw;
                    }
                }
            });
        }

        public void Reset(string connectionString, string tenant, string slugName)
        {
            this.ConnectionRetryPolicy.ExecuteAction(() =>
            {
                using (var dataContext = new SurveySqlDataContext(connectionString))
                {
                    var query = from survey in dataContext.SurveyDatas
                                where survey.Id == string.Format(CultureInfo.InvariantCulture, "{0}_{1}", tenant, slugName)
                                select survey;

                    foreach (var surveyData in query)
                    {
                        dataContext.SurveyDatas.DeleteOnSubmit(surveyData);
                    }

                    try
                    {
                        this.CommandRetryPolicy.ExecuteAction(() => dataContext.SubmitChanges());
                    }
                    catch (SqlException ex)
                    {
                        TraceHelper.TraceError(ex.TraceInformation());
                        throw;
                    }
                }
            });
        }
    }
}