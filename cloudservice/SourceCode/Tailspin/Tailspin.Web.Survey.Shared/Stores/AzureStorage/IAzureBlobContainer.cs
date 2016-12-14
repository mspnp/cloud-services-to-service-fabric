namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Shared.Stores.Azure;

    public interface IAzureBlobContainer<T>
    {
        Task EnsureExistsAsync();

        Task<T> GetAsync(string objId);
        IEnumerable<IListBlobItemWithName> GetBlobList();
        Uri GetUri(string objId);

        Task DeleteAsync(string objId);
        Task DeleteContainerAsync();

        Task SaveAsync(string objId, T obj);
//        Task SaveAsync(T obj);
    }
}