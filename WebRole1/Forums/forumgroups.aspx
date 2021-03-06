<%@ Page Language="C#" MasterPageFile="AspNetForumMaster.Master" AutoEventWireup="true" CodeBehind="forumgroups.aspx.cs" Inherits="aspnetforum.forumgroups" Title="Untitled Page" %>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">
    <div class="location"><img alt="" src="images/admin.gif" />&nbsp;
	<b><a href="admin.aspx"><asp:Label ID="lblAdmin" runat="server" EnableViewState="False">Administrator</asp:Label></a>
	&raquo;
	<asp:Label ID="lblEditForumGroups" runat="server" EnableViewState="False">Editing forum categories</asp:Label></b></div>
	
	<asp:DataGrid id="gridForumGroups" runat="server" AutoGenerateColumns="False" Width="100%"
		ShowHeader="False" BorderColor="Silver" CellPadding="7" OnEditCommand="gridForumGroups_EditCommand" OnCancelCommand="gridForumGroups_CancelCommand" OnUpdateCommand="gridForumGroups_UpdateCommand" OnItemCommand="gridForumGroups_ItemCommand">
		<AlternatingItemStyle BackColor="Lavender" />
		<Columns>
			<asp:BoundColumn ReadOnly="true" DataField="GroupID" HeaderText="GroupID" Visible="false"></asp:BoundColumn>
			<asp:BoundColumn DataField="GroupName" HeaderText="GroupName"></asp:BoundColumn>
            <asp:EditCommandColumn CancelText="Cancel" EditText="Edit" UpdateText="Update"></asp:EditCommandColumn>
			<asp:ButtonColumn Text="delete" CommandName="delete"></asp:ButtonColumn>
		</Columns>
	</asp:DataGrid>
	
	<asp:TextBox id="tbForumGroup" runat="server"></asp:TextBox>
	<asp:Button runat="server" ID="btnAdd" Text="+" onclick="btnAdd_Click" />
</asp:Content>
