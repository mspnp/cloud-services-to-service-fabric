using System;

namespace Tailspin.Shared.Models.Client
{
    [Serializable]
    public class QuestionAnswer
    {
        public string QuestionText { get; set; }

        public QuestionType QuestionType { get; set; }

        public string Answer { get; set; }

        public string PossibleAnswers { get; set; }
    }
}