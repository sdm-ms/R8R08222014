<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="recentposts.ascx.cs" Inherits="aspnetforum.recentposts" %>

<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" OnItemDataBound="rptMessagesList_ItemDataBound">
    <HeaderTemplate>
        <table width="100%" cellpadding="19">
    </HeaderTemplate>
	<ItemTemplate>
		<tr valign="top">
			<td style="width:120px" rowspan="2">
				<span class="gray">
				<%# Eval("CreationDate") %><br />
				Topic:</span>
				<a href='<%# aspnetforum.Utils.Various.GetTopicURL(Eval("TopicID"), Eval("Subject")) %>'><b><%# Eval("Subject") %></b></a>
				<br/><br/>
				<%# aspnetforum.Utils.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), Eval("PostsCount"), Eval("AvatarFileName"), Eval("FirstName"), Eval("LastName"))%>
			</td>
			<td style="border-bottom:none;">
				<%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString())%>
			</td>
		</tr>
		<tr><td valign="bottom" align="right" class="gray" style="border-top:none;">
		    <a runat="server" id="lnkQuote" visible="False"><asp:Label ID="lblQuote" runat="server" EnableViewState="False" meta:resourcekey="lblQuoteResource1">&bull; reply with quote</asp:Label></a>
		</td></tr>
	</ItemTemplate>
	<FooterTemplate></table></FooterTemplate>
</asp:repeater>