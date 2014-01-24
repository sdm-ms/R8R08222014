<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="PMSearchWordsFilter" Codebehind="PMSearchWordsFilter.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<asp:UpdatePanel runat="server" ID="SearchWordsUpdatePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <tr class="borderless">
            <td class="borderless">
                <table class="borderless">
                <tbody>
                    <tr class="borderless">
                        <td class="borderless">
                            <asp:Label ID="LabelBeforeTextBox" runat="server" Text="Search" class="filterFieldName"></asp:Label>
                        </td>
                    </tr>
                    <tr class="borderless">
                        <td class="borderless">
                            <asp:TextBox ID="TxtMain" runat="server" class="filterTextBoxFullLength"></asp:TextBox>
                        </td>
                    </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </ContentTemplate>
</asp:UpdatePanel>
