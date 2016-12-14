using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Tailspin.Web.Security
{
    public static class ClaimsIdentityExtensions
    {
        public static string FindFirstValue(this ClaimsIdentity principal, string claimType, bool throwIfNotFound = false)
        {
            var value = principal.FindFirst(claimType)?.Value;
            if (throwIfNotFound && string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"The supplied principal does not contain a claim of type {claimType}");
            }

            return value;
        }

        public static string GetIssuerValue(this ClaimsIdentity principal)
        {
            return principal.FindFirstValue(OpenIdConnectClaimTypes.IssuerValue, true);
        }

        public static string GetTenantIdValue(this ClaimsIdentity principal)
        {
            return principal.FindFirstValue(AzureADClaimTypes.TenantId, true);
        }

    }
}