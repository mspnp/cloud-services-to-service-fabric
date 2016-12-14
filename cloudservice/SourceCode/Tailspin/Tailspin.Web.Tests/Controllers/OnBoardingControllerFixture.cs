namespace Tailspin.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.Web.Controllers;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using System.Threading.Tasks;

    [TestClass]
    public class OnBoardingControllerFixture
    {
        [TestMethod]
        public async Task IndexReturnsTitleInTheModel()
        {
            var tenantStoreMock = new Mock<ITenantStore>();
            tenantStoreMock.Setup(t => t.GetTenantIds()).Returns(new List<string>() { "t1", "t2", "t3" });
            tenantStoreMock.Setup(t => t.GetTenantAsync(It.IsAny<string>())).Returns<string>(name => Task.FromResult(new Tenant { TenantId = name }));

            using (var controller = new OnBoardingController(tenantStoreMock.Object))
            {
                var result = await controller.Index() as ViewResult;
                var model = result.ViewData.Model as TenantPageViewData<IEnumerable<Tenant>>;

                Assert.AreEqual("On boarding", model.Title);
                Assert.AreEqual(3, model.ContentModel.Count());

                Assert.IsTrue(model.ContentModel.Select(t => t.TenantId).Contains("t1"));
                Assert.IsTrue(model.ContentModel.Select(t => t.TenantId).Contains("t2"));
                Assert.IsTrue(model.ContentModel.Select(t => t.TenantId).Contains("t3"));
            }

            tenantStoreMock.Verify(t => t.GetTenantIds(), Times.Once());
            tenantStoreMock.Verify(t => t.GetTenantAsync(It.IsAny<string>()), Times.Exactly(3));
        }
    }
}
