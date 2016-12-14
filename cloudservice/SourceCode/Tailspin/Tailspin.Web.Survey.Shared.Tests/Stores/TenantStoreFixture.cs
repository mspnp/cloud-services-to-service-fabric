namespace Tailspin.Web.Survey.Shared.Tests.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Moq;
    using Shared.Models;
    using Shared.Stores;
    using Shared.Stores.AzureStorage;
    using System.Threading.Tasks;

    [TestClass]
    public class TenantStoreFixture
    {
        [TestMethod]
        public async Task GetTenantCallsBlobStorageToRetrieveTenant()
        {
            var mockTenantBlobContainer = new Mock<IAzureBlobContainer<Tenant>>();
            var store = new TenantStore(mockTenantBlobContainer.Object, null);

            await store.GetTenantAsync("tenant");

            mockTenantBlobContainer.Verify(c => c.GetAsync("tenant"), Times.Once());
        }

        [TestMethod]
        public async Task GetTenantReturnsTenantFromBlobStorage()
        {
            var mockTenantBlobContainer = new Mock<IAzureBlobContainer<Tenant>>();
            var store = new TenantStore(mockTenantBlobContainer.Object, null);
            var tenant = new Tenant();
            mockTenantBlobContainer.Setup(c => c.GetAsync("tenant")).ReturnsAsync(tenant);

            var actualTenant = await store.GetTenantAsync("tenant");

            Assert.AreSame(tenant, actualTenant);
        }

        [TestMethod]
        public void GetTenantIdsReturnsBlobNamesFromContainer()
        {
            var mockTenantBlobContainer = new Mock<IAzureBlobContainer<Tenant>>();
            var store = new TenantStore(mockTenantBlobContainer.Object, null);
            var blobs = new List<IListBlobItemWithName>() { new MockListBlobItem("b1"), new MockListBlobItem("b2") };
            mockTenantBlobContainer.Setup(c => c.GetBlobList()).Returns(blobs);

            var tenantIds = store.GetTenantIds().ToList();

            Assert.AreEqual(2, tenantIds.Count());
            CollectionAssert.Contains(tenantIds, "b1");
            CollectionAssert.Contains(tenantIds, "b2");
        }

        [TestMethod]
        public async Task InitializeEnsuresContainerExists()
        {
            var mockTenantBlobContainer = new Mock<IAzureBlobContainer<Tenant>>();
            var store = new TenantStore(mockTenantBlobContainer.Object, new Mock<IAzureBlobContainer<byte[]>>().Object);

            await store.InitializeAsync();

            mockTenantBlobContainer.Verify(c => c.EnsureExistsAsync(), Times.Once());
        }

        [TestMethod]
        public async Task UploadLogoSavesLogoToContainer()
        {
            var mockLogosBlobContainer = new Mock<IAzureBlobContainer<byte[]>>();
            var mockTenantContainer = new Mock<IAzureBlobContainer<Tenant>>();
            var store = new TenantStore(mockTenantContainer.Object, mockLogosBlobContainer.Object);
            mockTenantContainer.Setup(c => c.GetAsync("tenant")).ReturnsAsync(new Tenant() { TenantId = "tenant" });
            mockLogosBlobContainer.Setup(c => c.GetUri(It.IsAny<string>())).Returns(new Uri("http://bloburi"));
            var logo = new byte[1];

            await store.UploadLogoAsync("tenant", logo);

            mockLogosBlobContainer.Verify(c => c.SaveAsync("tenant", logo), Times.Once());
        }

        [TestMethod]
        public async Task UploadLogoGetsTenatToUpdateFromContainer()
        {
            var mockLogosBlobContainer = new Mock<IAzureBlobContainer<byte[]>>();
            var mockTenantContainer = new Mock<IAzureBlobContainer<Tenant>>();
            var store = new TenantStore(mockTenantContainer.Object, mockLogosBlobContainer.Object);
            mockTenantContainer.Setup(c => c.GetAsync("tenant")).ReturnsAsync(new Tenant() { TenantId = "tenant" }).Verifiable();
            mockLogosBlobContainer.Setup(c => c.GetUri(It.IsAny<string>())).Returns(new Uri("http://bloburi"));

            await store.UploadLogoAsync("tenant", new byte[1]);

            mockTenantContainer.Verify();
        }

        [TestMethod]
        public async Task UploadLogoSaveTenatWithLogoUrl()
        {
            var mockLogosBlobContainer = new Mock<IAzureBlobContainer<byte[]>>();
            var mockTenantContainer = new Mock<IAzureBlobContainer<Tenant>>();
            var store = new TenantStore(mockTenantContainer.Object, mockLogosBlobContainer.Object);
            mockTenantContainer.Setup(c => c.GetAsync("tenant")).ReturnsAsync(new Tenant() { TenantId = "tenant" });
            mockLogosBlobContainer.Setup(c => c.GetUri(It.IsAny<string>())).Returns(new Uri("http://bloburi/"));

            await store.UploadLogoAsync("tenant", new byte[1]);

            mockTenantContainer.Verify(c => c.SaveAsync("tenant", It.Is<Tenant>(t => t.Logo == "http://bloburi/")));
        }

        private class MockListBlobItem : IListBlobItemWithName
        {
            public MockListBlobItem(string name)
            {
                this.Name = name;
            }

            public string Name { get; set; }

            public CloudBlobContainer Container
            {
                get { throw new NotImplementedException(); }
            }

            public CloudBlobDirectory Parent
            {
                get { throw new NotImplementedException(); }
            }

            public Uri Uri
            {
                get { throw new NotImplementedException(); }
            }

            public StorageUri StorageUri
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}