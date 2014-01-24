﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FluentAssertions.Assertions;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.ServiceHosting.Tools.DevelopmentStorage;
using Microsoft.ServiceHosting.Tools.DevelopmentFabric;
using System.Threading;
using System.Diagnostics;

namespace TestProject1
{


    [TestClass]
    public class TestAddAndResolveRatings
    {
        public TestHelper theTestHelper;
        public RaterooDataManipulation DataAccess;

        [TestInitialize()]
        public void Initialize()
        {
            GetIRaterooDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            theTestHelper = new TestHelper();
            DataAccess = new RaterooDataManipulation();
        }

        [TestMethod]
        public void RatingGroupIDsWork()
        {
            theTestHelper.CreateSimpleTestTable(true);
            theTestHelper.Rating.RatingGroupID.Should().NotBe(0);
            theTestHelper.RatingGroup.RatingGroupID.Should().NotBe(0);
            theTestHelper.Rating.TopmostRatingGroupID.Should().Be(theTestHelper.RatingGroup.RatingGroupID);
            DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theTestHelper.RatingGroup.RatingGroupID).RatingGroupID.Should().Equals(theTestHelper.RatingGroup.RatingGroupID);
            DataAccess.DataContext.GetTable<Rating>().Single(x => x.TopmostRatingGroupID == theTestHelper.RatingGroup.RatingGroupID).RatingGroup.RatingGroupID.Should().Equals(theTestHelper.RatingGroup.RatingGroupID);
        }
        
