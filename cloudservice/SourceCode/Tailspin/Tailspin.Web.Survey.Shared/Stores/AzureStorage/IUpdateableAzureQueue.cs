using System.Threading.Tasks;

namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    public interface IUpdateableAzureQueue
    {
        Task UpdateMessageAsync(AzureQueueMessage message);

        Task DeleteMessageAsync(AzureQueueMessage message);
    }
}
