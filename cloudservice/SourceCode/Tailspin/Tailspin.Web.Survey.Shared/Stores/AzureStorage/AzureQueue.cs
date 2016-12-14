using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Newtonsoft.Json;

namespace Tailspin.Web.Survey.Shared.Stores.AzureStorage
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    public class AzureQueue<T> : IAzureQueue<T>, IUpdateableAzureQueue
        where T : AzureQueueMessage
    {
        private readonly CloudStorageAccount account;
        private readonly TimeSpan visibilityTimeout;
        private readonly CloudQueue queue;

        public AzureQueue(CloudStorageAccount account)
            : this(account, typeof(T).Name.ToLowerInvariant())
        {
        }

        public AzureQueue(CloudStorageAccount account, string queueName)
            : this(account, queueName, TimeSpan.FromSeconds(30))
        {
        }

        public AzureQueue(CloudStorageAccount account, string queueName, TimeSpan visibilityTimeout)
        {
            this.account = account;
            this.visibilityTimeout = visibilityTimeout;

            var client = this.account.CreateCloudQueueClient();

            // Get a reference to queue - TODO: does it make sense to initialize here or when needed in methods?
            this.queue = client.GetQueueReference(queueName);
        }

        private static string GetSerializedMessage(T message)
        {
            // Changed JavaScriptSerializer to JSONNet
            return JsonConvert.SerializeObject(message);
        }

        private static T GetDeserializedMessage(IUpdateableAzureQueue queue, CloudQueueMessage message)
        {
            var deserializedMessage = JsonConvert.DeserializeObject<T>(message.AsString);
            deserializedMessage.SetUpdateableQueueReference(queue);
            deserializedMessage.SetMessageReference(message);
            return deserializedMessage;
        }

        public async Task EnsureExistsAsync()
        {
            await this.queue.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            await this.queue.ClearAsync().ConfigureAwait(false);
        }

        public async Task AddMessageAsync(T message)
        {
            await this.queue.AddMessageAsync(new CloudQueueMessage(GetSerializedMessage(message))).ConfigureAwait(false);
        }

        public async Task<T> GetMessageAsync()
        {
            var message =
                await this.queue.GetMessageAsync(this.visibilityTimeout, new QueueRequestOptions(), new OperationContext())
                    .ConfigureAwait(false);

            return message == null ? default(T) : GetDeserializedMessage(this, message);
        }

        public async Task<IEnumerable<T>> GetMessagesAsync(int maxMessagesToReturn)
        {
            var messages =
                await
                    this.queue.GetMessagesAsync(maxMessagesToReturn, this.visibilityTimeout, new QueueRequestOptions(),
                        new OperationContext())
                        .ConfigureAwait(false);

            // TODO: Revisit this to verify if removing yield return implementation causes failures
            var list = new List<T>();
            messages.ForEach(m => list.Add(GetDeserializedMessage(this, m)));
            return list;
        }

        public async Task DeleteMessageAsync(AzureQueueMessage message)
        {
            if (!(message is T))
            {
                throw new ArgumentException("Message should be instance of T", nameof(message));
            }

            await this.DeleteMessageAsync((T) message);
        }

        public async Task DeleteMessageAsync(T message)
        {
            var messageRef = message.GetMessageReference();
            if (messageRef == null)
            {
                throw new ArgumentException("Message reference cannot be null", nameof(messageRef));
            }

            await this.queue.DeleteMessageAsync(messageRef.Id, messageRef.PopReceipt).ConfigureAwait(false);
        }

        public async Task UpdateMessageAsync(AzureQueueMessage message)
        {
            if (!(message is T))
            {
                throw new ArgumentException("Message should be instance of T", nameof(message));
            }

            var messageRef = message.GetMessageReference();
            if (messageRef == null)
            {
                throw new ArgumentException("Message reference cannot be null", nameof(messageRef));
            }

            messageRef.SetMessageContent(GetSerializedMessage((T)message));
            await this.queue.UpdateMessageAsync(messageRef, this.visibilityTimeout,
                MessageUpdateFields.Visibility | MessageUpdateFields.Content).ConfigureAwait(false);
        }
    }
}