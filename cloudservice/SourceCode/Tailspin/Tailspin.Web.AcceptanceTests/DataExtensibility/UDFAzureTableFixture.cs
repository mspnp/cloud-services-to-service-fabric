namespace Tailspin.Web.AcceptanceTests.DataExtensibility
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage.Table;
    using Tailspin.Web.Survey.Extensibility;
    using Tailspin.Web.Survey.Shared.DataExtensibility;
    using Tailspin.Web.Survey.Shared.Helpers;

    [TestClass]
    public class UDFAzureTableFixture
    {
        private const string TableName = "tableForTest";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");
            new UDFAzureTable(account, TableName).EnsureExistsAsync().Wait();
        }

        [TestMethod]
        public async Task ShouldSaveAndRetrieveCustomEntity()
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");

            var key = "ShouldSaveAndRetrieveCustomEntity_RowKey";
            var customEntity = new CustomEntity()
            {
                PartitionKey = "ShouldSaveAndRetrieveCustomEntity",
                RowKey = key,
                Id = 5,
                Name = "five"
            };

            var udfAzureTable = new UDFAzureTable(account, TableName);
            await udfAzureTable.SaveAsync(customEntity);

            var storedEntity = await udfAzureTable.GetExtensionByPartitionRowKeyAsync(typeof(CustomEntity), "ShouldSaveAndRetrieveCustomEntity", key);

            Assert.IsNotNull(storedEntity);
            Assert.AreEqual(customEntity.ToString(), storedEntity.ToString());
        }

        [TestMethod]
        public async Task ShouldSaveAndRetrieveCustomEntities()
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");

            var customEntity1 = new CustomEntity()
            {
                PartitionKey = "ShouldSaveAndRetrieveCustomEntities",
                RowKey = "ShouldSaveAndRetrieveCustomEntities_RowKey1",
                Id = 6,
                Name = "six"
            };

            var customEntity2 = new CustomEntity()
            {
                PartitionKey = "ShouldSaveAndRetrieveCustomEntities",
                RowKey = "ShouldSaveAndRetrieveCustomEntities_RowKey2",
                Id = 7,
                Name = "seven"
            };

            var udfAzureTable = new UDFAzureTable(account, TableName);

            await udfAzureTable.SaveAsync(customEntity1);
            await udfAzureTable.SaveAsync(customEntity2);

            var storedEntities = await udfAzureTable.GetExtensionsByPartitionKeyAsync(typeof(CustomEntity), "ShouldSaveAndRetrieveCustomEntities");

            Assert.IsNotNull(storedEntities);
            Assert.AreEqual(customEntity1.ToString(), storedEntities.ToList()[0].ToString());
            Assert.AreEqual(customEntity2.ToString(), storedEntities.ToList()[1].ToString());
        }
        private class CustomEntity : TableEntity, IModelExtension
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return string.Format("Id: {0} - Name: {1}", this.Id, this.Name);
            }

            public bool IsChildOf(object parent)
            {
                throw new NotImplementedException();
            }
        }
    }
}
