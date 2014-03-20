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


using ClassLibrary1.Misc;
using ClassLibrary1.Model;

public partial class Tester : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //RaterooSupport DataTransitions = new RaterooSupport();
        //DataTransitions.AddMissingVolatilityTrackers();

        RaterooDataAccess theDataAccess = new RaterooDataAccess();
        if (theDataAccess.RaterooDB.GetTable<User>().SingleOrDefault(u => u.Username == "admin") != null)
        { // database has been created -- make sure this is admin
            bool allowEmergencyAdminAccess = false; // put breakpoint on next line to allow emergency access to this page
            if (!allowEmergencyAdminAccess && (string)HttpContext.Current.Profile.UserName != "admin")
            {
                PMRouting.Redirect(Response, new PMRoutingInfo(PMRouteID.Login));
                return;
            }
        }

        InFutureBy.Text = ((int)(TestableDateTime.Now - DateTime.Now).TotalMinutes).ToString();
    }

    protected void AddRandomUserRatings_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        int numRatings = Convert.ToInt32(NumRatings.Text);
        int maxNumUserRatings = Convert.ToInt32(MaxPerRating.Text);
        theTester.LaunchTestAddRandomUserRatings(numRatings, maxNumUserRatings);
    }

    protected void ForceHighStakes_Click(object sender, EventArgs e)
    {
        RaterooDataManipulation manip = new RaterooDataManipulation();
        int numRatings = Convert.ToInt32(ForceHighStakesNumRatingGroups.Text);
        int tblID = Convert.ToInt32(ForceHighStakesTblID.Text);
        int? userID = null;
        if (ForceHighStakesUserID.Text != "")
            userID = Convert.ToInt32(ForceHighStakesUserID.Text);
        manip.ChangeToHighStakes(tblID, numRatings, userID);
    }


    protected void ForceHighStakesEnd_Click(object sender, EventArgs e)
    {
        RaterooDataManipulation manip = new RaterooDataManipulation();
        int tblID = Convert.ToInt32(EndKnownHighStakesTblID.Text);
        manip.FinishKnownHighStakesSoon(tblID);
    }

    protected void AddUserRatingsToNew_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        int numUserRatings = Convert.ToInt32(NumUserRatings1.Text);
        theTester.LaunchTestAddUserRatings(numUserRatings);
    }
    protected void AddToExisting_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        int numUserRatings = Convert.ToInt32(NumUserRatings2.Text);
        theTester.LaunchTestAddUserRatingsToExisting(numUserRatings);
    }
    protected void MultipleTbls_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.LaunchTestSetupVarietyTbls();
    }
    protected void SingleTbl_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.LaunchTestSetupSingleTbl();
    }

    protected void RatingGroupResolution_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.LaunchTestResolutionCode();

    }

    protected void SingleTblRow_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.LaunchTestAddSingleTblRow();

    }
    protected void FDDelete_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.TestDeleteFieldDefinition(true);

    }
    protected void FDUndelete_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.TestDeleteFieldDefinition(false);

    }
    protected void MultipleTblRows_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.LaunchTestAddTblRows(Convert.ToInt32(NumTblRows.Text));

    }
    protected void CreateMultipleDomains_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.LaunchTestCreateMultipleDomains();

    }
    protected void MultipleTests_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.MultipleTests();

    }

    protected void RandomizeCurrentValue_Click(object sender, EventArgs e)
    {
        RaterooTestEnvironmentCreator theTester = new RaterooTestEnvironmentCreator();
        theTester.LaunchSetCurrentValueOfFirstRatingForRestaurants();
    }
    protected void Reset_Click(object sender, EventArgs e)
    {

        RaterooBuilder theBuilder = new RaterooBuilder();
        theBuilder.DeleteAndRebuild();

    }

    //protected void DebugAddRandomDBObject(IRaterooDataContext theDataContext)
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

    //protected void DebugAddRandomDBObjectManyTimes(int numTimes, bool submitChangesEach, IRaterooDataContext theDataContext)
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
    //    IRaterooDataContext theDataContext = GetIRaterooDataContext.New(true, true);
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

        RaterooBuilder theBuilder = new RaterooBuilder();
        theBuilder.DeleteAndRebuild();
        theBuilder.CreateStandard();

    }

    protected void Transition1_Click(object sender, EventArgs e)
    {
        RaterooDataManipulation DataTransitions = new RaterooDataManipulation();
        DataTransitions.DatabaseTransition20110324();

    }

    protected void Transition2_Click(object sender, EventArgs e)
    {
        RaterooDataManipulation DataTransitions = new RaterooDataManipulation();
        DataTransitions.SetNewPointTotalsFields();

    }

    protected void DeleteAllDataFromTable_Click(object sender, EventArgs e)
    {
        RaterooDataManipulation DataTransitions = new RaterooDataManipulation();
        DataTransitions.DeleteAllDataInTable(Convert.ToInt32(TblID.Text));

    }

    protected void DeleteUserRatingDataFromTable_Click(object sender, EventArgs e)
    {
        RaterooDataManipulation DataTransitions = new RaterooDataManipulation();
        DataTransitions.DeleteUserRatingDataInTable(Convert.ToInt32(TblID2.Text));

    }

    protected void Correction1_Click(object sender, EventArgs e)
    {
        RaterooDataManipulation DataTransitions = new RaterooDataManipulation();
        DataTransitions.Correction1();

    }

    protected void ClearCache_Click(object sender, EventArgs e)
    {
        PMCacheManagement.ClearCache();
    }


    protected void StartPreventingChanges(object sender, EventArgs e)
    {
        RaterooDataManipulation DataManipulation = new RaterooDataManipulation();
        PMDatabaseAndAzureRoleStatus.SetPreventChanges(DataManipulation.DataContext, true); 
    }

    protected void StopPreventingChanges(object sender, EventArgs e)
    {
        RaterooDataManipulation DataManipulation = new RaterooDataManipulation();
        PMDatabaseAndAzureRoleStatus.SetPreventChanges(DataManipulation.DataContext, false);
    }

    protected void AddFastAccessTables(object sender, EventArgs e)
    {
        RaterooDataManipulation DataTransitions = new RaterooDataManipulation();
        DataTransitions.AddFastAccessTables(new DenormalizedTableAccess(1));
    }

    protected void DropFastAccessTables(object sender, EventArgs e)
    {
        RaterooDataManipulation DataTransitions = new RaterooDataManipulation();
        DataTransitions.DropFastAccessTables(new DenormalizedTableAccess(1));
    }

    protected void GoToFuture_Click(object sender, EventArgs e)
    {
        TestableDateTime.UseFakeTimes();
        TestableDateTime.SleepOrSkipTime(Convert.ToInt32(FutureTime.Text) * 60 * 1000);
    }
}
