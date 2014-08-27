<%@ Page language="c#" Codebehind="topics.aspx.cs" EnableViewState="false" AutoEventWireup="True" Inherits="aspnetforum.topics" MasterPageFile="AspNetForumMaster.Master" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHEAD" ID="AspNetForumHead" runat="server">
<link rel="alternate" type="application/rss+xml" title="topics in this forum" id="rssDiscoverLink" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">

<div class="location">
    <a runat="server" id="rssLink" enableviewstate="false"><img alt="topics in this forum - RSS" src="images/rss20.gif" style="float:right" /></a>
	<h2><a href="default.aspx"><asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
	&raquo;
	<asp:Label id="lblCurForum" runat="server" EnableViewState="False"></asp:Label>
	</h2>
</div>

<div class="gray" id="divDescription" runat="server"></div>

<asp:repeater id="rptSubForumsList" runat="server" EnableViewState="False">
	<HeaderTemplate>
	    <h2><asp:Label ID="lblSubForums" runat="server" EnableViewState="False" meta:resourcekey="lblSubForumsResource1">Sub-Forums</asp:Label></h2>
		<table width="100%" cellpadding="5">
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td align="center"><img alt="" src="images/forum.gif" /></td>
			<td width="70%">
			    <h2><a href='<%# aspnetforum.Utils.Various.GetForumURL(Eval("ForumID"), Eval("Title")) %>'><%# Eval("Title") %></a></h2>
				<%# Eval("Description") %>
			</td>
			<td><asp:Label ID="lblThreads" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblThreadsResource1">Threads</asp:Label>
			    <br />
			    <%# Eval("Topics") %></td>
			<td style="white-space:nowrap"><asp:Label ID="lblLatestPost" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblLatestPostResource1">Latest post</asp:Label>
			    <br />
			    <%# aspnetforum.Utils.Message.GetMsgInfoByID(Eval("LatestMessageID"), this.Cmd) %></td>
		</tr>
	</ItemTemplate>
	<FooterTemplate>
		</table>
		<br />
	</FooterTemplate>
</asp:repeater>

