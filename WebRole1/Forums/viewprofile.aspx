<%@ Page language="c#" Codebehind="viewprofile.aspx.cs" EnableViewState="false" AutoEventWireup="True" Inherits="aspnetforum.viewprofile" MasterPageFile="aspnetforummaster.master" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
	<div class="location">
	<h2><asp:Label id="lblUser" runat="server" meta:resourcekey="lblUserResource1"></asp:Label>
		-
		<asp:Label ID="lblProfile" runat="server" meta:resourcekey="lblProfileResource1">profile</asp:Label></h2>
	</div>
	<table cellpadding="14" class="noborder" width="400px">
		<tr>
		    <td rowspan="7" valign="top">
				<img id="imgAvatar" runat="server" class="avatar" src="images/guestavatar.gif" alt="User avatar" />
		    </td>
			<td><asp:Label ID="lblUsernameTitle" CssClass="gray" runat="server" meta:resourcekey="lblUsernameTitleResource1">UserName:</asp:Label></td>
			<td>
			    <asp:Label id="lblUserName" runat="server"></asp:Label>
			    <asp:Label id="lblFullName" runat="server"></asp:Label>
			</td>
		</tr>
		<tr>
			<td><asp:Label ID="lblTotalPosts" CssClass="gray" runat="server" meta:resourcekey="lblTotalPostsResource1">Total posts:</asp:Label></td>
			<td><a id="lnkViewPosts" runat="server"></a></td>
		</tr>
		<tr id="trRating" runat="server">
			<td><asp:Label ID="lblRating" CssClass="gray" runat="server" meta:resourcekey="lblRatingResource1">Reputation:</asp:Label></td>
			<td><asp:Label ID="lblRatingValue" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td><asp:Label ID="lblReggedSince" CssClass="gray" runat="server" meta:resourcekey="lblReggedSinceResource1">Registered since:</asp:Label></td>
			<td><asp:Label id="lblRegistrationDate" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td><asp:Label ID="lblLastLogonDate" CssClass="gray" runat="server" meta:resourcekey="lblLastLogonDateResource1">Last logon date:</asp:Label></td>
			<td><asp:Label id="lblLastLogonDateValue" runat="server"></asp:Label></td>
		</tr>
        <tr>
			<td><asp:Label ID="lblInterestsTitle" CssClass="gray" runat="server" meta:resourcekey="lblInterestsTitleResource1">Interests:</asp:Label></td>
			<td><asp:Label id="lblInterests" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td><asp:Label ID="lblHomepage" CssClass="gray" runat="server" meta:resourcekey="lblHomepageResource1">Homepage:</asp:Label></td>
			<td><asp:HyperLink id="homepage" Target="_blank" runat="server" rel="nofollow"></asp:HyperLink></td>
		</tr>
	</table>
	<p><a id="lnkPersonalMsg" runat="server"><asp:Label ID="Label5" runat="server" meta:resourcekey="Label5Resource1">Send a private message</asp:Label></a></p>
	<div id="divGroups" runat="server">
    <span class="gray">Member of groups:</span>
    <asp:GridView CellPadding="7" id="gridGroups" Runat="server" Width="100%" AutoGenerateColumns="False"
		ShowHeader="False" BorderColor="Silver">
		<AlternatingRowStyle BackColor="Lavender" />
		<Columns>
			<asp:HyperLinkField DataTextField="Title" DataNavigateUrlFields="GroupID" DataNavigateUrlFormatString="editusergroup.aspx?GroupID={0}"></asp:HyperLinkField>
		</Columns>
    </asp:GridView>
    </div>
    <p><asp:Button Width="400" id="btnEditUser" runat="server" Text="edit..." meta:resourcekey="btnEditUserResource1"></asp:Button></p>
	<p><asp:Button Width="400" id="btnDelUser" runat="server" Text="delete this user (visible to administrators only)" onclick="btnDelUser_Click" meta:resourcekey="btnDelUserResource1"></asp:Button></p>
	<p><asp:Button Width="400" id="btnActivateUser" runat="server" Text="activate this user (visible to administrators only)" meta:resourcekey="btnActivateUserResource1" OnClick="btnActivateUser_Click"></asp:Button></p>
	<p><asp:Button Width="400" id="btnDisableUser" runat="server" Text="disable this user (visible to administrators only)" meta:resourcekey="btnDisableUserResource1" OnClick="btnDisableUser_Click"></asp:Button></p>
	<p><asp:Button Width="400" id="btnMakeAdmin" runat="server" Text="grant Administrator permissions (visible to administrators only)" meta:resourcekey="btnMakeAdminResource1" OnClick="btnMakeAdmin_Click"></asp:Button></p>
	<p><asp:Button Width="400" id="btnRevokeAdmin" runat="server" Text="revoke Administrator permissions (visible to administrators only)" meta:resourcekey="btnRevokeAdminResource1" OnClick="btnRevokeAdmin_Click"></asp:Button></p>
</asp:Content>