<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true"
    Inherits="NewUser" Title="R8R: Create New Account" Codebehind="NewUser.aspx.cs" %>

<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    Create New Account
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" BackColor="White" BorderColor="White"
        BorderStyle="None" BorderWidth="0px" ForeColor="Black" Font-Names="Trebuchet MS,Arial,Helvetica,Sans-Serif"
        Font-Size="16px" Width="100%" UserNameLabelText="Username:" OnCreatedUser="CreateUserWizard1_CreatedUser"
        OnCreatingUser="CreateUserWizard1_CreatingUser" CreateUserButtonType="Link" CreateUserButtonText="">
        <WizardSteps>
            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server" Title="Enter Account Information">
                <ContentTemplate>
                    <asp:Panel ID="CreateUserWizardPanel" runat="server" DefaultButton="StepNextButton">
                        <table class="mainPresentationText">
                            <tr style="height: 50px;">
                                <td >
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" CssClass="mainPresentationText">
                        Username:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="UserName" runat="server" CssClass="createUserInput"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                        CssClass="createUserError" ErrorMessage="Username is required." ToolTip="User Name is required."
                                        ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr style="height: 50px;">
                                <td >
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="mainPresentationText">
                        Password:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="createUserInput"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                        CssClass="createUserError" ErrorMessage="Password is required." ToolTip="Password is required."
                                        ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr style="height: 50px;">
                                <td >
                                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword"
                                        CssClass="mainPresentationText">
                        Confirm Password:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" CssClass="createUserInput"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                                        CssClass="createUserError" ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
                                        ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr style="height: 50px;">
                                <td >
                                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" CssClass="mainPresentationText">
                        E-mail:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="Email" runat="server" CssClass="createUserInput"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                        CssClass="createUserError" ErrorMessage="E-mail is required." ToolTip="E-mail is required."
                                        ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regEmail" ControlToValidate="Email" ValidationExpression=".*@.*\..*"
                                        ValidationGroup="CreateUserWizard1" ErrorMessage="You must enter a valid e-mail address."
                                        CssClass="createUserError" runat="server">
                                    </asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                        ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
                                        ValidationGroup="CreateUserWizard1" CssClass="createUserError"></asp:CompareValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" class="createUserError">
                                    <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext"
                                        Text="Create Account" ValidationGroup="CreateUserWizard1" CssClass="BtnBig" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server" OnActivate="CompleteWizardStep1_Activate">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="ContinuePanel" DefaultButton="ContinueButton">
                        <table class="mainPresentationText">
                            <tr>
                            </tr>
                            <tr>
                                <td>
                                    Your account has been successfully created.<br />
                                    <br />
                                    Before you receive any winnings, you will need to fill<br />
                                    in more information under the <a runat="server" href="MyAccount">My Account</a> link.
                                    <br />
                                    Continued use is subject to acceptance of the <a runat="server" href="Rules">Terms of Service</a>.
                                    
                                </td>
                            </tr>
                            <tr>
                                <td align="left" colspan="2">
                                    &nbsp;<asp:Button ID="ContinueButton" runat="server" Text="Continue" PostBackUrl="~/HomePage.aspx"
                                        CssClass="BtnBig" />
                                </td>
                            </tr>
                            <tr style="height: 20px;">
                            </tr>
                            <tr>
                                <td style="font-size: 12px;">
                                    <i>By using or creating an account on R8R, you agree to be bound by the <a runat="server" id="TermsAndCond"
                                        href="~/TermsOfService">Terms of Service</a>. </i>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
</asp:Content>
