namespace Tailspin.Web.Survey.Public.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Autofac;
    using Tailspin.Web.Survey.Public;
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
        public void ResolveISurveyStore()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ContainerBootstrapperModule>();
            containerBuilder.RegisterInstance(account);
            var container = containerBuilder.Build();

            var actualObject = container.Resolve<ISurveyStore>();

            Assert.IsInstanceOfType(actualObject, typeof(SurveyStore));
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
    }
}