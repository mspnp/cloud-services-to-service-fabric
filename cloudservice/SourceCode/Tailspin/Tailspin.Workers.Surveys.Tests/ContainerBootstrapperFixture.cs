namespace Tailspin.Workers.Surveys.Tests
{
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tailspin.Workers.Surveys;
    using Web.Survey.Shared.Stores;

    [TestClass]
    public class ContainerBootstrapperFixture
    {
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
    }
}
