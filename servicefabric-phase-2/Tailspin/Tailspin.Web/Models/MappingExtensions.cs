namespace Tailspin.Web.Models
{
    using Shared.Models;
    using System;
    using System.Linq;

    internal static class MappingExtensions
    {
        internal static Tailspin.SurveyManagementService.Client.Models.Survey ToSurvey(this SurveyModel surveyModel)
        {
            if (surveyModel == null)
            {
                throw new ArgumentNullException(nameof(surveyModel));
            }

            return new SurveyManagementService.Client.Models.Survey()
            {
                CreatedOn = surveyModel.CreatedOn,
                Questions = surveyModel.Questions.Select(q => q.ToQuestion()).ToList(),
                SlugName = surveyModel.SlugName,
                Title = surveyModel.Title
            };
        }

        internal static Tailspin.SurveyManagementService.Client.Models.Question ToQuestion(this Tailspin.Web.Shared.Models.Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            return new SurveyManagementService.Client.Models.Question()
            {
                PossibleAnswers = question.PossibleAnswers,
                Text = question.Text,
                Type = question.Type.ToQuestionType()
            };
        }

        internal static Tailspin.SurveyManagementService.Client.Models.QuestionType ToQuestionType(this Tailspin.Web.Shared.Models.QuestionType questionType)
        {
            Tailspin.SurveyManagementService.Client.Models.QuestionType result;
            switch (questionType)
            {
                case Tailspin.Web.Shared.Models.QuestionType.FiveStars:
                    result = SurveyManagementService.Client.Models.QuestionType.FiveStars;
                    break;
                case Tailspin.Web.Shared.Models.QuestionType.MultipleChoice:
                    result = SurveyManagementService.Client.Models.QuestionType.MultipleChoice;
                    break;
                case Tailspin.Web.Shared.Models.QuestionType.SimpleText:
                    result = SurveyManagementService.Client.Models.QuestionType.SimpleText;
                    break;
                default:
                    throw new ArgumentException($"Invalid question type: {questionType}");
            }

            return result;
        }

        internal static SurveyModel ToSurveyModel(this Tailspin.SurveyManagementService.Client.Models.SurveyInformation surveyInformation)
        {
            if (surveyInformation == null)
            {
                throw new ArgumentNullException(nameof(surveyInformation));
            }

            return new SurveyModel(surveyInformation.SlugName)
            {
                CreatedOn = surveyInformation.CreatedOn,
                Title = surveyInformation.Title
            };
        }

        internal static BrowseResponseModel ToBrowseResponseModel(this Tailspin.SurveyAnswerService.Client.Models.SurveyAnswerBrowsingContext browsingContext)
        {
            if (browsingContext == null)
            {
                throw new ArgumentNullException(nameof(browsingContext));
            }

            return new BrowseResponseModel()
            {
                NextAnswerId = browsingContext.NextAnswerId,
                PreviousAnswerId = browsingContext.PreviousAnswerId,
                SurveyAnswer = browsingContext.SurveyAnswer?.ToSurveyAnswer()
            };
        }

        internal static Tailspin.Web.Shared.Models.SurveyAnswer ToSurveyAnswer(this Tailspin.SurveyAnswerService.Client.Models.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new Shared.Models.SurveyAnswer()
            {
                CreatedOn = surveyAnswer.CreatedOn,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        internal static Tailspin.Web.Shared.Models.QuestionAnswer ToQuestionAnswer(this Tailspin.SurveyAnswerService.Client.Models.QuestionAnswer questionAnswer)
        {
            if (questionAnswer == null)
            {
                throw new ArgumentNullException(nameof(questionAnswer));
            }

            return new Shared.Models.QuestionAnswer()
            {
                Answer = questionAnswer.Answer,
                PossibleAnswers = questionAnswer.PossibleAnswers,
                QuestionText = questionAnswer.QuestionText,
                QuestionType = questionAnswer.QuestionType.ToQuestionType()
            };
        }

        internal static Tailspin.Web.Shared.Models.QuestionType ToQuestionType(this Tailspin.SurveyAnswerService.Client.Models.QuestionType questionType)
        {
            Tailspin.Web.Shared.Models.QuestionType result;
            switch (questionType)
            {
                case SurveyAnswerService.Client.Models.QuestionType.FiveStars:
                    result = Tailspin.Web.Shared.Models.QuestionType.FiveStars;
                    break;
                case SurveyAnswerService.Client.Models.QuestionType.MultipleChoice:
                    result = Tailspin.Web.Shared.Models.QuestionType.MultipleChoice;
                    break;
                case SurveyAnswerService.Client.Models.QuestionType.SimpleText:
                    result = Tailspin.Web.Shared.Models.QuestionType.SimpleText;
                    break;
                default:
                    throw new ArgumentException($"Invalid question type: {questionType}");
            }

            return result;
        }

        public static SurveyAnswersSummary ToSurveyAnswersSummary(this SurveyAnalysisService.Client.Models.SurveyAnswersSummary surveyAnswersSummary)
        {
            if (surveyAnswersSummary == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswersSummary));
            }

            return new SurveyAnswersSummary()
            {
                SlugName = surveyAnswersSummary.SlugName,
                TotalAnswers = surveyAnswersSummary.TotalAnswers,
                QuestionAnswersSummaries = surveyAnswersSummary.QuestionAnswersSummaries.Select(qas => qas.ToQuestionAnswerSummary()).ToList()
            };
        }

        public static QuestionAnswersSummary ToQuestionAnswerSummary(this SurveyAnalysisService.Client.Models.QuestionAnswersSummary questionAnswersSummary)
        {
            if (questionAnswersSummary == null)
            {
                throw new ArgumentNullException(nameof(questionAnswersSummary));
            }

            return new QuestionAnswersSummary()
            {
                QuestionText = questionAnswersSummary.QuestionText,
                QuestionType = (QuestionType)Enum.Parse(typeof(QuestionType), questionAnswersSummary.QuestionType.ToString()),
                AnswersSummary = questionAnswersSummary.AnswersSummary,
                PossibleAnswers = questionAnswersSummary.PossibleAnswers
            };
        }
    }
}
