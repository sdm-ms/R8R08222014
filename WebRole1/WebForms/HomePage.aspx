<%@ Page Language="C#" MasterPageFile="~/MasterPages/SlideshowPage.master" AutoEventWireup="true"
    Inherits="_Default" Title="Rateroo" Codebehind="HomePage.aspx.cs" %>
<%@ OutputCache Duration="1200" VaryByParam="none" VaryByCustom="noPostbackDefault" %>
<%@ Register Src="~/CommonControl/PMSlideshow.ascx" TagName="Slideshow" TagPrefix="Uc" %>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentHeadTextAndMain" runat="Server">
    <Uc:Slideshow ID="MainSlideshow" runat="server" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentUpdatingEachPostBack" runat="Server">
</asp:Content>
