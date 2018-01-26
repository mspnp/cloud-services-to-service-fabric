namespace Tailspin.Web.Tests
{
    using Autofac;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;

    [TestClass]
    public class ContainerBootstrapperModuleFixture
    {
        [TestMethod]
        public void ResolveISurveyStore()
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
