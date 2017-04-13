namespace Tailspin.Web.Survey.Public.Models
{
    using System;
    using System.Linq;

    public static class MappingExtensions
    {
        internal static Tailspin.Web.Shared.Models.Survey ToSurvey(
            this Tailspin.SurveyManagementService.Client.Models.SurveyInformation surveyInformation)
        {
            return new Shared.Models.Survey(surveyInformation.SlugName)
            {
                CreatedOn = surveyInformation.CreatedOn,
                Title = surveyInformation.Title
            };
        }

        internal static Tailspin.Web.Shared.Models.Survey ToSurvey(this Tailspin.SurveyManagementService.Client.Models.Survey survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            return new Shared.Models.Survey()
            {
                CreatedOn = survey.CreatedOn,
                Questions = survey.Questions.Select(q => q.ToQuestion()).ToList(),
                TenantId = "",
                Title = survey.Title
            };
        }

        internal static Tailspin.Web.Shared.Models.Question ToQuestion(this Tailspin.SurveyManagementService.Client.Models.Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            return new Shared.Models.Question()
            {
                PossibleAnswers = question.PossibleAnswers,
                Text = question.Text,
                Type = question.Type.ToQuestionType()
            };
        }

        internal static Tailspin.Web.Shared.Models.QuestionType ToQuestionType(this Tailspin.SurveyManagementService.Client.Models.QuestionType questionType)
        {
            Tailspin.Web.Shared.Models.QuestionType result;
            switch (questionType)
            {
                case SurveyManagementService.Client.Models.QuestionType.FiveStars:
                    result = Shared.Models.QuestionType.FiveStars;
                    break;
                case SurveyManagementService.Client.Models.QuestionType.MultipleChoice:
                    result = Shared.Models.QuestionType.MultipleChoice;
                    break;
                case SurveyManagementService.Client.Models.QuestionType.SimpleText:
                    result = Shared.Models.QuestionType.SimpleText;
                    break;
                default:
                    throw new ArgumentException($"Unsupported question type: {questionType}");
            }

            return result;
        }

        internal static Tailspin.SurveyAnswerService.Client.Models.QuestionType ToQuestionType(this Tailspin.Web.Shared.Models.QuestionType questionType)
        {
            Tailspin.SurveyAnswerService.Client.Models.QuestionType result;
            switch (questionType)
            {
                case Tailspin.Web.Shared.Models.QuestionType.FiveStars:
                    result = Tailspin.SurveyAnswerService.Client.Models.QuestionType.FiveStars;
                    break;
                case Tailspin.Web.Shared.Models.QuestionType.MultipleChoice:
                    result = Tailspin.SurveyAnswerService.Client.Models.QuestionType.MultipleChoice;
                    break;
                case Tailspin.Web.Shared.Models.QuestionType.SimpleText:
                    result = Tailspin.SurveyAnswerService.Client.Models.QuestionType.SimpleText;
                    break;
                default:
                    throw new ArgumentException($"Unsupported question type: {questionType}");
            }

            return result;
        }

        internal static Tailspin.Web.Shared.Models.SurveyAnswer ToSurveyAnswer(this Tailspin.SurveyManagementService.Client.Models.Survey survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            return new Shared.Models.SurveyAnswer()
            {
                QuestionAnswers = survey.Questions.Select(q => q.ToQuestionAnswer()).ToList(),
                SlugName = survey.SlugName,
                Title = survey.Title
            };
        }

        internal static Tailspin.Web.Shared.Models.QuestionAnswer ToQuestionAnswer(this Tailspin.SurveyManagementService.Client.Models.Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            return new Shared.Models.QuestionAnswer()
            {
                PossibleAnswers = question.PossibleAnswers,
                QuestionText = question.Text,
                QuestionType = question.Type.ToQuestionType()
            };
        }

        internal static Tailspin.SurveyAnswerService.Client.Models.SurveyAnswer ToSurveyAnswer(this Tailspin.Web.Shared.Models.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new SurveyAnswerService.Client.Models.SurveyAnswer()
            {
                CreatedOn = DateTime.UtcNow,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        internal static Tailspin.SurveyAnswerService.Client.Models.QuestionAnswer ToQuestionAnswer(this Tailspin.Web.Shared.Models.QuestionAnswer questionAnswer)
        {
            if (questionAnswer == null)
            {
                throw new ArgumentNullException(nameof(questionAnswer));
            }

            return new SurveyAnswerService.Client.Models.QuestionAnswer()
            {
                Answer = questionAnswer.Answer,
                PossibleAnswers = questionAnswer.PossibleAnswers,
                QuestionText = questionAnswer.QuestionText,
                QuestionType = questionAnswer.QuestionType.ToQuestionType()
            };
        }
    }
}
