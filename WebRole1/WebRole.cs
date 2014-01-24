using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using ClassLibrary1.Misc;
using System.Configuration;
using System.Web.Configuration;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {

            new ClassLibrary1.Misc.AzureStartupDiagnostic();

            DiagnosticMonitor.Start("DiagnosticsConnectionString");

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            //AzureCloudDrive theDrive = new AzureCloudDrive("imagestore.vhd");
            //Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            //AppSettingsSection appSettings = (AppSettingsSection)configuration.GetSection("appSettings");
            //if (appSettings != null)
            //{
            //    appSettings.Settings["ChartImageHandler"].Value = "storage=file;privateImages=false;timeout=1200;deleteAfterServicing=false;webDevServerUseConfigSettings=false;url=" + theDrive.driveLetter + ";";
            //    configuration.Save();
            //} 

            return base.OnStart();
        }


        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}
