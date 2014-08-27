<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_ViewCellAdministrativeOptions" Codebehind="ViewCellAdministrativeOptions.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>

<td>
<asp:ImageButton ID="ImageButton1" ImageUrl="~/images/Downward_Triangle.bmp"
    runat="server" Height="10" Width="10"></asp:ImageButton>
<Ajax:DropDownExtender runat="server" ID="DDE" TargetControlID="ImageButton1" DropDownControlID="AdminPanel">
</Ajax:DropDownExtender>
<asp:Panel ID="AdminPanel" runat="server">
    <asp:LinkButton ID="LinkBtnViewSettings" runat="server" Text="View Settings" 
        onclick="LinkBtnViewSettings_Click"></asp:LinkButton>
    <asp:LinkButton ID="LinkBtnViewUserRating" runat="server" Text="View UserRatings"
        OnClick="LinkBtnViewUserRating_Click"></asp:LinkButton>
    <asp:LinkButton ID="LinkBtnResolve" runat="server" Text="Rating Resolution"
        onclick="LinkBtnResolve_Click"></asp:LinkButton>
</asp:Panel>
</td>

