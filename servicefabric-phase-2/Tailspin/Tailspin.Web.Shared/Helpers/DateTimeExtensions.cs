namespace Tailspin.Web.Shared.Helpers
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
