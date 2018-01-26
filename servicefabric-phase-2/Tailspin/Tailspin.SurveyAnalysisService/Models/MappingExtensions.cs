namespace Tailspin.SurveyAnalysisService.Models
{
    using System;
    using System.Linq;
    using ClientModels = Tailspin.Shared.Models.Client;

    internal static class MappingExtensions
    {
        private static string ToQuestionType(this ClientModels.QuestionType questionType)
        {
            return Enum.GetName(typeof(ClientModels.QuestionType), questionType);
        }

        private static ClientModels.QuestionType ToQuestionType(this string questionType)
        {
            return (ClientModels.QuestionType)Enum.Parse(typeof(ClientModels.QuestionType), questionType);
        }

        private static ClientModels.QuestionType ToQuestionType(this QuestionType questionType)
        {
            return (ClientModels.QuestionType)Enum.Parse(typeof(QuestionType), questionType.ToString());
        }

        internal static Models.SurveyAnswer ToSurveyAnswer(this ClientModels.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new Models.SurveyAnswer()
            {
                CreatedOn = surveyAnswer.CreatedOn,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        internal static Models.QuestionAnswer ToQuestionAnswer(this ClientModels.QuestionAnswer questionAnswer)
        {
            if (questionAnswer == null)
            {
                throw new ArgumentNullException(nameof(questionAnswer));
            }

            return new Models.QuestionAnswer()
            {
                Answer = questionAnswer.Answer,
                PossibleAnswers = questionAnswer.PossibleAnswers,
                QuestionText = questionAnswer.QuestionText,
                QuestionType = questionAnswer.QuestionType.ToQuestionType()
            };
        }

        internal static ClientModels.SurveyAnswer ToSurveyAnswer(this Models.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new ClientModels.SurveyAnswer()
            {
                CreatedOn = surveyAnswer.CreatedOn,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        internal static ClientModels.QuestionAnswer ToQuestionAnswer(this Models.QuestionAnswer questionAnswer)
        {
            if (questionAnswer == null)
            {
                throw new ArgumentNullException(nameof(questionAnswer));
            }

            return new ClientModels.QuestionAnswer()
            {
                Answer = questionAnswer.Answer,
                PossibleAnswers = questionAnswer.PossibleAnswers,
                QuestionText = questionAnswer.QuestionText,
                QuestionType = questionAnswer.QuestionType.ToQuestionType()
            };
        }

        internal static ClientModels.SurveyAnswersSummary ToSurveyAnswersSummary(
            this SurveyAnswersSummary surveyAnswersSummary)
        {
            if (surveyAnswersSummary == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswersSummary));
            }

            return new ClientModels.SurveyAnswersSummary()
            {
                SlugName = surveyAnswersSummary.SlugName,
                TotalAnswers = surveyAnswersSummary.TotalAnswers,
                QuestionAnswersSummaries =
                    surveyAnswersSummary.QuestionAnswersSummaries.Select(qas => qas.ToQuestionAnswersSummary()).ToList()
            };
        }

        internal static SurveyAnswersSummary ToSurveyAnswersSummary(
            this ClientModels.SurveyAnswersSummary surveyAnswersSummary)
        {
            if (surveyAnswersSummary == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswersSummary));
            }

            return new SurveyAnswersSummary()
            {
                SlugName = surveyAnswersSummary.SlugName,
                QuestionAnswersSummaries =
                    surveyAnswersSummary.QuestionAnswersSummaries.Select(qas => qas.ToQuestionAnswersSummary()).ToList()
            };
        }

        internal static QuestionAnswersSummary ToQuestionAnswersSummary(
            this ClientModels.QuestionAnswersSummary questionAnswersSummary)
        {
            if (questionAnswersSummary == null)
            {
                throw new ArgumentException(nameof(questionAnswersSummary));
            }

            return new QuestionAnswersSummary()
            {
                AnswersSummary = questionAnswersSummary.AnswersSummary,
                PossibleAnswers = questionAnswersSummary.PossibleAnswers,
                QuestionText = questionAnswersSummary.QuestionText,
                QuestionType = (QuestionType)questionAnswersSummary.QuestionType
            };
        }

        internal static ClientModels.QuestionAnswersSummary ToQuestionAnswersSummary(
            this QuestionAnswersSummary questionAnswersSummary)
        {
            if (questionAnswersSummary == null)
            {
                throw new ArgumentException(nameof(questionAnswersSummary));
            }

            return new ClientModels.QuestionAnswersSummary()
            {
                AnswersSummary = questionAnswersSummary.AnswersSummary,
                PossibleAnswers = questionAnswersSummary.PossibleAnswers,
                QuestionText = questionAnswersSummary.QuestionText,
                QuestionType = questionAnswersSummary.QuestionType.ToQuestionType()
            };
        }
    }
}
