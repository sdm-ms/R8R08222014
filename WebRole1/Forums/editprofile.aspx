<%@ Page language="c#" Codebehind="editprofile.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.editprofile" MasterPageFile="aspNetForumMaster.Master" ValidateRequest="false" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
<div class="location">
    <h2><a href="default.aspx"><asp:Label ID="lblHome" runat="server" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
    &raquo;
    <asp:Label ID="lblProfile" runat="server" meta:resourcekey="lblProfileResource1">Profile</asp:Label></h2>
    <asp:HyperLink ID="lblMySubs" NavigateUrl="mysubscriptions.aspx" runat="server" meta:resourcekey="lblMySubsResource1">My subscriptions</asp:HyperLink> |
    <asp:HyperLink ID="lblInbox" NavigateUrl="privateinbox.aspx" runat="server" meta:resourcekey="lblInboxResource1">Personal messages - Inbox</asp:HyperLink> |
    <asp:HyperLink ID="lblSent" NavigateUrl="privatesent.aspx" runat="server" meta:resourcekey="lblSentResource1">Personal messages - Sent items</asp:HyperLink>
</div>
<asp:Label ID="lblNotLoggedIn" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" meta:resourcekey="lblNotLoggedInResource1">You are not signed in as a member. Please sign in to access your profile.</asp:Label>
<div id="divMain" runat="server">
<div><asp:Label id="lblResult" runat="server" EnableViewState="false" Font-Bold="True" ForeColor="Red" meta:resourcekey="lblResultResource1"></asp:Label></div>

<table cellpadding="9" width="500px" class="noborder">
	<tr>
		<th colspan="2"><asp:Label ID="lblMyProfile" runat="server" meta:resourcekey="lblMyProfileResource1">My Profile</asp:Label></th></tr>
	<tr>
		<td width="50%" align="right" class="gray">*<asp:Label ID="lblUsername" runat="server" meta:resourcekey="lblUsernameResource1">UserName:</asp:Label></td>
		<td><asp:TextBox id="tbUsername" runat="server" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right" class="gray">*<asp:Label ID="lblEmail" runat="server" meta:resourcekey="lblEmailResource1">Email (NOT shared):</asp:Label></td>
		<td><asp:TextBox id="tbEmail" runat="server" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right" class="gray"><asp:Label ID="lblFirstName" runat="server" meta:resourcekey="lblFirstNameResource1">First Name:</asp:Label></td>
		<td><asp:TextBox id="tbFirstName" runat="server" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right" class="gray"><asp:Label ID="lblLastName" runat="server" meta:resourcekey="lblLastNameResource1">Last Name:</asp:Label></td>
		<td><asp:TextBox id="tbLastName" runat="server" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right" class="gray"><asp:Label ID="lblHomepage" runat="server" meta:resourcekey="lblHomepageResource1">Homepage:</asp:Label></td>
		<td><asp:TextBox id="tbHomepage" runat="server" MaxLength="50" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right" class="gray"><asp:Label ID="lblInterests" runat="server" meta:resourcekey="lblInterestsResource1">Interests:</asp:Label></td>
		<td><asp:TextBox id="tbInterests" runat="server" MaxLength="255" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
	    <td align="right" class="gray">
	        <asp:Label ID="lblSignature" runat="server" meta:resourcekey="lblSignatureResource1">Signature:</asp:Label>
	        <br />
	        (No tags. "BBCode" only - [url][/url], [img][/img], [b][/b], [i][/i])
	    </td>
		<td><asp:TextBox id="tbSignature" runat="server" TextMode="MultiLine" Rows="3" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td></td><td>
			<asp:Button CssClass="gradientbutton" id="btnSave" runat="server" Text="save" onclick="btnSave_Click" meta:resourcekey="btnSaveResource1"></asp:Button></td></tr>
</table>
<br />
<table cellpadding="9" id="tblAvatar" runat="server" width="500px" class="noborder">
    <tr><th colspan="2"><asp:Label ID="lblAvatar" runat="server" meta:resourcekey="lblAvatarResource1">Avatar picture</asp:Label></th></tr>
    <tr><td colspan="2"><img id="imgAvatar" class="avatar" runat="server" src="images/guestavatar.gif" alt="" /></td></tr>
    <tr valign="top">
	    <td class="gray">
	        <asp:Label ID="lblAvatar2" runat="server" meta:resourcekey="lblAvatar2Resource1">Avatar picture</asp:Label>
	        <br />
	        (max <asp:Label ID="lblMaxSize" runat="server" meta:resourcekey="lblMaxSizeResource1"></asp:Label> bytes,
	        max <asp:Label ID="lblMaxDimenstions" runat="server" meta:resourcekey="lblMaxDimenstionsResource1"></asp:Label> pixels)<br />
	        <asp:Label ID="lblLeaveBlank" runat="server" meta:resourcekey="lblLeaveBlankResource1">leave blank to remove your current avatar</asp:Label>
	        ):</td>
		<td><asp:FileUpload ID="avatarUpload" runat="server" meta:resourcekey="avatarUploadResource1" /></td>
	</tr>
	<tr>
	    <td colspan="2">
	        <asp:Button CssClass="gradientbutton" ID="btnUpload" runat="server" Text="upload" OnClick="btnUpload_Click" meta:resourcekey="btnUploadResource1" />
	    </td>
	</tr>
</table>
<br />
<table cellpadding="9" width="500px" id="tblChangePsw" runat="server" class="noborder">
	<tr>
		<th colspan="2"><asp:Label ID="lblChangePsw" runat="server" meta:resourcekey="lblChangePswResource1">Change Password</asp:Label></th></tr>
	<tr>
		<td width="50%" align="right" class="gray"><asp:Label ID="lblOldPsw" runat="server" meta:resourcekey="lblOldPswResource1">Old password:</asp:Label></td>
		<td><asp:TextBox id="tbOldPsw" runat="server" TextMode="Password" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right" class="gray"><asp:Label ID="lblNewPsw" runat="server" meta:resourcekey="lblNewPswResource1">New password:</asp:Label></td>
		<td><asp:TextBox id="tbNewPsw1" runat="server" TextMode="Password" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right" class="gray"><asp:Label ID="lblConfPsw" runat="server" meta:resourcekey="lblConfPswResource1">Confirm new password:</asp:Label></td>
		<td><asp:TextBox id="tbNewPsw2" runat="server" TextMode="Password" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td></td><td>
			<asp:Button CssClass="gradientbutton" id="btnChangePsw" runat="server" Text="change" onclick="btnChangePsw_Click" meta:resourcekey="btnChangePswResource1"></asp:Button>
		</td></tr>
</table>
</div>
</asp:Content>