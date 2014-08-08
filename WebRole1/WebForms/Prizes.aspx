<%@ Page Title="Prize Board" Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="Prizes" Codebehind="Prizes.aspx.cs" %>

<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls"
    Assembly="ClassLibrary1" %>
<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    Prize Board
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
    <table id="maint" width="100%" class="mainTable mainTableFont mainTableWithBorders" cellspacing="0" border="0">
        <tr>
            <th class="nmcl">
                #
            </th>
            <th>
                Topic
            </th>
            <th>
                Username
            </th>
            <th>
                Date of Award
            </th>
            <th>
                Points Earned in Award Period
            </th>
            <th>
                $ Earned
            </th>
        </tr>
        <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
            DataKeyNames="" OnDataBinding="MainListView_DataBinding" OnItemCreated="MainListView_ItemCreated"
            OnItemDataBound="MainListView_ItemDataBound">
            <EmptyDataTemplate>
                <tr>
                    <td colspan="99">
                        Sorry. No prizes have been awarded yet.
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
                        <asp:Label ID="Label6" runat="server" Text='<%# FormatUserName(Eval("Username") as string) %>' />
                    </td>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Date", "{0:d}")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text='<%# FormatPoints(Container.DataItem, "PointsEarned")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text='<%# FormatPoints(Container.DataItem, "DollarsEarned")%>' />
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
