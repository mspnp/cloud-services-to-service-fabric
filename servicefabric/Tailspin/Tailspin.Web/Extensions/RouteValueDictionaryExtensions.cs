namespace Tailspin.Web.Areas.Survey.Extensions
{
    using Microsoft.AspNetCore.Routing;
    using System.Collections.Specialized;

    internal static class RouteValueDictionaryExtensions
    {
        public static void MergeQueryToRouteValues(this RouteValueDictionary routeValues, NameValueCollection queryValues)
        {
            foreach (string key in queryValues.AllKeys)
            {
                routeValues[key] = queryValues[key];
            }
        }
    }
}