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
using System.Collections.Generic;


using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
////using PredRatings;

public partial class Main_Table_HeaderRow : System.Web.UI.UserControl
{
    protected int TblTabID { get; set; }
    protected Action<int?, bool>ResortCateDesFn;
    protected int? LimitToThisTblColumnID;
    protected int? TblColumnToSortID;
    protected bool SortByEntityName = false;
    protected bool  DoSortOrderAscending;
    protected R8RDataAccess DataAccess { get; set; }
    protected bool rebinding = false;
    public LoadHeaderRowInfo theHeaderRowInfo { get; set; }

    public void Page_Load(object sender, EventArgs e)
    {
        if (theHeaderRowInfo != null)
        {
            int? TblColumnToSort;
            bool SortOrderAscending = true; // may change below
            TblTab theTblTab = theHeaderRowInfo.dataAccess.R8RDB.GetTable<TblTab>().Single(cg => cg.TblTabID == theHeaderRowInfo.TblTabID);
            TblColumnToSort = theHeaderRowInfo.TblColumnToSortID;
            SortByEntityName = theHeaderRowInfo.SortByTblRowName;
            SortOrderAscending = theHeaderRowInfo.ascending;
            //if (TblColumnToSort != null)
            //{
            //    TblColumn theSortCD = theHeaderRowInfo.dataAccess.R8RDB.GetTable<TblColumn>().SingleOrDefault(cd => cd.TblColumnID == TblColumnToSort && cd.Status == (Byte)StatusOfObject.Active);
            //    if (theSortCD == null)
            //        TblColumnToSort = null;
            //    else
            //        SortOrderAscending = theSortCD.DefaultSortOrderAscending;
            //}
            TableSortRule theTableSortRule;
            if (TblColumnToSort == null)
            {
                if (SortByEntityName)
                    theTableSortRule = new TableSortRuleRowName(SortOrderAscending);
                else
                {
                    //TblTab theTblTa2b;
                    //Tbl theTbl;
                    //PointsManager thePointsManager;
                    //TableLoading.GetTblAndPointsManagerForTblTab(DataAccess, theTblTab.TblTabID, out theTblTab2, out theTbl, out thePointsManager);
                    //bool userIsTrusted = DataAccess.UserCounts(thePointsManager.PointsManagerID, (int)userID);
                    theTableSortRule = new TableSortRuleNeedsRating(); // Doesn't matter for purpose of header row whether the user is untrusted
                }
            }
            else
                theTableSortRule = new TableSortRuleTblColumn((int)TblColumnToSort, SortOrderAscending);
            Setup(theHeaderRowInfo.dataAccess, theHeaderRowInfo.TblTabID, null, null, theTableSortRule);
            DataBind();
        }
    }

    public void Setup(R8RDataAccess dataAccess, int tblTabID, int? limitToThisTblColumnID, Action<int?, bool>resortCateDesFn, TableSortRule theTableSortRule)
    {
       
        DataAccess = dataAccess;
        TblTabID = tblTabID;
        LimitToThisTblColumnID = limitToThisTblColumnID;
        ResortCateDesFn = resortCateDesFn;
        if (theTableSortRule is TableSortRuleTblColumn)
        {
            TblColumnToSortID = ((TableSortRuleTblColumn)theTableSortRule).TblColumnToSortID;
            SortByEntityName = false;
            DoSortOrderAscending = ((TableSortRuleTblColumn)theTableSortRule).Ascending;
        }
        else if (theTableSortRule is TableSortRuleRowName)
        {
            SortByEntityName = true;
            TblColumnToSortID = null;
            DoSortOrderAscending = theTableSortRule.Ascending;
        }
        else if (theTableSortRule is TableSortRuleNeedsRating || theTableSortRule is TableSortRuleNeedsRatingUntrustedUser)
        {
            SortByEntityName = false;
            TblColumnToSortID = null;
            DoSortOrderAscending = theTableSortRule.Ascending; // ignored
        }
        HeaderLinqDataSource.Selecting += new EventHandler<LinqDataSourceSelectEventArgs>(HeaderLinqDataSource_Selecting);
    }

    protected void ResortMainTable(Guid? TblColumnID, bool sortAscending)
    {
        ResortCateDesFn(TblColumnID, sortAscending);
    }

  //protected void HeaderListView_OnItemCommand(object sender, ListViewCommandEventArgs e)
  //{
  //  if (String.Equals(e.CommandName, "CustomSort"))
  //  {
  //    ListViewDataItem dataItem = (ListViewDataItem)e.Item;
  //    TblColumnID = (int)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["TblColumnID"];
  //    CurrentlySorting = (bool)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["CurrentlySorting"];
  //    DoSortOrderAscending = (bool)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["DoSortOrderAscending"];
  //    if (CurrentlySorting)
  //        DoSortOrderAscending = !DoSortOrderAscending; // reverse current sort
  //       ResortCateDesFn(TblColumnID, DoSortOrderAscending);
        
       
  // }
  //}

