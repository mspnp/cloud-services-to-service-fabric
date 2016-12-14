namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Tailspin.Web.Survey.Shared.Helpers;

    public class AzureTable<T> : IAzureTable<T> where T : TableEntity, new ()
    {
        private readonly string tableName;
        private readonly CloudStorageAccount account;
        private readonly CloudTableClient tableClient;
        private readonly CloudTable table;

        public AzureTable(CloudStorageAccount account)
            : this(account, typeof(T).Name)
        {
        }

        public AzureTable(CloudStorageAccount account, string tableName)
        {
            this.tableName = tableName;
            this.account = account;
            tableClient = account.CreateCloudTableClient();
            table = tableClient.GetTableReference(tableName);
        }

        public CloudStorageAccount Account
        {
            get
            {
                return this.account;
            }
        }

        public async Task EnsureExistsAsync()
        {
            await table.CreateIfNotExistsAsync().ConfigureAwait(false);
        }
        
        public async Task<T> GetByPartitionRowKeyAsync(string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            return (T)retrievedResult.Result;
        }

        public async Task<IEnumerable<T>> GetByPartitionKeyAsync(string partitionKey)
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            return await ExecuteSegmentedTableQueryAsync(query).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetByRowKeyAsync(string rowKey)
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));
            return await ExecuteSegmentedTableQueryAsync(query).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetLatestAsync(int upperBound)
        {
            var query = new TableQuery<T>().Take(upperBound);
            return await ExecuteSegmentedTableQueryAsync(query).ConfigureAwait(false);
        }

        private async Task<List<T>> ExecuteSegmentedTableQueryAsync(TableQuery<T> query)
        {
            TableQuerySegment<T> querySegment = null;
            var tableEntityList = new List<TableEntity>();
            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                var tableContinuationToken = (querySegment != null) ? querySegment.ContinuationToken : null; 
                querySegment = await table.ExecuteQuerySegmentedAsync(query, tableContinuationToken).ConfigureAwait(false);
                tableEntityList.AddRange(querySegment);
            }

            return tableEntityList.OfType<T>().ToList();
        }

        public async Task AddAsync(T obj)
        {
            TableOperation insertOperation = TableOperation.Insert(obj);
            try
            {
                await table.ExecuteAsync(insertOperation).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                TraceHelper.TraceError(ex.TraceInformation());
            }
        }

        public async Task AddAsync(IEnumerable<T> objs)
        {
            TableBatchOperation batchAdd = new TableBatchOperation();

            foreach (var obj in objs)
            {
                batchAdd.Insert(obj);
            }

            await table.ExecuteBatchAsync(batchAdd).ConfigureAwait(false);
        }

        public async Task AddOrUpdateAsync(T obj)
        {
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(obj);

            try
            {
                await table.ExecuteAsync(insertOrReplaceOperation).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                TraceHelper.TraceError(ex.TraceInformation());
            }
        }

        public async Task AddOrUpdateAsync(IEnumerable<T> objs)
        {
            TableBatchOperation batchOperation = new TableBatchOperation();

            foreach (var obj in objs)
            {
                batchOperation.InsertOrReplace(obj);
            }

            await table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
        }

        public async Task DeleteAsync(T obj)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(obj.PartitionKey, obj.RowKey);

            // Execute the operation.
            var retrievedResult = await table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            var deleteEntity = (T)retrievedResult.Result;

            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                await table.ExecuteAsync(deleteOperation).ConfigureAwait(false);

                TraceHelper.TraceInformation($"Entity deleted. PartitionKey:{obj.PartitionKey} RowKey:{obj.RowKey}");
            }
            else
            {
                TraceHelper.TraceWarning($"Could not retrieve the entity for deletion. PartitionKey:{obj.PartitionKey} RowKey:{obj.RowKey}");
            }
        }

        public async Task DeleteAsync(IEnumerable<T> objs)
        {
            TableBatchOperation batchDelete = new TableBatchOperation();
            foreach (var obj in objs)
            {
                batchDelete.Delete(obj);
            }

            try
            {
                await table.ExecuteBatchAsync(batchDelete).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                TraceHelper.TraceError(ex.TraceInformation());
            }
        }
    }
}