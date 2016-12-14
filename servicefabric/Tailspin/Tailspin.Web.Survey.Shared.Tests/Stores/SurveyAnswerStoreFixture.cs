namespace Tailspin.Web.Survey.Shared.Tests.Stores
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using QueueMessages;
    using Shared.Stores;
    using Shared.Stores.AzureStorage;
    using Tailspin.Web.Survey.Shared.Models;
    using System.Threading.Tasks;

    [TestClass]
    public class SurveyAnswerStoreFixture
    {
        [TestMethod]
        public async Task SaveSurveyAnswerCreatesBlobContainerForGivenTenantAndSurvey()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                mockTenantStore.Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                null,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockTenantStore.Setup(t => t.GetTenantAsync("tenant")).ReturnsAsync(new Tenant());
            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(new Mock<IAzureBlobContainer<SurveyAnswer>>().Object);

            var surveyAnswer = new SurveyAnswer { TenantId = "tenant", SlugName = "slug-name" };
            await store.SaveSurveyAnswerAsync(surveyAnswer);

            mockTenantStore.Verify(t => t.GetTenantAsync("tenant"));
            mockSurveyContainerFactory.Verify(f => f.Create("tenant", "slug-name"));
        }

        [TestMethod]
        public async Task SaveSurveyAnswerEnsuresBlobContainerExists()
        {
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                mockTenantStore.Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                null,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockTenantStore.Setup(t => t.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(new Tenant());
            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(mockSurveyAnswerBlobContainer.Object);

            await store.SaveSurveyAnswerAsync(new SurveyAnswer());

            mockTenantStore.Verify(t => t.GetTenantAsync(It.IsAny<string>()));
            mockSurveyAnswerBlobContainer.Verify(c => c.EnsureExistsAsync());
        }

        [TestMethod]
        public async Task SaveSurveyAnswerSavesAnswerInBlobContainer()
        {
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                mockTenantStore.Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                null,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockTenantStore.Setup(t => t.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(new Tenant());
            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(mockSurveyAnswerBlobContainer.Object);

            var surveyAnswer = new SurveyAnswer();
            await store.SaveSurveyAnswerAsync(surveyAnswer);

            mockTenantStore.Verify(t => t.GetTenantAsync(It.IsAny<string>()));
            mockSurveyAnswerBlobContainer.Verify(c => c.SaveAsync(It.IsAny<string>(), It.Is<SurveyAnswer>(a => a == surveyAnswer)));
        }

        [TestMethod]
        public async Task SaveSurveyAnswerSavesAnswerInBlobContainerWithId()
        {
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                mockTenantStore.Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                null,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockTenantStore.Setup(t => t.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(new Tenant());
            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(mockSurveyAnswerBlobContainer.Object);

            await store.SaveSurveyAnswerAsync(new SurveyAnswer());

            mockTenantStore.Verify(t => t.GetTenantAsync(It.IsAny<string>()));
            mockSurveyAnswerBlobContainer.Verify(c => c.SaveAsync(It.Is<string>(s => s.Length == 19), It.IsAny<SurveyAnswer>()));
        }

        [TestMethod]
        public async Task SaveSurveyAnswerAddMessageToQueueWithSavedBlobId()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var mockSurveyAnswerStoredQueue = new Mock<IAzureQueue<SurveyAnswerStoredMessage>>();
            var store = new SurveyAnswerStore(
                mockTenantStore.Object,
                mockSurveyContainerFactory.Object, 
                mockSurveyAnswerStoredQueue.Object,
                null,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockTenantStore.Setup(t => t.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(new Tenant());
            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockSurveyAnswerBlobContainer.Object);            
            string blobId = string.Empty;
            mockSurveyAnswerBlobContainer.Setup(c => c.SaveAsync(It.IsAny<string>(), It.IsAny<SurveyAnswer>()))
                .Returns(Task.Delay(0))
                .Callback((string id, SurveyAnswer sa) => blobId = id);

            await store.SaveSurveyAnswerAsync(new SurveyAnswer());

            mockTenantStore.Verify(t => t.GetTenantAsync(It.IsAny<string>()));
            mockSurveyAnswerStoredQueue.Verify(
                q => q.AddMessageAsync(
                    It.Is<SurveyAnswerStoredMessage>(m => m.SurveyAnswerBlobId == blobId)));
        }

        [TestMethod]
        public async Task SaveSurveyAnswerAddMessageToQueueWithTenant()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var mockSurveyAnswerStoredQueue = new Mock<IAzureQueue<SurveyAnswerStoredMessage>>();
            var store = new SurveyAnswerStore(
                mockTenantStore.Object,
                mockSurveyContainerFactory.Object, 
                mockSurveyAnswerStoredQueue.Object, 
                null,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockTenantStore.Setup(t => t.GetTenantAsync("tenant")).ReturnsAsync(new Tenant());
            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(new Mock<IAzureBlobContainer<SurveyAnswer>>().Object);

            await store.SaveSurveyAnswerAsync(new SurveyAnswer { TenantId = "tenant" });

            mockTenantStore.Verify(t => t.GetTenantAsync("tenant"));
            mockSurveyAnswerStoredQueue.Verify(
                q => q.AddMessageAsync(
                    It.Is<SurveyAnswerStoredMessage>(m => m.TenantId == "tenant")));
        }

        [TestMethod]
        public async Task SaveSurveyAnswerAddMessageToQueueWithSurveySlugName()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var mockSurveyAnswerStoredQueue = new Mock<IAzureQueue<SurveyAnswerStoredMessage>>();
            var store = new SurveyAnswerStore(
                mockTenantStore.Object,
                mockSurveyContainerFactory.Object, 
                mockSurveyAnswerStoredQueue.Object, 
                null,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockTenantStore.Setup(t => t.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(new Tenant());
            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(new Mock<IAzureBlobContainer<SurveyAnswer>>().Object);

            await store.SaveSurveyAnswerAsync(new SurveyAnswer { SlugName = "slug-name" });

            mockTenantStore.Verify(t => t.GetTenantAsync(It.IsAny<string>()));
            mockSurveyAnswerStoredQueue.Verify(
                q => q.AddMessageAsync(
                    It.Is<SurveyAnswerStoredMessage>(m => m.SurveySlugName == "slug-name")));
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToAnswersListGetTheAnswersListBlob()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            await store.AppendSurveyAnswerIdToAnswersListAsync("tenant", "slug-name", string.Empty);

            mockSurveyAnswerIdsListContainer.Verify(c => c.GetAsync("tenant-slug-name"), Times.Once());
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToAnswersListSavesModifiedListToTheAnswersListBlob()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object, 
                mockSurveyAnswerIdsListContainer.Object);
           
            var answerIdsList = new List<string> { "id 1", "id 2" };
            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name")).ReturnsAsync(answerIdsList);
            List<string> savedList = null;
            mockSurveyAnswerIdsListContainer.Setup(c => c.SaveAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                                            .Callback<string, List<string>>((str, l) => savedList = l)
                                            .Returns(Task.Delay(0));

            await store.AppendSurveyAnswerIdToAnswersListAsync("tenant", "slug-name", "new id");

            Assert.AreEqual(3, savedList.Count);
            Assert.AreEqual("new id", savedList.Last());
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToAnswersListCreatesListWhenItDoesNotExistAndSavesIt()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);
            
            List<string> answerIdsList = null;
            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name")).ReturnsAsync(answerIdsList);
            List<string> savedList = null;
            mockSurveyAnswerIdsListContainer.Setup(c => c.SaveAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Callback<string, List<string>>((str, l) => savedList = l)
                .Returns(Task.Delay(0));

            await store.AppendSurveyAnswerIdToAnswersListAsync("tenant", "slug-name", "new id");

            Assert.AreEqual(1, savedList.Count);
            Assert.AreEqual("new id", savedList.Last());
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextGetTheAnswersListBlob()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            await store.GetSurveyAnswerBrowsingContextAsync("tenant", "slug-name", string.Empty);

            mockSurveyAnswerIdsListContainer.Verify(c => c.GetAsync("tenant-slug-name"), Times.Once());
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsPreviousId()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name"))
                                            .ReturnsAsync(new List<string> { "id 1", "id 2", "id 3" });

            var surveyAnswerBrowsingContext = await store.GetSurveyAnswerBrowsingContextAsync("tenant", "slug-name", "id 2");

            Assert.AreEqual("id 1", surveyAnswerBrowsingContext.PreviousId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNextId()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name"))
                                            .ReturnsAsync(new List<string> { "id 1", "id 2", "id 3" });

            var surveyAnswerBrowsingContext = await store.GetSurveyAnswerBrowsingContextAsync("tenant", "slug-name", "id 2");

            Assert.AreEqual("id 3", surveyAnswerBrowsingContext.NextId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNullNextIdAndPreviousIdWhenListDoesNotExist()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name"))
                                            .ReturnsAsync(default(List<string>));

            var surveyAnswerBrowsingContext = await store.GetSurveyAnswerBrowsingContextAsync("tenant", "slug-name", "id");

            Assert.IsNull(surveyAnswerBrowsingContext.PreviousId);
            Assert.IsNull(surveyAnswerBrowsingContext.NextId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNullNextIdWhenEndOfList()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name"))
                                            .ReturnsAsync(new List<string> { "id 1" });

            var surveyAnswerBrowsingContext = await store.GetSurveyAnswerBrowsingContextAsync("tenant", "slug-name", "id 1");

            Assert.IsNull(surveyAnswerBrowsingContext.NextId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNullPreviousIdWhenInBeginingOfList()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name"))
                                            .ReturnsAsync(new List<string> { "id 1" });

            var surveyAnswerBrowsingContext = await store.GetSurveyAnswerBrowsingContextAsync("tenant", "slug-name", "id 1");

            Assert.IsNull(surveyAnswerBrowsingContext.PreviousId);
        }

        [TestMethod]
        public async Task GetFirstSurveyAnswerIdGetTheAnswersListBlob()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            await store.GetFirstSurveyAnswerIdAsync("tenant", "slug-name");

            mockSurveyAnswerIdsListContainer.Verify(c => c.GetAsync("tenant-slug-name"), Times.Once());
        }

        [TestMethod]
        public async Task GetFirstSurveyAnswerIdReturnsTheAnswerWhichAppearsFirstOnTheListWhenListIsNotEmpty()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name"))
                                            .ReturnsAsync(new List<string> { "id" });

            var id = await store.GetFirstSurveyAnswerIdAsync("tenant", "slug-name");

            Assert.AreEqual("id", id);
        }

        [TestMethod]
        public async Task GetFirstSurveyAnswerIdReturnsEmprtyStringWhenListIsEmpty()
        {
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                new Mock<ISurveyAnswerContainerFactory>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyAnswerIdsListContainer.Setup(c => c.GetAsync("tenant-slug-name"))
                                            .ReturnsAsync(default(List<string>));

            var id = await store.GetFirstSurveyAnswerIdAsync("tenant", "slug-name");

            Assert.AreEqual(string.Empty, id);
        }

        [TestMethod]
        public async Task GetSurveyAnswerCreatesBlobContainerForGivenTenantAndSurvey()
        {
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object, 
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(new Mock<IAzureBlobContainer<SurveyAnswer>>().Object);

            await store.GetSurveyAnswerAsync("tenant", "slug-name", string.Empty);

            mockSurveyContainerFactory.Verify(f => f.Create("tenant", "slug-name"));
        }

        [TestMethod]
        public async Task GetSurveyAnswerEnsuresBlobContainerExists()
        {
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object, 
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(mockSurveyAnswerBlobContainer.Object);

            await store.GetSurveyAnswerAsync("tenant", "slug-name", string.Empty);

            mockSurveyAnswerBlobContainer.Verify(c => c.EnsureExistsAsync());
        }

        [TestMethod]
        public async Task GetSurveyAnswerGetsAnswerFromBlobContainer()
        {
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(mockSurveyAnswerBlobContainer.Object);

            await store.GetSurveyAnswerAsync("tenant", "slug-name", "id");

            mockSurveyAnswerBlobContainer.Verify(c => c.GetAsync("id"));
        }

        [TestMethod]
        public async Task GetSurveyAnswerReturnsAnswerObtainedFromBlobContainer()
        {
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(mockSurveyAnswerBlobContainer.Object);
            var surveyAnswer = new SurveyAnswer();
            mockSurveyAnswerBlobContainer.Setup(c => c.GetAsync(It.IsAny<string>()))
                                         .ReturnsAsync(surveyAnswer);

            var actualSurveyAnswer = await store.GetSurveyAnswerAsync("tenant", "slug-name", string.Empty);

            Assert.AreSame(surveyAnswer, actualSurveyAnswer);
        }

        [TestMethod]
        public async Task GetSurveyAnswerIdsReturnsList()
        {
            var azureBlobContainerMock = new Mock<IAzureBlobContainer<List<string>>>();
            azureBlobContainerMock.Setup(b => b.GetAsync("tenant-slug")).ReturnsAsync(new List<string>() { "1", "2", "3" });
            
            var surveyStore = new SurveyAnswerStore(null, null, null, null, azureBlobContainerMock.Object);
            var answers = await surveyStore.GetSurveyAnswerIdsAsync("tenant", "slug");

            Assert.AreEqual(3, answers.Count());

            azureBlobContainerMock.Verify(b => b.GetAsync("tenant-slug"), Times.Once());
        }

        [TestMethod]
        public async Task DeleteSurveyAnswersCreatesBlobContainer()
        {
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object, 
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(new Mock<IAzureBlobContainer<SurveyAnswer>>().Object);

            await store.DeleteSurveyAnswersAsync("tenant", "slug-name");

            mockSurveyContainerFactory.Verify(f => f.Create("tenant", "slug-name"), Times.Once());
        }

        [TestMethod]
        public async Task DeleteSurveyAnswersCallsDeleteFromBlobContainer()
        {
            var mockSurveyAnswerBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswer>>();
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object, 
                new Mock<IAzureBlobContainer<List<string>>>().Object);

            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(mockSurveyAnswerBlobContainer.Object);

            await store.DeleteSurveyAnswersAsync("tenant", "slug-name");

            mockSurveyAnswerBlobContainer.Verify(c => c.DeleteContainerAsync(), Times.Once());
        }

        [TestMethod]
        public async Task DeleteSurveyAnswersDeletesAnswersList()
        {
            var mockSurveyContainerFactory = new Mock<ISurveyAnswerContainerFactory>();
            var mockSurveyAnswerIdsListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var store = new SurveyAnswerStore(
                new Mock<ITenantStore>().Object,
                mockSurveyContainerFactory.Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                new Mock<IAzureQueue<SurveyAnswerStoredMessage>>().Object,
                mockSurveyAnswerIdsListContainer.Object);

            mockSurveyContainerFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(new Mock<IAzureBlobContainer<SurveyAnswer>>().Object);

            await store.DeleteSurveyAnswersAsync("tenant", "slug-name");

            mockSurveyAnswerIdsListContainer.Verify(c => c.DeleteAsync("tenant-slug-name"), Times.Once());
        }
    }
}