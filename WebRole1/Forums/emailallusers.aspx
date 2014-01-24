<%@ Page Title="" Language="C#" MasterPageFile="AspNetForumMaster.Master" AutoEventWireup="true" CodeBehind="emailallusers.aspx.cs" Inherits="aspnetforum.emailallusers" %>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<div class="location"><img alt="" src="images/admin.gif" />&nbsp;.
	<b><a href="admin.aspx"><asp:Label ID="lblAdmin" runat="server" EnableViewState="False" meta:resourcekey="lblAdminResource1" Text="Administrator"></asp:Label></a>
	&raquo;
	<asp:Label ID="lblEmailAllUsers" runat="server" EnableViewState="False" meta:resourcekey="lblEmailAllUsersResource1">Email all users</asp:Label></b></div>

    <table cellpadding="7" class="noborder" width="450">
    <tr><th colspan="2"><asp:Label ID="lblEmailAllUsers2" runat="server" EnableViewState="False" meta:resourcekey="lblEmailAllUsersResource1">Email all users</asp:Label></th></tr>
    <tr>
        <td class="gray">Subject *</td>
        <td><asp:textbox runat="server" ID="tbSubj" Text="Subject" Width="100%"></asp:textbox></td>
    </tr>
    <tr valign="top">
        <td class="gray">Body *</td>
        <td><asp:TextBox runat="server" ID="tbBody" TextMode="MultiLine" Rows="10" Text="Dear forum user!

We have some news for you.

Please visit our forum http://www.server.com/forum for more info" Width="100%"></asp:TextBox></td>
    </tr>
    <tr><td colspan="2"><asp:Button ID="btnSend" runat="server" onclick="btnSend_Click" CssClass="gradientbutton" Text="send" /></td></tr>
    </table>
</asp:Content>
