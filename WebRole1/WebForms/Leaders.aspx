<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="Leaders" Title="Rateroo: Leaderboard" Codebehind="Leaders.aspx.cs" %>
<%@ Register TagPrefix="Uc" TagName="ItemPath" Src="~/CommonControl/ItemPath.ascx" %>
<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls"
    Assembly="ClassLibrary1" %>
<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    <Uc:ItemPath ID="ItemPath1" runat="server"></Uc:ItemPath>
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <table>
        <tr style="height: 11px;">
            <td>
            </td>
        </tr>
    </table>
    <asp:LinqDataSource ID="MainLinqDataSource" runat="server" ContextTypeName="UserRatingDataAccess"
        OnSelecting="MainLinqDataSource_Selecting">
    </asp:LinqDataSource>
    <table id="maint" width="100%" class="mainTable mainTableWithBorders"
        cellspacing="0" border="0">
        <tr>
            <th class="nmcl">
                #
            </th>
            <th>
                User
            </th>
            <th>
                Expected $ This Period
            </th>
            <th>
                Lifetime Points
            </th>
            <th>
                Points This Period
            </th>
            <th>
                Pending Points
            </th>
        </tr>
        <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
            DataKeyNames="" OnDataBinding="MainListView_DataBinding"
            OnItemCreated="MainListView_ItemCreated" OnItemDataBound="MainListView_ItemDataBound">
            <EmptyDataTemplate>
                <tr>
                    <td colspan="99">
                        Sorry. No points found.
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
                    <td>
                        <asp:Literal ID="Label4" runat="server" Text='<%# FormatLinkToUsersRatings(Eval("Username") as string, (int) Eval("Userid")) %>' />
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text='<%# FormatPoints(Container.DataItem, "ExpectedWinnings")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text='<%# FormatPoints(Container.DataItem, "Lifetime")%>' />
                    </td>
                    <td> 
                        <asp:Label ID="Label8" runat="server" Text='<%# FormatPoints(Container.DataItem, "Current")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text='<%# FormatPoints(Container.DataItem, "Pending")%>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <tr id="PagerRow">
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
    </table>
</asp:Content>
