using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Provider;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tailspin.Web.Security;
using Tailspin.Web.Survey.Shared.Models;
using Tailspin.Web.Survey.Shared.Stores;

namespace Tailspin.Web
{
    public partial class Startup
    {
        private static string clientId = CloudConfigurationManager.GetSetting("MicrosoftEntraClientId");
        private static string entraIdInstance = "https://login.microsoftonline.com/{0}";
        private static string postLogoutRedirectUri = "https://localhost";
        private static string tenantId = CloudConfigurationManager.GetSetting("MicrosoftEntraTenantId");

        string authority = String.Format(CultureInfo.InvariantCulture, entraIdInstance, "common");

        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ITenantStore>());

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,
                    RedirectUri = postLogoutRedirectUri,
                    TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = false }, // Multi-tenant applications must implement custom logic to validate the issuer.
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        RedirectToIdentityProvider = context =>
                        {
                            if (context.OwinContext?.Authentication?.AuthenticationResponseChallenge != null &&
                                context.OwinContext.Authentication.AuthenticationResponseChallenge.Properties.IsSigningUp())
                            {
                                context.ProtocolMessage.Prompt = "admin_consent";
                            }

                            return Task.FromResult(0);
                        },

                        AuthenticationFailed = context =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/Error?message=" + context.Exception.Message);
                            return Task.FromResult(0);
                        },

                        SecurityTokenValidated = async context =>
                        {
                            NormalizeClaims(context.AuthenticationTicket);

                            ITenantStore tenantStore = context.OwinContext.Get<ITenantStore>();
                            if (context.AuthenticationTicket.Properties.IsSigningUp())
                            {
                                await SignUpTenantAsync(context.AuthenticationTicket, tenantStore);
                            }
                            else
                            {
                                await ValidateTenantAsync(context.AuthenticationTicket, tenantStore);
                            }
                        }
                    }
                });
        }

        private void NormalizeClaims(AuthenticationTicket ticket)
        {
            // If the issuer is Tailspin, add the "TenantAdministrator" claim.

            var principal = ticket.Identity;
            var issuer = principal.GetTenantIdValue();
            if (String.Equals(issuer, tenantId, StringComparison.OrdinalIgnoreCase))
            {
                var claim = new Claim(ClaimTypes.Role, TailspinRoles.TenantAdministrator, ClaimValueTypes.String, "LOCAL AUTHORITY");
                principal.AddClaim(claim);
            }
        }

        private async Task SignUpTenantAsync(AuthenticationTicket ticket, ITenantStore tenantStore)
        {
            var principal = ticket.Identity;
            var tenantId = principal.GetTenantIdValue();
            var issuerValue = principal.GetIssuerValue();

            // If this is the first time signing up, store the tenant information.
            var tenant = await tenantStore.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                tenant = new Tenant
                {
                    TenantId = tenantId,
                    IssuerValue = issuerValue,
                    SubscriptionKind = SubscriptionKind.Standard
                };

                await tenantStore.SaveTenantAsync(tenant);
            }
        }

        private async Task ValidateTenantAsync(AuthenticationTicket ticket, ITenantStore tenantStore)
        {
            var principal = ticket.Identity;
            var tenantId = principal.GetTenantIdValue();
            var isTailspinAdmin = principal.FindAll(ClaimTypes.Role).Any(x => x.Equals(TailspinRoles.TenantAdministrator));

            var tenant = await tenantStore.GetTenantAsync(tenantId);
            if (tenant == null && !isTailspinAdmin)
            {
                throw new SecurityTokenValidationException($"Tenant {tenantId} is not registered");
            }
        }


    }
}