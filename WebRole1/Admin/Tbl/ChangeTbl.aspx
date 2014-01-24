<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="ChangeTbl" Title="Rateroo" Codebehind="ChangeTbl.aspx.cs" %>
 
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register TagPrefix="Uc" TagName="changeTblannounce" Src="~/Admin/Announcements/MainAnnouncetab.ascx" %>
<%@ Register TagPrefix="Uc" TagName="changeTblimport" Src="~/Admin/Tbl/user control/changeTblImportExport.ascx" %>
<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    Administer Table
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <asp:UpdatePanel ID="CollectAdminUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <Ajax:TabContainer ID="TblManager" runat="Server" ActiveTabIndex="0" Width="600">
                <Ajax:TabPanel runat="server" ID="AnnounceTab" HeaderText="Announcements">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="Collectupdate" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Uc:changeTblannounce ID="changeTblannounce" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
                <Ajax:TabPanel runat="server" ID="TabImport" HeaderText="Import/Export">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="IEUpdate">
                            <ContentTemplate>
                                <Uc:changeTblimport ID="changeTblimport" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
            </Ajax:TabContainer>
            <a ID="AdministerPointsManager" runat="server">Change Points, Subsidy, and User Rights</a>
            &nbsp;
            <Uc:ModalPopUp runat="server" ID="PopUp" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
