using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;




    public partial class SubtablesList : System.Web.UI.UserControl
    {
        protected int? ColumnToSort;
        protected bool SortOrderAscending = true;
        protected bool rebinding = false;
        protected int rowBeingCreated = 0;
        bool ResetToTop = false;


        public RoutingInfoMainContent theLocation;
        public IR8RDataContext R8RDB;

        public class SubtablesDataNeeded
        {
            public RoutingInfoMainContent location { get; set; }
            public IR8RDataContext theDataContext { get; set; }
        }
        public SubtablesDataNeeded subtablesDataNeeded { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Setup(subtablesDataNeeded.location, subtablesDataNeeded.theDataContext);
        }

        public void Setup(RoutingInfoMainContent location, IR8RDataContext theDataContext)
        {
            theLocation = location;
            R8RDB = theDataContext;

            int? UserID = null;
            if (HttpContext.Current.Profile != null && ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser() != null)
                UserID = (int)ClassLibrary1.Misc.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
            PointsManager firstPointsManager = null;
            if (location != null)
            {
                Tbl aTable = R8RDB.GetTable<Tbl>().FirstOrDefault(x => x.HierarchyItems.Any() && location.theHierarchy.Contains(x.HierarchyItems.First()));
                if (aTable != null)
                    firstPointsManager = aTable.PointsManager;
            }
            if (firstPointsManager != null)
            {
                bool canViewPage = new R8RDataAccess().CheckUserRights(UserID, UserActionType.View, false, firstPointsManager.PointsManagerID, null);
                if (!canViewPage)
                {
                    Routing.Redirect(Response, new RoutingInfoLoginRedirect(Routing.Outgoing(theLocation)));
                    return;
                }
            };
        }


        public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var theQuery = R8RDB.GetTable<HierarchyItem>()
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
                HierarchyItems.GetPrizeInfoForHierarchyItem(theHierarchyItem, out totalPrize, out concludingDate);
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