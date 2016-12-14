namespace Tailspin.Web.Tests.Controllers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Tailspin.Web.Controllers;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using System.Threading.Tasks;
    using System.Web.Routing;

    [TestClass]
    public class AccountControllerFixture
    {
        [TestMethod]
        public async Task IndexReturnsEmptyViewName()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new AccountController(mockTenantStore.Object))
            {
                var result = await controller.Index() as ViewResult;

                Assert.AreEqual(string.Empty, result.ViewName);
            }
        }

        [TestMethod]
        public async Task IndexReturnsTitleInTheModel()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>()))
                .ReturnsAsync(new Tenant());

            using (var controller = new AccountController(mockTenantStore.Object))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as TenantMasterPageViewData;
                Assert.AreEqual("My Account", model.Title);
            }
        }

        [TestMethod]
        public async Task IndexReturnsTheTenantInTheModel()
        {
            Tenant tenant = new Tenant();
            var mockTenantStore = new Mock<ITenantStore>();
            mockTenantStore.Setup(r => r.GetTenantAsync(It.IsAny<string>())).ReturnsAsync(tenant);

            using (var controller = new AccountController(mockTenantStore.Object))
            {
                var result = await controller.Index() as ViewResult;

                var model = result.ViewData.Model as TenantPageViewData<Tenant>;
                Assert.AreSame(tenant, model.ContentModel);
            }
        }

        [TestMethod]
        public async Task UploadLogoCallsTheStoreWithTheLogo()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            var mockLogoFile = new Mock<HttpPostedFileBase>();
            
            var logoBytes = new byte[1];
            using (var stream = new MemoryStream(logoBytes))
            {
                mockLogoFile.Setup(f => f.ContentLength).Returns(1);
                mockLogoFile.Setup(f => f.InputStream).Returns(stream);

                using (var controller = new AccountController(mockTenantStore.Object))
                {
                    await controller.UploadLogo("tenant", mockLogoFile.Object);
                }
            }

            mockTenantStore.Verify(r => r.UploadLogoAsync("tenant", logoBytes), Times.Once());
        }

        [TestMethod]
        public async Task UploadLogoDoesNotCallTheStoreWhenContentLengthIs0()
        {
            var mockTenantStore = new Mock<ITenantStore>();
            var mockLogoFile = new Mock<HttpPostedFileBase>();
            mockLogoFile.Setup(f => f.ContentLength).Returns(0);

            using (var controller = new AccountController(mockTenantStore.Object))
            {
                await controller.UploadLogo("tenant", mockLogoFile.Object);
            }

            mockTenantStore.Verify(r => r.UploadLogoAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never());
        }

        [TestMethod]
        public async Task UploadLogoRedirectsToIndex()
        {
            var mockTenantStore = new Mock<ITenantStore>();

            using (var controller = new AccountController(mockTenantStore.Object))
            {
                var result = await controller.UploadLogo("tenant", new Mock<HttpPostedFileBase>().Object) as RedirectToRouteResult;

                Assert.AreEqual("Index", result.RouteValues["action"]);
            }
        }
    }
}