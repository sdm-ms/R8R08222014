<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_DollarSubsidy_DollarSubsidyInfo" Codebehind="DollarSubsidyInfo.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
 
  <asp:Panel ID="DollarinfoContent" runat="server" Style="display: none;" Width="520px" Height="650px" CssClass="modalPopup">
 <table width="100%" border="0" cellspacing="0" cellpadding="0">
<tr>
<td colspan="3" height="13"></td>
</tr>
  <tr>
    <td width="14" valign="top"><img src=  "../../images/left-corner.jpg"   alt="left-corner" width="14" height="62" /></td>
    <td valign="top"><table width="100%" border="0" cellspacing="0" cellpadding="0">
      <tr>
        <td valign="top">&nbsp;</td>
      </tr>
      <tr>
        <td valign="top" class="headTxt">Dollar Subsidy Information</td>
      </tr>
      
     
      <tr>
        <td valign="top">&nbsp;</td>
      </tr>
     
      <tr>
        <td valign="top" > 
       <table align="center" class="border">

<tr>
<td align="center">

    <asp:GridView ID="DollarInfoGrid" runat="server" AllowPaging="True"  GridLines="Both"
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="2" 
        ForeColor="#333333" BorderColor="Black" BorderStyle="Solid" 
        BorderWidth="1px" onpageindexchanging="DollarInfoGrid_PageIndexChanging">
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <RowStyle BackColor="#EFF3FB" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#2461BF" />
        <AlternatingRowStyle BackColor="White" />
        <Columns>
        <asp:TemplateField>
        <HeaderTemplate>
        PointsManager Name
        <Uc:Hover runat="server" Id="HoverPointsManagerName" MsgString="Name of the PointsManager"></Uc:Hover>
        </HeaderTemplate>
        <ItemTemplate>
        <asp:Label ID="Label2" runat="server" Text='<%# Eval("PointsManagerName") %>'></asp:Label>
        
        </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField >
        <HeaderTemplate>
       User Name
         <Uc:Hover runat="server" Id="HoverUserName" MsgString="Name of the User"></Uc:Hover>
        </HeaderTemplate>
        <ItemTemplate>
        <asp:Label ID="Label1" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
      
        </ItemTemplate>
        </asp:TemplateField>
       
        <asp:TemplateField >
        <HeaderTemplate>
    Email
         <Uc:Hover runat="server" Id="HoverEmail" MsgString="Email Id of user"></Uc:Hover>
        </HeaderTemplate>
        <ItemTemplate>
        <asp:Label ID="Label2" runat="server" Text='<%# Eval("Email") %>'></asp:Label>
      
        </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField>
        <HeaderTemplate>
        Address
         <Uc:Hover runat="server" Id="HoverAddress" MsgString="Address of user"></Uc:Hover>
        </HeaderTemplate>
        <ItemTemplate>
        <asp:Label ID="Label3" runat="server" Text='<%# Eval("Address") %>'></asp:Label>
        </ItemTemplate>
        </asp:TemplateField>
       
        <asp:TemplateField>
        <HeaderTemplate>
    Amount
         <Uc:Hover runat="server" Id="HoverAmount" MsgString="Amount recieved by user"></Uc:Hover>
        </HeaderTemplate>
        <ItemTemplate>
        <asp:Label ID="Label3" runat="server" Text='<%# Eval("Amount") %>'></asp:Label>
      
        </ItemTemplate>
        </asp:TemplateField>
       
        <asp:TemplateField >
        <HeaderTemplate>
        Recieved Date
         <Uc:Hover runat="server" Id="HoverRecievedDate" MsgString="Recieved date of amount"></Uc:Hover>
        </HeaderTemplate>
        <ItemTemplate>
        <asp:Label ID="Label3" runat="server" Text='<%# Eval("RecievedDate") %>'></asp:Label>
      
        </ItemTemplate>
        </asp:TemplateField>
        </Columns>
    </asp:GridView>
   
   

</td>
</tr>
<tr><td align="center">
   
    <Uc:ModalPopUp ID="PopUp" runat="server" />
    </td></tr>
</tr>
</table>
       
      </td>
      </tr>
      
     
    </table>
    </td>
    <td width="14" valign="top"><img src= "../../images/right-corner.jpg" alt="right-corner" width="14" height="62" /></td>
  </tr>
</table>

</asp:Panel>

<asp:LinkButton ID="EmptyButton" runat="server" Text="" Enabled="false" Visible="true"></asp:LinkButton>
<Ajax:ModalPopupExtender ID="DollarinfoExtender" runat="server" TargetControlID="EmptyButton"
    PopupControlID="DollarinfoContent" DropShadow="true" 
    BackgroundCssClass="modalBackground">
</Ajax:ModalPopupExtender>
