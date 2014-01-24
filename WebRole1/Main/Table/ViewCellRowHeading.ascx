<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Main_Table_ViewCellRowHeading" Codebehind="ViewCellRowHeading.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="FieldsDisplay" Src="~/Main/Table/FieldsDisplay.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<Uc:LiteralElement runat="server" id="FieldsDisplayDiv" OpeningTag="true" ClosingTag="false"
    ElementType="div" MarkupAttributeType1="class" MarkupAttributeContent1="loadrowpopup" />
    <asp:PlaceHolder ID="FieldsDisplayPlaceHolder" runat="server"></asp:PlaceHolder>
<Uc:LiteralElement runat="server" ID="FieldsDisplayDivClose" OpeningTag="false" ClosingTag="true"
    ElementType="div" />
<Uc:LiteralElement runat="server" ID="FieldsDisplayPopUp" OpeningTag="true" ClosingTag="false"
    ElementType="div" MarkupAttributeType1="style" MarkupAttributeContent1="display:none;" />
    <asp:PlaceHolder ID="PopUpFieldsDisplayPlaceHolder" runat="server" ></asp:PlaceHolder>
<Uc:LiteralElement runat="server" ID="FieldsDisplayPopUpClose" OpeningTag="false"
    ClosingTag="true" ElementType="div" />

