using System;
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
////using PredRatings;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using System.Collections.Generic;
using GoogleGeocoder;

using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class ChangeTbl : System.Web.UI.Page
{
    Guid TableId;
    
    ActionProcessor Obj = new ActionProcessor();
    protected void Page_Load(object sender, EventArgs e)
    {
        RoutingInfoMainContent Location = Routing.IncomingMainContent(Page.RouteData, Obj.DataContext);
        Tbl theTbl  = Location.lastItemInHierarchy.Tbl;
        TableId = theTbl.TblID;

        Response.Buffer = true;
        Response.ExpiresAbsolute = TestableDateTime.Now.AddDays(-1d);
        Response.Expires = -1500;
        Response.CacheControl = "no-cache";

        Guid? userId = (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        if (!(HttpContext.Current.Profile != null && userId != new Guid()) || !Obj.DataContext.GetTable<User>().Single(u => u.UserID == userId).SuperUser)
        {
            Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
            return;
        }

        changeTblannounce.Setup(TableId, null, null);

        AdministerPointsManager.HRef = Routing.Outgoing(new RoutingInfoMainContent(theTbl, null, null, false, false, false, false, false, false, true));
    }
      
   
    //protected void LinkBtnPauseOrResumeTbl_Click(object sender, EventArgs e)
    //{
    //    // the code manipulates the status of the Tbl
    //    try
    //    {
    //         TableId = int.Parse(Request.QueryString["TableId"]);
    //        int? ChangeGroupId = null;
    //    Guid userID = (Guid) ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
    //    StatusOfObject Status = (StatusOfObject)Obj.ObjDataAccess.GetTbl(TableId).Status;
    //    if (Status ==  StatusOfObject.Active)
    //    {
    //        Obj.ActivateOrInactiveTbl(TableId, StatusOfObject.Unavailable, UserId,ChangeGroupId);

    //        LinkBtnPauseOrResumeTbl.Text = "Make visible to this Tbl";
    //        PopUp.MsgString = "Tbl deactivated Successfully";
    //         PopUp.Show();
    //    }
    //    else
    //    {

    //        Obj.ActivateOrInactiveTbl(TableId, StatusOfObject.Active, UserId,ChangeGroupId);
    //        LinkBtnPauseOrResumeTbl.Text = "Make Invisible to this Tbl";
    //        PopUp.MsgString = "Tbl Activated Successfully";
    //        PopUp.Show();
    //    }
    //    }
    //    catch (Exception ex)
    //    {
    //        PopUp.MsgString = ex.Message;
    //        PopUp.Show();
    //    }
    //}
    
    
    
  

}
