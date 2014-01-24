<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="MyPointsSidebar.ascx.cs" Inherits="WebRole1.CommonControl.MyPointsSidebar" %>
<%@ Register TagPrefix="Uc" TagName="HeadTextBackgroundEffect" Src="~/CommonControl/HeadTextBackgroundEffect.ascx" %>
<%@ Register TagPrefix="Uc" TagName="PaymentGuaranteeInfo" Src="~/CommonControl/PaymentGuaranteeInfo.ascx" %>
<Uc:HeadTextBackgroundEffect ID="TheBackgroundEffect" runat="server" />
<div class="headTxt headTxtEffect">
    Prizes & Points
</div>
<div class="afterHeadTxtEffect">
    <div id="MyPointsSidebarDiv" class="MyPointsSidebarContent" runat="server">
        <span id="CurrentPeriod"><asp:Literal ID="CurrentPeriodContent" runat="server"></asp:Literal></span><br />
        <span id="CurrentPrizeInfo"><asp:Literal ID="CurrentPrizeInfoContent" runat="server"></asp:Literal></span><br />
        <Uc:PaymentGuaranteeInfo ID="ThePaymentGuaranteeInfo" runat="server" /><br />
        <span id="PointsThisPeriod"><asp:Literal ID="PointsThisPeriodContent" runat="server"></asp:Literal></span><br />
        <span id="PendingPointsThisPeriod"><asp:Literal ID="PendingPointsThisPeriodContent" runat="server"></asp:Literal></span><br />
        <b>Lifetime</b><br />
        <span id="ScoredRatings"><asp:Literal ID="ScoredRatingsContent" runat="server"></asp:Literal></span><br />
        <span id="PointsPerRating"><asp:Literal ID="PointsPerRatingContent" runat="server"></asp:Literal></span><br />
        <br />
        <button id="UpdateMyPoints" type="button">Update</button>
    </div>
</div>
