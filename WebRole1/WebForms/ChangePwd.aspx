<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="Default2" Title="Rateroo: Change Password" Codebehind="ChangePwd.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    Rateroo: Change Password
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentHeadText" Runat="Server">
    Change Password
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentMain" Runat="Server">
    <asp:ChangePassword ID="ChangePassword1" runat="server"  
        BorderStyle="None" BackColor="white"
        BorderWidth="0px" BorderColor="Black" ForeColor="Black" 
        Font-Size="16pt" Font-Names="Trebuchet MS, Arial, Helvetica, Sans-Serif"
        ContinueDestinationPageUrl="~/MyAccount" ChangePasswordTitleText="" 
        PasswordRecoveryText="Forgot password" PasswordRecoveryUrl="~/ForgotPwd" 
        SuccessText="Your password has been changed."
        >
        <ChangePasswordButtonStyle CssClass="BtnBig" />
        <CancelButtonStyle CssClass="BtnBig" />
        <ContinueButtonStyle CssClass="BtnBig" />
        
        <TextBoxStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="16px" />
        <LabelStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="16px"
            Font-Bold="true" HorizontalAlign="Left" />
        <FailureTextStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="16px"
            HorizontalAlign="Left" />
        <HyperLinkStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="14px" />
    </asp:ChangePassword>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentUpdatingEachPostBack" Runat="Server">
</asp:Content>

