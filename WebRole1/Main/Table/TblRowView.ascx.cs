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
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;


public partial class Main_Table_TblRowView : System.Web.UI.UserControl
{

    protected Guid RowId;
    protected Guid TblID;
    protected Action<int?, bool> ResortCateDesFn;
    protected bool defaultSortOrderAsc;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    protected bool CanPredict;
    protected bool CanAdminister;
    protected bool CanEditFields;
    protected bool MultipleTblTabs;
    protected TblDimension TheTblDimensions;
    protected R8RDataAccess DataAccess;
    TblRow TheTblRow;
    ActionProcessor Obj = new ActionProcessor();
    string SuppStyle;
    string SuppStyleHeader;


    public void Setup(Guid entityId, string suppStyle, string suppStyleHeader)
    {
        DataAccess = new R8RDataAccess();

        RowId = entityId;
        TheTblRow = DataAccess.R8RDB.GetTable<TblRow>().Single(x => x.TblRowID == RowId);
        CommentsContent.theTblRowOrNullForEntireTable = TheTblRow;
        TblID = TheTblRow.TblID;
        SuppStyle = suppStyle;
        SuppStyleHeader = suppStyleHeader;

        DetermineUserRights();
        CommentsContent.UserCanProposeComments = ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser() != null;
        CommentsContent.UserCanAddComments = CanPredict;
        CommentsContent.UserCanDeleteComments = CanEditFields;
        SetupDeletionStatus();
       
        TblDimensionAccess theCssAccess = new TblDimensionAccess(DataAccess);
        TheTblDimensions = theCssAccess.GetTblDimensionsForRegularTbl(TblID);

        RecentRatingsTable.PointsManagerID = TheTblRow.Tbl.PointsManagerID;
        RecentRatingsTable.TblRowID = RowId;

        FieldDisplayHtml mainFieldDisplayHtml = new FieldDisplayHtml();
        Main_Table_FieldsDisplay theMainFieldsDisplay = (Main_Table_FieldsDisplay)LoadControl("~/Main/Table/FieldsDisplay.ascx");
        mainFieldDisplayHtml = theMainFieldsDisplay.Setup(DataAccess.R8RDB, TheTblDimensions, FieldsLocation.TblRowPage, RowId, true);
        FieldsDisplayPlaceHolder.Controls.Add(theMainFieldsDisplay);

        if (CanEditFields)
            BtnChangeTblRow.Attributes.Add("href",Routing.Outgoing(new RoutingInfoMainContent(TheTblRow.Tbl,TheTblRow,null,true,false)));
        else
            BtnChangeTblRow.Attributes.Add("style","display:none;");

        if (rebinding)
            TblRowListView.DataBind();
    }




