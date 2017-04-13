namespace Tailspin.SurveyManagementService.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.SurveyManagementService.Client.Models;
    using Tailspin.SurveyManagementService.Store;

    [TestClass]
    public class SurveyManagementServiceFixture
    {
        private static CloudStorageAccount account;
        private AzureBlobContainer<Models.Survey> azureBlobContainer;
        private SurveyManagementService target;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            target = new SurveyManagementService(null, 
                                (tableName) => {
                                    var azureTable = new AzureTable<Models.SurveyInformationRow>(account, tableName);
                                    azureTable.EnsureExistsAsync().Wait();
                                    return azureTable;
                                }, 
                                (containerName) => {
                                    azureBlobContainer = new AzureBlobContainer<Models.Survey>(
                                        account,
                                        containerName);
                                    azureBlobContainer.EnsureExistsAsync().Wait();
                                    return azureBlobContainer;
                                });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            azureBlobContainer.DeleteContainerAsync().Wait();
        }

        [TestMethod]
        public async Task PublishAndGet()
        {
            var objId = Guid.NewGuid().ToString();
            var questions = new List<Question>();
            questions.Add(new Question { Text = "q1", Type = QuestionType.SimpleText });
            questions.Add(new Question { Text = "q2", Type = QuestionType.MultipleChoice, PossibleAnswers="a1"});
            questions.Add(new Question { Text = "q3", Type = QuestionType.FiveStars});

            var surveyInfo = await target.PublishSurveyAsync(new Survey { Title="test title"+objId , Questions=questions });

            Assert.AreEqual("test title"+objId, surveyInfo.Title);
            Assert.AreEqual("test-title"+objId, surveyInfo.SlugName);

            var survey = await target.GetSurveyAsync("test-title" + objId);

            Assert.AreEqual(3, survey.Questions.Count);

            var q1 = survey.Questions.Single(q => q.Text == "q1");
            var q2 = survey.Questions.Single(q => q.Text == "q2");
            var q3 = survey.Questions.Single(q => q.Text == "q3");

            Assert.AreEqual(QuestionType.SimpleText, q1.Type);
            Assert.AreEqual(QuestionType.MultipleChoice, q2.Type);
            Assert.AreEqual(QuestionType.FiveStars, q3.Type);
        }

        [TestMethod]
        public async Task GetLast10()
        {
            var objId = Guid.NewGuid().ToString();
            var questions = new List<Question>();
            questions.Add(new Question { Text = "q1", Type = QuestionType.SimpleText });

            //Publish 11 Surveys
            for (int i = 1; i < 12; i++)
            {
                await target.PublishSurveyAsync(new Survey { Title = $"test {i} {objId}", Questions = questions });
            }

            //Get 10 latest
            var latestsurveys = await target.GetLatestSurveysAsync();

            Assert.AreEqual(10, latestsurveys.Count);

            var q1 = latestsurveys.FirstOrDefault( s => s.SlugName.StartsWith("test-1-"));
            var q2 = latestsurveys.Single(s => s.SlugName.StartsWith("test-2-"));
            var q11 = latestsurveys.Single(s => s.SlugName.StartsWith("test-11-"));

            Assert.IsNull(q1);
            Assert.IsNotNull(q2);
            Assert.IsNotNull(q11);
        }

        [TestMethod]
        public async Task GetSurveyList()
        {
            var objId = Guid.NewGuid().ToString();
            var questions = new List<Question>();
            questions.Add(new Question { Text = "q1", Type = QuestionType.SimpleText });

            //Publish 20 Surveys
            for (int i = 0; i < 20; i++)
            {
                await target.PublishSurveyAsync(new Survey { Title = $"test {i} {objId}", Questions = questions });
            }

            var surveys = await target.ListSurveysAsync();

            Assert.IsTrue(surveys.Count > 0);
        }
    }
}