<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_DollarSubsidy_SetDollarSubsidy" Codebehind="SetDollarSubsidy.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="UC" TagName="Calender" Src=  "~/CommonControl/Date.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src= "~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Buttons" Src= "~/CommonControl/PageButtons.ascx" %>
 <%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src=  "~/CommonControl/ModalPopUp.ascx" %>


 <asp:Panel ID="DollarSubsidyPopupPanel" runat="server" Style="display: none;" Width="358px" Height="450px" CssClass="modalPopup">
 <table  border="0" cellspacing="0" cellpadding="0">

  <tr>
   
    <td valign="top" ><table  border="0" cellspacing="0" cellpadding="0">
     
    <tr>
        <td valign="top" class="headTxt">Set Dollar Subsidy</td>
      </tr>
      
     
      <tr>
        <td valign="top">&nbsp;</td>
      </tr>
     
      <tr>
        <td valign="top" > 
       <table  cellpadding="1" cellspacing="1" class="border">
   
    
    <tr>
    <td >Current Period Doller Subsidy</td>
    <td>
        <asp:TextBox ID="TxtCurrentPeriodDollersubsidy" runat="server" CssClass="inp" MaxLength ="21"></asp:TextBox>
        <Uc:Hover runat="server" ID="Hover1" MsgString="Enter Current Period Doller Subsidy" />
        <asp:RegularExpressionValidator ID="CurrentRegularExpressionValidator" 
        Display="None" ControlToValidate="TxtCurrentPeriodDollersubsidy" runat="server" 
        ErrorMessage="<b>Invalid Field value</b><br />Enter a numeric value only upto four decimal places." 
        ValidationExpression="\d{0,18}(\.\d{1,4})?"></asp:RegularExpressionValidator>
    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server" TargetControlID="CurrentRegularExpressionValidator"></Ajax:ValidatorCalloutExtender>
                    </td>
    </tr>
    <tr>
    <td >End Of Doller Subsidy Period</td>
    <td>
  
    <UC:Calender id="TxtDateTime" runat="server" Msgstring="Click here to enter End Of Doller Subsidy Period "></UC:Calender>
        
                    </td>
    </tr>
    <tr>
    <td >Next Period Doller Dubsidy</td>
    <td>
        <asp:TextBox ID="TxtNextPeriodDollerSubsidy" runat="server" CssClass="inp"></asp:TextBox>
        <Uc:Hover runat="server" ID="Hover3" MsgString="Enter Next Period Doller Dubsidy" />
        <asp:RegularExpressionValidator ID="NextRegularExpressionValidator" 
        Display="None" ControlToValidate="TxtNextPeriodDollerSubsidy" runat="server" 
        ErrorMessage="<b>Invalid Field value</b><br />Enter a numeric value only upto four decimal places." 
        ValidationExpression="\d{0,18}(\.\d{1,4})?"></asp:RegularExpressionValidator>
    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" runat="server" TargetControlID="NextRegularExpressionValidator"></Ajax:ValidatorCalloutExtender>
                    </td>
    </tr>
    <tr>
    <td>Next Period Length</td>
    <td>
        <asp:TextBox ID="TxtNextPeriodLength" runat="server" CssClass="inp" MaxLength ="13"></asp:TextBox>
        <Uc:Hover runat="server" ID="Hover4" MsgString="Enter Next Period Length" />
        <asp:RegularExpressionValidator ID="PeriodRegularExpressionValidator" 
        Display="None" ControlToValidate="TxtNextPeriodLength" runat="server" 
        ErrorMessage="<b>Invalid Field value</b><br />Enter a integer value ." 
        ValidationExpression="\d+"></asp:RegularExpressionValidator>
    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server" TargetControlID="PeriodRegularExpressionValidator"></Ajax:ValidatorCalloutExtender>
                    </td>
    </tr>
  
    
    <tr>
    <td>
    <asp:RadioButton runat="server" Checked="true" Text="Grant subsidy to all users"
        ID="RBtnGrantSubsidyToAllUser" AutoPostBack="True" OnCheckedChanged="RBtnGrantSubsidyToAllUser_CheckedChanged" />
    </td>
    </tr>
    <tr>
    <td>Minimum payments to qualify</td>
    <td>
        <asp:TextBox ID="TxtMinPointsToQualify" runat="server" CssClass="inp" MaxLength ="21"></asp:TextBox>
        <Uc:Hover runat="server" ID="Hover2" MsgString="Enter Minimum Payment" />
        <asp:RegularExpressionValidator ID="MRegularExpressionValidator" 
        Display="None" ControlToValidate="TxtMinPointsToQualify" runat="server" 
        ErrorMessage="<b>Invalid Field value</b><br />Enter a numeric value only upto four decimal places." 
        ValidationExpression="\d{0,18}(\.\d{1,4})?"></asp:RegularExpressionValidator>
    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" TargetControlID="MRegularExpressionValidator"></Ajax:ValidatorCalloutExtender>
    </td>
    </tr>
    <tr>
    <td>
    <asp:RadioButton runat="server" Text="Grant subsidy to users picked at random" 
            ID="RBtnGrantSubsidyAtRandom" AutoPostBack="true"
           oncheckedchanged="RBtnGrantSubsidyAtRandom_CheckedChanged" />
    </td>
    </tr>
    <tr >
    <td>Minimum points to be eligible</td>
    <td>
        <asp:TextBox ID="TxtMinPointsToEligible" runat="server" CssClass="inp" MaxLength ="23"></asp:TextBox>
          <Uc:Hover runat="server" ID="Hover6" MsgString="Enter Minimum Points" />
        <asp:RegularExpressionValidator ID="MinRegularExpressionValidator" 
        Display="None" ControlToValidate="TxtMinPointsToEligible" runat="server" 
        ErrorMessage="<b>Invalid Field value</b><br />Enter a numeric value only upto four decimal places." 
        ValidationExpression="\d{0,18}(\.\d{1,4})?"></asp:RegularExpressionValidator>
    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender5" runat="server" TargetControlID="MinRegularExpressionValidator"></Ajax:ValidatorCalloutExtender>
    </td>
    </tr>
    <tr>
    <td>Number of users</td>
    <td>
        <asp:TextBox ID="TxtNumOfUsers" runat="server" CssClass="inp" MaxLength ="4"></asp:TextBox>
        <Uc:Hover runat="server" ID="Hover5" MsgString="Enter number of users" />
        <asp:RangeValidator ID="NumUserRV" Display="None" MinimumValue="1"  MaximumValue="32767" Type="Integer" runat="server" ErrorMessage="<b>Invalid Field value</b><br />Enter a integer value between 1 and 32767." ControlToValidate="TxtNumOfUsers"></asp:RangeValidator>
         
    <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender6" runat="server" TargetControlID="NumUserRV"></Ajax:ValidatorCalloutExtender>
    </td>
    </tr>
     
     <tr>
    <td colspan="2" align="center">
    
         <Uc:Buttons runat="server" ID="Buttons" />
     
         </td>
    
    </tr>
        
    </table>
   
       
      </td>
      </tr>
      
     
    </table>
    </td>
    
  </tr>
</table>
</asp:Panel>
<asp:LinkButton ID="EmptyButton" runat="server" Text="" Enabled="false" Visible="true"></asp:LinkButton>
<Ajax:ModalPopupExtender ID="DollarSubsidyExtender" runat="server" TargetControlID="EmptyButton"
    PopupControlID="DollarSubsidyPopupPanel" DropShadow="true" 
    BackgroundCssClass="modalBackground">
</Ajax:ModalPopupExtender>