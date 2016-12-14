using System.Threading.Tasks;

namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Moq;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class AzureQueueMessageFixture
    {
        [TestMethod]
        public void SetAndGetMessageReference()
        {
            var cloudMessage = new CloudQueueMessage("dummy");
            var queueMessage = new TestAzureQueueMessage();
            queueMessage.SetMessageReference(cloudMessage);
            Assert.AreEqual(cloudMessage, queueMessage.GetMessageReference());
        }

        [TestMethod]
        public void SetAndGetUpdateableQueueReference()
        {
            var updateableQueue = new Mock<IUpdateableAzureQueue>();
            var queueMessage = new TestAzureQueueMessage();
            queueMessage.SetUpdateableQueueReference(updateableQueue.Object);
            Assert.AreEqual(updateableQueue.Object, queueMessage.GetUpdateableQueueReference());
        }

        [TestMethod]
        public async Task UpdateQueueMessageShouldCallIUpdateableAzureQueueUpdateMessage()
        {
            var updateableQueue = new Mock<IUpdateableAzureQueue>();
            var cloudMessage = new CloudQueueMessage("dummy");
            var queueMessage = new TestAzureQueueMessage();
            queueMessage.SetMessageReference(cloudMessage);
            queueMessage.SetUpdateableQueueReference(updateableQueue.Object);
            await queueMessage.UpdateQueueMessageAsync();

            updateableQueue.Verify(q => q.UpdateMessageAsync(queueMessage), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task UpdateQueueMessageShouldThrowIfIUpdateableAzureQueueIsNull()
        {
            var queueMessage = new TestAzureQueueMessage();
            await queueMessage.UpdateQueueMessageAsync();
        }

        private class TestAzureQueueMessage : AzureQueueMessage { }
    }
}
