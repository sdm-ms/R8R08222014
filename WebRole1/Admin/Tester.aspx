<%@ Page Language="C#" AutoEventWireup="true" Inherits="Tester" Codebehind="Tester.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

    	<br />
        
		<br />
		<asp:Button ID="PreventChanges" runat="server" OnClick="StartPreventingChanges" 
            Text="Start Preventing Changes " />
		<br />
		<asp:Button ID="AllowChanges" runat="server" OnClick="StopPreventingChanges" 
            Text="Stop Preventing Changes " />
		<br />
		<asp:Button ID="AddFastAccessTablesID" runat="server" OnClick="AddFastAccessTables" 
            Text="Add Fast Access Tables " />
		<br />
		<asp:Button ID="DropFastAccessTablesID" runat="server" OnClick="DropFastAccessTables" 
            Text="Drop Fast Access Tables " />
        <br />
        <br />
        <br />
        <asp:Button ID="ForceHighStakes" runat="server" OnClick="ForceHighStakes_Click" Text="Force high stakes (tblID, number rating groups, userID to target)" />
        <asp:TextBox ID="ForceHighStakesTblID" runat="server"></asp:TextBox>
        <asp:TextBox ID="ForceHighStakesNumRatingGroups" runat="server"></asp:TextBox>
        <asp:TextBox ID="ForceHighStakesUserID" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button21" runat="server" OnClick="ForceHighStakesEnd_Click" Text="End known high stakes (tblID)" />
        <asp:TextBox ID="EndKnownHighStakesTblID" runat="server"></asp:TextBox>
        <br />
        <br />
        <br />
        <br />
        <asp:Button ID="Button11" runat="server" OnClick="Transition1_Click" Text="Complete transition1 (Restaurant simplify) " />
        <br />
        <asp:Button ID="Button12" runat="server" OnClick="Transition2_Click" Text="Complete transition2 (SearchWords)" />
        <br />
        <asp:Button ID="Button13" runat="server" OnClick="Correction1_Click" Text="Correction1 (PointsTotals)" />
        <br />
        <asp:Button ID="Button17" runat="server" OnClick="ClearCache_Click" Text="Clear Cache" />
		<br />
        <br />
        <br />
        <asp:Button ID="Button16" runat="server" OnClick="RandomizeCurrentValue_Click" Text="Randomize Current Value of First Rating for Debugging Queries" />
        <br />
		<asp:Button ID="Button20" runat="server" OnClick="TestBulkCopyToFastAccess" 
            Text="TestBulkCopyToFastAccess " />
		<br />
		<asp:Button ID="Test1" runat="server" OnClick="AddUserRatingsToNew_Click" 
            Text="Create Rating, Add UserRatings " />
        <asp:TextBox ID="NumUserRatings1" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button7" runat="server" OnClick="AddToExisting_Click" Text="Add UserRatings To First Existing Event Rating" />
        <asp:TextBox ID="NumUserRatings2" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="AddRandomUserRatings" runat="server" OnClick="AddRandomUserRatings_Click" Text="Add random predictions (num ratings, max per rating)" />
        <asp:TextBox ID="NumRatings" runat="server"></asp:TextBox>
        <asp:TextBox ID="MaxPerRating" runat="server"></asp:TextBox>
        <br />
        <br />
        <br />
        <br />
        In future by minutes: <asp:Label ID="InFutureBy" runat="server"></asp:Label>
        <asp:Button ID="GoToFuture" runat="server" OnClick="GoToFuture_Click" Text="Go to future for testing (num minutes)" />
        <asp:TextBox ID="FutureTime" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button18" runat="server" OnClick="DeleteAllDataFromTable_Click" Text="Delete all data from table (tbl id)" />
        <asp:TextBox ID="TblID" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button19" runat="server" OnClick="DeleteUserRatingDataFromTable_Click" Text="Delete user rating data from table (tbl id)" />
        <asp:TextBox ID="TblID2" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="SingleTbl_Click" Text="Create Single Tbl" />
        <br />
        <asp:Button ID="Button9" runat="server" OnClick="MultipleTbls_Click" Text="Create Different Types of Tbls" />
        <br />
        <asp:Button ID="Button6" runat="server" OnClick="SingleTblRow_Click" Text="Add single entity to first Tbl" />
        <br />
        <asp:Button ID="Button14" runat="server" OnClick="FDDelete_Click" Text="Delete field definitions from first Tbl" />
        <br />
        <asp:Button ID="Button15" runat="server" OnClick="FDUndelete_Click" Text="Undelete field definitions from first Tbl" />
        <br />
        <asp:Button ID="Button8" runat="server" OnClick="MultipleTblRows_Click" Text="Add number entity to first Tbl" />
        <asp:TextBox ID="NumTblRows" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button2" runat="server" OnClick="RatingGroupResolution_Click" Text="Test Rating Resolution Code" />
        <br />
        <asp:Button ID="Button3" runat="server" OnClick="CreateMultipleDomains_Click" Text="Create multiple domains and Tbls test" />
        <br />
        <asp:Button ID="Button5" runat="server" OnClick="MultipleTests_Click" Text="Multiple tests" />
        <br />
        <asp:Button ID="Button10" runat="server" OnClick="Reset_Click" Text="Reset" />
        <br />
        <asp:Button ID="Button4" runat="server" OnClick="Standard_Click" Text="Reset and create standard Tbls" />
        <br />
        <br />
<%--        <asp:Button ID="DebugTest" runat="server" OnClick="DebugTest_Click" Text="Do a debugging test" />
		<br />
--%>    </div>
    </form>
</body>
</html>
