namespace Tailspin.Web.Survey.Shared.Helpers
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string SplitWords(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var regex = new Regex("[A-Z]", RegexOptions.Compiled);
            return regex.Replace(text, match => " " + match.Value);
        }
    }
}
