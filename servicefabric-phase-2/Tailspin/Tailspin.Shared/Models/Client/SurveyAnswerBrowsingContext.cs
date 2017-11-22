namespace Tailspin.Shared.Models.Client
{
    public class SurveyAnswerBrowsingContext
    {
        public SurveyAnswer SurveyAnswer { get; set; }

        public string PreviousAnswerId { get; set; }

        public string NextAnswerId { get; set; }
    }
}
