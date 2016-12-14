namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using System.Threading.Tasks;

    [TestClass]
    public class FilesBlobContainerFixture
    {
        private const string LogoStoreContainer = "logostorefortest";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");
            var logoStorage = new FilesBlobContainer(account, LogoStoreContainer, "xxx");
            logoStorage.EnsureExistsAsync().Wait();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");
            var logoStorage = new FilesBlobContainer(account, LogoStoreContainer, "xxx");
            logoStorage.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task SaveAndGetData()
        {
            var objId = Guid.NewGuid().ToString();

            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");
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