namespace Tailspin.Web.Survey.Shared.Models
{
    using System;
    using System.Globalization;

    public static class SurveyExtensions
    {
        public static SurveyData ToDataModel(this Survey survey)
        {
            var data = new SurveyData    
            {
                Id = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", survey.TenantId, survey.SlugName),
                Title = survey.Title,
                CreatedOn = survey.CreatedOn            
            };

            int idx = 0;
            foreach (var question in survey.Questions)
            {
                var questionData = new QuestionData
                {
                    Id = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", idx++, Guid.NewGuid()),
                    QuestionText = question.Text,
                    QuestionType = Enum.GetName(typeof(QuestionType), question.Type)
                };

                if (question.Type == QuestionType.MultipleChoice)
                {
                    string[] possibleAnswers = question.PossibleAnswers.Split('\n');
                    foreach (var possibleAnswer in possibleAnswers)
                    {
                        questionData.PossibleAnswerDatas.Add(new PossibleAnswerData
                        {
                            Id = Guid.NewGuid().ToString(),
                            QuestionId = questionData.Id,
                            Answer = possibleAnswer
                        });
                    }
                }

                data.QuestionDatas.Add(questionData);
            }

            return data;
        }
    }
}