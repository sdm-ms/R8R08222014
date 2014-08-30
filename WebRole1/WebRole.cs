using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using ClassLibrary1.Nonmodel_Code;
using System.Configuration;
using System.Web.Configuration;
using ClassLibrary1.EFModel;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            //System.Data.Entity.Database.SetInitializer<R8RContext>(new System.Data.Entity.CreateDatabaseIfNotExists<R8RContext>());

            new ClassLibrary1.Nonmodel_Code.AzureStartupDiagnostic();

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString");

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
