using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using System.Web.UI.HtmlControls;
using MoreStrings;
using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class Main_Table_Comments : System.Web.UI.UserControl
{
    public Tbl theTblOrNullForRowOnly;
    public TblRow theTblRowOrNullForEntireTable;
    R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
    public bool UserCanProposeComments { get; set; }
    public bool UserCanAddComments { get; set; }
    public bool UserCanDeleteComments { get; set; }
    public bool ShowDeletedAndProposedComments { get; set; }

    protected int? ColumnToSort;
    protected bool SortOrderAscending = true;
    protected bool rebinding = false;
    protected int rowBeingCreated = 0;
    bool ResetToTop = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            ShowDeletedAndProposedComments = false;
        if (UserCanDeleteComments)
            ViewProposedAndDeleted.Visible = true;
        else
            ViewProposedAndDeleted.Visible = false;
        if (UserCanProposeComments && !UserCanAddComments)
            AddComment.Text = "Propose Comment";
        if (! (theTblOrNullForRowOnly == null && (UserCanAddComments || UserCanProposeComments)))
        {
            AddCommentRow.Attributes.Add("style", "display:none;");
        }
        if (theTblOrNullForRowOnly != null)
            ViewAllTblComments.Attributes.Add("style", "display:none;");
        else
        {
            RoutingInfoMainContent theLocation = Routing.IncomingMainContent(Page.RouteData, null);
            RoutingInfoMainContent commentsPage = RoutingInfoMainContentFactory.GetRoutingInfo(theLocation.lastItemInHierarchy);
            commentsPage.commentsMode = true;
            ViewAllTblComments.Attributes.Add("href", commentsPage.GetOutgoingRoute());
        }
    }

    protected void ViewProposedAndDeleted_CheckedChanged(object sender, EventArgs e)
    {
        ShowDeletedAndProposedComments = ViewProposedAndDeleted.Checked;
        MainListView.DataBind();
    }

    internal class CommentData
    {
        public TblRow TblRow { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string AuthorAndDate { get; set; }
        public Guid CommentID { get; set; }
        public int Status { get; set; }
    }

    public void MainLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        DateTime alwaysHideCommentsDeletedBeforeThisTime = TestableDateTime.Now - new TimeSpan(7,0,0,0);
        bool isEntireTableQuery = theTblRowOrNullForEntireTable == null;
        var theQuery = theDataAccessModule.DataContext.GetTable<Comment>()
            .Where(x => 
                ((!isEntireTableQuery && x.TblRow.TblRowID == theTblRowOrNullForEntireTable.TblRowID) || (isEntireTableQuery && x.TblRow.Tbl.TblID == theTblOrNullForRowOnly.TblID))
                && (x.Status == (int)StatusOfObject.Active || (UserCanDeleteComments && ShowDeletedAndProposedComments && x.LastDeletedDate > alwaysHideCommentsDeletedBeforeThisTime)))
            .OrderByDescending(x => x.DateTime).Select(
                x => new CommentData { 
                    TblRow = x.TblRow,
                    Title = x.CommentTitle,
                    Comment = x.CommentText, 
                    AuthorAndDate = x.User.Username + " " + x.DateTime.ToString(), 
                    CommentID = x.CommentID, 
                    Status = x.Status 
                });
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
            CommentData commentData = (CommentData)dataItem.DataItem;
            
            Label Title = (Label)e.Item.FindControl("Title");
            Label Comment = (Label)e.Item.FindControl("Comment");
            Label AuthorAndDate = (Label)e.Item.FindControl("AuthorAndDate");
            Button DeleteComment = (Button)e.Item.FindControl("DeleteComment");

            Title.Text = commentData.Title;
            Comment.Text = commentData.Comment;
            AuthorAndDate.Text = commentData.AuthorAndDate;
            DeleteComment.CommandArgument = commentData.CommentID.ToString() + "," + commentData.Status.ToString();
            if (commentData.Status == (int)StatusOfObject.Unavailable)
                DeleteComment.Text = "Undelete Comment";
            if (commentData.Status == (int)StatusOfObject.Proposed)
                DeleteComment.Text = "Approve Comment";
            if (!UserCanDeleteComments)
                DeleteComment.Visible = false;

            if (theTblOrNullForRowOnly != null)
            {
                HtmlAnchor RowLink = (HtmlAnchor)e.Item.FindControl("RowLink");
                Label RowName = (Label)e.Item.FindControl("RowName");
                Label RowNameSeparator = (Label)e.Item.FindControl("RowNameSeparator");

                RowLink.Attributes.Add("href", Routing.Outgoing(RoutingInfoMainContentFactory.GetRoutingInfo(theTblOrNullForRowOnly, commentData.TblRow)));
                RowName.Text = commentData.TblRow.Name;
                RowNameSeparator.Text = "&nbsp;";
            }
        }
    }


    public void AddComment_Click(object sender, EventArgs e)
    {
        Guid? userID = (Guid?)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        ActionProcessor theProcess = new ActionProcessor();
        if (userID != null && (UserCanProposeComments || UserCanAddComments))
        {
            string commentTitle = MoreStringManip.StripHtml(NewCommentTitle.Text).Replace("\n", "<br/>");
            string commentText = MoreStringManip.StripHtml(NewCommentText.Text).Replace("\n", "<br/>");
            theProcess.CommentForTblRowCreate(theTblRowOrNullForEntireTable.TblRowID, commentTitle, commentText, TestableDateTime.Now, (Guid)userID, !UserCanAddComments, true, null);
        }
        Routing.Redirect(Response, Routing.Incoming(Page.RouteData, theProcess.DataContext));
    }

    private void GetCommentInfo(Button theButton, out Guid commentID, out bool isProposed, out bool isDeleted)
    {
        string[] commandData = theButton.CommandArgument.Split(',');
        commentID = new Guid(commandData[0]);
        int status = Int32.Parse(commandData[1]);
        isProposed = status == (int)StatusOfObject.Proposed;
        isDeleted = status == (int)StatusOfObject.Unavailable;
    }

    public void DeleteComment_Click(object sender, EventArgs e)
    {
        Guid commentID;
        bool isProposed;
        bool isDeleted;
        GetCommentInfo((Button)sender, out commentID, out isProposed, out isDeleted);
        Guid? userID = (Guid?)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");
        ActionProcessor theProcess = new ActionProcessor();
        if (userID != null && UserCanDeleteComments)
            theProcess.CommentForTblRowDeleteOrUndelete(commentID, !isDeleted && !isProposed, (Guid)userID, true, null);
        Routing.Redirect(Response, Routing.Incoming(Page.RouteData, theProcess.DataContext));
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
