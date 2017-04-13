namespace Tailspin.SurveyAnalysisService.Client.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
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

        [DataMember(Order = 0)]
        public string SlugName { get; set; }

        [DataMember(Order = 1)]
        public int TotalAnswers { get; set; }

        [DataMember(Order = 2)]
        public IList<QuestionAnswersSummary> QuestionAnswersSummaries { get; set; }
    }
}