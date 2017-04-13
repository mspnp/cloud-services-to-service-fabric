namespace Tailspin.SurveyAnalysisService.Client.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class QuestionAnswersSummary
    {
        [DataMember(Order = 0)]
        public QuestionType QuestionType { get; set; }

        [DataMember(Order = 1)]
        public string QuestionText { get; set; }

        [DataMember(Order = 2)]
        public string PossibleAnswers { get; set; }

        [DataMember(Order = 3)]
        public string AnswersSummary { get; set; }
    }
}