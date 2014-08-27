<%@ Page Title="" Language="C#" MasterPageFile="AspNetForumMaster.Master" AutoEventWireup="true" CodeBehind="moderator.aspx.cs" Inherits="aspnetforum.moderator" %>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">
    <div class="location"><img alt="" src="images/admin.gif" />
	<b><asp:Label ID="lblModerator" runat="server" EnableViewState="False" meta:resourcekey="lblModeratorResource1">Moderator</asp:Label></b>
	:
	<a href="unapprovedposts.aspx"><asp:Label ID="lblUnapproved" runat="server" EnableViewState="False" meta:resourcekey="lblUnapprovedResource1">unapproved posts</asp:Label></a> |
	<a href="complaints.aspx"><asp:Label ID="lblComplaints" runat="server" EnableViewState="False" meta:resourcekey="lblComplaintsResource1">complaints</asp:Label></a>
	</div>
</asp:Content>