    protected void DetermineUserRights()
    {
        Guid SubtopicId = DataAccess.R8RDB.GetTable<Tbl>().Single(x => x.TblID == TblID).PointsManagerID;

        CanPredict = false;
        CanAdminister = false;
        CanEditFields = false;
        if ((Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != null)
        {
            Guid UserId = (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            // Checking user rights to predict
            CanPredict = DataAccess.CheckUserRights(UserId, UserActionType.Predict, false, SubtopicId, TblID);
            CanAdminister = DataAccess.CheckUserRights(UserId, UserActionType.ResolveRatings, false, SubtopicId, TblID);
            CanEditFields = DataAccess.CheckUserRights(UserId, UserActionType.ChangeTblRows, false, SubtopicId, TblID);
        }
    }

    public void SetupDeletionStatus()
    {
        if (TheTblRow.Status == (Byte)StatusOfObject.Active || TheTblRow.Status == (Byte)StatusOfObject.DerivativelyUnavailable)
        {
            DeletionStatus.Text = "The table row represented on this page remains active.";
            DeletionStatus.Visible = true;
        }
        else if (TheTblRow.Status == (Byte)StatusOfObject.Unavailable)
            DeletionStatus.Text = "The table row represented on this page has been deleted.";
        else if (TheTblRow.Status == (Byte)StatusOfObject.Proposed)
            DeletionStatus.Text = "This table row has not yet been added to the table.";
        DeleteTblRow.Visible = false;
        UndeleteTblRow.Visible = false;
        if (CanEditFields)
        {
            if (TheTblRow.Status == (Byte) StatusOfObject.Active || TheTblRow.Status == (Byte) StatusOfObject.DerivativelyUnavailable)
                DeleteTblRow.Visible = true;
            else if  (TheTblRow.Status == (Byte) StatusOfObject.Unavailable)
                UndeleteTblRow.Visible = true;
        }
    }


    protected void PerformDeleteOrUndelete(bool delete)
    {
        if ((Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID") == null)
            throw new Exception("You must be logged in to make changes.");

        ActionProcessor theActionProcessor = new ActionProcessor();
        theActionProcessor.TblRowDeleteOrUndelete(RowId, delete, true, (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID"), null);
        theActionProcessor.DataContext.SubmitChanges();
        TblRow theTblRow = DataAccess.R8RDB.GetTable<TblRow>().Single(e => e.TblRowID == RowId);
        Routing.Redirect(Response, new RoutingInfoMainContent( theTblRow.Tbl, theTblRow, null));
        rebinding = true;
    }

    protected void DeleteTblRow_Click(object sender, EventArgs e)
    {
        if (!(TheTblRow.Status == (Byte)StatusOfObject.Active || TheTblRow.Status == (Byte)StatusOfObject.DerivativelyUnavailable))
            throw new Exception("The table row was already deleted.");
        PerformDeleteOrUndelete(true);
    }

    protected void UndeleteTblRow_Click(object sender, EventArgs e)
    {
        if (TheTblRow.Status == (Byte)StatusOfObject.Active)
            throw new Exception("The table row was already undeleted.");
        PerformDeleteOrUndelete(false);
    }

    public void TblRowLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    { 
        var theResult = Obj.DataContext.GetTable<TblTab>().Where(x => x.TblID == TblID && x.Status == Convert.ToByte(StatusOfObject.Active)).Select(x => new { TblTabID = x.TblTabID, TblTabName = x.Name });
        MultipleTblTabs = theResult.Count() > 1;
        e.Result = theResult;
    }

    public void TblRowListView_DataBinding(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        { // usually, we don't databind on postback
            rebinding = true;
        }
    }
    public void SetupChildren(ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            rowBeingCreated++;
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            Guid theTblTabID = (Guid)TblRowListView.DataKeys[dataItem.DisplayIndex].Values["TblTabID"];
            string theTblTabName = (string)TblRowListView.DataKeys[dataItem.DisplayIndex].Values["TblTabName"];
            ListViewDataItem CurrentItem = (ListViewDataItem)e.Item;

            CommonControl_LiteralElement theTableTag = (CommonControl_LiteralElement)CurrentItem.FindControl("TableInItemTemplate");
            theTableTag.AddAttribute("class", "mainTable mainTableWithBorders " + SuppStyle + " " + SuppStyleHeader);

            PlaceHolder MainTableBodyRowPlaceHolder = (PlaceHolder)CurrentItem.FindControl("BodyRowPlaceHolder");

            string myCacheKey = "MainTableBodyRow" + RowId.ToString() + "," + theTblTabID.ToString() + "," + CanPredict.ToString() + "," + CheckJavaScriptHelper.IsJavascriptEnabled.ToString();
            bool disableCaching = CanPredict && !CheckJavaScriptHelper.IsJavascriptEnabled && Page.IsPostBack; // We have a button we need to respond to, so no html caching.
            object cachedObject = CacheManagement.GetItemFromCache(myCacheKey);
            if (!disableCaching && cachedObject != null)
            {
                LiteralControl myLiteral = new LiteralControl();
                myLiteral.Text = (string)cachedObject;
                //Trace.TraceInformation("Using cached item for " + RowId + " Cache: " + myLiteral.Text);
                MainTableBodyRowPlaceHolder.Controls.Add(myLiteral);
            }
            else
            {
                // Create control from scratch.
                Main_Table_BodyRow MainTableBodyRow = (Main_Table_BodyRow)LoadControl("~/Main/Table/BodyRow.ascx");
                MainTableBodyRow.Setup(DataAccess, TblID, theTblTabID, null, null, RowId, rowBeingCreated, CanPredict, CanAdminister, rebinding, SelectionChanged, SuppStyle);
                if (!disableCaching)
                    MainTableBodyRow.DataBind();
                MainTableBodyRowPlaceHolder.Controls.Add(MainTableBodyRow);
                if (!disableCaching)
                {
                    string theContent = ((Control)MainTableBodyRow).MyRenderControl();
                    //Trace.TraceInformation("Creating new item for " + RowId + " : " + theContent);
                    string[] myDependencies = {
                                    "RatingsForTblRowIDAndTblTabID" + RowId.ToString() + "," + theTblTabID.ToString(),
                                    "ColumnsForTblID" + TblID.ToString()
                                                      };
                    CacheManagement.AddItemToCache(myCacheKey, myDependencies, theContent);
                }
            }

            Label TblTabNameLabel = (Label)CurrentItem.FindControl("LblTblTab");
            if (MultipleTblTabs)
                TblTabNameLabel.Text = theTblTabName;
            else
                TblTabNameLabel.Visible = false;

            PlaceHolder HeaderRowPlaceHolder = (PlaceHolder)CurrentItem.FindControl("HeaderRowPlaceHolder");
            Main_Table_HeaderRowOnTblRowPage MainTableHeaderRow = (Main_Table_HeaderRowOnTblRowPage)LoadControl("~/Main/Table/HeaderRowOnTblRowPage.ascx");
            MainTableHeaderRow.Setup(DataAccess, RowId, theTblTabID, null);
            HeaderRowPlaceHolder.Controls.Add(MainTableHeaderRow);
        }
    }

    public void SelectionChanged(int? newRow, int? newColumn)
    {
        if (newRow != (int?)ViewState["SelectedRow"])
        { // if we have a selection in the same row, then the ListView should change this automatically
            if (ViewState["SelectedRow"] != null)
            { // The previous selected row needs to be deselected and rebound.
                ListViewDataItem dataItem = TblRowListView.Items[(int)ViewState["SelectedRow"] - 1];
                Guid theTblRowID = (Guid)TblRowListView.DataKeys[dataItem.DisplayIndex].Value;
                PlaceHolder MainTableBodyRowPlaceHolder = (PlaceHolder)dataItem.FindControl("MainTableBodyRowPlaceHolder");
                Main_Table_BodyRow MainTableBodyRow = (Main_Table_BodyRow)((System.Web.UI.Control)(MainTableBodyRowPlaceHolder)).Controls[0];
                MainTableBodyRow.DeselectAndReBind();
            }
        }
        ViewState["SelectedRow"] = newRow;
        ViewState["SelectedColumn"] = newColumn;
    }

    public void TblRowListView_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding && !CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            SetupChildren(e);
        }
    }

    public void TblRowListView_ItemDataBound(object sender, ListViewItemEventArgs e)
    {

        if (!Page.IsPostBack || rebinding)
        {
            SetupChildren(e);
        }
    }

}
