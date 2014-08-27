<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="WebRole1.WebForms.AdminResetEmail" Title="R8R: Reset Email" Codebehind="AdminResetEmail.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SupplementalScripts" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BoxTopLeftContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentHeadText" Runat="Server">
    Reset Email
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentMain" Runat="Server">
    <asp:Label ID="Label1"
        runat="server" Text="Username of account:"></asp:Label>
    <asp:TextBox ID="UserNameToReset" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="Label2"
        runat="server" Text="Address to set to:"></asp:Label>
    <asp:TextBox ID="NewEmailAddress" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="ErrorMsg" runat="server" Visible="false" Text="User not found." ForeColor="Red"></asp:Label>
    <asp:Button ID="SetBtn" runat="server" Text="Set Email Address" 
        OnClick="SetEmailAddress_Click" />
    <asp:Button ID="Unlock" runat="server" Text="Unlock User Without Changing Address" 
        OnClick="UnlockUser_Click" />

    
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentUpdatingEachPostBack" Runat="Server">
</asp:Content>
