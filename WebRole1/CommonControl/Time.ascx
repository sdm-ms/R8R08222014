<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommonControl_Time" Codebehind="Time.ascx.cs" %>
<asp:DropDownList ID="hour" runat="server">
    <asp:ListItem Text="12" Value="0"></asp:ListItem>
    <asp:ListItem Text="1" Value="1"></asp:ListItem>
    <asp:ListItem Text="2" Value="2"></asp:ListItem>
    <asp:ListItem Text="3" Value="3"></asp:ListItem>
    <asp:ListItem Text="4" Value="4"></asp:ListItem>
    <asp:ListItem Text="5" Value="5"></asp:ListItem>
    <asp:ListItem Text="6" Value="6"></asp:ListItem>
    <asp:ListItem Text="7" Value="7"></asp:ListItem>
    <asp:ListItem Text="8" Value="8"></asp:ListItem>
    <asp:ListItem Text="9" Value="9"></asp:ListItem>
    <asp:ListItem Text="10" Value="10"></asp:ListItem>
    <asp:ListItem Text="11" Value="11"></asp:ListItem>
</asp:DropDownList>
<asp:DropDownList ID="minute" runat="server">
    <asp:ListItem Text="00" Value="0"></asp:ListItem>
    <asp:ListItem Text="05" Value="5"></asp:ListItem>
    <asp:ListItem Text="10" Value="10"></asp:ListItem>
    <asp:ListItem Text="15" Value="15"></asp:ListItem>
    <asp:ListItem Text="20" Value="20"></asp:ListItem>
    <asp:ListItem Text="25" Value="25"></asp:ListItem>
    <asp:ListItem Text="30" Value="30"></asp:ListItem>
    <asp:ListItem Text="35" Value="35"></asp:ListItem>
    <asp:ListItem Text="40" Value="40"></asp:ListItem>
    <asp:ListItem Text="45" Value="45"></asp:ListItem>
    <asp:ListItem Text="50" Value="50"></asp:ListItem>
    <asp:ListItem Text="55" Value="55"></asp:ListItem>
</asp:DropDownList>
<asp:DropDownList ID="timeOfDay" runat="server">
    <asp:ListItem Text="AM" Value="0"></asp:ListItem>
    <asp:ListItem Text="" Value="12"></asp:ListItem>
</asp:DropDownList>
