namespace Tailspin.SurveyManagementService.AcceptanceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.SurveyManagementService.Models;
    using Tailspin.SurveyManagementService.Store;

    [TestClass]
    public class AzureBlobContainerTFixture
    {
        private const string SurveyContainer = "surveyfortest";
        private static CloudStorageAccount account;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var surveyStorage = new AzureBlobContainer<Survey>(account, SurveyContainer);
            surveyStorage.EnsureExistsAsync().Wait();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var surveyStorage = new AzureBlobContainer<Survey>(account, SurveyContainer);
            surveyStorage.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task SaveAndGet()
        {
            var surveyStorage = new AzureBlobContainer<Survey>(account, SurveyContainer);
            var expectedSurvey = new Survey { Title = "title", SlugName = "slugname" };
            var question1 = new Question { Text = "text 1", Type = "SimpleText", PossibleAnswers = string.Empty };
            var question2 = new Question { Text = "text 2", Type = "MultipleChoice", PossibleAnswers = "answer 1\nanswer2" };
            var question3 = new Question { Text = "text 3", Type = "FiveStars", PossibleAnswers = string.Empty };
            (expectedSurvey.Questions as List<Question>).AddRange(new[] { question1, question2, question3 });

            await surveyStorage.SaveAsync(expectedSurvey.SlugName, expectedSurvey);
            var actualSurvey = await surveyStorage.GetAsync(expectedSurvey.SlugName);

            Assert.AreEqual(expectedSurvey.Title, actualSurvey.Title);
            Assert.AreEqual(expectedSurvey.SlugName, actualSurvey.SlugName);
            Assert.AreEqual(3, actualSurvey.Questions.Count);
            var actualQuestionAnswer1 = actualSurvey.Questions.SingleOrDefault(q =>
                q.Text == "text 1" &&
                q.Type == "SimpleText" &&
                q.PossibleAnswers == string.Empty);
            Assert.IsNotNull(actualQuestionAnswer1);
            var actualQuestionAnswer2 = actualSurvey.Questions.SingleOrDefault(q =>
                q.Text == "text 2" &&
                q.Type == "MultipleChoice" &&
                q.PossibleAnswers == "answer 1\nanswer2");
            Assert.IsNotNull(actualQuestionAnswer2);
            var actualQuestionAnswer3 = actualSurvey.Questions.SingleOrDefault(q =>
                q.Text == "text 3" &&
                q.Type == "FiveStars" &&
                q.PossibleAnswers == string.Empty);
            Assert.IsNotNull(actualQuestionAnswer3);
        }

        [TestMethod]
        public async Task SaveAndDelete()
        {
            var surveyStorage = new AzureBlobContainer<Survey>(account, SurveyContainer);
            var expectedSurvey = new Survey { Title = "title", SlugName = "slugname" };

            await surveyStorage.SaveAsync(expectedSurvey.SlugName, expectedSurvey);
            Survey savedSurvey = await surveyStorage.GetAsync(expectedSurvey.SlugName);
            Assert.IsNotNull(savedSurvey);

            await surveyStorage.DeleteAsync(expectedSurvey.SlugName);
            Survey deletedSurveyAnswer = await surveyStorage.GetAsync(expectedSurvey.SlugName);
            Assert.IsNull(deletedSurveyAnswer);
        }

        [TestMethod]
        public async Task GetReturnsNullWhenItDoesNotExist()
        {
            var storage = new AzureBlobContainer<object>(account, SurveyContainer);
            var nonExistingObject = "id-for-non-existing-object";

            object actualObject = await storage.GetAsync(nonExistingObject);

            Assert.IsNull(actualObject);
        }

        [TestMethod]
        public void GetUriThrows()
        {
            var surveyStorage = new AzureBlobContainer<Survey>(account, SurveyContainer);
            Assert.IsTrue(surveyStorage.GetUri("id").ToString().EndsWith("/surveyfortest/id"));
        }
    }
}
