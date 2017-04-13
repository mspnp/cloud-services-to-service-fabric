namespace Tailspin.SurveyAnalysisService.Models
{
    using System;
    using System.Linq;

    internal static class MappingExtensions
    {
        private static string ToQuestionType(this Client.Models.QuestionType questionType)
        {
            return Enum.GetName(typeof(Client.Models.QuestionType), questionType);
        }

        private static Client.Models.QuestionType ToQuestionType(this string questionType)
        {
            return (Client.Models.QuestionType)Enum.Parse(typeof(Client.Models.QuestionType), questionType);
        }

        private static Client.Models.QuestionType ToQuestionType(this QuestionType questionType)
        {
            return (Client.Models.QuestionType)Enum.Parse(typeof(QuestionType), questionType.ToString());
        }

        internal static Models.SurveyAnswer ToSurveyAnswer(this Client.Models.SurveyAnswer surveyAnswer)
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

        internal static Models.QuestionAnswer ToQuestionAnswer(this Client.Models.QuestionAnswer questionAnswer)
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

        internal static Client.Models.SurveyAnswer ToSurveyAnswer(this Models.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new Client.Models.SurveyAnswer()
            {
                CreatedOn = surveyAnswer.CreatedOn,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        internal static Client.Models.QuestionAnswer ToQuestionAnswer(this Models.QuestionAnswer questionAnswer)
        {
            if (questionAnswer == null)
            {
                throw new ArgumentNullException(nameof(questionAnswer));
            }

            return new Client.Models.QuestionAnswer()
            {
                Answer = questionAnswer.Answer,
                PossibleAnswers = questionAnswer.PossibleAnswers,
                QuestionText = questionAnswer.QuestionText,
                QuestionType = questionAnswer.QuestionType.ToQuestionType()
            };
        }

        internal static Client.Models.SurveyAnswersSummary ToSurveyAnswersSummary(
            this SurveyAnswersSummary surveyAnswersSummary)
        {
            if (surveyAnswersSummary == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswersSummary));
            }

            return new Client.Models.SurveyAnswersSummary()
            {
                SlugName = surveyAnswersSummary.SlugName,
                TotalAnswers = surveyAnswersSummary.TotalAnswers,
                QuestionAnswersSummaries =
                    surveyAnswersSummary.QuestionAnswersSummaries.Select(qas => qas.ToQuestionAnswersSummary()).ToList()
            };
        }

        internal static SurveyAnswersSummary ToSurveyAnswersSummary(
            this Client.Models.SurveyAnswersSummary surveyAnswersSummary)
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
            this Client.Models.QuestionAnswersSummary questionAnswersSummary)
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

        internal static Client.Models.QuestionAnswersSummary ToQuestionAnswersSummary(
            this QuestionAnswersSummary questionAnswersSummary)
        {
            if (questionAnswersSummary == null)
            {
                throw new ArgumentException(nameof(questionAnswersSummary));
            }

            return new Client.Models.QuestionAnswersSummary()
            {
                AnswersSummary = questionAnswersSummary.AnswersSummary,
                PossibleAnswers = questionAnswersSummary.PossibleAnswers,
                QuestionText = questionAnswersSummary.QuestionText,
                QuestionType = questionAnswersSummary.QuestionType.ToQuestionType()
            };
        }
    }
}
