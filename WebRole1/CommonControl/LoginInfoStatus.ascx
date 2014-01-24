<%@ Control Language="C#" AutoEventWireup="true" Inherits="LoginInfoStatus"
    CodeBehind="LoginInfoStatus.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<asp:LoginView ID="MyLoginView" runat="server">
    <AnonymousTemplate>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <asp:LoginName ID="MyLoginStatus" runat="Server" FormatString="Welcome, {0}!" />
        &nbsp; | &nbsp;
    </LoggedInTemplate>
</asp:LoginView>
<asp:HyperLink ID="LoginLink" runat="server" NavigateUrl="~/Login">Login</asp:HyperLink>
<asp:HyperLink ID="LogoutLink" runat="server" NavigateUrl="~/Logout">Logout</asp:HyperLink>
<div id="loginSep2" runat="server" style="display:inline;" >
&nbsp; | &nbsp;
</div>
<asp:HyperLink ID="CreateNewUserLink" runat="server" NavigateUrl="~/NewUser">Create Free Account</asp:HyperLink>
<asp:Literal ID="TheUserAccessInfo" runat="server"></asp:Literal>
