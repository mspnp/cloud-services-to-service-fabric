namespace Tailspin.Workers.Surveys.QueueHandlers
{
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public static class BatchMultipleQueueHandler
    {
        public static BatchMultipleQueueHandler<T> For<T>(IAzureQueue<T> queue, int batchSize) where T : AzureQueueMessage
        {
            return BatchMultipleQueueHandler<T>.For(queue, batchSize);
        }
    }
}