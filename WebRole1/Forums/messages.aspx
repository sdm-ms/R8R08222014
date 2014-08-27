<%@ Page language="c#" Codebehind="messages.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.messages" MasterPageFile="AspNetForumMaster.Master" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderHEAD" ID="AspNetForumHead" runat="server">
<link rel="alternate" type="application/rss+xml" title="topics in this forum" id="rssDiscoverLink" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
<div class="location">
    <a runat="server" id="rssLink" enableviewstate="false"><img alt="Messages in this topic - RSS" src="images/rss20.gif" style="float:right" /></a>
	<h2><a href="default.aspx"><asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
		&raquo;
		<asp:label id="lblCurForum" runat="server" EnableViewState="False"></asp:label>
		&raquo;
		<asp:label id="lblCurTopic" runat="server" EnableViewState="False" meta:resourcekey="lblCurTopicResource1"></asp:label></h2>
</div>

<div class="gray" id="divDescription" runat="server"></div>

<script src="jquery.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function RatePost(msgId, score) {
        //jquery ajax post
        $.post(
        "messagesajax.ashx", //url
        {Score: score, Mode: "Rate", MessageID: msgId },
        function(rating) {
            if (rating!="") {
                var sign = (rating > 0) ? "+" : "";
                $("#spanRating" + msgId).html(sign + rating);
                if (rating != 0) {
                    var color = (rating > 0) ? "green" : "red";
                    $("#spanRating" + msgId).css("color", color);
                }
            }
        });
    }
</script>

<div id="divPoll" runat="server" visible="false" align="center" style="padding:17px 17px 17px 17px">
    <h2><asp:Label ID="lblPollName" runat="server"></asp:Label></h2>
    <asp:RadioButtonList CssClass="radiolist" style="margin: 5px 5px 5px 5px;" ID="rblOptions" runat="server" DataTextField="OptionText" DataValueField="OptionID" Visible="false" CellPadding="5"></asp:RadioButtonList>
    <asp:Repeater id="rptVoteResults" runat="server">
        <HeaderTemplate>
            <table cellpadding="5" style="margin: 5px 5px 5px 5px">
        </HeaderTemplate>
        <ItemTemplate>
            <tr><td><b><%# Eval("OptionText") %>:</b></td><td align="left"><%# Eval("VoteCount") %> <img src="images/darkgradient.gif" height="10" width='<%# GetVotingBarWidth(Eval("VoteCount")) %>' alt="" /></td></tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Button ID="btnVote" runat="server" Text="vote" CssClass="gradientbutton" OnClick="btnVote_Click" meta:resourcekey="btnVoteResource1" />
</div>

<div class="smalltoolbar">
    <asp:Label ID="lblPagesTop" runat="server" EnableViewState="False" meta:resourcekey="lblPagesResource1">pages:</asp:Label>
    <%= pagerString %>
    <b><a id="linkAddPostTop" href="addpost.aspx" runat="server" enableviewstate="false"><asp:Label ID="lblReplyTop" runat="server" EnableViewState="False" meta:resourcekey="lblReplyResource1">add reply</asp:Label></a>
    <asp:Label runat="server" ID="lblClosedTop" Visible="False" EnableViewState="False" meta:resourcekey="lblClosedResource1">the topic is closed</asp:Label></b>
    <span id="spanSubcriptionTop" runat="server" enableviewstate="false">
        | <asp:LinkButton id="btnSubscribeTop" Runat="server" Visible="False" onclick="btnSubscribe_Click" EnableViewState="False" meta:resourcekey="btnSubscribeResource1">watch this topic for replies</asp:LinkButton>
        <asp:LinkButton id="btnUnsubscribeTop" Runat="server" Visible="False" onclick="btnUnsubscribe_Click" EnableViewState="False" meta:resourcekey="btnUnsubscribeResource1">stop watching this topic</asp:LinkButton>
    </span>
    <span id="spanMoveTop" runat="server">
        |
        <a href="javascript:void(0)" onclick="document.getElementById('spanMoveControlTop').style.display='';"><asp:Label ID="lblMoveTop" runat="server" EnableViewState="False" meta:resourcekey="lblMoveResource1">move thread to forum:</asp:Label></a>
        <span id="spanMoveControlTop" style="display:none">
        <br />
        <asp:DropDownList ID="ddlForumsTop" Runat="server" DataTextField="Title" DataValueField="ForumID" meta:resourcekey="ddlForumsResource1"></asp:DropDownList><asp:Button ID="btnMoveTop" Runat="server" Text="move" onclick="btnMoveTop_Click" CssClass="gradientbutton" EnableViewState="False" meta:resourcekey="btnMoveResource1"></asp:Button>
        </span>
    </span>
