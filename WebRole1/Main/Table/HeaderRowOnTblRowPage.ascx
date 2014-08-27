<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Main_Table_HeaderRowOnTblRowPage" Codebehind="HeaderRowOnTblRowPage.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellColumnHeading" Src="~/Main/Table/ViewCellColumnHeading.ascx" %>
<asp:ListView runat="server" ID="HeaderListView" DataSourceID="HeaderLinqDataSource"
    DataKeyNames="TblColumnID,Abbreviation,Name,WidthStyle,VerticalText" OnItemDataBound="HeaderListView_ItemDataBound"
    OnItemCreated="HeaderListView_ItemCreated">
    <LayoutTemplate>
        <th id="itemPlaceholder" runat="server">
        </th>
    </LayoutTemplate>
    <ItemTemplate>
        <Uc:ViewCellColumnHeading ID="Main_Table_ViewCellColumnHeading" runat="server"></Uc:ViewCellColumnHeading>
    </ItemTemplate>
</asp:ListView>
<asp:LinqDataSource runat="server" ID="HeaderLinqDataSource" OnSelecting="HeaderLinqDataSource_Selecting">
</asp:LinqDataSource>
