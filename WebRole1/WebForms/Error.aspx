<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true"
    Inherits="Error" Title="R8R: Error" Codebehind="Error.aspx.cs" %>

<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    Error
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <span class="mainPresentationText">Sorry, R8R encountered an unexpected error.</span>
    <br /> <br />
    <asp:Button runat="server" ID="ReturnToHomePage" Text="Return to Home Page" CssClass="BtnBig"
        PostBackUrl="~" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentUpdatingEachPostBack" runat="Server">
</asp:Content>
