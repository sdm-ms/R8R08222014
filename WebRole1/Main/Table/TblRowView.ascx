<%@ Control Language="C#" AutoEventWireup="true" Inherits="Main_Table_TblRowView" Codebehind="TblRowView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="ViewCellTopLeftCorner" Src="~/Main/Table/ViewCellTopLeftCorner.ascx" %>
<%@ Register TagPrefix="Uc" TagName="HeaderRow" Src="~/Main/Table/HeaderRow.ascx" %>
<%@ Register TagPrefix="Uc" TagName="HeaderRowOnTblRowPage" Src="~/Main/Table/HeaderRowOnTblRowPage.ascx" %>
<%@ Register TagPrefix="Uc" TagName="BodyRow" Src="~/Main/Table/BodyRow.ascx" %>
<%@ Register TagPrefix="Uc" TagName="RecentRatings" Src="~/Main/Table/RecentRatings.ascx" %>
<%@ Register TagPrefix="Uc" TagName="FieldsDisplay" Src="~/Main/Table/FieldsDisplay.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<%@ Register TagPrefix="Uc" TagName="Comments" Src="~/Main/Table/Comments.ascx" %>
<table border="0" width="100%" cellspacing="1" cellpadding="3">
    <tr>
        <td class="possibleBottom">
            <Ajax:TabContainer ID="TblManager" runat="Server" ActiveTabIndex="0" Width="600"
                OnClientActiveTabChanged="resizePageVertically">
                <Ajax:TabPanel runat="Server" ID="CommentsTab" HeaderText="Comments">
                    <ContentTemplate>
                        <Uc:Comments runat="Server" ID="CommentsContent" />
                    </ContentTemplate>
                </Ajax:TabPanel>
                <Ajax:TabPanel runat="Server" ID="InfoTab" HeaderText="Info">
                    <ContentTemplate>
                        <div runat="server" id="FieldsDisplayDiv" class="entityPageFields">
                            <asp:PlaceHolder ID="FieldsDisplayPlaceHolder" runat="server"></asp:PlaceHolder>
                            <br />
                            <a id="BtnChangeTblRow" runat="server">Change Information</a>
                        </div>
                    </ContentTemplate>
                </Ajax:TabPanel>
                <Ajax:TabPanel runat="Server" ID="RatingsTab" HeaderText="Current">
                    <ContentTemplate>
                        <asp:LinqDataSource ID="TblRowLinqDataSource" runat="server" OnSelecting="TblRowLinqDataSource_Selecting">
                        </asp:LinqDataSource>
                        <asp:ListView runat="server" ID="TblRowListView" DataSourceID="TblRowLinqDataSource"
                            DataKeyNames="TblTabID,TblTabName" OnDataBinding="TblRowListView_DataBinding"
                            OnItemCreated="TblRowListView_ItemCreated" OnItemDataBound="TblRowListView_ItemDataBound">
                            <LayoutTemplate>
                                <table width="100%">
                                    <tr id="itemPlaceholder" runat="server">
                                    </tr>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>                
                                        <table border="0" width="100%" cellspacing="1" cellpadding="3">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="LblTblTab" runat="server" cssClass="TblTabName"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <Uc:LiteralElement ID="TableInItemTemplate" runat="server" ElementType="table" OpeningTag="true" ClosingTag="false"
                                                        MarkupAttributeType1="width"
                                                        MarkupAttributeContent1="100%" />
                                                        <tr>
                                                            <asp:PlaceHolder ID="HeaderRowPlaceHolder" runat="server"></asp:PlaceHolder>
                                                        </tr>
                                                        <tr>
                                                            <asp:PlaceHolder ID="BodyRowPlaceHolder" runat="server"></asp:PlaceHolder>
                                                        </tr>
                                                    <Uc:LiteralElement ID="TableInItemTemplateClose" runat="server" ElementType="table" OpeningTag="false" ClosingTag="true" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
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
                <Ajax:TabPanel runat="Server" ID="StatusTab" HeaderText="Status">
                    <ContentTemplate>
                        <asp:Label runat="server" ID="DeletionStatus"></asp:Label>
                        <asp:Button runat="server" ID="DeleteTblRow" Text="Delete" 
                            onclick="DeleteTblRow_Click" CssClass="BtnBig" />
                        <asp:Button runat="server" ID="UndeleteTblRow" Text="Undelete" OnClick="UndeleteTblRow_Click"
                            CssClass="BtnBig" />
                    </ContentTemplate>
                </Ajax:TabPanel>
            </Ajax:TabContainer>
        </td>
    </tr>
    <tr>
        <td>
        </td>
    </tr>
</table>
