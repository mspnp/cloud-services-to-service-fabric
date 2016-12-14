namespace Tailspin.Web.Survey.Shared.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public class TenantStore : ITenantStore, IDisposable
    {
        private const string TenantAccountTag = "_accountinfo_";

        private readonly IAzureBlobContainer<Tenant> tenantBlobContainer;
        private readonly IAzureBlobContainer<byte[]> logosBlobContainer;

        public TenantStore(IAzureBlobContainer<Tenant> tenantBlobContainer, 
                           IAzureBlobContainer<byte[]> logosBlobContainer,
                           IInitializationStatusService initializationStatusService)
        {
            this.tenantBlobContainer = tenantBlobContainer;
            this.logosBlobContainer = logosBlobContainer;
            this.CacheEnabled = initializationStatusService.IsInitialized;
        }

        public bool CacheEnabled { get; set; }

        public async Task InitializeAsync()
        {
            await this.logosBlobContainer.EnsureExistsAsync().ConfigureAwait(false);

            await this.tenantBlobContainer.EnsureExistsAsync().ConfigureAwait(false);

            // This initialization method provisions two tenants for sample purposes. 
            // The following code will not be present on the real repository.
            if (await this.GetTenantAsync("adatum") == null)
            {
                await this.SaveTenantAsync(new Tenant
                {
                    TenantId = "Adatum",
                    HostGeoLocation = "Anywhere US",
                    WelcomeText = "Adatum company has already been configured to authenticate using Adatum's issuer",
                    SubscriptionKind = Models.SubscriptionKind.Premium,
                    ModelExtensionAssembly = "Adatum.ModelExtensions",
                    ModelExtensionNamespace = "Adatum.ModelExtensions",
                    IssuerIdentifier = "http://adatum/trust",
                    IssuerUrl = "https://localhost/Adatum.SimulatedIssuer.v2/",
                    IssuerThumbPrint = "f260042d59e14817984c6183fbc6bfc71baf5462",
                    ClaimType = "http://schemas.xmlsoap.org/claims/group",
                    ClaimValue = "Marketing Managers",
                    SqlAzureConnectionString =
                                            @"Server=tcp:YourDBServerName.database.windows.net,1433;Database=adatum-survey;User ID=YourUserID@YourDBServerName;Password=YourPassword;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",
                    DatabaseName = "adatum-survey.database.windows.net",
                    DatabaseUserName = "adatumuser",
                    DatabasePassword = "SecretPassword"
                });
            }

            if (await this.GetTenantAsync("fabrikam").ConfigureAwait(false) == null)
            {
                await this.SaveTenantAsync(new Tenant
                {
                    TenantId = "Fabrikam",
                    HostGeoLocation = "Anywhere US",
                    WelcomeText = "Fabrikam company has already been configured to authenticate using Fabrikam's issuer",
                    SubscriptionKind = Models.SubscriptionKind.Premium,
                    ModelExtensionAssembly = "Fabrikam.ModelExtensions",
                    ModelExtensionNamespace = "Fabrikam.ModelExtensions",
                    IssuerIdentifier = "http://fabrikam/trust",
                    IssuerUrl = "https://localhost/Fabrikam.SimulatedIssuer.v2/",
                    IssuerThumbPrint = "d2316a731b59683e744109278c80e2614503b17e",
                    ClaimType = "http://schemas.xmlsoap.org/claims/group",
                    ClaimValue = "Marketing Managers",
                    SqlAzureConnectionString = string.Empty
                }).ConfigureAwait(false);
            }
        }

        public async Task<Tenant> GetTenantAsync(string tenantId)
        {
            Func<Task<Tenant>> resolver = async () => await this.tenantBlobContainer.GetAsync(tenantId.ToLowerInvariant()).ConfigureAwait(false);

            if (this.CacheEnabled)
            {
                return await TenantCacheHelper.GetFromCacheAsync(tenantId, TenantAccountTag, resolver).ConfigureAwait(false);
            }
            else
            {
                return await resolver().ConfigureAwait(false);
            }
        }

        public IEnumerable<string> GetTenantIds()
        {
            return this.tenantBlobContainer.GetBlobList().Select(b => b.Name);
        }

        public async Task SaveTenantAsync(Tenant tenant)
        {
            await this.tenantBlobContainer.SaveAsync(tenant.TenantId.ToLowerInvariant(), tenant).ConfigureAwait(false);

            if (this.CacheEnabled)
            {
                await TenantCacheHelper.AddToCacheAsync(tenant.TenantId, TenantAccountTag, tenant).ConfigureAwait(false);
            }
        }

        public async Task UploadLogoAsync(string tenant, byte[] logo)
        {
            await this.logosBlobContainer.SaveAsync(tenant, logo).ConfigureAwait(false);

            var tenantToUpdate = await this.tenantBlobContainer.GetAsync(tenant).ConfigureAwait(false);
            tenantToUpdate.Logo = this.logosBlobContainer.GetUri(tenant).ToString();

            await this.SaveTenantAsync(tenantToUpdate).ConfigureAwait(false);
        }

        public void Dispose()
        {
            // no-op
        }
    }
}