namespace Tailspin.Web.Survey.Shared.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum QuestionType
    {
        SimpleText,
        MultipleChoice,
        FiveStars
    }

    [Serializable]
    [RequiredAnswer(ErrorMessage = "* Answers must be supplied for the question.")]
    public class Question
    {
        public Question()
        {
            this.Type = QuestionType.SimpleText;
        }

        [Required(ErrorMessage = "* You must provide a Question.")]
        [DisplayName("Question")]
        public string Text { get; set; }

        public QuestionType Type { get; set; }

        [DisplayName("Answers")]
        public string PossibleAnswers { get; set; }
    }
}