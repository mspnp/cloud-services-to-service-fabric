namespace Tailspin.Web.Survey.Shared.Tests.Helpers
{
    using System.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Tailspin.Web.Survey.Shared.Helpers;

    [TestClass]
    public class CloudConfigurationFixture
    {
        [TestMethod]
        public void GetStorageAccountReturnsDevelopmentAccount()
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");
            Assert.AreEqual(CloudStorageAccount.DevelopmentStorageAccount, account);            
        }

        [TestMethod]
        public void GetConfigurationSettingReturnsValue()
        {
            var value = CloudConfiguration.GetConfigurationSetting("ExistentSetting");
            Assert.AreEqual("ok", value);
        }

        [TestMethod]
        public void GetConfigurationSettingReturnsValueNoDefault()
        {
            var value = CloudConfiguration.GetConfigurationSetting("ExistentSetting", "default", false);
            Assert.AreEqual("ok", value);
        }

        [TestMethod]
        public void GetConfigurationSettingReturnsNull()
        {
            var value = CloudConfiguration.GetConfigurationSetting("NonExistentSetting");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void GetConfigurationSettingReturnsDefault()
        {
            var value = CloudConfiguration.GetConfigurationSetting("NonExistentSetting", "default", false);
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void GetConfigurationSettingThrows()
        {
            var value = CloudConfiguration.GetConfigurationSetting("NonExistentSetting", null, true);
            Assert.AreEqual("default", value);
        }
    }
}
