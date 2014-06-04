using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace ClassLibrary1.Misc
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
                : "ERROR GETTING CONFIGURATION SETTING"; // "Data Source=PC2012;Initial Catalog=Norm0001;Integrated Security=True;Connect Timeout=300";
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
