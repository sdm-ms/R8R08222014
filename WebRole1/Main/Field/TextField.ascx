<%@ Control Language="C#" AutoEventWireup="true" Inherits="TextFieldFilter" Codebehind="TextField.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<asp:UpdatePanel runat="server" ID="TextUpdatePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="borderless">
            <Uc:LiteralElement ID="TextRowOpening" runat="server" ElementType="tr" OpeningTag="true"
                ClosingTag="false" />
                <Uc:LiteralElement ID="TextLabelOpening" runat="server" ElementType="td" OpeningTag="true"
                    ClosingTag="false" />
                    <asp:Label ID="LabelBeforeTextBox" runat="server" Text="Text&nbsp;"></asp:Label>
                <Uc:LiteralElement ID="TextLabelClosing" runat="server" ElementType="td" OpeningTag="false"
                    ClosingTag="true" />
                <Uc:LiteralElement ID="TextCellOpening" runat="server" ElementType="td" OpeningTag="true"
                    ClosingTag="false" />
                    <asp:TextBox ID="TxtMain" runat="server" ></asp:TextBox>
                <Uc:LiteralElement ID="TextCellClosing" runat="server" ElementType="td" OpeningTag="false"
                    ClosingTag="true" />
            <Uc:LiteralElement ID="TextRowClosing" runat="server" ElementType="tr" OpeningTag="false"
                ClosingTag="true" />
            <Uc:LiteralElement ID="LinkRowOpening" runat="server" ElementType="tr" OpeningTag="true"
                ClosingTag="false" />
                <Uc:LiteralElement ID="LinkLabelOpening" runat="server" ElementType="td" OpeningTag="true"
                ClosingTag="false" />
                    <asp:Label ID="LinkText" runat="server" Text="Link&nbsp;"></asp:Label>
                <Uc:LiteralElement ID="LinkLabelClosing" runat="server" ElementType="td" OpeningTag="false"
                ClosingTag="true" />
                <Uc:LiteralElement ID="LinkCellOpening" runat="server" ElementType="td" OpeningTag="true"
                    ClosingTag="false" />
                    <asp:TextBox ID="LinkTextBox" runat="server"></asp:TextBox>
                <Uc:LiteralElement ID="LinkCellClosing" runat="server" ElementType="td" OpeningTag="false"
                    ClosingTag="true" />
            <Uc:LiteralElement ID="LinkRowClosing" runat="server" ElementType="tr" OpeningTag="false"
                ClosingTag="true" />
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
