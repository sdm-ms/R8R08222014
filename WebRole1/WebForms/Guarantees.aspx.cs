using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;



namespace WebRole1.WebForms
{
    public partial class Guarantees : System.Web.UI.Page
    {
        PMRoutingInfoMainContent theLocation;
        RaterooDataManipulation DataAccess;
        User TheUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataAccess = new RaterooDataManipulation();
            theLocation = PMRouting.IncomingMainContent(Page.RouteData, DataAccess.DataContext);
            ItemPath1.theHierarchyItem = theLocation.lastItemInHierarchy;
            if (HttpContext.Current.Profile != null && ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null)
                TheUser = DataAccess.DataContext.GetTable<User>().SingleOrDefault(x => x.UserID == (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"));
            if (TheUser == null)
                PMRouting.Redirect(Response, new PMRoutingInfo(PMRouteID.HomePage));



            PaymentGuaranteeInfo1.ThePointsManager = theLocation.thePointsManager;
            PaymentGuaranteeInfo1.ThePointsTotal = TheUser.PointsTotals.SingleOrDefault(x => x.PointsManager == theLocation.thePointsManager);
            PaymentGuaranteeInfo1.TheUser = TheUser;
            PaymentGuaranteeInfo1.Location = CommonControl.PaymentGuaranteeInfo.PaymentGuaranteeInfoLocation.GuaranteePage;
            PaymentGuaranteeInfo1.TheDataAccess = DataAccess;
        }
    }
}