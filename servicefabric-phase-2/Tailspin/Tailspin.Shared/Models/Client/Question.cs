namespace Tailspin.Shared.Models.Client
{
    public class Question
    {
        public string Text { get; set; }

        public QuestionType Type { get; set; } = QuestionType.SimpleText;

        public string PossibleAnswers { get; set; }
    }
}
