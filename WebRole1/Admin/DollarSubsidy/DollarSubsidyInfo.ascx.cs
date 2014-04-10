using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ClassLibrary1.Model;

public partial class Admin_DollarSubsidy_DollarSubsidyInfo : System.Web.UI.UserControl
{
    static int? SubtopicId = null;
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    public void setupDollarinfo(int? universeid)
    {
      SubtopicId = universeid;
        int UserId = (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");

        ActionProcessor Obj = new ActionProcessor();
        var CheckRights = Obj.DataContext.GetTable<UsersRight>().SingleOrDefault(x => x.UserID == UserId && x.PointsManagerID == SubtopicId);
        //Checking for superuser
        bool CheckSuperUser = Obj.DataContext.GetTable<User>().SingleOrDefault(x => x.UserID == UserId).SuperUser;
        bool MayAddTbl = false;
        if (CheckRights != null)
        {
            MayAddTbl = CheckRights.MayAddTbls;
        }
        // Checking user rights
        if (MayAddTbl == true || CheckSuperUser == true)
        {
            //Fetching the Dollar Subsidy infornmation
            GridDataBind();
        }
        else
        {
            PopUp.MsgString = StringConstants.StringNoRight;
            PopUp.Show();
        }
    }
    public void GridDataBind()
    {
        ActionProcessor Obj = new ActionProcessor();
         var GetInfo = Obj.DataContext.GetTable<PointsAdjustment>()
                               .Where(x => x.PointsManagerID == SubtopicId && x.CashValue > 0)
                               .Join(Obj.DataContext.GetTable<UserInfo>(), P => P.UserID, U => U.UserID, (P, U) => new { PointsManagerName = P.PointsManager.Name, UserName = U.FirstName + " " + U.LastName, Email = U.Email, Address = U.Address1 + " " + U.Address2, Amount = P.CashValue.ToString(), RecievedDate = P.WhenMade });


        if (GetInfo.Count() > 0)
        {
            //Binding to datagrid
            DollarInfoGrid.DataSource = GetInfo;

            DollarInfoGrid.DataBind();


        }
        else
        {
            DollarInfoGrid.Visible = false;
            PopUp.MsgString = StringConstants.StringNoItem;
            PopUp.Show();
        }
    }

    protected void DollarInfoGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DollarInfoGrid.PageIndex = e.NewPageIndex;
        GridDataBind();
    }
    public void SetupInitial()
    {

    }
    public void ShowUserControl()
    {
        DollarinfoContent.Visible =true;
        EmptyButton.Visible = true;
        DollarinfoExtender.Show();
     }

    public void HideUserControl()
    {
        DollarinfoContent.Visible = false;
        EmptyButton.Visible = false;
        DollarinfoExtender.Hide();
        
    }
  
}
