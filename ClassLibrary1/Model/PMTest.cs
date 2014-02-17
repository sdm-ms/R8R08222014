using System;
using System.Data;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Diagnostics;
using System.Text;

using GoogleGeocoder;


using System.Threading;

using ClassLibrary1.Misc;
using Microsoft.ServiceHosting.Tools.DevelopmentStorage;
using Microsoft.ServiceHosting.Tools.DevelopmentFabric;

namespace ClassLibrary1.Model
{

    public static class StartDevelopmentStorage
    {
        public static void Go()
        {

            DevStore ds = new DevStore();

            // IsRunning checks whether the service is running.
            if (!ds.IsRunning())
            {
                // EnsureRunning will try to start the service if it's not running. The parameter is immaterial
                ds.EnsureRunning(1000);
            }

            DevFabric df = new DevFabric();

            if (!df.IsDevFabricRunning())
            {
                df.EnsureDeveloperFabricRunning();
            }
        }
    }

    public class TestHelper
    {
        public PMActionProcessor ActionProcessor;

        public TestHelper(bool rebuild = true)
        {
            if (rebuild)
            {
                var builder = new RaterooBuilder();
                builder.DeleteAndRebuild();
            }
            ActionProcessor = new PMActionProcessor();
        }

        private int _superUserId;
        public int SuperUserId
        {
            get
            {
                if (_superUserId == 0)
                    _superUserId = ActionProcessor.DataContext.GetTable<User>().Single(u => u.Username == "admin").UserID;
                return _superUserId;
            }
        }
        public int NumUsers;
        public int[] UserIds;
        public decimal[] UserUserRatingAccuracy; // from 0 to 1 -- weight of correct answer vs. some random factor
        public decimal[] UserConfidence; // from 0 to 1 -- weight of user's guess vs. current prediction


        public static int uniqueTestIDForCreatingUsers = 0;

        // We want to reload from the database each time we request these

        Tbl _tbl;
        TblRow _tblRow;
        TblTab _tblTab;
        TblColumn _tblColumn;
        RatingGroup _ratingGroup;
        Rating _rating;

