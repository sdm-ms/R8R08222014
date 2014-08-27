<%@ Page language="c#" Codebehind="allusers.aspx.cs" AutoEventWireup="True" Inherits="aspnetforum.allusers" MasterPageFile="AspNetForumMaster.Master" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
    <div class="location">
	<h2><a href="default.aspx"><asp:Label ID="lblHome" runat="server" EnableViewState="False" meta:resourcekey="lblHomeResource1">Home</asp:Label></a>
	&raquo;
	<asp:Label ID="lblListUsers" runat="server" EnableViewState="False" meta:resourcekey="lblListUsersResource1">List of all users</asp:Label></h2></div>
	<script type="text/javascript">
	    function Search() {
	        var tbId = '<%= tbUsername.ClientID %>';
	        document.location.href = 'allusers.aspx?q='+document.getElementById(tbId).value;
	    }
	</script>
	<div><asp:TextBox ID="tbUsername" runat="server"></asp:TextBox>
	<button type="button" onclick="Search();"><asp:Label ID="lblSearch" runat="server" EnableViewState="False" meta:resourcekey="lblSearchResource1">search username</asp:Label></button>
	</div>
	<br />
    <table width="100%" cellpadding="4"><tr>
    <th></th>
    <th><a href="allusers.aspx"><asp:Label ID="lblUser" runat="server" EnableViewState="False" meta:resourcekey="lblUserResource1">Username</asp:Label></a></th>
    <th><a href="allusers.aspx?order=email">Email</a></th>
    <th><a href="allusers.aspx?order=regdate"><asp:Label ID="lblRegSince" runat="server" EnableViewState="False" meta:resourcekey="lblRegSinceResource1">Registered since</asp:Label></a></th>
    <th><a href="allusers.aspx?order=posts"><asp:Label ID="lblPosts" runat="server" EnableViewState="False" meta:resourcekey="lblPostsResource1">Posts</asp:Label></a></th>
    <th><a href="allusers.aspx?order=logondate"><asp:Label ID="lblLogonDate" runat="server" EnableViewState="False" meta:resourcekey="lblLogonDateResource1">Last logon date</asp:Label></a></th>
    <th></th></tr>
    <asp:repeater id="rptUsersList" runat="server" EnableViewState="False">
    <ItemTemplate>
    <tr>
        <td><a href='viewprofile.aspx?UserID=<%# Eval("UserID") %>'><img src='<%# aspnetforum.Utils.User.GetAvatarFileName(Eval("AvatarFileName")) %>' width="25" height="25" alt="<%# Eval("Username") %>" /></a></td>
	    <td><a href='viewprofile.aspx?UserID=<%# Eval("UserID") %>'>
	        <%# aspnetforum.Utils.User.GetUserDisplayName(Eval("UserName"), Eval("FirstName"), Eval("LastName"))%>
	    </a></td>
	    <td><%# ShowEmail(Eval("Email")) %></td>
	    <td><%# Eval("RegistrationDate") %></td>
	    <td><a href='viewpostsbyuser.aspx?UserID=<%# Eval("UserID") %>'><%# Eval("PostsCount") %></a></td>
	    <td><%# Eval("LastLogonDate") %></td>
	    <td>
	        <% if (IsAdministrator)
            { %>
                <input type="checkbox" name="cbDel<%# Eval("UserID") %>" />
	        <% } %>
	    </td>
    </tr>
    </ItemTemplate>
    </asp:repeater>
    <tr><th colspan="5">
	    <asp:Label ID="lblPager" runat="server" EnableViewState="False" meta:resourcekey="lblPagerResource1">pages:</asp:Label>
	    <%= pagerString %></th>
	    <th><asp:ImageButton ID="btnDel" ToolTip="delete selected" runat="server" ImageUrl="images/delete.png" OnClick="btnDel_Click" OnClientClick="return confirm('are you sure?')"></asp:ImageButton></th>
	    <th></th></tr>
    </table>
</asp:Content>