namespace Tailspin.AnswerAnalysisService.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Autofac;
    using Tailspin.Web.Survey.Shared.Stores;

    [TestClass]
    public class ContainerBootstrapperModuleFixture
    {
        CloudStorageAccount account;

        public ContainerBootstrapperModuleFixture()
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
        }

        [TestMethod]
        public void ResolveISurveyAnswerStore()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ContainerBootstrapperModule>();
            containerBuilder.RegisterInstance(account);
            var container = containerBuilder.Build();

            var actualObject = container.Resolve<ISurveyAnswerStore>();

            Assert.IsInstanceOfType(actualObject, typeof(SurveyAnswerStore));
        }

        [TestMethod]
        public void ResolveISurveyAnswersSummaryStore()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ContainerBootstrapperModule>();
            containerBuilder.RegisterInstance(account);
            var container = containerBuilder.Build();

            var actualObject = container.Resolve<ISurveyAnswersSummaryStore>();

            Assert.IsInstanceOfType(actualObject, typeof(SurveyAnswersSummaryStore));
        }
    }
}
