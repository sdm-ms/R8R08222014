<%@ Page Language="C#" AutoEventWireup="true" Title="Recently updated topics" CodeBehind="recenttopics.aspx.cs" Inherits="aspnetforum.recenttopics" MasterPageFile="AspNetForumMaster.Master" meta:resourcekey="PageResource1" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHEAD" ID="AspNetForumHead" runat="server">
<link rel="alternate" type="application/rss+xml" title="recent posts" href="recenttopics.aspx?rss=1" />
</asp:Content>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
	<div class="location">
	    <a href="recenttopics.aspx?rss=1" runat="server" id="rssLink" enableviewstate="false"><img alt="Recently updated topics - RSS" src="images/rss20.gif" style="float:right" /></a>
		<h2><a href="default.aspx">
			<asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
		&raquo;
		<asp:Label ID="lblRecent" runat="server" EnableViewState="False" meta:resourcekey="lblRecentResource1">Recently updated topics</asp:Label></h2>
	</div>
	<asp:repeater id="rptTopicsList" runat="server" EnableViewState="False">
		<HeaderTemplate>
			<table width="100%" cellpadding="5">
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td align="center"><img src="images/topic.gif" alt="topic" id="imgTopic" runat="server" /></td>
				<td width="70%"><h2><a href='<%# aspnetforum.Utils.Various.GetTopicURL(Eval("TopicID"), Eval("Subject")) %>'>
							<%# Eval("Subject") %>
						</a>
					</h2>
				</td>
				<td style="white-space:nowrap">
				    <asp:Label ID="lblLatestPost" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblLatestPostResource1">Latest Post</asp:Label>
				    <br />
				    <%# aspnetforum.Utils.Message.GetMsgInfoByID(Eval("LastMessageID"), Cmd) %>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:repeater>
	
	<div class="location">
		<h2><a href="default.aspx">
			<asp:Label ID="lblHome2" runat="server" EnableViewState="False" meta:resourcekey="lblHome2Resource1">Home</asp:Label></a>
		&raquo;
		<asp:Label ID="lblRecent2" runat="server" EnableViewState="False" meta:resourcekey="lblRecent2Resource1">Recently updated topics</asp:Label></h2>
	</div>
</asp:Content>