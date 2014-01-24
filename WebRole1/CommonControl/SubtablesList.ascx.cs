using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;




    public partial class SubtablesList : System.Web.UI.UserControl
    {
        protected int? ColumnToSort;
        protected bool SortOrderAscending = true;
        protected bool rebinding = false;
        protected int rowBeingCreated = 0;
        bool ResetToTop = false;


        public PMRoutingInfoMainContent theLocation;
        public IRaterooDataContext RaterooDB;

        public class SubtablesDataNeeded
        {
            public PMRoutingInfoMainContent location { get; set; }
            public IRaterooDataContext theDataContext { get; set; }
        }
        public SubtablesDataNeeded subtablesDataNeeded { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Setup(subtablesDataNeeded.location, subtablesDataNeeded.theDataContext);
        }

        public void Setup(PMRoutingInfoMainContent location, IRaterooDataContext theDataContext)
        {
            theLocation = location;
            RaterooDB = theDataContext;

            int? UserID = null;
            if (HttpContext.Current.Profile != null && ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null)
                UserID = (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            PointsManager firstPointsManager = null;
            if (location != null)
            {
                Tbl aTable = RaterooDB.GetTable<Tbl>().FirstOrDefault(x => x.HierarchyItems.Any() && location.theMenuHierarchy.Contains(x.HierarchyItems.First()));
                if (aTable != null)
                    firstPointsManager = aTable.PointsManager;
            }
            if (firstPointsManager != null)
            {
                bool canViewPage = new RaterooDataAccess().CheckUserRights(UserID, UserActionOldList.View, false, firstPointsManager.PointsManagerID, null);
                if (!canViewPage)
                {
                    PMRouting.Redirect(Response, new PMRoutingInfoLoginRedirect(PMRouting.Outgoing(theLocation)));
                    return;
                }
            };
        }


        public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var theQuery = RaterooDB.GetTable<HierarchyItem>()
                .Where(x => x.HigherHierarchyItemID == theLocation.lastItemInHierarchy.HierarchyItemID);

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
                HierarchyItem theHierarchyItem = (HierarchyItem)dataItem.DataItem;
                ListViewDataItem CurrentItem = (ListViewDataItem)e.Item;
                PlaceHolder thePlaceHolder = (PlaceHolder)e.Item.FindControl("HierarchyItemPlaceHolder");
                LinkButton theLinkButton = new LinkButton();
                theLinkButton.PostBackUrl = theHierarchyItem.RouteToHere;
                theLinkButton.Text = theHierarchyItem.HierarchyItemName;
                thePlaceHolder.Controls.Add(theLinkButton);


                DateTime? concludingDate;
                decimal totalPrize;
                PMHierarchyItems.GetPrizeInfoForHierarchyItem(theHierarchyItem, out totalPrize, out concludingDate);
                string dollarsString;
                const int decimalPlaces = 2;
                dollarsString = MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(totalPrize, decimalPlaces);
                string dateOfAwardString;
                if (totalPrize == 0)
                    dateOfAwardString = "N/A";
                else if (concludingDate == null)
                    dateOfAwardString = "See tables";
                else
                    dateOfAwardString = concludingDate.ToString();
                Label dollarsThisPeriod = (Label)e.Item.FindControl("DollarsThisPeriod");
                dollarsThisPeriod.Text = dollarsString;
                Label dateOfAward = (Label)e.Item.FindControl("DateOfAward");
                dateOfAward.Text = dateOfAwardString;
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
            //if (resetToTop)
            //    Pager.SetPageProperties(0, Pager.MaximumRows, false);
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