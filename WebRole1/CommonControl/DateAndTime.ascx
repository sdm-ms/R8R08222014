<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommonControl_DateAndTime" Codebehind="DateAndTime.ascx.cs" %>
<%@ Reference Control="~/CommonControl/Date.ascx" %>
<%@ Reference Control="~/CommonControl/Time.ascx" %>
<%@ Register Src="~/CommonControl/Date.ascx" TagPrefix="Uc" TagName="Date" %>
<%@ Register Src="~/CommonControl/Time.ascx" TagPrefix="Uc" TagName="Time" %>
<Uc:Date ID="TheDate" runat="server" />
<Uc:Time ID="TheTime" runat="server" />
