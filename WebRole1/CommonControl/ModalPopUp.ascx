<%@ Control Language="C#" AutoEventWireup="true" Inherits="User_Control_ModalPopUp" Codebehind="ModalPopUp.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="Ajax"%>
 <asp:Panel ID="PanelPopUp" runat="server"  Style="display: none" CssClass="modalPopup" > 
              <asp:Label ID="LblMsg" runat="server"></asp:Label> 
              <asp:Button ID="BtnOk" runat="server" CausesValidation="false" Text="OK" CssClass="Btn1" />
              <asp:HiddenField ID="Hidden" runat="server" />   
          </asp:Panel>    
   <Ajax:ModalPopupExtender ID="MpeMsg" runat="server" 
    OkControlID="BtnOk" BackgroundCssClass="modalBackground" 
    PopupControlID="PanelPopUp" 
    TargetControlID="Hidden" DynamicServicePath="~/WebServiceTool.asmx" 
    DynamicServiceMethod="GetContextKey" 
    DynamicControlID="LblMsg">
</Ajax:ModalPopupExtender>
           