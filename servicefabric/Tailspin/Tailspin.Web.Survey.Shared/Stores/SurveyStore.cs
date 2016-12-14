namespace Tailspin.Web.Survey.Shared.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Table;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using Microsoft.WindowsAzure.Storage;

    public class SurveyStore : ISurveyStore
    {
        private readonly IAzureTable<SurveyRow> surveyTable;
        private readonly IAzureTable<QuestionRow> questionTable;

        public SurveyStore(
            IAzureTable<SurveyRow> surveyTable, 
            IAzureTable<QuestionRow> questionTable,
            IInitializationStatusService initializationStatusService)
        {
            this.surveyTable = surveyTable;
            this.questionTable = questionTable;
            this.CacheEnabled = initializationStatusService.IsInitialized;
        }

        private bool CacheEnabled { get; set; }

        public async Task InitializeAsync()
        {
            await this.surveyTable.EnsureExistsAsync();
            await this.questionTable.EnsureExistsAsync();
        }

        public string GetStorageKeyFor(Survey survey)
        {
            var slugName = string.IsNullOrEmpty(survey.SlugName) ? GenerateSlug(survey.Title, 100) : survey.SlugName;
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", survey.TenantId, slugName);
        }

        public async Task SaveSurveyAsync(Survey survey)
        {
            if (string.IsNullOrEmpty(survey.SlugName) && string.IsNullOrEmpty(survey.Title))
            {
                throw new ArgumentNullException("survey", "The survey for saving has to have the slug or the title.");
            }

            var slugName = string.IsNullOrEmpty(survey.SlugName) ? GenerateSlug(survey.Title, 100) : survey.SlugName;

            var surveyRow = new SurveyRow
                                {
                                    SlugName = slugName,
                                    Title = survey.Title,
                                    CreatedOn = DateTime.UtcNow,
                                    PartitionKey = survey.TenantId
                                };

            surveyRow.RowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", survey.TenantId, surveyRow.SlugName);

            var questionRows = new List<QuestionRow>(survey.Questions.Count);
            for (int i = 0; i < survey.Questions.Count; i++)
            {
                var question = survey.Questions[i];
                var questionRow = new QuestionRow
                                      {
                                          Text = question.Text,
                                          Type = Enum.GetName(typeof(QuestionType), question.Type),
                                          PossibleAnswers = question.PossibleAnswers
                                      };

                questionRow.PartitionKey = surveyRow.RowKey;
                questionRow.RowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", DateTime.UtcNow.GetFormatedTicks(), i.ToString("D3"));

                questionRows.Add(questionRow);
            }

            //// First add the questions
            await this.questionTable.AddAsync(questionRows);

            try
            {
                //// Then add the survey
                //// If this fails, the questions may end up orphan but data will be consistent for the user
                await this.surveyTable.AddAsync(surveyRow);

                if (this.CacheEnabled)
                {
                    await TenantCacheHelper.AddToCacheAsync(survey.TenantId, slugName, survey).ConfigureAwait(false);
                }
            }

            catch (StorageException ex)
            {
                TraceHelper.TraceError(ex.TraceInformation());

                await this.questionTable.DeleteAsync(questionRows);
                throw;
            }
        }

        public async Task DeleteSurveyByTenantAndSlugNameAsync(string tenant, string slugName)
        {
            var surveyRow = await GetSurveyRowByTenantAndSlugNameAsync(this.surveyTable, tenant, slugName);

            if (surveyRow == null)
            {
                return;
            }

            await this.surveyTable.DeleteAsync(surveyRow);

            var surveyQuestionRows = await GetSurveyQuestionRowsByTenantAndSlugNameAsync(this.questionTable, tenant, slugName);
            await this.questionTable.DeleteAsync(surveyQuestionRows);

            if (this.CacheEnabled)
            {
                await TenantCacheHelper.RemoveFromCacheAsync(tenant, slugName).ConfigureAwait(false);
            }
        }

        public async Task<Survey> GetSurveyByTenantAndSlugNameAsync(string tenant, string slugName, bool getQuestions)
        {
            Func<Task<Survey>> resolver = async () =>
            {
                var surveyRow = await GetSurveyRowByTenantAndSlugNameAsync(this.surveyTable, tenant, slugName);

                if (surveyRow == null)
                {
                    return null;
                }

                var survey = new Survey(surveyRow.SlugName)
                {
                    TenantId = surveyRow.PartitionKey,
                    Title = surveyRow.Title,
                    CreatedOn = surveyRow.CreatedOn
                };

                if (getQuestions)
                {
                    var surveyQuestionRows = await GetSurveyQuestionRowsByTenantAndSlugNameAsync(this.questionTable, tenant, slugName);
                    foreach (var questionRow in surveyQuestionRows)
                    {
                        survey.Questions.Add(
                            new Question
                            {
                                Text = questionRow.Text,
                                Type = (QuestionType)Enum.Parse(typeof(QuestionType), questionRow.Type),
                                PossibleAnswers = questionRow.PossibleAnswers
                            });
                    }
                }

                return survey;
            };

            if (this.CacheEnabled)
            {
                return await TenantCacheHelper.GetFromCacheAsync(tenant, slugName, resolver).ConfigureAwait(false);
            }
            else
            {
                return await resolver().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Survey>> GetSurveysByTenantAsync(string tenant)
        {
            var surveys = await this.surveyTable.GetByPartitionKeyAsync(tenant);

            return surveys.Select(surveyRow => new Survey(surveyRow.SlugName)
                {
                    TenantId = surveyRow.PartitionKey,
                    Title = surveyRow.Title,
                    CreatedOn = surveyRow.CreatedOn
                });
        }

        public async Task<IEnumerable<Survey>> GetRecentSurveysAsync()
        {
            var surveys = await this.surveyTable.GetLatestAsync(10);

            return surveys.Select(surveyRow => new Survey(surveyRow.SlugName)
                {
                    TenantId = surveyRow.PartitionKey,
                    Title = surveyRow.Title
                });
        }

        private static async Task<SurveyRow> GetSurveyRowByTenantAndSlugNameAsync(IAzureTable<SurveyRow> surveyTable, string tenant, string slugName)
        {
            var rowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", tenant, slugName);

            var surveys = await surveyTable.GetByRowKeyAsync(rowKey);
            if (surveys == null) return null;

            return surveys.SingleOrDefault();
        }

        private static async Task<IEnumerable<QuestionRow>> GetSurveyQuestionRowsByTenantAndSlugNameAsync(IAzureTable<QuestionRow> questionTable, string tenant, string slugName)
        {
            var paritionKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", tenant, slugName);

            return await questionTable.GetByPartitionKeyAsync(paritionKey);
        }

        private static string GenerateSlug(string txt, int maxLength)
        {
            string str = RemoveAccent(txt).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", string.Empty);
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }

        private static string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private class DummyExtension : TableEntity { }
    }
}