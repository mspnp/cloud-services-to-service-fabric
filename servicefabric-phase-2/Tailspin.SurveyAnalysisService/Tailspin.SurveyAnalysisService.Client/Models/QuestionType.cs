namespace Tailspin.SurveyAnalysisService.Client.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum QuestionType
    {
        [EnumMember] SimpleText,
        [EnumMember] MultipleChoice,
        [EnumMember] FiveStars
    }
}