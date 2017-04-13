namespace Tailspin.SurveyManagementService.Models
{
    using System;
    using System.Linq;
    using Tailspin.SurveyManagementService.Client.Models;

    internal static class MappingExtensions
    {
        internal static SurveyInformationRow ToSurveyRow(this Client.Models.Survey survey, string partitionKey)
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

        internal static Models.Survey ToSurvey(this Client.Models.Survey survey)
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

        internal static Client.Models.Survey ToSurvey(this Models.Survey survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            return new Client.Models.Survey()
            {
                CreatedOn = survey.CreatedOn,
                Questions = survey.Questions.Select(q => q.ToQuestion()).ToList(),
                SlugName = survey.SlugName,
                Title = survey.Title
            };
        }

        internal static Models.Question ToQuestion(this Client.Models.Question question)
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

        internal static Client.Models.Question ToQuestion(this Models.Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            return new Client.Models.Question()
            {
                PossibleAnswers = question.PossibleAnswers,
                Text = question.Text,
                Type = question.Type.ToQuestionType()
            };
        }

        private static string ToQuestionType(this Client.Models.QuestionType questionType)
        {
            return Enum.GetName(typeof(Client.Models.QuestionType), questionType);
        }

        private static Client.Models.QuestionType ToQuestionType(this string questionType)
        {
            return (Client.Models.QuestionType)Enum.Parse(typeof(Client.Models.QuestionType), questionType);
        }

        internal static Client.Models.SurveyInformation ToSurveyInformation(this Models.SurveyInformationRow surveyRow)
        {
            if (surveyRow == null)
            {
                throw new ArgumentNullException(nameof(surveyRow));
            }

            return new SurveyInformation()
            {
                CreatedOn = surveyRow.CreatedOn,
                SlugName = surveyRow.SlugName,
                Title = surveyRow.Title
            };
        }
    }
}
