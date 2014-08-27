<%@ Control Language="C#" AutoEventWireup="true" Inherits="AngledText" Codebehind="AngledText.ascx.cs" %>
<%@ Register Assembly="Microsoft.Web.GeneratedImage" Namespace="Microsoft.Web" TagPrefix="cc1" %>
<cc1:GeneratedImage ID="GeneratedImage1"
    runat="server" ImageHandlerUrl="~/RotatedTextHandler.ashx">            
    <Parameters>
    <%--These settings are overriden in code--%>
        <cc1:ImageParameter Name="TheFontSize" Value="12" />
        <cc1:ImageParameter Name="TheText" Value="Default Text"/>
        <cc1:ImageParameter Name="TheAngle" Value="90" />
        <cc1:ImageParameter Name="TheFontName" Value="Trebuchet MS" />
    </Parameters>         
</cc1:GeneratedImage>