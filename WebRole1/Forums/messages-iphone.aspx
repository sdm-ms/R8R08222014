<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="messages-iphone.aspx.cs" Inherits="aspnetforum.messages_iphone" MasterPageFile="IPhone.Master" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<span class="graytitle">
    <asp:label id="lblCurForum" runat="server" EnableViewState="False"></asp:label>
	&raquo;
	<asp:label id="lblCurTopic" runat="server" EnableViewState="False" meta:resourcekey="lblCurTopicResource1"></asp:label>
    | <%= pagerString %>
</span>

<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" OnItemDataBound="rptMessagesList_ItemDataBound" OnItemCommand="rptMessagesList_ItemCommand">
    <ItemTemplate>
	    <ul class="pageitem">
		<li class="textbox">
		    <a name='post<%# Eval("MessageID") %>'></a>
		    <span class="header"><%# Eval("UserName") %> - <%# Eval("CreationDate") %></span>
		    <p><%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString()) %>
			<%# aspnetforum.Utils.Formatting.FormatSignature(Eval("Signature").ToString())%></p>
			<p>
			<a href='#post<%# Eval("MessageID") %>'>&bull; <asp:Label ID="lblPermalink" runat="server" EnableViewState="False" meta:resourcekey="lblPermalinkResource1">permalink</asp:Label></a>
			<a runat="server" id="lnkQuote" visible="False"><asp:Label ID="lblQuote" runat="server" EnableViewState="False" meta:resourcekey="lblQuoteResource1">&bull; reply with quote</asp:Label></a>
			<asp:LinkButton id="btnComplain" Visible="False" Runat="server" CommandName="complain" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnComplainResource1">&bull; report to moderator</asp:LinkButton>
		    <a runat="server" id="lnkEdit" visible="False"><asp:Label ID="lblEdit" runat="server" EnableViewState="False" meta:resourcekey="lblEditResource1">&bull; edit</asp:Label></a>
		    <asp:LinkButton id="btnModeratorApprove" Runat="server" Visible="False" CommandName="approve" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnModeratorApproveResource1">&bull; <b>approve</b></asp:LinkButton>
		    <asp:LinkButton id="btnModeratorDelete" OnClientClick="if(!confirm('Are you sure?')) return false;" Runat="server" Visible="False" CommandName="delete" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnModeratorDeleteResource1">&bull; delete</asp:LinkButton>
			</p>
		</li>
	</ul>
    </ItemTemplate>
</asp:repeater>

<ul class="pageitem" id="ulQuickReply" runat="server" enableviewstate="false">
<span class="header">Quick reply</span>
<li class="textbox"><asp:TextBox Width="100%" TextMode="MultiLine" Rows="3" runat="server" ID="tbQuickReply" EnableViewState="false"></asp:TextBox></li>
<li class="button"><asp:Button runat="server" ID="btnQuickReply" OnClick="btnQuickReply_Click" Text="post reply" EnableViewState="false" meta:resourcekey="btnQuickReplyResource1" /></li>
</ul>

<script type="text/javascript">
    if (document.location.href.toLowerCase().indexOf("messageid=") != -1) {
        document.location.hash = 'post<%= Request.QueryString["MessageID"] %>';
    }
</script>

</asp:Content>