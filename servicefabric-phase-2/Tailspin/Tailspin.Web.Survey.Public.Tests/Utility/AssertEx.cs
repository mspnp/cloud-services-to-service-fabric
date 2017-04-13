using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tailspin.Web.Survey.Public.Tests.Utility
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
                empty = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.NoExceptionThrown, new object[] { message, Type.GetTypeFromHandle(typeof(T).TypeHandle).Name });
                HandleFail("Assert.ThrowsException", empty, parameters);
                return default(T);
            }
            catch (Exception exception1) when (!exception1.GetType().Equals(typeof(AssertFailedException)))
            {
                Exception exception = exception1;
                if (!typeof(T).Equals(exception.GetType()))
                {
                    empty = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.WrongExceptionThrown, new object[] { message, Type.GetTypeFromHandle(typeof(T).TypeHandle).Name, exception.GetType().Name, exception.Message, exception.StackTrace });
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
                return FrameworkMessages.Common_NullInMessages.ToString();
            }
            string str = input.ToString();
            if (str == null)
            {
                return FrameworkMessages.Common_ObjectString.ToString();
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
            throw new AssertFailedException(string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AssertionFailed, new object[] { assertionName, empty }));
        }
    }
}
