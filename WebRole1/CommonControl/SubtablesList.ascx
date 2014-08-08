<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubtablesList.ascx.cs" Inherits="SubtablesList" %>
<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls" Assembly="ClassLibrary1" %>
<table>
    <tr style="height: 11px;">
        <td>
        </td>
    </tr>
</table>
<asp:LinqDataSource ID="MainLinqDataSource" runat="server" ContextTypeName="R8RDB"
    OnSelecting="MainLinqDataSource_Selecting">
</asp:LinqDataSource>
<table id="maint" width="100%" class="mainTable mainTableFont mainTableWithBorders" cellspacing="0"
    border="0">
    <tr>
        <th class="nmcl">
            #
        </th>
        <th>
            Topic
        </th>
        <th>
            Current Prizes ($)
        </th>
        <th>
            Date to be Awarded
        </th>
    </tr>
    <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
        DataKeyNames="" OnDataBinding="MainListView_DataBinding" OnItemCreated="MainListView_ItemCreated"
        OnItemDataBound="MainListView_ItemDataBound">
        <EmptyDataTemplate>
            <tr>
                <td colspan="99">
                    Sorry. No topics are currently available.
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
                    <SNFWC:dataitemcounter id="DataItemCounter1" indexformat="{0}. " indexoffset="1"
                        runat="server" />
                </td>
                <td>
                    <asp:PlaceHolder ID="HierarchyItemPlaceHolder" runat="server"></asp:PlaceHolder>
                </td>
                <td>
                    <asp:Label ID="DollarsThisPeriod" runat="server" />
                </td>
                <td>
                    <asp:Label ID="DateOfAward" runat="server" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
<%--    <tr id="SubtablesPager">
        <td colspan="99" class="pagerRow">
            <asp:Panel runat="server" ID="pagerFieldPanel">
                <div>
                    <asp:DataPager ID="Pager" runat="server" PagedControlID="MainListView" PageSize="10">
                        <Fields>
                            <SNFWC:GooglePagerField NextPageImageUrl="~/Images/button_arrow_right.gif" PreviousPageImageUrl="~/Images/button_arrow_left.gif" />
                        </Fields>
                    </asp:DataPager>
                </div>
            </asp:Panel>
        </td>
    </tr>--%>
</table>
