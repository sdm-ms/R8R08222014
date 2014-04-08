<%@ Control Language="C#" AutoEventWireup="true" Inherits="ChoiceFieldFilter" Codebehind="ChoiceField.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
    <asp:UpdatePanel runat="server" ID="UpdatePanelAroundChoiceGroup" UpdateMode="Conditional">
        <ContentTemplate>
            <input id="filterDepend" runat="server" type="hidden" class="filterDependency" />
            <asp:DropDownList ID="DdlChoice" runat="server" OnSelectedIndexChanged="DdlChoice_SelectedIndexChanged"
                AutoPostBack="true" CssClass="filterDropDown">
            </asp:DropDownList>
            <asp:LinqDataSource ID="LinqDataSourceChoices" runat="server" ContextTypeName="DataAccess"
                OnSelecting="LinqDataSourceChoices_Selecting" />
            <asp:ListView ID="ListMultipleChoices" runat="server" DataSourceID="LinqDataSourceChoices">
                <LayoutTemplate>
                    <br />
                    <table>
                        <tr id="itemPlaceHolder" runat="server">
                        </tr>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="ChoiceName" Text='<%# ((ClassLibrary1.Model.ChoiceMenuItem)Container.DataItem).Text %>'
                                CssClass="choiceNameList">
                            </asp:Label>
                        </td>
                        <td>
                            <asp:ImageButton ID="DeleteButton" runat="server" ImageUrl="~/images/cancel.gif"
                                OnCommand="DeleteButton_Click"
                                CommandName="DeleteItem" CommandArgument='<%# ((ClassLibrary1.Model.ChoiceMenuItem)Container.DataItem).Value %>' />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </ContentTemplate>
    </asp:UpdatePanel>
