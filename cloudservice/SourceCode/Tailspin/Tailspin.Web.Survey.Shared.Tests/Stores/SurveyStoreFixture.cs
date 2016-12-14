namespace Tailspin.Web.Survey.Shared.Tests.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using Tailspin.Web.Survey.Shared.Stores.Azure;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Table;
    using Extensibility;
    using DataExtensibility;

    [TestClass]
    public class SurveyStoreFixture
    {
        [TestMethod]
        public async Task GetSurveysByTenantFiltersByTenantInPartitionKey()
        {
            var surveyRow = new SurveyRow { PartitionKey = "tenant" };
            var surveyRows = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByPartitionKeyAsync("tenant")).ReturnsAsync(surveyRows);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var surveysForTenant = await store.GetSurveysByTenantAsync("tenant");
            
            Assert.AreEqual(1, surveysForTenant.Count());
        }

        [TestMethod]
        public async Task GetSurveysByTenantReturnsTenantIdFromThePartitionKey()
        {
            var surveyRow = new SurveyRow { PartitionKey = "tenant" };
            var surveyRowsToReturn = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByPartitionKeyAsync("tenant")).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetSurveysByTenantAsync("tenant");

            Assert.AreEqual("tenant", actualSurveys.First().TenantId);
        }

        [TestMethod]
        public async Task GetSurveysByTenantReturnsTitle()
        {
            var surveyRow = new SurveyRow { PartitionKey = "tenant", Title = "title" };
            var surveyRowsToReturn = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByPartitionKeyAsync("tenant")).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetSurveysByTenantAsync("tenant");

            Assert.AreEqual("title", actualSurveys.First().Title);
        }

        [TestMethod]
        public async Task GetSurveysByTenantReturnsSlugName()
        {
            var surveyRow = new SurveyRow { PartitionKey = "tenant", SlugName = "slug" };
            var surveyRowsToReturn = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByPartitionKeyAsync("tenant")).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetSurveysByTenantAsync("tenant");

            Assert.AreEqual("slug", actualSurveys.First().SlugName);
        }

        [TestMethod]
        public async Task GetSurveysByTenantReturnsCreatedOn()
        {
            var expectedDate = new DateTime(2000, 1, 1);
            var surveyRow = new SurveyRow { PartitionKey = "tenant", CreatedOn = expectedDate };
            var surveyRowsToReturn = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByPartitionKeyAsync("tenant")).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetSurveysByTenantAsync("tenant");

            Assert.AreEqual(expectedDate, actualSurveys.First().CreatedOn);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameFiltersByTenantAndSlugNameInRowKey()
        {
            string expectedRowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedRowKey };
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(expectedRowKey)).ReturnsAsync(new[] { surveyRow }).Verifiable();
            var store = new SurveyStore(mockSurveyTable.Object, default(IAzureTable<QuestionRow>), null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", false);

            Assert.IsNotNull(survey);
            mockSurveyTable.Verify();
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsTenantIdFromPartitionKey()
        {
            string expectedRowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedRowKey, PartitionKey = "tenant" };
            var surveyRows = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByRowKeyAsync(expectedRowKey)).ReturnsAsync(surveyRows);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", false);

            Assert.AreEqual("tenant", survey.TenantId);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsTitle()
        {
            string expectedRowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedRowKey, Title = "title" };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByRowKeyAsync(expectedRowKey)).ReturnsAsync(new[] { surveyRow });
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", false);

            Assert.AreEqual("title", survey.Title);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsSlugName()
        {
            string expectedRowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedRowKey, SlugName = "slug-name" };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByRowKeyAsync(expectedRowKey)).ReturnsAsync(new[] { surveyRow });
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", false);

            Assert.AreEqual("slug-name", survey.SlugName);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsCreatedOn()
        {
            string expectedRowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var expectedDate = new DateTime(2000, 1, 1);
            var surveyRow = new SurveyRow { RowKey = expectedRowKey, CreatedOn = expectedDate };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetByRowKeyAsync(expectedRowKey)).ReturnsAsync(new[] { surveyRow });
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);
            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", false);

            Assert.AreEqual(expectedDate, survey.CreatedOn);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsNullWhenNotFound()
        {
            var mock = new Mock<IAzureTable<SurveyRow>>();
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", false);

            Assert.IsNull(survey);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsWithQuestionsFilteredByTenantAndSlugNameInPartitionKey()
        {
            string expectedKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedKey };
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(expectedKey)).ReturnsAsync(new[] { surveyRow });
            var questionRow = new QuestionRow { PartitionKey = expectedKey, Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { questionRow };
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            mockQuestionTable.Setup(t => t.GetByPartitionKeyAsync(expectedKey)).ReturnsAsync(questions);
            var store = new SurveyStore(mockSurveyTable.Object, mockQuestionTable.Object, null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", true);

            Assert.AreEqual(1, survey.Questions.Count);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsWithQuestionText()
        {
            string expectedKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedKey };
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(expectedKey)).ReturnsAsync(new[] { surveyRow });
            var questionRow = new QuestionRow { PartitionKey = expectedKey, Text = "text", Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { questionRow };
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            mockQuestionTable.Setup(t => t.GetByPartitionKeyAsync(expectedKey)).ReturnsAsync(questions);
            var store = new SurveyStore(mockSurveyTable.Object, mockQuestionTable.Object, null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", true);

            Assert.AreEqual("text", survey.Questions.First().Text);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsWithQuestionType()
        {
            string expectedKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedKey };
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(expectedKey)).ReturnsAsync(new[] { surveyRow });
            var questionRow = new QuestionRow { PartitionKey = expectedKey, Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { questionRow };
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            mockQuestionTable.Setup(t => t.GetByPartitionKeyAsync(expectedKey)).ReturnsAsync(questions);
            var store = new SurveyStore(mockSurveyTable.Object, mockQuestionTable.Object, null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", true);

            Assert.AreEqual(QuestionType.SimpleText, survey.Questions.First().Type);
        }

        [TestMethod]
        public async Task GetSurveyByTenantAndSlugNameReturnsWithQuestionPossibleAnswers()
        {
            string expectedKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedKey };
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(expectedKey)).ReturnsAsync(new[] { surveyRow });
            var questionRow = new QuestionRow { PartitionKey = expectedKey, PossibleAnswers = "possible answers", Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { questionRow };
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            mockQuestionTable.Setup(t => t.GetByPartitionKeyAsync(expectedKey)).ReturnsAsync(questions);
            var store = new SurveyStore(mockSurveyTable.Object, mockQuestionTable.Object, null);

            var survey = await store.GetSurveyByTenantAndSlugNameAsync("tenant", "slug-name", true);

            Assert.AreEqual("possible answers", survey.Questions.First().PossibleAnswers);
        }

        [TestMethod]
        public async Task DeleteSurveyByTenantAndSlugNameDeletesSurveyWithTenantAndSlugNameInRowKeyFromSurveyTable()
        {
            string expectedRowKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedRowKey };
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(expectedRowKey)).ReturnsAsync(new[] { surveyRow });
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            mockQuestionTable.Setup(t => t.GetByPartitionKeyAsync(expectedRowKey)).ReturnsAsync(new QuestionRow[] { });
            var store = new SurveyStore(mockSurveyTable.Object, mockQuestionTable.Object, null);

            await store.DeleteSurveyByTenantAndSlugNameAsync("tenant", "slug-name");

            mockSurveyTable.Verify(t => t.DeleteAsync(It.Is<SurveyRow>(s => s.RowKey == expectedRowKey)));
        }

        [TestMethod]
        public async Task DeleteSurveyByTenantAndSlugNameDeleteQuestionsByTenantAndSlugNameInPartitionKeyFromQuestionTable()
        {
            string expectedKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", "tenant", "slug-name");
            var surveyRow = new SurveyRow { RowKey = expectedKey };
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(expectedKey)).ReturnsAsync(new[] { surveyRow });
            var questionRow = new QuestionRow { PartitionKey = expectedKey, PossibleAnswers = "possible answers", Type = Enum.GetName(typeof(QuestionType), QuestionType.SimpleText) };
            var questions = new[] { questionRow };
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            mockQuestionTable.Setup(t => t.GetByPartitionKeyAsync(expectedKey)).ReturnsAsync(questions);
            IEnumerable<QuestionRow> actualQuestionsToDelete = null;
            mockQuestionTable.Setup(t => t.DeleteAsync(It.IsAny<IEnumerable<QuestionRow>>()))
                             .Returns(Task.FromResult(0))
                             .Callback<IEnumerable<QuestionRow>>(q => actualQuestionsToDelete = q);

            var store = new SurveyStore(mockSurveyTable.Object, mockQuestionTable.Object, null);

            await store.DeleteSurveyByTenantAndSlugNameAsync("tenant", "slug-name");

            Assert.AreEqual(1, actualQuestionsToDelete.Count());
            Assert.AreSame(questionRow, actualQuestionsToDelete.First());
        }

        [TestMethod]
        public async Task DeleteSurveyByTenantAndSlugNameDoesNotDeleteSurveyAndQuestionWhenSurveyDoesNotExist()
        {
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            mockSurveyTable.Setup(t => t.GetByRowKeyAsync(It.IsAny<string>())).ReturnsAsync(null);
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            mockQuestionTable.Setup(t => t.GetByPartitionKeyAsync(It.IsAny<string>())).ReturnsAsync(new QuestionRow[] { });
            var store = new SurveyStore(mockSurveyTable.Object, mockQuestionTable.Object, null);

            await store.DeleteSurveyByTenantAndSlugNameAsync("tenant", "slug-name");

            mockSurveyTable.Verify(t => t.DeleteAsync(It.IsAny<SurveyRow>()), Times.Never());
            mockQuestionTable.Verify(t => t.DeleteAsync(It.IsAny<IEnumerable<QuestionRow>>()), Times.Never());
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromSurveyTableWithTitle()
        {
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            var store = new SurveyStore(mockSurveyTable.Object, new Mock<IAzureTable<QuestionRow>>().Object, null);
            var survey = new Survey { Title = "title" };

            await store.SaveSurveyAsync(survey);

            mockSurveyTable.Verify(
                c => c.AddAsync(
                    It.Is<SurveyRow>(s => s.Title == "title")),
                Times.Once());
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromSurveyTableWithTenant()
        {
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            var store = new SurveyStore(mockSurveyTable.Object, new Mock<IAzureTable<QuestionRow>>().Object, null);
            var survey = new Survey { Title = "title", TenantId = "tenant" };

            await store.SaveSurveyAsync(survey);

            mockSurveyTable.Verify(
                c => c.AddAsync(
                    It.Is<SurveyRow>(s => s.PartitionKey == "tenant")),
                Times.Once());
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromSurveyTableGeneratingTheSlugName()
        {
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            var store = new SurveyStore(mockSurveyTable.Object, new Mock<IAzureTable<QuestionRow>>().Object, null);
            var survey = new Survey { Title = "title to slug" };

            await store.SaveSurveyAsync(survey);

            mockSurveyTable.Verify(
                c => c.AddAsync(
                    It.Is<SurveyRow>(s => s.SlugName == "title-to-slug")),
                Times.Once());
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromSurveyTableSettingTheTenantAsThePartitionKey()
        {
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            var store = new SurveyStore(mockSurveyTable.Object, new Mock<IAzureTable<QuestionRow>>().Object, null);
            var survey = new Survey { Title = "title", TenantId = "tenant" };

            await store.SaveSurveyAsync(survey);

            mockSurveyTable.Verify(
                c => c.AddAsync(
                    It.Is<SurveyRow>(s => s.PartitionKey == "tenant")),
                Times.Once());
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromSurveyTableSettingTheTenantAndSlugNameAsTheRowKey()
        {
            var mockSurveyTable = new Mock<IAzureTable<SurveyRow>>();
            var store = new SurveyStore(mockSurveyTable.Object, new Mock<IAzureTable<QuestionRow>>().Object, null);
            var survey = new Survey { Title = "title to slug", TenantId = "tenant" };

            await store.SaveSurveyAsync(survey);

            mockSurveyTable.Verify(
                c => c.AddAsync(
                    It.Is<SurveyRow>(s => s.RowKey == "tenant_title-to-slug")),
                Times.Once());
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromQuestionTableWithAllTheQuestions()
        {
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            var store = new SurveyStore(new Mock<IAzureTable<SurveyRow>>().Object, mockQuestionTable.Object, null);
            var survey = new Survey 
            { 
                Title = "title",
                Questions = new List<Question>(new[] { new Question(), new Question() })
            };
            IEnumerable<QuestionRow> actualQuestionsToAdd = null;
            mockQuestionTable.Setup(t => t.AddAsync(It.IsAny<IEnumerable<QuestionRow>>()))
                             .Returns(Task.FromResult(0))
                             .Callback<IEnumerable<QuestionRow>>(q => actualQuestionsToAdd = q);

            await store.SaveSurveyAsync(survey);

            Assert.AreEqual(2, actualQuestionsToAdd.Count());
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromQuestionTableWithQuestionText()
        {
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            var store = new SurveyStore(new Mock<IAzureTable<SurveyRow>>().Object, mockQuestionTable.Object, null);
            var question = new Question { Text = "text" };
            var survey = new Survey
            {
                Title = "title",
                Questions = new List<Question>(new[] { question })
            };
            IEnumerable<QuestionRow> actualQuestionsToAdd = null;
            mockQuestionTable.Setup(t => t.AddAsync(It.IsAny<IEnumerable<QuestionRow>>()))
                             .Returns(Task.FromResult(0))
                             .Callback<IEnumerable<QuestionRow>>(q => actualQuestionsToAdd = q);

            await store.SaveSurveyAsync(survey);

            Assert.AreEqual("text", actualQuestionsToAdd.First().Text);
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromQuestionTableWithQuestionType()
        {
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            var store = new SurveyStore(new Mock<IAzureTable<SurveyRow>>().Object, mockQuestionTable.Object, null);
            var question = new Question { Type = QuestionType.SimpleText };
            var survey = new Survey
            {
                Title = "title",
                Questions = new List<Question>(new[] { question })
            };
            IEnumerable<QuestionRow> actualQuestionsToAdd = null;
            mockQuestionTable.Setup(t => t.AddAsync(It.IsAny<IEnumerable<QuestionRow>>()))
                             .Returns(Task.FromResult(0))
                             .Callback<IEnumerable<QuestionRow>>(q => actualQuestionsToAdd = q);

            await store.SaveSurveyAsync(survey);

            Assert.AreEqual(Enum.GetName(typeof(QuestionType), QuestionType.SimpleText), actualQuestionsToAdd.First().Type);
        }

        [TestMethod]
        public async Task SaveSurveyCallsAddFromQuestionTableWithQuestionPossibleAnswers()
        {
            var mockQuestionTable = new Mock<IAzureTable<QuestionRow>>();
            var store = new SurveyStore(new Mock<IAzureTable<SurveyRow>>().Object, mockQuestionTable.Object, null);
            var question = new Question { PossibleAnswers = "possible answers" };
            var survey = new Survey
            {
                Title = "title",
                Questions = new List<Question>(new[] { question })
            };
            IEnumerable<QuestionRow> actualQuestionsToAdd = null;
            mockQuestionTable.Setup(t => t.AddAsync(It.IsAny<IEnumerable<QuestionRow>>()))
                             .Returns(Task.FromResult(0))
                             .Callback<IEnumerable<QuestionRow>>(q => actualQuestionsToAdd = q);

            await store.SaveSurveyAsync(survey);

            Assert.AreEqual("possible answers", actualQuestionsToAdd.First().PossibleAnswers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveSurveyThrowsExceptionIfTitleAndSlugNameAreNullOrEmpty()
        {
            var store = new SurveyStore(new Mock<IAzureTable<SurveyRow>>().Object, new Mock<IAzureTable<QuestionRow>>().Object, null);
            var survey = new Survey();

            await store.SaveSurveyAsync(survey);
        }

        [TestMethod]
        public async Task GetRecentSurveysReturnsUpto10Surveys()
        {
            var surveyRowsToReturn = new List<SurveyRow>();
            for (int i = 0; i < 10; i++)
            {
                surveyRowsToReturn.Add(new SurveyRow());
            }

            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetLatestAsync(10)).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetRecentSurveysAsync();

            Assert.AreEqual(10, actualSurveys.Count());
        }

        [TestMethod]
        public async Task GetRecentSurveysReturnsTenantIdFromThePartitionKey()
        {
            var surveyRow = new SurveyRow { PartitionKey = "tenant" };
            var surveyRowsToReturn = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetLatestAsync(10)).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetRecentSurveysAsync();

            Assert.AreEqual("tenant", actualSurveys.First().TenantId);
        }

        [TestMethod]
        public async Task GetRecentSurveysReturnsTitle()
        {
            var surveyRow = new SurveyRow { PartitionKey = "tenant", Title = "title" };
            var surveyRowsToReturn = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetLatestAsync(10)).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetRecentSurveysAsync();

            Assert.AreEqual("title", actualSurveys.First().Title);
        }

        [TestMethod]
        public async Task GetRecentSurveysReturnsSlugName()
        {
            var surveyRow = new SurveyRow { PartitionKey = "tenant", SlugName = "slug" };
            var surveyRowsToReturn = new[] { surveyRow };
            var mock = new Mock<IAzureTable<SurveyRow>>();
            mock.Setup(t => t.GetLatestAsync(10)).ReturnsAsync(surveyRowsToReturn);
            var store = new SurveyStore(mock.Object, default(IAzureTable<QuestionRow>), null);

            var actualSurveys = await store.GetRecentSurveysAsync();

            Assert.AreEqual("slug", actualSurveys.First().SlugName);
        }

        [TestMethod]
        public async Task GetSurveyExtensionByTenantAndSlugName()
        {
            var mock = new Mock<IUDFAzureTable>();
            var customEntity = new CustomEntity { Id = 1 };
            mock.Setup(t => t.GetExtensionByPartitionRowKeyAsync(typeof(CustomEntity), "tenant", "slugname")).ReturnsAsync(customEntity);
            var store = new SurveyStore(default(IAzureTable<SurveyRow>), default(IAzureTable<QuestionRow>), mock.Object);

            var retrievedExtension = await store.GetSurveyExtensionByTenantAndSlugNameAsync("tenant", "slugname", typeof(CustomEntity));

            Assert.AreEqual(customEntity.ToString(), retrievedExtension.ToString());
        }

        [TestMethod]
        public async Task GetSurveyExtensionsByTenant()
        {
            var mock = new Mock<IUDFAzureTable>();
            var customEntities = new[] { new CustomEntity { Id = 1 }, new CustomEntity { Id = 2 } };
            mock.Setup(t => t.GetExtensionsByPartitionKeyAsync(typeof(CustomEntity), "tenant")).ReturnsAsync(customEntities);
            var store = new SurveyStore(default(IAzureTable<SurveyRow>), default(IAzureTable<QuestionRow>), mock.Object);

            var retrievedExtensions = await store.GetSurveyExtensionsByTenantAsync("tenant", typeof(CustomEntity));

            Assert.AreEqual(2, retrievedExtensions.Count());
            Assert.AreEqual(customEntities[0].ToString(), retrievedExtensions.ToList()[0].ToString());
            Assert.AreEqual(customEntities[1].ToString(), retrievedExtensions.ToList()[1].ToString());
        }

        private class CustomEntity : TableEntity, IModelExtension
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return string.Format("Id: {0} - Name: {1}", this.Id, this.Name);
            }

            public bool IsChildOf(object parent)
            {
                throw new NotImplementedException();
            }
        }
    }
}