<%@ Control Language="C#" AutoEventWireup="true" Inherits="User_Control_Hover" Codebehind="Hover.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="Ajax"%>
<asp:Image ID="HoverImage" runat="server"  ImageUrl="~/images/mark.gif" align="absmiddle" Height="20" Width="20"/>
     <asp:Panel ID="PopUp" CssClass="popupMenu" runat="server">
         <asp:Label ID="LblMsg" runat="server"></asp:Label>
      </asp:Panel>
     <Ajax:HoverMenuExtender runat="server" ID="H1" PopupControlID="PopUp" HoverCssClass="popupHover" TargetControlID="HoverImage" PopupPosition="Right" PopDelay="25"></Ajax:HoverMenuExtender>