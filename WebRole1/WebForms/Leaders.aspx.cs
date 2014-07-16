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
using ClassLibrary1.EFModel;



public partial class Leaders : System.Web.UI.Page
{
    internal Guid UserID;

    RoutingInfoMainContent theLocation;
    R8RDataAccess DataAccess;

    protected int? ColumnToSort;
    protected bool SortOrderAscending = true;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    bool ResetToTop = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Profile != null && ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null)
            UserID = (Guid)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID"); 
        DataAccess = new R8RDataAccess();
        theLocation = Routing.IncomingMainContent(Page.RouteData, null);
        bool canView = DataAccess.CheckUserRights(UserID, UserActionType.Predict, false, theLocation.theTbl.PointsManagerID, theLocation.theTbl.TblID);
        if (!canView)
            Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
        ItemPath1.theTbl = theLocation.theTbl;
    }

    public string FormatLinkToUsersRatings(string username, Guid userID)
    {
        return "<a href=\"/Ratings/" + userID.ToString() + "\">" + username + "</a>";
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


    internal class PointsInfo
    {
        public string PointsManagerName { get; set; }
        public Tbl FirstTable { get; set; }
        public decimal Lifetime { get; set; }
        public decimal Dollars { get; set; }
        public decimal Current { get; set; }
        public decimal Pending { get; set; }
        public decimal ExpectedWinnings { get; set; }
    }

    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
        var thePointsQuery = from p in theDataAccessModule.DataContext.GetTable<PointsTotal>()
                             where p.PointsManager == theLocation.thePointsManager
                             let allUserPendingPoints = p.PendingPoints + p.NotYetPendingPoints
                             let allPointsManagerPendingPoints = p.PointsManager.CurrentUserPoints + p.PointsManager.CurrentUserPendingPoints + p.PointsManager.CurrentUserNotYetPendingPoints
                             let divideByZeroAvoid = 0.0001M
                             let percentageOfPointsManagerPoints = allUserPendingPoints / (allPointsManagerPendingPoints + divideByZeroAvoid)
                             let expectedWinningsUnadjusted = (p.PointsManager.CurrentPeriodDollarSubsidy * percentageOfPointsManagerPoints)
                             let numPrizes = (p.PointsManager.NumPrizes == 0) ? -1 : p.PointsManager.NumPrizes /* we use -1 to avoid dividing by zero; note that linq to sql will evaluate both sides of a ||, even if unnecessary */
                             let expectedWinnings = (numPrizes == -1
                                                       || expectedWinningsUnadjusted < p.PointsManager.CurrentPeriodDollarSubsidy / numPrizes) ? expectedWinningsUnadjusted // we'll just calculate it in the same way as long as this produces an estimate of less than the average prize
                                                       : (p.PointsManager.CurrentPeriodDollarSubsidy / numPrizes) * (decimal)(1 - (Math.Pow((double)1 - (double)percentageOfPointsManagerPoints, (double)numPrizes))) // 1 - probability of not winning any prizes, disregarding the fact that probability rises for subsequent prizes */
                             where p.User.Username != "admin"
                             orderby p.CurrentPoints descending
                             select new
                             {
                                 Username = p.User.Username,
                                 Userid = p.User.UserID,
                                 PointsManagerName = p.PointsManager.Name,
                                 FirstTable = p.PointsManager.Tbls.FirstOrDefault(x => x.Name != "Changes"),
                                 Lifetime = p.TotalPoints,
                                 Current = p.CurrentPoints,
                                 Pending = allUserPendingPoints,
                                 ExpectedWinnings = expectedWinnings,
                                 Percentage = percentageOfPointsManagerPoints,
                                 Unadjusted = expectedWinningsUnadjusted
                             };

        e.Result = thePointsQuery;
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
            // add more if necessary
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
