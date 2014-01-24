<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.forums" MasterPageFile="AspNetForumMaster.Master" %>
<%@ Register TagPrefix="cc" TagName="RecentPosts" Src="recentposts.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHEAD" ID="AspNetForumHead" runat="server">
<link rel="alternate" type="application/rss+xml" title="recent posts" href="recent.aspx?rss=1" />
</asp:Content>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
	<div class="location">
		<a href="recent.aspx?rss=1" runat="server" id="rssLink" enableviewstate="false"><img alt="recent posts from all forums - RSS" src="images/rss20.gif" style="float:right" /></a>
	</div>
	<asp:Repeater ID="rptGroupsList" Runat="server" EnableViewState="False" OnItemDataBound="rptGroupsList_ItemDataBound">
		<HeaderTemplate>
			<table style="width:100%;" cellspacing="0" cellpadding="11">
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td style="background-color:#ebebeb;border:none;padding-top:15px" colspan="4"><h2><%# Eval("GroupName") %></h2></td>
			</tr>
			<asp:repeater id="rptForumsList" runat="server" EnableViewState="False">
				<ItemTemplate>
					<tr>
						<td align="center"><img alt="" src="<%# GetForumIcon(Eval("IconFile")) %>" height="32" width="32" /></td>
						<td style="width:70%;"><h2>
						    <a href='<%# aspnetforum.Utils.Various.GetForumURL(Eval("ForumID"), Eval("Title")) %>'><%# Eval("Title") %></a>
							</h2>
							<%# Eval("Description") %>
						</td>
						<td><asp:Label ID="lblThreads" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblThreadsResource1">Threads</asp:Label>
						    <br />
						    <%# aspnetforum.Utils.Various.GetForumTopicCount(Convert.ToInt32(Eval("ForumID")), Cmd) %></td>
						<td style="white-space:nowrap"><asp:Label ID="lblLatestPost" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblLatestPostResource1">Latest post</asp:Label>
						    <br />
						    <%# aspnetforum.Utils.Message.GetMsgInfoByID(Eval("LatestMessageID"), Cmd)%></td>
					</tr>
				</ItemTemplate>
			</asp:repeater>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:Repeater>
	<asp:Label ID="lblNoForums" runat="server" Visible="False" enableviewstate="False" meta:resourcekey="lblNoForumsResource1">No forums have been created yet. Contact the forum administrator.</asp:Label>
	
	<br /><br />

	<table style="width:100%;" cellpadding="11" cellspacing="0">
	<tr>
	    <td colspan="2" style="background-color:#ebebeb;border:none;"><h2><asp:Label ID="lblWhatsGoingOn" runat="server" meta:resourcekey="lblWhatsGoingOnResource1" enableviewstate="False">What's going on</asp:Label></h2></td>
	</tr>
	<tr>
	    <td>
	        <asp:Label CssClass="gray" ID="lblUsersOnline" runat="server" meta:resourcekey="lblUsersOnlineResource1" enableviewstate="False">Users online</asp:Label>
	        <%= aspnetforum.Utils.User.OnlineUsersCount %>&nbsp;&nbsp;
	        <asp:Label CssClass="gray" ID="lblMembersOnline" runat="server" meta:resourcekey="lblMembersResource1" enableviewstate="False">Members</asp:Label>
	        <%= aspnetforum.Utils.User.OnlineRegisteredUsersCount %>&nbsp;&nbsp;
	        <asp:Label CssClass="gray" ID="lblGuestsOnline" runat="server" meta:resourcekey="lblGuestsOnlineResource1" enableviewstate="False">Guests</asp:Label>
	        <%= aspnetforum.Utils.User.OnlineUsersCount-aspnetforum.Utils.User.OnlineRegisteredUsersCount%>
            <br /><br />
	        <asp:Label CssClass="gray" ID="lblThreads" runat="server" meta:resourcekey="lblThreadsResource1" enableviewstate="False">Threads</asp:Label>
	        <%= aspnetforum.Utils.Various.GetStats().ThreadCount %>&nbsp;&nbsp;
	        <asp:Label CssClass="gray" ID="lblPosts" runat="server" meta:resourcekey="lblPostsResource1" enableviewstate="False">Posts</asp:Label>
	        <%= aspnetforum.Utils.Various.GetStats().PostCount %>&nbsp;&nbsp;
	        <asp:Label CssClass="gray" ID="lblMembers" runat="server" meta:resourcekey="lblMembersResource1" enableviewstate="False">Members</asp:Label>
	        <%= aspnetforum.Utils.Various.GetStats().MemberCount %>
	    </td>
	</tr>
	<tr id="trRecentHeader" runat="server" enableviewstate="false">
	    <td style="background-color:#ebebeb;border:none;"><br /><br /><h2><asp:Label ID="lblRecentPosts" runat="server" meta:resourcekey="lblRecentPostsResource1" enableviewstate="False">Recent posts</asp:Label></h2></td>
	</tr>
	<tr id="trRecent" runat="server" enableviewstate="false">
	    <td style="border:none;padding:0px 0px 0px 0px"><cc:RecentPosts id="recentPosts" runat="server"></cc:RecentPosts></td>
	</tr>
	</table>
</asp:Content>