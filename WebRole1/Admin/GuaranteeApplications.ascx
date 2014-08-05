<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="GuaranteeApplications.ascx.cs" Inherits="WebRole1.Admin.GuaranteeApplications" %>

<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls" Assembly="ClassLibrary1" %>
<%@ Register TagPrefix="Uc" TagName="LiteralElement" Src="~/CommonControl/LiteralElement.ascx" %>
    <table id="CommentsTable" width="100%" class="mainTable mainTableFont mainTableSmall mainTableWithBorders mainTableHeadingLarge"
            cellspacing="0" border="0">
            <tr>
                <th class="nmcl">
                    #
                </th>
                <th>
                    Username
                </th>
                <th>
                    Download
                </th>
                <th>
                    Approve
                </th>
                <th>
                    Reject
                </th>
            </tr>
        <asp:LinqDataSource ID="MainLinqDataSource" runat="server" ContextTypeName="UserRatingDataAccess"
            OnSelecting="MainLinqDataSource_Selecting">
        </asp:LinqDataSource>
            <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
                DataKeyNames="FileName,UserID" OnDataBinding="MainListView_DataBinding"  OnItemCreated="MainListView_ItemCreated"
                OnItemDataBound="MainListView_ItemDataBound" 
                 >
                <EmptyDataTemplate>
                    <tr>
                        <td colspan="99">
                            No guarantee applications.
                        </td>
                    </tr>
                </EmptyDataTemplate>
                <LayoutTemplate>
                    <tr id="itemPlaceholder" runat="server">
                    </tr>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="maintr">
                        <td>
                            <SNFWC:DataItemCounter ID="DataItemCounter1" IndexFormat="{0}. " IndexOffset="1"
                                runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="Username" runat="server" Text='<% #Eval("Username") %>' />
                        </td>
                        <td>
                            <asp:LinkButton ID="DownloadBtn" runat="server" Text="Download"  OnClick="Download_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="PaymentLabel" runat="server" Text="Payment:"  />
                            <asp:TextBox ID="Amount" runat="server" Width="50" />
                            <asp:Button ID="ApproveBtn" runat="server" Text="Approve"  OnClick="Approve_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Button ID="RejectBtn" runat="server" Text="Reject"  OnClick="Reject_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
            <tr id="GuaranteeApplicationsPager">
                <td colspan="5" class="pagerRow">
                    <asp:Panel runat="server" ID="pagerFieldPanel">
                        <div>
                            <asp:DataPager ID="Pager" runat="server" PagedControlID="MainListView" PageSize="5">
                                <Fields>
                                    <SNFWC:GooglePagerField NextPageImageUrl="~/Images/button_arrow_right.gif" PreviousPageImageUrl="~/Images/button_arrow_left.gif" />
                                </Fields>
                            </asp:DataPager>
                        </div>
                    </asp:Panel>
                </td>
            </tr>
        </table>