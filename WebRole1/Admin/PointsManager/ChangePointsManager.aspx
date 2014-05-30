<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true" Inherits="ChangePointsManager" Title="R8R" Codebehind="ChangePointsManager.aspx.cs" %>

<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="AnnounceTab" Src="~/Admin/Announcements/MainAnnouncetab.ascx" %>
<%@ Register TagPrefix="Uc" TagName="DollarTab" Src="~/Admin/DollarSubsidy/DollarSubsidy.ascx" %>
<%@ Register TagPrefix="Uc" TagName="GrantUserRights" Src="~/Admin/GrantUserRights/GrantUserRights.ascx" %>
<%@ Register TagPrefix="Uc" TagName="RewardRatingSetting" Src="~/Admin/RewardRatingSetting.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Guarantees" Src="~/Admin/Guarantees.ascx" %>
<%@ Register TagPrefix="Uc" TagName="GuaranteeApplications" Src="~/Admin/GuaranteeApplications.ascx" %>
<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    Administer Points 
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <Ajax:TabContainer ID="universemanager" runat="Server" Width="600">
                <Ajax:TabPanel runat="server" ID="AnnounceTab" HeaderText="Announcements">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="Univerupdate" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Uc:AnnounceTab ID="announce" runat="server" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="announce" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
                <Ajax:TabPanel runat="server" ID="DollSubsidy" HeaderText="Dollar Subsidy">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="dollarupdate" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Uc:DollarTab runat="server" ID="DollarSubs" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
                
                <Ajax:TabPanel runat="server" ID="Guarantee" HeaderText="Guarantees">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="UpdatePanel4" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Uc:Guarantees runat="server" ID="Guarantees1" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
                <Ajax:TabPanel runat="server" ID="GuaranteeApplications" HeaderText="Guarantee Applications">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="UpdatePanel5" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Uc:GuaranteeApplications runat="server" ID="GuaranteeApplications1" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
                <Ajax:TabPanel runat="server" ID="RewardRating" HeaderText="Reward Rating">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Uc:RewardRatingSetting runat="server" ID="RewardRating1" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
                <Ajax:TabPanel runat="server" ID="UserRights" HeaderText="User Rights">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Uc:GrantUserRights runat="server" ID="GrantUserRights" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </Ajax:TabPanel>
            </Ajax:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
