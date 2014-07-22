using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ClassLibrary1.Model;
using ClassLibrary1.EFModel;


namespace WebRole1.Admin
{
    public class GuaranteeApplicationsItem
    {
        public string FileName { get; set; }
        public string Username { get; set; }
        public Guid UserID { get; set; }
    }

    public partial class GuaranteeApplications : System.Web.UI.UserControl
    {
        RoutingInfoMainContent theLocation;
        R8RDataAccess theDataAccessModule;

        protected int? ColumnToSort;
        protected bool SortOrderAscending = true;
        protected bool rebinding = false;
        protected int rowBeingCreated = 0;
        //bool ResetToTop = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            theLocation = Routing.IncomingMainContent(Page.RouteData, null);
            theDataAccessModule = new R8RDataAccess();
        }

        public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var theQuery = theDataAccessModule.R8RDB.GetTable<PointsTotal>().Where(x => x.PointsManager.PointsManagerID == theLocation.thePointsManager.PointsManagerID && x.PendingConditionalGuaranteeApplication != null && x.PendingConditionalGuaranteeApplication != "").OrderBy(x => x.User.WhenCreated).Select(x => new GuaranteeApplicationsItem { FileName = x.PendingConditionalGuaranteeApplication, Username = x.User.Username, UserID = x.User.UserID });
            e.Result = theQuery;
        }

        public void SetupChildren(ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                rowBeingCreated++;
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                GuaranteeApplicationsItem guaranteeApplicationsItem = (GuaranteeApplicationsItem)dataItem.DataItem;
                string commandArgument = guaranteeApplicationsItem.FileName + "$" + guaranteeApplicationsItem.UserID.ToString();

                LinkButton Download = (LinkButton)e.Item.FindControl("DownloadBtn");
                Button Approve = (Button)e.Item.FindControl("ApproveBtn");
                Button Reject = (Button)e.Item.FindControl("RejectBtn");

                Download.CommandArgument = commandArgument;
                Approve.CommandArgument = commandArgument;
                Reject.CommandArgument = commandArgument;
            }
        }

        public GuaranteeApplicationsItem GetGuaranteeApplicationsItemFromCommandArgument(string commandArgument)
        {
            string[] split = commandArgument.Split('$');
            GuaranteeApplicationsItem item = new GuaranteeApplicationsItem { FileName = split[0], UserID = new Guid(split[1]), Username = theDataAccessModule.R8RDB.GetTable<User>().Single(x => x.UserID == new Guid(split[1])).Username };
            return item;
        }

        public PointsTotal GetPointsTotalFromClickEvent(object sender)
        {
            Button button = (Button)sender;
            GuaranteeApplicationsItem item = GetGuaranteeApplicationsItemFromCommandArgument(button.CommandArgument);
            return theDataAccessModule.R8RDB.GetTable<PointsTotal>().SingleOrDefault(x => x.PointsManager == theLocation.thePointsManager && x.UserID == item.UserID);
        }

        public void Download_Click(object sender, EventArgs e)
        {
            PaymentGuarantees.DownloadConditionalGuaranteeApplicationForNewUser(Response, GetPointsTotalFromClickEvent(sender));
        }

        public void Approve_Click(object sender, EventArgs e)
        {
            ListViewDataItem dataItem = (ListViewDataItem) ((Button)sender).NamingContainer;
            TextBox amountTextBox = (TextBox) dataItem.FindControl("Amount");
            try
            {
                decimal amount = Convert.ToDecimal(amountTextBox.Text);
                PaymentGuarantees.ApproveConditionalGuaranteeForNewUser(GetPointsTotalFromClickEvent(sender), amount);
                theDataAccessModule.R8RDB.SubmitChanges();
            }
            catch
            {
                ((TextBox)FindControl("Amount")).ForeColor = System.Drawing.Color.Red;
            }
        }

        public void Reject_Click(object sender, EventArgs e)
        {
            PaymentGuarantees.RejectConditionalGuaranteeForNewUser(GetPointsTotalFromClickEvent(sender));
            theDataAccessModule.R8RDB.SubmitChanges();
        }


        public void MainListView_DataBinding(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            { // usually, we don't databound on postback
                rebinding = true;
            }
        }

        //protected void MainListView_OnItemCommand(object sender, ListViewCommandEventArgs e)
        //{
        //    ListViewDataItem dataItem = (ListViewDataItem)e.Item;
        //    DataKey currentDataKey = MainListView.DataKeys[dataItem.DisplayIndex];
        //    string fileName = (string)currentDataKey["FileName"];
        //    Guid userID = (int)currentDataKey["UserID"];
        //    switch (e.CommandName)
        //    {
        //        case "Download":
        //            Download(fileName, userID);
        //            break;
        //        case "Approve":
        //            Approve(fileName, userID);
        //            break;
        //        case "Reject":
        //            Reject(fileName, userID);
        //            break;
        //    }
        //}

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

        //public void ReBind(bool resetToTop)
        //{
        //    rebinding = true;
        //    if (resetToTop)
        //        Pager.SetPageProperties(0, Pager.MaximumRows, false);
        //    MainListView.DataBind();
        //}

    }
}