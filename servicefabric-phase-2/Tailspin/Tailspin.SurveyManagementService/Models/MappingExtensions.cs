namespace Tailspin.SurveyManagementService.Models
{
    using System;
    using System.Linq;
    using ClientModels = Tailspin.Shared.Models.Client;

    internal static class MappingExtensions
    {
        internal static SurveyInformationRow ToSurveyRow(this ClientModels.Survey survey, string partitionKey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException($"{nameof(partitionKey)} cannot be null, empty, or only whitespace");
            }

            return new SurveyInformationRow()
            {
                CreatedOn = survey.CreatedOn,
                PartitionKey = partitionKey,
                // Store the rows in reverse DateTime order
                RowKey = $"{DateTime.MaxValue.Ticks - survey.CreatedOn.Ticks:D19}",
                SlugName = survey.SlugName,
                Title = survey.Title
            };
        }

        internal static Models.Survey ToSurvey(this ClientModels.Survey survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            return new Models.Survey()
            {
                CreatedOn = survey.CreatedOn,
                Questions = survey.Questions.Select(q => q.ToQuestion()).ToList(),
                SlugName = survey.SlugName,
                Title = survey.Title
            };
        }

        internal static ClientModels.Survey ToSurvey(this Models.Survey survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            return new ClientModels.Survey()
            {
                CreatedOn = survey.CreatedOn,
                Questions = survey.Questions.Select(q => q.ToQuestion()).ToList(),
                SlugName = survey.SlugName,
                Title = survey.Title
            };
        }

        internal static Models.Question ToQuestion(this ClientModels.Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            return new Models.Question()
            {
                PossibleAnswers = question.PossibleAnswers,
                Text = question.Text,
                Type = question.Type.ToQuestionType()
            };
        }

        internal static ClientModels.Question ToQuestion(this Models.Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            return new ClientModels.Question()
            {
                PossibleAnswers = question.PossibleAnswers,
                Text = question.Text,
                Type = question.Type.ToQuestionType()
            };
        }

        private static string ToQuestionType(this ClientModels.QuestionType questionType)
        {
            return Enum.GetName(typeof(ClientModels.QuestionType), questionType);
        }

        private static ClientModels.QuestionType ToQuestionType(this string questionType)
        {
            return (ClientModels.QuestionType)Enum.Parse(typeof(ClientModels.QuestionType), questionType);
        }

        internal static ClientModels.SurveyInformation ToSurveyInformation(this Models.SurveyInformationRow surveyRow)
        {
            if (surveyRow == null)
            {
                throw new ArgumentNullException(nameof(surveyRow));
            }

            return new ClientModels.SurveyInformation()
            {
                CreatedOn = surveyRow.CreatedOn,
                SlugName = surveyRow.SlugName,
                Title = surveyRow.Title
            };
        }
    }
}
