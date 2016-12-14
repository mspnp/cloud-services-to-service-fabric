namespace Tailspin.Web.Survey.Shared.Helpers
{
    using System.Configuration;
    using System.Globalization;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;

    public static class CloudConfiguration
    {
        public static CloudStorageAccount GetStorageAccount(string settingName)
        {
            var connString = CloudConfigurationManager.GetSetting(settingName);
            return CloudStorageAccount.Parse(connString);
        }

        public static string GetConfigurationSetting(string settingName)
        {
            // Get the configuration string from the service configuration file.
            // If it fails, will look for the setting in the appSettings section of the web.config
            return CloudConfigurationManager.GetSetting(settingName);
        }

        public static string GetConfigurationSetting(string settingName, string defaultValue, bool throwIfNull)
        {
            var settingValue = CloudConfiguration.GetConfigurationSetting(settingName);

            if (string.IsNullOrWhiteSpace(settingValue))
            {
                settingValue = defaultValue;
            }

            if (string.IsNullOrEmpty(settingValue) && throwIfNull)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "Cannot find configuration value for {0}.", settingName));
            }

            return settingValue;
        }
    }
}
