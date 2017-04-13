using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tailspin.SurveyAnswerService.Tests
{
    public class AssertEx
    {
        public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> action, string message, params object[] parameters)
            where T : Exception
        {
            T t;
            string empty = string.Empty;
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            try
            {
                await action();
                empty = string.Format(CultureInfo.CurrentCulture, "NoExceptionThrown=No exception thrown. {1} exception was expected. {0}",
                    new object[] { message, Type.GetTypeFromHandle(typeof(T).TypeHandle).Name });
                HandleFail("Assert.ThrowsException", empty, parameters);
                return default(T);
            }
            catch (Exception exception1) when (!exception1.GetType().Equals(typeof(AssertFailedException)))
            {
                Exception exception = exception1;
                if (!typeof(T).Equals(exception.GetType()))
                {
                    empty = string.Format(
                        CultureInfo.CurrentCulture,
                        "WrongExceptionThrown=Threw exception {2}, but exception {1} was expected. {0}{5}Exception Message: {3}{5}Stack Trace: {4}",
                        new object[]
                        {
                            message,
                            Type.GetTypeFromHandle(typeof(T).TypeHandle).Name,
                            exception.GetType().Name,
                            exception.Message,
                            exception.StackTrace,
                            Environment.NewLine
                        });
                    HandleFail("Assert.ThrowsException", empty, parameters);
                }
                t = (T)exception;
            }
            return t;
        }

        internal static string ReplaceNulls(object input)
        {
            if (input == null)
            {
                return "(null)";
            }
            string str = input.ToString();
            if (str == null)
            {
                return "(object)";
            }
            return Assert.ReplaceNullChars(str);
        }

        internal static void HandleFail(string assertionName, string message, params object[] parameters)
        {
            string empty = string.Empty;
            if (!string.IsNullOrEmpty(message))
            {
                empty = (parameters != null ? string.Format(CultureInfo.CurrentCulture, ReplaceNulls(message), parameters) : ReplaceNulls(message));
            }
            throw new AssertFailedException(string.Format(
                CultureInfo.CurrentCulture,
                "AssertionFailed={0} failed. {1}",
                new object[] { assertionName, empty }));
        }
    }
}
