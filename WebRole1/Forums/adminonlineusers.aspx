<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminonlineusers.aspx.cs" Inherits="aspnetforum.adminonlineusers" MasterPageFile="AspNetForumMaster.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="AspNetForumContentPlaceHolder">

<div class="location"><img alt="" src="images/admin.gif" />&nbsp;
<b><a href="admin.aspx"><asp:Label ID="lblAdmin" runat="server" EnableViewState="False" meta:resourcekey="lblAdminResource1" Text="Administrator"></asp:Label></a>
&raquo;
<asp:Label ID="lblOnlineUsers" runat="server" EnableViewState="False" meta:resourcekey="lblOnlineUsersResource1">Online users</asp:Label></b></div>

<table cellpadding="7">
<asp:Repeater ID="rptUsers" runat="server">
<ItemTemplate>
    <tr>
    <td><%# Eval("UserName")%></td>
    <td><%# Eval("CurrentURL") %></td>
    <td><%# Eval("LastActivity")%></td>
    </tr>
</ItemTemplate>
</asp:Repeater>
</table>

</asp:Content>