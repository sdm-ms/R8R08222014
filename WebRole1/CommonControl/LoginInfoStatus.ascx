<%@ Control Language="C#" AutoEventWireup="true" Inherits="LoginInfoStatus"
    CodeBehind="LoginInfoStatus.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<asp:Label ID="loggedInUser" runat="server"></asp:Label>
<%--<asp:LoginView ID="MyLoginView" runat="server">
    <AnonymousTemplate>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <asp:LoginName ID="MyLoginStatus" runat="Server" FormatString="Welcome, {0}!" />
        &nbsp; | &nbsp;
    </LoggedInTemplate>
</asp:LoginView>--%>
<%--<a href="Account/Login" id="Loginbtn">Login</a>
<a href="Account/LogOff" id="Logoutbtn" style="display:none;">Logout</a>--%>
<asp:HyperLink ID="LoginLink" runat="server" NavigateUrl="~/Account/Login">Login</asp:HyperLink>
<asp:HyperLink ID="LogoutLink" runat="server" NavigateUrl="~/Account/LogOff">Logout</asp:HyperLink>
<div id="loginSep2" runat="server" style="display:inline;" >
&nbsp; | &nbsp;
</div>
<%--<a href="Account/Register" id="CreateNewUserbtn">Create Free Account</a>--%>
<asp:HyperLink ID="CreateNewUserLink" runat="server" NavigateUrl="~/Account/Register">Create Free Account</asp:HyperLink>
<asp:Literal ID="TheUserAccessInfo" runat="server"></asp:Literal>
