<%@ Page Title="" Language="C#" MasterPageFile="AspNetForumMaster.Master" AutoEventWireup="true" CodeBehind="complaints.aspx.cs" Inherits="aspnetforum.complaints" %>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">
    <div class="location">
        <img alt="" src="images/admin.gif" />
		<b><a href="moderator.aspx">
		    <asp:Label ID="lblModerator" runat="server" EnableViewState="False" meta:resourcekey="lblModeratorResource1">Moderator</asp:Label></a>
		&raquo;
		<asp:Label ID="lblComplaintsList" runat="server" EnableViewState="False" meta:resourcekey="lblComplaintsListResource1">List of complaints</asp:Label></b>
	</div>
	
	<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" 
        OnItemCommand="rptMessagesList_ItemCommand" 
        onitemdatabound="rptMessagesList_ItemDataBound">
	    <HeaderTemplate>
	        <table width="100%" cellpadding="5">
	    </HeaderTemplate>
		<ItemTemplate>
			<tr>
			    <th style="width:120px"></th>
				<th>
					<div style="float:left"><%# Eval("CreationDate") %></div>
				    <div style="float:right">
					    <asp:LinkButton id="btnModeratorUnflag" Runat="server" CommandName="remove" CommandArgument='<%# Eval("MessageID") %>' meta:resourcekey="btnModeratorDeleteResource1">&bull; flag as complete (remove from the list)</asp:LinkButton>
				    </div>
				</th>
			</tr>
			<tr valign="top">
				<td>
					topic: <a href='<%# aspnetforum.Utils.Various.GetTopicURL(Eval("TopicID"), Eval("Subject")) %>'>
						<b><%# Eval("Subject") %></b></a>
					<br/><br/>
					<%# aspnetforum.Utils.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), Eval("PostsCount"), Eval("AvatarFileName"), Eval("FirstName"), Eval("LastName"))%>
				</td>
				<td>
				    <span style="color:Red">reported by:
				    <b>
				    <asp:Repeater ID="rptComplainUsers" runat="server">
				        <ItemTemplate><%# Eval("UserName")%></ItemTemplate>
				    </asp:Repeater>
				    </b>
				    </span>
				    <hr />
					<%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString())%>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
		    <tr>
			<th colspan="2">
				<asp:Label ID="lblPages" runat="server" EnableViewState="False" meta:resourcekey="lblPagesResource1">pages:</asp:Label>
				<%# pagerString %>
			</th></tr>
		    </table>
		</FooterTemplate>
	</asp:repeater>
	
	<div id="divNothingFound" runat="server" visible="false" enableviewstate="false">
	No messages.
	</div>
	
	<div class="location">
        <img alt="" src="images/admin.gif" />
		<b><a href="moderator.aspx">
		    <asp:Label ID="lblModerator2" runat="server" EnableViewState="False" meta:resourcekey="lblModeratorResource1">Moderator</asp:Label></a>
		&raquo;
		<asp:Label ID="lblComplaintsList2" runat="server" EnableViewState="False" meta:resourcekey="lblComplaintsListResource1">List of complaints</asp:Label></b>
	</div>
</asp:Content>