</div>

<table id="tblError" runat="server" visible="false" width="100%" cellpadding="11" enableviewstate="false">
<tr><td><div id="divError" runat="server" enableviewstate="false" visible="false"></div></td></tr>
</table>

<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" OnItemDataBound="rptMessagesList_ItemDataBound" OnItemCommand="rptMessagesList_ItemCommand">
    <HeaderTemplate>
        <table style="width:100%;" cellpadding="19">
    </HeaderTemplate>
    <ItemTemplate>
	    <tr valign="top">
		    <td style="width:120px" rowspan="2">
		        <a name='post<%# Eval("MessageID") %>'></a>
		        <span class="gray"><%# Eval("CreationDate") %></span>
		        <br /><br />
				<%# aspnetforum.Utils.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), Eval("PostsCount"), Eval("AvatarFileName"), _forumID, Eval("FirstName"), Eval("LastName"))%>
				
				<%# _isModerator ? (Eval("IPAddress").ToString()!="" ? "<br/><br/><span class='gray'>Posted from IP:</span> " + Eval("IPAddress").ToString() : "") : "" %>
		    </td>
		    <td style="border-bottom:none;">
				<%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString()) %>
				<%# aspnetforum.Utils.Formatting.FormatSignature(Eval("Signature").ToString())%>
			    <asp:Repeater ID="rptFiles" runat="server">
			    <HeaderTemplate>
					<br /><br />
					<div class="gray">
					<asp:Label ID="lblAttachments" runat="server" EnableViewState="False" meta:resourcekey="lblAttachmentsResource1">Attachments:</asp:Label><br />
			    </HeaderTemplate>
			    <ItemTemplate>
			        <a href='getattachment.ashx?fileid=<%# Eval("FileID") %>'>
			        <%# aspnetforum.Utils.Attachments.GetThumbnail(Eval("FileName").ToString(), Convert.ToInt32(Eval("UserID"))) %>
			        <%# Eval("FileName") %></a><br />
			    </ItemTemplate>
			    <FooterTemplate></div></FooterTemplate>
			    </asp:Repeater>
		    </td>
	    </tr>
	    <tr><td valign="bottom" align="right" class="gray" style="border-top:none;">
	        <span id="spanRate" runat="server" enableviewstate="false">
	            <%# RenderMsgRating(Eval("MessageID"), Eval("Rating")) %>
	            <a href="javascript:void(0)" onclick="RatePost(<%# Eval("MessageID") %>, 1)"><img src="images/thumbsup.png" alt="thumbs up" /></a>
	            <a href="javascript:void(0)" onclick="RatePost(<%# Eval("MessageID") %>, -1)"><img src="images/thumbsdown.png" alt="thumbs down" /></a>
	        </span>
	        <a href='#post<%# Eval("MessageID") %>'>&bull; <asp:Label ID="lblPermalink" runat="server" EnableViewState="False" meta:resourcekey="lblPermalinkResource1">permalink</asp:Label></a>
			<a runat="server" id="lnkQuote" visible="False"><asp:Label ID="lblQuote" runat="server" EnableViewState="False" meta:resourcekey="lblQuoteResource1">&bull; reply with quote</asp:Label></a>
			<asp:LinkButton id="btnComplain" Visible="False" Runat="server" CommandName="complain" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnComplainResource1">&bull; report to moderator</asp:LinkButton>
		    <a runat="server" id="lnkEdit" visible="False"><asp:Label ID="lblEdit" runat="server" EnableViewState="False" meta:resourcekey="lblEditResource1">&bull; edit</asp:Label></a>
		    <asp:LinkButton id="btnModeratorApprove" Runat="server" Visible="False" CommandName="approve" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnModeratorApproveResource1">&bull; <b>approve</b></asp:LinkButton>
		    <asp:LinkButton id="btnModeratorDelete" OnClientClick="if(!confirm('Are you sure?')) return false;" Runat="server" Visible="False" CommandName="delete" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnModeratorDeleteResource1">&bull; delete</asp:LinkButton>
	    </td></tr>
    </ItemTemplate>
    <FooterTemplate>
        <%--<tr><th colspan="2">
        <asp:Label ID="lblPages" runat="server" EnableViewState="False" meta:resourcekey="lblPagesResource1">pages:</asp:Label>
        <%# pagerString %></th></tr>--%>
        </table>
    </FooterTemplate>
