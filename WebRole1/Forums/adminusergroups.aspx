<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminusergroups.aspx.cs" Inherits="aspnetforum.adminusergroups" MasterPageFile="AspNetForumMaster.Master" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
<div class="location"><img alt="" src="images/admin.gif" />&nbsp;
<b><a href="admin.aspx"><asp:Label ID="lblAdmin" runat="server" EnableViewState="False" meta:resourcekey="lblAdminResource1" Text="Administrator"></asp:Label></a>
&raquo;
<asp:Label ID="lblUserGroups" runat="server" EnableViewState="False" meta:resourcekey="lblUserGroupsResource1">User groups</asp:Label></b></div>
<p><b><asp:Label ID="lblAvailableGroups" runat="server" EnableViewState="False" meta:resourcekey="lblAvailableGroupsResource1" Text="Available user groups:"></asp:Label></b>
	<br />
	<asp:DataGrid CellPadding="7" id="gridGroups" Runat="server" Width="100%" AutoGenerateColumns="False" EnableViewState="False"
		ShowHeader="False" OnItemCommand="gridGroups_ItemCommand" BorderColor="Silver" meta:resourcekey="gridGroupsResource1">
		<AlternatingItemStyle BackColor="Lavender" />
		<Columns>
			<asp:BoundColumn Visible="False" DataField="GroupID"></asp:BoundColumn>
			<asp:BoundColumn DataField="Title" HeaderText="Title"></asp:BoundColumn>
			<asp:HyperLinkColumn Text="edit" DataNavigateUrlField="GroupID" DataNavigateUrlFormatString="editusergroup.aspx?GroupID={0}" meta:resourcekey="HyperLinkColumnResource1"></asp:HyperLinkColumn>
			<asp:ButtonColumn Text="delete" CommandName="delete" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
		</Columns>
	</asp:DataGrid>
	<asp:Label ID="lblNoGroups" Runat="server" Visible="False" meta:resourcekey="lblNoGroupsResource1" Text="No groups found."></asp:Label>
</p>

<table width="100%" border="1" cellpadding="7" class="noborder">
    <tr><th colspan="2"><b><asp:Label ID="lblAddNew" runat="server" EnableViewState="False" meta:resourcekey="lblAddNewResource1" Text="Add a new group:"></asp:Label></b></th></tr>
	<tr>
		<td class="gray"><asp:Label ID="lblTitle" runat="server" EnableViewState="False" meta:resourcekey="lblTitleResource1" Text="Title:"></asp:Label></td>
		<td style="width:100%"><asp:TextBox id="tbGroupTitle" runat="server" Width="100%" meta:resourcekey="tbGroupTitleResource1"></asp:TextBox></td>
	</tr>
	<tr><td colspan="2"><asp:Button id="btnAddGroup" runat="server" Text="add" onclick="btnAddGroup_Click" meta:resourcekey="btnAddGroupResource1"></asp:Button></td></tr>
</table>
</asp:Content>