namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using System.Threading.Tasks;

    [TestClass]
    public class AzureBlobContainerFixture
    {
        private const string AzureBlobTestContainer = "azureblobcontainerfortest";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var azureBlobContainer = new TestAzureBlobContainer(
                CloudConfiguration.GetStorageAccount("DataConnectionString"),
                AzureBlobTestContainer);
            azureBlobContainer.EnsureExistsAsync().Wait();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var azureBlobContainer = new TestAzureBlobContainer(
                 CloudConfiguration.GetStorageAccount("DataConnectionString"),
                 AzureBlobTestContainer);
            azureBlobContainer.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task DeleteShouldRemoveTheBlob()
        {
            var objId = Guid.NewGuid().ToString();

            var azureBlobContainer = new TestAzureBlobContainer(
                CloudConfiguration.GetStorageAccount("DataConnectionString"),
                AzureBlobTestContainer);
            await azureBlobContainer.SaveAsync(objId, "testText");

            Assert.IsNotNull(await azureBlobContainer.GetAsync(objId));

            await azureBlobContainer.DeleteAsync(objId);

            Assert.IsNull(await azureBlobContainer.GetAsync(objId));
        }

        [TestMethod]
        public async Task GetShouldRetrieveTheBlob()
        {
            var objId = Guid.NewGuid().ToString();

            var azureBlobContainer = new TestAzureBlobContainer(
                CloudConfiguration.GetStorageAccount("DataConnectionString"),
                AzureBlobTestContainer);
            await azureBlobContainer.SaveAsync(objId, "testText");

            Assert.IsNotNull(await azureBlobContainer.GetAsync(objId));
        }

        [TestMethod]
        public async Task SaveShouldStoreTheBlob()
        {
            var objId = Guid.NewGuid().ToString();

            var azureBlobContainer = new TestAzureBlobContainer(
                CloudConfiguration.GetStorageAccount("DataConnectionString"),
                AzureBlobTestContainer);
            await azureBlobContainer.SaveAsync(objId, "testText");

            Assert.IsNotNull(await azureBlobContainer.GetAsync(objId));
        }

        [TestMethod]
        public async Task GetBlobListReturnsAllBlobsInContainer()
        {
            var objId1 = Guid.NewGuid().ToString();
            var objId2 = Guid.NewGuid().ToString();

            var azureBlobContainer = new TestAzureBlobContainer(
                CloudConfiguration.GetStorageAccount("DataConnectionString"),
                AzureBlobTestContainer);
            await azureBlobContainer.SaveAsync(objId1, "testText");
            await azureBlobContainer.SaveAsync(objId2, "testText");

            var blobList = azureBlobContainer.GetBlobList().Select(b => b.Name).ToList();

            CollectionAssert.Contains(blobList, objId1);
            CollectionAssert.Contains(blobList, objId2);
        }

        [TestMethod]
        public void GetUriReturnsContainerUrl()
        {
            var objId = Guid.NewGuid().ToString();

            var azureBlobContainer = new TestAzureBlobContainer(
                CloudConfiguration.GetStorageAccount("DataConnectionString"),
                AzureBlobTestContainer);
            Assert.AreEqual(
                string.Format("http://127.0.0.1:10000/devstoreaccount1/{0}/{1}", AzureBlobTestContainer, objId),
                azureBlobContainer.GetUri(objId).ToString());
        }


        [TestMethod]
        public async Task CreateNewBlob()
        {
            var azureBlobContainer = new TestAzureBlobContainer(
                CloudConfiguration.GetStorageAccount("DataConnectionString"),
                AzureBlobTestContainer);

            var objId = Guid.NewGuid().ToString();

            var text = await azureBlobContainer.GetAsync(objId);

            Assert.IsNull(text);

            await azureBlobContainer.SaveAsync(objId, "testText");

            text = await azureBlobContainer.GetAsync(objId);

            Assert.IsNotNull(text);
        }

        private class TestAzureBlobContainer : AzureBlobContainer<string>
        {
            public TestAzureBlobContainer(CloudStorageAccount account, string containerName) : base(account, containerName) { }

            protected override async Task<string> DoGetAsync(string objId)
            {

                var blob = this.Container.GetBlockBlobReference(objId);
                await blob.FetchAttributesAsync();
                return await blob.DownloadTextAsync();
            }

            protected override Task DoSaveAsync(string objId, string obj)
            {
                var blob = this.Container.GetBlockBlobReference(objId);
                return blob.UploadTextAsync(obj);
            }

        }
    }
}