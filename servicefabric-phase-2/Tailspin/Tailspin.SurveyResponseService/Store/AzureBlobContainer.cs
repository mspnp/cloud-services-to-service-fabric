namespace Tailspin.SurveyResponseService.Store
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class AzureBlobContainer<T> : IAzureBlobContainer<T>
    {
        protected const int BlobRequestTimeout = 120;

        protected readonly CloudBlobContainer Container;
        protected readonly CloudStorageAccount Account;

        public AzureBlobContainer(CloudStorageAccount account, string containerName)
        {
            this.Account = account;
            var client = account.CreateCloudBlobClient();
            this.Container = client.GetContainerReference(containerName);
        }

        public virtual async Task DeleteAsync(string objId)
        {
            var blob = this.Container.GetBlobReference(objId);
            await blob.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        public virtual async Task DeleteContainerAsync()
        {
            await this.Container.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        public virtual async Task EnsureExistsAsync()
        {
            await this.Container.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        public virtual async Task<T> GetAsync(string objId)
        {
            try
            {
                var blob = this.Container.GetBlockBlobReference(objId);
                await blob.FetchAttributesAsync()
                    .ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(await blob.DownloadTextAsync()
                    .ConfigureAwait(false));
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                return default(T);
            }
        }

        public virtual IEnumerable<CloudBlockBlob> GetBlobList()
        {
            return this.Container
                .ListBlobs()
                .Cast<CloudBlockBlob>();
        }

        public virtual Uri GetUri(string objId)
        {
            CloudBlob blob = this.Container.GetBlobReference(objId);
            return blob.Uri;
        }

        public virtual async Task SaveAsync(string objId, T obj)
        {
            if (string.IsNullOrWhiteSpace(objId))
            {
                throw new ArgumentException($"{nameof(objId)} cannot null, empty, or only whitespace");
            }

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var blob = this.Container.GetBlockBlobReference(objId);
            blob.Properties.ContentType = "application/json";
            await blob.UploadTextAsync(JsonConvert.SerializeObject(obj));
        }

        public async Task<bool> ExistsAsync()
        {
            return await this.Container.ExistsAsync()
                .ConfigureAwait(false);
        }
    }
}
