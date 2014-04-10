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
using System.Diagnostics;
using MoreStrings;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ClassLibrary1.Model;

public partial class Main_Table_BodyRow : System.Web.UI.UserControl
{
    public LoadBodyRowInfo theBodyRowInfo { get; set; }// use this when creating the BodyRow from a web service
    bool isInSortedColumn;
    protected bool rebinding = false;
    protected int? LimitToThisTblColumn; // if not null, we'll show only the one cell
    protected int? TblColumnToSort;
    protected RaterooDataAccess DataAccess; // don't make this a property -- we don't want to persist viewstate on this or we'll get stale values
    protected int TblID { get; set; }
    protected int TblTabID { get; set; }
    protected int TblRowID { get; set; }
    protected int RowNumber { get; set; }
    protected bool CanPredict { get; set; }
    protected bool CanAdminister { get; set; }
    protected Action<int?, int?> ParentSelectionChangedHandler { get; set; }
    protected string SuppStyle { get; set; }

    public void ActivateRebinding()
    {
        rebinding = true;
    }

    public void Page_Load(object sender, EventArgs e)
    {
        if (theBodyRowInfo != null)
        {
            Setup(theBodyRowInfo.dataAccess, theBodyRowInfo.theTblID, theBodyRowInfo.TblTabID, null, null, theBodyRowInfo.theTblRowID, 0, true, false, false, null, theBodyRowInfo.suppStyle);
            DataBind();
        }
    }

    public void Setup(RaterooDataAccess theDataAccess, int tblID, int tblTabID, int? limitToThisTblColumn, int? tblColumnToSort, int entityID, int rowNumber, bool canPredict, bool canAdminister, bool doRebind, Action<int?, int?> theParentSelectionChangedHandler, string suppStyle)
    {
        if (doRebind)
            ActivateRebinding();
        DataAccess = theDataAccess;
        TblID = tblID;
        TblTabID = tblTabID;
        LimitToThisTblColumn = limitToThisTblColumn;
        TblColumnToSort = tblColumnToSort;
        TblRowID = entityID;
        if (TblRowID == 0)
            throw new Exception("Internal error: row id must be specified.");
        RowNumber = rowNumber;
        CanPredict = canPredict;
        CanAdminister = canAdminister;
        ParentSelectionChangedHandler = theParentSelectionChangedHandler;
        SuppStyle = suppStyle;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void BodyRowLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        e.Result = DataAccess.RaterooDB.GetTable<TblColumn>().Where(x => x.TblTabID == TblTabID 
            && (LimitToThisTblColumn == null || x.TblColumnID == LimitToThisTblColumn)
            && x.Status == (byte)StatusOfObject.Active).Select(x => new { TblColumnID = x.TblColumnID });
    }