        public Tbl Tbl { get { if (_tbl == null) return _tbl; return ActionProcessor.DataContext.GetTable<Tbl>().Single(x => x.TblID == _tbl.TblID); } set { _tbl = value; } }
        public TblRow TblRow { get { if (_tblRow == null) return _tblRow; return ActionProcessor.DataContext.GetTable<TblRow>().Single(x => x.TblRowID == _tblRow.TblRowID); } set { _tblRow = value; } }
        public TblTab TblTab { get { if (_tblTab == null) return _tblTab; return ActionProcessor.DataContext.GetTable<TblTab>().Single(x => x.TblTabID == _tblTab.TblTabID); } set { _tblTab = value; } }
        public TblColumn TblColumn { get { if (_tblColumn == null) return _tblColumn; return ActionProcessor.DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == _tblColumn.TblColumnID); } set { _tblColumn = value; } }
        public RatingGroup RatingGroup { get { if (_ratingGroup == null) return _ratingGroup; return ActionProcessor.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == _ratingGroup.RatingGroupID); } set { _ratingGroup = value; } }
        public Rating Rating { get { if (_rating == null) return _rating; return ActionProcessor.DataContext.GetTable<Rating>().Single(x => x.RatingID == _rating.RatingID); } set { _rating = value; } }

        public void FinishUserRatingAdd(RaterooDataManipulation rdm)
        {
            if (RaterooDataManipulation.AddUserRatingLockForTesting == null)
            {
                RaterooDataManipulation.AddUserRatingLockForTesting = new object();
                Thread.Sleep(50); // give enough time for any past loop to finish
            }
            rdm.DataContext.SubmitChanges();
            rdm.ResetDataContexts(); // we must reset the data contexts; otherwise, when using the real database, we will still have the old data, not taking into account subsequent changes
            Thread myThread = new Thread(FinishUserRatingAdd_Helper);
            myThread.Name = "FinishUserRating " + TestableDateTime.Now.ToString();
            myThread.Start();
            while (myThread.IsAlive)
                Thread.Sleep(1);
        }

        private void FinishUserRatingAdd_Helper()
        {
            RaterooDataManipulation rdm = new RaterooDataManipulation();
            rdm.CompleteMultipleAddUserRatings();
            rdm.DataContext.SubmitChanges();
            rdm.ResetDataContexts();
        }

        public void CreateSimpleTestTable(bool useExtraLongRatingPhaseGroup)
        {
            var simp = new PMSimpleTestTable();
            if (useExtraLongRatingPhaseGroup)
                simp.UseExtraLongRatingPhaseGroup();
            simp.Create();

            Tbl = ActionProcessor.DataContext.GetTable<Tbl>().FirstOrDefault(x => x.Name.Contains("Test"));
            AddTblRowsToTbl(Tbl.TblID, 1);
            ActionProcessor.DataContext.SubmitChanges();

            TblRow = ActionProcessor.DataContext.GetTable<TblRow>().FirstOrDefault();
            Rating = ActionProcessor.DataContext.GetTable<Rating>().FirstOrDefault();
            RatingGroup = Rating.RatingGroup;
            TblColumn = RatingGroup.TblColumn;
            TblTab = RatingGroup.TblColumn.TblTab;

        }


        public void CreateSimpleEventTestTable()
        {
            new PMSimpleEventTestTable().Create();

            Tbl = ActionProcessor.DataContext.GetTable<Tbl>().SingleOrDefault(x => x.Name.Contains("Test"));
            AddTblRowsToTbl(Tbl.TblID, 1);
            ActionProcessor.DataContext.SubmitChanges();

            TblRow = ActionProcessor.DataContext.GetTable<TblRow>().SingleOrDefault();
            Rating = ActionProcessor.DataContext.GetTable<Rating>().SingleOrDefault();
            RatingGroup = Rating.RatingGroup;
            TblColumn = RatingGroup.TblColumn;
            TblTab = RatingGroup.TblColumn.TblTab;
        }

        public IEnumerable<TblRow> AddTblRowsToTbl(int TblID, int numTblRows)
        {
            List<TblRow> tblRows = new List<TblRow>();
            for (int i = 0; i < numTblRows; i++)
            {
                tblRows.Add(ActionProcessor.TblRowCreate(TblID, SuperUserId, null, "Name " + i.ToString()));
            }
            return tblRows;
        }

        public void CreateUsers(int theNumUsers, bool usersForThisTestOnly = false)
        {
            NumUsers = theNumUsers;
            UserIds = new int[NumUsers];
            UserUserRatingAccuracy = new decimal[NumUsers];
            UserConfidence = new decimal[NumUsers];

            string testInUsernameString = "";
            if (usersForThisTestOnly)
                testInUsernameString = "test " + uniqueTestIDForCreatingUsers + " ";
            for(int i = 0; i<NumUsers;i++)
            {

                var existingUser = ActionProcessor.DataContext.GetTable<User>().FirstOrDefault(u => u.Username == "user" + testInUsernameString + i.ToString());
                if (existingUser == null)
                    UserIds[i] = ActionProcessor.UserAdd("user" + i.ToString(), false, "mbabramo@gmail.com", "password" + i.ToString(), false);
                else
                    UserIds[i] = existingUser.UserID;
                //Action.SetUserVerificationStatus(theUsers[i], true, null);
                UserUserRatingAccuracy[i] = (decimal) RandomGenerator.GetRandom();
                UserConfidence[i] = (decimal)RandomGenerator.GetRandom();
            }
        }

        public decimal GetUserUserRating(decimal correctUserRating, decimal? currentUserRating, decimal minUserRating, decimal maxUserRating, int theUserNumber)
        {
            decimal randomUserRating = RandomGenerator.GetRandom(minUserRating, maxUserRating);
            decimal weightOfCorrectUserRating = UserUserRatingAccuracy[theUserNumber];
            decimal userInitialUserRating = (weightOfCorrectUserRating * correctUserRating) + (1 - weightOfCorrectUserRating) * randomUserRating;
            decimal weightUserInitialUserRating = UserConfidence[theUserNumber];
            if (currentUserRating == null)
                return userInitialUserRating;
            return weightUserInitialUserRating * userInitialUserRating + (1 - weightUserInitialUserRating) * (decimal)currentUserRating;
        }

        public void AddUserRatingToRating(int ratingID, decimal correctUserRating)
        {
            AddUserRatingToRating(ratingID, correctUserRating, null);
        }


        public void AddUserRatingToRating(int ratingID, decimal correctUserRating, decimal? specificUserRating)
        {
            AddUserRatingToRating(ratingID, correctUserRating, specificUserRating, false);
        }

        public void AddUserRatingToRating(int ratingID, decimal correctUserRating, decimal? specificUserRating, bool useSuperUser)
        {
            int theUserNumber = RandomGenerator.GetRandom(0, NumUsers - 1);
            Rating theRating = ActionProcessor.DataContext.GetTable<Rating>().Single(m => m.RatingID == ratingID);
            decimal? currentUserRating = theRating.CurrentValue;
            decimal theUserRating;
            if (specificUserRating != null && specificUserRating >= theRating.RatingCharacteristic.MinimumUserRating && specificUserRating <= theRating.RatingCharacteristic.MaximumUserRating)
                theUserRating = (decimal)specificUserRating;
            else
                theUserRating = GetUserUserRating(correctUserRating, currentUserRating, theRating.RatingCharacteristic.MinimumUserRating, theRating.RatingCharacteristic.MaximumUserRating, (int)theUserNumber);
            int numTries = 0;
        TRYLABEL:
            try
            {
                numTries++;
                UserRatingResponse theResponse = new UserRatingResponse();
                ActionProcessor.UserRatingAdd(ratingID, theUserRating, useSuperUser ? SuperUserId : UserIds[theUserNumber], ref theResponse);
                if (!theResponse.result.success)
                    throw new Exception("User rating add failed.");
                //Trace.TraceInformation("UserRating adding result: " + theResponse.result.userMessage);
            }
            catch (Exception e)
            {
                if (e is ChangeConflictException || e.Message.Contains("busy processing"))
                {
                    Trace.TraceError("Handling change conflict or busy processing exception.");
                    if (numTries < 5)
                    {
                        ActionProcessor.ResetDataContexts();
                        Thread.Sleep(2000);
                        goto TRYLABEL;
                    }
                    throw new Exception("Repeated change conflict exceptions");
                }
                else
                    throw e;
            }
        }

        public bool exitImmediately = false;
        public void WaitIdleTasks()
        {
            exitImmediately = false;
            ActionProcessor.DataContext.SubmitChanges();
            long? initialLoopSetCompleted = null;
            bool hasBeenNotBusy = true; // now that we are requesting pauses at the end of this routine we don't have to wait for it not to have been busy.
            bool hasBeenBusyAfterBeingNotBusy = false; // we want it to be not busy, busy, and then not busy again
            bool hasBeenNotBusyAfterBeingBusyAfterBeingNotBusy = false;
            BackgroundThread.CurrentlyPaused = false;
            while (!hasBeenNotBusyAfterBeingBusyAfterBeingNotBusy && !exitImmediately)
            {
                //Trace.TraceInformation("WaitIdleTasks still more work to do.");
                BackgroundThread.Instance.EnsureBackgroundTaskIsRunning(false);
                if (initialLoopSetCompleted == null)
                    initialLoopSetCompleted = BackgroundThread.Instance.LoopSetsCompleted();
                bool moreWorkToDo = BackgroundThread.Instance.IsBackgroundTaskBusy();
                if (hasBeenNotBusy == false && !moreWorkToDo)
                    hasBeenNotBusy = true;
                if (moreWorkToDo && hasBeenNotBusy)
                    hasBeenBusyAfterBeingNotBusy = true;
                if (!moreWorkToDo && hasBeenBusyAfterBeingNotBusy)
                {
                    hasBeenNotBusyAfterBeingBusyAfterBeingNotBusy = true;
                    BackgroundThread.Instance.RequestPauseAndWaitForPauseToBegin();
                }
                else
                {
                    Thread.Sleep(1); // so we don't eat up CPU
                    const int maxLoopSets = 10;
                    long? totalLoopSetsCompleted = BackgroundThread.Instance.LoopSetsCompleted();
                    long? loopSetsCompletedThisWait = totalLoopSetsCompleted - initialLoopSetCompleted;
                    if (loopSetsCompletedThisWait == 5)
                    {
                        Debug.WriteLine("5 Loop sets completed. Breakpoint here to figure out what is happening.");
                    }
                    if (loopSetsCompletedThisWait > maxLoopSets)
                        throw new Exception("Idle tasks appear to be in an infinite loop."); // we can put a conditional breakpoint earlier if we want to more easily debut
                }
            }
            //Trace.TraceInformation("WaitIdleTasks complete.");
            ActionProcessor.ResetDataContexts(); // Otherwise, we may have stale data in our data context.
            exitImmediately = false;
        }


        private void TestHelperAddFinalUserRatings(bool oneRatingFocus, decimal actualProbability)
        {
            IQueryable<Rating> theRatings = null;
            if (oneRatingFocus)
                theRatings = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.RatingID == ActionProcessor.DataContext.NewOrFirst<Rating>(m => m.RatingCharacteristic.Name == "Event").RatingID);
            else
                theRatings = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.RatingID > 0); // all ratings

            var theRatingsGroups = theRatings
                .Select(x => x.TopmostRatingGroupID).Distinct().ToList(); // all rating groups
            List<int> theRatings2 = new List<int>();
            foreach (var mg in theRatingsGroups)
            { // Add one rating from each set of rating group hierarchies to the list to close.
                IQueryable<Rating> theRatingsInGroup = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.TopmostRatingGroupID == mg);
                int pickRating2 = RandomGenerator.GetRandom(1, theRatingsInGroup.Count());
                Rating theRating = theRatingsInGroup.Skip(pickRating2 - 1).First();
                theRatings2.Add(theRating.RatingID);
            }
            foreach (var ratingToClose in theRatings2)
            {
                decimal realizedValue;
                if (RandomGenerator.GetRandom() < (double)actualProbability)
                    realizedValue = 100;
                else
                    realizedValue = 0;
                // have theTestHelper.superUser add the prediction
                AddUserRatingToRating(ratingToClose, actualProbability * 100, realizedValue, true);
            }
        }

        public void TestHelperResolveRatings(bool oneRatingFocus)
        {
            TestHelperResolveRatings(oneRatingFocus, TestableDateTime.Now, false, false);
        }

        public void TestHelperResolveRatings(bool oneRatingFocus, DateTime asOf, bool cancelPreviousResolutions, bool resolveByUnwinding)
        {
            int numTries = 1;
        TRYLABEL:
            try
            {
                numTries++;
                Trace.TraceInformation("TestHelperResolveRatings asOf: " + asOf);
                IQueryable<Rating> theRatings = null;
                if (oneRatingFocus)
                    theRatings = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.RatingID == ActionProcessor.DataContext.NewOrFirst<Rating>(m => m.RatingCharacteristic.Name == "Event").RatingID);
                else
                    theRatings = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.RatingID > 0); // all ratings

                foreach (var debugRating in theRatings)
                    Trace.TraceInformation("On list: Rating " + debugRating.RatingID + " value: " + debugRating.CurrentValue + " ratinggroup " + debugRating.RatingGroupID);

                // now close the rating groups
                var theTopRatingGroups = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.RatingID > 0).Select(m => m.RatingGroup2).Distinct().ToList();
                foreach (var theTopRatingGroup in theTopRatingGroups)
                    ActionProcessor.ResolveRatingGroup(theTopRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, asOf, SuperUserId, null);
                ActionProcessor.DataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                if (e is ChangeConflictException || e.Message.Contains("busy processing"))
                {
                    Trace.TraceInformation("Handling change conflict or busy processing exception for ResolveRatings.");
                    if (numTries < 5)
                    {
                        ActionProcessor.ResetDataContexts();
                        Thread.Sleep(2000);
                        goto TRYLABEL;
                    }
                    throw new Exception("Repeated change conflict exceptions");
                }
                else
                    throw e;
            }
        }

        public void TestHelperAddRandomUserRatings(int numRatings, int maxNumUserRatingsPerRating)
        {
            User theAdmin = ActionProcessor.DataContext.GetTable<User>().Single(u => u.Username == "admin");
            IQueryable<Rating> theRatings = null;
            int theRatingsCount = 0;
            theRatings = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.RatingID > 0); // all ratings
            theRatingsCount = theRatings.Count();
            int countSoFar = 0;
            for (int j = 1; j <= numRatings; j++)
            {
                try
                {
                    int pickRating = RandomGenerator.GetRandom(1, theRatingsCount);
                    Rating theRating = theRatings.Skip(pickRating - 1).First();
                    int numUserRatings = RandomGenerator.GetRandom(1, maxNumUserRatingsPerRating);
                    for (int i = 1; i <= numUserRatings; i++)
                    {
                        decimal randomUserRating = RandomGenerator.GetRandom(theRating.RatingCharacteristic.MinimumUserRating, theRating.RatingCharacteristic.MaximumUserRating);
                        UserRatingResponse theResponse = new UserRatingResponse();
                        ActionProcessor.UserRatingAdd(theRating.RatingID, randomUserRating, theAdmin.UserID, ref theResponse);
                        countSoFar++;
                        if (countSoFar % 1000 == 0)
                            ActionProcessor.DataContext.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("You cannot make another rating on this table cell until Rateroo trusts your changes")) // We won't report this as an error
                        Trace.TraceInformation("EXCEPTION: " + ex.Message);
                }
            }
            ActionProcessor.DataContext.SubmitChanges();
        }

        public void TestHelperAddUserRatings(bool oneRatingFocus, int numUserRatings, decimal actualProbability)
        {
            for (int i = 1; i <= numUserRatings; i++)
            {
                try
                {
                    IQueryable<Rating> theRatings = null;
                    int theRatingsCount = 0;
                    do
                    {
                        if (oneRatingFocus)
                        {
                            Rating anEventRating = ActionProcessor.DataContext.NewOrFirst<Rating>(m => m.RatingCharacteristic.Name == "Event");
                            List<Rating> theList = new List<Rating>();
                            theList.Add(anEventRating);
                            theRatings = theList.AsQueryable();
                        }
                        else
                            theRatings = ActionProcessor.DataContext.GetTable<Rating>().Where(x => x.RatingID > 0); // all ratings
                        theRatingsCount = theRatings.Count();
                        if (theRatingsCount == 0)
                        {
                            BackgroundThread.Instance.EnsureBackgroundTaskIsRunning(false); // allow ratings to be created

                        }
                    } while (theRatingsCount == 0);
                    int pickRating = RandomGenerator.GetRandom(1, theRatingsCount);
                    Rating theRating = theRatings.Skip(pickRating - 1).First();
                    int predictionNum = i;
                    AddUserRatingToRating(theRating.RatingID, (decimal)actualProbability * 100);
                    Trace.TraceInformation("UserRating for rating " + theRating.RatingID + " set to be added Approx.Time " + TestableDateTime.Now);
                    ActionProcessor.DataContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("You cannot make another rating on this table cell until Rateroo trusts your changes")) // We won't report this as an error
                        Trace.TraceInformation("EXCEPTION: " + ex.Message);
                }
                if (i % 25 == 0)
                {
                    BackgroundThread.Instance.EnsureBackgroundTaskIsRunning(false);
                    Thread.Sleep(15000);
                }
            }
        }

    }

    public class RaterooTestEnvironmentCreator
    {

        TestHelper theTestHelper;

        public RaterooTestEnvironmentCreator(TestHelper testHelperToUse = null)
        {
            theTestHelper = testHelperToUse ?? new TestHelper();
        }

        public enum TestPhasesTypes
        {
            fifteenSecondPhasesLinear,
            oneMinPhasesQuadratic,
            oneMinPhasesLinear,
            oneMinPhasesSquareRoot,
            oneMinPhasesLogarithmic,
            lowSubsidyThenHighSubsidyRepeatingPhase,
            singleIndefinitePhase,
            singleTimedPhase,
            threeTimedPhases,
            pickAtRandom
        }

        public enum TestRatingCharacteristicsTypes
        {
            rating,
            eventUserRating,
            numericRangeMinusToPlus,
            numericRangeOnlyPlus,
            numericRangeZeroThousand,
            pickAtRandom
        }

        public enum TestRatingGroupAttributesTypes
        {
            rating,
            predictEvent,
            predictEventWithDefault,
            predictNumberFromRangeMinusToPlus,
            predictNumberFromRangeOnlyPlus,
            multipleContingencies,
            multipleContingenciesWithDefaults,
            twoLevelHierarchyEvent,
            fourLevelHierarchyEvent,
            twoLevelHierarchyRangeUnconstrained,
            twoLevelHierarchyRangeConstrained,
            pickAtRandom
        }

        public enum TestFieldsTypes
        {
            numberZero10,
            numberWideRange,
            choiceGroupBasic,
            choiceGroupMultiple,
            choiceGroupDependent,
            choiceGroupDependentOnMultiple,
            date,
            /* time,
            dateAndTime, */
            text,
            address
        }

        protected int CreateRatingPhaseGroup(TestPhasesTypes myPhasesType, int myChangesGroup)
        {
            if (myPhasesType == TestPhasesTypes.pickAtRandom)
            {
                myPhasesType = (TestPhasesTypes)RandomGenerator.GetRandom((int)TestPhasesTypes.oneMinPhasesQuadratic, (int)TestPhasesTypes.pickAtRandom - 1);
                Trace.TraceInformation("Random phase type: " + myPhasesType.ToString());
            }

            int myRatingPhaseGroup = -1;

            switch (myPhasesType)
            {
                case TestPhasesTypes.fifteenSecondPhasesLinear:
                    List<RatingPhaseData> thePhasesLinearVeryShort = new List<RatingPhaseData>(); ;
                    RatingPhaseData fifteenSecondRepeatingPhaseLinear = new RatingPhaseData(1000, ScoringRules.Linear, true, false, null, 15, 5, true, null);
                    thePhasesLinearVeryShort.Add(fifteenSecondRepeatingPhaseLinear);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(thePhasesLinearVeryShort, "Fifteen second linear phases", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.oneMinPhasesQuadratic:
                    List<RatingPhaseData> thePhasesQuadratic = new List<RatingPhaseData>();
                    RatingPhaseData oneMinuteRepeatingPhaseQuadratic = new RatingPhaseData(1000, ScoringRules.Quadratic, true, false, null, 60, 20, true, null);
                    thePhasesQuadratic.Add(oneMinuteRepeatingPhaseQuadratic);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(thePhasesQuadratic, "One minute quadratic phases", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.oneMinPhasesLinear:
                    List<RatingPhaseData> thePhasesLinear = new List<RatingPhaseData>(); ;
                    RatingPhaseData oneMinuteRepeatingPhaseLinear = new RatingPhaseData(1000, ScoringRules.Linear, true, false, null, 60, 20, true, null);
                    thePhasesLinear.Add(oneMinuteRepeatingPhaseLinear);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(thePhasesLinear, "One minute Linear phases", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.oneMinPhasesSquareRoot:
                    List<RatingPhaseData> thePhasesSquareRoot = new List<RatingPhaseData>(); ;
                    RatingPhaseData oneMinuteRepeatingPhaseSquareRoot = new RatingPhaseData(1000, ScoringRules.SquareRoot, true, false, null, 60, 20, true, null);
                    thePhasesSquareRoot.Add(oneMinuteRepeatingPhaseSquareRoot);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(thePhasesSquareRoot, "One minute SquareRoot phases", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.oneMinPhasesLogarithmic:
                    List<RatingPhaseData> thePhasesLogarithmic = new List<RatingPhaseData>(); ;
                    RatingPhaseData oneMinuteRepeatingPhaseLogarithmic = new RatingPhaseData(1000, ScoringRules.Logarithmic, true, false, null, 60, 20, true, null);
                    thePhasesLogarithmic.Add(oneMinuteRepeatingPhaseLogarithmic);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(thePhasesLogarithmic, "One minute Logarithmic phases", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.lowSubsidyThenHighSubsidyRepeatingPhase:
                    List<RatingPhaseData> lowSubsidyThenHighSubsidyRepeatingPhaseData = new List<RatingPhaseData>(); ;
                    RatingPhaseData lowSubsidyPhase = new RatingPhaseData(200, ScoringRules.Quadratic, true, false, null, 60, 20, false, null);
                    RatingPhaseData highSubsidyPhase = new RatingPhaseData(1000, ScoringRules.Quadratic, true, false, null, 60, 20, true, null);
                    lowSubsidyThenHighSubsidyRepeatingPhaseData.Add(lowSubsidyPhase);
                    lowSubsidyThenHighSubsidyRepeatingPhaseData.Add(highSubsidyPhase);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(lowSubsidyThenHighSubsidyRepeatingPhaseData, "Low subsidy then high subsidy repeating phases", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.singleIndefinitePhase:
                    List<RatingPhaseData> singleIndefinitePhaseData = new List<RatingPhaseData>(); ;
                    RatingPhaseData singleIndefinitePhaseDatum = new RatingPhaseData(1000, ScoringRules.Quadratic, false, false, null, null, 0, false, null);
                    singleIndefinitePhaseData.Add(singleIndefinitePhaseDatum);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(singleIndefinitePhaseData, "Single indefinite phase", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.singleTimedPhase:
                    List<RatingPhaseData> singleTimedPhaseData = new List<RatingPhaseData>(); ;
                    RatingPhaseData singleTimedPhaseDatum = new RatingPhaseData(1000, ScoringRules.Quadratic, true, false, null, 60, 20, false, null);
                    singleTimedPhaseData.Add(singleTimedPhaseDatum);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(singleTimedPhaseData, "Single Timed phase", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestPhasesTypes.threeTimedPhases:
                    List<RatingPhaseData> threeTimedPhaseData = new List<RatingPhaseData>(); ;
                    RatingPhaseData phaseNum1 = new RatingPhaseData(1000, ScoringRules.Quadratic, true, false, null, 60, 20, false, null);
                    RatingPhaseData phaseNum2 = new RatingPhaseData(1000, ScoringRules.Quadratic, true, false, null, 60, 20, false, null);
                    RatingPhaseData phaseNum3 = new RatingPhaseData(1000, ScoringRules.Quadratic, true, false, null, 60, 20, false, null);
                    threeTimedPhaseData.Add(phaseNum1);
                    threeTimedPhaseData.Add(phaseNum2);
                    threeTimedPhaseData.Add(phaseNum3);
                    myRatingPhaseGroup = theTestHelper.ActionProcessor.RatingPhaseGroupCreate(threeTimedPhaseData, "Three Timed phases", true, true, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                default:
                    throw new Exception("Undefined test phase type.");
                    
            }

            return myRatingPhaseGroup;
        }

        protected int CreateRatingCharacteristic(TestRatingCharacteristicsTypes myType, int myRatingPhase, int? subsidyDensityRange, int myChangesGroup)
        {
            if (myType == TestRatingCharacteristicsTypes.pickAtRandom)
            {
                myType = (TestRatingCharacteristicsTypes)RandomGenerator.GetRandom((int)TestRatingCharacteristicsTypes.rating, (int)TestRatingCharacteristicsTypes.pickAtRandom - 1);
                Trace.TraceInformation("Random rating characteristic: " + myType.ToString());
            }

            int myRatingCharacteristic = -1;

            switch (myType)
            {
                case TestRatingCharacteristicsTypes.rating:
                    myRatingCharacteristic = theTestHelper.ActionProcessor.RatingCharacteristicsCreate(myRatingPhase, subsidyDensityRange, 0, 10, 1, "Rating", true, false, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestRatingCharacteristicsTypes.eventUserRating:
                    myRatingCharacteristic = theTestHelper.ActionProcessor.RatingCharacteristicsCreate(myRatingPhase, subsidyDensityRange, 0, 100, 1, "Event", true, false, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestRatingCharacteristicsTypes.numericRangeMinusToPlus:
                    myRatingCharacteristic = theTestHelper.ActionProcessor.RatingCharacteristicsCreate(myRatingPhase, subsidyDensityRange, -500, 500, 0, "Range -500 to 500", true, false, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestRatingCharacteristicsTypes.numericRangeOnlyPlus:
                    myRatingCharacteristic = theTestHelper.ActionProcessor.RatingCharacteristicsCreate(myRatingPhase, subsidyDensityRange, 100, 1000, 0, "Range 100 to 1000", true, false, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                case TestRatingCharacteristicsTypes.numericRangeZeroThousand:
                    myRatingCharacteristic = theTestHelper.ActionProcessor.RatingCharacteristicsCreate(myRatingPhase, subsidyDensityRange, 0, 1000, 0, "Range 0 to 1000", true, false, theTestHelper.SuperUserId, myChangesGroup);
                    break;

                default:
                    throw new Exception("Undefined rating characteristic.");
            }

            return myRatingCharacteristic;
        }


        protected int? CreateRatingGroupAttributes(TestRatingGroupAttributesTypes myType, int theRatingPhase,  int? theSubsidyDensityRange, int? theRatingCondition, int thePointsManagerID, int myChangesGroup)
        {
            if (myType == TestRatingGroupAttributesTypes.pickAtRandom)
            {
                myType = (TestRatingGroupAttributesTypes)RandomGenerator.GetRandom((int)TestRatingGroupAttributesTypes.rating, (int)TestRatingGroupAttributesTypes.pickAtRandom - 1);
                Trace.TraceInformation("Random rating attributes: " + myType.ToString());
            }

            int? myRatingGroupAttributes = -1;
            int myRatingCharacteristic = -1;

            RatingHierarchyData theHierarchy = new RatingHierarchyData();

            switch (myType)
            {
                case TestRatingGroupAttributesTypes.rating:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.rating, theRatingPhase, theSubsidyDensityRange,myChangesGroup);
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, "Rating", RatingGroupTypes.singleNumber, null, "A test rating", true, 0, true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;

                case TestRatingGroupAttributesTypes.predictEvent:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.eventUserRating, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, "Event", RatingGroupTypes.probabilitySingleOutcome, null, "A test rating", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;

                case TestRatingGroupAttributesTypes.predictEventWithDefault:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.eventUserRating, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, "Event", RatingGroupTypes.probabilitySingleOutcome, (decimal)25.2, "A test rating", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;

                case TestRatingGroupAttributesTypes.predictNumberFromRangeMinusToPlus:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.numericRangeMinusToPlus, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, "Range", RatingGroupTypes.singleNumber, null, "A test rating", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;

                case TestRatingGroupAttributesTypes.predictNumberFromRangeOnlyPlus:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.numericRangeOnlyPlus, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, "Range", RatingGroupTypes.singleNumber, null, "A test rating", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;

                case TestRatingGroupAttributesTypes.multipleContingencies:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.eventUserRating, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    theHierarchy.Add("Contingency 1", null, 1, "The description");
                    theHierarchy.Add("Contingency 2", null, 1, "The description");
                    theHierarchy.Add("Contingency 3", null, 1, "The description");
                    theHierarchy.Add("Contingency 4", null, 1, "The description");
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, 100, theHierarchy, "Multiple contingencies", RatingGroupTypes.probabilityMultipleOutcomes, "Multiple contingencies description", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;


                case TestRatingGroupAttributesTypes.multipleContingenciesWithDefaults:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.eventUserRating, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    theHierarchy.Add("Contingency 1", (decimal)10, 1, "The description");
                    theHierarchy.Add("Contingency 2", (decimal)30, 1, "The description");
                    theHierarchy.Add("Contingency 3", null, 1, "The description");
                    theHierarchy.Add("Contingency 4", (decimal)20, 1, "The description");
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, 100, theHierarchy, "Multiple contingencies with defaults", RatingGroupTypes.probabilityMultipleOutcomes, "Multiple contingencies description", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;


                case TestRatingGroupAttributesTypes.twoLevelHierarchyEvent:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.eventUserRating, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    theHierarchy.Add("Contingency 1", null, 1, "The description");
                    theHierarchy.Add("Contingency 1A", null, 2, "The description");
                    theHierarchy.Add("Contingency 1B", null, 2, "The description");
                    theHierarchy.Add("Contingency 1C", null, 2, "The description");
                    theHierarchy.Add("Contingency 2", null, 1, "The description");
                    theHierarchy.Add("Contingency 2A", null, 2, "The description");
                    theHierarchy.Add("Contingency 2B", null, 2, "The description");
                    theHierarchy.Add("Contingency 2C", null, 2, "The description");
                    theHierarchy.Add("Contingency 3", null, 1, "The description");
                    theHierarchy.Add("Contingency 3A", null, 2, "The description");
                    theHierarchy.Add("Contingency 3B", null, 2, "The description");
                    theHierarchy.Add("Contingency 3C", null, 2, "The description");
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, 100, theHierarchy, "Two level event hierarchy", RatingGroupTypes.probabilityHierarchyTop, "Multiple contingencies description", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;


                case TestRatingGroupAttributesTypes.fourLevelHierarchyEvent:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.eventUserRating, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    theHierarchy.Add("Contingency 1", null, 1, "The description");
                    theHierarchy.Add("Contingency 1A", null, 2, "The description");
                    theHierarchy.Add("Contingency 1A1", (decimal)13, 3, "The description");
                    theHierarchy.Add("Contingency 1A11", null, 4, "The description");
                    theHierarchy.Add("Contingency 1A12", null, 4, "The description");
                    theHierarchy.Add("Contingency 1A13", null, 4, "The description");
                    theHierarchy.Add("Contingency 1A14", null, 4, "The description");
                    theHierarchy.Add("Contingency 1A2", (decimal)18, 3, "The description");
                    theHierarchy.Add("Contingency 1A21", (decimal)1.42, 4, "The description");
                    theHierarchy.Add("Contingency 1A22", (decimal)2.42, 4, "The description");
                    theHierarchy.Add("Contingency 1A23", null, 4, "The description");
                    theHierarchy.Add("Contingency 1A24", null, 4, "The description");
                    theHierarchy.Add("Contingency 1B", null, 2, "The description");
                    theHierarchy.Add("Contingency 1C", null, 2, "The description");
                    theHierarchy.Add("Contingency 2", null, 1, "The description");
                    theHierarchy.Add("Contingency 2A", null, 2, "The description");
                    theHierarchy.Add("Contingency 2B", null, 2, "The description");
                    theHierarchy.Add("Contingency 2C", null, 2, "The description");
                    theHierarchy.Add("Contingency 3", (decimal)22, 1, "The description");
                    theHierarchy.Add("Contingency 3A", null, 2, "The description");
                    theHierarchy.Add("Contingency 3B", null, 2, "The description");
                    theHierarchy.Add("Contingency 3C", null, 2, "The description");
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, 100, theHierarchy, "Four level event hierarchy", RatingGroupTypes.probabilityHierarchyTop, "Multiple contingencies description", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;


                case TestRatingGroupAttributesTypes.twoLevelHierarchyRangeUnconstrained:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.numericRangeOnlyPlus, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    theHierarchy.Add("Contingency 1", null, 1, "The description");
                    theHierarchy.Add("Contingency 1A", null, 2, "The description");
                    theHierarchy.Add("Contingency 1B", null, 2, "The description");
                    theHierarchy.Add("Contingency 1C", null, 2, "The description");
                    theHierarchy.Add("Contingency 2", null, 1, "The description");
                    theHierarchy.Add("Contingency 2A", null, 2, "The description");
                    theHierarchy.Add("Contingency 2B", null, 2, "The description");
                    theHierarchy.Add("Contingency 2C", null, 2, "The description");
                    theHierarchy.Add("Contingency 3", null, 1, "The description");
                    theHierarchy.Add("Contingency 3A", null, 2, "The description");
                    theHierarchy.Add("Contingency 3B", null, 2, "The description");
                    theHierarchy.Add("Contingency 3C", null, 2, "The description");
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, null, theHierarchy, "Two level hierarchy range unconstrained", RatingGroupTypes.hierarchyNumbersTop, "Multiple contingencies description", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;


                case TestRatingGroupAttributesTypes.twoLevelHierarchyRangeConstrained:
                    myRatingCharacteristic = CreateRatingCharacteristic(TestRatingCharacteristicsTypes.numericRangeZeroThousand, theRatingPhase, theSubsidyDensityRange, myChangesGroup);
                    theHierarchy.Add("Contingency 1", null, 1, "The description");
                    theHierarchy.Add("Contingency 1A", null, 2, "The description");
                    theHierarchy.Add("Contingency 1B", null, 2, "The description");
                    theHierarchy.Add("Contingency 1C", null, 2, "The description");
                    theHierarchy.Add("Contingency 2", null, 1, "The description");
                    theHierarchy.Add("Contingency 2A", null, 2, "The description");
                    theHierarchy.Add("Contingency 2B", null, 2, "The description");
                    theHierarchy.Add("Contingency 2C", null, 2, "The description");
                    theHierarchy.Add("Contingency 3", null, 1, "The description");
                    theHierarchy.Add("Contingency 3A", null, 2, "The description");
                    theHierarchy.Add("Contingency 3B", null, 2, "The description");
                    theHierarchy.Add("Contingency 3C", null, 2, "The description");
                    myRatingGroupAttributes = theTestHelper.ActionProcessor.RatingGroupAttributesCreate(myRatingCharacteristic, theRatingCondition, (decimal) 2000, theHierarchy, "Two level hierarchy range constrained to 2000 (each entry up to 1000)", RatingGroupTypes.hierarchyNumbersTop, "Multiple contingencies description", true, false, theTestHelper.SuperUserId, myChangesGroup, thePointsManagerID);
                    break;

                default:
                    throw new Exception("Undefined rating group attribute.");
            }

            return myRatingGroupAttributes;
        }

        public void CreateChoiceGroups(int pointsManagerID)
        {
            bool alreadyExist = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroup>().Where(x => x.PointsManagerID == pointsManagerID && !x.Name.Contains("Change choices") ).Any();
            if (alreadyExist)
                return;

            ChoiceGroupData theBasicGroup = new ChoiceGroupData();
            for (int i = 1; i <= 12; i++)
            {
                theBasicGroup.AddChoiceToGroup("Choice S" + i.ToString());
            }
            int choiceGroupBasicID = theTestHelper.ActionProcessor.ChoiceGroupCreate(pointsManagerID, theBasicGroup, ChoiceGroupSettingsMask.GetStandardSetting(), null, true, true, theTestHelper.SuperUserId, null, "ChoiceGroup single");

            ChoiceGroupData theMultipleGroup = new ChoiceGroupData();
            for (int i = 1; i <= 12; i++)
            {
                theMultipleGroup.AddChoiceToGroup("Choice M" + i.ToString());
            }
            int choiceGroupMultipleID = theTestHelper.ActionProcessor.ChoiceGroupCreate(pointsManagerID, theMultipleGroup, ChoiceGroupSettingsMask.GetChoiceGroupSetting(true,false,false,false,false,false,false,false), null, true, true, theTestHelper.SuperUserId, null, "ChoiceGroup multiple");

            ChoiceGroupData theDependentGroup = new ChoiceGroupData();
            for (int i = 1; i <= 5; i++)
                theDependentGroup.AddChoiceToGroup("Always here " + i.ToString(), null);
            for (int i = 1; i <= 12; i++)
            {
                int choiceInSingleGroupID = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceInGroup>().Single(cig => cig.ChoiceText == "Choice S" + i.ToString() && cig.ChoiceGroup.PointsManagerID == pointsManagerID).ChoiceInGroupID;
                for (int j = 1; j <= 3; j++)
                {
                    theDependentGroup.AddChoiceToGroup("Choice S" + i.ToString() + " " + j.ToString(), choiceInSingleGroupID);
                }
            }
            int choiceGroupDependentID = theTestHelper.ActionProcessor.ChoiceGroupCreate(pointsManagerID, theDependentGroup, ChoiceGroupSettingsMask.GetStandardSetting(), choiceGroupBasicID, true, true, theTestHelper.SuperUserId, null, "ChoiceGroup dependent");


            ChoiceGroupData theDependentOnMultipleGroup = new ChoiceGroupData();
            for (int i = 1; i <= 5; i++)
                theDependentOnMultipleGroup.AddChoiceToGroup("Always here " + i.ToString(), null);
            for (int i = 1; i <= 12; i++)
            {
                int choiceInMultipleGroupID = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceInGroup>().Single(cig => cig.ChoiceText == "Choice M" + i.ToString() && cig.ChoiceGroup.PointsManagerID == pointsManagerID).ChoiceInGroupID;
                for (int j = 1; j <= 12; j++)
                {
                    theDependentOnMultipleGroup.AddChoiceToGroup("Choice M" + i.ToString() + " " + j.ToString(), choiceInMultipleGroupID);
                }
            }
            int choiceGroupDependentOnMultipleID = theTestHelper.ActionProcessor.ChoiceGroupCreate(pointsManagerID, theDependentOnMultipleGroup, ChoiceGroupSettingsMask.GetStandardSetting(), choiceGroupMultipleID, true, true, theTestHelper.SuperUserId, null, "ChoiceGroup dependonmult");

        }

        protected void CreateFieldDefinitionsForTbl(int TblID, int numToCreate)
        {
            int pointsManagerID = theTestHelper.ActionProcessor.DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID).PointsManagerID;
            int choiceGroupBasicID = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroup>().Single(c => c.PointsManagerID == pointsManagerID && c.Name.Contains("ChoiceGroup single")).ChoiceGroupID;
            int choiceGroupMultipleID = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroup>().Single(c => c.PointsManagerID == pointsManagerID && c.Name.Contains("ChoiceGroup multiple")).ChoiceGroupID;
            int choiceGroupDependentID = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroup>().Single(c => c.PointsManagerID == pointsManagerID && c.Name.Contains("ChoiceGroup dependent")).ChoiceGroupID;
            int choiceGroupDependentOnMultipleID = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroup>().Single(c => c.PointsManagerID == pointsManagerID && c.Name.Contains("ChoiceGroup dependonmult")).ChoiceGroupID;
            int? lastBasicChoiceGroupField = null;
            int? lastMultipleChoiceGroupField = null;

            for (int i = 1; i <= numToCreate; i++)
            {
                bool keepTrying = true;
                TestFieldsTypes theType = TestFieldsTypes.text;
                while (keepTrying)
                {
                    theType = (TestFieldsTypes)RandomGenerator.GetRandom((int)TestFieldsTypes.numberZero10, (int)TestFieldsTypes.address);
                    keepTrying = false;
                    if (theType == TestFieldsTypes.choiceGroupDependent && lastBasicChoiceGroupField == null)
                        keepTrying = true; // can't do dependent group yet.
                    if (theType == TestFieldsTypes.choiceGroupDependentOnMultiple && lastMultipleChoiceGroupField == null)
                        keepTrying = true;
                }
                bool useAsFilter = true;
                //if (RandomGenerator.GetRandom(0, 1) == 1)
                //    useAsFilter = false;
                string fieldName = theType.ToString() + i.ToString();
                switch (theType)
                {
                    case TestFieldsTypes.address:
                        theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.AddressField, true, true, true, theTestHelper.SuperUserId, null);
                        break;
                    case TestFieldsTypes.choiceGroupBasic:
                        int lastBasicFieldDefinitionID = theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.ChoiceField, useAsFilter, choiceGroupBasicID, null, true, true, theTestHelper.SuperUserId, null);
                        if (useAsFilter == true)
                            lastBasicChoiceGroupField = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(cgfd => cgfd.FieldDefinitionID == lastBasicFieldDefinitionID).ChoiceGroupFieldDefinitionID;
                        break;
                    case TestFieldsTypes.choiceGroupMultiple:
                        int lastMultipleFieldDefinitionID = theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.ChoiceField, useAsFilter, choiceGroupMultipleID, null, true, true, theTestHelper.SuperUserId, null);
                        if (useAsFilter == true)
                            lastMultipleChoiceGroupField = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(cgfd => cgfd.FieldDefinitionID == lastMultipleFieldDefinitionID).ChoiceGroupFieldDefinitionID;
                        break;
                    case TestFieldsTypes.choiceGroupDependent:
                        theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.ChoiceField, useAsFilter, choiceGroupDependentID, lastBasicChoiceGroupField, true, true, theTestHelper.SuperUserId, null);
                        break;
                    case TestFieldsTypes.choiceGroupDependentOnMultiple:
                        theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.ChoiceField, useAsFilter, choiceGroupDependentOnMultipleID, lastMultipleChoiceGroupField, true, true, theTestHelper.SuperUserId, null);
                        break;
                    case TestFieldsTypes.date:
                        theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.DateTimeField, useAsFilter, true, false, true, true, theTestHelper.SuperUserId, null);
                        break;
                    //case TestFieldsTypes.dateAndTime:
                    //    theTestHelper.Action.FieldDefinitionCreate(TblID, fieldName, FieldTypes.DateTimeField, useAsFilter, true, true, true, true, theTestHelper.superUser, null);
                    //    break;
                    //case TestFieldsTypes.time:
                    //    theTestHelper.Action.FieldDefinitionCreate(TblID, fieldName, FieldTypes.DateTimeField, useAsFilter, false, true, true, true, theTestHelper.superUser, null);
                    //    break;
                    case TestFieldsTypes.numberWideRange:
                        theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.NumberField, useAsFilter, (decimal)-10, (decimal)20, 3, true, true, theTestHelper.SuperUserId, null);
                        break;
                    case TestFieldsTypes.numberZero10:
                        theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.NumberField, useAsFilter, (decimal)0, (decimal)10, 1, true, true, theTestHelper.SuperUserId, null);
                        break;
                    case TestFieldsTypes.text:
                        theTestHelper.ActionProcessor.FieldDefinitionCreate(TblID, fieldName, FieldTypes.TextField, useAsFilter, true, true, true, true, true, theTestHelper.SuperUserId, null);
                        break;

                }
            }
        }

        protected int GetRandomFieldDisplaySetting()
        {

            bool ReservedForFutureUse = RandomGenerator.GetRandom(1,2) == 1;
            bool DisplayGoogleMapForAddress = RandomGenerator.GetRandom(1, 2) == 1;
            bool DisplayInTopRightCorner = RandomGenerator.GetRandom(1, 2) == 1;
            bool NewLineBeforeFieldValue = RandomGenerator.GetRandom(1, 2) == 1;
            bool NewLineBeforeFieldName = RandomGenerator.GetRandom(1, 2) == 1;
            bool IncludeFieldName = true;
            bool LargerFont = RandomGenerator.GetRandom(1, 2) == 1;
            bool SmallerFont = RandomGenerator.GetRandom(1, 2) == 1;
            bool Visible = true;
            int theValue = FieldsDisplaySettingsMask.GetFieldDisplaySetting(ReservedForFutureUse, DisplayGoogleMapForAddress, DisplayInTopRightCorner, NewLineBeforeFieldValue, NewLineBeforeFieldName, IncludeFieldName, LargerFont, SmallerFont, Visible);
            return theValue;
        }

        protected void TestCreateFieldDisplaySettings(int theTblID)
        {
            List<int> theFieldDefinitions = new List<int>();
            List<int> displayInTableDisplaySettings = new List<int>();
            List<int> displayInPopUpDisplaySettings  = new List<int>();
            List<int> displayInTblRowPageDisplaySettings = new List<int>();

            theFieldDefinitions = theTestHelper.ActionProcessor.DataContext.GetTable<FieldDefinition>().Where(x => x.TblID == theTblID && x.Status == (byte)StatusOfObject.Active).Select(x => x.FieldDefinitionID).ToList();
            foreach (var theFieldDesc in theFieldDefinitions)
            {
                displayInTableDisplaySettings.Add(GetRandomFieldDisplaySetting());
                displayInPopUpDisplaySettings.Add(GetRandomFieldDisplaySetting());
                displayInTblRowPageDisplaySettings.Add(GetRandomFieldDisplaySetting());
            }

            theTestHelper.ActionProcessor.FieldDefinitionChangeDisplaySettings(theFieldDefinitions, displayInTableDisplaySettings, displayInPopUpDisplaySettings, displayInTblRowPageDisplaySettings, true, theTestHelper.SuperUserId, null);
        }


        protected void TestFillInFieldRandomly(TblRow theTblRow, FieldDefinition theFieldDefinition)
        {
            switch (theFieldDefinition.FieldType)
            {
                case (int) FieldTypes.AddressField:
                    string addressString = "";
                    int pickAddressNum = RandomGenerator.GetRandom(1,6);
                    switch (pickAddressNum)
                    {
                        case 1:
                            addressString = "5215 Washington Blvd.; Arlington, VA 22205";
                            break;
                        case 2:
                            addressString = "900 N. Stuart St.; Arlington, VA";
                            break;
                        case 3:
                            addressString = "2000 H St. NW; Washington, DC";
                            break;
                        case 4:
                            addressString = "nonexistent address; Alexandria, VA";
                            break;
                        case 5:
                            addressString = "completely nonexistent address";
                            break;
                        case 6:
                            addressString = "Glebe Rd. & Washington Blvd., Arlington, VA";
                            break;
                    }
                    Coordinate theCoordinate = Geocode.GetCoordinatesAndReformatAddress(ref addressString);
                    theTestHelper.ActionProcessor.AddressFieldCreateOrReplace(theTblRow, theFieldDefinition.FieldDefinitionID, addressString, theCoordinate.Latitude, theCoordinate.Longitude, theTestHelper.SuperUserId, null);
                    break;
                case (int) FieldTypes.ChoiceField:
                    ChoiceGroupFieldDefinition theCGFieldDefinition = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                    int theChoiceGroupID = theCGFieldDefinition.ChoiceGroupID;
                    IQueryable<ChoiceInField> theChoicesMade = null;
                    List<ChoiceInGroup> theChoicesAvailable = null;
                    if (theCGFieldDefinition.DependentOnChoiceGroupFieldDefinitionID == null)
                        theChoicesAvailable = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceInGroup>().Where(x => x.ChoiceGroupID == theChoiceGroupID).ToList();
                    else
                    {
                        ChoiceGroupFieldDefinition dependentCGFieldDefinition = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.ChoiceGroupFieldDefinitionID == theCGFieldDefinition.DependentOnChoiceGroupFieldDefinitionID);
                        int theDependentFieldDefinitionID = dependentCGFieldDefinition.FieldDefinition.FieldDefinitionID;
                        theChoicesMade = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceInField>().Where(cif => cif.ChoiceField.Field.FieldDefinitionID == theDependentFieldDefinitionID);
                        foreach (var theChoiceMade in theChoicesMade)
                        {
                            var someChoicesAvailable = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceInGroup>().Where(x => x.ChoiceGroupID == theChoiceGroupID
                                && theChoiceMade.ChoiceInGroupID == x.ActiveOnDeterminingGroupChoiceInGroupID);
                            if (theChoicesAvailable == null)
                                theChoicesAvailable = someChoicesAvailable.ToList();
                            else
                                theChoicesAvailable = theChoicesAvailable.Concat(someChoicesAvailable.ToList()).ToList();
                        }
                        var alwaysAvailableChoices = theTestHelper.ActionProcessor.DataContext.GetTable<ChoiceInGroup>().Where(x => x.ChoiceGroupID == theChoiceGroupID
                                && null == x.ActiveOnDeterminingGroupChoiceInGroupID);
                        if (theChoicesAvailable == null)
                            theChoicesAvailable = alwaysAvailableChoices.ToList();
                        else
                            theChoicesAvailable = theChoicesAvailable.Concat(alwaysAvailableChoices.ToList()).ToList();
                    }
                    int numChoices = theChoicesAvailable.Count();
                    if (theCGFieldDefinition.ChoiceGroup.AllowMultipleSelections)
                    {
                        int numChoicesToMake = RandomGenerator.GetRandom(0, Math.Min(4,numChoices));
                        List<int> multipleChoices = new List<int>();
                        for (int choiceNum = 1; choiceNum <= numChoicesToMake; choiceNum++)
                        {
                            int theChoiceInGroupID = theChoicesAvailable.Skip(RandomGenerator.GetRandom(0, numChoices - 1)).First().ChoiceInGroupID;
                            if (!multipleChoices.Any(x => x == theChoiceInGroupID))
                                multipleChoices.Add(theChoiceInGroupID);
                        }
                        theTestHelper.ActionProcessor.ChoiceFieldWithMultipleChoicesCreateOrReplace(theTblRow,theFieldDefinition.FieldDefinitionID,multipleChoices,theTestHelper.SuperUserId,null);
                    }
                    else
                    {           
                        int? theChoiceInGroupID = null;     
                        if (RandomGenerator.GetRandom(0,10) < 8)
                           theChoiceInGroupID = theChoicesAvailable.Skip(RandomGenerator.GetRandom(0, numChoices - 1)).First().ChoiceInGroupID;
                        theTestHelper.ActionProcessor.ChoiceFieldWithSingleChoiceCreateOrReplace(theTblRow, theFieldDefinition.FieldDefinitionID, theChoiceInGroupID, theTestHelper.SuperUserId, null);
                    }
                    break;

                case (int) FieldTypes.DateTimeField:
                    // DateTimeFieldDefinition theDTFieldDefinition = theTestHelper.Action.RaterooDB.GetTable<DateTimeFieldDefinition>().Single(dt => dt.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                    DateTime? theTime = null;
                    if (RandomGenerator.GetRandom(0,5) > 0)
                        theTime = new DateTime(RandomGenerator.GetRandom(1900, 2050), RandomGenerator.GetRandom(1, 12), RandomGenerator.GetRandom(1, 28), RandomGenerator.GetRandom(0, 23), RandomGenerator.GetRandom(0, 59), RandomGenerator.GetRandom(0, 59));
                    theTestHelper.ActionProcessor.DateTimeFieldCreateOrReplace(theTblRow, theFieldDefinition.FieldDefinitionID, theTime, theTestHelper.SuperUserId, null);
                    break;

                case (int) FieldTypes.NumberField:
                    NumberFieldDefinition theNumFieldDefinition = theTestHelper.ActionProcessor.DataContext.GetTable<NumberFieldDefinition>().Single(nfd => nfd.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                    decimal? theNum = null;
                    if (RandomGenerator.GetRandom(0, 5) > 0)
                    {
                        theNum = (decimal) RandomGenerator.GetRandom((double) theNumFieldDefinition.Minimum, (double) theNumFieldDefinition.Maximum);
                        theNum = (decimal) Math.Round((double) theNum, (int) theNumFieldDefinition.DecimalPlaces, MidpointRounding.ToEven);
                    }
                    theTestHelper.ActionProcessor.NumericFieldCreateOrReplace(theTblRow, theFieldDefinition.FieldDefinitionID, theNum, theTestHelper.SuperUserId, null);
                    break;

                case (int) FieldTypes.TextField:
                    TextFieldDefinition theTextFieldDefinition = theTestHelper.ActionProcessor.DataContext.GetTable<TextFieldDefinition>().Single(dt => dt.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                    string randomString = "";
                    if (RandomGenerator.GetRandom(0,5) > 0 && theTextFieldDefinition.IncludeText)
                        randomString = RandomString(RandomGenerator.GetRandom(0, 20), false);
                    if (theTextFieldDefinition.IncludeText && !theTextFieldDefinition.IncludeLink)
                        theTestHelper.ActionProcessor.TextFieldCreateOrReplace(theTblRow, theFieldDefinition.FieldDefinitionID, randomString, theTestHelper.SuperUserId, null);
                    if (theTextFieldDefinition.IncludeLink)
                    {
                        string theLink = "";
                        if (RandomGenerator.GetRandom(0, 1) == 0)
                        {
                            int randomPict = RandomGenerator.GetRandom(0, 4);
                            switch (randomPict)
                            {
                                case 0:
                                    theLink = "http://animals.nationalgeographic.com/staticfiles/NGS/Shared/StaticFiles/animals/images/primary/gray-kangaroo.jpg";
                                    break;

                                case 1:
                                    theLink = "http://imagecache2.allposters.com/images/pic/PTGPOD/541447~Australian-Kangaroo-Posters.jpg";
                                    break;

                                case 2:
                                    theLink = "http://www.giladorigami.com/P_Kangaroo_Engel.jpg";
                                    break;

                                case 3:
                                    theLink = "http://brokenpicturelink.com/asdf.jpg";
                                    break;

                                case 4:
                                    theLink = "http://mlb.mlb.com/images/players/mugshot/ph_490314.jpg";
                                    break;
                            }
                        }
                        else
                        {
                            if (RandomGenerator.GetRandom(0, 5) > 0)
                                theLink = "http://" + RandomString(RandomGenerator.GetRandom(0, 20), false);

                        }
                        theTestHelper.ActionProcessor.TextWithLinkFieldCreateOrReplace(theTblRow, theFieldDefinition.FieldDefinitionID, randomString, theLink, theTestHelper.SuperUserId, null);
                    }
                    break;
            }
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        protected void TestFillInFieldsForTblRowsInTbl(int TblID)
        {
            var theTblRows = theTestHelper.ActionProcessor.DataContext.GetTable<TblRow>().Where(e => e.TblID == TblID  && e.Fields.Count() == 0).ToList();
            var theFieldDefinitions = theTestHelper.ActionProcessor.DataContext.GetTable<FieldDefinition>().Where(fd => fd.TblID == TblID).ToList();
            foreach (var theTblRow in theTblRows)
            {
                foreach (var theFieldDefinition in theFieldDefinitions)
                {
                    if (RandomGenerator.GetRandom(0,5) > 0)
                        TestFillInFieldRandomly(theTblRow, theFieldDefinition);
                }
            }
        }

        protected int CreateTbl(string name, TestRatingGroupAttributesTypes myType, int theRatingPhase, int? theSubsidyDensityRange, int? theRatingCondition, int thePointsManagerID)
        {
            int myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);
            int? myRatingGroupAttributes = CreateRatingGroupAttributes(myType,theRatingPhase,theSubsidyDensityRange,theRatingCondition,thePointsManagerID,myChangesGroup);
            bool oneRatingPerRatingGroup = new[] { TestRatingGroupAttributesTypes.predictEvent, TestRatingGroupAttributesTypes.predictEventWithDefault, TestRatingGroupAttributesTypes.predictNumberFromRangeMinusToPlus, TestRatingGroupAttributesTypes.predictNumberFromRangeOnlyPlus, TestRatingGroupAttributesTypes.rating }.Contains(myType);
            return theTestHelper.ActionProcessor.TblCreate(thePointsManagerID, myRatingGroupAttributes, "Category group", true, true, theTestHelper.SuperUserId, myChangesGroup, name, false, oneRatingPerRatingGroup, "MyTblRow", "The entity addition criteria would be spelled out here.", true, true, "wf250","wf25");
        }

        protected void PointsManagerChangeDollarSubsidySettings(int pointsManagerID)
        {
            int myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);

            decimal currentPeriodDollarSubsidy = 1000;
            DateTime endOfDollarSubsidyPeriod = TestableDateTime.Now.AddMinutes(3);
            decimal nextPeriodDollarSubsidy = 500;
            int nextPeriodLength = 120; // seconds
            short numPrizes = 4;
            decimal minimumPayment = 5;
            PointsManager thePointsManager = theTestHelper.ActionProcessor.DataContext.GetTable<PointsManager>().Single(u => u.PointsManagerID == pointsManagerID);
            theTestHelper.ActionProcessor.PointsManagerChangeSettings(thePointsManager.PointsManagerID, currentPeriodDollarSubsidy, endOfDollarSubsidyPeriod, nextPeriodDollarSubsidy, nextPeriodLength, numPrizes, minimumPayment, true, theTestHelper.SuperUserId, myChangesGroup);
        }

        protected int CreatePointsManager(string name, TestRatingGroupAttributesTypes myType, int theRatingPhase, int? theSubsidyDensityRange, int? theRatingCondition, int theDomainID)
        {
            int myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);
            int pointsManagerID = theTestHelper.ActionProcessor.PointsManagerCreate(theDomainID, null, true, true, theTestHelper.SuperUserId, myChangesGroup, name);
            int? myRatingGroupAttributes = CreateRatingGroupAttributes(myType, theRatingPhase, theSubsidyDensityRange, theRatingCondition, pointsManagerID, myChangesGroup);
            PointsManagerChangeDollarSubsidySettings(pointsManagerID);
            CreateChoiceGroups(pointsManagerID); 
            myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);
            //theTestHelper.Action.RewardTblCreate(pointsManagerID, -10M, 10M, 172800, 86400, 0.75M, null, 1000, true, theTestHelper.superUser, myChangesGroup); 
            theTestHelper.ActionProcessor.RewardTblCreate(pointsManagerID, -10M, 10M, 80, 30, 0.75M, 7.5M, 100, true, theTestHelper.SuperUserId, myChangesGroup);
            theTestHelper.ActionProcessor.UsersRightsCreate(null, pointsManagerID, true, true, false, true, true, false, false, false, false, false, false, "Standard rights", true, true, theTestHelper.SuperUserId, null);
            return pointsManagerID;
        }

        protected int CreateDomain(string name, TestRatingGroupAttributesTypes myType, int theRatingPhase, int? theSubsidyDensityRange, int? theRatingCondition)
        {
            int myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);
            int theDomainID = theTestHelper.ActionProcessor.DomainCreate(true,true,false,true,false,theTestHelper.SuperUserId,myChangesGroup,name);
            int theTblDimensionsID = theTestHelper.ActionProcessor.DataContext.GetTable<TblDimension>().FirstOrDefault().TblDimensionsID;
            theTestHelper.ActionProcessor.DomainChangeAppearance(theDomainID, theTblDimensionsID, true, theTestHelper.SuperUserId, myChangesGroup);
            return theDomainID;
        }

        protected void CreateDomainsTest()
        {
            var myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);
            var myPhaseType = TestPhasesTypes.singleIndefinitePhase;
            int theRatingPhaseID = CreateRatingPhaseGroup(myPhaseType, myChangesGroup);
            var myType = TestRatingGroupAttributesTypes.predictEvent;
            int numDomains = 3;
            for (int domain = 1; domain <= numDomains; domain++)
            {
                int theDomainID = CreateDomain("Domain " +domain, myType, theRatingPhaseID, null, null);
                int numPointsManagers;
                int rand1 = RandomGenerator.GetRandom(1, 3);
                if (rand1 == 1)
                    numPointsManagers = 0;
                else if (rand1 == 2)
                    numPointsManagers = 1;
                else
                    numPointsManagers = RandomGenerator.GetRandom(2, 5);
                for (int universe = 1; universe <= numPointsManagers; universe++)
                {
                    int numTbls;
                    int rand2 = RandomGenerator.GetRandom(1, 3);
                    if (rand2 == 1)
                        numTbls = 0;
                    else // if (rand2 == 2)
                        numTbls = 1;
                    //else
                    //    numTbls = RandomGenerator.GetRandom(2, 5);
                    int thePointsManagerID = CreatePointsManager("PointsManager " + domain + " " + universe, myType, theRatingPhaseID, null, null, theDomainID);
                    for (int Tbl = 1; Tbl <= numTbls; Tbl++)
                    {
                        int theTblID = CreateTbl("Tbl " + domain + " " + universe + " " + Tbl, TestRatingGroupAttributesTypes.rating, theRatingPhaseID, null, null, thePointsManagerID);
                    }
                }
            }
        }

        protected int CreateDomainPointsManagerAndTbl(string name, TestPhasesTypes myPhaseType, TestRatingGroupAttributesTypes myType)
        {
            int myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);
            int theRatingPhaseID = CreateRatingPhaseGroup(myPhaseType, myChangesGroup);
            int theDomainID = CreateDomain(name, myType, theRatingPhaseID, null, null);
            myChangesGroup = theTestHelper.ActionProcessor.ChangesGroupCreate(null, null, theTestHelper.SuperUserId, null, null, null, null);
            int thePointsManagerID = CreatePointsManager(name + " universe",myType,theRatingPhaseID,null,null,theDomainID);
            int theTblID = CreateTbl(name + " Tbl", myType, theRatingPhaseID, null, null, thePointsManagerID);
            theTestHelper.ActionProcessor.HierarchyItemCreate(null, null, theTestHelper.ActionProcessor.DataContext.GetTable<Tbl>().Single(x => x.TblID == theTblID), true, name + " Tbl", theTestHelper.SuperUserId, null);
