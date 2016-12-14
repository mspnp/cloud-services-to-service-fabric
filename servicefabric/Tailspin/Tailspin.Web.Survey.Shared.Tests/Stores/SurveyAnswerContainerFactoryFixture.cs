namespace Tailspin.Web.Survey.Shared.Tests.Stores
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Autofac;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    [TestClass]
    public class SurveyAnswerContainerFactoryFixture
    {
        [TestMethod]
        public void NewInstanceIsTypeOfSurveyAnswerContainerFactory()
        {
            var instance = new SurveyAnswerContainerFactory(null);
            Assert.IsInstanceOfType(instance, typeof(SurveyAnswerContainerFactory));
        }

        [TestMethod]
        public void CreateReturnsAnswersContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(CloudStorageAccount.DevelopmentStorageAccount);
            containerBuilder.RegisterType<EntitiesBlobContainer<SurveyAnswer>>().As<IAzureBlobContainer<SurveyAnswer>>();
            var container = containerBuilder.Build();

            var factory = new SurveyAnswerContainerFactory(container);
                
            var blobContainer = factory.Create("tenant", "surveySlug");
            Assert.IsInstanceOfType(blobContainer, typeof(EntitiesBlobContainer<SurveyAnswer>));
        }
    }
}
