<%@ Control Language="C#" AutoEventWireup="true" Inherits="GoogleMap" Codebehind="GoogleMap.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="Ajax"%>
<%@ Register Assembly="GMaps" Namespace="Subgurim.Controles" tagprefix="Google" %>

<table ID="GoogleMapTable" runat="server" width="100%">
    <tr>
        <td align="center">
            <div id="Gdiv" runat="server">
            <Google:GMap ID="GM" runat="server" Width="100%" Height="200px" />
            </div>
        </td>
    </tr>
</table>