    protected void SetupChildItem(ListViewItemEventArgs e)
    {
        // Trace.TraceInformation("Creating item -- selectedindex = " + BodyRowListView.SelectedIndex.ToString());
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            int theTblColumnID = (int)BodyRowListView.DataKeys[dataItem.DisplayIndex].Value;

            int? theRatingGroupID = DataAccess.GetRatingGroupForTblRowCategory(TblRowID, theTblColumnID);

            if (theRatingGroupID != null)
            {
                bool multipleOutcomes = true;
                RatingGroupTypes theType = (RatingGroupTypes) DataAccess.RaterooDB.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == theRatingGroupID).RatingGroupAttribute.TypeOfRatingGroup;
                if (theType == RatingGroupTypes.probabilitySingleOutcome || theType == RatingGroupTypes.singleDate || theType == RatingGroupTypes.singleNumber)
                    multipleOutcomes = false;
                //TradingStatus theTradingStatus = (TradingStatus) DataAccess.RaterooDB.GetTable<RatingGroup>().Single(m => m.RatingGroupID == theRatingGroupID).TradingStatus;
                int columnNumber = dataItem.DisplayIndex;

                PlaceHolder thePlaceHolder = (PlaceHolder)e.Item.FindControl("CellMainPlaceholder");
                if (thePlaceHolder != null)
                { 
                    // set up unselected cell -- note: we're not currently taking advantage of isInSortedColumn feature,
                    // so we won't take that into account in caching.
                    if (theTblColumnID == TblColumnToSort)
                    {
                       isInSortedColumn = true;
                       
                    }
                    else
                    {
                        isInSortedColumn = false;
                    }

                    string myCacheKey = "ViewCellMainUnselected" + theRatingGroupID.ToString() + "," + CanPredict.ToString() + "," + CheckJavaScriptHelper.IsJavascriptEnabled.ToString();
                    bool disableCaching = CanPredict && !CheckJavaScriptHelper.IsJavascriptEnabled; // We have a button we need to respond to, so no html caching.
                    object cachedObject = CacheManagement.GetItemFromCache(myCacheKey);
                    if (!disableCaching && cachedObject != null)
                    {
                        LiteralControl myLiteral = new LiteralControl();
                        myLiteral.Text = (string)cachedObject;
                        //Trace.TraceInformation("Using cached item for " + theRatingGroupID + " Cache: " + myLiteral.Text);
                        thePlaceHolder.Controls.Add(myLiteral);
                    }
                    else
                    {
                        // Create control from scratch.
                        Control theControl = LoadControl("~/Main/Table/ViewCellMainUnselected.ascx");
                        Main_Table_ViewCellMainUnselected theMainCell = (Main_Table_ViewCellMainUnselected) theControl;
                        theMainCell.Setup(DataAccess, SelectItemAndReBind, columnNumber, theTblColumnID, (int)theRatingGroupID, isInSortedColumn, multipleOutcomes, TradingStatus.Active, CanPredict, rebinding, SuppStyle);
                        if (!disableCaching)
                            theMainCell.DataBind();
                        thePlaceHolder.Controls.Add(theMainCell);
                        if (!disableCaching)
                        {
                            string theContent = theMainCell.MyRenderControl();
                            //Trace.TraceInformation("Creating new item for " + theRatingGroupID + " : " + theContent);
                            string[] myDependencies = {
                                    "RatingGroupID" + theRatingGroupID.ToString()
                                                      };
                            CacheManagement.AddItemToCache(myCacheKey, myDependencies, theContent);
                        }
                    }


                }
                else
                { 
                    // set up selected cell from selecteditemtemplate
                    thePlaceHolder = (PlaceHolder)e.Item.FindControl("CellMainSelectedPlaceholder");
                    Main_Table_ViewCellMainSelected theMainCell = (Main_Table_ViewCellMainSelected)LoadControl("~/Main/Table/ViewCellMainSelected.ascx");
                    theMainCell.Setup(DataAccess, DeselectAndReBind, SelectItemAndReBind, columnNumber, theTblColumnID, (int)theRatingGroupID, isInSortedColumn, multipleOutcomes, TradingStatus.Active, CanPredict, CanAdminister, rebinding, SuppStyle);
                    thePlaceHolder.Controls.Add(theMainCell);
                }
            }
        }
    }

    protected void ReBind()
    {
        rebinding = true;
        DataAccess = new RaterooDataAccess(); // reset data contexts so we get fresh data
        BodyRowListView.DataBind();
    }

    protected void Deselect()
    {
        BodyRowListView.SelectedIndex = -1;
    }

    protected void SelectItem(int newIndex)
    {
        BodyRowListView.SelectedIndex = newIndex;
    }

    public void DeselectAndReBind()
    {
        Deselect();
        ReBind();
    }

    public void SelectItemAndReBind(int newIndex)
    {
        SelectItem(newIndex);
        ReBind();
    }

    protected void BodyRowListView_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding && !CheckJavaScriptHelper.IsJavascriptEnabled)
        {
            SetupChildItem(e);
        }

    }

    protected void BodyRowListView_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (!Page.IsPostBack || rebinding)
        {
            SetupChildItem(e);
        }
    }

    protected void BodyRowListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newCol = BodyRowListView.SelectedIndex;
        ParentSelectionChangedHandler((newCol == -1) ? -1 : RowNumber, newCol);
    }
}
