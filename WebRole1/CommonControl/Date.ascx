<%@ Control Language="C#" AutoEventWireup="true" Inherits="TblRow_Date" Codebehind="Date.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<asp:TextBox ID="TxtDate" runat="server" CssClass="filterTextBoxForDate"></asp:TextBox>
<%--<asp:RegularExpressionValidator ID="DateREV" Display="None" runat="server" ControlToValidate="TxtDate"
    ErrorMessage="<b>Invalid value.</b><br />Enter date in mm/dd/yyyy format." ValidationExpression="(0[1-9]|[1-9]|1[012])[- /.](0[1-9]|[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"></asp:RegularExpressionValidator>
<Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" TargetControlID="DateREV"
    runat="server">
</Ajax:ValidatorCalloutExtender>--%>
<asp:ImageButton runat="Server" ID="Image1" ImageUrl="~/images/Calendar_scheduleHS.png"
    AlternateText="Click to show calendar" CausesValidation="False" />
<Ajax:CalendarExtender ID="Cal" runat="Server" TargetControlID="TxtDate" PopupButtonID="Image1" />
