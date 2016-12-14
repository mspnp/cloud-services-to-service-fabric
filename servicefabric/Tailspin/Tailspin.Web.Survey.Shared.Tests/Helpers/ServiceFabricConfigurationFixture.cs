namespace Tailspin.Web.Survey.Shared.Tests.Helpers
{
    using System.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.Web.Survey.Shared.Helpers;

    [TestClass]
    public class ServiceFabricConfigurationFixture
    {
        [TestMethod]
        public void GetStorageAccountReturnsDevelopmentAccount()
        {
            // TODO: Verify if this test is feasible given that service fabric activation context is not available at the time of runing this test
            Assert.IsTrue(true);
        }
    }
}
