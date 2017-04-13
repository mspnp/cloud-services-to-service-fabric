namespace Tailspin.SurveyAnswerService.Client.Models
{
    public class SurveyAnswerBrowsingContext
    {
        public SurveyAnswerBrowsingContext()
        {
        }

        public SurveyAnswer SurveyAnswer { get; set; }

        public string PreviousAnswerId { get; set; }

        public string NextAnswerId { get; set; }
    }
}
