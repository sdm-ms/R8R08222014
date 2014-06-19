using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Services;
using System.Xml.Linq;
using Subgurim.Controles;


using System.Web.Script.Services;
using GoogleGeocoder;
using MoreStrings;

using System.Diagnostics;
using System.Web.Profile;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

public partial class ViewTbl : System.Web.UI.Page
{
    internal R8RDataAccess DataAccess = new R8RDataAccess();
    FilterRules theFilterRules;
    Main_Table_WithTabSelector MainTableWithTabSelector = null;
    Main_Table_TblRowView MainTableTblRowView = null;
    Main_Table_TableCellView MainTableCellView = null;
    internal RoutingInfoMainContent theLocation;



    protected void Page_Init(object sender, EventArgs e)
    {
        CacheManagement.DisablePageCaching(); // No browser caching (we will use server caching)
        DataAccess.R8RDB.SetPageLoadOptions();
        try
        {
            theLocation = Routing.IncomingMainContent(Page.RouteData, DataAccess.R8RDB);
            theFilterRules = null; // When adding filtering in URL, should set theFilterRules here from theLocation.
        }
        catch
        {
            Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
            return;
        }
        ItemPath1.theTbl = theLocation.theTbl;
        if (theLocation.theTblRow != null)
            ItemPath1.theTblRow = theLocation.theTblRow;
        if (theLocation.theTblColumn != null)
            ItemPath1.theTblColumn = theLocation.theTblColumn;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (BackgroundThreadManager.RunBackgroundTaskFromWebRole)
            BackgroundThreadManager.Instance.EnsureBackgroundTaskIsRunning(true);
    }

    protected void SetChildrenProperties()
    { // cannot be called during init event, because during init, children load first
        int? entityID = null;
        if (theLocation.theTblRow != null)
            entityID = theLocation.theTblRow.TblRowID;
        bool includeFieldsBox = false; // We initially built this with the capability of showing narrow results on the left of the page, but decided tentatively it was too inefficient.
        if (theLocation.theTblRow == null)
        {
            if (!includeFieldsBox)
                FieldsBox.Visible = false; /* we'll still go through the logic, but it will load very fast after caching */
            //ProfileSimple.Start("LoadTableWithTabSelector");
            MainTableWithTabSelector = (Main_Table_WithTabSelector)LoadControl("~/Main/Table/WithTabSelector.ascx");
            MainContentPlaceHolder.Controls.Add(MainTableWithTabSelector);
            //ProfileSimple.End("LoadTableWithTabSelector");
            if (theFilterRules == null)
                theFilterRules = new FilterRules(theLocation.theTbl.TblID, true, false);
            //ProfileSimple.Start("SetupBeforeFieldsBox");
            MainTableWithTabSelector.SetupBeforeFieldsBox(GetFilteredAndSortedQuery, GetFilterRules, theLocation.theTbl.TblID, DataAccess, theLocation.theTbl.SuppStylesMain, theLocation.theTbl.SuppStylesHeader);
            //ProfileSimple.End("SetupBeforeFieldsBox");
            //ProfileSimple.Start("FieldsBoxSetup");
            if (includeFieldsBox)
                FieldsBox.Setup(theLocation.theTbl.TblID, MainTableWithTabSelector.GetTblTabID(), FieldsBoxMode.filterWithButton, BtnFilter_Click);
            //ProfileSimple.End("FieldsBoxSetup");
            //ProfileSimple.Start("SetupAfterFieldsBox");
            MainTableWithTabSelector.SetupAfterFieldsBox(includeFieldsBox ? FieldsBox : null);
            //ProfileSimple.End("SetupAfterFieldsBox");
        }
        else
        {
            FieldsBox.Visible = false;
            if (theLocation.theTblColumn == null)
            { // We're viewing a single entity.
                MainTableTblRowView = (Main_Table_TblRowView)LoadControl("~/Main/Table/TblRowView.ascx");
                MainContentPlaceHolder.Controls.Add(MainTableTblRowView);
                MainTableTblRowView.Setup(theLocation.theTblRow.TblRowID, theLocation.theTbl.SuppStylesMain, theLocation.theTbl.SuppStylesHeader);
            }
            else
            { // We're viewing a single rating.
                MainTableCellView = (Main_Table_TableCellView)LoadControl("~/Main/Table/TableCellView.ascx");
                MainContentPlaceHolder.Controls.Add(MainTableCellView);
                MainTableCellView.Setup(theLocation.theTblRow.TblRowID, (int) theLocation.theTblColumn.TblColumnID);
            }
        }
    }


