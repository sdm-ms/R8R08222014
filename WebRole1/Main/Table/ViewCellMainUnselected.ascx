<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_ViewCellMainUnselected" Codebehind="ViewCellMainUnselected.ascx.cs" %>
<%@ Register  TagPrefix="Uc" TagName="ViewCellMainData" Src="~/Main/Table/ViewCellMainData.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<Uc:LiteralElement ID="tdTag" runat="server" ElementType="td" OpeningTag="true" ClosingTag="false"  />
<input id="grpC" runat="server" type="hidden" class="mgID" />
<Uc:ViewCellMainData runat="server" ID="CellData" EnableViewState="false"></Uc:ViewCellMainData>
<Uc:LiteralElement ID="tdTagClose" runat="server" ElementType="td" OpeningTag="false"
    ClosingTag="true" />
