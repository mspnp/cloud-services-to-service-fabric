namespace Tailspin.Web.Survey.Shared.Tests.Stores
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using QueueMessages;
    using Shared.Stores;
    using Shared.Stores.AzureStorage;
    using System.Threading.Tasks;

    [TestClass]
    public class SurveyTransferStoreFixture
    {
        [TestMethod]
        public async Task TransferAddsMessageToQueue()
        {
            var mockSurveyTransferQueue = new Mock<IAzureQueue<SurveyTransferMessage>>();
            var store = new SurveyTransferStore(mockSurveyTransferQueue.Object);

            await store.TransferAsync("tenant", "slugName");

            mockSurveyTransferQueue.Verify(q => q.AddMessageAsync(It.Is<SurveyTransferMessage>(m => m.Tenant == "tenant" && m.SlugName == "slugName")));
        }

        [TestMethod]
        public async Task InitializeEnsuresMessageQueueExists()
        {
            var mockSurveyTransferQueue = new Mock<IAzureQueue<SurveyTransferMessage>>();
            var store = new SurveyTransferStore(mockSurveyTransferQueue.Object);

            await store.InitializeAsync();

            mockSurveyTransferQueue.Verify(q => q.EnsureExistsAsync());
        }
    }
}