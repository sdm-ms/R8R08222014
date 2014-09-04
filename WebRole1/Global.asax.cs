using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.Web.UI;
using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using System.Web.Configuration;
using System.Configuration;
using System.Reflection;
using System.Web.Profile;
using System.Web.Mvc;
using System.Web.Optimization;

namespace WebRole1
{
    public class Global : System.Web.HttpApplication
    {
        public override void Init()
        {
            base.Init();
            this.AcquireRequestState += showRouteValues;
        }


        protected void showRouteValues(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context == null)
                return;
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context)); 
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            Routing.RegisterRoutes(routes);
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (arg.Contains("noPostback"))
            {
                if (context.Request.Form.Count == 0)
                    return arg;
            }
            return context.Request.Form.GetHashCode().ToString() + TestableDateTime.Now.ToLongTimeString(); // unique string to prevent caching
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            // Set the connection strings for user profile information from the azure setting
            //var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            //var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
            //section.ConnectionStrings["ApplicationServices"].ConnectionString = AzureSetup.GetConfigurationSetting("R8RConnectionString");
            //configuration.Save();

            /* NOTE: With jquery-1.4.2, we were getting an error on unloading pages (e.g., refreshing them) in IE6 */
            ScriptManager.ScriptResourceMapping.AddDefinition("jQuery", new ScriptResourceDefinition
            {

                Path = "~/js/jquery-1.4.1.min.js",

                DebugPath = "~/js/jquery-1.4.1.js",

                CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.4.1.min.js",

                CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.4.1.js"

            });
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

        // See http://social.msdn.microsoft.com/Forums/en/MSWinWebChart/thread/31975aad-807d-4056-9910-03ce04a143be
        static bool _chartInitialized;
        void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            RealUserProfileCollection.SetProviderConnectionString(ConnectionString.GetUserProfileDatabaseConnectionString());
            if (!_chartInitialized && this.Context.Handler is System.Web.UI.DataVisualization.Charting.ChartHttpHandler)
            {
                System.Web.UI.DataVisualization.Charting.Chart chart = new System.Web.UI.DataVisualization.Charting.Chart();
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (HtmlTextWriter w = new HtmlTextWriter(sw))
                    {
                        try
                        {
                            chart.RenderControl(w);
                        }
                        catch
                        {
                        }
                    }
                }
                _chartInitialized = true;
            }
        }

    }
}
