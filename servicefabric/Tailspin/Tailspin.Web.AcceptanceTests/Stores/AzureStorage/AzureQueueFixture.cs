using System.Threading.Tasks;

namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class AzureQueueFixture
    {
        private static CloudStorageAccount account;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var newSurveyAnswerQueue = new AzureQueue<MessageForTests>(account);
            newSurveyAnswerQueue.EnsureExistsAsync().Wait();
        }

        [TestMethod]
        public async Task AddAndGetMessage()
        {
            var queue = new AzureQueue<MessageForTests>(account);
            var message = new MessageForTests { Content = "content" };

            await queue.ClearAsync();
            await queue.AddMessageAsync(message);
            var actualMessage = await queue.GetMessageAsync();

            Assert.AreEqual(message.Content, actualMessage.Content);
        }

        [TestMethod]
        public async Task PurgeAndGetMessageReturnNull()
        {
            var queue = new AzureQueue<MessageForTests>(account);
            var message = new MessageForTests { Content = "content" };

            await queue.AddMessageAsync(message);
            await queue.ClearAsync();
            var actualMessage = await queue.GetMessageAsync();

            Assert.IsNull(actualMessage);
        }

        [TestMethod]
        public async Task AddAndGetMessages()
        {
            var newSurveyAnswerQueue = new AzureQueue<MessageForTests>(account);
            const int maxMessagesToReturn = 2;

            await newSurveyAnswerQueue.AddMessageAsync(new MessageForTests());
            await newSurveyAnswerQueue.AddMessageAsync(new MessageForTests());
            var actualMessages = await newSurveyAnswerQueue.GetMessagesAsync(maxMessagesToReturn);

            Assert.AreEqual(2, actualMessages.Count());
        }

        [TestMethod]
        public async Task AddAndGetAndDeleteMessage()
        {
            var queue = new AzureQueue<MessageForTests>(account);
            var message = new MessageForTests { Content = "content" };

            await queue.ClearAsync();
            await queue.AddMessageAsync(message);
            var addedMessage = await queue.GetMessageAsync();
            await queue.DeleteMessageAsync(addedMessage);
            var actualMessage = await queue.GetMessageAsync();

            Assert.IsNull(actualMessage);
        }

        [TestMethod]
        public async Task AddAndDeleteMessage()
        {
            var queue = new AzureQueue<MessageForTests>(account);
            var message = new MessageForTests { Content = "content" };

            await queue.ClearAsync();
            await queue.AddMessageAsync(message);
            var retrievedMessage = await queue.GetMessageAsync();

            Assert.AreEqual(message.Content, retrievedMessage.Content);

            await queue.DeleteMessageAsync(retrievedMessage);

            var actualMessage = await queue.GetMessageAsync();

            Assert.IsNull(actualMessage);
        }

        [TestMethod]
        public async Task GetAndUpdateMessage()
        {
            var queue = new AzureQueue<MessageForTests>(account, "messagefortests", TimeSpan.FromSeconds(1));
            var message = new MessageForTests { Content = "content" };

            await queue.ClearAsync();
            await queue.AddMessageAsync(message);
            var retrievedMessage = await queue.GetMessageAsync();

            Assert.AreEqual("content", retrievedMessage.Content);

            retrievedMessage.Content = "newContent";
            await queue.UpdateMessageAsync(retrievedMessage);

            await Task.Delay(1000);
            retrievedMessage = await queue.GetMessageAsync();
            Assert.AreEqual("newContent", retrievedMessage.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateMessageThrowIfTryToUpdateOtherMessageType()
        {
            var queue = new AzureQueue<MessageForTests>(account, "messagefortests", TimeSpan.FromSeconds(1));
            await queue.UpdateMessageAsync(new OtherMessage());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateMessageThrowIfMessageRefIsNull()
        {
            var queue = new AzureQueue<MessageForTests>(account, "messagefortests", TimeSpan.FromSeconds(1));
            await queue.UpdateMessageAsync(new MessageForTests());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteMessageThrowIfMessageRefIsNull()
        {
            var queue = new AzureQueue<MessageForTests>(account);
            var message = new MessageForTests { Content = "content" };

            await queue.DeleteMessageAsync(message);
        }

        private class MessageForTests : AzureQueueMessage
        {
            public string Content { get; set; }
        }

        private class OtherMessage : AzureQueueMessage { }
    }
}
