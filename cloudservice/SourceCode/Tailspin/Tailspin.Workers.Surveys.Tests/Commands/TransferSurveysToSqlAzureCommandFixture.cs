namespace Tailspin.Workers.Surveys.Tests.Commands
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Surveys.Commands;
    using Web.Survey.Shared.Models;
    using Web.Survey.Shared.QueueMessages;
    using Web.Survey.Shared.Stores;
    using System.Threading.Tasks;

    [TestClass]
    public class TransferSurveysToSqlAzureCommandFixture
    {
        [TestMethod]
        public void RunGetsSurveyFromStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveySqlStore = new Mock<ISurveySqlStore>();
            var command = new TransferSurveysToSqlAzureCommand(mockSurveyAnswerStore.Object, mockSurveyStore.Object, mockTenantStore.Object, mockSurveySqlStore.Object);
            var message = new SurveyTransferMessage { Tenant = "tenant", SlugName = "slugName" };
            var tenant = new Tenant { SqlAzureConnectionString = "connectionString" };
            mockTenantStore.Setup(r => r.GetTenantAsync("tenant")).ReturnsAsync(tenant);
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync("tenant", "slugName", true)).ReturnsAsync(new Survey());

            command.Run(message);

            mockSurveyStore.Verify(r => r.GetSurveyByTenantAndSlugNameAsync(message.Tenant, message.SlugName, true));
        }

        [TestMethod]
        public void RunGetsSurveyAnswerIdsFromStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveySqlStore = new Mock<ISurveySqlStore>();
            var command = new TransferSurveysToSqlAzureCommand(mockSurveyAnswerStore.Object, mockSurveyStore.Object, mockTenantStore.Object, mockSurveySqlStore.Object);
            var message = new SurveyTransferMessage { Tenant = "tenant", SlugName = "slugName" };
            var survey = new Survey("slugName")
                             {
                                 TenantId = "tenant",
                             };
            survey.Questions.Add(new Question
                                     {
                                         Text = "What is your favorite food?",
                                         PossibleAnswers = "Coffee\nPizza\nSalad",
                                         Type = QuestionType.MultipleChoice
                                     });
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync("tenant", "slugName", true)).ReturnsAsync(survey);
            var tenant = new Tenant { SqlAzureConnectionString = "connectionString" };
            mockTenantStore.Setup(r => r.GetTenantAsync("tenant")).ReturnsAsync(tenant);

            command.Run(message);

            mockSurveyAnswerStore.Verify(r => r.GetSurveyAnswerIdsAsync(message.Tenant, "slugName"));
        }

        [TestMethod]
        public void RunGetsSurveyAnswersForIdFromStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveySqlStore = new Mock<ISurveySqlStore>();
            var command = new TransferSurveysToSqlAzureCommand(mockSurveyAnswerStore.Object, mockSurveyStore.Object, mockTenantStore.Object, mockSurveySqlStore.Object);
            var message = new SurveyTransferMessage { Tenant = "tenant", SlugName = "slugName" };
            var survey = new Survey("slugName")
                             {
                                 TenantId = "tenant",
                             };
            survey.Questions.Add(new Question
                                     {
                                         Text = "What is your favorite food?",
                                         PossibleAnswers = "Coffee\nPizza\nSalad",
                                         Type = QuestionType.MultipleChoice
                                     });
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync("tenant", "slugName", true)).ReturnsAsync(survey);
            mockSurveyStore.Setup(r => r.GetSurveysByTenantAsync("tenant")).ReturnsAsync(new List<Survey> { survey });
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerIdsAsync("tenant", "slugName")).ReturnsAsync(new List<string> { "id" });
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerAsync("tenant", "slugName", "id")).ReturnsAsync(new SurveyAnswer());
            var tenant = new Tenant { SqlAzureConnectionString = "connectionString" };
            mockTenantStore.Setup(r => r.GetTenantAsync("tenant")).ReturnsAsync(tenant);

            command.Run(message);

            mockSurveyAnswerStore.Verify(r => r.GetSurveyAnswerAsync(message.Tenant, "slugName", "id"));
        }

        [TestMethod]
        public void RunGetsTenantFromStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveySqlStore = new Mock<ISurveySqlStore>();
            var command = new TransferSurveysToSqlAzureCommand(mockSurveyAnswerStore.Object, mockSurveyStore.Object, mockTenantStore.Object, mockSurveySqlStore.Object);
            var message = new SurveyTransferMessage { Tenant = "tenant", SlugName = "slugName" };
            var tenant = new Tenant { SqlAzureConnectionString = "connectionString" };
            mockTenantStore.Setup(r => r.GetTenantAsync("tenant")).ReturnsAsync(tenant);
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync("tenant", "slugName", true)).ReturnsAsync(new Survey());

            command.Run(message);

            mockTenantStore.Verify(r => r.GetTenantAsync(message.Tenant));
        }

        [TestMethod]
        public void RunResetsSqlStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveySqlStore = new Mock<ISurveySqlStore>();
            var command = new TransferSurveysToSqlAzureCommand(mockSurveyAnswerStore.Object, mockSurveyStore.Object, mockTenantStore.Object, mockSurveySqlStore.Object);
            var message = new SurveyTransferMessage { Tenant = "tenant", SlugName = "slugName" };
            var survey = new Survey("slugName")
            {
                TenantId = "tenant",
            };
            survey.Questions.Add(new Question
            {
                Text = "What is your favorite food?",
                PossibleAnswers = "Coffee\nPizza\nSalad",
                Type = QuestionType.MultipleChoice
            });
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync("tenant", "slugName", true)).ReturnsAsync(survey);
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerIdsAsync("tenant", "slugName")).ReturnsAsync(new List<string> { "id" });
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerAsync("tenant", "slugName", "id")).ReturnsAsync(new SurveyAnswer());
            var tenant = new Tenant { SqlAzureConnectionString = "connectionString" };
            mockTenantStore.Setup(r => r.GetTenantAsync("tenant")).ReturnsAsync(tenant);

            command.Run(message);

            mockSurveySqlStore.Verify(r => r.Reset("connectionString", "tenant", "slugName"));
        }

        [TestMethod]
        public void RunSavesToSqlStore()
        {
            var mockSurveyStore = new Mock<ISurveyStore>();
            var mockSurveyAnswerStore = new Mock<ISurveyAnswerStore>();
            var mockTenantStore = new Mock<ITenantStore>();
            var mockSurveySqlStore = new Mock<ISurveySqlStore>();
            var command = new TransferSurveysToSqlAzureCommand(mockSurveyAnswerStore.Object, mockSurveyStore.Object, mockTenantStore.Object, mockSurveySqlStore.Object);
            var message = new SurveyTransferMessage { Tenant = "tenant", SlugName = "slugName" };
            var survey = new Survey("slugName")
                             {
                                 TenantId = "tenant",
                             };
            survey.Questions.Add(new Question
                                     {
                                         Text = "What is your favorite food?",
                                         PossibleAnswers = "Coffee\nPizza\nSalad",
                                         Type = QuestionType.MultipleChoice
                                     });
            mockSurveyStore.Setup(r => r.GetSurveyByTenantAndSlugNameAsync("tenant", "slugName", true)).ReturnsAsync(survey);
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerIdsAsync("tenant", "slugName")).ReturnsAsync(new List<string> { "id" });
            mockSurveyAnswerStore.Setup(r => r.GetSurveyAnswerAsync("tenant", "slugName", "id")).ReturnsAsync(new SurveyAnswer());
            var tenant = new Tenant { SqlAzureConnectionString = "connectionString" };
            mockTenantStore.Setup(r => r.GetTenantAsync("tenant")).ReturnsAsync(tenant);

            command.Run(message);

            mockSurveySqlStore.Verify(r => r.SaveSurvey("connectionString", It.IsAny<SurveyData>()));
        }
    }
}