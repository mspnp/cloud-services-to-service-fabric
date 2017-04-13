namespace Tailspin.SurveyManagementService.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.SurveyManagementService.Client.Models;
    using Tailspin.SurveyManagementService.Models;
    using Tailspin.SurveyManagementService.Store;

    [TestClass]
    public class SurveyManagementServiceFixture
    {
        [TestMethod]
        public async Task GetSurveyFiltersBySlugName()
        {
            string slugName = "slug-name";
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            var surveyModel = new Models.Survey { SlugName = slugName };
            mock.Setup(t => t.GetAsync(slugName)).ReturnsAsync(surveyModel).Verifiable();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync(slugName);

            Assert.IsNotNull(survey);
            mock.Verify();
            Assert.AreEqual(surveyModel.SlugName, survey.SlugName);
        }

        [TestMethod]
        public async Task GetSurveyReturnsTitle()
        {
            string slugName = "slug-name";
            var surveyModel = new Models.Survey { Title = "title" };
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            mock.Setup(t => t.GetAsync(slugName)).ReturnsAsync(surveyModel).Verifiable();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync(slugName);

            Assert.AreEqual("title", survey.Title);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsCreatedOn()
        {
            string slugName = "slug-name";
            var expectedDate = new DateTime(2000, 1, 1);
            var surveyModel = new Models.Survey { CreatedOn = expectedDate };
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            mock.Setup(t => t.GetAsync(slugName)).ReturnsAsync(surveyModel).Verifiable();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync(slugName);

            Assert.AreEqual(expectedDate, survey.CreatedOn);
        }

        [TestMethod]
        public async Task GetSurveyReturnsNullWhenNotFound()
        {
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync("slug-name");

            Assert.IsNull(survey);
        }

        [TestMethod]
        public async Task GetSurveyReturnsWithQuestionsFilteredBySlugName()
        {
            string slugName = "slug-name";
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            var question = new Models.Question { Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { question };
            var surveyModel = new Models.Survey { Questions = questions };
            mock.Setup(t => t.GetAsync(slugName)).ReturnsAsync(surveyModel).Verifiable();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync(slugName);

            Assert.AreEqual(1, survey.Questions.Count);
        }

        [TestMethod]
        public async Task GetSurveyReturnsWithQuestionText()
        {
            string slugName = "slug-name";
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            var question = new Models.Question { Text = "text", Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { question };
            var surveyModel = new Models.Survey { Questions = questions };
            mock.Setup(t => t.GetAsync(slugName)).ReturnsAsync(surveyModel).Verifiable();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync(slugName);

            Assert.AreEqual("text", survey.Questions.First().Text);
        }

        [TestMethod]
        public async Task GetSurveyReturnsWithQuestionType()
        {
            string slugName = "slug-name";
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            var question = new Models.Question { Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { question };
            var surveyModel = new Models.Survey { Questions = questions };
            mock.Setup(t => t.GetAsync(slugName)).ReturnsAsync(surveyModel).Verifiable();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync(slugName);

            Assert.AreEqual(QuestionType.SimpleText, survey.Questions.First().Type);
        }

        [TestMethod]
        public async Task GetSurveyReturnsWithQuestionPossibleAnswers()
        {
            string slugName = "slug-name";
            var mock = new Mock<IAzureBlobContainer<Models.Survey>>();
            var question = new Models.Question { PossibleAnswers = "possible answers", Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { question };
            var surveyModel = new Models.Survey { Questions = questions };
            mock.Setup(t => t.GetAsync(slugName)).ReturnsAsync(surveyModel).Verifiable();
            var target = new SurveyManagementService(null, null, (containerName) => mock.Object);

            var survey = await target.GetSurveyAsync(slugName);

            Assert.AreEqual("possible answers", survey.Questions.First().PossibleAnswers);
        }

        [TestMethod]
        public async Task PublishSurveyCallsAddFromSurveyTableWithTitle()
        {
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var mockSurveyBlobContainer = new Mock<IAzureBlobContainer<Models.Survey>>();
            mockSurveyInformationTable.Setup(t => t.GetByStringPropertiesAsync(It.IsAny<ICollection<KeyValuePair<string, string>>>()))
                .ReturnsAsync(new SurveyInformationRow[] { });

            var target = new SurveyManagementService(null, 
                                (tableName) => mockSurveyInformationTable.Object, 
                                (containerName) => mockSurveyBlobContainer.Object);
            var survey = new Client.Models.Survey { Title = "title" };

            await target.PublishSurveyAsync(survey);

            mockSurveyInformationTable.Verify(
                c => c.AddAsync(
                    It.Is<SurveyInformationRow>(s => s.Title == "title")),
                Times.Once());
        }

        [TestMethod]
        public async Task PublishSurveyCallsAddFromSurveyTableGeneratingTheSlugName()
        {
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var mockSurveyBlobContainer = new Mock<IAzureBlobContainer<Models.Survey>>();
            mockSurveyInformationTable.Setup(t => t.GetByStringPropertiesAsync(It.IsAny<ICollection<KeyValuePair<string, string>>>()))
                .ReturnsAsync(new SurveyInformationRow[] { });
            var target = new SurveyManagementService(null, 
                                (tableName) => mockSurveyInformationTable.Object, 
                                (containerName) => mockSurveyBlobContainer.Object);
            var survey = new Client.Models.Survey { Title = "title to slug" };

            await target.PublishSurveyAsync(survey);

            mockSurveyInformationTable.Verify(
                c => c.AddAsync(
                    It.Is<SurveyInformationRow>(s => s.SlugName == "title-to-slug")),
                Times.Once());
        }

        [TestMethod]
        public async Task PublishSurveyCallsAddFromSurveyTableWithAllTheQuestions()
        {
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var mockSurveyBlobContainer = new Mock<IAzureBlobContainer<Models.Survey>>();
            mockSurveyInformationTable.Setup(t => t.GetByStringPropertiesAsync(It.IsAny<ICollection<KeyValuePair<string, string>>>()))
                .ReturnsAsync(new SurveyInformationRow[] { });

            var target = new SurveyManagementService(null,
                                (tableName) => mockSurveyInformationTable.Object,
                                (containerName) => mockSurveyBlobContainer.Object);
            var survey = new Client.Models.Survey
            {
                Title = "title",
                Questions = new List<Client.Models.Question>(new[] { new Client.Models.Question(), new Client.Models.Question() })
            };

            await target.PublishSurveyAsync(survey);

            mockSurveyBlobContainer.Verify(
                c => c.SaveAsync(
                    "title",
                    It.Is<Models.Survey>(s => s.Questions.Count == 2)),
                Times.Once());
        }

        [TestMethod]
        public async Task PublishSurveyCallsSaveFromSurveyBlobWithQuestionText()
        {
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var mockSurveyBlobContainer = new Mock<IAzureBlobContainer<Models.Survey>>();
            mockSurveyInformationTable.Setup(t => t.GetByStringPropertiesAsync(It.IsAny<ICollection<KeyValuePair<string, string>>>()))
                .ReturnsAsync(new SurveyInformationRow[] { });

            var target = new SurveyManagementService(null,
                                (tableName) => mockSurveyInformationTable.Object,
                                (containerName) => mockSurveyBlobContainer.Object);
            var question = new Client.Models.Question { Text = "text" };
            var survey = new Client.Models.Survey
            {
                Title = "title",
                Questions = new List<Client.Models.Question>(new[] { question })
            };

            await target.PublishSurveyAsync(survey);

            mockSurveyBlobContainer.Verify(
                c => c.SaveAsync(
                    "title",
                    It.Is<Models.Survey>(s => s.Questions.First().Text == "text")),
                Times.Once());
        }

        [TestMethod]
        public async Task PublishSurveyCallsSaveFromSurveyBlobWithQuestionType()
        {
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var mockSurveyBlobContainer = new Mock<IAzureBlobContainer<Models.Survey>>();
            mockSurveyInformationTable.Setup(t => t.GetByStringPropertiesAsync(It.IsAny<ICollection<KeyValuePair<string, string>>>()))
                .ReturnsAsync(new SurveyInformationRow[] { });

            var target = new SurveyManagementService(null,
                                (tableName) => mockSurveyInformationTable.Object,
                                (containerName) => mockSurveyBlobContainer.Object);
            var question = new Client.Models.Question { Type = QuestionType.SimpleText };
            var survey = new Client.Models.Survey
            {
                Title = "title",
                Questions = new List<Client.Models.Question>(new[] { question })
            };

            await target.PublishSurveyAsync(survey);

            mockSurveyBlobContainer.Verify(
                c => c.SaveAsync(
                    "title",
                    It.Is<Models.Survey>(s => s.Questions.First().Type == Enum.GetName(typeof(QuestionType), QuestionType.SimpleText))),
                Times.Once());
        }

        [TestMethod]
        public async Task PublishSurveyCallsSaveFromSurveyBlobWithQuestionPossibleAnswers()
        {
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var mockSurveyBlobContainer = new Mock<IAzureBlobContainer<Models.Survey>>();
            mockSurveyInformationTable.Setup(t => t.GetByStringPropertiesAsync(It.IsAny<ICollection<KeyValuePair<string, string>>>()))
                .ReturnsAsync(new SurveyInformationRow[] { });

            var target = new SurveyManagementService(null,
                                (tableName) => mockSurveyInformationTable.Object,
                                (containerName) => mockSurveyBlobContainer.Object);
            var question = new Client.Models.Question { PossibleAnswers = "possible answers" };
            var survey = new Client.Models.Survey
            {
                Title = "title",
                Questions = new List<Client.Models.Question>(new[] { question })
            };

            await target.PublishSurveyAsync(survey);

            mockSurveyBlobContainer.Verify(
                c => c.SaveAsync(
                    "title",
                    It.Is<Models.Survey>(s => s.Questions.First().PossibleAnswers == "possible answers")),
                Times.Once());
        }

        [TestMethod]
        public async Task GetRecentSurveysReturnsUpto10Surveys()
        {
            var surveyInformationRowsToReturn = new List<SurveyInformationRow>();
            for (int i = 0; i < 10; i++)
            {
                surveyInformationRowsToReturn.Add(new SurveyInformationRow());
            }

            var mock = new Mock<IAzureTable<SurveyInformationRow>>();
            mock.Setup(t => t.GetLatestAsync(10)).ReturnsAsync(surveyInformationRowsToReturn);
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();

            var target = new SurveyManagementService(null, (tableName) => mock.Object, null);

            var actualSurveys = await target.GetLatestSurveysAsync();

            Assert.AreEqual(10, actualSurveys.Count());
        }
        
        [TestMethod]
        public async Task GetRecentSurveysReturnsTitle()
        {
            var surveyInformationRow = new SurveyInformationRow { Title = "title" };
            var surveyRowsToReturn = new[] { surveyInformationRow };
            var mock = new Mock<IAzureTable<SurveyInformationRow>>();
            mock.Setup(t => t.GetLatestAsync(10)).ReturnsAsync(surveyRowsToReturn);
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var target = new SurveyManagementService(null, (tableName) => mock.Object, null);

            var actualSurveys = await target.GetLatestSurveysAsync();

            Assert.AreEqual("title", actualSurveys.First().Title);
        }

        [TestMethod]
        public async Task GetRecentSurveysReturnsSlugName()
        {
            var surveyInformationRow = new SurveyInformationRow { SlugName = "slug" };
            var surveyRowsToReturn = new[] { surveyInformationRow };
            var mock = new Mock<IAzureTable<SurveyInformationRow>>();
            mock.Setup(t => t.GetLatestAsync(10)).ReturnsAsync(surveyRowsToReturn);
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var target = new SurveyManagementService(null, (tableName) => mock.Object, null);

            var actualSurveys = await target.GetLatestSurveysAsync();

            Assert.AreEqual("slug", actualSurveys.First().SlugName);
        }

        [TestMethod]
        public async Task ListSurveysReturnsTitle()
        {
            var surveyInformationRow = new SurveyInformationRow { Title = "title" };
            var surveyRowsToReturn = new[] { surveyInformationRow };
            var mock = new Mock<IAzureTable<SurveyInformationRow>>();
            mock.Setup(t => t.GetByPartitionKeyAsync(It.IsAny<string>())).ReturnsAsync(surveyRowsToReturn);
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var target = new SurveyManagementService(null, (tableName) => mock.Object, null);

            var actualSurveys = await target.ListSurveysAsync();

            Assert.AreEqual("title", actualSurveys.First().Title);
        }

        [TestMethod]
        public async Task ListSurveysReturnsSlugName()
        {
            var surveyInformationRow = new SurveyInformationRow { SlugName = "slug" };
            var surveyRowsToReturn = new[] { surveyInformationRow };
            var mock = new Mock<IAzureTable<SurveyInformationRow>>();
            mock.Setup(t => t.GetByPartitionKeyAsync(It.IsAny<string>())).ReturnsAsync(surveyRowsToReturn);
            var mockSurveyInformationTable = new Mock<IAzureTable<SurveyInformationRow>>();
            var target = new SurveyManagementService(null, (tableName) => mock.Object, null);

            var actualSurveys = await target.ListSurveysAsync();

            Assert.AreEqual("slug", actualSurveys.First().SlugName);
        }
    }
}