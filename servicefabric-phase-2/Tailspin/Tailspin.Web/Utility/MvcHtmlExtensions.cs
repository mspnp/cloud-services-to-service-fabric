namespace Tailspin.Web.Utility
{
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Globalization;
    using System.Net;
    using Tailspin.Web.Shared.Helpers;

    public static class MvcHtmlExtensions
    {
        public static IHtmlContent SurveyLink(this IHtmlHelper htmlHelper, string linkText, string surveySlug)
        {
            var publicSurveysWebsiteUrl = ServiceFabricConfiguration.GetConfigurationSettingValue("Endpoints",
                "PublicSurveyWebsiteUrl", "http://127.0.0.1/");
            
            var surveyLink = string.Format(CultureInfo.InvariantCulture, "{0}/survey/{1}", publicSurveysWebsiteUrl, surveySlug);
            var tagBuilder = new TagBuilder("a");
            tagBuilder.InnerHtml.AppendHtml(!string.IsNullOrEmpty(linkText) ? WebUtility.HtmlEncode(linkText) : string.Empty);
            tagBuilder.MergeAttribute("href", surveyLink);
            tagBuilder.TagRenderMode = TagRenderMode.Normal;
            return tagBuilder;
        }
    }
}