<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_RecentRatings" Codebehind="RecentRatings.ascx.cs" %>
<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls"
    Assembly="ClassLibrary1" %>
<asp:UpdatePanel ID="RecentRatingsUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:LinqDataSource ID="MainLinqDataSource" runat="server" ContextTypeName="UserRatingDataAccess"
        OnSelecting="MainLinqDataSource_Selecting">
    </asp:LinqDataSource>
    <table id="recentRatingsTable" width="100%" class="mainTable mainTableFont mainTableSmall mainTableWithBorders mainTableHeadingLarge"
        cellspacing="0" border="0">
        <tr>
            <th class="nmcl">
                #
            </th>
            <th id="RatingHeader" runat="server">
                Item
            </th>
            <th>
                User
            </th>
            <th>
                Date
            </th>
            <th>
                Time
            </th>
            <th>
                Last Trusted
            </th>
            <th>
                New Rating
            </th>
        </tr>
        <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
            DataKeyNames="" OnDataBinding="MainListView_DataBinding"
            OnItemCreated="MainListView_ItemCreated" OnItemDataBound="MainListView_ItemDataBound">
            <EmptyDataTemplate>
                <tr>
                    <td colspan="99">
                        No previous ratings.
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
                        <SNFWC:dataitemcounter id="DataItemCounter1" indexformat="{0}. "
                            indexoffset="1" runat="server" />
                    </td>
                    <td id="RatingCell" runat="server">
                        <asp:PlaceHolder ID="RatingPlaceHolder" runat="server"></asp:PlaceHolder>
                    </td>
                    <td>
                        <a id="UserLink" runat="server" />
                        <asp:Label ID="UserPoints" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Date", "{0:d}")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Date", "{0:t}")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text='<% #FormatRating(Container.DataItem, "Previous") %>' />
                    </td>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text='<% #FormatRatingWithPossibleAsterisk(Container.DataItem, "NewRating") %>' />
                    </td>
                    
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <tr id="RecentRatingsPager">
            <td colspan="99" class="pagerRow">
                <asp:Panel runat="server" ID="pagerFieldPanel">
                    <div>
                        <asp:DataPager ID="Pager" runat="server" PagedControlID="MainListView" PageSize="10">
                            <Fields>
                                <SNFWC:GooglePagerField NextPageImageUrl="~/Images/button_arrow_right.gif"
                                    PreviousPageImageUrl="~/Images/button_arrow_left.gif" />
                            </Fields>
                        </asp:DataPager>
                    </div>
                </asp:Panel>
            </td>
        </tr>
        <%--<tr id="asteriskRow">
            <td colspan="99" class="asteriskRow">
                <span><i>* Current rating, but by an untrusted user</i></span>
            </td>
        </tr>--%>
    </table>
    </ContentTemplate>
</asp:UpdatePanel>
