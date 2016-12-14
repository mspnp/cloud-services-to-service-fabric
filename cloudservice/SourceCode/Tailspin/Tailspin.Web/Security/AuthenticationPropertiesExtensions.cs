using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tailspin.Web.Security
{
    internal static class AuthenticationPropertiesExtensions
    {
        internal static bool IsSigningUp(this AuthenticationProperties properties)
        {
            string signupValue = null;
            bool isSigningUp = false;
            if (properties.Dictionary.TryGetValue("signup", out signupValue))
            {
                if (!bool.TryParse(signupValue, out isSigningUp))
                {
                    throw new InvalidOperationException($"'{signupValue}' is an invalid boolean value");
                }
            }
            return isSigningUp;

        }
    }
}