﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;

////using PredRatings;

public partial class ChangePointsManager : System.Web.UI.Page
{
    PMActionProcessor Obj = new PMActionProcessor();
    public int? SubtopicId;

    // Change PointsManager Page
    protected void Page_Load(object sender, EventArgs e)
    {

        PMRoutingInfoMainContent Location = PMRouting.IncomingMainContent(Page.RouteData, Obj.DataContext);
        PointsManager thePointsManager = Location.lastItemInHierarchy.Tbl.PointsManager;
        int PointsManagerid = thePointsManager.PointsManagerID;

            Response.Buffer = true;
            Response.ExpiresAbsolute = TestableDateTime.Now.AddDays(-1d);
            Response.Expires = -1500;
            Response.CacheControl = "no-cache";
            if (!(HttpContext.Current.Profile != null && (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != 0) || !Obj.DataContext.GetTable<User>().Single(u => u.UserID == (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID")).SuperUser)
            {
                PMRouting.Redirect(Response, new PMRoutingInfo(PMRouteID.Login));
                return;
            }

            announce.Setup(null, PointsManagerid, null);
            DollarSubs.Setup(PointsManagerid);
            GrantUserRights.Setup(PointsManagerid);
            RewardRating1.Setup(PointsManagerid);
            Guarantees1.Setup(PointsManagerid);
        
        }
   


  
}