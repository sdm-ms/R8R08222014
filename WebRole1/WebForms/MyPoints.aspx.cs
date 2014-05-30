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

using WebRole1.CommonControl;
using ClassLibrary1.Model;

public partial class MyPoints : System.Web.UI.Page
{
    internal int UserID;


    protected int? ColumnToSort;
    protected bool SortOrderAscending = true;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    bool ResetToTop = false;
    internal R8RDataManipulation theDataAccessModule;

    protected void Page_Load(object sender, EventArgs e)
    {
        theDataAccessModule = new R8RDataManipulation();
        if (HttpContext.Current.Profile != null && (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID") != 0)
            UserID = (int) ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
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
        public PointsManager PointsManager { get; set; }
        public string PointsManagerName {get; set;}
        public Tbl FirstTable {get; set;}
        public PointsTotal PointsTotal { get; set; }
        public decimal Lifetime {get; set;}
        public decimal Dollars {get; set;}
        public decimal Current {get; set;}
        public decimal Pending { get; set; }
        public decimal ExpectedWinnings {get; set;}
    }

    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        var thePointsQuery = from p in theDataAccessModule.DataContext.GetTable<PointsTotal>()
                                  where p.User.UserID == UserID
                                  let thisUserPendingAndCurrentPoints = p.PendingPoints + p.CurrentPoints
                                  let thisUserNotYetPendingPoints = p.NotYetPendingPoints
                                  let allPointsManagerPendingPoints = p.PointsManager.CurrentUserPoints + p.PointsManager.CurrentUserPendingPoints
                                  let divideByZeroAvoid = 0.0001M
                                  let percentageOfPointsManagerPoints = thisUserPendingAndCurrentPoints /(allPointsManagerPendingPoints + divideByZeroAvoid)
                                  let expectedWinningsUnadjusted = (p.PointsManager.CurrentPeriodDollarSubsidy * percentageOfPointsManagerPoints)
                                  let numPrizes = (p.PointsManager.NumPrizes == 0) ? -1 : p.PointsManager.NumPrizes /* we use -1 to avoid dividing by zero; note that linq to sql will evaluate both sides of a ||, even if unnecessary */
                                  let expectedWinnings = (numPrizes == -1 
                                                            || expectedWinningsUnadjusted < p.PointsManager.CurrentPeriodDollarSubsidy / numPrizes) ? expectedWinningsUnadjusted // we'll just calculate it in the same way as long as this produces an estimate of less than the average prize
                                                            : (p.PointsManager.CurrentPeriodDollarSubsidy / numPrizes) * (decimal) (1 - (Math.Pow((double) 1 -(double) percentageOfPointsManagerPoints, (double)numPrizes))) // 1 - probability of not winning any prizes, disregarding the fact that probability rises for subsequent prizes */
                                  select new 
                                    {
                                        Key = new {UserID = p.UserID, PointsManagerID = p.PointsManagerID},
                                        PointsTotal = p,
                                        PointsManager = p.PointsManager,
                                        PointsManagerName = p.PointsManager.Name,
                                        FirstTable = p.PointsManager.Tbls.FirstOrDefault(x => x.Name != "Changes"),
                                        Lifetime = p.TotalPoints,
                                        Current = p.CurrentPoints,
                                        Pending = thisUserPendingAndCurrentPoints - p.CurrentPoints,
                                        ExpectedWinnings = expectedWinnings,
                                        Percentage = percentageOfPointsManagerPoints,
                                        Unadjusted = expectedWinningsUnadjusted
                                    };
        var theDollarsQuery = theDataAccessModule.DataContext.GetTable<PointsAdjustment>()
            .Where(p => p.User.UserID == UserID && p.CashValue > 0)
            .Select(p => new 
            {
                Key = new {UserID = p.UserID, PointsManagerID = p.PointsManagerID},
                Dollars = p.CashValue
            });
        var theQuery = from p in thePointsQuery
                       join d in theDollarsQuery on p.Key equals d.Key into g
                       from d in g.DefaultIfEmpty()
                       orderby p.PointsManagerName
                       select new PointsInfo
                       {
                           PointsManager = p.PointsManager,
                           PointsManagerName = p.PointsManagerName,
                           FirstTable = p.FirstTable,
                           PointsTotal = p.PointsTotal,
                           Lifetime = p.Lifetime,
                           Dollars = d.Dollars ?? 0,
                           Current = p.Current,
                           Pending = p.Pending,
                           ExpectedWinnings = p.ExpectedWinnings
                       }
                       ;

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
            PointsInfo myPointsInfo = (PointsInfo)dataItem.DataItem;

            PlaceHolder thePlaceHolder = (PlaceHolder)e.Item.FindControl("PathToItem");
            CommonControl_ItemPath theItemPath = (CommonControl_ItemPath)LoadControl("~/CommonControl/ItemPath.ascx");
            theItemPath.theTbl = myPointsInfo.FirstTable;
            thePlaceHolder.Controls.Add(theItemPath);

            PlaceHolder thePlaceHolder2 = (PlaceHolder)e.Item.FindControl("GuaranteeInfo");
            PaymentGuaranteeInfo guaranteeInfo = (PaymentGuaranteeInfo)LoadControl("~/CommonControl/PaymentGuaranteeInfo.ascx");
            guaranteeInfo.ThePointsManager = myPointsInfo.PointsManager;
            guaranteeInfo.ThePointsTotal = myPointsInfo.PointsTotal;
            guaranteeInfo.TheUser = myPointsInfo.PointsTotal.User;
            guaranteeInfo.TheDataAccess = theDataAccessModule;
            thePlaceHolder2.Controls.Add(guaranteeInfo);
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