/*            theTestHelper.Action.InsertableContentCreate(theDomainID, null, null, "domainTest", "This is non-overridable domain content.", true, false, (short)InsertableLocation.TopOfViewTblContent, true, theTestHelper.superUser, null);
            theTestHelper.Action.InsertableContentCreate(theDomainID, null, null, "domainTest2", "This is overridable domain content.", true, true, (short)InsertableLocation.TopOfViewTblContent, true, theTestHelper.superUser, null);
            theTestHelper.Action.InsertableContentCreate(null, thePointsManagerID, null, "universeTest", "This is non-overridable universe content.", true, false, (short)InsertableLocation.TopOfViewTblContent, true, theTestHelper.superUser, null);
            theTestHelper.Action.InsertableContentCreate(null, thePointsManagerID, null, "universeTest2", "This is overridable universe content.", true, true, (short)InsertableLocation.TopOfViewTblContent, true, theTestHelper.superUser, null);
            theTestHelper.Action.InsertableContentCreate(null, null, theTblID, "TblTest", "This is non-overridable Tbl content.", true, false, (short)InsertableLocation.TopOfViewTblContent, true, theTestHelper.superUser, null);
            theTestHelper.Action.InsertableContentCreate(null, null, theTblID, "TblTest2", "This is overridable Tbl content (should appear anyway).", true, true, (short)InsertableLocation.TopOfViewTblContent, true, theTestHelper.superUser, null);

 */
            return theTblID;
        }

        protected int CreateTblTab(string name, int TblID)
        {
            return theTestHelper.ActionProcessor.TblTabCreate(TblID, name, true, true, theTestHelper.SuperUserId, null);
        }

        protected int CreateTblColumn(string abbreviation, string name, string explanation, int TblTab, int defaultRatingGroupAttributes, string prefixString, string suffixString, decimal? extraDecimalPlaceAbove)
        {
            return CreateTblColumn(abbreviation, name, explanation, TblTab, defaultRatingGroupAttributes, prefixString, suffixString, extraDecimalPlaceAbove, "", "");
        }

        protected int CreateTblColumn(string abbreviation, string name, string explanation, int TblTab, int defaultRatingGroupAttributes, string prefixString, string suffixString, decimal? extraDecimalPlaceAbove, string suppStylesMain, string suppStylesHeader, bool trackTrustWithinTableColumn = false)
        {
            int theCatDesc = theTestHelper.ActionProcessor.TblColumnCreate(TblTab, defaultRatingGroupAttributes, abbreviation, name, explanation,"wv1",trackTrustWithinTableColumn, true, true, theTestHelper.SuperUserId, null);
            if (prefixString != "" || suffixString != "" || extraDecimalPlaceAbove != null)
                theTestHelper.ActionProcessor.TblColumnFormattingCreate(theCatDesc, prefixString, suffixString, false, extraDecimalPlaceAbove, null, null, suppStylesHeader, suppStylesMain, true, true, theTestHelper.SuperUserId, null);
            return theCatDesc;
        }


        public void SetupTbls(TestPhasesTypes myPhases)
        {
            SetupTbls(1, 1, 1, 1, 0, TestRatingGroupAttributesTypes.rating, TestRatingGroupAttributesTypes.twoLevelHierarchyRangeConstrained, myPhases);
        }


        public void SetupTbls(int numTblsPerRatingGroupType, int TblTabsPerTbl, int TblColumnsPerTblTab, int numTblRowsPerTbl, int numFieldsToCreate, TestRatingGroupAttributesTypes firstType, TestRatingGroupAttributesTypes lastType, TestPhasesTypes myPhases)
        {

            for (int i = (int)(firstType); i <= (int)(lastType); i++)
            {
                for (int collNum = 1; collNum <= numTblsPerRatingGroupType; collNum++)
                {
                    int theTblID = CreateDomainPointsManagerAndTbl("test " + i.ToString(), myPhases, (TestRatingGroupAttributesTypes)i);
                    for (int j = 0; j < TblTabsPerTbl; j++)
                    {
                        int theTblTab = CreateTblTab("Group " + j.ToString(), theTblID);
                        for (int k = 0; k < TblColumnsPerTblTab; k++)
                        {
                            int? theDefaultRatingGroupAttributesID = theTestHelper.ActionProcessor.DataContext.GetTable<Tbl>().Single(c => c.TblID == theTblID).DefaultRatingGroupAttributesID;
                            string prefixString = "", suffixString = "";
                            decimal? addDecimalPlaceAbove = null;
                            if (i == (int)TestRatingGroupAttributesTypes.predictEvent ||
                                i == (int)TestRatingGroupAttributesTypes.multipleContingencies ||
                                i == (int)TestRatingGroupAttributesTypes.multipleContingenciesWithDefaults ||
                                i == (int)TestRatingGroupAttributesTypes.predictEventWithDefault ||
                                i == (int)TestRatingGroupAttributesTypes.twoLevelHierarchyEvent ||
                                i == (int)TestRatingGroupAttributesTypes.fourLevelHierarchyEvent)
                            {
                                suffixString = "%";
                            }
                            string suppStyleMain = "";
                            string suppStyleHeader = "";
                            if (i == (int)TestRatingGroupAttributesTypes.fourLevelHierarchyEvent || i == (int)TestRatingGroupAttributesTypes.twoLevelHierarchyEvent || i == (int)TestRatingGroupAttributesTypes.twoLevelHierarchyRangeConstrained || i == (int)TestRatingGroupAttributesTypes.twoLevelHierarchyRangeUnconstrained)
                            {
                                suppStyleMain = "mainTableXXSmall";
                                suppStyleHeader = "mainTableHeadingXSmall";
                            }
                            if (i == (int)TestRatingGroupAttributesTypes.rating)
                                addDecimalPlaceAbove = 9.9M;
                            CreateTblColumn("G" + j.ToString() + "C" + k.ToString(), "cat descr " + j.ToString() + " " + k.ToString(), "Explanation of column " + j.ToString() + " " + k.ToString(), theTblTab, (int)theDefaultRatingGroupAttributesID, prefixString, suffixString, addDecimalPlaceAbove, suppStyleMain, suppStyleHeader);

                        }

                    }
                    CreateFieldDefinitionsForTbl(theTblID, numFieldsToCreate);
                    TestCreateFieldDisplaySettings(theTblID);
                    TestRandomizeTblColumnSortOptions(theTblID);
                    AddTblRowsToTblFillInAndLaunch(numTblRowsPerTbl - 1, theTblID);
                    AddTblRowsToTblFillInAndLaunch(1, theTblID); // do last one separately so that we can profile it separately.
                }
            }
        }

        public void AddTblRowsToTblFillInAndLaunch(int numTblRows, int theTblID)
        {
            theTestHelper.AddTblRowsToTbl(theTblID, numTblRows);
            TestFillInFieldsForTblRowsInTbl(theTblID);
            theTestHelper.ActionProcessor.DataContext.SubmitChanges();
            // theTestHelper.Action.TblCreateRatingsAndBeginTrading(theTblID, theTestHelper.superUser, null);
            theTestHelper.WaitIdleTasks();
        }


        protected void TestRandomizeTblColumnSortOptions(int theTblID)
        {
            var theCatDescs = theTestHelper.ActionProcessor.DataContext.GetTable<TblColumn>().Where(x => x.TblTab.TblID == theTblID && x.Status == (byte)StatusOfObject.Active).ToList();
            foreach (var theCatDesc in theCatDescs)
            {
                bool useAsFilter = RandomGenerator.GetRandom(1, 2) == 1;
                bool sortable = RandomGenerator.GetRandom(1, 2) == 1;
                bool sortAsc = RandomGenerator.GetRandom(1, 2) == 1;

                theTestHelper.ActionProcessor.TblColumnChangeSortOptions(theCatDesc.TblColumnID, useAsFilter, sortable, sortAsc, true, theTestHelper.SuperUserId, null);
            }
        }

        

        public void TestPrep(int numUsers)
        {
            RaterooBuilder theBuilder = new RaterooBuilder();
            theBuilder.DeleteAndRebuild();
            theTestHelper.CreateUsers(numUsers);
        }

        public void TestRating()
        {
            TestPrep(10);
            int numRatings = 1;
            SetupTbls(1, 1, 1, numRatings, 0, TestRatingGroupAttributesTypes.rating, TestRatingGroupAttributesTypes.rating, TestPhasesTypes.singleIndefinitePhase);
            int numUserRatings = 20;
            for (int i = 1; i <= numUserRatings; i++)
            {
                try
                {
                    var theRatings = theTestHelper.ActionProcessor.DataContext.GetTable<Rating>();
                    int pickRating = RandomGenerator.GetRandom(1, theRatings.Count());
                    Rating theRating = theRatings.Skip(pickRating - 1).First();
                    int predictionNum = i;
                    Trace.TraceInformation("UserRating " + predictionNum + " rating " + theRating.RatingID);
                    theTestHelper.AddUserRatingToRating(theRating.RatingID, (decimal)7.5);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("You cannot make another rating on this table cell until Rateroo trusts your changes")) // We won't report this as an error
                        Trace.TraceError("EXCEPTION: " + ex.Message);
                }
            }
        }

        public void TestEvent(bool useExistingRating, int numUsers, int numRatingsIfNew, TestPhasesTypes thePhasesTypes, TestRatingGroupAttributesTypes theRatingType, int numUserRatings)
        {
            if (useExistingRating)
                theTestHelper.CreateUsers(numUsers); // don't do other setup
            else
                TestHelperRatingSetup(theTestHelper.NumUsers, numRatingsIfNew, thePhasesTypes, theRatingType);
            const decimal actualProbability = (decimal) 0.75;
            theTestHelper.TestHelperAddUserRatings(useExistingRating,numUserRatings, actualProbability);
            //TestHelperAddFinalUserRatings(useExistingRating,actualProbability);
            //TestHelperResolveRatings(useExistingRating);
            theTestHelper.WaitIdleTasks();
            Trace.TraceInformation("Main testing loop complete.");
        }

        public void TestDeleteFieldDefinition(bool delete)
        {
            Tbl theTbl = theTestHelper.ActionProcessor.DataContext.GetTable<Tbl>().Where(x => x.Name != "Changes").OrderBy(x => x.TblID).FirstOrDefault();
            User superUser = theTestHelper.ActionProcessor.DataContext.GetTable<User>().Single(u => u.Username == "admin");
            var theFieldDefinitions = theTbl.FieldDefinitions;
            foreach (var fd in theFieldDefinitions)
                theTestHelper.ActionProcessor.FieldDefinitionDeleteOrUndelete(fd.FieldDefinitionID,delete,true,theTestHelper.SuperUserId,null);
        }

        public void TestResolveSetupAndComplete(TestPhasesTypes thePhasesTypes, TestRatingGroupAttributesTypes theRatingType, int numCycles, int maxUserRatingsPerCycle)
        {
            TestHelperRatingSetup(3,4, thePhasesTypes, theRatingType);
            //theTestHelper.TestResolve(numCycles, maxUserRatingsPerCycle);
        }

        private void TestHelperRatingSetup(int numUsers, int numRatings, TestPhasesTypes myPhases, TestRatingGroupAttributesTypes myRatingType)
        {
            TestPrep(numUsers);
            //if (numRatings == 1)
            //    myPhases = TestPhasesTypes.fifteenSecondPhasesLinear;
            SetupTbls(1, 1, 1, numRatings, 0, myRatingType, myRatingType, myPhases);
        }

        


        public void LaunchTestAddRandomUserRatings(int numRatings, int maxNumUserRatingsPerRating)
        {
            theTestHelper.TestHelperAddRandomUserRatings(numRatings, maxNumUserRatingsPerRating);
        }

        public void LaunchTestAddUserRatings(int numUserRatings)
        {
            for (int count = 1; count <= 1; count++)
            {
                Trace.TraceInformation("TESTEVENT # " + count);
                TestEvent(false,10,25,TestPhasesTypes.oneMinPhasesSquareRoot, TestRatingGroupAttributesTypes.multipleContingencies, numUserRatings);
            }
            BackgroundThread.Instance.EnsureBackgroundTaskIsRunning(false);
        }

        public void LaunchTestAddUserRatingsToExisting(int numUserRatings)
        {
            for (int count = 1; count <= 1; count++)
            {
                Trace.TraceInformation("TESTEVENT # " + count);
                TestEvent(true,10,0, TestPhasesTypes.oneMinPhasesLinear, TestRatingGroupAttributesTypes.multipleContingencies, numUserRatings);
            }
        }

        public void LaunchTestSetupSingleTbl()
        {
            TestPrep(5);
            SetupTbls(1, 2, 2, 3, 6, TestRatingGroupAttributesTypes.rating, TestRatingGroupAttributesTypes.rating, TestPhasesTypes.singleIndefinitePhase);
        }

        public void LaunchTestSetupVarietyTbls()
        {
            TestPrep(5);
            SetupTbls(1, 2, 2, 25, 4, TestRatingGroupAttributesTypes.rating, TestRatingGroupAttributesTypes.twoLevelHierarchyEvent, TestPhasesTypes.singleIndefinitePhase); 
            //SetupTbls(1, 1, 1, 1, 2, TestRatingGroupAttributesTypes.rating, TestRatingGroupAttributesTypes.twoLevelHierarchyEvent, TestPhasesTypes.singleIndefinitePhase);
        }

        public void LaunchSetCurrentValueOfFirstRatingForRestaurants()
        { // This is useful in debugging restaurant queries.
            Tbl theTbl = theTestHelper.ActionProcessor.DataContext.GetTable<Tbl>().Single(x => x.Name == "Restaurants");
            bool moreWorkToDo = true;
            int totalComplete = 0;
            while (moreWorkToDo)
            {
                Trace.TraceInformation("Total complete " + totalComplete);
                var theMGs = theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroup>().Where(x => x.TblRow.TblID == theTbl.TblID && x.CurrentValueOfFirstRating == null).Take(5000);
                moreWorkToDo = theMGs.Any();
                if (moreWorkToDo)
                    foreach (var mg in theMGs)
                        mg.CurrentValueOfFirstRating = RandomGenerator.GetRandom((decimal) 0, (decimal) 10);
                totalComplete += 5000;
                theTestHelper.ActionProcessor.DataContext.SubmitChanges();
            }
        }


        public void LaunchTestResolutionCode()
        {
            for (int count = 1; count <= 15; count++)
            {
                Trace.TraceInformation("TESTRESOLVE # " + count);
                TestResolveSetupAndComplete(TestPhasesTypes.oneMinPhasesLinear, TestRatingGroupAttributesTypes.predictEvent,5,9);
            }
        }

        public void LaunchTestCreateMultipleDomains()
        {
            TestPrep(5);
            CreateDomainsTest();
        }

        public void LaunchTestAddSingleTblRow()
        {
            int TblID = theTestHelper.ActionProcessor.DataContext.GetTable<Tbl>().First(c => c.Name != "Changes").TblID;
            AddTblRowsToTblFillInAndLaunch(1, TblID);
            
        }


        public void LaunchTestAddTblRows(int theNum)
        {
            int TblID = theTestHelper.ActionProcessor.DataContext.GetTable<Tbl>().First(c => c.Name != "Changes").TblID;
            AddTblRowsToTblFillInAndLaunch(theNum, TblID);
        }

        public void MultipleTests()
        {
            LaunchTestCreateMultipleDomains();
            LaunchTestSetupVarietyTbls();
            LaunchTestResolutionCode();
            LaunchTestAddUserRatings(50);
        }

    }
}