</asp:repeater>

<div class="smalltoolbar">
    <asp:Label ID="lblPages" runat="server" EnableViewState="False" meta:resourcekey="lblPagesResource1">pages:</asp:Label>
    <%= pagerString %>
    <b><a id="linkAddPost" href="addpost.aspx" runat="server" enableviewstate="false"><asp:Label ID="lblReply" runat="server" EnableViewState="False" meta:resourcekey="lblReplyResource1">add reply</asp:Label></a>
    <asp:Label runat="server" ID="lblClosed" Visible="False" EnableViewState="False" meta:resourcekey="lblClosedResource1">the topic is closed</asp:Label></b>
    <span id="spanSubscription" runat="server" enableviewstate="false">
        | <asp:LinkButton id="btnSubscribe" Runat="server" Visible="False" onclick="btnSubscribe_Click" EnableViewState="False" meta:resourcekey="btnSubscribeResource1">watch this topic for replies</asp:LinkButton>
        <asp:LinkButton id="btnUnsubscribe" Runat="server" Visible="False" onclick="btnUnsubscribe_Click" EnableViewState="False" meta:resourcekey="btnUnsubscribeResource1">stop watching this topic</asp:LinkButton>
    </span>
    <span id="spanMove" runat="server">
        &nbsp;|&nbsp;
        <a href="javascript:void(0)" onclick="document.getElementById('spanMoveControl').style.display='';"><asp:Label ID="lblMove" runat="server" EnableViewState="False" meta:resourcekey="lblMoveResource1">move thread to forum:</asp:Label></a>
        <span id="spanMoveControl" style="display:none">
        <br />
        <asp:DropDownList ID="ddlForums" Runat="server" DataTextField="Title" DataValueField="ForumID" meta:resourcekey="ddlForumsResource1"></asp:DropDownList><asp:Button ID="btnMove" Runat="server" Text="move" onclick="btnMove_Click" CssClass="gradientbutton" EnableViewState="False" meta:resourcekey="btnMoveResource1"></asp:Button>
        </span>
    </span>
</div>

<div class="location">
	<h2><a href="default.aspx"><asp:Label ID="lblHome2" runat="server" EnableViewState="False" meta:resourcekey="lblHome2Resource1">Home</asp:Label></a>
	&raquo;
	<asp:label id="lblCurForumBottom" runat="server" EnableViewState="False"></asp:label>
	&raquo;
	<asp:label id="lblCurTopicBottom" runat="server" EnableViewState="False" meta:resourcekey="lblCurTopicBottomResource1"></asp:label></h2>
</div>

<div id="divQuickReply" runat="server" enableviewstate="false" visible="false">
<br />
<asp:TextBox Rows="2" TextMode="MultiLine" Height="34" Width="50%" ID="tbQuickReply" runat="server" EnableViewState="false"></asp:TextBox>
<asp:Button runat="server" Height="38" ID="btnQuickReply" OnClick="btnQuickReply_Click" Text="quick reply" EnableViewState="false" meta:resourcekey="btnQuickReplyResource1" />
</div>

<script type="text/javascript">
    if (document.location.href.toLowerCase().indexOf("messageid=") != -1) {
        document.location.hash = 'post<%= Request.QueryString["MessageID"] %>';
    }
</script>
</asp:Content>