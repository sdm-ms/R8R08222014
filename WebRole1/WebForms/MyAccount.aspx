<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="MyAccount" Title="Rateroo: My Account" Codebehind="MyAccount.aspx.cs" %>

<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    My Account
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <asp:Panel ID="MyAccountInfo" runat="server" DefaultButton="SubmitChanges">
        <a id="changepwdlink" runat="server" href="~/ChangePwd">Change Password</a></asp:Panel>
        <br />
        <table class="accountInfo">
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server">First Name</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="FirstName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server">Last Name</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="LastName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label11" runat="server">Email Address</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="EMailAddress" runat="server"></asp:TextBox>
                </td>
                <td>
                    <span style="font-size:12px; font-weight:normal; font-style:italic;">This must be a valid address to receive payment through PayPal.</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server">Address (Line 1)</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="Address1" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server">Address (Line 2)</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="Address2" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server">City</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="City" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label6" runat="server">State/Region</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="State" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server">Zip Code (or Postal Code)</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ZipCode" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label8" runat="server">Country</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="Country" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label9" runat="server">Home Phone</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="HomePhone" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label10" runat="server">Work Phone</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="WorkPhone" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="CancelChanges" runat="server" Text="Cancel" CssClass="BtnBig" OnClick="CancelChanges_Click" />
        &nbsp; &nbsp;
        <asp:Button ID="SubmitChanges" runat="server" Text="Submit Changes" CssClass="BtnBig" OnClick="SubmitChanges_Click" />
        <br />
</asp:Content>
