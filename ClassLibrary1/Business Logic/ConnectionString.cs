using ClassLibrary1.Nonmodel_Code;
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
        private static void CheckHardCodedAppropriate()
        {
            if (!GetIR8RDataContext.UseRealDatabase)
                throw new Exception("Internal error. We should use the hard coded connections only for testing against the real database.");
            if (RoleEnvironment.IsAvailable)
                throw new Exception("Internal error. We should not be using hard coded connections within the role environment. Load from config files instead.");
        }

        private static string HardCodedUserProfileDatabase()
        {
            CheckHardCodedAppropriate();

            const string hardCoded = @"Data Source=.;Initial Catalog=Norm0006;Integrated Security=True;Connect Timeout=300";
            return hardCoded;
        }


        public static string GetUserProfileDatabaseConnectionString()
        {
            return AzureSetup.GetConfigurationSetting("ApplicationServices") ?? HardCodedUserProfileDatabase();
        }

        private static string HardCodedNormalizedDatabase()
        {
            CheckHardCodedAppropriate();

            const string hardCoded = @"Data Source=.;Initial Catalog=Norm0006;Integrated Security=True;Connect Timeout=300";
            return hardCoded;
        }

        public static string GetR8RNormalizedDatabaseConnectionString()
        {
            return AzureSetup.GetConfigurationSetting("R8RConnectionString") ?? HardCodedNormalizedDatabase();
        }
    }
}
