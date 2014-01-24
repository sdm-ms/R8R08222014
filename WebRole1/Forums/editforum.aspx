<%@ Page language="c#" ValidateRequest="false" Codebehind="editforum.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.editforum" MasterPageFile="AspNetForumMaster.Master" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
	<div class="location"><img alt="" src="images/admin.gif" />&nbsp;
	<b><a href="admin.aspx"><asp:Label ID="lblAdmin" runat="server" EnableViewState="False" meta:resourcekey="lblAdminResource1">Administrator</asp:Label></a>
	&raquo;
	<asp:Label ID="lblEditForum" runat="server" EnableViewState="False" meta:resourcekey="lblEditForumResource1">Editing forum</asp:Label></b></div>
	
	<div><b><asp:Label ID="lblParams" runat="server" EnableViewState="False" meta:resourcekey="lblParamsResource1">Forum parameters:</asp:Label></b>
	    <table cellpadding="7" class="noborder">
		    <tr>
			    <td class="gray"><asp:Label ID="lblTitle" runat="server" EnableViewState="False" meta:resourcekey="lblTitleResource1">Title:</asp:Label></td>
			    <td style="width:80%;"><asp:textbox id="tbTitle" runat="server" Width="100%" meta:resourcekey="tbTitleResource1"></asp:textbox></td>
		    </tr>
		    <tr>
			    <td class="gray"><asp:Label ID="lblDescr" runat="server" EnableViewState="False" meta:resourcekey="lblDescrResource1">Description:</asp:Label></td>
			    <td><asp:textbox id="tbDescr" runat="server" Width="100%" meta:resourcekey="tbDescrResource1"></asp:textbox></td>
		    </tr>
		    <tr>
			    <td class="gray"><asp:Label ID="lblForumGroup" runat="server" EnableViewState="False" meta:resourcekey="lblForumGroupResource1">Forum category:</asp:Label></td>
			    <td class="gray">
				    <asp:Label ID="lblSelect" runat="server" EnableViewState="False" meta:resourcekey="lblSelectResource1">select from a list:</asp:Label>
				    <asp:DropDownList id="ddlForumGroup" runat="server" DataTextField="GroupName" DataValueField="GroupID" meta:resourcekey="ddlForumGroupResource1"></asp:DropDownList>
				    <a href="forumgroups.aspx">...</a>
				    <asp:Label ID="lblEnterNew" runat="server" EnableViewState="False" meta:resourcekey="lblEnterNewResource1">or enter a new value:</asp:Label>
				    <asp:TextBox id="tbForumGroup" runat="server" meta:resourcekey="tbForumGroupResource1"></asp:TextBox></td>
		    </tr>
		    <tr>
		        <td class="gray"><asp:Label ID="lblOrderByNumber" runat="server" EnableViewState="False" meta:resourcekey="lblOrderByNumberResource1">Sort by number (forums are ordered by this number in the list):</asp:Label></td>
		        <td><asp:TextBox ID="tbOrderByNumber" runat="server"></asp:TextBox></td>
		    </tr>
		    <tr>
			    <td colspan="2">
			    <asp:CheckBox id="cbPremoderated" runat="server" meta:resourcekey="lblPremoderResource1" Text="Premoderated"></asp:CheckBox></td>
		    </tr>
		    <tr>
			    <td colspan="2">
			    <asp:CheckBox id="cbMembersOnly" runat="server" meta:resourcekey="lblMembersOnlyResource1" Text="Members only (guests are not allowed to read this forum)"></asp:CheckBox></td>
		    </tr>
		    <tr>
			    <td colspan="2">
			    <asp:CheckBox id="cbRestrictTopicCreation" runat="server" Text="Restrict topic creation to moderators only"></asp:CheckBox></td>
		    </tr>
		    <tr>
		        <td class="gray">Icon (recommended GIF or PNG 32x32, leave blank to reset to default):</td>
		        <td><asp:FileUpload ID="iconUpload" runat="server" /></td>
		    </tr>
		    <tr><td colspan="2"><asp:button id="btnSave" runat="server" Text="save" onclick="btnSave_Click" meta:resourcekey="btnSaveResource1"></asp:button></td></tr>
	    </table>
	</div>
	
	<br /><br /><br />
	<div><b><asp:Label ID="lblModerators" runat="server" EnableViewState="False" meta:resourcekey="lblModeratorsResource1">Forum moderators:</asp:Label></b>
	<hr />
		<asp:datagrid id="gridModerators" runat="server" AutoGenerateColumns="False" ShowHeader="False"
			BorderColor="Silver" EnableViewState="False" CellPadding="5" OnItemCommand="gridModerators_ItemCommand" meta:resourcekey="gridModeratorsResource1">
			<AlternatingItemStyle BackColor="Lavender" />
			<Columns>
				<asp:BoundColumn Visible="False" DataField="UserID" HeaderText="UserID"></asp:BoundColumn>
				<asp:BoundColumn DataField="UserName" HeaderText="UserName"></asp:BoundColumn>
				<asp:ButtonColumn Text="delete" CommandName="delete" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
			</Columns>
		</asp:datagrid>
		<asp:Label ID="lblNoModerators" Runat="server" Visible="False" meta:resourcekey="lblNoModeratorsResource1">No moderators assigned</asp:Label>
		<div>
		<asp:Label ID="lblUsername" runat="server" EnableViewState="False" meta:resourcekey="lblUsernameResource1">Username:</asp:Label>
		<asp:TextBox id="tbModerator" runat="server" meta:resourcekey="tbModeratorResource1"></asp:TextBox>
		<asp:Button id="btnAdd" runat="server" Text="add moderator" onclick="btnAdd_Click" meta:resourcekey="btnAddResource1"></asp:Button></div>
		</div>
	
	<br /><br /><br />
	<div><b><asp:Label ID="lblAllowed" runat="server" EnableViewState="False" meta:resourcekey="lblAllowedResource1">Allowed groups:</asp:Label></b>
	<hr />
		<asp:DataGrid ID="gridGroups" Runat="server" EnableViewState="False" AutoGenerateColumns="False"
			BorderColor="Silver" OnItemCommand="gridGroups_ItemCommand" meta:resourcekey="gridGroupsResource1">
			<AlternatingItemStyle BackColor="Lavender" />
			<Columns>
				<asp:BoundColumn Visible="False" DataField="GroupID"></asp:BoundColumn>
				<asp:BoundColumn DataField="Title" HeaderText="GroupName"></asp:BoundColumn>
				<asp:TemplateColumn HeaderText="Allow Reading">
					<ItemTemplate>
						<asp:CheckBox ID="chkAllowReading" Runat="server" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "AllowReading")) %>' meta:resourcekey="chkAllowReadingResource1">
						</asp:CheckBox>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn HeaderText="Allow Posting">
					<ItemTemplate>
						<asp:CheckBox ID="chkAllowPosting" Runat="server" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "AllowPosting")) %>' meta:resourcekey="chkAllowPostingResource1">
						</asp:CheckBox>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:ButtonColumn CommandName="save" Text="save changes..." meta:resourcekey="ButtonColumnResource2"></asp:ButtonColumn>
				<asp:ButtonColumn CommandName="delete" Text="delete" meta:resourcekey="ButtonColumnResource3"></asp:ButtonColumn>
			</Columns>
		</asp:DataGrid>
		<asp:Label ID="lblFFA" Runat="server" Visible="False" meta:resourcekey="lblFFAResource1">No explicit group-permissions assigned to this forum. That means that the forum is <b>Free-For-All</b></asp:Label><br />
		<asp:Label ID="lblAddPerm" runat="server" EnableViewState="False" meta:resourcekey="lblAddPermResource1">Add permission to group:</asp:Label>
		<asp:DropDownList ID="ddlGroups" Runat="server" DataTextField="Title" DataValueField="GroupID" meta:resourcekey="ddlGroupsResource1"></asp:DropDownList>
		<asp:CheckBox ID="chkAllowReadingNew" Runat="server" Text="Allow reading" meta:resourcekey="chkAllowReadingNewResource1"></asp:CheckBox>
		<asp:CheckBox ID="chkAllowPostingNew" Runat="server" Text="Allow posting" meta:resourcekey="chkAllowPostingNewResource1"></asp:CheckBox>
		<asp:Button ID="btnAddPermission" Runat="server" Text="add permission" onclick="btnAddPermission_Click" meta:resourcekey="btnAddPermissionResource1"></asp:Button>
        <a href="adminusergroups.aspx"><asp:Label ID="lblUserGroups" runat="server" EnableViewState="False" meta:resourcekey="lblUserGroupsResource1">edit user-groups</asp:Label></a>
		<br /><br />
		NOTE: If you set posting permissions ONLY (and no read permissions for the forum), the forum will be still readable by all users (including guests).
	</div>
</asp:Content>