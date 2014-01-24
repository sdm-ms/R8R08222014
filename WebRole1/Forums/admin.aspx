<%@ Page language="c#" ValidateRequest="false" Codebehind="admin.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.admin" MasterPageFile="AspNetForumMaster.Master" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
	<div class="location"><img alt="" src="images/admin.gif" />
	&nbsp;
	<b><asp:Label ID="lblAdministrator" runat="server" EnableViewState="False" meta:resourcekey="lblAdministratorResource1">Administrator</asp:Label></b>
	:
	<a href="editsubforums.aspx"><asp:Label ID="lblSubforums" runat="server" EnableViewState="False" meta:resourcekey="lblSubforumsResource1">edit subforums</asp:Label></a> |
	<a href="adminusergroups.aspx"><asp:Label ID="lblUserGroups" runat="server" EnableViewState="False" meta:resourcekey="lblUserGroupsResource1">edit user-groups</asp:Label></a> |
	<a href="adduser.aspx"><asp:Label ID="lblAddUser" runat="server" EnableViewState="False" meta:resourcekey="lblAddUserResource1">manualy add a user</asp:Label></a> |
	<a href="nonactivatedusers.aspx"><asp:Label ID="lblNonActivated" runat="server" EnableViewState="False" meta:resourcekey="lblNonActivatedResource1">NON-activated users</asp:Label></a> |
	<a href="emailallusers.aspx"><asp:Label ID="lblEmailAll" runat="server" EnableViewState="False" meta:resourcekey="lblEmailAllResource1">email all users</asp:Label></a>
	</div>
	
	<p>
		<b><asp:Label ID="lblAvailableForums" runat="server" EnableViewState="False" meta:resourcekey="lblAvailableForumsResource1">Available forums:</asp:Label></b>
		<br />
		<asp:DataGrid id="gridForums" runat="server" AutoGenerateColumns="False" Width="100%" EnableViewState="False"
			ShowHeader="False" BorderColor="Silver" OnItemCommand="gridForums_ItemCommand" meta:resourcekey="gridForumsResource1" OnItemDataBound="gridForums_ItemDataBound" CellPadding="7">
			<Columns>
				<asp:BoundColumn Visible="False" DataField="ForumID" HeaderText="ForumID"></asp:BoundColumn>
				<asp:HyperLinkColumn DataTextField="Title" DataNavigateUrlField="ForumID" DataNavigateUrlFormatString="editforum.aspx?ForumID={0}" HeaderText="Title"></asp:HyperLinkColumn>
				<asp:ButtonColumn Text="delete" CommandName="delete" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
			</Columns>
		</asp:DataGrid>
		<asp:Label ID="lblNoForums" Runat="server" Visible="False" meta:resourcekey="lblNoForumsResource1">No forums found.</asp:Label>
	</p>
	<br />
	<p>
	    <table class="noborder" cellpadding="7">
	        <tr><th colspan="2"><asp:Label ID="lblAddNew" runat="server" EnableViewState="False" meta:resourcekey="lblAddNewResource1">Add a new forum:</asp:Label></th></tr>
		    <tr>
			    <td align="right" class="gray" nowrap="nowrap"><asp:Label ID="lblForumTitle" runat="server" EnableViewState="False" meta:resourcekey="lblForumTitleResource1">Title (50 chars max):</asp:Label></td>
			    <td style="width:100%;">
				    <asp:TextBox id="tbTitle" runat="server" Width="100%" MaxLength="50" meta:resourcekey="tbTitleResource1"></asp:TextBox></td>
		    </tr>
		    <tr>
			    <td align="right" class="gray" nowrap="nowrap"><asp:Label ID="lblForumDescription" runat="server" EnableViewState="False" meta:resourcekey="lblForumDescriptionResource1">Description (255 chars max):</asp:Label></td>
			    <td>
				    <asp:TextBox id="tbDescr" runat="server" Width="100%" MaxLength="255" meta:resourcekey="tbDescrResource1"></asp:TextBox></td>
		    </tr>
		    <tr>
			    <td align="right" class="gray" nowrap="nowrap"><asp:Label ID="lblForumGroup" runat="server" EnableViewState="False" meta:resourcekey="lblForumGroupResource1">Forum category:</asp:Label></td>
			    <td>
				    <asp:Label ID="lblSelectGroup" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblSelectGroupResource1">select a category from this list:</asp:Label>
				    <asp:DropDownList id="ddlForumGroup" runat="server" DataTextField="GroupName" DataValueField="GroupID" meta:resourcekey="ddlForumGroupResource1"></asp:DropDownList>
				    <button onclick="document.location.href='forumgroups.aspx';" type="button">...</button>
				    &nbsp;&nbsp;
				    <asp:Label ID="lblEnterGroup" CssClass="gray" runat="server" EnableViewState="False" meta:resourcekey="lblEnterGroupResource1">or enter a new value:</asp:Label>
				    <asp:TextBox id="tbForumGroup" runat="server"></asp:TextBox></td>
		    </tr>
		    <tr><td></td><td><asp:Button CssClass="gradientbutton" id="btnAdd" runat="server" Text="add" onclick="btnAdd_Click" meta:resourcekey="btnAddResource1"></asp:Button></td></tr>
	    </table>
    </p>
	<asp:Label ID="lblError" runat="server" Visible="False" EnableViewState="False" ForeColor="Red" meta:resourcekey="lblErrorResource1">Error: a forum was not created. Please fill all the fields.</asp:Label>
</asp:Content>