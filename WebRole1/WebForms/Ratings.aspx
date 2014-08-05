<%@ Page Language="C#" MasterPageFile="~/MasterPages/StandardPage.master" AutoEventWireup="true"
    Inherits="Ratings" Title="R8R: Ratings" Codebehind="Ratings.aspx.cs" %>

<%@ Register TagPrefix="SNFWC" Namespace="SqlNetFrameworkWebControls"
    Assembly="ClassLibrary1" %>
<%@ Register TagPrefix="Uc" TagName="ItemPath" Src="~/CommonControl/ItemPath.ascx" %>

<asp:Content ID="MyContentHead" ContentPlaceHolderID="ContentHeadText" runat="Server">
    <asp:Label id="whoseRatings" runat="server"></asp:Label>
    
</asp:Content>
<asp:Content ID="MyContentMain" ContentPlaceHolderID="ContentMain" runat="server">
    <%--<script type="text/javascript" src="<%=ResolveUrl("~/js/FontSize.js") %>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            setFontSize('5px');
        });
    </script>--%>
    <table >
        <tr style="height:11px;">
            <td></td>
        </tr>
    </table>
    <asp:LinqDataSource ID="MainLinqDataSource" runat="server" ContextTypeName="UserRatingDataAccess"
        OnSelecting="MainLinqDataSource_Selecting" AutoPage="false">
    </asp:LinqDataSource>
    <table id="maint" width="100%" class="mainTable mainTableFont mainTableWithBorders mainTableSmall mainTableHeadingLarge" cellspacing="0"
        border="0">
        <tr>
            <th class="nmcl">
                #
            </th>
            <th>
                Item
            </th>
            <th>
                Date
            </th>
            <th>
                Time
            </th>
            <th >
                Rating
            </th>
            <th>
                Points
            </th>
        </tr>
        <asp:ListView runat="server" ID="MainListView" DataSourceID="MainLinqDataSource"
            DataKeyNames="" OnDataBinding="MainListView_DataBinding" OnItemCreated="MainListView_ItemCreated"
            OnItemDataBound="MainListView_ItemDataBound" >
            <EmptyDataTemplate>
                <tr>
                    <td colspan="99">
                        Sorry. No ratings found. Ratings you entered may take a few minutes to appear.
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
                        <asp:PlaceHolder ID="PathToItem" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Date", "{0:d}")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Date", "{0:t}")%>' />
                    </td>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text='<% #FormatRatingAll(Container.DataItem) %>' />
                    </td>
                    <td>
                        <asp:Label ID="Label6" runat="server" Text='<% #FormatPointsAll(Container.DataItem)%>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <tr id="PagerRow">
            <td colspan="99" class="pagerRow">
                <asp:Panel runat="server" ID="pagerFieldPanel">
                    <div>
                        <asp:DataPager ID="Pager" runat="server" PagedControlID="MainListView" PageSize="10">
                            <Fields>
                                <SNFWC:GooglePagerField NextPageImageUrl="~/Images/button_arrow_right.gif"
                                    PreviousPageImageUrl="~/Images/button_arrow_left.gif" />
                            </Fields>
                        </asp:DataPager>
                    </div>
                </asp:Panel>
            </td>
        </tr>
        <tr id="asteriskRow">
            <td colspan="99" class="asteriskRow">
                <span><i>* Subject to change based on recent or future ratings.</i></span>
            </td>
        </tr>
        <tr id="highStakesOnly">
            <td colspan="99" class="asteriskRow">
                <asp:CheckBox ID="NotHighStakesCheckbox" runat="server" 
                    Text="Show regular ratings (least at stake)" 
                    AutoPostBack="true"
                    oncheckedchanged="NotHighStakesCheckbox_CheckedChanged" />
                <br />
                <asp:CheckBox ID="HighStakesKnownCheckbox" runat="server" 
                    Text="Show ratings made when already picked for peer review (more at stake)" 
                    AutoPostBack="true"
                    oncheckedchanged="HighStakesKnownCheckbox_CheckedChanged"/>
                <br />
                <asp:CheckBox ID="HighStakesPreviouslySecretCheckbox" runat="server" 
                    Text="Show ratings later randomly picked for peer review (most at stake)" 
                    AutoPostBack="true"
                    oncheckedchanged="HighStakesPreviouslySecretCheckbox_CheckedChanged"/>
            </td>
        </tr>
        <tr id="mimicRow" runat="server">
            <td colspan="99" class="mimicRevertRatings">
                <span>These buttons allow you to mimic or revert untrusted single ratings by this user that other users have not already changed.</span>
                <br />
                <span>Choosing either of these will result in new ratings being added to your account.</span>
                <br />
                <asp:Button ID="Mimic" runat="server" Text="Mimic Untrusted Ratings" 
                    onclick="Mimic_Click" />
                &nbsp;
                <asp:Button ID="Revert" runat="server" Text="Revert Untrusted Ratings" 
                    onclick="Revert_Click" />
                &nbsp;
                <asp:Button ID="RevertTrusted" runat="server" Visible="false" Text="Revert Ratings Even if Trusted" 
                    onclick="RevertTrusted_Click" />
            </td>
        </tr>
        <tr id="userTimeRow" runat="server">
            <td colspan="99" class="mimicRevertRatings">
                <span>Time user has spent in last day: </span><asp:Label ID="TimeSpan1" runat="server"></asp:Label><br />
                <span>Time user has spent in last three days: </span><asp:Label ID="TimeSpan2" runat="server"></asp:Label><br />
                <span>Time user has spent in last week: </span><asp:Label ID="TimeSpan3" runat="server"></asp:Label><br />
                <span>Time user has spent in last four weeks: </span><asp:Label ID="TimeSpan4" runat="server"></asp:Label><br />
            </td>
        </tr>
    </table>
</asp:Content>
