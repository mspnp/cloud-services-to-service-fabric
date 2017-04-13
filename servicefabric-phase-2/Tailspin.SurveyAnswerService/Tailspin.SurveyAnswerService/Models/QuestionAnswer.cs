namespace Tailspin.SurveyAnswerService.Models
{
    public class QuestionAnswer
    {
        public QuestionAnswer()
        {
        }

        public string QuestionText { get; set; }

        public string QuestionType { get; set; }

        public string Answer { get; set; }

        public string PossibleAnswers { get; set; }
    }
}
