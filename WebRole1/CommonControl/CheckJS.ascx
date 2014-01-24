<%@ Control Language="C#" AutoEventWireup="true" Inherits="CheckJS" Codebehind="CheckJS.ascx.cs" %>
<asp:HiddenField ID="hfClientJSEnabled" runat="server" Value="False" />

<script type="text/javascript">
    document.getElementById('<%= hfClientJSEnabled.ClientID %>').value = "True";
    if (document.getElementById('<%= hfClientJSEnabled.ClientID %>').value != '<%= IsJSEnabled %>')
    {        
        window.location.href= '<%= GetAppendedUrl(JSQRYPARAM, JSENABLED) %>';
    }
</script>

