namespace Tailspin.Web.Survey.Shared.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Shared.Models;

    // ITenantStore inherits IDisposable so that it can be used with OWIN's CreatePerOwinContext
    public interface ITenantStore : IDisposable
    {
        Task InitializeAsync();
        Task<Tenant> GetTenantAsync(string tenant);
        IEnumerable<string> GetTenantIds();
        Task SaveTenantAsync(Tenant tenant);
        Task UploadLogoAsync(string tenant, byte[] logo);
    }
}