namespace Tailspin.Web.Survey.Public.Tests
{
    using Autofac;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SurveyAnswerService.Client;
    using SurveyManagementService.Client;
    using Tailspin.Web.Survey.Public;

    [TestClass]
    public class ContainerBootstrapperModuleFixture
    {
        [TestMethod]
        public void ResolveISurveyManagementService()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ContainerBootstrapperModule>();
            var container = containerBuilder.Build();

            var actualObject = container.Resolve<ISurveyManagementService>();

            Assert.IsNotNull(actualObject);
        }

        [TestMethod]
        public void ResolveISurveyAnswerService()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ContainerBootstrapperModule>();
            var container = containerBuilder.Build();

            var actualObject = container.Resolve<ISurveyAnswerService>();

            Assert.IsNotNull(actualObject);
        }
    }
}