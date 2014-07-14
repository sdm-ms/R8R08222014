using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Misc;

public partial class Prizes : System.Web.UI.Page
{
    protected int? ColumnToSort;
    protected bool SortOrderAscending = true;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    bool ResetToTop = false;
    bool userIsSuperUser = false;


    public string FormatUserName(string username)
    {
        if (userIsSuperUser)
        {
            IUserProfileInfo user = UserProfileCollection.LoadByUsername(username);
            return username + " " + user.Email;
        }
        else
            return username;
    }

    public string FormatPoints(object dataItem, string itemString)
    {
        try
        {
            decimal points = (decimal)DataBinder.Eval(dataItem, itemString);
            const int decimalPlaces = 2;
            string resultString = MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(points, decimalPlaces);
            return resultString;
        }
        catch
        {
            return "--";
        }
    }

    internal class PrizeInfo
    {
        public string PointsManagerName { get; set; }
        public Tbl FirstTable { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public decimal PointsEarned { get; set; }
        public decimal? DollarsEarned { get; set; }
    }

    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
        User theUser = theDataAccessModule.DataContext.GetTable<User>().SingleOrDefault(u => u.UserID == (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"));
        if (theUser != null && theUser.SuperUser)
            userIsSuperUser = true;
        var theQuery = theDataAccessModule.DataContext.GetTable<PointsAdjustment>()
            .Where(p => p.CashValue > 0)
            .Select(p => new PrizeInfo
            {
                PointsManagerName = p.PointsManager.Name,
                FirstTable = p.PointsManager.Tbls.Where(x => x.Name != "Changes").FirstOrDefault(),
                Username = p.User.Username,
                Date = p.WhenMade,
                PointsEarned = 0 - p.CurrentAdjustment,
                DollarsEarned = p.CashValue
            })
            .OrderByDescending(p => p.Date)
            .ThenBy(p => p.PointsManagerName)
            .ThenBy(p => p.FirstTable.TblID)
            .ThenBy(p => p.DollarsEarned);
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
            PrizeInfo myPrizeInfo = (PrizeInfo)dataItem.DataItem;
            PlaceHolder thePlaceHolder = (PlaceHolder)e.Item.FindControl("PathToItem");
            CommonControl_ItemPath theItemPath = (CommonControl_ItemPath)LoadControl("~/CommonControl/ItemPath.ascx");
            theItemPath.theTbl = myPrizeInfo.FirstTable;
            thePlaceHolder.Controls.Add(theItemPath);
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
