<%@ Page language="c#" Codebehind="privatesent.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.privatesent" MasterPageFile="AspNetForumMaster.Master" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
<div class="location">
	<h2><a href="default.aspx"><asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
	&raquo;
	<a href="editprofile.aspx"><asp:Label ID="lblProfile" runat="server" EnableViewState="False" meta:resourcekey="lblProfileResource1">Profile</asp:Label></a>
	&raquo;
	<asp:Label ID="lblSent" runat="server" EnableViewState="False" meta:resourcekey="lblSentResource1">Personal messages - Sent Items</asp:Label></h2></div>

<asp:Label ID="lblNotLoggedIn" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" meta:resourcekey="lblNotLoggedInResource1">You are not signed in as a member. Please sign in to access your private messages.</asp:Label>
<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" 
onitemdatabound="rptMessagesList_ItemDataBound">
    <HeaderTemplate><table width="100%" cellpadding="19"></HeaderTemplate>
	<ItemTemplate>
		<tr valign="top">
			<td style="width:120px;">
			    <span class="gray"><%# Eval("CreationDate") %></span>
			    <br /><br />
				<%# aspnetforum.Utils.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), null, Eval("AvatarFileName"), Eval("FirstName"), Eval("LastName"))%>
			</td>
			<td>
				<%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString())%>
				<asp:Repeater ID="rptFiles" runat="server">
			    <HeaderTemplate>
					<br /><br />
					<div class="gray">
					<asp:Label ID="lblAttachments" runat="server" EnableViewState="False" meta:resourcekey="lblAttachmentsResource1">Attachments:</asp:Label><br />
			    </HeaderTemplate>
			    <ItemTemplate>
			        <a href='getattachment.ashx?fileid=<%# Eval("FileID") %>'>
			        <%# aspnetforum.Utils.Attachments.GetThumbnail(Eval("FileName").ToString(), Convert.ToInt32(Eval("UserID"))) %>
			        <%# Eval("FileName") %></a><br />
			    </ItemTemplate>
			    <FooterTemplate></div></FooterTemplate>
			    </asp:Repeater>
			</td>
		</tr>
	</ItemTemplate>
	<FooterTemplate>
	    <tr><th colspan="2">
	    <asp:Label ID="lblPager" runat="server" EnableViewState="False" meta:resourcekey="lblPagerResource1">pages:</asp:Label>
	    <%# pagerString %></th></tr>
</table>
	</FooterTemplate>
</asp:repeater>
</asp:Content>