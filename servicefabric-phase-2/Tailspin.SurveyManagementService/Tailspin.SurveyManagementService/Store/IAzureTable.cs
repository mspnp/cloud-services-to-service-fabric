namespace Tailspin.SurveyManagementService.Store
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public interface IAzureTable<T> where T : TableEntity, new()
    {
        CloudStorageAccount Account { get; }
        Task EnsureExistsAsync();
        Task<T> GetByPartitionRowKeyAsync(string partitionKey, string rowKey);
        Task<ICollection<T>> GetByStringPropertiesAsync(ICollection<KeyValuePair<string, string>> properties);
        Task<ICollection<T>> GetByPartitionKeyAsync(string partitionKey);
        Task<ICollection<T>> GetByRowKeyAsync(string rowKey);
        Task<ICollection<T>> GetLatestAsync(int upperBound);
        Task AddAsync(T obj);
        Task AddAsync(IEnumerable<T> objs);
        Task AddOrUpdateAsync(T obj);
        Task AddOrUpdateAsync(IEnumerable<T> objs);
        Task DeleteAsync(T obj);
        Task DeleteAsync(IEnumerable<T> objs);
    }
}
