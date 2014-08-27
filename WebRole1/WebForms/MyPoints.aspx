<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="MyPoints" Title="R8R: My Points and Winnings" Codebehind="MyPoints.aspx.cs" %>

<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls"
    Assembly="ClassLibrary1" %>
<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    My Points and Winnings
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
    <table id="maint" width="100%" class="mainTableSmall mainTableWithBorders mainTableHeadingSmall"
        cellspacing="0" border="0">
        <tr>
            <th class="nmcl">
                #
            </th>
            <th>
                Topic
            </th>
            <th>
                Lifetime $ Earned
            </th>
            <th>
                Preliminary Estimate
                Expected $ This Period
            </th>
            <th>
                Guaranteed $ For
                This Period
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
                        <asp:PlaceHolder ID="PathToItem" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="Label6" runat="server" Text='<%# FormatPoints(Container.DataItem, "Dollars")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text='<%# FormatPoints(Container.DataItem, "ExpectedWinnings")%>' />
                    </td>
                    <td>
                        <asp:PlaceHolder ID="GuaranteeInfo" runat="server" />
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
            <td colspan="8" class="pagerRow">
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
        </tr><tr id="warningRow" runat="server">
            <td colspan="8" class="mimicRevertRatings">
                <span><b>Important note:</b> If you rate poorly, it may appear at first that you 
                are earning many points and that your expected winnings are high, but as soon as 
                other users start challenging your ratings, pending points may disappear and 
                expected winnings may fall all the way to zero.</span>
            </td>
        </tr>
    </table>
</asp:Content>
