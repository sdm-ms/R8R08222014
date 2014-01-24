<%@ Page Title="Rateroo: Change Row Information" Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" ValidateRequest="false" AutoEventWireup="true" Inherits="Row" Codebehind="Row.aspx.cs" %>
<%@ Register TagPrefix="Uc" TagName="PMFieldsBox" Src="~/Main/Field/PMFieldsBox.ascx" %>
<%@ Reference Control="~/Main/Field/PMFieldsBox.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ItemPath" Src="~/CommonControl/ItemPath.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SupplementalScripts" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BoxTopLeftContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="HeadContent" runat="server" ContentPlaceHolderID="ContentHeadText">
    <uc:itempath id="ItemPath1" runat="server">
    </uc:itempath>
</asp:Content>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentMain">
    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="text-align: left;"
        id="Table1" runat="server">
        <tr>
            <td>
                <uc:pmfieldsbox runat="server" id="FieldsBox" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="Label1" CssClass="rowEditMessage" Text="Changes may take a few minutes to appear."></asp:Label>
            </td>
        </tr>
        <tr class="possibleBottom">
            <td>
                <asp:Label runat="server" ID="ChangeNotice" CssClass="rowEditMessage"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentUpdatingEachPostBack" Runat="Server">
</asp:Content>

