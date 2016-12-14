namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class AzureBlobContainerFixture
    {
        private const string AzureBlobTestContainer = "azureblobcontainerfortest";
        private static CloudStorageAccount account;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var azureBlobContainer = new TestAzureBlobContainer(
                account,
                AzureBlobTestContainer);
            azureBlobContainer.EnsureExistsAsync().Wait();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var azureBlobContainer = new TestAzureBlobContainer(
                 account,
                 AzureBlobTestContainer);
            azureBlobContainer.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task DeleteShouldRemoveTheBlob()
        {
            var objId = Guid.NewGuid().ToString();

            var azureBlobContainer = new TestAzureBlobContainer(
                account,
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
                account,
                AzureBlobTestContainer);
            await azureBlobContainer.SaveAsync(objId, "testText");

            Assert.IsNotNull(await azureBlobContainer.GetAsync(objId));
        }

        [TestMethod]
        public async Task SaveShouldStoreTheBlob()
        {
            var objId = Guid.NewGuid().ToString();

            var azureBlobContainer = new TestAzureBlobContainer(
                account,
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
                account,
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
                account,
                AzureBlobTestContainer);
            Assert.AreEqual($"http://127.0.0.1:10000/devstoreaccount1/{AzureBlobTestContainer}/{objId}",
                azureBlobContainer.GetUri(objId).ToString());
        }


        [TestMethod]
        public async Task CreateNewBlob()
        {
            var azureBlobContainer = new TestAzureBlobContainer(
                account,
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