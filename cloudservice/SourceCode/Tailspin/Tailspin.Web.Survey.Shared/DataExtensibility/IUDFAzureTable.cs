namespace Tailspin.Web.Survey.Shared.DataExtensibility
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Table;
    using Extensibility;

    public interface IUDFAzureTable
    {
        Task<IModelExtension> GetExtensionByPartitionRowKeyAsync(Type entityType, string partitionKey, string rowKey);
        Task<IEnumerable<IModelExtension>> GetExtensionsByPartitionKeyAsync(Type entityType, string partitionKey);
        Task DeleteAsync(TableEntity entity);
        Task EnsureExistsAsync();
        Task SaveAsync(TableEntity entity);
    }
}
