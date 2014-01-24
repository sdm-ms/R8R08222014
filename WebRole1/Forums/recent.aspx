<%@ Page language="c#" Codebehind="recent.aspx.cs" Title="Recent Messages" AutoEventWireup="True" Inherits="aspnetforum.recent" MasterPageFile="AspNetForumMaster.Master" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="cc" TagName="RecentPosts" Src="recentposts.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHEAD" ID="AspNetForumHead" runat="server">
<link rel="alternate" type="application/rss+xml" title="recent posts" href="recent.aspx?rss=1" />
</asp:Content>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
<div class="location">
    <a href="recent.aspx?rss=1" runat="server" id="rssLink" enableviewstate="false"><img alt="recent posts - RSS" src="images/rss20.gif" style="float:right" /></a>
	<h2><a href="default.aspx">
		<asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
	&raquo;
	<asp:Label ID="lblRecent" runat="server" EnableViewState="False" meta:resourcekey="lblRecentResource1">Recent messages</asp:Label></h2>
</div>

<cc:RecentPosts id="recentPosts" runat="server"></cc:RecentPosts>

<div class="location">
	<h2><a href="default.aspx">
		<asp:Label ID="lblHome2" runat="server" EnableViewState="False" meta:resourcekey="lblHome2Resource1">Home</asp:Label></a>
	&raquo;
	<asp:Label ID="lblRecent2" runat="server" EnableViewState="False" meta:resourcekey="lblRecent2Resource1">Recent messages</asp:Label></h2>
</div>
</asp:Content>