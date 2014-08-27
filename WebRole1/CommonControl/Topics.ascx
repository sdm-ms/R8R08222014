<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommonControl_Topics" Codebehind="Topics.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="HeadTextBackgroundEffect" Src="~/CommonControl/HeadTextBackgroundEffect.ascx" %>
<Uc:HeadTextBackgroundEffect ID="TheBackgroundEffect" runat="server" />
<div class="headTxt headTxtEffect">
    Topics
</div>
<div class="afterHeadTxtEffect">
    <div id="TopicsCell" runat="server">
        <asp:PlaceHolder runat="server" ID="Place1"></asp:PlaceHolder>
    </div>
</div>
