namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IAzureQueue<T> where T : AzureQueueMessage
    {
        Task EnsureExistsAsync();

        Task ClearAsync();

        Task AddMessageAsync(T message);

        Task<T> GetMessageAsync();

        Task<IEnumerable<T>> GetMessagesAsync(int maxMessagesToReturn);

        Task DeleteMessageAsync(T message);
    }
}