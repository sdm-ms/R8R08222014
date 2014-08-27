<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_Table" Codebehind="Table.ascx.cs" %>
<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls"
    Assembly="ClassLibrary1" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellTopLeftCorner" Src="~/Main/Table/ViewCellTopLeftCorner.ascx" %>
<%@ Register TagPrefix="Uc" TagName="HeaderRow" Src="~/Main/Table/HeaderRow.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellRowHeading" Src="~/Main/Table/ViewCellRowHeading.ascx" %>
<%@ Register TagPrefix="Uc" TagName="BodyRow" Src="~/Main/Table/BodyRow.ascx" %>
<%@ Reference VirtualPath="~/Main/Table/BodyRow.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>

<asp:LinqDataSource ID="MainLinqDataSource" runat="server" ContextTypeName="UserRatingDataAccess"
    OnSelecting="MainLinqDataSource_Selecting">
</asp:LinqDataSource>
<div id="divAroundMainTable" runat="server" class="divAroundMainTable possibleBottom">
    <Uc:LiteralElement ID="maintLiteral" runat="server" OpeningTag="true" ClosingTag="false" ElementType="table"
        MarkupAttributeType1="ID" MarkupAttributeContent1="maint" />
        <tr>
            <Uc:HeaderRow ID="MainTableHeaderRow" runat="server" />
        </tr>
        <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
            DataKeyNames="TblRowID" OnDataBinding="MainListView_DataBinding" OnItemCreated="MainListView_ItemCreated"
            OnItemDataBound="MainListView_ItemDataBound">
            <EmptyDataTemplate>                
            </EmptyDataTemplate>
            <LayoutTemplate>
                <tr id="itemPlaceholder" runat="server">
                </tr>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="maintr" class="somerow prow <%# Eval("ExtraCSSClass") %>">
                    <td>
                        <SNFWC:dataitemcounter id="DataItemCounter1" indexformat="{0}. "
                            indexoffset="1" runat="server" />
                    </td>
                    <td>
                        <Uc:ViewCellRowHeading ID="MainTableViewCellRowHeading" runat="server" TblRowID='<%# Eval("TblRowID") %>' />
                    </td>
                    <asp:PlaceHolder ID="MainTableBodyRowPlaceHolder" runat="server"></asp:PlaceHolder>
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <Uc:LiteralElement ID="PagerRowLiteral" runat="server" OpeningTag="true" ClosingTag="false" ElementType="tr"
             MarkupAttributeType1="ID"  MarkupAttributeContent1="PagerRow" />
            <Uc:LiteralElement ID="TdInPagerRow" runat="server" OpeningTag="true" ClosingTag="false" ElementType="td"
             MarkupAttributeType1="colspan" MarkupAttributeContent1="99" MarkupAttributeType2="class" MarkupAttributeContent2="pagerRow" />
                <asp:Panel runat="server" ID="pagerFieldPanel" >
                    <div>
                        <asp:Image ID="RefreshPageStill2" runat="server" ImageUrl="~/images/Button-Refresh-Still.gif"
                            class="pageRefreshBtn2" />
                        <asp:Image ID="RefreshPageMoving2" runat="server" ImageUrl="~/images/Button-Refresh-Moving.gif"
                            class="pageRefreshBtn2" Style="display: none;" />
                        <asp:Image ID="b_cancelBulk" runat="server" ImageUrl="~/images/cancel.gif" Style="display: none;"
                            class="pageRefreshBtn2" />
                        <asp:Image ID="b_submitBulk" runat="server" ImageUrl="~/images/accept.gif" Style="display: none;"
                            class="pageRefreshBtn2" />
                        <asp:DataPager ID="Pager" runat="server" PagedControlID="MainListView" PageSize="10">
                            <Fields>
                                <SNFWC:GooglePagerField NextPageImageUrl="~/Images/darrowright.gif"
                                    PreviousPageImageUrl="~/Images/darrowleft.gif" ButtonCssClass="pagerImages" />
                            </Fields>
                        </asp:DataPager>
                    </div>
                </asp:Panel>
            <Uc:LiteralElement ID="TdInPagerRowClose" runat="server" OpeningTag="false" ClosingTag="true" ElementType="td" />
        <Uc:LiteralElement ID="PagerRowLiteralClose" runat="server" OpeningTag="false" ClosingTag="true" ElementType="tr"
             />
    <Uc:LiteralElement ID="maintLiteralClose" runat="server" OpeningTag="false" ClosingTag="true" ElementType="table" />    
</div>
