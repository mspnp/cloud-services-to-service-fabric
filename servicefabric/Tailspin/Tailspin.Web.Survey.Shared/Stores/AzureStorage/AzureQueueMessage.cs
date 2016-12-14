using System.Threading.Tasks;

namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System;
    using Microsoft.WindowsAzure.Storage.Queue;

    public abstract class AzureQueueMessage
    {
        [NonSerialized]
        private IUpdateableAzureQueue updateableQueueReference;

        [NonSerialized]
        private CloudQueueMessage messageReference;

        public CloudQueueMessage GetMessageReference()
        {
            return this.messageReference;
        }
        
        public IUpdateableAzureQueue GetUpdateableQueueReference()
        {
            return this.updateableQueueReference;
        }

        public void SetMessageReference(CloudQueueMessage reference)
        {
            this.messageReference = reference;
        }
        
        public void SetUpdateableQueueReference(IUpdateableAzureQueue reference)
        {
            this.updateableQueueReference = reference;
        }

        public async Task DeleteQueueMessageAsync()
        {
            if (this.updateableQueueReference == null)
            {
                throw new InvalidOperationException("GetUpdateableQueueReference() cannot return null");
            }

            await this.updateableQueueReference.DeleteMessageAsync(this).ConfigureAwait(false);
        }

        public async Task UpdateQueueMessageAsync()
        {
            if (this.updateableQueueReference == null)
            {
                throw new InvalidOperationException("GetUpdateableQueueReference() cannot return null");
            }

            await this.updateableQueueReference.UpdateMessageAsync(this).ConfigureAwait(false);
        }
    }
}