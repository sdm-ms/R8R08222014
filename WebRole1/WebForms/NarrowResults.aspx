<%@ Page Language="C#" AutoEventWireup="true" Inherits="NarrowResults" Codebehind="NarrowResults.aspx.cs" %>

<%@ Register TagPrefix="Uc" TagName="PMFieldsBox" Src="~/Main/Field/PMFieldsBox.ascx" %>
<%@ Reference Control="~/Main/Field/PMFieldsBox.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/StyleSheet.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnablePartialRendering="true" runat="server">
    </asp:ScriptManager>
    <div style="width: 230px; margin: 10px 5px 10px 10px; padding: 0px 0px 0px 0px;">
        <input type="hidden" id="narrowResultsCount" runat="server" />
        <div style="display:none;" id="FilterRulesInfo" runat="server" >
            <asp:Literal ID="LiteralFilterRules" runat="server"></asp:Literal>
        </div>
        <Uc:PMFieldsBox runat="server" ID="FieldsBox" />
    </div>
    </form>
</body>
</html>