    class HeaderRowInfoType
    {
        public Guid? TblColumnID { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string WidthStyle { get; set; }
        public bool SortableColumn { get; set; }
        public bool CurrentlySorting { get; set; }
        public bool DoSortOrderAscending { get; set; }
        public bool VerticalText { get; set; }
    }

    protected void HeaderLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {

        // first, figure out whether to use vertical columns.
        bool useVerticalColumns = NumberandTableFormatter.UseVerticalColumns(DataAccess, TblTabID, LimitToThisTblColumnID, false); 

        // This seems to be called twice when the page first loads;
        // I'm not sure why. But it doesn't take long to run.
       var theQuery = DataAccess.R8RDB.GetTable<TblColumn>()
                .Where(x => x.TblTabID == TblTabID 
                        && (LimitToThisTblColumnID == null || x.TblColumnID == LimitToThisTblColumnID)
                        && x.Status == (byte)StatusOfObject.Active)
                .Select(x => new HeaderRowInfoType
                {
                    TblColumnID = x.TblColumnID,
                    Abbreviation = x.Abbreviation,
                    Name = x.Name,
                    WidthStyle = x.WidthStyle,
                    SortableColumn = x.Sortable,
                    CurrentlySorting = TblColumnToSortID != null && x.TblColumnID == TblColumnToSortID,
                    DoSortOrderAscending = (TblColumnToSortID != null && x.TblColumnID == TblColumnToSortID) ? DoSortOrderAscending : x.DefaultSortOrderAscending,
                    VerticalText = useVerticalColumns

                });
       Tbl theTbl = DataAccess.R8RDB.GetTable<TblTab>().Single(x => x.TblTabID == TblTabID).Tbl;
       HeaderRowInfoType numColumnHeader = new HeaderRowInfoType
       {
           TblColumnID = -1,
           Abbreviation = "#",
           Name = "#",
           WidthStyle = "nmcl " + theTbl.WidthStyleNumCol,
           SortableColumn = false,
           CurrentlySorting = false,
           DoSortOrderAscending = true,
           VerticalText = false
       };
       string theNameForTblRow = theTbl.TypeOfTblRow;
       HeaderRowInfoType nameColumnHeader = new HeaderRowInfoType
                {
                    TblColumnID = -1, // using null here causes problem on postback; we'll change it back below
                    Abbreviation = "",
                    Name = theNameForTblRow,
                    WidthStyle = theTbl.WidthStyleEntityCol,
                    SortableColumn = true,
                    CurrentlySorting = SortByEntityName,
                    DoSortOrderAscending = SortByEntityName ? DoSortOrderAscending : true,
                    VerticalText = false
                };
        List<HeaderRowInfoType> completeList = new List<HeaderRowInfoType>();
        completeList.Add(numColumnHeader);
        completeList.Add(nameColumnHeader);

        var theResult = (completeList.ToList()).Concat(theQuery.ToList());
        e.Result = theResult;
       
    }

    public void SetupChild(ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            Guid? TblColumnID = (int?)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["TblColumnID"];
            if (TblColumnID == -1) // see note above
                TblColumnID = null; 
            string theAbbreviation = (string)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["Abbreviation"];
            string theName = (string)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["Name"];
            string theWidthStyle = (string)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["WidthStyle"];
            bool theSortableColumn = (bool)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["SortableColumn"];
            bool theCurrentlySorting = (bool)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["CurrentlySorting"];
            bool theDoSortOrderAscending = (bool)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["DoSortOrderAscending"];
            bool verticalText = (bool)HeaderListView.DataKeys[dataItem.DisplayIndex].Values["VerticalText"];
            Main_Table_ViewCellColumnHeading theColumnHeading = (Main_Table_ViewCellColumnHeading)e.Item.FindControl("Main_Table_ViewCellColumnHeading");
            theColumnHeading.Setup(DataAccess, ResortMainTable, TblColumnID, null, theAbbreviation, theName, theWidthStyle, theSortableColumn, theCurrentlySorting, theDoSortOrderAscending, (LimitToThisTblColumnID != null), verticalText);

        }
    }

    protected void HeaderListView_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding)
        {
            SetupChild(e);
        }
        
    }

    protected void HeaderListView_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
       
        if (!Page.IsPostBack || rebinding)
        {
            SetupChild(e);
        }
       

    }

    public void ReBind(int tblTabID, TableSortRule aTableSortRule)
    {
        rebinding = true;
        TblTabID = tblTabID;
        if (aTableSortRule is TableSortRuleTblColumn)
        {
            TblColumnToSortID = ((TableSortRuleTblColumn)aTableSortRule).TblColumnToSortID;
            DoSortOrderAscending = aTableSortRule.Ascending;
            SortByEntityName = false;
        }
        else if (aTableSortRule is TableSortRuleRowName)
        {
            SortByEntityName = true;
            TblColumnToSortID = null;
            DoSortOrderAscending = aTableSortRule.Ascending;
        }
        else
            throw new Exception("Internal error: only sorting by column and entity name is supported without javascript.");
        HeaderListView.DataBind();
    }
}
