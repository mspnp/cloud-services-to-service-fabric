namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class FilesBlobContainerFixture
    {
        private const string LogoStoreContainer = "logostorefortest";
        private static CloudStorageAccount account;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var logoStorage = new FilesBlobContainer(account, LogoStoreContainer, "xxx");
            logoStorage.EnsureExistsAsync().Wait();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var logoStorage = new FilesBlobContainer(account, LogoStoreContainer, "xxx");
            logoStorage.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task SaveAndGetData()
        {
            var objId = Guid.NewGuid().ToString();

            var logoStorage = new FilesBlobContainer(account, LogoStoreContainer, "xxx");

            var data = new byte[] { 1, 2, 3, 4 };
            await logoStorage.SaveAsync(objId, data);

            var retrievedData = await logoStorage.GetAsync(objId);

            var result = from x in data
                         join y in retrievedData on x equals y
                         select x;

            Assert.IsTrue(data.Length == retrievedData.Length && result.Count() == data.Length);
        }
    }
}