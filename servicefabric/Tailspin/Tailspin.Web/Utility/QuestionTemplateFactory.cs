using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tailspin.Web.Survey.Shared.Models;

namespace Tailspin.Web.Utility
{
    public static class QuestionTemplateFactory
    {
        private static string GetBaseTemplateName(QuestionType questionType)
        {
            switch (questionType)
            {
                case QuestionType.SimpleText:
                    return "SimpleText";
                case QuestionType.MultipleChoice:
                    return "MultipleChoice";
                case QuestionType.FiveStars:
                    return "FiveStars";
                default:
                    return "String";
            }
        }

        public static async Task<IHtmlContent> PartialForAsync(this IHtmlHelper html, QuestionAnswersSummary questionAnswersSummary)
        {
            return await html.PartialAsync("~/Views/Shared/DisplayTemplates/Summary-" + GetBaseTemplateName(questionAnswersSummary.QuestionType) + ".cshtml", questionAnswersSummary, null);
        }

        public static async Task<IHtmlContent> PartialForAsync(this IHtmlHelper html, QuestionAnswer questionAnswer)
        {
            return await html.PartialAsync("~/Views/Shared/DisplayTemplates/Answer-" + GetBaseTemplateName(questionAnswer.QuestionType) + ".cshtml", questionAnswer, null);
        }
    }
}