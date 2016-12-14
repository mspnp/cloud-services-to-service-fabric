namespace Tailspin.Web.Tests.Controllers
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Tailspin.Web.Controllers;
    using Tailspin.Web.Models;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

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

                Assert.IsNull(result.ViewName);
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
            var mockLogoFile = new Mock<IFormFile>();
            
            var logoBytes = new byte[1];
            using (var stream = new MemoryStream(logoBytes))
            {
                mockLogoFile.Setup(f => f.Length).Returns(1);
                mockLogoFile.Setup(f => f.OpenReadStream()).Returns(stream);

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
            var mockLogoFile = new Mock<IFormFile>();
            mockLogoFile.Setup(f => f.Length).Returns(0);

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
                var result = await controller.UploadLogo("tenant", new Mock<IFormFile>().Object) as RedirectToActionResult;

                Assert.AreEqual("Index", result.ActionName);
            }
        }
    }
}