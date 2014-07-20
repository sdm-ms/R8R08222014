using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace ClassLibrary1.Nonmodel_Code
{
    public static class AzureSetup
    {
        public static void SetConfigurationSettingPublisher()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                var connectionString = GetConfigurationSettingFromPublisher(configName);
                configSetter(connectionString);
                //configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
            });
        }

        public static string GetConfigurationSetting(string configName)
        {
            SetConfigurationSettingPublisher();
            return RoleEnvironment.IsAvailable ? RoleEnvironment.GetConfigurationSettingValue(configName)
                : null;
                     // : ConfigurationManager.AppSettings[configName];
        }

        internal static string GetConfigurationSettingFromPublisher(string configName)
        {
            return RoleEnvironment.IsAvailable
                      ? RoleEnvironment.GetConfigurationSettingValue(configName)
                      : ConfigurationManager.AppSettings[configName];
        }

    }
}
