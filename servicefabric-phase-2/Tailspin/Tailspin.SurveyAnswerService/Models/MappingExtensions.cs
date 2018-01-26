namespace Tailspin.SurveyAnswerService.Models
{
    using System;
    using System.Linq;
    using ClientModels = Tailspin.Shared.Models.Client;
    using ApiModels = Tailspin.Shared.Models.Api;

    public static class MappingExtensions
    {

        private static string ToQuestionType(this ClientModels.QuestionType questionType)
        {
            return Enum.GetName(typeof(ClientModels.QuestionType), questionType);
        }

        private static ClientModels.QuestionType ToQuestionType(this string questionType)
        {
            return (ClientModels.QuestionType)Enum.Parse(typeof(ClientModels.QuestionType), questionType);
        }


        public static ApiModels.SurveyAnswer ToSurveyAnswer(this ClientModels.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new ApiModels.SurveyAnswer()
            {
                CreatedOn = surveyAnswer.CreatedOn,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        public static ApiModels.QuestionAnswer ToQuestionAnswer(this ClientModels.QuestionAnswer questionAnswer)
        {
            if (questionAnswer == null)
            {
                throw new ArgumentNullException(nameof(questionAnswer));
            }

            return new ApiModels.QuestionAnswer()
            {
                Answer = questionAnswer.Answer,
                PossibleAnswers = questionAnswer.PossibleAnswers,
                QuestionText = questionAnswer.QuestionText,
                QuestionType = questionAnswer.QuestionType.ToQuestionType()
            };
        }

        public static ClientModels.SurveyAnswer ToSurveyAnswer(this ApiModels.SurveyAnswer surveyAnswer)
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

        public static ClientModels.QuestionAnswer ToQuestionAnswer(this ApiModels.QuestionAnswer questionAnswer)
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

        public static ClientModels.SurveyAnswer ToAnalysisServiceSurveyAnswer(this ApiModels.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new ClientModels.SurveyAnswer()
            {
                CreatedOn = surveyAnswer.CreatedOn,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToAnalysisServiceQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        public static ClientModels.QuestionAnswer ToAnalysisServiceQuestionAnswer(this ApiModels.QuestionAnswer questionAnswer)
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
                QuestionType = (ClientModels.QuestionType)Enum.Parse(typeof(ClientModels.QuestionType), questionAnswer.QuestionType)
            };
        }
    }
}
