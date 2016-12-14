namespace Tailspin.Web.Survey.Shared.Helpers
{
    using System;

    public static class DateTimeExtensions
    {
        public static string GetFormatedTicks(this DateTime dateTime)
        {
            return string.Format("{0:D19}", dateTime.Ticks);
        }
    }
}
