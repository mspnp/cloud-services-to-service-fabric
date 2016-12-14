namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System;
    using Microsoft.WindowsAzure.Storage;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class EntitiesBlobContainer<T> : AzureBlobContainer<T>
    {
        public EntitiesBlobContainer(CloudStorageAccount account)
            : base(account)
        {
        }

        public EntitiesBlobContainer(CloudStorageAccount account, string containerName)
            : base(account, containerName)
        {
        }

        protected override async Task<T> DoGetAsync(string objId)
        {
            var blob = this.Container.GetBlockBlobReference(objId);
            await blob.FetchAttributesAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(await blob.DownloadTextAsync().ConfigureAwait(false));
        }

        protected override Task DoSaveAsync(string objId, T obj)
        {
            if (string.IsNullOrWhiteSpace(objId))
            {
                throw new ArgumentNullException("objId", "ObjectId cannot be null or empty");
            }

            var blob = this.Container.GetBlockBlobReference(objId);
            blob.Properties.ContentType = "application/json";
            return blob.UploadTextAsync(JsonConvert.SerializeObject(obj));
        }
    }
}