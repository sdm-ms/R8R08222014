<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RewardRatingSettingScreen.ascx.cs" Inherits="WebRole1.Admin.RewardRatingSettingScreen" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="Ajax"%>

<table class="border">

<tr>
    <td>
    Probability of Reward for Database Changes
    </td>
   <td>
    <asp:TextBox ID="prob" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Multiplier for Database Changes
    </td>
   <td>
    <asp:TextBox ID="mult" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Probability of High Stakes for Rating
    </td>
   <td>
    <asp:TextBox ID="HighStakesProbability" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Multiplier for High Stakes When Already Known
    </td>
   <td>
    <asp:TextBox ID="HighStakesKnownMultiplier" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Multiplier for High Stakes Randomly Selected
    </td>
   <td>
    <asp:TextBox ID="HighStakesSecretMultiplier" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Increased high stakes probability for novices
    </td>
   <td>
    <asp:TextBox ID="HighStakesNoviceOn" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Number of first ratings automatically set to high stakes for novices
    </td>
   <td>
    <asp:TextBox ID="HighStakesNoviceNumAutomatic" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Number of next ratings automatically set to 1/3 probability of high stakes
    </td>
   <td>
    <asp:TextBox ID="HighStakesNoviceNumOneThird" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Number of next ratings automatically set to 1/10 probability of high stakes
    </td>
   <td>
    <asp:TextBox ID="HighStakesNoviceNumOneTenth" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Target number of high stakes rows (if greater, numbers above will be automatically reduced)
    </td>
   <td>
    <asp:TextBox ID="HighStakesNoviceTargetNum" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    Multiply numbers above by this percent for probability of automatically selecting database changes
    </td>
   <td>
    <asp:TextBox ID="DatabaseChangeSelectHighStakesNoviceNumPct" runat="server"></asp:TextBox>
   </td>
</tr>
  <tr>
    <td>
    <asp:Button ID="SetBtn" runat="server" Text="Set" OnClick="ChangeSettings" 
            CausesValidation="False" />
    </td>
</tr>
 </table>
