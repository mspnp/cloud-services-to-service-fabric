namespace Tailspin.Workers.Surveys.QueueHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using Tailspin.Workers.Surveys.Commands;
    
    public class BatchMultipleQueueHandler<T> : GenericQueueHandler<T> where T : AzureQueueMessage
    {
        private readonly IList<QueueBatchConfiguration> queuesConfiguration;
        private TimeSpan interval;

        protected BatchMultipleQueueHandler(IAzureQueue<T> queue, int batchSize)
        {
            this.queuesConfiguration = new List<QueueBatchConfiguration>();
            this.queuesConfiguration.Add(QueueBatchConfiguration.BuildConfig(queue, batchSize));
            this.interval = TimeSpan.FromMilliseconds(200);
        }

        public static BatchMultipleQueueHandler<T> For(IAzureQueue<T> queue, int batchSize)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }

            batchSize = Math.Max(1, batchSize);

            return new BatchMultipleQueueHandler<T>(queue, batchSize);
        }

        public BatchMultipleQueueHandler<T> AndFor(IAzureQueue<T> queue, int batchSize)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }

            batchSize = Math.Max(1, batchSize);

            this.queuesConfiguration.Add(QueueBatchConfiguration.BuildConfig(queue, batchSize));

            return this;
        }

        public BatchMultipleQueueHandler<T> Every(TimeSpan intervalBetweenRuns)
        {
            this.interval = intervalBetweenRuns;

            return this;
        }

        public virtual void Do(IBatchCommand<T> batchCommand)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await this.CycleAsync(batchCommand);
                }
            }, TaskCreationOptions.LongRunning);
        }

        protected async Task CycleAsync(IBatchCommand<T> batchCommand)
        {
            try
            {
                batchCommand.PreRun();

                bool continueProcessing;
                do
                {
                    continueProcessing = false;
                    foreach (var queueConfig in this.queuesConfiguration)
                    {
                        var messages = await queueConfig.Queue.GetMessagesAsync(queueConfig.BatchSize).ConfigureAwait(false);
                        await GenericQueueHandler<T>.ProcessMessagesAsync(queueConfig.Queue, messages, batchCommand.Run);
                        continueProcessing |= messages.Count() >= queueConfig.BatchSize;
                    }
                }
                while (continueProcessing);

                batchCommand.PostRun();

                this.Sleep(this.interval);
            }
            catch (TimeoutException ex)
            {
                TraceHelper.TraceWarning(ex.TraceInformation());
            }
        }

        private class QueueBatchConfiguration
        {
            public IAzureQueue<T> Queue { get; set; }
            
            public int BatchSize { get; set; }

            public static QueueBatchConfiguration BuildConfig(IAzureQueue<T> queue, int batchSize)
            {
                return new QueueBatchConfiguration() { Queue = queue, BatchSize = batchSize };
            }
        }
    }
}
