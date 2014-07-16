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
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

////using PredRatings;

public partial class ChangePointsManager : System.Web.UI.Page
{
    ActionProcessor Obj = new ActionProcessor();
    public Guid? SubtopicId;

    // Change PointsManager Page
    protected void Page_Load(object sender, EventArgs e)
    {

        RoutingInfoMainContent Location = Routing.IncomingMainContent(Page.RouteData, Obj.DataContext);
        PointsManager thePointsManager = Location.lastItemInHierarchy.Tbl.PointsManager;
        Guid PointsManagerid = thePointsManager.PointsManagerID;

            Response.Buffer = true;
            Response.ExpiresAbsolute = TestableDateTime.Now.AddDays(-1d);
            Response.Expires = -1500;
            Response.CacheControl = "no-cache";
            if (!(HttpContext.Current.Profile != null && (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != new Guid()) || !Obj.DataContext.GetTable<User>().Single(u => u.UserID == (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID")).SuperUser)
            {
                Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
                return;
            }

            announce.Setup(null, PointsManagerid, null);
            DollarSubs.Setup(PointsManagerid);
            GrantUserRights.Setup(PointsManagerid);
            RewardRating1.Setup(PointsManagerid);
            Guarantees1.Setup(PointsManagerid);
        
        }
   


  
}
