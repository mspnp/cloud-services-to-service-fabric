using System.Threading.Tasks;

namespace Tailspin.Workers.Surveys.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage.Queue.Fakes;
    using Moq;
    using Tailspin.Workers.Surveys.Commands;
    using Tailspin.Workers.Surveys.QueueHandlers;
    using Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class BatchProcessingQueueHandlerFixture
    {
        [TestMethod]
        public void ForCreatesHandlerForGivenQueue()
        {
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();

            var queueHandler = BatchMultipleQueueHandler.For(mockQueue.Object, 1);

            Assert.IsInstanceOfType(queueHandler, typeof(BatchMultipleQueueHandler<MessageStub>));
        }

        [TestMethod]
        public void EveryReturnsSameHandlerForGivenQueue()
        {
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            var queueHandler = new BatchProcessingQueueHandlerStub(mockQueue.Object);

            var returnedQueueHandler = queueHandler.Every(TimeSpan.Zero);

            Assert.AreSame(queueHandler, returnedQueueHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForThrowsWhenQueueIsNull()
        {
            BatchMultipleQueueHandler.For(default(IAzureQueue<MessageStub>), 1);
        }

        [TestMethod]
        public void DoCallsPreRunForBatch()
        {
            var message1 = new MessageStub();
            var message2 = new MessageStub();
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            var queue = new Queue<IEnumerable<MessageStub>>();
            queue.Enqueue(new[] { message1, message2 });
            mockQueue.Setup(q => q.GetMessagesAsync(32)).ReturnsAsync(queue.Count > 0 ? queue.Dequeue() : new MessageStub[] { });
            var command = new Mock<IBatchCommand<MessageStub>>();
            var queueHandler = new BatchProcessingQueueHandlerStub(mockQueue.Object);

            queueHandler.Do(command.Object);

            command.Verify(c => c.PreRun(), Times.Once());
        }

        [TestMethod]
        public void DoCallsPostRunForBatch()
        {
            var message1 = new MessageStub();
            var message2 = new MessageStub();
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            var queue = new Queue<IEnumerable<MessageStub>>();
            queue.Enqueue(new[] { message1, message2 });
            mockQueue.Setup(q => q.GetMessagesAsync(32)).ReturnsAsync(queue.Count > 0 ? queue.Dequeue() : new MessageStub[] { });
            var command = new Mock<IBatchCommand<MessageStub>>();
            var queueHandler = new BatchProcessingQueueHandlerStub(mockQueue.Object);

            queueHandler.Do(command.Object);

            command.Verify(c => c.PostRun(), Times.Once());
        }

        [TestMethod]
        public void DoRunsGivenCommandForEachMessage()
        {
            var message1 = new MessageStub();
            var message2 = new MessageStub();
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            var queue = new Queue<IEnumerable<MessageStub>>();
            queue.Enqueue(new[] { message1, message2 });
            mockQueue.Setup(q => q.GetMessagesAsync(32)).ReturnsAsync(queue.Count > 0 ? queue.Dequeue() : new MessageStub[] { });
            var command = new Mock<IBatchCommand<MessageStub>>();
            var queueHandler = new BatchProcessingQueueHandlerStub(mockQueue.Object);

            queueHandler.Do(command.Object);

            command.Verify(c => c.Run(It.IsAny<MessageStub>()), Times.Exactly(2));
            command.Verify(c => c.Run(message1));
            command.Verify(c => c.Run(message2));
        }

        [TestMethod]
        public void DoDeletesMessageWhenRunIsSuccessfull()
        {
            var message = new MessageStub();
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            var queue = new Queue<IEnumerable<MessageStub>>();
            queue.Enqueue(new[] { message });
            mockQueue.Setup(q => q.GetMessagesAsync(32)).ReturnsAsync(queue.Count > 0 ? queue.Dequeue() : new MessageStub[] { });
            var command = new Mock<IBatchCommand<MessageStub>>();
            command.Setup(c => c.Run(It.IsAny<MessageStub>())).Returns(true);
            var queueHandler = new BatchProcessingQueueHandlerStub(mockQueue.Object);
            
            queueHandler.Do(command.Object);

            mockQueue.Verify(q => q.DeleteMessageAsync(message));
        }

        [TestMethod]
        public void DoDeletesMessageWhenRunIsNotSuccessfullAndMessageHasBeenDequeuedMoreThanFiveTimes()
        {
            using (ShimsContext.Create())
            {
                var message = new MessageStub();
                var mockCloudQueueMessage = new ShimCloudQueueMessage();
                mockCloudQueueMessage.DequeueCountGet = () => 6;
                message.SetMessageReference(mockCloudQueueMessage);
                var mockQueue = new Mock<IAzureQueue<MessageStub>>();
                var queue = new Queue<IEnumerable<MessageStub>>();
                queue.Enqueue(new[] { message });
                mockQueue.Setup(q => q.GetMessagesAsync(32)).ReturnsAsync(queue.Count > 0 ? queue.Dequeue() : new MessageStub[] { });
                var command = new Mock<IBatchCommand<MessageStub>>();
                command.Setup(c => c.Run(It.IsAny<MessageStub>())).Throws(new Exception("This will cause the command to fail"));
                var queueHandler = new BatchProcessingQueueHandlerStub(mockQueue.Object);

                queueHandler.Do(command.Object);

                mockQueue.Verify(q => q.DeleteMessageAsync(message));
            }
        }

        public class MessageStub : AzureQueueMessage
        {
        }

        private class BatchProcessingQueueHandlerStub : BatchMultipleQueueHandler<MessageStub>
        {
            public BatchProcessingQueueHandlerStub(IAzureQueue<MessageStub> queue)
                : base(queue, 32)
            {
            }

            public override void Do(IBatchCommand<MessageStub> batchCommand)
            {
                this.CycleAsync(batchCommand).Wait();
            }
        }
    }
}
