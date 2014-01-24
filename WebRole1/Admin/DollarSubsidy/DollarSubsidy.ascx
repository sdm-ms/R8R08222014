<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_DollarSubsidy_DollarSubsidy" Codebehind="DollarSubsidy.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx"%>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="Ajax"%>
<%@ Register TagPrefix="Uc" TagName="Dollarsubs" Src="~/Admin/DollarSubsidy/SetDollarSubsidy.ascx"%>
 <asp:PlaceHolder ID="DollarPlaceHolder" runat="server">

<table class="border">

  <tr><td>
  
  <asp:LinkButton ID="LinkDollar" runat="server" CausesValidation="False" 
            CssClass="hyperTxt" OnClick="LinkDollar_Click">Set Dollar Subsidy</asp:LinkButton>
            <asp:PlaceHolder ID="DollsubsPopupHolder" runat="server">
</asp:PlaceHolder>
     
   </td>
        <td>

            <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel3" runat="server">
                <ProgressTemplate>
                    <asp:Image ID="ImageProgress" runat="server" 
                        ImageUrl=  "~/images/progress_48.gif" Height="23px" Width="27px" />
                </ProgressTemplate>
            </asp:UpdateProgress>  
            </td>
            
   </tr>
 </table>

  </asp:PlaceHolder>  