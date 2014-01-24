<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="ForgotPwd" Title="Rateroo: Recover Password" Codebehind="ForgotPwd.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SupplementalScripts" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BoxTopLeftContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentHeadText" Runat="Server">
    Reset Password
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentMain" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentUpdatingEachPostBack" Runat="Server">
    <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" BorderStyle="None" BackColor="white"
        BorderWidth="0px" BorderColor="Black" ForeColor="Black" Font-Size="16pt" Font-Names="Trebuchet MS, Arial, Helvetica, Sans-Serif"
        BorderPadding="4" UserNameRequiredErrorMessage="Username is required." 
        GeneralFailureText="An error occurred. Try again." 
        QuestionFailureText="Incorrect answer." 
        UserNameFailureText="Account not found." SuccessPageUrl="~" 
        onsendingmail="PasswordRecovery1_SendingMail" 
        onsendmailerror="PasswordRecovery1_SendMailError" 
        onuserlookuperror="PasswordRecovery1_UserLookupError"  >
        <UserNameTemplate>
            <table border="0" cellpadding="4" cellspacing="0" style="border-collapse: collapse;">
                <tr>
                    <td>
                        <table border="0" cellpadding="0">
                            <tr>
                                <td align="left" colspan="2" style="font-size:14px;">
                                    Enter your username to receive a new password by e-mail.
                                    <br /> <br />
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Username:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                        ErrorMessage="Username is required." ToolTip="Username is required." ValidationGroup="PasswordRecovery1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" style="color: Red;">
                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" colspan="2">
                                    <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Submit" 
                                        ValidationGroup="PasswordRecovery1" CssClass="BtnBig" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </UserNameTemplate>
    </asp:PasswordRecovery>
</asp:Content>

