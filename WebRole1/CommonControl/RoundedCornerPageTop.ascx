<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommonControl_RoundedCornerPageTop" Codebehind="RoundedCornerPageTop.ascx.cs" %>
<%@ Register TagPrefix="Uc" TagName="LoginInfoStatus" Src="~/CommonControl/LoginInfoStatus.ascx" %>
<asp:ScriptManagerProxy ID="MyScriptManagerProxy" runat="server">
        <Scripts>
        </Scripts>
</asp:ScriptManagerProxy>
<div id="topOfRoundedCorner" class="pageTopRoundedCorner">
    <table id="backbox" class="borderless pageTopBackBox">
        <tr id="backbox-top">
            <td style="width:15px; height: 15px; background-image: url(/images/backbox-top-left.gif);" >
            </td>
            <td style="background-image: url(/images/backbox-top-center.png); background-repeat: repeat-x;">
            </td>
            <td style="width:15px; background-image: url(/images/backbox-top-right.gif);" >
            </td>
        </tr>
        <tr id="backbox-center">
            <td style="width:15px; height: 80px; background-image: url(/images/backbox-center.png);" >
            </td>
            <td style="width:912px;height:80px; background-image: url(/images/backbox-center.png); background-repeat: repeat;">
            </td>
            <td style="width:15px; height: 80px; background-image: url(/images/backbox-center.png);" >
            </td>
        </tr>
        <tr id="backbox-bottom">
            <td style="width:15px; height: 15px; background-image: url(/images/backbox-bottom-left.gif);" >
            </td>
            <td style="background-image: url(/images/backbox-bottom-center.png); background-repeat: repeat-x;">
            </td>
            <td style="width:15px; background-image: url(/images/backbox-bottom-right.gif);" >
            </td>
        </tr>
    </table>
    <div class="alphaVersion">
        <asp:Image ID="alpha" runat="server" class="reppng" ImageUrl="~/images/alpha.png" />
    </div>
    <div class="pageTopLogo">
        <a id="ImageButtonAnchor" runat="server">
            <asp:Image ID="ImageButton1" runat="server" ImageUrl="~/images/top_logo.gif" />
        </a>
    </div>
    <div class="pageTopSearchBox">
        <asp:Panel ID="PanelAroundSearchBox" runat="server" DefaultButton="DoSearch">
            <asp:TextBox ID="SearchBox" runat="server" ClientIDMode="Static" CssClass="pageTopSearchTextBox"></asp:TextBox>
            <asp:ImageButton ID="DoSearch" runat="server" CssClass="pageTopSearchButton reppng"  
                ImageUrl="~/images/go-normal.png" OnClick="DoSearch_Click" AlternateText="Search" />
        </asp:Panel>
    </div>
    <div class="pageTopWelcomeArea">
        <% Response.WriteSubstitution(new HttpResponseSubstitutionCallback(ClassLibrary1.Model.PMCacheSubstitution.LoginInfoStatus)); %>
        <%--<Uc:LoginInfoStatus runat="server" ID="UcLogin" />--%>
    </div>
    <div class="pageTopMenuLinks">
        <table>
            <tr>
                <td ><a href="/" id="menuButton">
                    <img ID="ImgHome" src="/images/Home.png" class="reppng" alt="Home" title="Home" /></a></td> 
                <td ><a href="/Ratings" runat="server" id="menuButton5">
                    <img ID="ImgRatings" src="/images/MyRatings.png" class="reppng" alt="My Ratings" title="My Ratings" /></a></td>
                <td ><a href="/MyPoints" id="menuButton4">
                    <img ID="ImgPointsWinnings" src="/images/PointsWinnings.png" class="reppng" alt="My Points &amp; Winnings" title="My Points & Winnings" /></a></td>
                <td ><a href="/Prizes" id="menuButton8">
                    <img ID="ImgPrizeBoard" src="/images/PrizeBoard.png" class="reppng" alt="Prize Board" title="Prize Board" /></a></td>
                <td ><a href="/Forums" id="menuButton7">
                    <img ID="ImgForums" src="/images/Forums.png" class="reppng" alt="Forums" title="Forums" /></a></td>
                <td ><a href="/MyAccount" id="menuButton6">
                    <img ID="ImgMyAccount" src="/images/MyAccount.png" class="reppng" alt="My Account" title="My Account" /></a></td>
                <td ><a href="/Help" id="menuButton2">
                    <img ID="ImgHelp" src="/images/Help.png" class="reppng" alt="Help" title="Help" /></a></td>
            </tr>
        </table>
    </div>
</div>
<script type="text/javascript">
    setupAutoComplete(); 
    $(document).ready(function () {
        treeSetup();
        $(".reppng").replacePngWithGif().removeClass("reppng");
    });
</script>
