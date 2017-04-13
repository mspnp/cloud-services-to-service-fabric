namespace Tailspin.SurveyAnswerService.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Moq;
    using Tailspin.SurveyAnalysisService.Client;
    using Tailspin.SurveyAnswerService.Store;

    [TestClass]
    public class SurveyAnswerServiceFixture
    {
        private static CloudStorageAccount account;
        private AzureBlobContainer<Models.SurveyAnswer> azureBlobSurveyAnswerContainer;
        private AzureBlobContainer<List<string>> azureBlogStringListContainer;
        private SurveyAnswerService target;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            target = new SurveyAnswerService(null,
                                (containerName) => {
                                    azureBlobSurveyAnswerContainer = new AzureBlobContainer<Models.SurveyAnswer>(
                                        account,
                                        containerName);
                                    azureBlobSurveyAnswerContainer.EnsureExistsAsync().Wait();
                                    return azureBlobSurveyAnswerContainer;
                                },
                                
                                (containerName) => {
                                    azureBlogStringListContainer = new AzureBlobContainer<List<string>>(
                                        account,
                                        containerName);
                                    azureBlogStringListContainer.EnsureExistsAsync().Wait();
                                    return azureBlogStringListContainer;
                                },
                                
                                new Mock<ISurveyAnalysisService>().Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            azureBlobSurveyAnswerContainer.DeleteContainerAsync().Wait();
            azureBlogStringListContainer.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task SaveAndGet()
        {
            var objId = Guid.NewGuid().ToString();
            var questionAnswers1 = new List<Client.Models.QuestionAnswer>();
            questionAnswers1.Add(new Client.Models.QuestionAnswer { QuestionText = "q1", QuestionType = Client.Models.QuestionType.SimpleText, Answer = "testanswer1" });
            questionAnswers1.Add(new Client.Models.QuestionAnswer { QuestionText = "q2", QuestionType = Client.Models.QuestionType.FiveStars, Answer = "3" });

            var questionAnswers2 = new List<Client.Models.QuestionAnswer>();
            questionAnswers2.Add(new Client.Models.QuestionAnswer { QuestionText = "q1", QuestionType = Client.Models.QuestionType.SimpleText, Answer = "testanswer2" });
            questionAnswers2.Add(new Client.Models.QuestionAnswer { QuestionText = "q2", QuestionType = Client.Models.QuestionType.FiveStars, Answer = "5" });

            await target.SaveSurveyAnswerAsync(new Client.Models.SurveyAnswer { SlugName = "test-title" + objId, QuestionAnswers = questionAnswers1 });
            await target.SaveSurveyAnswerAsync(new Client.Models.SurveyAnswer { SlugName = "test-title" + objId, QuestionAnswers = questionAnswers2 });

            var surveyAnswerBrowsingContext = await target.GetSurveyAnswerBrowsingContextAsync("test-title" + objId, null);

            Assert.AreEqual(2, surveyAnswerBrowsingContext.SurveyAnswer.QuestionAnswers.Count);
            Assert.AreEqual("testanswer1", surveyAnswerBrowsingContext.SurveyAnswer.QuestionAnswers.Single(a => a.QuestionText == "q1").Answer);

            Assert.IsNull(surveyAnswerBrowsingContext.PreviousAnswerId);

            var surveyAnswerBrowsingContextNext = await target.GetSurveyAnswerBrowsingContextAsync("test-title" + objId, surveyAnswerBrowsingContext.NextAnswerId);

            Assert.AreEqual(2, surveyAnswerBrowsingContextNext.SurveyAnswer.QuestionAnswers.Count);
            Assert.AreEqual("testanswer2", surveyAnswerBrowsingContextNext.SurveyAnswer.QuestionAnswers.Single(a => a.QuestionText == "q1").Answer);

            Assert.IsNull(surveyAnswerBrowsingContextNext.NextAnswerId);

        }
    }
}
