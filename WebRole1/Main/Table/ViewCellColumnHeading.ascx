<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Main_Table_ViewCellColumnHeading" Codebehind="ViewCellColumnHeading.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="AngledText" Src="~/CommonControl/AngledText.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<th id="cellcolumnheading" runat="server" class="sort">
    <Uc:LiteralElement runat="server" id="ColumnPopUpMouseOverArea" OpeningTag="true" ClosingTag="false" ElementType="div" 
         MarkupAttributeType1="class" MarkupAttributeContent1="loadcolumnpopup"/>
        <asp:PlaceHolder ID="MyPlaceHolder" runat="server"></asp:PlaceHolder>
    <Uc:LiteralElement runat="server" ID="ColumnPopUpMouseOverAreaClose" OpeningTag="false"
        ClosingTag="true" ElementType="div"/>
    <Uc:LiteralElement runat="server" ID="ColumnPopUpContent" OpeningTag="true" ClosingTag="false" ElementType="div" 
         MarkupAttributeType1="style" MarkupAttributeContent1="display:none;" /> 
        <asp:Literal ID="ColumnPopUpLiteral" runat="server"></asp:Literal>
    <Uc:LiteralElement runat="server" ID="ColumnPopUpContentClose" OpeningTag="false" ClosingTag="true" ElementType="div" />
</th>
