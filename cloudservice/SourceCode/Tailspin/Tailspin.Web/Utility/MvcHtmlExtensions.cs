namespace Tailspin.Web.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Tailspin.Web.Survey.Shared.Helpers;

    public static class MvcHtmlExtensions
    {
        public static MvcHtmlString SurveyLink(this HtmlHelper htmlHelper, string linkText, string tenant, string surveySlug)
        {
            string publicSurveysWebsiteUrl = CloudConfiguration.GetConfigurationSetting("PublicSurveyWebsiteUrl", string.Empty, true);
            
            var surveyLink = string.Format(CultureInfo.InvariantCulture, "{0}/survey/{1}/{2}", publicSurveysWebsiteUrl, tenant, surveySlug);
            var tagBuilder = new TagBuilder("a");
            tagBuilder.InnerHtml = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;
            tagBuilder.MergeAttribute("href", surveyLink);

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}