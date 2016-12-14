namespace Tailspin.Web.Survey.Public.Utility
{
    using Shared.Models;
    using System;

    public static class QuestionTemplateFactory
    {
        public static string Create(QuestionAnswer questionAnswer)
        {
            switch (questionAnswer.QuestionType)
            {
                case QuestionType.SimpleText:
                    return "SimpleText";
                case QuestionType.MultipleChoice:
                    return "MultipleChoice";
                case QuestionType.FiveStars:
                    return "FiveStars";
                default:
                    throw new ArgumentException("Invalid QuestionType value", "questionType");
            }
        }
    }
}