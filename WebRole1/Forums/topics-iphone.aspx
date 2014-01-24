<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="topics-iphone.aspx.cs" Inherits="aspnetforum.topics_iphone" MasterPageFile="IPhone.Master" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<asp:repeater id="rptSubForumsList" runat="server" EnableViewState="False">
    <HeaderTemplate><span class="graytitle">Subforums</span><ul class="pageitem"></HeaderTemplate>
    <ItemTemplate>
        <li class="menu"><a href='<%# aspnetforum.Utils.Various.GetForumURL(Eval("ForumID"), Eval("Title")) %>'><span class="name"><%# Eval("Title") %></span><span class="arrow"></span></a></li>
    </ItemTemplate>
    <FooterTemplate></ul></FooterTemplate>
</asp:repeater>


<span class="graytitle"><%= pagerString %> <a href="addpost.aspx" runat="server" id="linkAddTopic" enableviewstate="false">add topic</a></span>
	
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

<span class="graytitle"><%= pagerString %></span>

</asp:Content>