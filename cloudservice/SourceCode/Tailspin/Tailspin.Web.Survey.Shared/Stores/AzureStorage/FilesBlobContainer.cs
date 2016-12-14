namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Threading.Tasks;

    public class FilesBlobContainer : AzureBlobContainer<byte[]>
    {
        private readonly string contentType;

        public FilesBlobContainer(CloudStorageAccount account, string containerName, string contentType)
            : base(account, containerName)
        {
            this.contentType = contentType;
        }

        public override async Task EnsureExistsAsync()
        {
            await base.EnsureExistsAsync().ConfigureAwait(false);
            await this.Container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }).ConfigureAwait(false);
        }

        protected override async Task<byte[]> DoGetAsync(string objId)
        {
            var blob = this.Container.GetBlockBlobReference(objId);
            await blob.FetchAttributesAsync().ConfigureAwait(false);

            byte[] byteArray = new Byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(byteArray, 0).ConfigureAwait(false);

            return byteArray;
        }

        protected override Task DoSaveAsync(string objId, byte[] obj)
        {
            var blob = this.Container.GetBlockBlobReference(objId);
            blob.Properties.ContentType = this.contentType;
            return blob.UploadFromByteArrayAsync(obj, 0, obj.Length);
        }

    }
}