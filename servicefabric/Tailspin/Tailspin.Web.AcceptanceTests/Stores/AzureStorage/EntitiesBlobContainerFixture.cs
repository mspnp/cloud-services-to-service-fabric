namespace Tailspin.Web.AcceptanceTests.Stores.AzureStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class EntitiesBlobContainerFixture
    {
        private const string SurveyAnswersContainer = "surveyanswersfortest";
        private static CloudStorageAccount account;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var surveyAnswerStorage = new EntitiesBlobContainer<SurveyAnswer>(account, SurveyAnswersContainer);
            surveyAnswerStorage.EnsureExistsAsync().Wait();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            var surveyAnswerStorage = new EntitiesBlobContainer<SurveyAnswer>(account, SurveyAnswersContainer);
            surveyAnswerStorage.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task SaveAndGet()
        {
            var surveyAnswerStorage = new EntitiesBlobContainer<SurveyAnswer>(account, SurveyAnswersContainer);
            var surveyAnswerId = new Guid("{DB0298D2-432B-41A7-B80F-7E7A025FA267}").ToString();
            var expectedSurveyAnswer = new SurveyAnswer { TenantId = "tenant", Title = "title", SlugName = "slugname" };
            var question1 = new QuestionAnswer { QuestionText = "text 1", QuestionType = QuestionType.SimpleText, PossibleAnswers = string.Empty };
            var question2 = new QuestionAnswer { QuestionText = "text 2", QuestionType = QuestionType.MultipleChoice, PossibleAnswers = "answer 1\nanswer2" };
            var question3 = new QuestionAnswer { QuestionText = "text 3", QuestionType = QuestionType.FiveStars, PossibleAnswers = string.Empty };
            (expectedSurveyAnswer.QuestionAnswers as List<QuestionAnswer>).AddRange(new[] { question1, question2, question3 });

            await surveyAnswerStorage.SaveAsync(surveyAnswerId, expectedSurveyAnswer);
            var actualSurveyAnswer = await surveyAnswerStorage.GetAsync(surveyAnswerId);

            Assert.AreEqual(expectedSurveyAnswer.TenantId, actualSurveyAnswer.TenantId);
            Assert.AreEqual(expectedSurveyAnswer.Title, actualSurveyAnswer.Title);
            Assert.AreEqual(expectedSurveyAnswer.SlugName, actualSurveyAnswer.SlugName);
            Assert.AreEqual(3, actualSurveyAnswer.QuestionAnswers.Count);
            var actualQuestionAnswer1 = actualSurveyAnswer.QuestionAnswers.SingleOrDefault(q =>
                q.QuestionText == "text 1" &&
                q.QuestionType == QuestionType.SimpleText &&
                q.PossibleAnswers == string.Empty);
            Assert.IsNotNull(actualQuestionAnswer1);
            var actualQuestionAnswer2 = actualSurveyAnswer.QuestionAnswers.SingleOrDefault(q =>
                q.QuestionText == "text 2" &&
                q.QuestionType == QuestionType.MultipleChoice &&
                q.PossibleAnswers == "answer 1\nanswer2");
            Assert.IsNotNull(actualQuestionAnswer2);
            var actualQuestionAnswer3 = actualSurveyAnswer.QuestionAnswers.SingleOrDefault(q =>
                q.QuestionText == "text 3" &&
                q.QuestionType == QuestionType.FiveStars &&
                q.PossibleAnswers == string.Empty);
            Assert.IsNotNull(actualQuestionAnswer3);
        }

        [TestMethod]
        public async Task SaveAndDelete()
        {
            var surveyAnswerStorage = new EntitiesBlobContainer<SurveyAnswer>(account, SurveyAnswersContainer);
            var surveyAnswerId = new Guid("{A0E27D4B-4CD9-43B0-B29E-FE61096A529A}").ToString();
            var expectedSurveyAnswer = new SurveyAnswer { TenantId = "tenant", Title = "title", SlugName = "slugname" };

            await surveyAnswerStorage.SaveAsync(surveyAnswerId, expectedSurveyAnswer);
            SurveyAnswer savedSurveyAnswer = await surveyAnswerStorage.GetAsync(surveyAnswerId);
            Assert.IsNotNull(savedSurveyAnswer);

            await surveyAnswerStorage.DeleteAsync(surveyAnswerId);
            SurveyAnswer deletedSurveyAnswer = await surveyAnswerStorage.GetAsync(surveyAnswerId);
            Assert.IsNull(deletedSurveyAnswer);
        }

        [TestMethod]
        public async Task GetReturnsNullWhenItDoesNotExist()
        {
            var storage = new EntitiesBlobContainer<object>(account);
            var nonExistingObject = "id-for-non-existing-object";

            object actualObject = await storage.GetAsync(nonExistingObject);

            Assert.IsNull(actualObject);
        }

        [TestMethod]
        public void GetUriThrows()
        {
            var surveyAnswerStorage = new EntitiesBlobContainer<SurveyAnswer>(account, SurveyAnswersContainer);
            Assert.IsTrue(surveyAnswerStorage.GetUri("id").ToString().EndsWith("/surveyanswersfortest/id"));
        }
    }
}
