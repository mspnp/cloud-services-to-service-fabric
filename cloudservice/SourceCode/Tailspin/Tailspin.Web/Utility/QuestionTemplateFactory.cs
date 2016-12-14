using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tailspin.Web.Survey.Shared.Models;
using System.Web.Mvc.Html;

namespace Tailspin.Web.Utility
{
    public static class QuestionTemplateFactory
    {
        public static string Create(QuestionAnswersSummary questionAnswersSummary)
        {
            switch (questionAnswersSummary.QuestionType)
            {
                case QuestionType.SimpleText:
                    return "Summary-SimpleText";
                case QuestionType.MultipleChoice:
                    return "Summary-MultipleChoice";
                case QuestionType.FiveStars:
                    return "Summary-FiveStars";

                default:
                    throw new ArgumentException("Invalid QuestionType value", "questionAnswersSummary");
            }
        }

        public static string Create(QuestionAnswer questionAnswer)
        {
            switch (questionAnswer.QuestionType)
            {
                case QuestionType.SimpleText:
                    return "Answer-SimpleText";
                case QuestionType.MultipleChoice:
                    return "Answer-MultipleChoice";
                case QuestionType.FiveStars:
                    return "Answer-FiveStars";

                default:
                    throw new ArgumentException("Invalid QuestionType value", "questionAnswer");
            }
        }
    }
}