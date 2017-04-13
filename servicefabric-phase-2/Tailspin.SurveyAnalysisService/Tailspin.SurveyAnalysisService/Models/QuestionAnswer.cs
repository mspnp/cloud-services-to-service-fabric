namespace Tailspin.SurveyAnalysisService.Models
{
    public class QuestionAnswer
    {
        public string QuestionText { get; set; }

        public string QuestionType { get; set; }

        public string Answer { get; set; }

        public string PossibleAnswers { get; set; }
    }
}