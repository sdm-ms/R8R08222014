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

using System.Collections.Generic;
using ClassLibrary1.Model;

public partial class SearchResults : System.Web.UI.Page
{
    static int maxResults = 150; // We are limiting results here for now, because we have to put everything in memory that we return.
    internal int UserID;


    protected int? ColumnToSort;
    protected bool SortOrderAscending = true;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    bool ResetToTop = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Profile != null && (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != 0)
            UserID = (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
    }


    public string FormatAsString(object dataItem)
    {
        string theString = dataItem as string;
        if (theString == null)
            return "";
        return theString;
    }


    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
        string decodedPhrase = PMRouting.IncomingSearchResults(Page.RouteData, theDataAccessModule.DataContext).searchTerms;
        IQueryable<string> theQuery = RaterooDataManipulation.GetItemPathStringsForPhrase(theDataAccessModule.DataContext,decodedPhrase, maxResults).AsQueryable();
        e.Result = theQuery;
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
            //int theTblRowID = (int)MainListView.DataKeys[dataItem.DisplayIndex].Values["TblRowID"];
            //int theTblID = (int)MainListView.DataKeys[dataItem.DisplayIndex].Values["TblID"];
            //int theRatingID = (int)MainListView.DataKeys[dataItem.DisplayIndex].Values["RatingID"];
            //ListViewDataItem CurrentItem = (ListViewDataItem)e.Item;
            //PlaceHolder thePlaceHolder = (PlaceHolder)e.Item.FindControl("PathToItem");
            //CommonControl_ItemPath theItemPath = (CommonControl_ItemPath)LoadControl("~/CommonControl/ItemPath.ascx");
            //theItemPath.Setup(theTblID, theTblRowID, theRatingID);
            //thePlaceHolder.Controls.Add(theItemPath);
        }
    }

    public void MainListView_ItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (Page.IsPostBack && !rebinding && !CheckJavaScriptHelper.IsJavascriptEnabled)
        {
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

    public void ReBind(bool resetToTop)
    {
        rebinding = true;
        if (resetToTop)
            Pager.SetPageProperties(0, Pager.MaximumRows, false);
        MainListView.DataBind();
    }

    public void ResortTable(int? theColumn, bool NewSortOrder)
    {
        // Add code for sorting
        ColumnToSort = theColumn;
        SortOrderAscending = NewSortOrder;
        ReBind(ResetToTop);

    }
}
