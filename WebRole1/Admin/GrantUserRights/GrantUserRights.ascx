<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GrantUserRights.ascx.cs" Inherits="WebApplication1.Admin.GrantUserRights.GrantUserRights" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="Ajax"%>

<table class="border">

<tr><td colspan="2">
    <asp:Label ID="existingUsers" runat="server"></asp:Label>
        </td>
            
   </tr><tr><td colspan="2">
    <asp:Label ID="Label1" runat="server" Text="Enter users to authorize or deauthorize below:"></asp:Label>
        </td>
            
   </tr>
  <tr><td>
  
  <asp:TextBox ID="usernames" runat="server" Width="400" TextMode="MultiLine"></asp:TextBox>
     
   </td>
        <td>

            <asp:Button ID="Authorize" runat="server" Text="Authorize" 
                OnClick="Authorize_Click" CausesValidation="false" /> 
            <asp:Button ID="Deauthorize" runat="server" Text="Deauthorize" OnClick="Deauthorize_Click" CausesValidation="false" /> 
        </td>
            
   </tr>
 </table>
