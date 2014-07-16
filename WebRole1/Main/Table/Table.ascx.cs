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
using System.Xml.Linq;
using System.Diagnostics;
using Subgurim.Controles;

using MoreStrings;

using System.Xml.Serialization;
using System.IO;
using System.Text;

using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class Main_Table_Table : System.Web.UI.UserControl
{
    protected Func<int?, TableSortRule, bool, IQueryable<TblRow>> GetFilteredAndSortedQueryFn;
    Func<bool, bool, FilterRules> GetFilterRulesFn;
    protected Guid TblID;
    protected Guid TblTabID;
    protected TableSortRule theTableSortRule;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    protected bool CanPredict;
    protected bool CanAdminister;
    protected bool CanEditFields;
    protected bool CommentsEnabled;
    protected TblDimension TheTblDimensions;
    protected R8RDataAccess DataAccess;
    bool resetToTop=true;
    bool resetSortToDefaultSettings = false;
    string SuppStyle, SuppStyleHeader;
  

    public void Setup(Func<int?, TableSortRule, bool, IQueryable<TblRow>> getFilteredAndSortedQueryFn, Func<bool, bool, FilterRules> getFilterRulesFn, Guid tblID, Guid tblTabID, string suppStyle, string suppStyleHeader)
    {
        DataAccess = new R8RDataAccess();
        GetFilteredAndSortedQueryFn = getFilteredAndSortedQueryFn;
        GetFilterRulesFn = getFilterRulesFn;
        TblID = tblID;
        Tbl theTbl = DataAccess.R8RDB.GetTable<Tbl>().Single(c => c.TblID == TblID);
        // We can't just set attributes on a table element itself, because then the whole table must be an htmltableTbl, which
        // you can't do if you have "td" inside a template. So we use a literal, through the LiteralElement user control.
        SuppStyle = suppStyle;
        SuppStyleHeader = suppStyleHeader;
        string classNames = "mainTable " + SuppStyle + " " + SuppStyleHeader;
        if (CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            classNames += " mainTablePositioning";
            PagerRowLiteral.AddAttribute("style", "display:none;");
        }
        maintLiteral.AddAttribute("class", classNames);
        TblTabID = tblTabID;
        SetDefaultSort();
        DetermineUserRights(DataAccess);
        if (!CheckJavaScriptHelper.IsJavascriptEnabled)
            MainTableHeaderRow.Setup(DataAccess, TblTabID, null, ResortTable, theTableSortRule);
        TblDimensionAccess theCssAccess = new TblDimensionAccess(DataAccess);
        TheTblDimensions = theCssAccess.GetTblDimensionsForRegularTbl(TblID);
    }

    protected void DetermineUserRights(R8RDataAccess dataAccess)
    {
        Guid SubtopicId = dataAccess.R8RDB.GetTable<Tbl>().Single(x => x.TblID == TblID).PointsManagerID;

        CanPredict = false;
        CanAdminister = false;
        CanEditFields = false;
        if ((Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != new Guid())
        {
            Guid? UserId = (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            //if (UserId == 0)
            //    UserId = null;
            // Checking user rights to predict
            CanPredict = dataAccess.CheckUserRights(UserId, UserActionType.Predict, false, SubtopicId, TblID);
            CanAdminister = dataAccess.CheckUserRights(UserId, UserActionType.ResolveRatings, false, SubtopicId, TblID);
            CanEditFields = dataAccess.CheckUserRights(UserId, UserActionType.ChangeTblRows, false, SubtopicId, TblID);
        }

        CommentsEnabled = true; // modify later if we turn off comments altogether for certain Tbls

    }


    public void SelectionChanged(int? newRow, int? newColumn)
    {
        if (newRow != (int?) ViewState["SelectedRow"])
        { // if we have a selection in the same row, then the ListView should change this automatically
            if ( ViewState["SelectedRow"] != null)
            { // The previous selected row needs to be deselected and rebound.
                ListViewDataItem dataItem = MainListView.Items[(int) ViewState["SelectedRow"] - 1];
                Guid theTblRowID = (Guid)MainListView.DataKeys[dataItem.DisplayIndex].Value;
                PlaceHolder MainTableBodyRowPlaceHolder = (PlaceHolder)dataItem.FindControl("MainTableBodyRowPlaceHolder");
                Main_Table_BodyRow MainTableBodyRow = (Main_Table_BodyRow)((System.Web.UI.Control)(MainTableBodyRowPlaceHolder)).Controls[0];
                MainTableBodyRow.DeselectAndReBind();
            }
        }
        ViewState["SelectedRow"] = newRow;
        ViewState["SelectedColumn"] = newColumn;
    }


    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        if (CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            e.Result = new List<TblRow>().AsQueryable();
            return;
        }

        const int maxResults = 100;
        IQueryable<TblRow> filteredAndSortedQuery = GetFilteredAndSortedQueryFn(maxResults, theTableSortRule, false);
        e.Result = filteredAndSortedQuery.Select(x => new { 
                            TblRowID = x.TblRowID, 
                            ExtraCSSClass = x.Status == (Byte) StatusOfObject.Unavailable ? "deletedTblRow" : "", 
                            TblTabId = TblTabID });           
    }

    public void MainListView_DataBinding(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        { // usually, we don't databound on postback
            rebinding = true;
        }
    }

    public void SetupChildren(ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            rowBeingCreated++;
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            Guid theTblRowID = (Guid)MainListView.DataKeys[dataItem.DisplayIndex].Value;
            ListViewDataItem CurrentItem = (ListViewDataItem)e.Item;
            Main_Table_ViewCellRowHeading MainTableViewCellRowHeading = (Main_Table_ViewCellRowHeading)e.Item.FindControl("MainTableViewCellRowHeading");
            MainTableViewCellRowHeading.Setup(DataAccess, TheTblDimensions, TblID, theTblRowID, CommentsEnabled, CanEditFields, rebinding);
            PlaceHolder MainTableBodyRowPlaceHolder = (PlaceHolder)e.Item.FindControl("MainTableBodyRowPlaceHolder");

            bool javascriptIsEnabled = CheckJavaScriptHelper.IsJavascriptEnabled;
            Control theControlToUseAsBodyRow;
            bool disableCaching = CanPredict && !javascriptIsEnabled && Page.IsPostBack; // We have a button we need to respond to, so no html caching.
            string myCacheKey = "MainTableBodyRow" + theTblRowID.ToString() + "," + TblTabID.ToString() + "," + CanPredict.ToString() + "," + javascriptIsEnabled.ToString();

            object cachedObject = CacheManagement.GetItemFromCache(myCacheKey);
            if (!disableCaching && cachedObject != null)
            {
                LiteralControl myLiteral = new LiteralControl();
                myLiteral.Text = (string)cachedObject;
                //Trace.TraceInformation("Using cached item for " + theTblRowID + " Cache: " + myLiteral.Text);
                theControlToUseAsBodyRow = myLiteral;
            }
            else
            {
                // Create control from scratch.
                Main_Table_BodyRow MainTableBodyRow = (Main_Table_BodyRow)LoadControl("~/Main/Table/BodyRow.ascx");
                MainTableBodyRow.Setup(DataAccess, TblID, TblTabID, null, (theTableSortRule is TableSortRuleTblColumn ? (Guid?)((TableSortRuleTblColumn)theTableSortRule).TblColumnToSortID : null), theTblRowID, rowBeingCreated, CanPredict, CanAdminister, rebinding, SelectionChanged, SuppStyle);
                if (!disableCaching)
                    MainTableBodyRow.DataBind();
                theControlToUseAsBodyRow = MainTableBodyRow;
                if (!disableCaching)
                {
                    string theContent = ((Control)MainTableBodyRow).MyRenderControl();
                    //Trace.TraceInformation("Creating new item for " + theTblRowID + " : " + theContent);
                    string[] myDependencies = {
                                    "RatingsForTblRowIDAndTblTabID" + theTblRowID.ToString() + "," + TblTabID.ToString(),
                                    "ColumnsForTblID" + TblID.ToString()
                                                      };
                    CacheManagement.AddItemToCache(myCacheKey, myDependencies, theContent);
                }
            }
            MainTableBodyRowPlaceHolder.Controls.Add(theControlToUseAsBodyRow);

        }
    }

    public void MainListView_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (CheckJavaScriptHelper.IsJavascriptEnabled || (Page.IsPostBack && !rebinding))
        { 
            // We only need to do a preliminary drawing of the page if we need to process an event in the ListView,
            // which will only occur if we have Javascript enabled.
            SetupChildren(e);
        }
    }

    public void MainListView_ItemDataBound(object sender, ListViewItemEventArgs e)
    {

        if (!Page.IsPostBack || rebinding)
        {
            SetupChildren(e);
        }
    }

    public void ReBind(Guid tblTabID, bool resetToTop, bool resetSortToDefault, bool reloadFields)
    {
        rebinding = true;
        if (resetToTop)
        {
            Pager.SetPageProperties(0, Pager.MaximumRows, false);
        }
        resetSortToDefaultSettings = resetSortToDefault;
        TblTabID = tblTabID;
        DataAccess = new R8RDataAccess(); // Reset data context references, since there may have been additions to the database.
    }

    public void SetDefaultSort()
    {
        if (!Page.IsPostBack || resetSortToDefaultSettings)
        {
            Guid? TblColumnToSort = null;
            bool SortOrderAscending = false;
            DataAccess.GetDefaultSortForTblTab(TblTabID, ref TblColumnToSort, ref SortOrderAscending);
            if (TblColumnToSort == null)
                theTableSortRule = new TableSortRuleRowName(SortOrderAscending);
            else
                theTableSortRule = new TableSortRuleTblColumn((Guid)TblColumnToSort, SortOrderAscending);
            ViewState["TableSortRule"] = theTableSortRule;
        }
    }


    public void ResortTable(Guid? TblColumnID, bool NewSortOrder)
    {
        if (TblColumnID == null)
            theTableSortRule = new TableSortRuleRowName(NewSortOrder);
        else
            theTableSortRule = new TableSortRuleTblColumn((Guid)TblColumnID, NewSortOrder);
        ViewState["TableSortRule"] = theTableSortRule;
        ReBind(TblTabID, resetToTop, false, false);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack && ViewState["TableSortRule"] != null)
        {
            theTableSortRule = (TableSortRule)ViewState["TableSortRule"];
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (rebinding)
        {
            //Trace.TraceInformation("Rebinding.");
            if (resetSortToDefaultSettings)
                SetDefaultSort();
            MainListView.DataBind();
            MainTableHeaderRow.ReBind(TblTabID, theTableSortRule);
        }
    }




}
