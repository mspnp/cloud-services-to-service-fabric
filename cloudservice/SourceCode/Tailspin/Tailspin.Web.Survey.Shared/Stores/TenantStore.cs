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

        public TenantStore(IAzureBlobContainer<Tenant> tenantBlobContainer, IAzureBlobContainer<byte[]> logosBlobContainer)
        {
            this.tenantBlobContainer = tenantBlobContainer;
            this.logosBlobContainer = logosBlobContainer;
            this.CacheEnabled = false;
        }

        public bool CacheEnabled { get; set; }

        public async Task InitializeAsync()
        {
            await this.logosBlobContainer.EnsureExistsAsync().ConfigureAwait(false);
            await this.tenantBlobContainer.EnsureExistsAsync().ConfigureAwait(false);
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