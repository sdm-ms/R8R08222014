<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Guarantees.ascx.cs" Inherits="WebRole1.Admin.Guarantees" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="Ajax"%>

<table class="border">

<tr>
    <td>
    Dollar Value Per Point
    </td>
   <td>
    <asp:TextBox ID="dollarValuePerPoint" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Discount For Guarantees (1 = no discount)
    </td>
   <td>
    <asp:TextBox ID="discountForGuarantees" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Total Unconditional Guarantees Earned Ever
    </td>
   <td>
    <asp:Label ID="totalUnconditionalGuaranteesEarnedEver" runat="server"></asp:Label>
   </td>
</tr>
  <tr>
    <td>
    Total Conditional Guarantees Earned Ever
    </td>
   <td>
    <asp:Label ID="totalConditionalGuaranteesEarnedEver" runat="server"></asp:Label>
   </td>
</tr>
  <tr>
    <td>
    Total Conditional Guarantees Pending
    </td>
   <td>
    <asp:Label ID="totalConditionalGuaranteesPending" runat="server"></asp:Label>
   </td>
</tr>
  <tr>
    <td>
    Maximum Total Guarantees
    </td>
   <td>
    <asp:TextBox ID="maximumTotalGuarantees" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Maximum Guarantee Payment Per Hour
    </td>
   <td>
    <asp:TextBox ID="maximumGuaranteePaymentPerHour" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Allow Applications When No Conditional Guarantees Available
    </td>
   <td>
    <asp:TextBox ID="allowApplicationsWhenNoConditionalGuaranteesAvailable" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Conditional Guarantees Available For New Users
    </td>
   <td>
    <asp:TextBox ID="conditionalGuaranteesAvailableForNewUsers" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Conditional Guarantees Available For Existing Users
    </td>
   <td>
    <asp:TextBox ID="conditionalGuaranteesAvailableForExistingUsers" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Conditional Guarantees Time Block In Hours
    </td>
   <td>
    <asp:TextBox ID="conditionalGuaranteeTimeBlockInHours" runat="server"></asp:TextBox>
   </td>
</tr>

  <tr>
    <td>
    <asp:Button ID="SetBtn" runat="server" Text="Set" OnClick="ChangeSettings" 
            CausesValidation="False" />
    </td>
</tr>
 </table>