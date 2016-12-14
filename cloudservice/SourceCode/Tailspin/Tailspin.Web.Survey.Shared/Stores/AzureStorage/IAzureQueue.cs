using System.Threading.Tasks;

namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System.Collections.Generic;
    using Tailspin.Web.Survey.Shared.Stores.Azure;

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