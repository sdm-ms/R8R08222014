﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
#else
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif

using FluentAssertions;
using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.ServiceHosting.Tools.DevelopmentStorage;
using Microsoft.ServiceHosting.Tools.DevelopmentFabric;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestProject1
{


    [TestClass]
    public class TestAddAndResolveRatings
    {
        public TestHelper theTestHelper;
        public R8RDataManipulation DataAccess;

        [TestInitialize()]
        public void Initialize()
        {
            GetIR8RDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            TestableDateTime.UseFakeTimes();
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            theTestHelper = new TestHelper();
            DataAccess = new R8RDataManipulation();
        }

        [TestMethod]
        [Category("IntegrationTest")]
        public void RatingGroupIDsWork()
        {
            theTestHelper.CreateSimpleTestTable(true);
            theTestHelper.Rating.RatingGroupID.Should().NotBe(new Guid());
            theTestHelper.RatingGroup.RatingGroupID.Should().NotBe(new Guid());
            theTestHelper.Rating.TopmostRatingGroupID.Should().Be(theTestHelper.RatingGroup.RatingGroupID);
            DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theTestHelper.RatingGroup.RatingGroupID).RatingGroupID.Should().Equals(theTestHelper.RatingGroup.RatingGroupID);
            DataAccess.DataContext.GetTable<Rating>().Single(x => x.TopmostRatingGroupID == theTestHelper.RatingGroup.RatingGroupID).RatingGroup.RatingGroupID.Should().Equals(theTestHelper.RatingGroup.RatingGroupID);
        }

        [TestMethod]
        [Category("Integrationtest")]
        public void TestCreateTableAndAddUserRatingsFromFirstUserViaWebService()
        {
            theTestHelper.CreateSimpleTestTable(true, false);
            theTestHelper.CreateUsers(2);
            UserEditResponse theResponse = new UserEditResponse();
            Guid user0 = theTestHelper.UserIds[0];
            Guid user1 = theTestHelper.UserIds[1];

            TblRow tblRow = DataAccess.DataContext.GetTable<TblRow>().OrderBy(x => x.WhenCreated).First();
            Guid tblID = tblRow.TblID;
            TblColumn tblCol = DataAccess.DataContext.GetTable<TblColumn>().Where(x => x.TblTab.TblID == tblID).OrderBy(x => x.WhenCreated).First();

            User theUser = DataAccess.DataContext.GetTable<User>().FirstOrDefault(x => x.UserID == user0);
            UserAccessInfo theInfo = R8RDataManipulation.GetUserAccessInfoForSpecificUser(theUser.Username);

            // first do it based on TblRowID/TblColumnID ==> should generate the Rating
            // we'll do this on a lot of threads at once; after the first one succeeds, the others should throw exceptions but then recover by figuring out that the rating has been added and then add the rating.
            
            Task[] tasks = new Task[10];
            for (int i = 0; i < 10; i++)
                tasks[i] = Task.Factory.StartNew(() => AddUserRatingViaTblRowAndTblColumnIDs(tblRow, tblCol, theInfo, (double) i));
            Task.WaitAll(tasks);
            theTestHelper.WaitIdleTasks();

            // now do it based on rating
            Rating rating = DataAccess.DataContext.GetTable<Rating>().Single(x => x.RatingGroup.TblRowID == tblRow.TblRowID && x.RatingGroup.TblColumnID == tblCol.TblColumnID);
            RatingAndUserRatingString theRatingAndUserRating = new RatingAndUserRatingString() { ratingID = rating.RatingID.ToString(), theUserRating = 6.5.ToString() };
            List<RatingAndUserRatingString> allRatingsAndUserRatingsForRatingGroup = new List<RatingAndUserRatingString>() { theRatingAndUserRating };
            UserEditResponse response = new WebServices.WebService().ProcessRatings(theInfo, allRatingsAndUserRatingsForRatingGroup);
            response.result.success.Should().Be(true);
            theTestHelper.WaitIdleTasks();
        }

        private void AddUserRatingViaTblRowAndTblColumnIDs(TblRow tblRow, TblColumn tblCol, UserAccessInfo theInfo, double theRating)
        {
            RatingAndUserRatingString theRatingAndUserRating = new RatingAndUserRatingString() { ratingID = tblRow.TblRowID.ToString() + "/" + tblCol.TblColumnID.ToString(), theUserRating = theRating.ToString() };
            List<RatingAndUserRatingString> allRatingsAndUserRatingsForRatingGroup = new List<RatingAndUserRatingString>() { theRatingAndUserRating };
            UserEditResponse response = new WebServices.WebService().ProcessRatings(theInfo, allRatingsAndUserRatingsForRatingGroup);
            response.result.success.Should().Be(true);
        }

        public void TestCreateTableAndAddUserRatingFromFirstUser(bool isEvent)
        {
            if (isEvent)
                theTestHelper.CreateSimpleEventTestTable();
            else
                theTestHelper.CreateSimpleTestTable(true);
            theTestHelper.CreateUsers(7);
            UserEditResponse theResponse = new UserEditResponse();
            Guid user0 = theTestHelper.UserIds[0]; 
            Guid user1 = theTestHelper.UserIds[1];
            Guid user2 = theTestHelper.UserIds[2];
            Guid user3 = theTestHelper.UserIds[3];
            Guid user4 = theTestHelper.UserIds[4];
            Guid user5 = theTestHelper.UserIds[5];
            Guid user6 = theTestHelper.UserIds[6];

            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, isEvent ? 70M : 7M, user0, ref theResponse);
            theTestHelper.WaitIdleTasks();
            Rating theRating = DataAccess.DataContext.GetTable<Rating>().Single(x => x.RatingID == theTestHelper.Rating.RatingID);
            theRating.CurrentValue.Should().Be(isEvent ? 70M : 7M);
        }


        [TestMethod]
        [Category("Long")]
        public void TestCreateTableAndAddLotsOfUserRatingFromFirstUser()
        {
            theTestHelper.CreateSimpleEventTestTable();
            theTestHelper.CreateUsers(7);            
            Guid user1 = theTestHelper.UserIds[1];
            Guid user2 = theTestHelper.UserIds[2];
            Guid user3 = theTestHelper.UserIds[3];
            Guid user4 = theTestHelper.UserIds[4];
            Guid user5 = theTestHelper.UserIds[5];
            Guid user6 = theTestHelper.UserIds[6];

            UserEditResponse theResponse = new UserEditResponse();
            int numToAdd = 1100;
            decimal random = 0;
            Guid user0 = theTestHelper.UserIds[0];
            for (int i = 1; i <= numToAdd; i++)
            {
                random = (decimal)Math.Round(RandomGenerator.GetRandom(), 3) * 100;
                theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, random, user0, ref theResponse);
                if (i % 50 == 0)
                    theTestHelper.FinishUserRatingAdd(DataAccess);
            }
            theTestHelper.WaitIdleTasks();
            (DataAccess.DataContext.GetTable<UserRating>().Count() >= numToAdd).Should().BeTrue();
        }

        public void TestAddRandomUserRatingFromAnotherUser(bool isEvent)
        {
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;
            decimal random = (decimal)Math.Round(RandomGenerator.GetRandom(),3); 
            if (random == 0.7M)
                random = 0.701M; // so it's not exactly same as first rating -- needed for our assertions to be correct
            UserEditResponse theResponse = new UserEditResponse();
            int userNumber = RandomGenerator.GetRandom(1, 6); // exclude first user who we are testing things about
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, isEvent ? random * 100 : random * 10, theTestHelper.UserIds[userNumber], ref theResponse);
            Trace.TraceInformation("Trying to add another HERE after UserRatingAdd " + random + " response of " + theResponse.result.success + " " + theResponse.currentValues.FirstOrDefault().theUserRating);
            theTestHelper.WaitIdleTasks();
            Rating theRating = DataAccess.DataContext.GetTable<Rating>().Single(x => x.RatingID == theTestHelper.Rating.RatingID);
            theRating.CurrentValue.Should().Be(isEvent ? random * 100 : random * 10);
        }

        [TestMethod]
        [Category("Long")]
        public void TestAddUserRating_ShortTermResolution_LongTermResolution()
        {
            TestAddUserRating_ShortTermResolution_LongTermResolution_Helper(true);
            TestAddUserRating_ShortTermResolution_LongTermResolution_Helper(false);
        }

        public void TestAddUserRating_ShortTermResolution_LongTermResolution_Helper(bool addAdditionalUserRatingsAfterFirst)
        {
            Initialize();

            RandomGenerator.SeedOverride = 1;

            /* before pending */
            TestCreateTableAndAddUserRatingFromFirstUser(true);
            UserRating theUserRating = DataAccess.DataContext.GetTable<UserRating>().Single();
            Guid user0 = theTestHelper.UserIds[0];
            User theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0);
            PointsTotal thePointsTotal = theUser.PointsTotals.Single();
            RatingGroup theRatingGroup = theUserRating.Rating.RatingGroup;
            theTestHelper.WaitIdleTasks(); 
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().Single(); // reload
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().Include(x => x.UserRatingGroup).OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsHaveBecomePending.Should().BeFalse();
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeFalse();
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeFalse();
            (theUserRating.PendingPointsLongTerm == 0).Should().BeTrue();
            (theUserRating.PendingPointsShortTerm == 0).Should().BeTrue();
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeTrue();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            DateTime timeBeforePending = TestableDateTime.Now;
            var theRatingGroupPhaseStatusInitial = theUserRating.Rating.RatingGroup.RatingGroupPhaseStatuses.OrderByDescending(x => x.ActualCompleteTime).First();

            /* before short term resolution */
            TimeSpan timeUntilPending = (DateTime) theUserRating.WhenPointsBecomePending - TestableDateTime.Now + TimeSpan.FromMinutes(1);
            TestableDateTime.SleepOrSkipTime((int) timeUntilPending.TotalMilliseconds);
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            else
                theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsHaveBecomePending.Should().BeTrue();
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeTrue();
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeTrue();
            (theUserRating.PendingPointsLongTerm == 0).Should().BeFalse();
            (theUserRating.PendingPointsShortTerm == 0).Should().BeFalse();
            theUserRating.ShortTermResolutionReflected.Should().BeFalse();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            Assert.AreEqual(thePointsTotal.TotalPoints, 0);
            //thePointsTotal.TotalPoints.Should().Be(0);
            DateTime timeWhenPendingButNotResolvedShortTerm = TestableDateTime.Now;

            /* after short term resolution */
            TimeSpan timeUntilShortTermResolution = (DateTime)theRatingGroupPhaseStatusInitial.ShortTermResolveTime - TestableDateTime.Now + TimeSpan.FromMinutes(1);
            TestableDateTime.SleepOrSkipTime((int)timeUntilShortTermResolution.TotalMilliseconds);
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            else
                theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            if (addAdditionalUserRatingsAfterFirst)
                (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            else
                (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            decimal shortTermPointsEarnedInitially = theUserRating.PointsEarnedShortTerm;
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            TestableDateTime.SleepOrSkipTime(5000);
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            DateTime timeImmediatelyAfterShortTermResolution = TestableDateTime.Now;

            /* long term resolution after short term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            bool cancelPreviousResolutions = false;
            bool resolveByUnwinding = false;
            DateTime effectiveTimeOfInitialResolution = TestableDateTime.Now;
            decimal valueAtTimeOfInitialResolution = (decimal)theTestHelper.Rating.CurrentValue;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, user0, null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().NotBe(null);
            decimal longTermResolutionValueAfterInitialResolution = (decimal) theUserRating.LongTermResolutionValue;
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeFalse();
            decimal longTermPointsEarnedInitially = theUserRating.PointsEarnedLongTerm;
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);
            DateTime timeImmediatelyAfterInitialLongTermResolution = TestableDateTime.Now;

            /* cancelling initial long term resolution --> should be back to time after short term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            DateTime timeImmediatelyAfterCancellation = TestableDateTime.Now;

            /* recreating initial long term resolution */
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, user0, null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theTestHelper.Rating.CurrentValue.Should().Be(valueAtTimeOfInitialResolution);
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            if (!addAdditionalUserRatingsAfterFirst)
                theUserRating.PointsEarnedLongTerm.Should().Be(shortTermPointsEarnedInitially, "because short and long term are of equal weight");
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().Be(longTermResolutionValueAfterInitialResolution);
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);

            /* now cancel the resolution and re-resolve it as of an earlier time before short term resolution */
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeWhenPendingButNotResolvedShortTerm, user0, null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating.ShortTermResolutionReflected.Should().BeFalse("because the final resolution is now BEFORE the short-term resolution");
            theUserRating.ResolvedShortTerm.Should().BeTrue("because the short term is still resolved when the long term is");
            theUserRating.ShortTermResolutionValue.Should().Be(theUserRating.LongTermResolutionValue);
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            theUserRating.PointsEarnedShortTerm.Should().Be(theUserRating.PointsEarnedLongTerm, "because the short term points will be granted based on the long term resolution value");

            /* cancel resolution again and re-resolve it at time of initial long term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, user0, null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            if (!addAdditionalUserRatingsAfterFirst)
                theUserRating.PointsEarnedLongTerm.Should().Be(shortTermPointsEarnedInitially, "because short and long term are of equal weight");
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().Be(longTermResolutionValueAfterInitialResolution);
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);

            /* cancel resolution again and re-resolve it at time before the ratings were pending */
            TestableDateTime.SleepOrSkipTime(5000);
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeBeforePending, user0, null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsHaveBecomePending.Should().BeTrue("because we're now after the points have become pending even though resolution was earlier");
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeTrue();
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeTrue();
            (theUserRating.PendingPointsLongTerm == 0).Should().BeTrue();
            (theUserRating.PendingPointsShortTerm == 0).Should().BeTrue();
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse("because now this is resolved");
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeFalse("because now this is resolved");

            /* cancel resolution yet again and again re-resolve it at time of initial long term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, user0, null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            if (!addAdditionalUserRatingsAfterFirst)
                theUserRating.PointsEarnedLongTerm.Should().Be(shortTermPointsEarnedInitially, "because short and long term are of equal weight");
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().Be(longTermResolutionValueAfterInitialResolution);
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);


            /* now try to resolve by unwinding at three times -- also we'll simplify by not canceling first, allowing this to be automatically done*/
            cancelPreviousResolutions = false;
            resolveByUnwinding = true;

            /* first, before short term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeWhenPendingButNotResolvedShortTerm, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeTrue();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            (thePointsTotal.TotalPoints == 0).Should().BeTrue();


            /* second, after short term resolution before initial long term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);


            /* third, after initial long term resolution (should have same results) */
            TestableDateTime.SleepOrSkipTime(5000);
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterInitialLongTermResolution, user0, null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingGroup.WhenCreated).First(); // reload
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == user0); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
        }

        [TestMethod]
        [Category("Long")]
        public void TestRatingResolution_AlternativeMethod()
        {
            theTestHelper.CreateSimpleEventTestTable();
            new TestRatingResolution().RatingResolutionTest(); 
        }

        [TestMethod]
        [Category("UnitTest")]
        public void TestLoad()
        {
            theTestHelper.CreateSimpleTestTable(false);
            var dc = GetIR8RDataContext.New();
            Guid firstRating = dc.GetTable<Rating>().OrderBy(x => x.RatingGroup.WhenCreated).First().RatingID;
            User admin = dc.GetTable<User>().Where(x => x.SuperUser).First();
            RatingsAndRelatedInfoLoader.Load(dc, new List<Guid> { firstRating }, admin);
            // this just makes sure that the load produces no errors -- it doesn't make sure that the context is properly loaded in
        }

        [TestMethod]
        [Category("IntegrationTest")]
        public void VolatilityRecordedProperly()
        {
            Initialize();
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;

            const decimal minRating = 0M;
            const decimal maxRating = 10M;

            theTestHelper.CreateSimpleTestTable(true);
            theTestHelper.CreateUsers(10);

            Guid user0 = theTestHelper.UserIds[0];
            Guid user1 = theTestHelper.UserIds[1];
            Guid user2 = theTestHelper.UserIds[2];
            Guid user3 = theTestHelper.UserIds[3];
            Guid user4 = theTestHelper.UserIds[4];
            Guid user5 = theTestHelper.UserIds[5];
            Guid user6 = theTestHelper.UserIds[6];
            // we don't care about user 1 but will add a user rating just to get things started at a particular rating value
            UserEditResponse theResponse = new UserEditResponse();
            decimal basis = 5M; // midpoint, in absence of ratings
            decimal user1RatingValue = 7M;
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user1RatingValue, user1, ref theResponse);
            theTestHelper.WaitIdleTasks();

            float volatilityObservedDay;
            float volatilityObservedHour;
            float volatilityObservedWeek;
            float volatilityObservedYear;
            GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek, out volatilityObservedYear);
            float expectedAbsoluteVolatility = (float)Math.Abs(user1RatingValue - basis);
            float maximumVolatility = (float)(maxRating - minRating);
            float expectedRelativeVolatility = expectedAbsoluteVolatility / maximumVolatility; // starting from 0 volatility
            volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedYear.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            float distanceFromStartObservedDay;
            float distanceFromStartObservedHour;
            float distanceFromStartObservedWeek;
            float distanceFromStartObservedYear;
            GetVolatilityDistanceFromStartForRating(theTestHelper.Rating, out distanceFromStartObservedHour, out distanceFromStartObservedDay, out distanceFromStartObservedWeek, out distanceFromStartObservedYear);
            float expectedDistanceFromStart = (float)(user1RatingValue - basis);
            float maximumDistanceFromStart = (float)(maxRating - minRating);
            float expectedRelativeDistanceFromStart = expectedDistanceFromStart / maximumDistanceFromStart; // starting from 0 distanceFromStart
            distanceFromStartObservedHour.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedDay.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedWeek.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedYear.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);


            decimal user2RatingValue = 8M;
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user2RatingValue, user2, ref theResponse);
            theTestHelper.WaitIdleTasks();
            GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek, out volatilityObservedYear);
            expectedAbsoluteVolatility = (float)Math.Abs(user2RatingValue - user1RatingValue);
            maximumVolatility = (float)(maxRating - minRating);
            expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedYear.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            GetVolatilityDistanceFromStartForRating(theTestHelper.Rating, out distanceFromStartObservedHour, out distanceFromStartObservedDay, out distanceFromStartObservedWeek, out distanceFromStartObservedYear);
            expectedDistanceFromStart = (float)(user2RatingValue - basis);
            maximumDistanceFromStart = (float)(maxRating - minRating);
            expectedRelativeDistanceFromStart = expectedDistanceFromStart / maximumDistanceFromStart; // starting from 0 distanceFromStart
            distanceFromStartObservedHour.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedDay.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedWeek.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedYear.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            float pushbackObservedHour, pushbackObservedDay, pushbackObservedWeek, pushbackObservedYear;
            GetPushbackFromStartForRating(theTestHelper.Rating, out pushbackObservedHour, out pushbackObservedDay, out pushbackObservedWeek, out pushbackObservedYear);
            float pushback = (expectedRelativeVolatility - Math.Abs(expectedRelativeDistanceFromStart));
            pushbackObservedHour.Should().BeApproximately(pushback, 0.01F);
            float pushbackProportionObservedHour, pushbackProportionObservedDay, pushbackProportionObservedWeek, pushbackProportionObservedYear;
            GetPushbackProportionFromStartForRating(theTestHelper.Rating, out pushbackProportionObservedHour, out pushbackProportionObservedDay, out pushbackProportionObservedWeek, out pushbackProportionObservedYear);
            float pushbackProportion = expectedRelativeVolatility == 0 ? 0 : pushback/expectedRelativeVolatility;
            pushbackProportionObservedHour.Should().BeApproximately(pushbackProportion, 0.01F);

            decimal user3RatingValue = 7M;
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user3RatingValue, user3, ref theResponse);
            theTestHelper.WaitIdleTasks();
            GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek, out volatilityObservedYear);
            expectedAbsoluteVolatility = (float)Math.Abs(user3RatingValue - user2RatingValue);
            maximumVolatility = (float)(maxRating - minRating);
            expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedYear.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            GetVolatilityDistanceFromStartForRating(theTestHelper.Rating, out distanceFromStartObservedHour, out distanceFromStartObservedDay, out distanceFromStartObservedWeek, out distanceFromStartObservedYear);
            expectedDistanceFromStart = (float)(user3RatingValue - basis);
            maximumDistanceFromStart = (float)(maxRating - minRating);
            expectedRelativeDistanceFromStart = expectedDistanceFromStart / maximumDistanceFromStart; // starting from 0 distanceFromStart
            distanceFromStartObservedHour.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedDay.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedWeek.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedYear.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            GetPushbackFromStartForRating(theTestHelper.Rating, out pushbackObservedHour, out pushbackObservedDay, out pushbackObservedWeek, out pushbackObservedYear);
            pushback = (expectedRelativeVolatility - Math.Abs(expectedRelativeDistanceFromStart));
            pushbackObservedHour.Should().BeApproximately(pushback, 0.01F);
            pushbackObservedDay.Should().BeApproximately(pushback, 0.01F);
            pushbackObservedWeek.Should().BeApproximately(pushback, 0.01F);
            pushbackObservedYear.Should().BeApproximately(pushback, 0.01F);
            GetPushbackProportionFromStartForRating(theTestHelper.Rating, out pushbackProportionObservedHour, out pushbackProportionObservedDay, out pushbackProportionObservedWeek, out pushbackObservedYear);
            pushbackProportion = pushback / expectedRelativeVolatility;
            pushbackProportionObservedHour.Should().BeApproximately(pushbackProportion, 0.01F);
            pushbackProportionObservedDay.Should().BeApproximately(pushbackProportion, 0.01F);
            pushbackProportionObservedWeek.Should().BeApproximately(pushbackProportion, 0.01F);
            pushbackObservedYear.Should().BeApproximately(pushbackProportion, 0.01F);

            // The following is deleted because we have changed behavior so that the second user rating of two contemporaneous user ratings gets an adjustment factor of 0.
            //// now add two at once by different users (we cannot use user 2 again because user 2 is not wholly trusted now that its rating was partly reverted)
            //decimal user4RatingValue = 3M;
            //decimal user5RatingValue = 10M;
            //theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user4RatingValue, theuser4, ref theResponse);
            //theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user5RatingValue, theuser5, ref theResponse);
            //theTestHelper.WaitIdleTasks();
            //GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            //// count volatility for two separately
            //// first for user 4's new userrating
            //expectedAbsoluteVolatility = (float)Math.Abs(user4RatingValue - user3RatingValue);
            //maximumVolatility = (float)(maxRating - minRating);
            //expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            //// now for user 5's new userrating
            //expectedAbsoluteVolatility = (float)Math.Abs(user5RatingValue - user4RatingValue);
            //maximumVolatility = (float)(maxRating - minRating);
            //expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            //// see if we're right
            //volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            //volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            //volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            //GetVolatilityDistanceFromStartForRating(theTestHelper.Rating, out distanceFromStartObservedHour, out distanceFromStartObservedDay, out distanceFromStartObservedWeek);
            //expectedDistanceFromStart = (float)(user5RatingValue - basis);
            //maximumDistanceFromStart = (float)(maxRating - minRating);
            //expectedRelativeDistanceFromStart = expectedDistanceFromStart / maximumDistanceFromStart; // starting from 0 distanceFromStart
            //distanceFromStartObservedHour.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            //distanceFromStartObservedDay.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            //distanceFromStartObservedWeek.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            //GetPushbackFromStartForRating(theTestHelper.Rating, out pushbackObservedHour, out pushbackObservedDay, out pushbackObservedWeek);
            //pushback = (expectedRelativeVolatility - Math.Abs(expectedRelativeDistanceFromStart));
            //pushbackObservedHour.Should().BeApproximately(pushback, 0.01F);
            //GetPushbackProportionFromStartForRating(theTestHelper.Rating, out pushbackProportionObservedHour, out pushbackProportionObservedDay, out pushbackProportionObservedWeek);
            //pushbackProportion = pushback / expectedRelativeVolatility;
            //pushbackProportionObservedHour.Should().BeApproximately(pushbackProportion, 0.01F);

            // now, add time
            TestableDateTime.SleepOrSkipTime((long) TimeSpan.FromHours(23.1).TotalMilliseconds);
            theTestHelper.WaitIdleTasks();
            GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek, out volatilityObservedYear);
            volatilityObservedHour.Should().BeApproximately(0, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            GetVolatilityDistanceFromStartForRating(theTestHelper.Rating, out distanceFromStartObservedHour, out distanceFromStartObservedDay, out distanceFromStartObservedWeek, out distanceFromStartObservedYear);
            distanceFromStartObservedHour.Should().BeApproximately(0, 0.01F);
            distanceFromStartObservedDay.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedWeek.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            distanceFromStartObservedYear.Should().BeApproximately(expectedRelativeDistanceFromStart, 0.01F);
            GetPushbackFromStartForRating(theTestHelper.Rating, out pushbackObservedHour, out pushbackObservedDay, out pushbackObservedWeek, out pushback);
            pushbackObservedHour.Should().BeApproximately(0, 0.01F);
            pushbackObservedDay.Should().BeApproximately(pushback, 0.01F);
            pushbackObservedWeek.Should().BeApproximately(pushback, 0.01F);
            GetPushbackProportionFromStartForRating(theTestHelper.Rating, out pushbackProportionObservedHour, out pushbackProportionObservedDay, out pushbackProportionObservedWeek, out pushbackProportionObservedYear);
            pushbackProportionObservedHour.Should().BeApproximately(0, 0.01F);
            pushbackProportionObservedDay.Should().BeApproximately(pushbackProportion, 0.01F);
            pushbackProportionObservedWeek.Should().BeApproximately(pushbackProportion, 0.01F);
            pushbackProportionObservedYear.Should().BeApproximately(pushbackProportion, 0.01F);

            // now just test whether further time cancels out the TotalMovement user rating (in which case others will follow)
            TestableDateTime.SleepOrSkipTime((long) TimeSpan.FromDays(5).TotalMilliseconds);
            theTestHelper.WaitIdleTasks();
            GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek, out volatilityObservedYear);
            volatilityObservedHour.Should().BeApproximately(0, 0.01F);
            volatilityObservedDay.Should().BeApproximately(0, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedYear.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            TestableDateTime.SleepOrSkipTime((long)TimeSpan.FromDays(3).TotalMilliseconds);
            theTestHelper.WaitIdleTasks();
            GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek, out volatilityObservedYear);
            volatilityObservedHour.Should().BeApproximately(0, 0.01F);
            volatilityObservedDay.Should().BeApproximately(0, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(0, 0.01F);
            volatilityObservedYear.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            TestableDateTime.SleepOrSkipTime((long)TimeSpan.FromDays(367).TotalMilliseconds);
            theTestHelper.WaitIdleTasks();
            GetVolatilityTotalMovementForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek, out volatilityObservedYear);
            volatilityObservedHour.Should().BeApproximately(0, 0.01F);
            volatilityObservedDay.Should().BeApproximately(0, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(0, 0.01F);
            volatilityObservedYear.Should().BeApproximately(0, 0.01F);
        }

        private void GetVolatilityTotalMovementForRating(Rating rating, out float volatilityObservedHour, out float volatilityObservedDay, out float volatilityObservedWeek, out float volatilityObservedYear)
        {
            RatingGroup rg = rating.RatingGroup;
            volatilityObservedHour = (float) rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneHour).TotalMovement;
            volatilityObservedDay = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneDay).TotalMovement;
            volatilityObservedWeek = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneWeek).TotalMovement;
            volatilityObservedYear = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneYear).TotalMovement;
        }

        private void GetVolatilityDistanceFromStartForRating(Rating rating, out float volatilityObservedHour, out float volatilityObservedDay, out float volatilityObservedWeek, out float volatilityObservedYear)
        {
            RatingGroup rg = rating.RatingGroup;
            volatilityObservedHour = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneHour).DistanceFromStart;
            volatilityObservedDay = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneDay).DistanceFromStart;
            volatilityObservedWeek = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneWeek).DistanceFromStart;
            volatilityObservedYear = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneYear).DistanceFromStart;
        }

        private void GetPushbackFromStartForRating(Rating rating, out float volatilityObservedHour, out float volatilityObservedDay, out float volatilityObservedWeek, out float volatilityObservedYear)
        {
            RatingGroup rg = rating.RatingGroup;
            volatilityObservedHour = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneHour).Pushback;
            volatilityObservedDay = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneDay).Pushback;
            volatilityObservedYear = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneWeek).Pushback;
            volatilityObservedWeek = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneYear).Pushback;
        }

        private void GetPushbackProportionFromStartForRating(Rating rating, out float volatilityObservedHour, out float volatilityObservedDay, out float volatilityObservedWeek, out float volatilityObservedYear)
        {
            RatingGroup rg = rating.RatingGroup;
            volatilityObservedHour = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneHour).PushbackProportion;
            volatilityObservedDay = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneDay).PushbackProportion;
            volatilityObservedWeek = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneWeek).PushbackProportion;
            volatilityObservedYear = (float)rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneYear).PushbackProportion;
        }
    }

    public class TestRatingResolution
    {
        public TestHelper theTestHelper = new TestHelper();
        public bool testFailed = false;

        public void RatingResolutionTest()
        {
            TestResolve(7, 8);
            if (testFailed)
                throw new Exception("Test failed.");
        }

        public void TestResolve(int numCycles, int maxUserRatingsPerCycle)
        {
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;
            List<DateTime> theDateTimes = new List<DateTime>();
            List<DateTime> originalDateTimes = new List<DateTime>();
            List<UserPointsRecordSnapshot> theUserRecords = new List<UserPointsRecordSnapshot>();
            List<UserRatingPointsRecordSnapshot> theUserRatingRecords = new List<UserRatingPointsRecordSnapshot>();
            theTestHelper.CreateSimpleEventTestTable();
            theTestHelper.CreateUsers(5);
            Guid user1 = theTestHelper.UserIds[1];
            Guid user2 = theTestHelper.UserIds[2];
            Guid user3 = theTestHelper.UserIds[3];
            Guid user4 = theTestHelper.UserIds[4];

            int numResolutions = 0;
            for (int cycle = 1; cycle <= numCycles; cycle++)
            {
                int numUserRatings = maxUserRatingsPerCycle; //  RandomGenerator.GetRandom(0, maxUserRatingsPerCycle);
                const decimal actualProbability = (decimal)0.75;
                Trace.TraceInformation("Adding predictions (before resolving) cycle " + cycle.ToString());
                theTestHelper.TestHelperAddUserRatings(false, numUserRatings, actualProbability);
                // TestHelperAddFinalUserRatings(actualProbability);
                theTestHelper.WaitIdleTasks();
                originalDateTimes.Add(TestableDateTime.Now);
                TestHelperReportAllUserRatings(cycle, true, false, false);
                Trace.TraceInformation("Resolving cycle " + cycle.ToString() + " effective time approx: " + originalDateTimes[cycle - 1].ToLongTimeString());
                theTestHelper.TestHelperResolveRatings(true, originalDateTimes[cycle - 1], false, false); // this is before there has been any short term resolution
                theTestHelper.WaitIdleTasks();
                numResolutions++;
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.WhenCreated).FirstOrDefault().CancelPreviousResolutions.Should().BeFalse();
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int) StatusOfObject.Active).Count().Should().Be(numResolutions);
                TestableDateTime.UseFakeTimes();
                bool moveForward = RandomGenerator.GetRandom() > 0.5;
                int howManyDays = RandomGenerator.GetRandom(1, 14);
                if (moveForward)
                    TestableDateTime.SleepOrSkipTime(1000 * 3600 * 24 * howManyDays); 
                theTestHelper.WaitIdleTasks();
                theDateTimes.Add(TestableDateTime.Now); // just in case short term is resolved during the wait idle tasks period
                TestHelperRecordPoints(theUserRecords, theUserRatingRecords, cycle);
                TestHelperReportAllUserRatings(cycle, false, false, false);
                Trace.TraceInformation("Canceling resolving cycle " + cycle.ToString() + " time " + TestableDateTime.Now.ToLongTimeString());
                theTestHelper.TestHelperResolveRatings(true, TestableDateTime.Now, true, false); // now, unresolve
                theTestHelper.WaitIdleTasks();
                numResolutions++;
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.WhenCreated).FirstOrDefault().CancelPreviousResolutions.Should().BeTrue();
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                TestHelperReportAllUserRatings(cycle, false, true, false);
            }
            for (int cycle = 1; cycle <= numCycles; cycle++)
            {
                Trace.TraceInformation("Re-resolving cycle " + cycle.ToString() + " time: " + originalDateTimes[cycle - 1].ToLongTimeString() + " to " + theDateTimes[cycle - 1].ToLongTimeString());
                theTestHelper.TestHelperResolveRatings(true, theDateTimes[cycle - 1], false, false);
                theTestHelper.WaitIdleTasks();
                numResolutions++;
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.WhenCreated).FirstOrDefault().CancelPreviousResolutions.Should().BeFalse();
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                TestHelperReportAllUserRatings(cycle, false, false, true);
                TestHelperCheckPoints(theUserRecords, theUserRatingRecords, cycle);
                Trace.TraceInformation("Re-canceling resolving cycle " + cycle.ToString());
                theTestHelper.TestHelperResolveRatings(true, TestableDateTime.Now, true, false); // now, unresolve
                theTestHelper.WaitIdleTasks();
                numResolutions++;
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.WhenCreated).FirstOrDefault().CancelPreviousResolutions.Should().BeTrue();
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                TestHelperReportAllUserRatings(cycle, false, true, true);
                if (cycle == numCycles)
                { // test resolve by unwinding
                    Trace.TraceInformation("Re-resolving cycle " + cycle.ToString() + " by unwinding");
                    theTestHelper.TestHelperResolveRatings(true, theDateTimes[cycle - 1], false, true);
                    theTestHelper.WaitIdleTasks();
                    numResolutions++;
                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.WhenCreated).FirstOrDefault().CancelPreviousResolutions.Should().BeFalse();

                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.WhenCreated).FirstOrDefault().ResolveByUnwinding.Should().BeTrue();
                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                    Trace.TraceInformation("Long term points should be 0 for list below.");
                    TestHelperReportAllUserRatings(cycle + 100, false, true, true);

                    Trace.TraceInformation("Re-resolving cycle " + cycle.ToString() + " after unwinding");
                    theTestHelper.TestHelperResolveRatings(true, theDateTimes[cycle - 1], false, false);
                    theTestHelper.WaitIdleTasks();
                    numResolutions += 2; // because ti will first cancel and then add another resolution
                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.WhenCreated).FirstOrDefault().CancelPreviousResolutions.Should().BeFalse();
                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                    Trace.TraceInformation("Long term points should not generally be zeor for list below.");
                    TestHelperReportAllUserRatings(cycle, false, false, true);
                    TestHelperCheckPoints(theUserRecords, theUserRatingRecords, cycle);
                }
            }
            Trace.TraceInformation("Resolve: testing loop complete.");
        }

        internal class UserPointsRecordSnapshot
        {
            public Guid userID;
            public int afterCycle;
            public decimal points;

            public UserPointsRecordSnapshot(Guid theUserID, int theAfterCycle, decimal thePoints)
            {
                userID = theUserID;
                afterCycle = theAfterCycle;
                points = thePoints;
            }
        }

        internal class UserRatingPointsRecordSnapshot
        {
            public Guid predID;
            public int afterCycle;
            public decimal points;

            public UserRatingPointsRecordSnapshot(Guid thePredID, int theAfterCycle, decimal thePoints)
            {
                predID = thePredID;
                afterCycle = theAfterCycle;
                points = thePoints;
            }
        }

        internal void TestHelperReportAllUserRatings(int cycle, bool beforeResolve, bool afterUndo, bool secondPass)
        {
            var theUserRatings = theTestHelper.ActionProcessor.DataContext.GetTable<UserRating>()
                .Include(x => x.Rating.RatingGroup)
                .Include(x => x.UserRatingGroup.RatingGroupPhaseStatus)
                .Include(x => x.RatingPhaseStatus.RatingGroupPhaseStatus)
                .OrderBy(x => x.UserRatingGroup.WhenCreated);
            Debug.WriteLine("All user ratings: ");
            int userRatingNumber = 0;
            foreach (var theUserRating in theUserRatings.ToList())
            {
                userRatingNumber++;
                Debug.WriteLine("User rating: " + userRatingNumber + " current time: " + TestableDateTime.Now.ToLongTimeString());
                Debug.WriteLine(String.Format("RATING={7} CYCLE={0} PASS={1} BEFOREUNDO={2} ID={3} GROUP={4} MADE={5} USER={6}  \n ROUND={8} SHORT_REF={18} LONG_REF={19} SHORT_EARNED={9} LONG_EARNED={10} TOTAL={11} SHORTTIME={12} SHORTVAL={13} \n NOT_PENDING_ST={14} NOT_PENDING_LT={15} PEND={16} LONGVAL={17} \n"
                    , cycle, //0
                    secondPass ? 2 : 1, //1
                    afterUndo ? false : true, //2 
                    theUserRating.UserRatingID, //3
                    theUserRating.UserRatingGroupID, //4
                    theUserRating.UserRatingGroup.WhenCreated.ToLongTimeString(), //5
                    theUserRating.UserID, //6
                    theUserRating.RatingID, //7
                    theUserRating.UserRatingGroup.RatingGroupPhaseStatus.RoundNum, //8
                    theUserRating.PointsEarnedShortTerm, //9
                    theUserRating.PointsEarnedLongTerm, //10
                    theUserRating.PointsEarned, //11
                    theUserRating.RatingPhaseStatus.RatingGroupPhaseStatus.ShortTermResolveTime, //12
                    theUserRating.ShortTermResolutionValue, //13
                    theUserRating.NotYetPendingPointsShortTerm, //14
                    theUserRating.NotYetPendingPointsLongTerm, //15
                    theUserRating.PendingPoints, //16
                    theUserRating.LongTermResolutionValue, //17
                    theUserRating.ShortTermResolutionReflected, //18
                    theUserRating.LongTermResolutionReflected //19
                    ));
                var theMPS = theUserRating.UserRatingGroup.RatingGroup.RatingGroupPhaseStatuses.Single(x => x.StartTime <= theUserRating.UserRatingGroup.WhenCreated && (x.ActualCompleteTime == null || x.ActualCompleteTime >= theUserRating.UserRatingGroup.WhenCreated));
                Debug.Write(String.Format(" ROUND={0} START_TIME={1} END_TIME={2} \n", theMPS.RoundNum, theMPS.StartTime.ToLongTimeString(), theMPS.ActualCompleteTime.ToLongTimeString()));
                Debug.WriteLine("");
            }
            var theUsers = theTestHelper.ActionProcessor.DataContext.GetTable<PointsTotal>().OrderBy(x => x.User.WhenCreated).ToList();
            foreach (var theUser in theUsers)
            {
                string reportString = String.Format("POINTTOTALS: user {0},  points {1}", theUser.UserID, theUser.TotalPoints);
                Trace.TraceInformation(reportString);
            }
        }

        internal void TestHelperRecordPoints(List<UserPointsRecordSnapshot> theList, List<UserRatingPointsRecordSnapshot> theList2, int theCycle)
        {
            var theUsers = theTestHelper.ActionProcessor.DataContext.GetTable<PointsTotal>().Where(x => true).ToList();
            foreach (var theUser in theUsers)
            {
                string reportString = String.Format("user {0}, cycle {1}, points {2}", theUser.UserID, theCycle, theUser.TotalPoints);
                Trace.TraceInformation(reportString);
                theList.Add(new UserPointsRecordSnapshot(theUser.UserID, theCycle, theUser.TotalPoints));
            }
            var theUserRatings = theTestHelper.ActionProcessor.DataContext.GetTable<UserRating>().Where(x => true).ToList();
            foreach (var theUserRating in theUserRatings)
            {
                //string reportString = String.Format("user {0}, cycle {1}, points {2}", theUser.UserID, theCycle, theUser.TotalPoints);
                //Trace.TraceInformation(reportString);
                theList2.Add(new UserRatingPointsRecordSnapshot(theUserRating.UserRatingID, theCycle, theUserRating.PointsEarned));
            }

        }

        internal void TestHelperCheckPoints(List<UserPointsRecordSnapshot> theList, List<UserRatingPointsRecordSnapshot> theList2, int theCycle)
        {
            var theUsers = theTestHelper.ActionProcessor.DataContext.GetTable<PointsTotal>().OrderBy(x => x.User.WhenCreated).ToList();
            foreach (var theUser in theUsers)
            {
                var recordedUser = theList.SingleOrDefault(x => x.afterCycle == theCycle && x.userID == theUser.UserID);
                decimal recordedPoints = (recordedUser == null) ? 0 : recordedUser.points;
                if (theUser.TotalPoints != recordedPoints)
                {
                    //throw new Exception(String.Format("POINT INCONSISTENCY: user {0}, cycle {1}, points {2}, should be {3}", theUser.UserID, theCycle, theUser.TotalPoints, recordedPoints));
                    Trace.TraceInformation(String.Format("POINT INCONSISTENCY: user {0}, cycle {1}, points {2}, should be {3}", theUser.UserID, theCycle, theUser.TotalPoints, recordedPoints));
                    testFailed = true;
                }
            }

            var theUserRatings = theTestHelper.ActionProcessor.DataContext.GetTable<UserRating>().Where(x => true).ToList();
            foreach (var theUserRating in theUserRatings)
            {
                var recordedPred = theList2.SingleOrDefault(x => x.afterCycle == theCycle && x.predID == theUserRating.UserRatingID);
                decimal recordedPoints = (recordedPred == null) ? 0 : recordedPred.points;
                if (theUserRating.PointsEarned != recordedPoints)
                {
                    Trace.TraceInformation(String.Format("PREDICTION INCONSISTENCY: prediction {0}, cycle {1}, points {2}, should be {3}", theUserRating.UserRatingID, theCycle, theUserRating.PointsEarned, recordedPoints));
                    testFailed = true;
                    //throw new Exception(String.Format("PREDICTION INCONSISTENCY: prediction {0}, cycle {1}, points {2}, should be {3}", theUserRating.UserRatingID, theCycle, theUserRating.PointsEarned, recordedPoints));
                }
            }
        }
    }
}
