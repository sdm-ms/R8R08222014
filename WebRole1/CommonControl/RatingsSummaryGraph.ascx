<%@ Control Language="C#" AutoEventWireup="true" Inherits="RatingsSummaryGraph" Codebehind="RatingsSummaryGraph.ascx.cs" %>
     
  <br />
<asp:Chart ID="Chart1" runat="server" ImageStorageMode="UseHttpHandler" ImageLocation="IgnoredDoNotDelete" ImageType="Png" Height="296px" Width="430px"
    Palette="BrightPastel" BorderlineDashStyle="Solid" BackSecondaryColor="White"
    BackGradientStyle="TopBottom" BorderWidth="2" BackColor="#D3DFF0" BorderColor="26, 59, 105"
    >
    <Legends>
        <asp:Legend IsTextAutoFit="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold"> 
        </asp:Legend>
    </Legends>
    <BorderSkin SkinStyle="Emboss"></BorderSkin>
    <Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid"
            BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent"
            BackGradientStyle="TopBottom">
            <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False"
                WallWidth="0" IsClustered="False"></Area3DStyle>
            <AxisY LineColor="64, 64, 64, 64">
                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                <MajorGrid LineColor="64, 64, 64, 64" />
            </AxisY>
            <AxisX LineColor="64, 64, 64, 64">
                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                <MajorGrid LineColor="64, 64, 64, 64" />
            </AxisX>
        </asp:ChartArea>
    </ChartAreas>
</asp:Chart>
<br />