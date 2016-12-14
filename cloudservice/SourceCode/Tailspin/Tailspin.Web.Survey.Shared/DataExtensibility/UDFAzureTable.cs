namespace Tailspin.Web.Survey.Shared.DataExtensibility
{
    using System;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Extensibility;

    public class UDFAzureTable : IUDFAzureTable
    {
        private readonly CloudStorageAccount account;
        private readonly string tableName;

        public UDFAzureTable(CloudStorageAccount account, string tableName)
        {
            this.account = account;
            this.tableName = tableName;
        }

        public async Task<IModelExtension> GetExtensionByPartitionRowKeyAsync(Type entityType, string partitionKey, string rowKey)
        {
            var azureTableType = typeof(AzureTable<>).MakeGenericType(new Type[] { entityType });
            var azureTableInstance = Activator.CreateInstance(azureTableType, this.account, this.tableName);

            var extensionTask = (Task)azureTableType
                .GetMethod("GetByPartitionRowKeyAsync")
                .Invoke(azureTableInstance, new[] { partitionKey, rowKey });
           
            await extensionTask.ConfigureAwait(false);

            var taskType = typeof(Task<>).MakeGenericType(new Type[] { entityType });
            var result = taskType.GetProperty("Result").GetValue(extensionTask) as IModelExtension;
            return result;
        }

        public async Task<IEnumerable<IModelExtension>> GetExtensionsByPartitionKeyAsync(Type entityType, string partitionKey)
        {
            var azureTableType = typeof(AzureTable<>).MakeGenericType(new Type[] { entityType });
            var azureTableInstance = Activator.CreateInstance(azureTableType, this.account, this.tableName);

            var extensionTask = (Task)azureTableType
                .GetMethod("GetByPartitionKeyAsync")
                .Invoke(azureTableInstance, new[] { partitionKey });

            await extensionTask.ConfigureAwait(false);

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(new Type[] { entityType });
            var taskType = typeof(Task<>).MakeGenericType(new Type[] { enumerableType });
            var result = taskType.GetProperty("Result").GetValue(extensionTask) as IEnumerable<IModelExtension>;
            return result;
        }
        
        public async Task EnsureExistsAsync()
        {
            await new AzureTable<TableEntity>(this.account, this.tableName).EnsureExistsAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(TableEntity entity)
        {
            await new AzureTable<TableEntity>(this.account, this.tableName).DeleteAsync(entity).ConfigureAwait(false);
        }
        
        public async Task SaveAsync(TableEntity entity)
        {
            await new AzureTable<TableEntity>(this.account, this.tableName).AddAsync(entity).ConfigureAwait(false);
        }
    }
}
