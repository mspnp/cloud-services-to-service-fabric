namespace Tailspin.Shared.Models.Client
{
    using System.Collections.Generic;

    public class SurveyAnswersSummary
    {
        public SurveyAnswersSummary()
            : this(string.Empty)
        {
        }

        public SurveyAnswersSummary(string slugName)
        {
            this.SlugName = slugName;
            this.QuestionAnswersSummaries = new List<QuestionAnswersSummary>();
        }

        public string SlugName { get; set; }

        public int TotalAnswers { get; set; }

        public IList<QuestionAnswersSummary> QuestionAnswersSummaries { get; set; }
    }
}