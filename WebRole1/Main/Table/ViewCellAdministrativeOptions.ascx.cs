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

public partial class Main_Table_ViewCellAdministrativeOptions : System.Web.UI.UserControl
{
    //Rating reolution user control
    int RatingID;
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    //Property RatingId
    public Guid RatingId
    {
        get
        {
            return RatingID;
        }
        set
        {
            RatingID = value;
        }
    }
   
   

    //This event redirect the user to rating information page
    protected void LinkBtnViewSettings_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Rating/RatingGroupInfo.aspx?RatingGroupId=" + this.RatingId);
    }
    //This event redirect the user to resolve rating page
    protected void LinkBtnResolve_Click(object sender, EventArgs e)
    {
        int RatingGroupId = this.RatingId;
        Response.Redirect("~/Rating/ResolveRatings.aspx?RatingGroupId=" + RatingGroupId);
    }
    protected void LinkBtnViewUserRating_Click(object sender, EventArgs e)
    {
        int RatingGroupId = this.RatingId;
        Response.Redirect("~/Main/View/ViewRatingUserRatings.aspx?RatingGroupId=" + RatingGroupId);
    }
}
