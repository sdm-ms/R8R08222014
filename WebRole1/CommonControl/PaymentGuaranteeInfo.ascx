<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentGuaranteeInfo.ascx.cs" Inherits="WebRole1.CommonControl.PaymentGuaranteeInfo" %>

<%--<asp:UpdatePanel ID="PGIUpdate" runat="server">
    <ContentTemplate>--%>
        <div style="display:inline;">
            <asp:Literal ID="CurrentInfo" runat="server" />
            <asp:Literal ID="DocumentationUploadInstructions" runat="server" />
            <asp:FileUpload ID="DocumentationUpload" runat="server" Width="300" />
            <asp:Button ID="Apply" runat="server" Text="Apply" 
                onclick="Apply_Click"/>
            <asp:Label ID="ErrorMessage" runat="server" ForeColor="Red" />
        </div>
<%--    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="Apply" />
    </Triggers>
</asp:UpdatePanel>--%>