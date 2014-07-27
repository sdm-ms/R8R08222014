using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using System.Diagnostics;


using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class Tester : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //R8RSupport DataTransitions = new R8RSupport();
        //DataTransitions.AddMissingVolatilityTrackers();

        R8RDataAccess theDataAccess = new R8RDataAccess();
        if (theDataAccess.R8RDB.GetTable<User>().SingleOrDefault(u => u.Username == "admin") != null)
        { // database has been created -- make sure this is admin
            bool allowEmergencyAdminAccess = false; // put breakpoint on next line to allow emergency access to this page
            if (!allowEmergencyAdminAccess && (string)HttpContext.Current.Profile.UserName != "admin")
            {
                Routing.Redirect(Response, new RoutingInfo(RouteID.Login));
                return;
            }
        }

        InFutureBy.Text = ((int)(TestableDateTime.Now - DateTime.Now).TotalMinutes).ToString();
    }

    protected void AddRandomUserRatings_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        int numRatings = Convert.ToInt32(NumRatings.Text);
        int maxNumUserRatings = Convert.ToInt32(MaxPerRating.Text);
        theTester.LaunchTestAddRandomUserRatings(numRatings, maxNumUserRatings);
    }

    protected void ForceHighStakes_Click(object sender, EventArgs e)
    {
        R8RDataManipulation manip = new R8RDataManipulation();
        int numRatings = Convert.ToInt32(ForceHighStakesNumRatingGroups.Text);
        Guid tblID = new Guid(ForceHighStakesTblID.Text);
        Guid? userID = null;
        if (ForceHighStakesUserID.Text != "")
            userID = new Guid(ForceHighStakesUserID.Text);
        manip.ChangeToHighStakes(tblID, numRatings, userID);
    }


    protected void ForceHighStakesEnd_Click(object sender, EventArgs e)
    {
        R8RDataManipulation manip = new R8RDataManipulation();
        Guid tblID = new Guid(EndKnownHighStakesTblID.Text);
        manip.FinishKnownHighStakesSoon(tblID);
    }

    protected void AddUserRatingsToNew_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        int numUserRatings = Convert.ToInt32(NumUserRatings1.Text);
        theTester.LaunchTestAddUserRatings(numUserRatings);
    }
    protected void AddToExisting_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        int numUserRatings = Convert.ToInt32(NumUserRatings2.Text);
        theTester.LaunchTestAddUserRatingsToExisting(numUserRatings);
    }
    protected void MultipleTbls_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.LaunchTestSetupVarietyTbls();
    }
    protected void SingleTbl_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.LaunchTestSetupSingleTbl();
    }

    protected void RatingGroupResolution_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.LaunchTestResolutionCode();

    }

    protected void SingleTblRow_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.LaunchTestAddSingleTblRow();

    }
    protected void FDDelete_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.TestDeleteFieldDefinition(true);

    }
    protected void FDUndelete_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.TestDeleteFieldDefinition(false);

    }
    protected void MultipleTblRows_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.LaunchTestAddTblRows(Convert.ToInt32(NumTblRows.Text));

    }
    protected void CreateMultipleDomains_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.LaunchTestCreateMultipleDomains();

    }
    protected void MultipleTests_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.MultipleTests();

    }

    protected void RandomizeCurrentValue_Click(object sender, EventArgs e)
    {
        R8RTestEnvironmentCreator theTester = new R8RTestEnvironmentCreator();
        theTester.LaunchSetCurrentValueOfFirstRatingForRestaurants();
    }
    protected void Reset_Click(object sender, EventArgs e)
    {

        R8RBuilder theBuilder = new R8RBuilder();
        theBuilder.DeleteAndRebuild();

    }

    //protected void DebugAddRandomDBObject(IR8RDataContext theDataContext)
    //{
    //    LongProcess addedLongProcess = new LongProcess
    //    {
    //        AdditionalInfo = null,
    //        Complete = true,
    //        DelayBeforeRestart = 100,
    //        EarliestRestart = TestableDateTime.Now,
    //        Object1ID = 0,
    //        Object2ID = 0,
    //        Priority = 20,
    //        ProgressInfo = 10,
    //        ResetWhenComplete = false,
    //        Started = true,
    //        TypeOfProcess = 10
    //    };
    //    theDataContext.GetTable<LongProcesse>().InsertOnSubmit(addedLongProcess);

    //}

    //protected void DebugAddRandomDBObjectManyTimes(int numTimes, bool submitChangesEach, IR8RDataContext theDataContext)
    //{
    //    Trace.TraceInformation("Adding random object " + numTimes + " times, submitChangesEach: " + submitChangesEach);
    //    DateTime startTime = TestableDateTime.Now;
    //    for (int i = 1; i <= numTimes; i++)
    //    {
    //        DebugAddRandomDBObject(theDataContext);
    //        if (submitChangesEach)
    //            theDataContext.SubmitChanges();
    //    }
    //    if (!submitChangesEach)
    //        theDataContext.SubmitChanges();
    //    TimeSpan totalTime = TestableDateTime.Now - startTime;
    //    Trace.TraceInformation("Total time " + totalTime.ToString());
    //}

    //protected void DebugTest_Click(object sender, EventArgs e)
    //{
    //    IR8RDataContext theDataContext = GetIR8RDataContext.New(true, true);
    //    DebugAddRandomDBObjectManyTimes(100, true, theDataContext);
    //    DebugAddRandomDBObjectManyTimes(100, false, theDataContext);
    //    DebugAddRandomDBObjectManyTimes(1000, true, theDataContext);
    //    DebugAddRandomDBObjectManyTimes(1000, false, theDataContext);
    //    DebugAddRandomDBObjectManyTimes(1000, true, theDataContext);
    //    DebugAddRandomDBObjectManyTimes(1000, false, theDataContext);
    //    DebugAddRandomDBObjectManyTimes(1000, true, theDataContext);
    //    DebugAddRandomDBObjectManyTimes(1000, false, theDataContext);
    //}

    protected void Standard_Click(object sender, EventArgs e)
    {

        R8RBuilder theBuilder = new R8RBuilder();
        theBuilder.DeleteAndRebuild();
        theBuilder.CreateStandard();

    }

    protected void Transition1_Click(object sender, EventArgs e)
    {
        R8RDataManipulation DataTransitions = new R8RDataManipulation();
        DataTransitions.DatabaseTransition20110324();

    }

    protected void Transition2_Click(object sender, EventArgs e)
    {
        R8RDataManipulation DataTransitions = new R8RDataManipulation();
        DataTransitions.SetNewPointTotalsFields();

    }

    protected void DeleteAllDataFromTable_Click(object sender, EventArgs e)
    {
        R8RDataManipulation DataTransitions = new R8RDataManipulation();
        DataTransitions.DeleteAllDataInTable(new Guid(TblID.Text));

    }

    protected void DeleteUserRatingDataFromTable_Click(object sender, EventArgs e)
    {
        R8RDataManipulation DataTransitions = new R8RDataManipulation();
        DataTransitions.DeleteUserRatingDataInTable(new Guid(TblID2.Text));

    }

    protected void Correction1_Click(object sender, EventArgs e)
    {
        R8RDataManipulation DataTransitions = new R8RDataManipulation();
        throw new NotImplementedException();
    }

    protected void ClearCache_Click(object sender, EventArgs e)
    {
        CacheManagement.ClearCache();
    }


    protected void StartPreventingChanges(object sender, EventArgs e)
    {
        R8RDataManipulation DataManipulation = new R8RDataManipulation();
        DatabaseAndAzureRoleStatus.SetPreventChanges(DataManipulation.DataContext, true); 
    }

    protected void StopPreventingChanges(object sender, EventArgs e)
    {
        R8RDataManipulation DataManipulation = new R8RDataManipulation();
        DatabaseAndAzureRoleStatus.SetPreventChanges(DataManipulation.DataContext, false);
    }

    protected void AddFastAccessTables(object sender, EventArgs e)
    {
        R8RDataManipulation DataTransitions = new R8RDataManipulation();
        DataTransitions.AddFastAccessTables(new DenormalizedTableAccess(1));
    }

    protected void DropFastAccessTables(object sender, EventArgs e)
    {
        R8RDataManipulation DataTransitions = new R8RDataManipulation();
        DataTransitions.DropFastAccessTables(new DenormalizedTableAccess(1));
    }

    protected void GoToFuture_Click(object sender, EventArgs e)
    {
        TestableDateTime.UseFakeTimes();
        TestableDateTime.SleepOrSkipTime(Convert.ToInt32(FutureTime.Text) * 60 * 1000);
    }
}
