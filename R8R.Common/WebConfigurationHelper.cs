
namespace R8R.Common
{

    public static class WebConfigurationHelper
    {
        public static string GetConnectionString(string connectionStringName)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;
        }
        public static string GetAppSettingValue(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}