    protected void Page_SaveStateComplete(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        System.Threading.Thread.CurrentThread.Name = TestableDateTime.Now.ToString();

        try
        {
            SetChildrenProperties();
            int? anyUserID = null;
            if (HttpContext.Current.Profile != null)
            {
                IUserProfileInfo currentUser = ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser();
                anyUserID = currentUser == null ? null : (int?)currentUser.GetProperty("UserID");
            }
            if (anyUserID == 0)
                anyUserID = null;
            bool canViewPage = DataAccess.CheckUserRights(anyUserID, UserActionType.View, false, null, theLocation.theTbl.TblID);
            if (!canViewPage)
            {
                Routing.Redirect(Response, new RoutingInfoLoginRedirect(Routing.Outgoing(theLocation)));
                return;
            }

            TopOfViewTblContent.Setup(theLocation.theDomain.DomainID, theLocation.thePointsManager.PointsManagerID, theLocation.theTbl.TblID, InsertableLocation.TopOfViewTblContent, DataAccess);

            BtnViewChanges.Visible = false;
            if (HttpContext.Current.Profile != null && (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != 0)
            {
                int UserId = (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
                //Session["UserId"] = UserId.ToString(); // to allow access in webmethod

                // checking for the user right to add a entity
                bool someUsersHaveRights = DataAccess.CheckUserRights(UserId, UserActionType.ChangeTblRows, false, theLocation.thePointsManager.PointsManagerID, theLocation.theTbl.TblID);
                bool IsValidForAddTblRow = someUsersHaveRights && DataAccess.UserIsTrustedToMakeDatabaseChanges(theLocation.thePointsManager.PointsManagerID, UserId) && theLocation.theTbl.Name != "Changes";

                if (IsValidForAddTblRow == false)
                {
                    BtnAddTblRow.Visible = false;
                }
                else if (theLocation.theTblRow != null)
                {
                    //if (theLocation.theTblColumn == null)
                    //{
                    //    BtnAddTblRow.Text = "Change Information";
                    //    AdministrativeOptions.Attributes.Add("class", "changeTblRowArea");
                    //}
                    //else
                        BtnAddTblRow.Visible = false;
                }
                else
                {
                    BtnViewChanges.Visible = true;
                    BtnAddTblRow.Text = "Add " + DataAccess.R8RDB.GetTable<Tbl>().Single(c => c.TblID == theLocation.theTbl.TblID).TypeOfTblRow;
                }
                bool IsValidForTblAdministration = DataAccess.TblAdministrationLinkVisible(UserId, theLocation.theTbl.TblID);
                if (IsValidForTblAdministration == true && theLocation.theTblRow == null)
                {
                    BtnAdministration.Visible = true;
                    BtnAdministration.PostBackUrl = Routing.Outgoing(new RoutingInfoMainContent(theLocation.theTbl, null, null, false, false, false, false, false, true, false));
                }
                else
                    BtnAdministration.Visible = false;
            }
            else
            {
                BtnAddTblRow.Visible = false;
                BtnAdministration.Visible = false;
            }

            if (BtnViewChanges.Visible || BtnAddTblRow.Visible || BtnAdministration.Visible)
                AdministrativeOptions.Attributes.Add("class", "addTblRowArea");

            if (theLocation.theTblRow == null)
            {
                string TblName = theLocation.theTbl.Name;
                Page.Title = "R8R: " + TblName;
            }
            else
            {
                Page.Title = "R8R: " + theLocation.theTblRow.Name;
            }

        }
        catch (Exception ex)
        {
            PopUp.MsgString = ex.Message;
            PopUp.Show();
        }

    }

    public IQueryable<TblRow> GetFilteredAndSortedQuery(int? maxNumResults, TableSortRule theTableSortRule, bool showDeletedItems)
    {
        LoadFilterRules(showDeletedItems);
        return theFilterRules.GetFilteredAndSortedQuery(DataAccess.R8RDB, maxNumResults, theTableSortRule, false, true);
       
    }

    public FilterRules GetFilterRules(bool reload, bool showDeletedItems)
    {
        if (reload)
            LoadFilterRules(showDeletedItems);
        else 
            theFilterRules.ActiveOnly = !showDeletedItems;
        return theFilterRules;
    }

    public void LoadFilterRules(bool showDeletedItems)
    {
        if (FieldsBox == null)
            theFilterRules = new FilterRules(theLocation.theTbl.TblID, !showDeletedItems, false);
        else
            theFilterRules = FieldsBox.GetFilterRules(!showDeletedItems, false);
    }

    protected void AddOrChangeTblRow_Click(object sender, EventArgs e)
    {
        if (theLocation.theTblRow != null)
            Routing.Redirect(Response, new RoutingInfoMainContent(theLocation.theTbl, theLocation.theTblRow, null, true, false));
        else
            Routing.Redirect(Response, new RoutingInfoMainContent(theLocation.theTbl, null, null, false,true));

    }


    protected void ViewChanges_Click(object sender, EventArgs e)
    {
        if (theLocation.theTblRow == null)
        {
            HierarchyItem changesHierarchyItem = theLocation.lastItemInHierarchy.HierarchyItems.FirstOrDefault(x => x.Tbl.Name == "Changes");
            if (changesHierarchyItem != null)
                Routing.Redirect(Response, new RoutingInfoMainContent(changesHierarchyItem, null, null));
        }
    }

    protected void BtnFilter_Click(object sender, EventArgs e)
    {
        try
        {
            if (MainTableWithTabSelector != null)
                MainTableWithTabSelector.UpdateMainTable(true, false, false, false, true);
        }
        catch (Exception ex)
        {
            PopUp.MsgString = ex.Message;
            PopUp.Show();
        }
    }

    // The following is necessary to get RenderControl to work properly for
    // controls not yet added to the web page.
    public override void VerifyRenderingInServerForm(Control control)
    {
        return;
    }


    /// <summary>
    /// Return information that can be used to authenticate the user in subsequent
    /// calls to web services that are not page methods. (We use web services rather
    /// than page methods for updating ratings because that allows for asynchronous
    /// completion of the requests in the order in which they are completed.)
    /// </summary>
    /// <returns></returns>
    [GenerateScriptType(typeof(RatingAndUserRatingString))]
    [GenerateScriptType(typeof(UserEditResponse))]
    [GenerateScriptType(typeof(UserAccessInfo))]
    [GenerateScriptType(typeof(TablePopulateResponse))]
    [System.Web.Services.WebMethod(EnableSession=true)]
    public static UserAccessInfo GetUserAccessInfo()
    {
        return R8RDataManipulation.GetUserAccessInfoForCurrentUser();
    }

}
