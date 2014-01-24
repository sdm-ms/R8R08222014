<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Main_Table_WithCategorySelector" Codebehind="WithCategorySelector.ascx.cs" %>
<%@ Reference VirtualPath="~/Main/Table/Table.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="PMFieldsBox" Src="~/Main/Field/PMFieldsBox.ascx" %>
<%@ Reference Control="~/Main/Field/PMFieldsBox.ascx" %>
<table border="0" width="100%" cellspacing="0" cellpadding="0">
    <tr>
        <td>
            <table class="borderless" style="width: 100%; position:relative; top:2px; left:-2px;">
                <tr style="height:5px;"><td /></tr>
                <tr>
                    <td id="TableSelector" runat="server" align="left" class="TblTabSelector">
                        <asp:DropDownList ID="DdlCategory" runat="server" OnSelectedIndexChanged="DdlCategory_SelectedIndexChanged"
                            AutoPostBack="True" style="display:none;">
                        </asp:DropDownList>
                    </td>
                    <td id="btnsAboveMainTable" align="right">
                        <asp:Image ID="RefreshPageStill" runat="server" ImageUrl="~/images/Button-Refresh-Still.gif"
                            class="pageRefreshBtn" />
                        <asp:Image ID="RefreshPageMoving" runat="server" ImageUrl="~/images/Button-Refresh-Moving.gif"
                            class="pageRefreshBtn" Style="display: none;" />
                        <asp:Image ID="cancelBulk" runat="server" ImageUrl="~/images/cancel.gif" Style="display: none;"
                            class="pageRefreshBtn" />
                        <asp:Image ID="submitBulk" runat="server" ImageUrl="~/images/accept.gif" Style="display: none;"
                            class="pageRefreshBtn" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <asp:UpdatePanel ID="UpdatePanelAroundMainTable" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Literal ID="tInfoLiteral" runat="server"></asp:Literal>
                    <asp:PlaceHolder ID="MainTablePlaceholder" runat="server"></asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
</table>

