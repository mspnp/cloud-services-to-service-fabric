namespace Tailspin.AnswerAnalysisService.QueueHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public abstract class GenericQueueHandler<T> where T : AzureQueueMessage
    {
        protected static async Task ProcessMessagesAsync(IAzureQueue<T> queue, IEnumerable<T> messages, Func<T, bool> action)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            foreach (var message in messages)
            {
                var allowDelete = false;
                var corruptMessage = false;

                try
                {
                    allowDelete = action(message);
                }
                catch (Exception ex)
                {
                    TraceHelper.TraceWarning(ex.TraceInformation());
                    allowDelete = false;
                    corruptMessage = true;
                }
                finally
                {
                    if (allowDelete || (corruptMessage && message.GetMessageReference().DequeueCount > 5))
                    {
                        await queue.DeleteMessageAsync(message).ConfigureAwait(false);
                    }
                }
            }
        }

        protected virtual void Sleep(TimeSpan interval)
        {
            Thread.Sleep(interval);
        }
    }
}