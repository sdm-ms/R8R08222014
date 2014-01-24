<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="recenttopics-iphone.aspx.cs" MasterPageFile="IPhone.Master" Inherits="aspnetforum.recenttopics_iphone" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<span class="graytitle">Recent topics</span>

<asp:repeater id="rptTopicsList" runat="server" EnableViewState="False">
    <HeaderTemplate><ul class="pageitem"></HeaderTemplate>
	<ItemTemplate>
		<li class="menu"><a href='<%# aspnetforum.Utils.Various.GetTopicURL(Eval("TopicID"), Eval("Subject")) %>'>
			<span class="name"><%# Eval("Subject") %></span><span class="arrow"></span>
		</a></li>
	</ItemTemplate>
	<FooterTemplate>
		</ul>
	</FooterTemplate>
</asp:repeater>

</asp:Content>