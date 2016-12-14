namespace Tailspin.Workers.Surveys.Tests.Commands
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.QueueMessages;
    using Tailspin.Web.Survey.Shared.Stores;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using Tailspin.Workers.Surveys.Commands;

    [TestClass]
    public class UpdatingSurveyResultsSummaryCommandFixture
    {
        [TestMethod]
        public void PreRunClearsSurveyAnswersCache()
        {
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, new Mock<ISurveyAnswerStore>().Object, new Mock<ISurveyAnswersSummaryStore>().Object);

            command.PreRun();

            mockTenantSurveyProcessingInfoCache.Verify(c => c.Clear(), Times.Once());
        }

        [TestMethod]
        public void RunAddTheAnswerIdToTheListInTheStore()
        {
            var mockUpdateableQueue = new Mock<IUpdateableAzureQueue>();
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, mockSurveyAnswerStore.Object, new Mock<ISurveyAnswersSummaryStore>().Object);
            var message = new SurveyAnswerStoredMessage
            {
                TenantId = "tenant",
                SurveySlugName = "slug-name",
                SurveyAnswerBlobId = "id",
                AppendedToAnswers = false
            };
            message.SetUpdateableQueueReference(mockUpdateableQueue.Object);

            command.Run(message);

            mockSurveyAnswerStore.Verify(r => r.AppendSurveyAnswerIdToAnswersListAsync("tenant", "slug-name", "id"));
            mockUpdateableQueue.Verify(q => q.UpdateMessageAsync(message), Times.Once());
        }

        [TestMethod]
        public void RunGetsTheSurveyAnswerFromTheStore()
        {
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, mockSurveyAnswerStore.Object, new Mock<ISurveyAnswersSummaryStore>().Object);
            mockTenantSurveyProcessingInfoCache.Setup(c => c["tenant-slug-name"])
                                         .Returns(new TenantSurveyProcessingInfo("tenant", "slug-name"));
            var message = new SurveyAnswerStoredMessage
            {
                TenantId = "tenant",
                SurveySlugName = "slug-name",
                SurveyAnswerBlobId = "id",
                AppendedToAnswers = true
            };

            command.Run(message);

            mockSurveyAnswerStore.Verify(r => r.GetSurveyAnswerAsync("tenant", "slug-name", "id"));
        }

        [TestMethod]
        public void RunGetsTheSurveyAnswersSummaryFromTheCache()
        {
            var mockUpdateableQueue = new Mock<IUpdateableAzureQueue>();
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, mockSurveyAnswerStore.Object, new Mock<ISurveyAnswersSummaryStore>().Object);
            mockTenantSurveyProcessingInfoCache.Setup(c => c.ContainsKey("tenant-slug-name")).Returns(true);
            mockTenantSurveyProcessingInfoCache.Setup(c => c["tenant-slug-name"]).Returns(new TenantSurveyProcessingInfo("tenant", "slug-name"));
            var message = new SurveyAnswerStoredMessage
            {
                TenantId = "tenant",
                SurveySlugName = "slug-name",
                AppendedToAnswers = false
            };
            message.SetUpdateableQueueReference(mockUpdateableQueue.Object);

            command.Run(message);

            mockTenantSurveyProcessingInfoCache.Verify(c => c["tenant-slug-name"], Times.Once());
            mockUpdateableQueue.Verify(q => q.UpdateMessageAsync(message), Times.Once());
        }

        [TestMethod]
        public void RunCreatesNewSummaryAndAddsItToCacheWhenNotFoundInCache()
        {
            var mockUpdateableQueue = new Mock<IUpdateableAzureQueue>();
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, mockSurveyAnswerStore.Object, new Mock<ISurveyAnswersSummaryStore>().Object);
            var message = new SurveyAnswerStoredMessage
            {
                TenantId = "tenant",
                SurveySlugName = "slug-name",
                AppendedToAnswers = false
            };
            message.SetUpdateableQueueReference(mockUpdateableQueue.Object);

            mockTenantSurveyProcessingInfoCache.Setup(c => c["tenant-slug-name"])
                                         .Returns(default(TenantSurveyProcessingInfo));

            command.Run(message);

            mockTenantSurveyProcessingInfoCache.VerifySet(
                c => c["tenant-slug-name"] = It.Is<TenantSurveyProcessingInfo>(s => s.AnswersSummary.Tenant == "tenant" && s.AnswersSummary.SlugName == "slug-name"),
                Times.Once());
            mockUpdateableQueue.Verify(q => q.UpdateMessageAsync(message), Times.Once());
        }

        [TestMethod]
        public void RunAddsTheSurveyAnswerToTheSummary()
        {
            var mockTenantSurveyProcessingInfo = new Mock<TenantSurveyProcessingInfo>("tenant", "slug-name");
            var mockTenantSurveyProcessingInfoObj = mockTenantSurveyProcessingInfo.Object;
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, mockSurveyAnswerStore.Object, new Mock<ISurveyAnswersSummaryStore>().Object);
            mockTenantSurveyProcessingInfoCache.Setup(c => c.ContainsKey("tenant-slug-name")).Returns(true);
            mockTenantSurveyProcessingInfoCache.Setup(c => c["tenant-slug-name"]).Returns(mockTenantSurveyProcessingInfoObj);
            var surveyAnswer = new SurveyAnswer() { TenantId = "tenant", SlugName = "slug-name" };
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(surveyAnswer);
            var message = new SurveyAnswerStoredMessage
            {
                TenantId = "tenant",
                SurveySlugName = "slug-name",
                AppendedToAnswers = true
            };

            command.Run(message);

            Assert.AreEqual(1, mockTenantSurveyProcessingInfoObj.AnswersSummary.TotalAnswers);
        }

        [TestMethod]
        public void PostRunMergesTheSummaryFromTheReporitoryWithTheSumamryInTheCache()
        {
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockSurveyAnswersSummaryStore = new Mock<ISurveyAnswersSummaryStore>();

            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, mockSurveyAnswerStore.Object, mockSurveyAnswersSummaryStore.Object);
            var mockTenantSurveyProcessingInfo = new Mock<TenantSurveyProcessingInfo>("tenant", "slug-name");
            var mockTenantSurveyProcessingInfoObj = mockTenantSurveyProcessingInfo.Object;
            mockTenantSurveyProcessingInfoObj.AnswersSummary.TotalAnswers = 1;
            mockTenantSurveyProcessingInfoObj.AnswersSummary.QuestionAnswersSummaries.Add(new QuestionAnswersSummary() { QuestionType = QuestionType.FiveStars, AnswersSummary = "1" });
            mockTenantSurveyProcessingInfoCache.Setup(c => c.Values).Returns(new[] { mockTenantSurveyProcessingInfoObj });

            command.PostRun();

            mockSurveyAnswersSummaryStore.Verify(s => s.MergeSurveyAnswersSummaryAsync(mockTenantSurveyProcessingInfoObj.AnswersSummary), Times.Once());
        }

        [TestMethod]
        public void PostRunMergesTheSummaryToTheStore()
        {
            var mockTenantSurveyProcessingInfoCache = new Mock<IDictionary<string, TenantSurveyProcessingInfo>>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockSurveyAnswersSummaryStore = new Mock<ISurveyAnswersSummaryStore>();
            var command = new UpdatingSurveyResultsSummaryCommand(mockTenantSurveyProcessingInfoCache.Object, mockSurveyAnswerStore.Object, mockSurveyAnswersSummaryStore.Object);
            var tenantSurveyProcessingInfo = new TenantSurveyProcessingInfo("tenant", "slug-name");
            mockTenantSurveyProcessingInfoCache.Setup(c => c.Values).Returns(new[] { tenantSurveyProcessingInfo });

            command.PostRun();

            mockSurveyAnswersSummaryStore.Verify(
                r => r.MergeSurveyAnswersSummaryAsync(It.Is<SurveyAnswersSummary>(s => s == tenantSurveyProcessingInfo.AnswersSummary)),
                Times.Once());
        }
    }
}