<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="ViewTbl" Title="R8R" Codebehind="Main.aspx.cs" %>
<%@ OutputCache Duration="10" VaryByParam="all" VaryByCustom="noPostbackMain" %>
<%--<%@ OutputCache Duration="30" VaryByParam="TableId" %>--%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="FieldsBox" Src="~/Main/Field/FieldsBox.ascx" %>
<%@ Reference Control="~/Main/Field/FieldsBox.ascx" %>
<%@ Reference Control="~/Main/Table/WithTabSelector.ascx" %>
<%@ Register TagPrefix="Uc" TagName="TblRowView" Src="~/Main/Table/TblRowView.ascx" %>
<%@ Reference Control="~/Main/Table/TblRowView.ascx" %>
<%@ Register TagPrefix="Uc" TagName="TableCellView" Src="~/Main/Table/TableCellView.ascx" %>
<%@ Reference Control="~/Main/Table/TableCellView.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="InsertableContent" Src="~/CommonControl/InsertableContent.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ItemPath" Src="~/CommonControl/ItemPath.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SupplementalScripts" runat="Server">
    <script type="text/javascript">
	    $(document).ready(function() {
	    });
	</script>
    <asp:ScriptManagerProxy ID="MyScriptManagerProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/js/viewtbl.js?v=511" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/WebService.asmx" InlineScript="true" />
        </Services>
    </asp:ScriptManagerProxy>
</asp:Content>
<asp:Content ID="ContentNarrowSearch" ContentPlaceHolderID="BoxTopLeftContentPlaceHolder"
    runat="server">
<%--    <asp:PlaceHolder runat="server" ID="FieldsBoxPlaceHolder" />--%>
        <% Response.WriteSubstitution(new HttpResponseSubstitutionCallback(ClassLibrary1.Model.CacheSubstitution.MyPointsSidebar)); %>
    <Uc:FieldsBox runat="server" ID="FieldsBox" />
</asp:Content>
<asp:Content ID="HeadContent" runat="server" ContentPlaceHolderID="ContentHeadText">
    <Uc:ItemPath ID="ItemPath1" runat="server">
    </Uc:ItemPath>
</asp:Content>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentMain">
    <input type="hidden" id="shouldPopulate" value="true" />
    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="text-align: left;"
        id="Table1" runat="server" class="possibleBottom">
        <tr>
            <td>
                <div id="GoogleMap" class="map"></div>
            </td>
        </tr>
        <tr>
            <td align="left">
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="TblMessage">
                            <Uc:InsertableContent runat="server" ID="TopOfViewTblContent" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" valign="top">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%"  cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="2" >
                                                <asp:PlaceHolder ID="MainContentPlaceHolder" runat="server"></asp:PlaceHolder>
                                                <%-- <Uc:WithTabSelector ID="MainTableWithTabSelector" runat="server" />--%>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                <div id="AdministrativeOptions" runat="server">
                    <asp:Button ID="BtnAddTblRow" runat="server" CssClass="BtnBig possibleBottom" OnClick="AddOrChangeTblRow_Click" UseSubmitBehavior="true" ></asp:Button>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="BtnViewChanges" runat="server" Text="Rate Selected Changes" CssClass="BtnBig possibleBottom" OnClick="ViewChanges_Click"
                        UseSubmitBehavior="true"></asp:Button>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="BtnAdministration" runat="server" Text="Change Page" CssClass="BtnBig possibleBottom"
                        Visible="false" UseSubmitBehavior="true"></asp:Button>
                </div>
                <Uc:ModalPopUp runat="server" ID="PopUp" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentUpdatingEachPostBack" runat="Server">
    <div id="cluetipouter">
        <div id="cluetipdiv">
            <a id="cluetipanchor" href="#messagetext" rel="#messagetext" title="Note" >
                <p id="messagetext" visible="false">
                </p>
            </a>
        </div>
    </div>
</asp:Content>
