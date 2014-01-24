<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="PMCategoryFilter" Codebehind="PMCategoryFilter.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<asp:UpdatePanel runat="server" ID="CategoryUpdatePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <table >
            <tr>
<%--                <td>
                    <asp:Label ID="Label1" runat="server" Text="From"></asp:Label>
                </td>--%>
                <td>
                    <asp:TextBox ID="TxtFrom" runat="server" CssClass="filterTextBox"></asp:TextBox>
<%--                    <asp:RegularExpressionValidator ID="FromBoxValidator" runat="server" Display="None"
                        ControlToValidate="TxtFrom" ValidationExpression="(^-?([0-9]*|\d*\d{1}?\d*)$)"
                        ErrorMessage="Numeric data required"></asp:RegularExpressionValidator>
                    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" TargetControlID="FromBoxValidator"
                        runat="server">
                    </Ajax:ValidatorCalloutExtender>--%>
                </td>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="&nbsp;To&nbsp;"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TxtTo" runat="server" CssClass="filterTextBox"></asp:TextBox>
<%--                    <asp:RegularExpressionValidator ID="ToBoxValidator" runat="server" Display="None"
                        ControlToValidate="TxtTo" ValidationExpression="(^-?([0-9]*|\d*\d{1}?\d*)$)"
                        ErrorMessage="Numeric data required"></asp:RegularExpressionValidator>
                    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" TargetControlID="ToBoxValidator"
                        runat="server">
                    </Ajax:ValidatorCalloutExtender>--%>
                    <%-- <asp:Button ID="BtnClear" runat="server" Text="Clear" CssClass="Btn1" OnClick="BtnClear_Click"
                        CausesValidation="False" />--%>
                </td>
            </tr>
<%--            <tr>

            </tr>--%>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
