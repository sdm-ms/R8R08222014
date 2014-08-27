<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_ViewCellMultipleOutcome" Codebehind="ViewCellMultipleOutcome.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="CellRatingValue" Src="~/Main/Table/ViewCellRatingValue.ascx" %>

<asp:ListView ID="MultipleListView" runat="server" 
    DataSourceID="MultipleLinqDataSource"
    DataKeyNames="ratingName,ratingID,value,hierarchyLevel,decimalPlaces,minVal,maxVal,description"
    OnItemCreated="MultipleListView_ItemCreated"
    OnItemDataBound="MultipleListView_ItemDataBound" onselectedindexchanging="MultipleListView_SelectedIndexChanging"
    
    >
    <LayoutTemplate>
        <table class="borderless" width="100%">
            <tr id="itemPlaceholder" runat="server">
            </tr>
        </table>
    </LayoutTemplate>
    <ItemTemplate>
        <tr>
            <td >
                <asp:Literal ID="precedingSpace" runat="server"></asp:Literal>
                <asp:Literal ID="NameMkt" runat="server"></asp:Literal>
                <asp:Literal ID="followingSpace" runat="server"></asp:Literal>
            </td>
            <td class="numInMultOut">
                <asp:PlaceHolder ID="RatingValuePlaceHolder" runat="server"></asp:PlaceHolder>
            </td>
        </tr>
    </ItemTemplate>
</asp:ListView>
<asp:LinqDataSource ID="MultipleLinqDataSource" runat="server" OnSelecting="MultipleLinqDataSource_Selecting">
</asp:LinqDataSource>
