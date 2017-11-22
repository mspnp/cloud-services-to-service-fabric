namespace Tailspin.SurveyManagementService.Models
{
    using Tailspin.Shared.Models.Client;
    using System;

    public class Question
    {
        public Question()
        {
        }

        public string Text { get; set; }

        public string Type { get; set; } = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText);

        public string PossibleAnswers { get; set; }
    }
}
