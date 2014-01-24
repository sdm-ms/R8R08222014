<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Admin_Announcements_AddInsertableContents" Codebehind="AddInsertableContents.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="PopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Buttons" Src="~/CommonControl/PageButtons.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<asp:Panel ID="AnnouncePopupContent" runat="server" Style="display: none;" Width="520px"
    Height="390px" CssClass="modalPopup">
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <tr>
            <td width="14" valign="top">
            </td>
            <td valign="top" align="center">
                <table border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td  align="left" valign="top" id="Tdannouc" runat="server" class="trebu21redtxt brdrbottom">
                            Create New Announcements
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <table class="border" border="0" cellpadding="1" cellspacing="1">
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:RadioButton ID="RButtonEveryWhere" runat="server" Visible="false" GroupName="rdgWhere"
                                            Text="EveryWhere" OnCheckedChanged="RButtonEveryWhere_CheckedChanged" class="browncolor" />
                                        &nbsp;&nbsp;<asp:RadioButton ID="RButtonLocation" Checked="true" runat="server" GroupName="rdgWhere"
                                            Text="Specific Location" OnCheckedChanged="RButtonLocation_CheckedChanged" class="browncolor"  />
                                    </td>
                                </tr>
                                <tr id="ListRow" runat="Server">
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="browncolor" >
                                        Name*
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="TxtName" runat="server" CssClass="inp" MaxLength="50"></asp:TextBox>
                                        <Uc:Hover ID="Hover1" runat="server" MsgString="Enter Name" />
                                        <asp:RequiredFieldValidator ID="NameRequiredFieldValidator" Display="None" ControlToValidate="TxtName"
                                            runat="server" ErrorMessage="<b>Required Field Missing</b><br />A name is required."></asp:RequiredFieldValidator>
                                        <Ajax:ValidatorCalloutExtender ID="NameValidate" runat="server" TargetControlID="NameRequiredFieldValidator">
                                        </Ajax:ValidatorCalloutExtender>
                                    </td>
                                </tr>
                                 <tr  runat="Server">
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="browncolor"  valign="top">
                                        Content*
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="TxtContent" runat="server" TextMode="MultiLine" CssClass="inp" Height="100"
                                            Width="400" TabIndex="1"></asp:TextBox>
                                        <Uc:Hover ID="Hover2" runat="server" MsgString="Enter Content of Announcement" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="None" ControlToValidate="TxtContent"
                                            runat="server" ErrorMessage="<b>Required Field Missing</b><br />Content of announcement is required."></asp:RequiredFieldValidator>
                                        <Ajax:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" TargetControlID="RequiredFieldValidator1">
                                        </Ajax:ValidatorCalloutExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:CheckBox ID="ChkIncludeHtml" runat="server" Text="Content includes html" TabIndex="3"
                                            class="browncolor"  />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:CheckBox ID="ChkOverridable" runat="server" Text="Overridable" TabIndex="4"
                                           class="browncolor"  />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:CheckBox ID="ChkActivated" runat="server" Text="Activated" class="browncolor"  />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <Uc:Buttons ID="Buttons" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:LinkButton ID="EmptyButton" runat="server" Text="" Enabled="false" Visible="true"></asp:LinkButton>
<Ajax:ModalPopupExtender ID="AnnounceExtender" runat="server" TargetControlID="EmptyButton"
    PopupControlID="AnnouncePopupContent" DropShadow="true" BackgroundCssClass="modalBackground">
</Ajax:ModalPopupExtender>
