using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using System.Web.UI.HtmlControls;
using ClassLibrary1.Model;

public partial class Main_Table_RecentRatings : System.Web.UI.UserControl
{
    public int? TopRatingGroupID { get; set; }
    public int? TblRowID { get; set; }
    public int? PointsManagerID { get; set; }
    protected int? ColumnToSort;
    protected bool SortOrderAscending = true;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    bool ResetToTop = false;
    protected PMRoutingInfoMainContent Location { get; set; }
    RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();

    protected void Page_Load(object sender, EventArgs e)
    {
        Location = PMRouting.IncomingMainContent(Page.RouteData, theDataAccessModule.DataContext);
        if (Location.theTblColumn != null)
            RatingHeader.Visible = false;
    }

    

    protected string FormatPoints(object dataItem, string itemString)
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


    protected void AppendPoints(StringBuilder myStringBuilder, UserRating theUserRating, bool includeShortTerm, bool includeLongTerm)
    {
        decimal amount = 0;
        if (includeShortTerm)
            amount += Math.Round(theUserRating.PotentialPointsShortTerm, 1);
        if (includeLongTerm)
            amount += Math.Round(theUserRating.PotentialPointsLongTerm, 1);
        myStringBuilder.Append(amount.ToString());
        if (
            (includeShortTerm && !theUserRating.ResolvedShortTerm)
            || (includeLongTerm && !theUserRating.Resolved)
            )
            myStringBuilder.Append("*");
    }

    protected string FormatPointsAll(object dataItem)
    {
        UserRating theRating = DataBinder.Eval(dataItem, "UserRating") as UserRating;
        decimal? LongTermPointsWeight = (decimal?) DataBinder.Eval(dataItem, "LongTermPointsWeight");
        StringBuilder myStringBuilder = new StringBuilder();
        if (LongTermPointsWeight == 0 || LongTermPointsWeight == 1)
        {
            AppendPoints(myStringBuilder, theRating, true, true);
        }
        else
        {
            myStringBuilder.Append("Short-term: ");
            AppendPoints(myStringBuilder, theRating, true, false);
            myStringBuilder.Append("<br/>Long-term: ");
            AppendPoints(myStringBuilder, theRating, false, true);
        }
        return myStringBuilder.ToString();
    }

    protected string FormatRating(object dataItem, string itemString)
    {
        string resultString = "--";
        try
        {
            decimal? rating = (decimal?)DataBinder.Eval(dataItem, itemString);
            if (rating == null)
                return resultString;
            int decimalPlaces = Convert.ToInt32(DataBinder.Eval(dataItem, "DecimalPlaces"));
            resultString = MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(rating, decimalPlaces);
        }
        catch
        {
        }
        return resultString;
    }

    protected string FormatRatingWithPossibleAsterisk(object dataItem, string itemString)
    {
        string resultString = "--";
        try
        {
            decimal? rating = (decimal?)DataBinder.Eval(dataItem, itemString);
            if (rating == null)
                return resultString;
            int decimalPlaces = Convert.ToInt32(DataBinder.Eval(dataItem, "DecimalPlaces"));
            resultString = MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(rating, decimalPlaces);
            bool asterisk = ((bool?)DataBinder.Eval(dataItem, "Trusted")) == false;
            if (asterisk)
                resultString += "*";
        }
        catch
        {
        }
        return resultString;
    }

    internal class RecentRatingInfo
    {
        public UserRating UserRating { get; set; }
        public Rating Rating { get; set; }
        public User User { get; set; }
        public PointsTotal PointsTotal { get; set; }
        public DateTime Date { get; set; }
        public decimal? Previous { get; set; }
        public decimal? NewRating { get; set; }
        public bool? Trusted { get; set; } /* must be nullable b/c of quirk in CLR/SQL type mismatch */
        public byte DecimalPlaces { get; set; }
        public decimal LongTermPointsWeight { get; set; }
    }

    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        IQueryable<UserRating> beginningOfQuery;
        if (TopRatingGroupID == null)
            beginningOfQuery = theDataAccessModule.DataContext.GetTable<UserRating>().Where(p => p.Rating.RatingGroup.TblRowID == TblRowID);
        else
            beginningOfQuery = theDataAccessModule.DataContext.GetTable<UserRating>().Where(p => p.Rating.TopmostRatingGroupID == TopRatingGroupID);

        IQueryable<RecentRatingInfo> theQuery = beginningOfQuery
            .OrderByDescending(p => p.UserRatingGroup.WhenMade)
            .ThenByDescending(p => p.UserRatingGroupID)
            .ThenBy(p => p.Rating.NumInGroup)
            .Select(p => new RecentRatingInfo
            {
                UserRating = p,
                Rating = p.Rating,
                User = p.User,
                PointsTotal = p.User.PointsTotals.SingleOrDefault(x => x.PointsManagerID == p.Rating.RatingGroup.TblRow.Tbl.PointsManagerID),
                Date = p.UserRatingGroup.WhenMade,
                Previous = (p.PreviousDisplayedRating != null) ? (decimal?) p.PreviousRatingOrVirtualRating : (decimal?) null,
                Trusted = ((p.Rating.CurrentValue == p.Rating.LastTrustedValue) || (p.Rating.CurrentValue != p.NewUserRating)) || p.Rating.CurrentValue == null, 
                NewRating = p.NewUserRating,
                DecimalPlaces = p.Rating.RatingCharacteristic.DecimalPlaces,
                LongTermPointsWeight = p.Rating.RatingGroup.RatingGroupAttribute.LongTermPointsWeight
            }
            );
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
            RecentRatingInfo theInfo = (RecentRatingInfo)dataItem.DataItem;

            HtmlAnchor theAnchor = (HtmlAnchor)e.Item.FindControl("UserLink");
            theAnchor.HRef = PMRouting.Outgoing(new PMRoutingInfoRatings(theInfo.User.UserID));
            theAnchor.InnerText = theInfo.User.Username;

            Label theUserPointsLabel = (Label)e.Item.FindControl("UserPoints");
            int points = 0;
            if (theInfo.PointsTotal != null)
                points = (int) Math.Round(theInfo.PointsTotal.TotalPoints);
            theUserPointsLabel.Text = " (" + points.ToString() + " pts)";

            PlaceHolder thePlaceHolder = (PlaceHolder)e.Item.FindControl("RatingPlaceHolder");
            CommonControl_ItemPath theItemPath = (CommonControl_ItemPath)LoadControl("~/CommonControl/ItemPath.ascx");
            theItemPath.theTblRow = theInfo.Rating.RatingGroup.TblRow;
            theItemPath.theTbl = theItemPath.theTblRow.Tbl;
            theItemPath.theTblColumn = theInfo.Rating.RatingGroup.TblColumn;
            if (Location.theTblRow != null)
            {
                theItemPath.SuppressRow = true;
                theItemPath.SuppressTable = true;
                if (Location.theTblColumn != null)
                {
                    HtmlTableCell theTableCell = (HtmlTableCell)e.Item.FindControl("RatingCell");
                    theTableCell.Visible = false;
                }
                
            }
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
