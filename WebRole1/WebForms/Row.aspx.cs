using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;



public partial class Row : System.Web.UI.Page
{
    public RoutingInfoMainContent Location { get; set; }
    public FieldsBoxMode Mode { get; set; }
    public Action<object, EventArgs> TheAction;
    ActionProcessor theActionProcessor = new ActionProcessor();

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            FieldsBox.Mode = FieldsBoxMode.addTblRow; // may be changed later, but we need to know that we're in addTblRow or modifyFields early in life cycle.
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Location = Routing.IncomingMainContent(Page.RouteData, theActionProcessor.DataContext);
        if (Location.addMode)
            Mode = FieldsBoxMode.addTblRow;
        else if (Location.editMode)
            Mode = FieldsBoxMode.modifyFields;
        else
            throw new Exception("Internal error: Invalid mode for Row page.");
        
        R8RDataAccess DataAccess = new R8RDataAccess();
        int? UserId = null;
        if (HttpContext.Current.Profile != null)
            UserId = (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        if (UserId == 0 || UserId == null)
            Routing.Redirect(Response, new RoutingInfoLoginRedirect(Routing.OutgoingToCurrentRoute(Page.RouteData, DataAccess.R8RDB)));

        bool someUsersHaveRights = DataAccess.CheckUserRights(UserId, UserActionType.ChangeTblRows, false, Location.thePointsManager.PointsManagerID, Location.theTbl.TblID);
        bool IsValidForAddTblRow = someUsersHaveRights && DataAccess.UserIsTrustedAtLeastSomewhatToEnterRatings(Location.thePointsManager.PointsManagerID, (int) UserId) && Location.theTbl.Name != "Changes";
        if (!IsValidForAddTblRow)
            Routing.Redirect(Response,new RoutingInfo(RouteID.HomePage));

        int? anyUserID = null;
        if (HttpContext.Current.Profile != null)
            anyUserID = (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        if (anyUserID == 0)
            anyUserID = null;
        bool canChangeTblRows = theActionProcessor.CheckUserRights(anyUserID, UserActionType.ChangeTblRows, false, null, Location.theTbl.TblID);
        if (!canChangeTblRows)
        {
            Routing.Redirect(Response, new RoutingInfoLoginRedirect(Routing.Outgoing(Location)));
            return;
        }
        
        if (Mode == FieldsBoxMode.addTblRow)
            TheAction = AddTblRow;
        else
            TheAction = ModifyFields;

        if (Mode == FieldsBoxMode.modifyFields)
            FieldsBox.Setup(Location.theTblRow, Mode, TheAction);
        else
            FieldsBox.Setup(Location.theTbl.TblID, null, Mode, TheAction);

        RewardRatingSetting theSettings = theActionProcessor.DataContext.GetTable<RewardRatingSetting>().SingleOrDefault(rms => rms.PointsManagerID == Location.theTbl.PointsManagerID);
        if (theSettings != null)
        {
            if (Location.theTbl.TblRowAdditionCriteria != "")
                ChangeNotice.Text += Location.theTbl.TblRowAdditionCriteria + " ";
            if (theSettings.ProbOfRewardEvaluation > 0 && theSettings.ProbOfRewardEvaluation < 1)
                ChangeNotice.Text += "There is a " + (Math.Floor(theSettings.ProbOfRewardEvaluation * 100)).ToString() + "% chance that a change you make will be selected to be rated in Changes. That rating will determine how many points you receive or are penalized for the change.";
            else if (theSettings.ProbOfRewardEvaluation == 1)
                ChangeNotice.Text += "Any changes you make will be rated in Changes. That rating will determine how many points you receive or are penalized for the change.";
        }
        ItemPath1.theTbl = Location.theTbl;
        ItemPath1.theTblRow = Location.theTblRow ;
    }

    public void ReturnToRowPage()
    {
        Routing.Redirect(Response, new RoutingInfoMainContent( Location.theTbl, Location.theTblRow /* may be null */, null));
    }

    public void AddTblRow(object sender, EventArgs e)
    {
        FieldSetDataInfo theDataInfo = FieldsBox.GetFieldSetDataInfo();
        List<UserSelectedRatingInfo> theUserSelectedRatingInfos = FieldsBox.GetUserSelectedRatingInfos(); /* where user overrides rating type */
        theActionProcessor.TblRowCreateWithFields(theDataInfo, (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"), theUserSelectedRatingInfos);
        theActionProcessor.DataContext.SubmitChanges();
        ReturnToRowPage();
    }

    public void ModifyFields(object sender, EventArgs e)
    {
        FieldSetDataInfo theDataInfo = FieldsBox.GetFieldSetDataInfo();
        theActionProcessor.FieldSetImplement(theDataInfo, (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"), true, true);
        theActionProcessor.DataContext.SubmitChanges();
        ReturnToRowPage();
    }
}
