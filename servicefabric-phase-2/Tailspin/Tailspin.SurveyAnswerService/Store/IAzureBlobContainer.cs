namespace Tailspin.SurveyAnswerService.Store
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Blob;

    public interface IAzureBlobContainer<T>
    {
        Task EnsureExistsAsync();

        Task<T> GetAsync(string objId);
        IEnumerable<CloudBlockBlob> GetBlobList();
        Uri GetUri(string objId);

        Task DeleteAsync(string objId);
        Task DeleteContainerAsync();

        Task SaveAsync(string objId, T obj);

        Task<bool> ExistsAsync();
    }
}