        public void TestCreateTableAndAddUserRatingFromFirstUser(bool isEvent)
        {
            if (isEvent)
                theTestHelper.CreateSimpleEventTestTable();
            else
                theTestHelper.CreateSimpleTestTable(true);
            theTestHelper.CreateUsers(7);
            UserRatingResponse theResponse = new UserRatingResponse();
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, isEvent ? 70M : 7M, theTestHelper.UserIds[0], ref theResponse);
            theTestHelper.WaitIdleTasks();
            Rating theRating = DataAccess.DataContext.GetTable<Rating>().Single(x => x.RatingID == theTestHelper.Rating.RatingID);
            theRating.CurrentValue.Should().Be(isEvent ? 70M : 7M);
        }

        [TestMethod]
        public void TestCreateTableAndAddLotsOfUserRatingFromFirstUser()
        {
            theTestHelper.CreateSimpleEventTestTable();
            theTestHelper.CreateUsers(7);
            UserRatingResponse theResponse = new UserRatingResponse();
            int numToAdd = 1100;
            decimal random = 0;
            for (int i = 1; i <= numToAdd; i++)
            {
                random = (decimal)Math.Round(RandomGenerator.GetRandom(), 3) * 100;
                theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, random, theTestHelper.UserIds[0], ref theResponse);
            }
            theTestHelper.WaitIdleTasks();
            (DataAccess.DataContext.GetTable<UserRating>().Count() >= numToAdd).Should().BeTrue();
            theTestHelper.Rating.CurrentValue.Should().Be(random);
        }

        public void TestAddRandomUserRatingFromAnotherUser(bool isEvent)
        {
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;
            decimal random = (decimal)Math.Round(RandomGenerator.GetRandom(),3); 
            if (random == 0.7M)
                random = 0.701M; // so it's not exactly same as first rating -- needed for our assertions to be correct
            UserRatingResponse theResponse = new UserRatingResponse();
            int userNumber = RandomGenerator.GetRandom(1, 6); // exclude first user who we are testing things about
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, isEvent ? random * 100 : random * 10, theTestHelper.UserIds[userNumber], ref theResponse);
            Trace.TraceInformation("Trying to add another HERE after UserRatingAdd " + random + " response of " + theResponse.result.success + " " + theResponse.currentValues.First().theUserRating);
            theTestHelper.WaitIdleTasks();
            Rating theRating = DataAccess.DataContext.GetTable<Rating>().Single(x => x.RatingID == theTestHelper.Rating.RatingID);
            theRating.CurrentValue.Should().Be(isEvent ? random * 100 : random * 10);
        }

        [TestMethod]
        public void TestAddUserRating_ShortTermResolution_LongTermResolution()
        {
            TestAddUserRating_ShortTermResolution_LongTermResolution_Helper(true);
            TestAddUserRating_ShortTermResolution_LongTermResolution_Helper(false);
        }

        public void TestAddUserRating_ShortTermResolution_LongTermResolution_Helper(bool addAdditionalUserRatingsAfterFirst)
        {
            Initialize();

            RandomGenerator.SeedOverride = 1; // DEBUG

            /* before pending */
            TestCreateTableAndAddUserRatingFromFirstUser(true);
            UserRating theUserRating = DataAccess.DataContext.GetTable<UserRating>().Single();
            User theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]);
            PointsTotal thePointsTotal = theUser.PointsTotals.Single();
            RatingGroup theRatingGroup = theUserRating.Rating.RatingGroup;
            theTestHelper.WaitIdleTasks(); 
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().Single(); // reload
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theUserRating.PointsHaveBecomePending.Should().BeFalse();
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeFalse();
            (theUserRating.NotYetPendingPointsLongTerm == 0).Should().BeFalse();
            (theUserRating.PendingPointsLongTerm == 0).Should().BeTrue();
            (theUserRating.PendingPointsShortTerm == 0).Should().BeTrue();
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeTrue();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            DateTime timeBeforePending = TestableDateTime.Now;
            var theRatingGroupPhaseStatusInitial = theUserRating.Rating.RatingGroup.RatingGroupPhaseStatus.OrderByDescending(x => x.ActualCompleteTime).First();

            /* before short term resolution */
            TimeSpan timeUntilPending = (DateTime) theUserRating.WhenPointsBecomePending - TestableDateTime.Now + TimeSpan.FromMinutes(1);
            TestableDateTime.SleepOrSkipTime((int) timeUntilPending.TotalMilliseconds);
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            else
                theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
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
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            if (addAdditionalUserRatingsAfterFirst)
                (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            else
                (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            decimal shortTermPointsEarnedInitially = theUserRating.PointsEarnedShortTerm;
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
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
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().NotBeNull();
            decimal longTermResolutionValueAfterInitialResolution = (decimal) theUserRating.LongTermResolutionValue;
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeFalse();
            decimal longTermPointsEarnedInitially = theUserRating.PointsEarnedLongTerm;
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);
            DateTime timeImmediatelyAfterInitialLongTermResolution = TestableDateTime.Now;

            /* cancelling initial long term resolution --> should be back to time after short term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            DateTime timeImmediatelyAfterCancellation = TestableDateTime.Now;

            /* recreating initial long term resolution */
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theTestHelper.Rating.CurrentValue.Should().Be(valueAtTimeOfInitialResolution);
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            if (!addAdditionalUserRatingsAfterFirst)
                theUserRating.PointsEarnedLongTerm.Should().Be(shortTermPointsEarnedInitially, "because short and long term are of equal weight");
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().Be(longTermResolutionValueAfterInitialResolution);
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);

            /* now cancel the resolution and re-resolve it as of an earlier time before short term resolution */
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeWhenPendingButNotResolvedShortTerm, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
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
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            if (!addAdditionalUserRatingsAfterFirst)
                theUserRating.PointsEarnedLongTerm.Should().Be(shortTermPointsEarnedInitially, "because short and long term are of equal weight");
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().Be(longTermResolutionValueAfterInitialResolution);
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);

            /* cancel resolution again and re-resolve it at time before the ratings were pending */
            TestableDateTime.SleepOrSkipTime(5000);
            cancelPreviousResolutions = true;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeBeforePending, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
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
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution /* shouldn't matter */, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            if (addAdditionalUserRatingsAfterFirst)
                TestAddRandomUserRatingFromAnotherUser(true);
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeFalse();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
            cancelPreviousResolutions = false;
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, effectiveTimeOfInitialResolution, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            theUserRating.PointsEarnedShortTerm.Should().Be(shortTermPointsEarnedInitially);
            if (!addAdditionalUserRatingsAfterFirst)
                theUserRating.PointsEarnedLongTerm.Should().Be(shortTermPointsEarnedInitially, "because short and long term are of equal weight");
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionValue.Should().Be(longTermResolutionValueAfterInitialResolution);
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially + longTermPointsEarnedInitially);


            /* now try to resolve by unwinding at three times -- also we'll simplify by not canceling first, allowing this to be automatically done*/
            cancelPreviousResolutions = false;
            resolveByUnwinding = true;

            /* first, before short term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeWhenPendingButNotResolvedShortTerm, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeTrue();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            (thePointsTotal.TotalPoints == 0).Should().BeTrue();


            /* second, after short term resolution before initial long term resolution */
            TestableDateTime.SleepOrSkipTime(5000);
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterShortTermResolution, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);


            /* third, after initial long term resolution (should have same results) */
            TestableDateTime.SleepOrSkipTime(5000);
            theRatingGroup = DataAccess.DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == theRatingGroup.RatingGroupID); // reload
            theTestHelper.ActionProcessor.ResolveRatingGroup(theRatingGroup, true, cancelPreviousResolutions, resolveByUnwinding, timeImmediatelyAfterInitialLongTermResolution, theTestHelper.UserIds[0], null);
            theTestHelper.WaitIdleTasks();
            theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().FirstOrDefault(x => x.Status == (int)StatusOfObject.Proposed).Should().BeNull();
            theUserRating = DataAccess.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).First(); // reload
            (theUserRating.PointsEarnedShortTerm == 0).Should().BeFalse();
            (theUserRating.PointsEarnedLongTerm == 0).Should().BeTrue();
            theUserRating.ShortTermResolutionReflected.Should().BeTrue();
            theUserRating.LongTermResolutionReflected.Should().BeTrue();
            theUser = DataAccess.DataContext.GetTable<User>().Single(u => u.UserID == theTestHelper.UserIds[0]); // reload user ...
            thePointsTotal = theUser.PointsTotals.Single(); // ... to reload pointstotal
            thePointsTotal.TotalPoints.Should().Be(shortTermPointsEarnedInitially);
        }

        [TestMethod]
        public void TestRatingResolution_AlternativeMethod()
        {
            theTestHelper.CreateSimpleEventTestTable();
            new TestRatingResolution().RatingResolutionTest(); 
        }

        [TestMethod]
        public void VolatilityRecordedProperly()
        {
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;

            const decimal minRating = 0M;
            const decimal maxRating = 10M;

            theTestHelper.CreateSimpleTestTable(true);
            theTestHelper.CreateUsers(10);
            // we don't care about user 1 but will add a user rating just to get things started at a particular rating value
            UserRatingResponse theResponse = new UserRatingResponse();
            decimal basis = 5M; // midpoint, in absence of ratings
            decimal user1RatingValue = 7M;
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user1RatingValue, theTestHelper.UserIds[1], ref theResponse);
            theTestHelper.WaitIdleTasks();

            float volatilityObservedDay;
            float volatilityObservedHour;
            float volatilityObservedWeek;
            GetVolatilityForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            float expectedAbsoluteVolatility = (float)Math.Abs(user1RatingValue - basis);
            float maximumVolatility = (float)(maxRating - minRating);
            float expectedRelativeVolatility = expectedAbsoluteVolatility / maximumVolatility; // starting from 0 volatility
            volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            decimal user2RatingValue = 8M;
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user2RatingValue, theTestHelper.UserIds[2], ref theResponse);
            theTestHelper.WaitIdleTasks();
            GetVolatilityForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            expectedAbsoluteVolatility = (float)Math.Abs(user2RatingValue - user1RatingValue);
            maximumVolatility = (float)(maxRating - minRating);
            expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            decimal user3RatingValue = 7M;
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user3RatingValue, theTestHelper.UserIds[3], ref theResponse);
            theTestHelper.WaitIdleTasks();
            GetVolatilityForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            expectedAbsoluteVolatility = (float)Math.Abs(user3RatingValue - user2RatingValue);
            maximumVolatility = (float)(maxRating - minRating);
            expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            // now add two at once by different users (we cannot use user 2 again because user 2 is not wholly trusted now that its rating was partly reverted)
            decimal user4RatingValue = 3M;
            decimal user5RatingValue = 10M;
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user4RatingValue, theTestHelper.UserIds[4], ref theResponse);
            theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, user5RatingValue, theTestHelper.UserIds[5], ref theResponse);
            theTestHelper.WaitIdleTasks();
            // DEBUG: We end up with errors here, because the PreviousDisplayedOrVirtualRating is 7M for both of users 4 and 5. It seems that when there are two simultaneous user ratings being added, with no idle tasks between them, both get full credit in terms of what the previous displayed rating was. This may make sense from the perspective of awarding points (or maybe not, since it could lead to redundant points), but certainly doesn't from the perspective of volatility, since simultaneous ratings will increase volatility. It would make sense to see if there is a way of determining what the most recent value is of the Rating when we add a UserRating, even if there has been some interim change.
            GetVolatilityForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            // count volatility for two separately
            // first for user 4's new userrating
            expectedAbsoluteVolatility = (float)Math.Abs(user4RatingValue - user3RatingValue);
            maximumVolatility = (float)(maxRating - minRating);
            expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            // now for user 5's new userrating
            expectedAbsoluteVolatility = (float)Math.Abs(user5RatingValue - user4RatingValue);
            maximumVolatility = (float)(maxRating - minRating);
            expectedRelativeVolatility += expectedAbsoluteVolatility / maximumVolatility; // add to previous
            // see if we're right
            volatilityObservedHour.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            // now, add time
            TestableDateTime.SleepOrSkipTime((long) TimeSpan.FromHours(23.1).TotalMilliseconds);
            theTestHelper.WaitIdleTasks();
            GetVolatilityForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            volatilityObservedHour.Should().BeApproximately(0, 0.01F);
            volatilityObservedDay.Should().BeApproximately(expectedRelativeVolatility, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            TestableDateTime.SleepOrSkipTime((long) TimeSpan.FromDays(5).TotalMilliseconds);
            theTestHelper.WaitIdleTasks();
            GetVolatilityForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            volatilityObservedHour.Should().BeApproximately(0, 0.01F);
            volatilityObservedDay.Should().BeApproximately(0, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(expectedRelativeVolatility, 0.01F);

            TestableDateTime.SleepOrSkipTime((long)TimeSpan.FromDays(3).TotalMilliseconds);
            theTestHelper.WaitIdleTasks();
            GetVolatilityForRating(theTestHelper.Rating, out volatilityObservedHour, out volatilityObservedDay, out volatilityObservedWeek);
            volatilityObservedHour.Should().BeApproximately(0, 0.01F);
            volatilityObservedDay.Should().BeApproximately(0, 0.01F);
            volatilityObservedWeek.Should().BeApproximately(0, 0.01F);
        }

        private void GetVolatilityForRating(Rating rating, out float volatilityObservedHour, out float volatilityObservedDay, out float volatilityObservedWeek)
        {
            RatingGroup rg = rating.RatingGroup;
            volatilityObservedHour = (float) rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneHour).Volatility;
            volatilityObservedDay = (float) rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneDay).Volatility;
            volatilityObservedWeek = (float) rg.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneWeek).Volatility;
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
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.RatingGroupResolutionID).First().CancelPreviousResolutions.Should().BeFalse();
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
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.RatingGroupResolutionID).First().CancelPreviousResolutions.Should().BeTrue();
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                TestHelperReportAllUserRatings(cycle, false, true, false);
            }
            for (int cycle = 1; cycle <= numCycles; cycle++)
            {
                Trace.TraceInformation("Re-resolving cycle " + cycle.ToString() + " time: " + originalDateTimes[cycle - 1].ToLongTimeString() + " to " + theDateTimes[cycle - 1].ToLongTimeString());
                theTestHelper.TestHelperResolveRatings(true, theDateTimes[cycle - 1], false, false);
                theTestHelper.WaitIdleTasks();
                numResolutions++;
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.RatingGroupResolutionID).First().CancelPreviousResolutions.Should().BeFalse();
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                TestHelperReportAllUserRatings(cycle, false, false, true);
                TestHelperCheckPoints(theUserRecords, theUserRatingRecords, cycle);
                Trace.TraceInformation("Re-canceling resolving cycle " + cycle.ToString());
                theTestHelper.TestHelperResolveRatings(true, TestableDateTime.Now, true, false); // now, unresolve
                theTestHelper.WaitIdleTasks();
                numResolutions++;
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.RatingGroupResolutionID).First().CancelPreviousResolutions.Should().BeTrue();
                theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                TestHelperReportAllUserRatings(cycle, false, true, true);
                if (cycle == numCycles)
                { // test resolve by unwinding
                    Trace.TraceInformation("Re-resolving cycle " + cycle.ToString() + " by unwinding");
                    theTestHelper.TestHelperResolveRatings(true, theDateTimes[cycle - 1], false, true);
                    theTestHelper.WaitIdleTasks();
                    numResolutions++;
                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.RatingGroupResolutionID).First().CancelPreviousResolutions.Should().BeFalse();

                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.RatingGroupResolutionID).First().ResolveByUnwinding.Should().BeTrue();
                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().Where(x => x.Status == (int)StatusOfObject.Active).Count().Should().Be(numResolutions);
                    Trace.TraceInformation("Long term points should be 0 for list below.");
                    TestHelperReportAllUserRatings(cycle + 100, false, true, true);

                    Trace.TraceInformation("Re-resolving cycle " + cycle.ToString() + " after unwinding");
                    theTestHelper.TestHelperResolveRatings(true, theDateTimes[cycle - 1], false, false);
                    theTestHelper.WaitIdleTasks();
                    numResolutions += 2; // because ti will first cancel and then add another resolution
                    theTestHelper.ActionProcessor.DataContext.GetTable<RatingGroupResolution>().OrderByDescending(x => x.RatingGroupResolutionID).First().CancelPreviousResolutions.Should().BeFalse();
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
            public int userID;
            public int afterCycle;
            public decimal points;

            public UserPointsRecordSnapshot(int theUserID, int theAfterCycle, decimal thePoints)
            {
                userID = theUserID;
                afterCycle = theAfterCycle;
                points = thePoints;
            }
        }

        internal class UserRatingPointsRecordSnapshot
        {
            public int predID;
            public int afterCycle;
            public decimal points;

            public UserRatingPointsRecordSnapshot(int thePredID, int theAfterCycle, decimal thePoints)
            {
                predID = thePredID;
                afterCycle = theAfterCycle;
                points = thePoints;
            }
        }

        internal void TestHelperReportAllUserRatings(int cycle, bool beforeResolve, bool afterUndo, bool secondPass)
        {
            var theUserRatings = theTestHelper.ActionProcessor.DataContext.GetTable<UserRating>().OrderBy(x => x.UserRatingID).Where(x => true);
            Debug.WriteLine("All user ratings: ");
            int userRatingNumber = 0;
            foreach (var theUserRating in theUserRatings)
            {
                userRatingNumber++;
                Debug.WriteLine("User rating: " + userRatingNumber + " current time: " + TestableDateTime.Now.ToLongTimeString());
                Debug.WriteLine(String.Format("RATING={7} CYCLE={0} PASS={1} BEFOREUNDO={2} ID={3} GROUP={4} MADE={5} USER={6}  \n ROUND={8} SHORT_REF={18} LONG_REF={19} SHORT_EARNED={9} LONG_EARNED={10} TOTAL={11} SHORTTIME={12} SHORTVAL={13} \n NOT_PENDING_ST={14} NOT_PENDING_LT={15} PEND={16} LONGVAL={17} \n"
                    , cycle, //0
                    secondPass ? 2 : 1, //1
                    afterUndo ? false : true, //2 
                    theUserRating.UserRatingID, //3
                    theUserRating.UserRatingGroupID, //4
                    theUserRating.UserRatingGroup.WhenMade.ToLongTimeString(), //5
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
                var theMPS = theUserRating.UserRatingGroup.RatingGroup.RatingGroupPhaseStatus.Single(x => x.StartTime <= theUserRating.UserRatingGroup.WhenMade && (x.ActualCompleteTime == null || x.ActualCompleteTime >= theUserRating.UserRatingGroup.WhenMade));
                Debug.Write(String.Format(" ROUND={0} START_TIME={1} END_TIME={2} \n", theMPS.RoundNum, theMPS.StartTime.ToLongTimeString(), theMPS.ActualCompleteTime.ToLongTimeString()));
                Debug.WriteLine("");
            }
            var theUsers = theTestHelper.ActionProcessor.DataContext.GetTable<PointsTotal>().Where(x => x.PointsTotalID > 0).ToList();
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
            var theUsers = theTestHelper.ActionProcessor.DataContext.GetTable<PointsTotal>().Where(x => x.PointsTotalID > 0).ToList();
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