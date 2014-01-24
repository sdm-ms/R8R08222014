<%@ Page Title="" Language="C#" MasterPageFile="AspNetForumMaster.Master" AutoEventWireup="true" CodeBehind="updatedtopics.aspx.cs" Inherits="aspnetforum.updatedtopics" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<div class="location">
	<h2><a href="default.aspx">
		<asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
	&raquo;
	<asp:Label ID="lblUpdated" runat="server" EnableViewState="False" meta:resourcekey="lblUpdatedResource1">Topics updated since your last visit</asp:Label></h2>
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


</asp:Content>
