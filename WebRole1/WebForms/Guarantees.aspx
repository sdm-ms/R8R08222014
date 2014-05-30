<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" CodeBehind="Guarantees.aspx.cs" Title="R8R: Guaranteed Payments" Inherits="WebRole1.WebForms.Guarantees" %>
<%@ Register TagPrefix="Uc" TagName="ItemPath" Src="~/CommonControl/ItemPath.ascx" %>
<%@ Register TagPrefix="Uc" TagName="PaymentGuaranteeInfo" Src="~/CommonControl/PaymentGuaranteeInfo.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SupplementalScripts" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BoxTopLeftContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentHeadText" runat="server">
    <Uc:ItemPath ID="ItemPath1" runat="server"></Uc:ItemPath>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentMain" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentUpdatingEachPostBack" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentOutsideOfUpdatePanel1" runat="server">
    <Uc:PaymentGuaranteeInfo ID="PaymentGuaranteeInfo1" runat="server" />
</asp:Content>
