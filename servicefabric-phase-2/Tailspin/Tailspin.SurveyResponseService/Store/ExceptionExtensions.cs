namespace Tailspin.SurveyResponseService.Store
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public static class ExceptionExtensions
    {
        private const string Line = "==============================================================================";

        public static string TraceInformation(this Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            var exceptionInformation = new StringBuilder();

            exceptionInformation.Append(BuildMessage(exception));
            Exception inner = exception.InnerException;
            while (inner != null)
            {
                exceptionInformation.Append($"{Environment.NewLine}{Environment.NewLine}{BuildMessage(inner)}");
                inner = inner.InnerException;
            }

            return exceptionInformation.ToString();
        }

        private static string BuildMessage(Exception ex) =>
            $"{Line}{Environment.NewLine}{ex.GetType().Name}:{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}{Line}";
    }
}
