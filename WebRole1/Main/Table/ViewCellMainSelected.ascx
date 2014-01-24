<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_ViewCellMainSelected" Codebehind="ViewCellMainSelected.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellMainData" Src="~/Main/Table/ViewCellMainData.ascx" %>
<%@ Reference Control="~/Main/Table/ViewCellAdministrativeOptions.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<Uc:LiteralElement ID="tdTag" runat="server" ElementType="td" OpeningTag="true" ClosingTag="false" />
<input id="grpC" runat="server" type="hidden" class="mgID" />
<Uc:ViewCellMainData runat="server" id="CellData"></Uc:ViewCellMainData>
<div>
    <asp:ImageButton ID="BtnEnter" runat="server" ImageUrl="~/images/accept.gif" OnClick="BtnEnter_Click" />
    <asp:ImageButton ID="BtnCancel" runat="server" ImageUrl="~/images/cancel.gif" OnClick="BtnCancel_Click" />
    <asp:PlaceHolder ID="AdministrativeOptions" runat="server"></asp:PlaceHolder>
</div>
<Uc:LiteralElement ID="tdTagClose" runat="server" ElementType="td" OpeningTag="false" ClosingTag="true" />
