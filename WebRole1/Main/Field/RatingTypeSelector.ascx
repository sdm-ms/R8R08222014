<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RatingTypeSelector.ascx.cs" Inherits="RatingTypeSelector" %>
<asp:UpdatePanel runat="server" ID="RatingTypePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <table>
            <tr>
                <td>
                    <asp:RadioButton ID="standardRating" runat="server" GroupName="ratingTypes" Selected="true" Text="Standard Rating (0-10)" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="yesNoRating" runat="server" GroupName="ratingTypes" Selected="false"
                        Text="Yes/No (0%-100%)" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="multipleChoiceRating" runat="server" GroupName="ratingTypes"
                        Selected="false" Text="Multiple Choice (adding up to 100%)" />
                    <div id="multipleChoiceDiv" runat="server">
                        <asp:TextBox ID="multipleChoiceList" TextMode="MultiLine" runat="server" Width="225px" Height="150px"></asp:TextBox>
                        <br />
                        <i>Enter each choice on a separate line.</i>
                        <br />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="rangeOfNumbersRating" runat="server" GroupName="ratingTypes"
                        Selected="false" Text="Range of Numbers (?-?)" />
                    <br />
                    <asp:TextBox ID="rangeFrom" runat="server" Text=""></asp:TextBox>
                    &nbsp;To&nbsp;
                    <asp:TextBox ID="rangeTo" runat="server" Text=""></asp:TextBox>
                    <br />
                    <br />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
