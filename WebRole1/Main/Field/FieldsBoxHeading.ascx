﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="FieldsBoxHeading" Codebehind="FieldsBoxHeading.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="HeadTextBackgroundEffect" Src="~/CommonControl/HeadTextBackgroundEffect.ascx" %>
<uc:headtextbackgroundeffect id="TheBackgroundEffect" runat="server" />
<div class="headTxt headTxtEffect">
    Narrow Results
</div>
<asp:Literal ID="myLiteral" runat="server" Text='<div class=\"afterHeadTxtEffect\">' />
