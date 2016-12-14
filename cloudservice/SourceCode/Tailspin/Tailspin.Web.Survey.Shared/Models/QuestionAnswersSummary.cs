namespace Tailspin.Web.Survey.Shared.Models
{
    using System;

    public class QuestionAnswersSummary
    {
        public string AnswersSummary { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionText { get; set; }

        public string PossibleAnswers { get; set; }
    }
}