namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class AzureTableFixture
    {
        private const string TableName = "tableForTest";
        private static CloudStorageAccount account;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var azureTable = new AzureTable<TestRow>(account, TableName);
            azureTable.EnsureExistsAsync().Wait();
        }

        [TestMethod]
        public void CreateNewInstance()
        {
            var azureTable = new AzureTable<TestRow>(account);
            Assert.IsInstanceOfType(azureTable, typeof(AzureTable<TestRow>));
        }

        [TestMethod]
        public async Task DeleteAndAddAndGet()
        {
            var azureTable = new AzureTable<TestRow>(account, TableName);
            var row = new TestRow
            {
                PartitionKey = "partition_key_DeleteAndAddAndGet",
                RowKey = "row_key_DeleteAndAddAndGet",
                Content = "content"
            };

            await azureTable.DeleteAsync(row);
            TestRow deletedRow = (await azureTable.GetByRowKeyAsync(row.RowKey)).SingleOrDefault();
            Assert.IsNull(deletedRow);

            await azureTable.AddAsync(row);
            TestRow savedRow = await azureTable.GetByPartitionRowKeyAsync(row.PartitionKey, row.RowKey);
            Assert.IsNotNull(savedRow);
            Assert.AreEqual("content", savedRow.Content);

            await azureTable.DeleteAsync(row);
            TestRow actualRow = (await azureTable.GetByRowKeyAsync(row.RowKey)).SingleOrDefault();
            Assert.IsNull(actualRow);
        }

        [TestMethod]
        public async Task SaveManyAndDeleteMany()
        {
            var azureTable = new AzureTable<TestRow>(account, TableName);
            var row1 = new TestRow { PartitionKey = "partition_key_SaveManyAndDeleteMany", RowKey = "row_key_1_SaveManyAndDeleteMany", ETag = "*" };
            var row2 = new TestRow { PartitionKey = "partition_key_SaveManyAndDeleteMany", RowKey = "row_key_2_SaveManyAndDeleteMany", ETag = "*" };

            await azureTable.DeleteAsync(new[] { row1, row2 });
            var rowsToDelete = await azureTable.GetByPartitionKeyAsync("partition_key_SaveManyAndDeleteMany");
            Assert.AreEqual(0, rowsToDelete.Count());

            row1.ETag = string.Empty;
            row2.ETag = string.Empty;
            await azureTable.AddAsync(new[] { row1, row2 });
            var insertedRows = await azureTable.GetByPartitionKeyAsync("partition_key_SaveManyAndDeleteMany");
            Assert.AreEqual(2, insertedRows.Count());

            await azureTable.DeleteAsync(new[] { row1, row2 });
            var actualRows = await azureTable.GetByPartitionKeyAsync("partition_key_SaveManyAndDeleteMany");
            Assert.AreEqual(0, actualRows.Count());
        }

        [TestMethod]
        public async Task AddOrUpdateAddsWhenNotExists()
        {
            var azureTable = new AzureTable<TestRow>(account, TableName);

            var row = new TestRow
            {
                PartitionKey = "partition_key_AddOrUpdateAddsWhenNotExists",
                RowKey = "row_key_AddOrUpdateAddsWhenNotExists",
                Content = "content"
            };

            await azureTable.DeleteAsync(row);
            TestRow deletedRow = (await azureTable.GetByRowKeyAsync(row.RowKey)).SingleOrDefault();
            Assert.IsNull(deletedRow);

            await azureTable.AddOrUpdateAsync(row);
            TestRow savedRow = await azureTable.GetByPartitionRowKeyAsync("partition_key_AddOrUpdateAddsWhenNotExists", row.RowKey);
            Assert.IsNotNull(savedRow);
            Assert.AreEqual("content", savedRow.Content);
        }

        [TestMethod]
        public async Task AddOrUpdateUpdatesWhenExists()
        {
            var azureTable = new AzureTable<TestRow>(account, TableName);

            var row = new TestRow
            {
                PartitionKey = "partition_key_AddOrUpdateUpdatesWhenExists",
                RowKey = "row_key_AddOrUpdateUpdatesWhenExists",
                Content = "content"
            };

            await azureTable.DeleteAsync(row);
            TestRow deletedRow = (await azureTable.GetByRowKeyAsync(row.RowKey)).SingleOrDefault(); 
            Assert.IsNull(deletedRow);

            await azureTable.AddAsync(row);
            TestRow savedRow = await azureTable.GetByPartitionRowKeyAsync("partition_key_AddOrUpdateUpdatesWhenExists", row.RowKey);
            Assert.IsNotNull(savedRow);
            Assert.AreEqual("content", savedRow.Content);

            row.Content = "content modified";
            await azureTable.AddOrUpdateAsync(row);
            TestRow updatedRow = await azureTable.GetByPartitionRowKeyAsync("partition_key_AddOrUpdateUpdatesWhenExists", row.RowKey);
            Assert.IsNotNull(updatedRow);
            Assert.AreEqual("content modified", updatedRow.Content);

        }

        [TestMethod]
        public async Task AddOrUpdateMany()
        {
            var azureTable = new AzureTable<TestRow>(account, TableName);
            var row1 = new TestRow { PartitionKey = "partition_key_AddOrUpdateMany", RowKey = "row_key_1_AddOrUpdateMany", Content = "content 1", ETag = "*" };
            var row2 = new TestRow { PartitionKey = "partition_key_AddOrUpdateMany", RowKey = "row_key_2_AddOrUpdateMany", Content = "content 2", ETag = "*" };

            await azureTable.DeleteAsync(new[] { row1, row2 });
            var rowsToDelete = await azureTable.GetByPartitionKeyAsync("partition_key_AddOrUpdateMany");
            Assert.AreEqual(0, rowsToDelete.Count());

            await azureTable.AddAsync(row1);
            var actualRows = await azureTable.GetByPartitionKeyAsync("partition_key_AddOrUpdateMany");
            Assert.AreEqual(1, actualRows.Count());

            row1.Content = "content modified";
            await azureTable.AddOrUpdateAsync(new[] { row1, row2 });
            var insertedRows = await azureTable.GetByPartitionKeyAsync("partition_key_AddOrUpdateMany");
            Assert.AreEqual(2, insertedRows.Count());

            TestRow updatedRow = await azureTable.GetByPartitionRowKeyAsync("partition_key_AddOrUpdateMany", row1.RowKey);
            Assert.IsNotNull(updatedRow);
            Assert.AreEqual("content modified", updatedRow.Content);

        }

        [TestMethod]
        public async Task GetByPartitionRowKey_Returns_SpecificEntity()
        {
            var azureTable = new AzureTable<TestRow>(account, TableName);
            var row1 = new TestRow { PartitionKey = "GetByPartitionRowKey_Returns_SpecificEntity", RowKey = "row_key_1_GetByPartitionRowKey_Returns_SpecificEntity", Content = "content 1", ETag = "*" };
            var row2 = new TestRow { PartitionKey = "GetByPartitionRowKey_Returns_SpecificEntity", RowKey = "row_key_2_GetByPartitionRowKey_Returns_SpecificEntity", Content = "content 2", ETag = "*" };
            var row3 = new TestRow { PartitionKey = "GetByPartitionRowKey_Returns_SpecificEntity", RowKey = "row_key_3_GetByPartitionRowKey_Returns_SpecificEntity", Content = "content 3", ETag = "*" };

            await azureTable.DeleteAsync(new[] { row1, row2, row3 });
            var rowsToDelete = await azureTable.GetByPartitionKeyAsync("GetByPartitionRowKey_Returns_SpecificEntity");
            Assert.AreEqual(0, rowsToDelete.Count());

            await azureTable.AddAsync(new[] { row1, row2, row3 });
            var retrievedRow = await azureTable.GetByPartitionRowKeyAsync("GetByPartitionRowKey_Returns_SpecificEntity", "row_key_2_GetByPartitionRowKey_Returns_SpecificEntity");
            Assert.AreEqual(row2.Content, retrievedRow.Content);
        }

        [TestMethod]
        public async Task GetByPartitionKey_Returns_SpecificEntities()
        {
            var azureTable = new AzureTable<TestRow>(account, TableName);
            var row1 = new TestRow { PartitionKey = "partition_key_1_GetByPartitionKey_Returns_SpecificEntities", RowKey = "row_key_1_GetByPartitionKey_Returns_SpecificEntities", Content = "content 1", ETag = "*" };
            var row2 = new TestRow { PartitionKey = "partition_key_2_GetByPartitionKey_Returns_SpecificEntities", RowKey = "row_key_2_GetByPartitionKey_Returns_SpecificEntities", Content = "content 2", ETag = "*" };
            var row3 = new TestRow { PartitionKey = "partition_key_2_GetByPartitionKey_Returns_SpecificEntities", RowKey = "row_key_3_GetByPartitionKey_Returns_SpecificEntities", Content = "content 3", ETag = "*" };

            await azureTable.DeleteAsync(row1);
            await azureTable.DeleteAsync(row2);
            await azureTable.DeleteAsync(row3);

            await azureTable.AddAsync(row1);
            await azureTable.AddAsync(row2);
            await azureTable.AddAsync(row3);

            var retrievedRows = await azureTable.GetByPartitionKeyAsync("partition_key_2_GetByPartitionKey_Returns_SpecificEntities");
            Assert.AreEqual(2, retrievedRows.Count());
            Assert.AreEqual(row2.Content, retrievedRows.ToList()[0].Content);
        }

        private class TestRow : TableEntity
        {
            public string Content { get; set; }
        }
    }
}
