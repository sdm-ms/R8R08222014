<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommonControl_PageButtons" Codebehind="PageButtons.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src= "~/CommonControl/ModalPopUp.ascx" %>
<table>
<tr>
<td>
    <asp:Button ID="BtnImplement" runat="server" Text="OK" CssClass="Btn1" CausesValidation="false" Width="70px"
        onclick="BtnImplement_Click" />
</td>
<td>
    <asp:Button ID="BtnCancel" runat="server" Text="Cancel" CssClass="Btn1"  CausesValidation="false" Width="70px"
        onclick="BtnCancel_Click" />
</td>

</tr>

</table>
<Uc:ModalPopUp ID="PopUp" runat="server"></Uc:ModalPopUp>