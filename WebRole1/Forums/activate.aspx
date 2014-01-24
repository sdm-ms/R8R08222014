<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="activate.aspx.cs" Inherits="aspnetforum.activate" MasterPageFile="AspNetForumMaster.Master" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">

	<div class="location">
		<b><a href="default.aspx">Home</a> &raquo; Activation </b>
	</div>
	<asp:Label ID="lblSuccess" runat="server" Visible="false" meta:resourcekey="lblSuccessResource1">Activation successful. You are welcome to login</asp:Label>
	<asp:Label ID="lblError" runat="server" Visible="false" meta:resourcekey="lblErrorResource1">ERROR! Not activated - wrong code or username</asp:Label>
</asp:Content>