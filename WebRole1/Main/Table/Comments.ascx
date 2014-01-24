<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_Comments"
    CodeBehind="Comments.ascx.cs" %>
<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls" Assembly="ClassLibrary1" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<asp:UpdatePanel ID="CommentsUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:LinqDataSource ID="MainLinqDataSource" runat="server" ContextTypeName="UserRatingDataAccess"
            OnSelecting="MainLinqDataSource_Selecting">
        </asp:LinqDataSource>
        <table id="CommentsTable" width="100%" class="mainTable mainTableSmall mainTableWithBorders mainTableHeadingLarge"
            cellspacing="0" border="0">
            <tr>
                <th class="nmcl">
                    #
                </th>
                <th>
                    Comment
                </th>
            </tr>
            <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
                DataKeyNames="" OnDataBinding="MainListView_DataBinding" OnItemCreated="MainListView_ItemCreated"
                OnItemDataBound="MainListView_ItemDataBound">
                <EmptyDataTemplate>
                    <tr>
                        <td colspan="99">
                            No comments.
                        </td>
                    </tr>
                </EmptyDataTemplate>
                <LayoutTemplate>
                    <tr id="itemPlaceholder" runat="server">
                    </tr>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="maintr">
                        <td>
                            <SNFWC:DataItemCounter ID="DataItemCounter1" IndexFormat="{0}. " IndexOffset="1"
                                runat="server" />
                        </td>
                        <td>
                            <b><a id="RowLink" runat="server"><asp:Label ID="RowName" runat="server"></asp:Label></a><asp:Label ID="RowNameSeparator" runat="server"></asp:Label><asp:Label ID="Title" runat="server" /></b>
                            <br />
                            <div class="comment">
                                <asp:Label ID="Comment" runat="server" CssClass="comment"  />
                            </div>
                            <br />
                            <asp:Button ID="DeleteComment" runat="server" Text="Delete Inappropriate Comment"
                                OnClick="DeleteComment_Click" Style="font-size: 12px;" />
                            <i><asp:Label ID="AuthorAndDate" runat="server" /></i>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
            <tr id="CommentsPager">
                <td colspan="2" class="pagerRow">
                    <asp:Panel runat="server" ID="pagerFieldPanel">
                        <div>
                            <asp:DataPager ID="Pager" runat="server" PagedControlID="MainListView" PageSize="5">
                                <Fields>
                                    <SNFWC:GooglePagerField NextPageImageUrl="~/Images/button_arrow_right.gif" PreviousPageImageUrl="~/Images/button_arrow_left.gif" />
                                </Fields>
                            </asp:DataPager>
                        </div>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <asp:CheckBox ID="ViewProposedAndDeleted" runat="server" Text="View Proposed and Recently Deleted Comments"
            OnCheckedChanged="ViewProposedAndDeleted_CheckedChanged" AutoPostBack="true" />
        <br />
        <a id="ViewAllTblComments" runat="server">View Most Recent Comments for All Table Rows</a>
        <table id="AddCommentRow" runat="server" class="addCommentArea">
            <tr >
                <td colspan="2">
                    <asp:Button ID="AddComment" runat="server" Text="Add Comment" OnClick="AddComment_Click" />
                    <br />
                    <span>Title</span>
                    <br />
                    <asp:TextBox ID="NewCommentTitle" runat="server" Width="95%"></asp:TextBox>
                    <br />
                    <span>Comment</span>
                    <br />
                    <asp:TextBox ID="NewCommentText" runat="server" TextMode="MultiLine" Height="150px" Width="95%"></asp:TextBox>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
