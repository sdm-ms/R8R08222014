<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Main_Table_TableCellView" Codebehind="TableCellView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellColumnHeading" Src="~/Main/Table/ViewCellColumnHeading.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellMainUnselected" Src="~/Main/Table/ViewCellMainUnselected.ascx" %>
<%@ Register TagPrefix="Uc" TagName="HeaderRow" Src="~/Main/Table/HeaderRow.ascx" %>
<%@ Register TagPrefix="Uc" TagName="HeaderRowOnTblRowPage" Src="~/Main/Table/HeaderRowOnTblRowPage.ascx" %>
<%@ Register TagPrefix="Uc" TagName="BodyRow" Src="~/Main/Table/BodyRow.ascx" %>
<%@ Register TagPrefix="Uc" TagName="FieldsDisplay" Src="~/Main/Table/FieldsDisplay.ascx" %>
<%@ Register TagPrefix="Uc" TagName="RatingOverTimeGraph" Src="~/CommonControl/RatingOverTimeGraph.ascx" %>
<%@ Register TagPrefix="Uc" TagName="RecentRatings" Src="~/Main/Table/RecentRatings.ascx" %>
<%@ Register TagPrefix="Uc" TagName="RatingGroupResolution" Src="~/Main/Table/RatingResolution.ascx" %> 
<table border="0" width="50%" cellspacing="1" cellpadding="3">
    <tr style="height:20px;">
    </tr>
    <tr>
        <td>
            <table width="100%" class="mainTable mainTableWithBorders">
                <tr>
                    <asp:PlaceHolder ID="HeaderRowPlaceHolder" runat="server"></asp:PlaceHolder>
                </tr>
                <tr>
                    <asp:PlaceHolder ID="BodyRowPlaceHolder" runat="server"></asp:PlaceHolder>
                </tr>
            </table>
        </td>
    </tr>
    <tr style="height:20px;">
    </tr>
</table>
<Ajax:TabContainer ID="TblManager" runat="Server" ActiveTabIndex="0" Width="600" >
    <Ajax:TabPanel runat="Server" ID="General" HeaderText="Chart">
        <ContentTemplate>
            <table border="0" width="100%" cellspacing="1" cellpadding="3">
                <tr style="height: 20px;">
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="ChartUpdatePanel" runat="server">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="ChartPlaceHolder" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </Ajax:TabPanel>
    <Ajax:TabPanel runat="Server" ID="TabPanel1" HeaderText="Recent">
        <ContentTemplate>
            <table border="0" width="100%" cellspacing="1" cellpadding="3">
                <tr>
                    <td>
                        <table border="0" width="100%" cellspacing="1" cellpadding="3">
                            <tr>
                                <td>
                                    <Uc:RecentRatings ID="RecentRatingsTable" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </Ajax:TabPanel>
    <Ajax:TabPanel runat="Server" ID="TabPanel3" HeaderText="Info">
        <ContentTemplate>
            <table border="0" width="100%" cellspacing="1" cellpadding="3">
                <tr>
                    <td>
                        <div runat="server" id="FieldsDisplayDiv" class="entityPageFields">
                            <asp:PlaceHolder ID="FieldsDisplayPlaceHolder" runat="server"></asp:PlaceHolder>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </Ajax:TabPanel>
    <Ajax:TabPanel runat="Server" ID="TabPanel2" HeaderText="Status">
        <ContentTemplate>
            <table border="0" width="100%" cellspacing="1" cellpadding="3">
                <tr>
                </tr>
                <tr>
                    <td>
                        <asp:PlaceHolder ID="ResolveRatingsPlaceHolder" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </Ajax:TabPanel>
</Ajax:TabContainer>  



