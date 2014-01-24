<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_ViewCellRatingValueSelected" Codebehind="ViewCellRatingValueSelected.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>

<asp:TextBox ID="TheValue" runat="server" Width="50px" >
</asp:TextBox>
<%--<asp:RegularExpressionValidator ID="ValueValidator" runat="server" Display="None" ControlToValidate="TheValue" ValidationExpression="(^-?([0-9]*(\.[0-9]+)?)$)"
 ErrorMessage="Numeric data required">
</asp:RegularExpressionValidator>

 <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" TargetControlID="ValueValidator"
    runat="server">
  </Ajax:ValidatorCalloutExtender>--%>
                    