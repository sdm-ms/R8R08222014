<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="MainAnnounceTab" Codebehind="MainAnnouncetab.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register TagPrefix="Uc" TagName="AnnounPopUp" Src="~/Admin/Announcements/AddInsertableContents.ascx" %>
<%--<%@ Register TagPrefix
="Uc" TagName="announcepopup" Src= "~/Admin/Announcements/AddInsertableContents.ascx"%>--%>
<asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="border" border="0">
            <tr>
                <td>
                    <asp:PlaceHolder runat="server" ID="AnnouncePlaceHolder">
                        <table border="0">
                            <tr>
                                <td style="height:25px">
                                    <asp:Button ID="BtnNewAnnouncement" runat="server" Text="New Announcement" CausesValidation="false"
                                        OnClick="BtnNewAnnouncement_Click" CssClass="Btn1" />
                                </td>
                                <td>
                                    <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel3" runat="server">
                                        <ProgressTemplate>
                                            <asp:Image ID="ImageProgress" runat="server" ImageUrl="~/images/progress_48.gif"
                                                Height="23px" Width="27px" />
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                </td>
                            </tr>
                        </table>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="AnnouncePopupPlaceHolder" runat="server"></asp:PlaceHolder>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
