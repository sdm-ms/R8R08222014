<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="recent-iphone.aspx.cs" Inherits="aspnetforum.recent_iphone" MasterPageFile="IPhone.Master" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<span class="graytitle">Recent posts</span>

<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False">
    <ItemTemplate>
	    <ul class="pageitem">
		<li class="textbox">
		    <span class="header"><a href='<%# aspnetforum.Utils.Various.GetTopicURL(Eval("TopicID"), Eval("Subject")) %>'><%# Eval("Subject") %></a>
		    <br /><%# Eval("UserName") %> - <%# Eval("CreationDate") %></span>
		    <p><%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString()) %></p>
		</li>
	</ul>
    </ItemTemplate>
</asp:repeater>

</asp:Content>