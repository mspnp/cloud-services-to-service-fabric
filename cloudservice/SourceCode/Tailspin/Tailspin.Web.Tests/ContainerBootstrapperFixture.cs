namespace Tailspin.Web.Tests
{
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Web.Survey.Shared.Stores;

    [TestClass]
    public class ContainerBootstrapperFixture
    {
        [TestMethod]
        public void ResolveISurveyStore()
        {
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container);
                var actualObject = container.Resolve<ISurveyStore>();

                Assert.IsInstanceOfType(actualObject, typeof(SurveyStore));
            }
        }

        [TestMethod]
        public void ResolveISurveyAnswerStore()
        {
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container);
                var actualObject = container.Resolve<ISurveyAnswerStore>();

                Assert.IsInstanceOfType(actualObject, typeof(SurveyAnswerStore));
            }
        }

        [TestMethod]
        public void ResolveISurveyAnswersSummaryStore()
        {
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container);
                var actualObject = container.Resolve<ISurveyAnswersSummaryStore>();

                Assert.IsInstanceOfType(actualObject, typeof(SurveyAnswersSummaryStore));
            }
        }

        [TestMethod]
        public void ResolveITenantStore()
        {
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container);
                var actualObject = container.Resolve<ITenantStore>();

                Assert.IsInstanceOfType(actualObject, typeof(TenantStore));
            }
        }
    }
}
