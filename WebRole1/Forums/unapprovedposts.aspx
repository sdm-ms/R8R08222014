<%@ Page Title="" Language="C#" MasterPageFile="AspNetForumMaster.Master" AutoEventWireup="true" CodeBehind="unapprovedposts.aspx.cs" Inherits="aspnetforum.unapprovedposts" %>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">
    <div class="location">
        <img alt="" src="images/admin.gif" />
		<b><a href="moderator.aspx">Moderator</a>
		&raquo;
		List of new postings that have to be approved</b>
	</div>
	
	<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" OnItemCommand="rptMessagesList_ItemCommand">
	    <HeaderTemplate>
	        <table width="100%" cellpadding="5">
	    </HeaderTemplate>
		<ItemTemplate>
			<tr>
			    <th style="width:120px"></th>
				<th>
					<div style="float:left"><%# Eval("CreationDate") %></div>
				    <div style="float:right">
					    <asp:LinkButton id="btnModeratorApprove" Runat="server" CommandName="approve" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnModeratorApproveResource1">&bull; <b>approve</b></asp:LinkButton>
					    <asp:LinkButton id="btnModeratorDelete" Runat="server" CommandName="delete" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnModeratorDeleteResource1">&bull; delete</asp:LinkButton>
				    </div>
				</th>
			</tr>
			<tr valign="top">
				<td>
					topic: <a href='<%# aspnetforum.Utils.Various.GetTopicURL(Eval("TopicID"), Eval("Subject")) %>'>
						<b><%# Eval("Subject") %></b></a>
					<br/><br/>
					<%# aspnetforum.Utils.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), Eval("PostsCount"), Eval("AvatarFileName"), Eval("FirstName"), Eval("LastName"))%>
				</td>
				<td>
					<%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString())%>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
		    <tr>
			<th colspan="2">
				<asp:Label ID="lblPages" runat="server" EnableViewState="False" meta:resourcekey="lblPagesResource1">pages:</asp:Label>
				<%# pagerString %>
			</th></tr>
		    </table>
		</FooterTemplate>
	</asp:repeater>
	
	<div id="divNothingFound" runat="server" visible="false" enableviewstate="false">
	No messages.
	</div>
	
	<div class="location">
	    <hr />
	    <img alt="" src="images/admin.gif" />
		<b><a href="moderator.aspx">moderator</a>
		&raquo; List of new postings that have to be approved</b>
	</div>
</asp:Content>
