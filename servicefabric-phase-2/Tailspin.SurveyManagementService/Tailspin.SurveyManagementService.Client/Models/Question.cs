using System.Runtime.Serialization;

namespace Tailspin.SurveyManagementService.Client.Models
{
    [DataContract]
    public class Question
    {
        public Question()
        {
        }

        [DataMember(Order = 0)]
        public string Text { get; set; }

        [DataMember(Order = 1)]
        public QuestionType Type { get; set; } = QuestionType.SimpleText;

        [DataMember(Order = 2)]
        public string PossibleAnswers { get; set; }
    }
}
