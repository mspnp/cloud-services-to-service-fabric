namespace Tailspin.Workers.Surveys.QueueHandlers
{
    using System;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using Tailspin.Workers.Surveys.Commands;

    public class QueueHandler<T> : GenericQueueHandler<T> where T : AzureQueueMessage
    {
        private readonly IAzureQueue<T> queue;
        private TimeSpan interval;

        protected QueueHandler(IAzureQueue<T> queue)
        {
            this.queue = queue;
            this.interval = TimeSpan.FromMilliseconds(200);
        }

        public static QueueHandler<T> For(IAzureQueue<T> queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }

            return new QueueHandler<T>(queue);
        }

        public QueueHandler<T> Every(TimeSpan intervalBetweenRuns)
        {
            this.interval = intervalBetweenRuns;

            return this;
        }

        public virtual void Do(ICommand<T> command)
        {
            Task.Factory.StartNew(
                async () =>
                {
                    while (true)
                    {
                        await this.CycleAsync(command);
                    }
                },
                TaskCreationOptions.LongRunning);
        }

        protected async Task CycleAsync(ICommand<T> command)
        {
            try
            {
                await GenericQueueHandler<T>.ProcessMessagesAsync(this.queue, await this.queue.GetMessagesAsync(1), command.Run);

                // TODO: Change to Task.Await
                this.Sleep(this.interval);
            }
            catch (TimeoutException ex)
            {
                TraceHelper.TraceWarning(ex.TraceInformation());
            }
        }
    }
}
