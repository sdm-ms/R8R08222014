<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Admin_Tbl_user_control_changeTblImportExport" Codebehind="changeTblImportExport.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<%@ Register TagPrefix="Uc" TagName="Hover" Src="~/CommonControl/Hover.ascx" %>
<%@ Register TagPrefix="Uc" TagName="ModalPopUp" Src="~/CommonControl/ModalPopUp.ascx" %>
<%@ Register TagPrefix="Uc" TagName="PMFieldsBox" Src="~/Main/Field/PMFieldsBox.ascx" %>
<%@ Reference Control="~/Main/Field/PMFieldsBox.ascx" %>

<table>
    <tr>
        <td>
            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="BtnDownLoadXSD" />
                </Triggers>
                <ContentTemplate>
                    <asp:Button ID="BtnDownLoadXSD" runat="server" OnClick="BtnDownLoadXSD_Click" Text="Download XML schema (XSD) file"
                        CausesValidation="false" CssClass="Btn1" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="BtnConvert" />
                </Triggers>
                <ContentTemplate>
                    <asp:FileUpload runat="server" ID="ConvertFileUpload" EnableViewState="true" CssClass="inp" />
                    <asp:Button ID="BtnConvert" CssClass="Btn1" runat="server" Text="Convert Excel file to XML file"
                        OnClick="BtnConvert_Click" CausesValidation="false" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:UpdatePanel ID="ImportUpdatePanel" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="BtnImport" />
                </Triggers>
                <ContentTemplate>
                    <asp:FileUpload runat="server" ID="XMLFileUpload" EnableViewState="true" CssClass="inp" />
                    <asp:Button ID="BtnImport" CssClass="Btn1" runat="server" Text="Import entities from XML file"
                        OnClick="BtnImport_Click" CausesValidation="false" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="BtnExport" />
                </Triggers>
                <ContentTemplate>
                    <asp:Button ID="BtnExport" runat="server" Text="Export entities to XML file" CssClass="Btn1"
                        OnClick="BtnExport_Click" CausesValidation="false" />
                    <asp:CheckBox runat="server" ID="ChkInActivate" Text="Include inactivated entities" />
                    <asp:CheckBox runat="server" ID="ChkIncValues" Text="Include item values" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td>
            <Uc:ModalPopUp ID="TblRowPopUp" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <%--Export TblRow to XML file Start--%>
            <table class="border" style="width:100%;">
                <tr>
                    <td >
                        <div style="width:300px; position:relative; padding:0 0 0 0;">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <Uc:PMFieldsBox runat="server" ID="FieldsBox" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
                <tr>

                </tr>
                <tr>
                    <td>
<%--                        <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:PostBackTrigger ControlID="Button1" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:FileUpload runat="server" ID="FileUpload1" EnableViewState="true" />
                                <asp:Button ID="Button2" runat="server" Text="Import" OnClick="Button1_Click" />
                                <asp:Button ID="Button3" runat="server" Text="Create Xml File" OnClick="Button2_Click"
                                    CausesValidation="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>--%>
                    </td>
                </tr>
            </table>
            <%--Export TblRow to XML file ends--%>
        </td>
    </tr>
</table>
