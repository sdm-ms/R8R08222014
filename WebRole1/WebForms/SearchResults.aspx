<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="SearchResults" Title="R8R: Search Results" Codebehind="SearchResults.aspx.cs" %>

<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls"
    Assembly="ClassLibrary1" %>
<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
Search Results
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
                Search Match
            </th>
        </tr>
        <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
            DataKeyNames="" OnDataBinding="MainListView_DataBinding"
            OnItemCreated="MainListView_ItemCreated" OnItemDataBound="MainListView_ItemDataBound">
            <EmptyDataTemplate>
                <tr>
                    <td colspan="99">
                        Sorry. No matching items were found.
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
                        <SNFWC:DataItemCounter id="DataItemCounter1" indexformat="{0}. "
                            indexoffset="1" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="Topic" runat="server" Text='<% #FormatAsString(Container.DataItem) %>' />
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
