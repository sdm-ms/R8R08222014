<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true"
    Inherits="Login" Title="R8R: Login" Codebehind="Login.aspx.cs" %>

<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    Log In
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <table class="borderless" style="width:200px;">
        <tr >
            <td style="height:13px;">
                <asp:Panel ID="panelLogin" runat="server" DefaultButton="Login1$LoginButton">
                    <asp:Login ID="Login1" runat="server" BorderStyle="None" BackColor="white" BorderWidth="0px"
                        BorderColor="Black" ForeColor="Black" Font-Size="16pt" Font-Names="Trebuchet MS, Arial, Helvetica, Sans-Serif"
                        CreateUserText="Create a new user..." CreateUserUrl="~/NewUser" PasswordRecoveryUrl="~/ForgotPwd"
                        UserNameLabelText="Username:" OnLoggedIn="OnLoggedIn" OnLoggingIn="OnLoggingIn" OnLoginError="OnLoginError"
                        TitleText="" PasswordRecoveryText="Recover lost password..." RememberMeText="Remember me on this computer"
                        UserNameRequiredErrorMessage="Username is required." FailureText="Login unsuccessful. Try again." LabelStyle-VerticalAlign="Top">
                        <CheckBoxStyle Font-Size="12px" Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" />
                        <TextBoxStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="16px" />
                        <LoginButtonStyle CssClass="BtnBig" />
                        <LabelStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="16px" Font-Bold="true" />
                        <FailureTextStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="16px"
                            HorizontalAlign="Left" />
                        <HyperLinkStyle Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif" Font-Size="14px" />
                    </asp:Login>
                </asp:Panel>
            </td>
        </tr>
        <tr style="height:20px;">
        </tr>
        <tr>
            <td style="font-size:12px;">
                <i>By using R8R, you agree to be bound by the <a runat="server" id="TermsAndCond"
                    href="~/TermsOfService">Terms of Service</a>. </i>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentUpdatingEachPostBack" runat="Server">
</asp:Content>