<asp:Label ID="lblDenied" Runat="server" ForeColor="Red" Font-Bold="True" Visible="False" EnableViewState="False" meta:resourcekey="lblDeniedResource1">Access to this forum is restricted</asp:Label>
<div id="divMain" runat="server" enableviewstate="false">
	<div class="smalltoolbar">
	    <%= pagerString %>
        <b><a id="linkAddTopic2" visible="false" runat="server" enableviewstate="false">
		    <asp:Label ID="lblNewTopic2" runat="server" EnableViewState="False" meta:resourcekey="lblNewTopic2Resource1">new topic</asp:Label></a></b>
	    <asp:Label ID="lblRegister2" CssClass="gray" visible="false" runat="server" EnableViewState="False" meta:resourcekey="lblRegister2Resource1">please login or <a href="register.aspx">register</a> to create a topic</asp:Label>
	    <span id="spanSubscribe2" runat="server" enableviewstate="false">
            | <asp:LinkButton id="btnSubscribe2" Runat="server" Visible="False" onclick="btnSubscribe_Click" EnableViewState="False" meta:resourcekey="btnSubscribe2Resource1">watch this forum for new topics</asp:LinkButton>
            <asp:LinkButton id="btnUnsubscribe2" Runat="server" Visible="False" onclick="btnUnsubscribe_Click" EnableViewState="False" meta:resourcekey="btnUnsubscribe2Resource1">stop watching this forum</asp:LinkButton>
        </span>
	</div>
	
	<table id="tblError" runat="server" visible="false" width="100%" cellpadding="11" enableviewstate="false">
	<tr><td><div id="divError" runat="server" enableviewstate="false" visible="false"></div></td></tr>
	</table>
	
	<asp:repeater id="rptTopicsList" runat="server" EnableViewState="False" OnItemDataBound="rptTopicsList_ItemDataBound" OnItemCommand="rptTopicsList_ItemCommand">
		<HeaderTemplate>
			<table width="100%" cellpadding="5">
			<%--<tr>
				<th> </th>
				<th><asp:Label ID="lblThread" runat="server" EnableViewState="False" meta:resourcekey="lblThreadResource1">Thread</asp:Label></th>
				<th nowrap="nowrap"><asp:Label ID="lblLatestPost" runat="server" EnableViewState="False" meta:resourcekey="lblLatestPostResource1">Latest Post</asp:Label></th>
				<th><asp:Label ID="lblViews" runat="server" EnableViewState="False" meta:resourcekey="lblViewsResource1">Views</asp:Label></th>
				<th><asp:Label ID="lblReplies" runat="server" EnableViewState="False" meta:resourcekey="lblRepliesResource1">Posts</asp:Label></th>
			</tr>--%>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td align="center"><img src="images/topic.gif" alt="topic" id="imgTopic" runat="server" enableviewstate="false" /></td>
				<td width="70%"><h2>
				        <a href='<%# aspnetforum.Utils.Various.GetTopicURL(Eval("TopicID"), Eval("Subject")) %>'>
							<%# Eval("Subject") %>
						</a>
						<span class="gray">
						    <asp:Label runat="server" EnableViewState="False" meta:resourcekey="lblFromResource1">from</asp:Label>
						<%# DisplayUserName(Eval("UserName"), Eval("UserID"), Eval("FirstName"), Eval("LastName"))%></span>
					</h2>
					<div style="float:right" class="gray">
						<asp:LinkButton id="btnModeratorApprove" Runat="server" Visible="False" CommandName="approve" CommandArgument='<%# Eval("TopicID") %>' meta:resourcekey="btnModeratorApproveResource1">&bull;<b>approve</b></asp:LinkButton>
						<asp:LinkButton OnClientClick="if(!confirm('Are you sure?')) return false;" id="btnModeratorDelete" Runat="server" Visible="False" CommandName="delete" CommandArgument='<%# Eval("TopicID") %>' meta:resourcekey="btnModeratorDeleteResource1">&bull;delete</asp:LinkButton>
						<asp:LinkButton ID="btnModeratorStick" runat="server" Visible="False" CommandName="stick" CommandArgument='<%# Eval("TopicID") %>' meta:resourcekey="btnModeratorStickResource1">&bull; make sticky</asp:LinkButton>
						<asp:LinkButton ID="btnModeratorUnStick" runat="server" Visible="False" CommandName="unstick" CommandArgument='<%# Eval("TopicID") %>' meta:resourcekey="btnModeratorUnStickResource1">&bull; unstick</asp:LinkButton>
						<asp:LinkButton ID="btnModeratorClose" runat="server" Visible="False" CommandName="close" CommandArgument='<%# Eval("TopicID") %>' meta:resourcekey="btnModeratorCloseResource1">&bull; close</asp:LinkButton>
						<asp:LinkButton ID="btnModeratorReopen" runat="server" Visible="False" CommandName="reopen" CommandArgument='<%# Eval("TopicID") %>' meta:resourcekey="btnModeratorReopenResource1">&bull; reopen</asp:LinkButton>
					</div>
					<span class="gray"><%# ShowPageLinks(Eval("Messages"), Eval("TopicID"), Eval("Subject"))%></span>
				</td>
				<td style="white-space:nowrap"><asp:Label ID="lblLatestPost" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblLatestPostResource1">Latest Post</asp:Label>
				    <br />
				    <%# aspnetforum.Utils.Message.GetMsgInfoByID(Eval("LastMessageID"), Eval("Subject"), Cmd)%></td>
				<td><asp:Label ID="lblViews" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblViewsResource1">Views</asp:Label>
				    <br />
				    <%# Eval("ViewsCount") %></td>
				<td><asp:Label ID="lblReplies" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblRepliesResource1">Posts</asp:Label>
				    <br />
				    <%# Eval("Messages") %></td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			<%--<tr><th colspan="5"><%# pagerString %></th></tr>--%>
			</table>
		</FooterTemplate>
	</asp:repeater>
	<div class="smalltoolbar" id="divToolbarBottom" runat="server" enableviewstate="false">
	    <%= pagerString %>
	    <b><a id="linkAddTopic" visible="false" runat="server" enableviewstate="false"><asp:Label ID="lblNewTopic" runat="server" EnableViewState="False" meta:resourcekey="lblNewTopicResource1">new topic</asp:Label></a></b>
	    <asp:Label ID="lblRegister" CssClass="gray" visible="false" runat="server" EnableViewState="False" meta:resourcekey="lblRegisterResource1">please login or <a href="register.aspx">register</a> to create a topic</asp:Label>
	    <span id="spanSubscribe" runat="server" enableviewstate="false">
	        | <asp:LinkButton id="btnSubscribe" Runat="server" Visible="False" onclick="btnSubscribe_Click" EnableViewState="False" meta:resourcekey="btnSubscribeResource1">watch this forum for new topics</asp:LinkButton>
	        <asp:LinkButton id="btnUnsubscribe" Runat="server" Visible="False" onclick="btnUnsubscribe_Click" EnableViewState="False" meta:resourcekey="btnUnsubscribeResource1">stop watching this forum</asp:LinkButton>
	    </span>
	</div>
</div>
<div class="location">
    <h2><a href="default.aspx"><asp:Label ID="lblHome2" runat="server" EnableViewState="False" meta:resourcekey="lblHome2Resource1">Home</asp:Label></a> &raquo;
    <asp:Label id="lblCurForumBottom" runat="server" EnableViewState="False" meta:resourcekey="lblCurForumBottomResource1"></asp:Label></h2>
</div>
</asp:Content>