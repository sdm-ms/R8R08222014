<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_BodyRow" Codebehind="BodyRow.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellMainSelected" Src="~/Main/Table/ViewCellMainSelected.ascx" %>
<%@ Reference Control="~/Main/Table/ViewCellMainUnselected.ascx" %>

<asp:ListView runat="server" ID="BodyRowListView" DataSourceID="BodyRowLinqDataSource"
    OnItemCreated="BodyRowListView_ItemCreated" OnItemDataBound="BodyRowListView_ItemDataBound"
    DataKeyNames="TblColumnID" OnSelectedIndexChanged="BodyRowListView_SelectedIndexChanged">
    <LayoutTemplate>
        <asp:PlaceHolder id="itemPlaceholder" runat="server" >
        </asp:PlaceHolder>
    </LayoutTemplate>
    <ItemTemplate>
        <asp:PlaceHolder ID="CellMainPlaceholder" runat="server" />
    </ItemTemplate>
    <SelectedItemTemplate>
        <asp:PlaceHolder ID="CellMainSelectedPlaceholder" runat="server" />
    </SelectedItemTemplate>
</asp:ListView>
<asp:LinqDataSource ID="BodyRowLinqDataSource" runat="server" OnSelecting="BodyRowLinqDataSource_Selecting">
</asp:LinqDataSource>
