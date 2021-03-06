﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="FieldsBox" Codebehind="FieldsBox.ascx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="FieldsBoxHeading" Src="~/Main/Field/FieldsBoxHeading.ascx" %>
<%@ Reference Control="~/Main/Field/FieldsBoxHeading.ascx" %>
<%@ Register TagPrefix="Uc" TagName="AnyField" Src="~/Main/Field/AnyField.ascx" %>
<%@ Register TagPrefix="Uc" TagName="SearchWordsFilter" Src="~/Main/Field/SearchWordsFilter.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
<table>
<tbody>
<tr>
    <td colspan="2" valign="top">
        <asp:Panel ID="PanelAroundFilterBox" runat="server" DefaultButton="BtnAction">
                <asp:PlaceHolder ID="HeadingPlaceHolder" runat="server"></asp:PlaceHolder>
                <table id="SurroundingTable" runat="server" width="100%" border="0" cellpadding="0"
                    cellspacing="0">
                    <tbody>
                    <tr>
                        <td colspan="3" valign="top">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="0" class="filterTable">
                                        <Uc:LiteralElement ID="ValidationErrorRow1" runat="server" ElementType="tr" OpeningTag="true"
                                            ClosingTag='false' />
                                            <td>
                                                <asp:CustomValidator ID="FieldsValidator" runat="server" 
                                                    OnServerValidate="ValidateFields"></asp:CustomValidator>
                                            </td>
                                            <Uc:LiteralElement ID="ValidationErrorRow2" runat="server" ClosingTag="true" 
                                                ElementType="tr" OpeningTag="false" />
                                            <Uc:LiteralElement ID="EntityNameSelector" runat="server" ClosingTag="false" 
                                                ElementType="tr" OpeningTag="true" />
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Label ID="EntityNameLabel" runat="server" CssClass="filterFieldName" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="EntityName" runat="server" CssClass="setFieldsTextBox" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <Uc:LiteralElement ID="EntityNameSelector2" runat="server" ClosingTag="true" 
                                                ElementType="tr" OpeningTag="false" />
                                            <asp:PlaceHolder ID="SearchWordsPlaceHolder" runat="server">
                                            </asp:PlaceHolder>
                                            <asp:ListView ID="FieldsBoxListViewFields" runat="server" 
                                                DataKeyNames="FieldDefinitionID,FieldName,FieldType,FieldNum" 
                                                DataSourceID="LinqDataSourceFields" 
                                                OnDataBinding="FieldsBoxListViewFields_DataBinding" 
                                                OnItemCreated="FieldsBoxListViewFields_ItemCreated" 
                                                OnItemDataBound="FieldsBoxListViewFields_ItemDataBound">
                                                <LayoutTemplate>
                                                    <tr ID="itemPlaceholder" runat="server">
                                                    </tr>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <tr class="borderless">
                                                        <td class="borderless">
                                                            <table class="borderlessSomeSpace">
                                                                <tr class="borderlessSomeSpace">
                                                                    <td colspan="2">
                                                                        <asp:Label ID="FieldName" runat="server" CssClass="filterFieldName" 
                                                                            Text="<%# ((ClassLibrary1.Model.FieldDefinitionInfo)Container.DataItem).FieldName %>">
                                                                </asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr class="borderlessSomeSpace">
                                                                    <td class="borderlessSomeSpace">
                                                                        <Uc:AnyField ID="FilterField" runat="server" 
                                                                            FieldInfo="<%# ((ClassLibrary1.Model.FieldDefinitionInfo)Container.DataItem) %>" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </tr>
                                    </table>
                                    <asp:ListView runat="server" ID="FieldsBoxListViewColumns" DataSourceID="LinqDataSourceColumns"
                                        DataKeyNames="TblColumnID,TblColumnName,DefaultSortOrderAsc,Sortable"
                                        OnDataBinding="FieldsBoxListViewColumns_DataBinding" OnItemCreated="FieldsBoxListViewColumns_ItemCreated"
                                        OnItemDataBound="FieldsBoxListViewColumns_ItemDataBound">
                                        <LayoutTemplate>
                                            <table cellspacing="0" class="filterTable">
                                                <tr id="itemPlaceholder" runat="server">
                                                </tr>
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr class="borderless">
                                                <td class="borderless">
                                                    <table>
                                                    <tbody>
                                                        <tr class="borderlessSomeSpace">
                                                            <td colspan="2">
                                                                <asp:Label runat="server" ID="FieldName" Text='<%# ((ClassLibrary1.Model.TblColumnInfo)Container.DataItem).TblColumnName %>'
                                                                    CssClass="filterFieldName">
                                                                </asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr class="borderlessSomeSpace">
                                                            <td class="borderless">
                                                                <Uc:AnyField runat="server" ID="FilterField" CatDesInfo='<%# ((ClassLibrary1.Model.TblColumnInfo)Container.DataItem) %>' />
                                                            </td>
                                                        </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                    <asp:LinqDataSource ID="LinqDataSourceFields" runat="server" ContextTypeName="UserRatingDataAccess"
                                        OnSelecting="LinqDataSourceFields_Selecting">
                                    </asp:LinqDataSource>
                                    <asp:LinqDataSource ID="LinqDataSourceColumns" runat="server" ContextTypeName="UserRatingDataAccess"
                                        OnSelecting="LinqDataSourceColumns_Selecting">
                                    </asp:LinqDataSource>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="BtnCancel" runat="server" Text="Cancel" OnClick="BtnCancel_Click" CssClass="BtnBig" />
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdateNarrowResults" runat="server">
                                <ContentTemplate>
                                    <asp:Button ID="BtnAction" runat="server" Text="Narrow Results" CssClass="BtnBig" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td>
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdateNarrowResults"
                                DisplayAfter="100">
                                <ProgressTemplate>
                                    <asp:Image ID="RefreshPageMoving2" runat="server" ImageUrl="~/images/Button-Refresh-Moving.gif"
                                        class="pageRefreshBtn2" />
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </td>
                        <td></td>
                    </tr>
                    </tbody>
                </table>
        </asp:Panel>
    </td>
</tr>
<tr style="height:30px;">
    <td colspan="2" valign="top">
        <asp:CheckBox ID="ShowDeletedItems" runat="server" Text="Show deleted rows" CssClass="filterFieldName" />
    </td>
</tr>
<tr style="height:30px;">
    <td colspan="2" valign="top">
        <asp:CheckBox ID="HighStakesOnly" runat="server" Text="Show peer review rows only" CssClass="filterFieldName" />
    </td>
</tr>

</tbody>
</table>
