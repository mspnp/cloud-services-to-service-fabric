namespace Tailspin.Web.Survey.Shared.Tests.Stores
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Shared.Stores;
    using Shared.Stores.AzureStorage;
    using System.Threading.Tasks;
    using Tailspin.Web.Survey.Shared.Models;

    [TestClass]
    public class SurveyAnswerSummaryStoreFixture
    {
        [TestMethod]
        public async Task SaveSurveyAnswersSummarySavesBlob()
        {
            var mockSurveyAnswersSummaryBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswersSummary>>();
            var store = new SurveyAnswersSummaryStore(mockSurveyAnswersSummaryBlobContainer.Object);
            var surveyAnswersSummary = new SurveyAnswersSummary { Tenant = "tenant", SlugName = "slug-name" };

            await store.SaveSurveyAnswersSummaryAsync(surveyAnswersSummary);

            mockSurveyAnswersSummaryBlobContainer.Verify(
                c => c.SaveAsync("tenant-slug-name", surveyAnswersSummary),
                Times.Once());
        }

        [TestMethod]
        public async Task GetSurveyAnswersSummaryGetsFromBlobContainer()
        {
            var mockSurveyAnswersSummaryBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswersSummary>>();
            var store = new SurveyAnswersSummaryStore(mockSurveyAnswersSummaryBlobContainer.Object);

            await store.GetSurveyAnswersSummaryAsync("tenant", "slug-name");

            mockSurveyAnswersSummaryBlobContainer.Verify(
                c => c.GetAsync("tenant-slug-name"),
                Times.Once());
        }

        [TestMethod]
        public async Task GetSurveyAnswersSummaryReturnsSumamryReadFromBlobContainer()
        {
            var mockSurveyAnswersSummaryBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswersSummary>>();
            var store = new SurveyAnswersSummaryStore(mockSurveyAnswersSummaryBlobContainer.Object);
            var surveyAnswersSummary = new SurveyAnswersSummary();
            mockSurveyAnswersSummaryBlobContainer.Setup(c => c.GetAsync("tenant-slug-name")).ReturnsAsync(surveyAnswersSummary);

            SurveyAnswersSummary actualSurveyAnswersSummary = await store.GetSurveyAnswersSummaryAsync("tenant", "slug-name");

            Assert.AreSame(surveyAnswersSummary, actualSurveyAnswersSummary);
        }

        [TestMethod]
        public async Task DeleteSurveyAnswersSummarySavesBlob()
        {
            var mockSurveyAnswersSummaryBlobContainer = new Mock<IAzureBlobContainer<SurveyAnswersSummary>>();
            var store = new SurveyAnswersSummaryStore(mockSurveyAnswersSummaryBlobContainer.Object);

            await store.DeleteSurveyAnswersSummaryAsync("tenant", "slug-name");

            mockSurveyAnswersSummaryBlobContainer.Verify(
                c => c.DeleteAsync("tenant-slug-name"),
                Times.Once());
        }
    }
}