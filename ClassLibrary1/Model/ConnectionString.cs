using ClassLibrary1.Misc;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Model
{
    public static class ConnectionString
    {
        private static string HardCodedUserProfileDatabase()
        {
            if (!GetIR8RDataContext.UseRealDatabase || RoleEnvironment.IsAvailable)
                throw new Exception("Internal error. We should use the hard coded connections only for the test project against the real database.");

            const string hardCoded = "Data Source=PC2012;Initial Catalog=Norm0001;Integrated Security=True;Connect Timeout=300";
            return hardCoded;
        }

        public static string GetUserProfileDatabaseConnectionString()
        {
            return AzureSetup.GetConfigurationSetting("ApplicationServices") ?? HardCodedUserProfileDatabase();
        }

        private static string HardCodedNormalizedDatabase()
        {
            if (!GetIR8RDataContext.UseRealDatabase || RoleEnvironment.IsAvailable)
                throw new Exception("Internal error. We should use the hard coded connections only for the test project against the real database.");

            const string hardCoded = "Data Source=PC2012;Initial Catalog=Norm0001;Integrated Security=True;Connect Timeout=300";
            return hardCoded;
        }

        public static string GetR8RNormalizedDatabaseConnectionString()
        {
            return AzureSetup.GetConfigurationSetting("R8RConnectionString") ?? HardCodedNormalizedDatabase();
        }
    }
}
