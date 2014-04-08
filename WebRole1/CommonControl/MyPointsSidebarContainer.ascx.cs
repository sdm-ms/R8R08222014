using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;


namespace WebRole1.CommonControl
{
    public partial class MyPointsSidebarContainer : System.Web.UI.UserControl
    {
        internal RoutingInfoMainContent theLocation;
        internal RaterooDataManipulation theDataAccess;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
                return;

            MyPointsSidebar sidebar = (MyPointsSidebar)Page.LoadControl("~/CommonControl/MyPointsSidebar.ascx");

            theDataAccess = new RaterooDataManipulation();

            PointsManager thePointsManager = null;
            IUserProfileInfo theUser = ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser();
            try
            {
                theLocation = Routing.IncomingMainContent(Page.RouteData, theDataAccess.DataContext);
                thePointsManager = theLocation.thePointsManager;
            }
            catch
            {
                Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
                return;
            }

            PointsTotal thePointsTotal  = theDataAccess.DataContext.GetTable<PointsTotal>().SingleOrDefault(x => x.User.Username == theUser.Username && x.PointsManager == thePointsManager);
            if (thePointsTotal == null)
                thePointsTotal = theDataAccess.AddPointsTotal(theDataAccess.DataContext.GetTable<User>().Single(x => x.Username == theUser.Username), thePointsManager);

            sidebar.TheDataAccess = theDataAccess;
            sidebar.ThePointsManager = thePointsManager;
            sidebar.ThePointsTotal = thePointsTotal;
            ThePlaceHolder.Controls.Add(sidebar);
        }
    }
}