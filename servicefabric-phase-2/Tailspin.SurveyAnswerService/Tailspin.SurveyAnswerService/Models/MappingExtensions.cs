namespace Tailspin.SurveyAnswerService.Models
{
    using System;
    using System.Linq;

    public static class MappingExtensions
    {

        private static string ToQuestionType(this Client.Models.QuestionType questionType)
        {
            return Enum.GetName(typeof(Client.Models.QuestionType), questionType);
        }

        private static Client.Models.QuestionType ToQuestionType(this string questionType)
        {
            return (Client.Models.QuestionType)Enum.Parse(typeof(Client.Models.QuestionType), questionType);
        }


        public static Models.SurveyAnswer ToSurveyAnswer(this Client.Models.SurveyAnswer surveyAnswer)
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

        public static Models.QuestionAnswer ToQuestionAnswer(this Client.Models.QuestionAnswer questionAnswer)
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

        public static Client.Models.SurveyAnswer ToSurveyAnswer(this Models.SurveyAnswer surveyAnswer)
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

        public static Client.Models.QuestionAnswer ToQuestionAnswer(this Models.QuestionAnswer questionAnswer)
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

        public static SurveyAnalysisService.Client.Models.SurveyAnswer ToAnalysisServiceSurveyAnswer(this Models.SurveyAnswer surveyAnswer)
        {
            if (surveyAnswer == null)
            {
                throw new ArgumentNullException(nameof(surveyAnswer));
            }

            return new SurveyAnalysisService.Client.Models.SurveyAnswer()
            {
                CreatedOn = surveyAnswer.CreatedOn,
                QuestionAnswers = surveyAnswer.QuestionAnswers.Select(qa => qa.ToAnalysisServiceQuestionAnswer()).ToList(),
                SlugName = surveyAnswer.SlugName,
                Title = surveyAnswer.Title
            };
        }

        public static SurveyAnalysisService.Client.Models.QuestionAnswer ToAnalysisServiceQuestionAnswer(this Models.QuestionAnswer questionAnswer)
        {
            if (questionAnswer == null)
            {
                throw new ArgumentNullException(nameof(questionAnswer));
            }

            return new SurveyAnalysisService.Client.Models.QuestionAnswer()
            {
                Answer = questionAnswer.Answer,
                PossibleAnswers = questionAnswer.PossibleAnswers,
                QuestionText = questionAnswer.QuestionText,
                QuestionType = (SurveyAnalysisService.Client.Models.QuestionType)Enum.Parse(typeof(SurveyAnalysisService.Client.Models.QuestionType), questionAnswer.QuestionType)
            };
        }
    }
}
