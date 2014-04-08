<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="AddressFieldFilter" Codebehind="AddressField.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<asp:UpdatePanel runat="server" ID="AddressUpdatePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <table >
            <Uc:LiteralElement ID="WithinMile" runat="server" ElementType="tr" OpeningTag="true" ClosingTag="false" />
                    <td align="left">
                        Within &nbsp
                        <asp:TextBox ID="TxtMile" runat="server" Width="60px"></asp:TextBox>
<%--                        <asp:RegularExpressionValidator ID="MileREV" Display="None" ControlToValidate="TxtMile"
                            runat="server" ErrorMessage="<b>Invalid Field value</b><br />Enter a numeric value."
                            ValidationExpression="\d{0,18}(\.\d{1,4})?"></asp:RegularExpressionValidator>
                        <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender9" runat="server" TargetControlID="MileREV">
                        </Ajax:ValidatorCalloutExtender>--%>
                        &nbsp miles of:
                    </td>
            <Uc:LiteralElement ID="WithinMileClose" runat="server" ElementType="tr" OpeningTag="false" ClosingTag="true" />
            <tr>
                <td colspan="6" align="left">
                    <asp:TextBox ID="TxtAddress" runat="server" TextMode="MultiLine" Width="200px" Height="36px"></asp:TextBox>
                </td>
<%--                <asp:CustomValidator ID="CustomValidator1" ControlToValidate="TxtAddress" Display="None"
                    runat="server" ErrorMessage="This address could not be found."
                    OnServerValidate="ValidateAddress" />
                <Ajax:validatorcalloutextender id="ValidatorCalloutExtender1" targetcontrolid="CustomValidator1"
                    runat="server" />--%>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
