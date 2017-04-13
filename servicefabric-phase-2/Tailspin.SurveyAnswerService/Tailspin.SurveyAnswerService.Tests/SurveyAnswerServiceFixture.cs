namespace Tailspin.SurveyAnswerService.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.SurveyAnalysisService.Client;
    using Tailspin.SurveyAnswerService.Store;
    using Tailspin.SurveyAnswerService.Client.Models;

    [TestClass]
    public class SurveyAnswerServiceFixture
    {
        private StatelessServiceContext CreateServiceContext()

        {

            return new StatelessServiceContext(
                new NodeContext(string.Empty, new NodeId(0, 0), 0, string.Empty, string.Empty),
                new Mock<ICodePackageActivationContext>().Object,
                string.Empty,
                new Uri("fabric:/Mock"),
                null,
                Guid.NewGuid(),
                0);
        }

        [TestMethod]
        public async Task SaveSurveyAnswerWithNullSurveyAnswer()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>().Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<List<string>>>().Object);

            await AssertEx.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.SaveSurveyAnswerAsync(null),
                string.Empty, null);
        }

        [TestMethod]
        public async Task SaveSurveyAnswerCreatesBlobContainerForGivenSurvey()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>().Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<List<string>>>().Object);

            var surveyAnswer = new SurveyAnswer()
            {
                SlugName = "slug-name"
            };
            await service.SaveSurveyAnswerAsync(surveyAnswer);
            mockSurveyAnswerContainerFactory.Verify(f => f("slug-name-answers"));
        }

        [TestMethod]
        public async Task SaveSurveyAnswerEnsuresBlobContainerExists()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<List<string>>>().Object);

            var surveyAnswer = new SurveyAnswer()
            {
                SlugName = "slug-name"
            };
            await service.SaveSurveyAnswerAsync(surveyAnswer);
            mockSurveyAnswerContainer.Verify(f => f.EnsureExistsAsync());
        }

        [TestMethod]
        public async Task SaveSurveyAnswerSavesAnswerInBlobContainer()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<List<string>>>().Object);

            var surveyAnswer = new SurveyAnswer()
            {
                SlugName = "slug-name"
            };

            await service.SaveSurveyAnswerAsync(surveyAnswer);
            mockSurveyAnswerContainer.Verify(f => f.SaveAsync(It.IsAny<string>(),
                It.Is<Tailspin.SurveyAnswerService.Models.SurveyAnswer>(s => s.SlugName == "slug-name")));
        }

        [TestMethod]
        public async Task SaveSurveyAnswerSavesAnswerInBlobContainerWithId()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<List<string>>>().Object);

            var surveyAnswer = new SurveyAnswer()
            {
                SlugName = "slug-name"
            };

            await service.SaveSurveyAnswerAsync(surveyAnswer);
            mockSurveyAnswerContainer.Verify(f => f.SaveAsync(It.Is<string>(s => s.Length == 19),
                It.IsAny<Tailspin.SurveyAnswerService.Models.SurveyAnswer>()));
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToAnswersListGetTheAnswersListBlob()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);

            var surveyAnswer = new SurveyAnswer()
            {
                SlugName = "slug-name"
            };

            await service.SaveSurveyAnswerAsync(surveyAnswer);
            mockSurveyAnswerListContainer.Verify(f => f.SaveAsync(It.Is<string>(s => s == surveyAnswer.SlugName),
                It.IsAny<List<string>>()), Times.Once);
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToAnswersListSavesModifiedListToTheAnswersListBlob()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);

            var answerIdsList = new List<string> { "id 1", "id 2" };
            List<string> savedList = null;
            mockSurveyAnswerListContainer.Setup(c => c.GetAsync("slug-name"))
                .ReturnsAsync(answerIdsList);
            mockSurveyAnswerListContainer.Setup(c => c.SaveAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Callback<string, List<string>>((s, l) => savedList = l)
                .Returns(Task.FromResult(0));


            var surveyAnswer = new SurveyAnswer()
            {
                SlugName = "slug-name"
            };

            await service.SaveSurveyAnswerAsync(surveyAnswer);
            Assert.AreEqual(3, savedList.Count);
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToAnswersListCreatesListWhenItDoesNotExistAndSavesIt()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);

            List<string> answerIdsList = null;
            List<string> savedList = null;
            mockSurveyAnswerListContainer.Setup(c => c.GetAsync("slug-name"))
                .ReturnsAsync(answerIdsList);
            mockSurveyAnswerListContainer.Setup(c => c.SaveAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Callback<string, List<string>>((s, l) => savedList = l)
                .Returns(Task.FromResult(0));


            var surveyAnswer = new SurveyAnswer()
            {
                SlugName = "slug-name"
            };

            await service.SaveSurveyAnswerAsync(surveyAnswer);
            Assert.AreEqual(1, savedList.Count);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextGetTheAnswersListBlob()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);

            var browsingContext = await service.GetSurveyAnswerBrowsingContextAsync("slug-name", string.Empty);
            mockSurveyAnswerListContainer.Verify(c => c.GetAsync("slug-name"), Times.Once);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsPreviousId()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);
            mockSurveyAnswerContainer.Setup(c => c.ExistsAsync())
                .Returns(Task.FromResult(true));
            mockSurveyAnswerContainer.Setup(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tailspin.SurveyAnswerService.Models.SurveyAnswer());
            mockSurveyAnswerListContainer.Setup(c => c.GetAsync("slug-name"))
                .ReturnsAsync(new List<string>()
                {
                    "id 1",
                    "id 2",
                    "id 3"
                });
            var browsingContext = await service.GetSurveyAnswerBrowsingContextAsync("slug-name", "id 2");
            Assert.AreEqual("id 1", browsingContext.PreviousAnswerId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNextId()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);
            mockSurveyAnswerContainer.Setup(c => c.ExistsAsync())
                .Returns(Task.FromResult(true));
            mockSurveyAnswerContainer.Setup(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tailspin.SurveyAnswerService.Models.SurveyAnswer());
            mockSurveyAnswerListContainer.Setup(c => c.GetAsync("slug-name"))
                .ReturnsAsync(new List<string>()
                {
                    "id 1",
                    "id 2",
                    "id 3"
                });
            var browsingContext = await service.GetSurveyAnswerBrowsingContextAsync("slug-name", "id 2");
            Assert.AreEqual("id 3", browsingContext.NextAnswerId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNullNextIdAndPreviousIdWhenListDoesNotExist()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);
            mockSurveyAnswerContainer.Setup(c => c.ExistsAsync())
                .Returns(Task.FromResult(true));
            var browsingContext = await service.GetSurveyAnswerBrowsingContextAsync("slug-name", "id");
            Assert.IsNull(browsingContext.PreviousAnswerId);
            Assert.IsNull(browsingContext.NextAnswerId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNullNextIdWhenEndOfList()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);
            mockSurveyAnswerContainer.Setup(c => c.ExistsAsync())
                .Returns(Task.FromResult(true));
            mockSurveyAnswerContainer.Setup(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tailspin.SurveyAnswerService.Models.SurveyAnswer());
            mockSurveyAnswerListContainer.Setup(c => c.GetAsync("slug-name"))
                .ReturnsAsync(new List<string>()
                {
                    "id 1"
                });
            var browsingContext = await service.GetSurveyAnswerBrowsingContextAsync("slug-name", "id 1");
            Assert.IsNull(browsingContext.NextAnswerId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextReturnsNullPreviousIdWhenInBeginingOfList()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);
            mockSurveyAnswerContainer.Setup(c => c.ExistsAsync())
                .Returns(Task.FromResult(true));
            mockSurveyAnswerContainer.Setup(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tailspin.SurveyAnswerService.Models.SurveyAnswer());
            mockSurveyAnswerListContainer.Setup(c => c.GetAsync("slug-name"))
                .ReturnsAsync(new List<string>()
                {
                    "id 1"
                });
            var browsingContext = await service.GetSurveyAnswerBrowsingContextAsync("slug-name", "id 1");
            Assert.IsNull(browsingContext.PreviousAnswerId);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextWithNullSlugName()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>().Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<List<string>>>().Object);

            await AssertEx.ThrowsExceptionAsync<ArgumentException>(
                async () => await service.GetSurveyAnswerBrowsingContextAsync(null, null),
                string.Empty, null);
        }

        [TestMethod]
        public async Task GetSurveyAnswerBrowsingContextWithEmptySlugName()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>().Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(new Mock<IAzureBlobContainer<List<string>>>().Object);

            await AssertEx.ThrowsExceptionAsync<ArgumentException>(
                async () => await service.GetSurveyAnswerBrowsingContextAsync(string.Empty, null),
                string.Empty, null);
        }

        [TestMethod]
        public async Task GetFirstSurveyAnswerIdGetTheAnswersListBlob()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var mockSurveyAnswerContainer = new Mock<IAzureBlobContainer<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainer = new Mock<IAzureBlobContainer<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);

            mockSurveyAnswerContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerContainer.Object);
            mockSurveyAnswerListContainerFactory.Setup(f => f(It.IsAny<string>()))
                .Returns(mockSurveyAnswerListContainer.Object);
            mockSurveyAnswerContainer.Setup(c => c.ExistsAsync())
                .Returns(Task.FromResult(true));
            var surveyAnswer = new Tailspin.SurveyAnswerService.Models.SurveyAnswer();
            mockSurveyAnswerContainer.Setup(c => c.GetAsync("id 1"))
                .ReturnsAsync(surveyAnswer);
            mockSurveyAnswerListContainer.Setup(c => c.GetAsync("slug-name"))
                .ReturnsAsync(new List<string>()
                {
                    "id 1"
                });
            var browsingContext = await service.GetSurveyAnswerBrowsingContextAsync("slug-name", null);
            Assert.IsNotNull(browsingContext.SurveyAnswer);
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToSurveyAnswerListWithNullSlugName()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);
            var privateService = new PrivateObject(service);
            var task = (Task)privateService.Invoke(
                "AppendSurveyAnswerIdToSurveyAnswerListAsync",
                new Type[] { typeof(string), typeof(string) },
                new object[] { null, null });
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(async () => await task, string.Empty, null);
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToSurveyAnswerListWithEmptySlugName()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);
            var privateService = new PrivateObject(service);
            var task = (Task)privateService.Invoke(
                "AppendSurveyAnswerIdToSurveyAnswerListAsync",
                new Type[] { typeof(string), typeof(string) },
                new object[] { string.Empty, null });
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(async () => await task, string.Empty, null);
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToSurveyAnswerListWithNullSurveyAnswerId()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);
            var privateService = new PrivateObject(service);
            var task = (Task)privateService.Invoke(
                "AppendSurveyAnswerIdToSurveyAnswerListAsync",
                new Type[] { typeof(string), typeof(string) },
                new object[] { "slug-name", null });
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(async () => await task, string.Empty, null);
        }

        [TestMethod]
        public async Task AppendSurveyAnswerIdToSurveyAnswerListWithEmptySurveyAnswerId()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);
            var privateService = new PrivateObject(service);
            var task = (Task)privateService.Invoke(
                "AppendSurveyAnswerIdToSurveyAnswerListAsync",
                new Type[] { typeof(string), typeof(string) },
                new object[] { "slug-name", string.Empty });
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(async () => await task, string.Empty, null);
        }

        [TestMethod]
        public async Task GetSurveyAnswerListWithNullSlugName()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);
            var privateService = new PrivateObject(service);
            var task = (Task)privateService.Invoke(
                "GetSurveyAnswerListAsync",
                new Type[] { typeof(string) },
                new object[] { null });
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(async () => await task, string.Empty, null);
        }

        [TestMethod]
        public async Task GetSurveyAnswerListWithEmptySlugName()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);
            var privateService = new PrivateObject(service);
            var task = (Task)privateService.Invoke(
                "GetSurveyAnswerListAsync",
                new Type[] { typeof(string) },
                new object[] { string.Empty });
            await AssertEx.ThrowsExceptionAsync<ArgumentException>(async () => await task, string.Empty, null);
        }

        [TestMethod]
        public void CreateServiceInstanceListenersEqualsOne()
        {
            var mockSurveyAnswerContainerFactory = new Mock<AzureBlobContainerFactory<Tailspin.SurveyAnswerService.Models.SurveyAnswer>>();
            var mockSurveyAnswerListContainerFactory = new Mock<AzureBlobContainerFactory<List<string>>>();
            var service = new SurveyAnswerService(
                this.CreateServiceContext(),
                mockSurveyAnswerContainerFactory.Object,
                mockSurveyAnswerListContainerFactory.Object,
                new Mock<ISurveyAnalysisService>().Object);
            var privateService = new PrivateObject(service);
            var listeners = (IEnumerable<ServiceInstanceListener>)privateService.Invoke(
                "CreateServiceInstanceListeners");
            Assert.AreEqual(1, listeners.Count());
        }
    }
}