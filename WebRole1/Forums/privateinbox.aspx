<%@ Page language="c#" Codebehind="privateinbox.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.privateinbox" MasterPageFile="AspNetForumMaster.Master" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
<div class="location">
	<h2><a href="default.aspx"><asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
	&raquo;
	<a href="editprofile.aspx"><asp:Label ID="lblProfile" runat="server" EnableViewState="False" meta:resourcekey="lblProfileResource1">Profile</asp:Label></a>
	&raquo;
	<asp:Label ID="lblInbox" runat="server" EnableViewState="False" meta:resourcekey="lblInboxResource1">Personal messages - Inbox</asp:Label></h2></div>
<asp:Label ID="lblNotLoggedIn" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" meta:resourcekey="lblNotLoggedInResource1">You are not signed in as a member. Please sign in to access your private messages.</asp:Label>
<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" 
    OnItemCommand="rptMessagesList_ItemCommand" 
    onitemdatabound="rptMessagesList_ItemDataBound">
    <HeaderTemplate><table width="100%" cellpadding="19"></HeaderTemplate>
	<ItemTemplate>
		<tr valign="top">
			<td style="width:120px;" rowspan="2">
			    <span id="span1" runat="server" style="color:#FFA767" visible='<%# Convert.ToBoolean(Eval("New")) %>'>NEW</span>
			    <span class="gray"><%# Eval("CreationDate") %></span>
			    <br /><br />
				<%# aspnetforum.Utils.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), null, Eval("AvatarFileName"), Eval("FirstName"), Eval("LastName"))%>
			</td>
			<td style="border-bottom:none">
			    <%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString())%>
			    <%# aspnetforum.Utils.Formatting.FormatSignature(Eval("Signature").ToString())%>
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
		<tr><td class="gray" style="border-top:none" align="right" valign="bottom">
		    <a href='addprivatemsg.aspx?ToUserID=<%# Eval("UserID") %>&amp;Quote=<%# Eval("MessageID") %>'>
		    <asp:Label ID="lblReply" runat="server" EnableViewState="False" meta:resourcekey="lblReplyResource1">reply</asp:Label></a>
		    &bull;
		    <asp:LinkButton id="btnDelete" runat="server" OnClientClick="if(!confirm('Are you sure?')) return false;" CommandName="delete" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnDeleteResource1">delete</asp:LinkButton>
		</td></tr>
	</ItemTemplate>
	<FooterTemplate>
	    <tr><th colspan="2">
	    <asp:Label ID="lblPager" runat="server" EnableViewState="False" meta:resourcekey="lblPagerResource1">pages:</asp:Label>
	    <%# pagerString %></th></tr></table>
	</FooterTemplate>
</asp:repeater>
</asp:Content>