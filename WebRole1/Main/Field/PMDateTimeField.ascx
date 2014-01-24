<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="PMDateTimeFieldFilter" Codebehind="PMDateTimeField.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Date" Src="~/CommonControl/Date.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<asp:UpdatePanel runat="server" ID="DateUpdatePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <table >
            <tr>
<%--                <td>
                    From
                </td>--%>
                <Uc:LiteralElement ID="TD1a" runat="server" ElementType="td" OpeningTag="true" ClosingTag="false" />
                <Uc:Date ID="FromDate" runat="server" />
                <Uc:LiteralElement ID="TD1b" runat="server" ElementType="td" OpeningTag="false" ClosingTag="true" />
                <Uc:LiteralElement ID="TD2a" runat="server" ElementType="td" OpeningTag="true" ClosingTag="false" />
                <asp:Label ID="Label2" runat="server" Text="&nbsp;To&nbsp;"></asp:Label>
                <Uc:LiteralElement ID="TD2b" runat="server" ElementType="td" OpeningTag="false" ClosingTag="true" />
<%--                    <asp:TextBox ID="TxtFromDate" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="FromDateREV" Display="None" runat="server" ControlToValidate="TxtFromDate"
                        ErrorMessage="<b>Invalid date</b><br />Enter date in mm/dd/yyyy format." ValidationExpression="(0[1-9]|[1-9]|1[012])[- /.](0[1-9]|[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"></asp:RegularExpressionValidator>
                    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" TargetControlID="FromDateREV"
                        runat="server">
                    </Ajax:ValidatorCalloutExtender>
                    <asp:ImageButton runat="Server" ID="Image1" ImageUrl="~/images/Calendar_scheduleHS.png"
                        AlternateText="Click to show calendar" CausesValidation="False" /><br />
                    <Ajax:CalendarExtender ID="Cal2" runat="Server" TargetControlID="TxtFromDate" PopupButtonID="Image1" />
--%>
                <td>
                    <Uc:Date ID="ToDate" runat="server" />
                    <%--                    <asp:TextBox ID="TxtToDate" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="ToDateREV" Display="None" runat="server" ControlToValidate="TxtToDate"
                        ErrorMessage="<b>Invalid field value</b><br />Enter date in mm/dd/yyyy format."
                        ValidationExpression="([1-9]|1[012])[- /.]([1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"></asp:RegularExpressionValidator>
                    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" TargetControlID="ToDateREV"
                        runat="server">
                    </Ajax:ValidatorCalloutExtender>
                    <asp:ImageButton runat="Server" ID="Image2" ImageUrl="~/images/Calendar_scheduleHS.png"
                        AlternateText="Click to show calendar" CausesValidation="False" /><br />
                    <Ajax:CalendarExtender ID="Cal1" runat="Server" TargetControlID="TxtToDate" PopupButtonID="Image2" />
 --%>
                </td>
            </tr>
<%--            <tr>
               
            </tr>--%>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
