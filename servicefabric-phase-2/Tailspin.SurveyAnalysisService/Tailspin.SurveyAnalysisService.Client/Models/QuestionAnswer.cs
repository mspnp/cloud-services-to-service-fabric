namespace Tailspin.SurveyAnalysisService.Client.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class QuestionAnswer
    {
        [DataMember(Order = 0)]
        public string QuestionText { get; set; }

        [DataMember(Order = 1)]
        public QuestionType QuestionType { get; set; }

        [DataMember(Order = 2)]
        public string Answer { get; set; }

        [DataMember(Order = 3)]
        public string PossibleAnswers { get; set; }
    }
}