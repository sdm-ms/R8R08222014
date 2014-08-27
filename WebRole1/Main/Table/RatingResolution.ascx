<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Main_Table_RatingGroupResolution" Codebehind="RatingResolution.ascx.cs" %>
<%@ Register Src="~/CommonControl/DateAndTime.ascx" TagName="DateAndTime" TagPrefix="Uc" %>
<%@ Register Src="~/CommonControl/ModalPopup.ascx" TagName="ModalPopUp" TagPrefix="Uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Ajax" %>
<br />
<asp:Panel ID="TitlePanel" runat="server" CssClass="collapsePanelHeader" Visible="false"> <%--Change Visible and uncomment at bottom of page to make this collapsible--%>
    <div style="padding: 5px; cursor: pointer; vertical-align: middle;">
        <div style="float: left;">
            <asp:Label ID="ConcludeInfoShort" runat="server" Text="" CssClass="collapsePanelHeaderText">
            </asp:Label>
        </div>
        <div style="float: left; margin-left: 20px;">
            <asp:Label ID="Label1" runat="server" CssClass="collapsePanelHeaderText">(Show Details...)</asp:Label>
        </div>
        <div style="float: right; vertical-align: middle;">
            <asp:ImageButton ID="Image1" runat="server" ImageUrl="~/images/expand_blue.jpg" AlternateText="(Show Details...)" />
        </div>
    </div>
</asp:Panel>
<br />
<asp:Panel ID="MainPanel" runat="server" CssClass="collapsePanel">
    <asp:Label ID="ConcludeInfo" runat="server" CssClass="standardOptionsView"></asp:Label>
    <br />
    <br />
    <div id="undoConcludeRegion" runat="server">
        <asp:Button ID="UndoConclude" runat="server" OnClick="UndoConclude_Click" Text="Undo Conclude"
            CssClass="BtnBig" />
    </div>
    <div id="concludeRegion" runat="server">
        <asp:Button ID="Conclude" runat="server" OnClick="Conclude_Click" Text="Conclude"
            CssClass="BtnBig" />
        <asp:RadioButtonList ID="TimingOptions" runat="server" CssClass="standardOptionsView">
            <asp:ListItem ID="NowOption" Selected="True" Text="As of now">
            </asp:ListItem>
            <asp:ListItem ID="OtherTimeOption" Selected="False" Text="As of specific time:">
            </asp:ListItem>
        </asp:RadioButtonList>
        <Uc:DateAndTime ID="TheDateAndTime" runat="server" CssClass="standardOptionsView" />
        <br />
        <span><i>Ratings should generally be concluded only where the true value has become
            settled (for example, where the rating is a forecast of whether a team will win
            a sports event). When ratings are resolved as of a specific time (for example, the
            instant a sports event becomes settled), later ratings will receive zero points. Users
            who resolve concluded ratings may receive bonus points for doing so.</i></span>
        <br />
        <br />
        <br />
        <asp:RadioButtonList ID="PointsRule" runat="server" CssClass="standardOptionsView">
            <asp:ListItem ID="NormalAssignment" Selected="True" Text="Points should be based on table cell as of now.">
            </asp:ListItem>
            <asp:ListItem ID="Unwind" Selected="False" Text="All pending ratings should receive zero points, because the rating was cancelled or became inapplicable at the time specified above.">
            </asp:ListItem>
        </asp:RadioButtonList>
    </div>
</asp:Panel>
<%--<Ajax:CollapsiblePanelExtender ID="CollapsiblePanel" runat="server" TargetControlID="MainPanel"
    CollapseControlID="TitlePanel" ExpandControlID="TitlePanel" Collapsed="true"
    TextLabelID="Label1" CollapsedText="(Show Status Details)" ExpandedText="(Hide Status Details)"
    ImageControlID="Image1" ExpandedImage="~/images/collapse_blue.jpg" CollapsedImage="~/images/expand_blue.jpg" BehaviorID="statusPanelBehavior">
</Ajax:CollapsiblePanelExtender>--%>
<Uc:ModalPopUp runat="server" ID="PopUp" />
