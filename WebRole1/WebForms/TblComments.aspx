<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Codebehind="TblComments.aspx.cs" Inherits="WebApplication1.TblComments" %>

<%@ Register TagPrefix="Uc" TagName="ItemPath" Src="~/CommonControl/ItemPath.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Comments" Src="~/Main/Table/Comments.ascx" %>
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
    <Uc:Comments runat="Server" ID="CommentsContent" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentUpdatingEachPostBack" runat="server">
</asp:Content>
