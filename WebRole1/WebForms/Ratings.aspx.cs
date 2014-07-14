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

using System.Text;
using MoreStrings;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

public partial class Ratings : System.Web.UI.Page
{
    internal int UserIDOfRatingsBeingViewed;
    internal int? UserIDOfBrowsingUser;
    protected bool browsingUserIsTrusted = false;
    protected bool browsingUserIsAdmin = false;

    protected int? ColumnToSort;
    protected bool SortOrderAscending = true;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    bool ResetToTop = false;
    R8RDataManipulation theDataAccessModule;

    bool notHighStakesRatings ;
    bool highStakesKnownRatings ;
    bool highStakesPreviouslySecretRatings ;

    protected void Page_Init(object sender, EventArgs e)
    {
        theDataAccessModule = new R8RDataManipulation();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RoutingInfoRatings theRatingsInfo = Routing.IncomingRatings(Page.RouteData, theDataAccessModule.DataContext);
        if (HttpContext.Current.Profile != null && ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null)
            UserIDOfBrowsingUser = (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        if (UserIDOfBrowsingUser != null)
        {
            User browsingUser = theDataAccessModule.DataContext.GetTable<User>().SingleOrDefault(u => u.UserID == UserIDOfBrowsingUser);
            if (browsingUser != null)
            {
                browsingUserIsAdmin = browsingUser.Username == "admin";
                RevertTrusted.Visible = browsingUserIsAdmin;
            }
        }

        if (theRatingsInfo.userID == null)
        {
            if (UserIDOfBrowsingUser == null)
                Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
            UserIDOfRatingsBeingViewed = (int) UserIDOfBrowsingUser;
            whoseRatings.Text = "My Ratings";
        }
        else
        {
            UserIDOfRatingsBeingViewed = (int)theRatingsInfo.userID;
            User theUser = theDataAccessModule.DataContext.GetTable<User>().SingleOrDefault(u => u.UserID == UserIDOfRatingsBeingViewed);
            if (theUser == null)
                Routing.Redirect(Response, new RoutingInfo(RouteID.HomePage));
            whoseRatings.Text = theUser.Username + "&#146;s Ratings";
        }

        //if (!browsingUserIsTrusted || UserIDOfRatingsBeingViewed == UserIDOfBrowsingUser) // now we're restricting this to admins
        mimicRow.Visible = false; // this functionality has been disabled as no longer necessary

        if (browsingUserIsAdmin)
        {
            userTimeRow.Visible = true;
            SetUserTimeSpans();
        }
        else
            userTimeRow.Visible = false;

        if (!Page.IsPostBack)
        {
            NotHighStakesCheckbox.Checked = true;
            HighStakesKnownCheckbox.Checked = true;
            HighStakesPreviouslySecretCheckbox.Checked = true;
        }
         GetInfoFromCheckBoxes();
    }

    protected void GetInfoFromCheckBoxes()
    {
        notHighStakesRatings = NotHighStakesCheckbox.Checked ;
        highStakesKnownRatings = HighStakesKnownCheckbox.Checked ;
        highStakesPreviouslySecretRatings = HighStakesPreviouslySecretCheckbox.Checked ;
    }

    protected decimal? GetItemValue(object dataItem, string itemString)
    {
        try
        {
            decimal? points = (decimal?)DataBinder.Eval(dataItem, itemString);
            return points;
        }
        catch
        {
            return null;
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
        UserRating theUserRating = DataBinder.Eval(dataItem, "UserRating") as UserRating;
        decimal? LongTermPointsWeight = GetItemValue(dataItem, "LongTermPointsWeight");
        StringBuilder myStringBuilder = new StringBuilder();
        if (theUserRating.NotYetPendingPoints != 0 && theUserRating.EnteredUserRating != theUserRating.PreviousRatingOrVirtualRating)
            myStringBuilder.Append("Not yet scored");
        else if (LongTermPointsWeight == 0 || LongTermPointsWeight == 1)
            AppendPoints(myStringBuilder, theUserRating, true, false);
        else if (LongTermPointsWeight == 1)
            AppendPoints(myStringBuilder, theUserRating, false, true);
        else
        {
            myStringBuilder.Append("Short-term: ");
            AppendPoints(myStringBuilder, theUserRating, true, false);
            myStringBuilder.Append("<br/>Long-term: ");
            AppendPoints(myStringBuilder, theUserRating, false, true);
        }
        return myStringBuilder.ToString();
    }

    protected string FormatRating(object dataItem, string itemString)
    {
        try
        {
            decimal? rating = (decimal?)DataBinder.Eval(dataItem, itemString);
            if (rating == null)
                return "--";
            int decimalPlaces = Convert.ToInt32(DataBinder.Eval(dataItem, "DecimalPlaces"));
            string resultString = MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(rating, decimalPlaces);
            return resultString;
        }
        catch
        {
            return "--";
        }
    }

    protected string FormatRatingAll(object dataItem)
    {
        StringBuilder myStringBuilder = new StringBuilder();

        myStringBuilder.Append("Previous:&nbsp;");
        myStringBuilder.Append(FormatRating(dataItem, "Previous"));
        myStringBuilder.Append("<br>");
        myStringBuilder.Append("<b>Mine:&nbsp;");
        myStringBuilder.Append(FormatRating(dataItem, "Rating"));
        myStringBuilder.Append("</b>");
        myStringBuilder.Append("<br>");
        myStringBuilder.Append("Now:&nbsp;");
        myStringBuilder.Append(FormatRating(dataItem, "Current"));
        return myStringBuilder.ToString();
    }

    internal class MyRatingInfo
    {
        public UserRating UserRating { get; set; }
        public TblRow TblRow { get; set; }
        public TblColumn TblColumn { get; set; }
        public Tbl Tbl { get; set; }
        public Rating RatingObject { get; set; }
        public RatingGroup RatingGroup { get; set; }
        public DateTime Date { get; set; }
        public decimal Rating { get; set; }
        public decimal? Previous { get; set; }
        public decimal? Current { get; set; }
        public decimal LongTermPointsWeight { get; set; }
        public byte DecimalPlaces { get; set; }
    }

    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {

        // We are putting the skip and take in the middle of the query for performance reasons. This
        // requires our setting AutoPage to false on the LinqDataSource control.
        var beginningOfQuery = theDataAccessModule.DataContext.GetTable<UserRating>()
            .Where(p => p.UserID == UserIDOfRatingsBeingViewed)
            .Where(p => (notHighStakesRatings && !p.HighStakesKnown && !p.HighStakesPreviouslySecret) || (highStakesKnownRatings && p.HighStakesKnown) || (highStakesPreviouslySecretRatings && p.HighStakesPreviouslySecret))
            .OrderByDescending(p => p.UserRatingGroup.WhenMade)
            .ThenBy(p => p.Rating.NumInGroup);


        var theQuery = beginningOfQuery
            .Skip(Pager.StartRowIndex).Take(Pager.PageSize)
            .ToList() /* given how linq sql constructs queries, this provides large performance benefits for users with many thousands of ratings */
            .Select(p => new MyRatingInfo
            {
                UserRating = p,
                TblRow = p.Rating.RatingGroup.TblRow,
                TblColumn = p.Rating.RatingGroup.TblColumn,
                Tbl = p.Rating.RatingGroup.TblRow.Tbl,
                RatingObject = p.Rating,
                RatingGroup = p.Rating.RatingGroup, /* load it eagerly so that item path can have info */
                Date = p.UserRatingGroup.WhenMade,
                Rating = p.EnteredUserRating,
                Previous = (p.PreviousDisplayedRating != null) ? (decimal?) p.PreviousRatingOrVirtualRating : (decimal?) null,
                Current = p.Rating.CurrentValue,
                LongTermPointsWeight = p.Rating.RatingGroup.RatingGroupAttribute.LongTermPointsWeight,
                DecimalPlaces = p.Rating.RatingCharacteristic.DecimalPlaces
            }
            )
            ;

        e.Arguments.StartRowIndex = Pager.StartRowIndex;
        e.Arguments.MaximumRows = Pager.PageSize;
        e.Arguments.TotalRowCount = beginningOfQuery.Count();
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
            MyRatingInfo myRatingInfo = (MyRatingInfo)dataItem.DataItem;
            PlaceHolder thePlaceHolder = (PlaceHolder) e.Item.FindControl("PathToItem");
            CommonControl_ItemPath theItemPath = (CommonControl_ItemPath)LoadControl("~/CommonControl/ItemPath.ascx");
            theItemPath.theTbl = myRatingInfo.Tbl;
            theItemPath.theTblRow = myRatingInfo.TblRow;
            theItemPath.theTblColumn = myRatingInfo.TblColumn;
            theItemPath.theRating = myRatingInfo.RatingObject;
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
        ReBind( ResetToTop);

    }

    protected void MimicOrRevert(bool mimic, bool untrustedOnly = true)
    {
        throw new NotImplementedException();
        //if (browsingUserIsTrusted && UserIDOfBrowsingUser != null)
        //    theDataAccessModule.RevertOrMimicRatings((int)UserIDOfRatingsBeingViewed, (int)UserIDOfBrowsingUser, mimic,untrustedOnly);
        //mimicRow.Visible = false;
        //ReBind(true);
    }

    protected void Mimic_Click(object sender, EventArgs e)
    {
        MimicOrRevert(true);
    }

    protected void Revert_Click(object sender, EventArgs e)
    {
        MimicOrRevert(false);
    }

    protected void RevertTrusted_Click(object sender, EventArgs e)
    {
        if (!browsingUserIsAdmin)
            throw new Exception("Internal error: Revert trusted ratings button should be available only to admin.");
        MimicOrRevert(false, false);
    }

    protected void NotHighStakesCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        GetInfoFromCheckBoxes();
        ReBind(true);
    }

    protected void HighStakesKnownCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        GetInfoFromCheckBoxes();
        ReBind(true);
    }

    protected void HighStakesPreviouslySecretCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        GetInfoFromCheckBoxes();
        ReBind(true);
    }

    protected void SetUserTimeSpans()
    {

        TimeSpan1.Text = RaterTime.GetTimeForUser(UserIDOfRatingsBeingViewed, TestableDateTime.Now - new TimeSpan(1, 0, 0, 0), TestableDateTime.Now).ToString();
        TimeSpan2.Text = RaterTime.GetTimeForUser(UserIDOfRatingsBeingViewed, TestableDateTime.Now - new TimeSpan(3, 0, 0, 0), TestableDateTime.Now).ToString();
        TimeSpan3.Text = RaterTime.GetTimeForUser(UserIDOfRatingsBeingViewed, TestableDateTime.Now - new TimeSpan(7, 0, 0, 0), TestableDateTime.Now).ToString();
        TimeSpan4.Text = RaterTime.GetTimeForUser(UserIDOfRatingsBeingViewed, TestableDateTime.Now - new TimeSpan(28, 0, 0, 0), TestableDateTime.Now).ToString();
    }
}
