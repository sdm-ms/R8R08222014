<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NumberFieldFilter" Codebehind="NumberField.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<asp:UpdatePanel runat="server" ID="NumberUpdatePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="0 0 0 0" cellspacing="0 0 0 0">
            <tr>
<%--                <td>
                    <asp:Label ID="Label1" runat="server" Text="From"></asp:Label>
                </td>--%>
                <Uc:LiteralElement ID="TD1a" runat="server" ElementType="td" OpeningTag="true" ClosingTag="false" />
                    <asp:TextBox ID="TxtFrom" runat="server" CssClass="filterTextBox"></asp:TextBox>
<%--                    <asp:RegularExpressionValidator ID="FromBoxValidator" runat="server" Display="None"
                        ControlToValidate="TxtFrom" ValidationExpression="(^-?([0-9]*|\d*\d{1}?\d*)$)"
                        ErrorMessage="Numeric data required."></asp:RegularExpressionValidator>
                    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" TargetControlID="FromBoxValidator"
                        runat="server">
                    </Ajax:ValidatorCalloutExtender>--%>
                <Uc:LiteralElement ID="TD1b" runat="server" ElementType="td" OpeningTag="false" ClosingTag="true" />
                <Uc:LiteralElement ID="TD2a" runat="server" ElementType="td" OpeningTag="true" ClosingTag="false" />
                    <asp:Label ID="Label2" runat="server" Text="&nbsp;To&nbsp;"></asp:Label>
                <Uc:LiteralElement ID="TD2b" runat="server" ElementType="td" OpeningTag="false" ClosingTag="true" />
                <td>
                    <asp:TextBox ID="TxtTo" runat="server" CssClass="filterTextBox"></asp:TextBox>
<%--                    <asp:RegularExpressionValidator ID="ToBoxValidator" runat="server"
                        ControlToValidate="TxtTo" ValidationExpression="(^-?([0-9]*|\d*\d{1}?\d*)$)"
                        ErrorMessage="Numeric data required"></asp:RegularExpressionValidator>--%>
<%--                    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" TargetControlID="ToBoxValidator"
                        runat="server">
                    </Ajax:ValidatorCalloutExtender>--%>
                    <%--<asp:Button ID="BtnClear" runat="server" Text="Clear" CssClass="Btn1" OnClick="BtnClear_Click"
                        CausesValidation="False" />--%>
                </td>
            </tr>
<%--            <tr>

            </tr>--%>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
