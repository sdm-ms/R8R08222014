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
using Reflection = System.Reflection;

namespace TestProject1
{
    [TestClass]
    public class TestUserInteractionsAndTrust
    {
        TestHelper _testHelper;
        RaterooDataManipulation _dataManipulation;
        const float Precision = 0.0001F; // The allowable deviation from the "correct" answer that our calculations can be and still pass the tests

        [TestInitialize()]
        public void Initialize()
        {
            GetIRaterooDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds()); // go to next day
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            _testHelper = new TestHelper();
            _dataManipulation = new RaterooDataManipulation();
        }

        [TestMethod]
        public void AdjustmentFactorCalculatesCorrectlyInSeveralDifferentSituations()
        {
            PMAdjustmentFactor.CalculateAdjustmentFactor(6M, 6, 7, null).Should().BeApproximately(1.0F, 0f);
            PMAdjustmentFactor.CalculateAdjustmentFactor(7M, 6, 7, null).Should().BeApproximately(0, 0f);
            PMAdjustmentFactor.CalculateAdjustmentFactor(10, 6, 7, null)
                .Should().BeApproximately(-3F, 0f);
            PMAdjustmentFactor.CalculateAdjustmentFactor(6.5M, 6, 7, null).Should().BeApproximately(0.5F, 0f);

            PMAdjustmentFactor.CalculateAdjustmentFactor(1000M, 100M, 10M, 10M)
                .Should().BeApproximately(2.0F, 0f);
            PMAdjustmentFactor.CalculateAdjustmentFactor(100M, 1000M, 10M, 10M).Should().BeApproximately(0.5F, 0f);
            PMAdjustmentFactor.CalculateAdjustmentFactor(1M, 100M, 10M, 10M).Should().BeApproximately(-1.0F, 0f);

            PMAdjustmentFactor.CalculateAdjustmentFactor(27M, 9M, 3M, 3M)
                .Should().BeApproximately(2.0F, 0f);
            PMAdjustmentFactor.CalculateAdjustmentFactor(9M, 27M, 3M, 3M).Should().BeApproximately(0.5F, 0f);
            PMAdjustmentFactor.CalculateAdjustmentFactor(1M, 9M, 3M, 3M).Should().BeApproximately(-1.0F, 0f);
        }

        [TestMethod]
        public void TestAdjustmentPcts_InReverse()
        {
            PMAdjustmentFactor.GetRatingToAcceptFromAdjustmentFactor(5M, 6M, 0.50F, null).Should().Be(5.5M);
            PMAdjustmentFactor.GetRatingToAcceptFromAdjustmentFactor(5M, 3M, 0.10F, null).Should().Be(4.8M);
            PMAdjustmentFactor.GetRatingToAcceptFromAdjustmentFactor(5M, 3M, 2.00F, null).Should().Be(3.0M, "because adjustment pcts higher than 1 are treated as 1");
            PMAdjustmentFactor.GetRatingToAcceptFromAdjustmentFactor(5M, 3M, -1.0F, null).Should().Be(5.0M, "because adjustment pcts lower than 0 are treated as 0");

            PMAdjustmentFactor.GetRatingToAcceptFromAdjustmentFactor(10M, 1000M, 0.5F, 10M).Should().Be(100M);
        }

        [TestMethod]
        public void RatingHasCorrectRatingValuesAfter1TrustedUserRating()
        {
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(2); // Must create one more user than needed...Not sure why.  Maybe to make room for the SuperUser?  But why doesn't the SuperUser creation make its own room?

            UserRatingResponse theResponse = new UserRatingResponse();
            decimal userRatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, userRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            _testHelper.Rating.CurrentValue.Should().Be(userRatingValue);
            _testHelper.Rating.LastTrustedValue.Should().Be(userRatingValue);
        }

        [TestMethod]
        public void RatingHasCorrectRatingValuesAfter2TrustedUserRatings()
        {
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(3); // Must create one more user than needed...Not sure why.  Maybe to make room for the SuperUser?  But why doesn't the SuperUser creation make its own room?

            UserRatingResponse theResponse = new UserRatingResponse();
            decimal user1RatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1RatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user2RatingValue = 8M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2RatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            _testHelper.Rating.CurrentValue.Should().Be(user2RatingValue);
            _testHelper.Rating.LastTrustedValue.Should().Be(user2RatingValue);
        }

        [TestMethod]
        public void RatingHasCorrectRatingValuesAfter3TrustedUserRatings()
        {
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(4);

            UserRatingResponse theResponse = new UserRatingResponse();
            decimal user1RatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1RatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user2RatingValue = 8M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2RatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user3RatingValue = 5M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3RatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();

            _testHelper.Rating.CurrentValue.Should().Be(user3RatingValue);
            _testHelper.Rating.LastTrustedValue.Should().Be(user3RatingValue);
        }

        [TestMethod]
        public void UserInteractionStatsAreCorrectBetweenTwoTrustedUsersWhoSequentiallyRateARatingPreviouslyRatedByOneOtherTrustedUserWhereAdjustmentPercentageIsBetween0And1()
        {
            const decimal maxRating = 10M;
            const decimal minRating = 0M;

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(4); // Must create one more user than needed...Not sure why.  Maybe to make room for the SuperUser?  But why doesn't the SuperUser creation make its own room?

            UserRatingResponse theResponse = new UserRatingResponse();
            decimal user1Rating1UserRatingValue = 7M;
            // Must ensure that this rating is trusted
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1Rating1UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();
            _testHelper.Rating.CurrentValue.Should().Be(user1Rating1UserRatingValue);

            decimal user2Rating1UserRatingValue = 8M;
            decimal user3Rating1UserRatingValue = 7.5M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2Rating1UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3Rating1UserRatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();

            UserInteraction user2User3Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>()
                .Single(x =>
                    x.User.UserID == _testHelper.UserIds[2] &&
                    x.User1.UserID == _testHelper.UserIds[3]);
            List<UserInteractionStat> user2User3InteractionStats = user2User3Interaction.UserInteractionStats.ToList();

            decimal basisRatingValue = user1Rating1UserRatingValue;
            decimal ratingValue = user2Rating1UserRatingValue;
            decimal currentRatingValue = user3Rating1UserRatingValue;
            float expectedAdjustmentFactor = PMAdjustmentFactor.CalculateAdjustmentFactor(currentRatingValue, ratingValue, basisRatingValue, null);

            expectedAdjustmentFactor.Should().BeInRange(0.0F, 1.0F, "because this test is specifically intended to test adjustment factors between 0F and 1F.");

            float ratingMagnitude = PMAdjustmentFactor.CalculateRelativeMagnitude(ratingValue, basisRatingValue, minRating, maxRating, null);
            float expectedAbsoluteVolatility = (float)Math.Abs(ratingValue / basisRatingValue);
            float maximumVolatility = (float)(maxRating - minRating);
            float expectedRelativeVolatility = expectedAbsoluteVolatility / maximumVolatility;

            List<float> expectedStats = new List<float>(TrustTrackerStatManager.NumStats);
            float expectedNoExtraWeightingStat1 = (1 * ratingMagnitude);
            expectedStats.Add(expectedNoExtraWeightingStat1);
            float expectedLargeDeltaRatingStat1 = (float)Math.Pow(ratingMagnitude, 2) * ratingMagnitude;
            expectedStats.Add(expectedLargeDeltaRatingStat1);
            expectedStats.Add((float)Math.Pow(1 - ratingMagnitude, 2) * ratingMagnitude);

            // Volatility is zero, for some reason.
            //expectedStats.Add(expectedRelativeVolatility * ratingMagnitude);
            //expectedStats.Add(expectedRelativeVolatility * ratingMagnitude);
            expectedStats.Add(0);
            expectedStats.Add(0);

            expectedStats.Add(PMAdjustmentFactor.CalculateRelativeMagnitude(ratingValue, (maxRating - minRating) / 2, minRating, maxRating, null) * ratingMagnitude);
            
            // Both zero, for some reason
            //expectedStats.Add(
            //    PMAdjustmentPercentages.CalculateRelativeMagnitude(currentRatingValue, basisRatingValue, minRating, maxRating, null) * ratingMagnitude);
            //expectedStats.Add(
            //    (1F / // this user has rated the rating once
            //    3F) * // Three users have rating this rating in total
            //    ratingMagnitude);
            expectedStats.Add(0);
            expectedStats.Add(0);

            List<float> expectedAverageAdjustmentPercentageWeightedByStats = new List<float>();
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
                expectedAverageAdjustmentPercentageWeightedByStats.Add((expectedAdjustmentFactor * expectedStats[i]) / (expectedStats[i]));

            List<float> actualAverageAdjustmentFactorWeightedByStats = new List<float>();
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
                actualAverageAdjustmentFactorWeightedByStats.Add(user2User3InteractionStats[i].AvgAdjustmentPctWeighted);

            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
                actualAverageAdjustmentFactorWeightedByStats[i]
                    .Should().BeApproximately(expectedAverageAdjustmentPercentageWeightedByStats[i], 0F);
        }

        [TestMethod]
        public void UserInteractionStatsAreZeroBetweenTwoUsersWhoRateARatingPreviouslyRatedByATrustedUserAndThenSubsequentlyRatedByAnotherTrustedUser()
        {
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(5); // Must create one more user than needed...Not sure why.  Maybe to make room for the SuperUser?  But why doesn't the SuperUser creation make its own room?

            UserRatingResponse theResponse = new UserRatingResponse();

            decimal user1Rating1UserRatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1Rating1UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user2Rating1UserRatingValue = 8M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2Rating1UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user3Rating1UserRatingValue = 7.5M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3Rating1UserRatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();

            /* Have another user replace user 3 as the latest-rating user */
            decimal user4Rating1UserRatingValue = 9M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user4Rating1UserRatingValue, _testHelper.UserIds[4], ref theResponse);
            _testHelper.WaitIdleTasks();
        }

        [TestMethod]
        public void UserInteractionStatsForAUserWhoRatesARatingPreviouslyRatedByThreeTrustedUsersShouldBeStableAcrossWaitIdleTasksAndRecalculation()
        {
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(5);

            UserRatingResponse theResponse = new UserRatingResponse();

            decimal user1UserRatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user2UserRatingValue = 8M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user3UserRatingValue = 7.5M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3UserRatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();

            UserInteraction user2User3Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>()
                .Single(x =>
                    x.User.UserID == _testHelper.UserIds[2] &&
                    x.User1.UserID == _testHelper.UserIds[3]);
            List<UserInteractionStat> user2User3InteractionStats = user2User3Interaction.UserInteractionStats.ToList();

            /* Have another user replace user 3 as the latest-rating user */
            decimal user4UserRatingValue = 8.1M; // Should produce a retrospective adjustment factor of 1.1
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user4UserRatingValue, _testHelper.UserIds[4], ref theResponse);
            _testHelper.WaitIdleTasks();

            // becauser user 4 is now the latest user to rate, user 2's adjustment factor will depend on user 2 and user 4's ratings.
            //float ratingMagnitude = 
            //    (float)((user4Rating1UserRatingValue - user1Rating1UserRatingValue) / (user2Rating1UserRatingValue - user1Rating1UserRatingValue));
            float ratingMagnitude =
                PMAdjustmentFactor.CalculateRelativeMagnitude(user4UserRatingValue, user2UserRatingValue,
                    0M, 10M, logBase:null);
            float noWeightingStat = 1f * ratingMagnitude;
            float adjustmentFactor = PMAdjustmentFactor.CalculateAdjustmentFactor(user4UserRatingValue,
                user2UserRatingValue, user1UserRatingValue);
            float averageAdjustmentFactorWeightedByNoWeightingStat =
                (noWeightingStat * adjustmentFactor) /
                noWeightingStat; // b/c there is only one user rating contributing here

            /* look at user interaction with user 2 and 4 */
            UserInteraction user2User4Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x =>
                x.User.UserID == _testHelper.UserIds[2] &&
                x.User1.UserID == _testHelper.UserIds[4]);
            List<UserInteractionStat> user2User4InteractionStats = user2User4Interaction.UserInteractionStats.ToList();
            float adjustmentPctCalculatedInUserInteraction = user2User4InteractionStats[0].AvgAdjustmentPctWeighted;
            adjustmentPctCalculatedInUserInteraction.Should().BeApproximately(1.1F, 0.01F, "because rating moved 110 percent of user 2's movement as of user 4 entry");
            adjustmentPctCalculatedInUserInteraction.Should().BeApproximately(averageAdjustmentFactorWeightedByNoWeightingStat, 0.01F, "because manual calculation should produce same result");

            /* running idle tasks shouldn't change this */
            _testHelper.WaitIdleTasks();
            user2User4Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x =>
                x.User.UserID == _testHelper.UserIds[2] &&
                x.User1.UserID == _testHelper.UserIds[4]);
            user2User4InteractionStats = user2User4Interaction.UserInteractionStats.ToList();
            user2User4InteractionStats[0].AvgAdjustmentPctWeighted
                .Should().BeApproximately(averageAdjustmentFactorWeightedByNoWeightingStat, Precision);

            /* updating the rating shouldn't change this */
            var mostRecentUserRating = _dataManipulation.DataContext.GetTable<UserRating>().OrderByDescending(x => x.UserRatingID).First();
            mostRecentUserRating.ForceRecalculate = true;
            _dataManipulation.DataContext.SubmitChanges();
            _testHelper.WaitIdleTasks();
            user2User4Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x =>
                x.User.UserID == _testHelper.UserIds[2] &&
                x.User1.UserID == _testHelper.UserIds[4]);
            user2User4InteractionStats = user2User4Interaction.UserInteractionStats.ToList();
            user2User4InteractionStats[0].AvgAdjustmentPctWeighted
                .Should().BeApproximately(averageAdjustmentFactorWeightedByNoWeightingStat, Precision);
        }

        /// <summary>
        /// I think the idea is that the test is being applied to user 3 (because we need to have an easy way to see whether the 
        /// previous rating is questionable, so we need this to be at least the third rating). So, I think that lastTrustedRating = 7, 
        /// currentRating = 8, and enteredRating = 7.5. But I think for this to work, we need to be sure that user 1's rating will be 
        /// trusted and user 2's rating will not be trusted, and I don't see anything in this code that does it. So that may be 
        /// something that I neglected, or maybe there is something that I'm not thinking of (perhaps I ensured that in the initialization 
        /// routines for the table). Feel free to change as necessary so that the test is actually testing correctly.
        /// </summary>
        [TestMethod]
        public void UserInteractionStatsForTwoUsersWhoEachRateTwoRatingsShouldEqualPredictedValues()
        {
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true; // we're looking at user interaction stats, so it's easiest to test this way
            PMTrustCalculations.NumPerfectScoresToGiveNewUser = 0;
            TrustTrackerStatManager.MinAdjustmentFactorToCreditUserRating = 0;

            const decimal minRating = 0M;
            const decimal maxRating = 10M;

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(5);

            UserRatingResponse theResponse = new UserRatingResponse();

            decimal user1Rating1UserRatingValue = 1M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1Rating1UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user2Rating1UserRatingValue = 10M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2Rating1UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user3Rating1UserRatingValue = 2M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3Rating1UserRatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();

            /* Have another user replace user 3 as the latest-rating user */
            decimal user4Rating1UserRatingValue = 6M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user4Rating1UserRatingValue, _testHelper.UserIds[4], ref theResponse);
            _testHelper.WaitIdleTasks();

            /* Now let's add other ratings that will make for another user interaction between users 2 and 4 */

            TestableDateTime.SleepOrSkipTime(50000);
            _testHelper.WaitIdleTasks();
            //theTestHelper.WaitIdleTasks(); // There's no reason to wait twice on idle tasks, is there?
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, 1);
            _testHelper.WaitIdleTasks();

            Rating rating2 = _dataManipulation.DataContext.GetTable<Rating>().OrderByDescending(x => x.RatingID).First(); // TODO this is an example of where TestHelper is not being useful; relying upon the highest ID rating to be the most recent is bad form.  It's probably always correct, but it would make for one heck of a bug if for some reason it were not.  It would be much better if TestHelper had something like TestHelper.CreateRating(...) returning the Rating.
            int rating2Id = rating2.RatingID;

            decimal user1Rating2UserRatingValue = 9.5M;
            _testHelper.ActionProcessor.UserRatingAdd(rating2Id, user1Rating2UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            rating2 = _dataManipulation.DataContext.GetTable<Rating>().Single(x => x.RatingID == rating2.RatingID); // reload it

            rating2.CurrentValue.Should().Be(user1Rating2UserRatingValue);
            rating2.LastTrustedValue.Should().Be(user1Rating2UserRatingValue);

            decimal user2Rating2UserRatingValue = 1.0M;
            _testHelper.ActionProcessor.UserRatingAdd(rating2Id, user2Rating2UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            rating2 = _dataManipulation.DataContext.GetTable<Rating>().Single(x => x.RatingID == rating2.RatingID); // reload it

            rating2.CurrentValue.Should().Be(user2Rating2UserRatingValue);
            rating2.LastTrustedValue.Should().Be(user2Rating2UserRatingValue);
            
            { // Debug
                Debug.WriteLine(String.Format("<Before> rating2.CurrentValue: {0}, user4.SkepticalTrust/OverallTrust: {1}/{2}", 
                    rating2.CurrentValue, 
                    _testHelper.ActionProcessor.DataContext.GetTable<TrustTracker>()
                        .Single(tt  => tt.UserID == _testHelper.UserIds[4])
                        .SkepticalTrustLevel,
                    _testHelper.ActionProcessor.DataContext.GetTable<TrustTracker>()
                        .Single(tt  => tt.UserID == _testHelper.UserIds[4])
                        .OverallTrustLevel
                        ));
            }
            decimal user4Rating2UserRatingValue = 10M;
            _testHelper.ActionProcessor.UserRatingAdd(rating2Id, user4Rating2UserRatingValue, _testHelper.UserIds[4], ref theResponse); // -200%
            _testHelper.WaitIdleTasks();
            {
                // Debug
                Debug.WriteLine(String.Format("<After> rating2.CurrentValue: {0}, user4.SkepticalTrust/OverallTrust: {1}/{2}", 
                    rating2.CurrentValue, 
                    _testHelper.ActionProcessor.DataContext.GetTable<TrustTracker>()
                        .Single(tt  => tt.UserID == _testHelper.UserIds[4])
                        .SkepticalTrustLevel,
                    _testHelper.ActionProcessor.DataContext.GetTable<TrustTracker>()
                        .Single(tt  => tt.UserID == _testHelper.UserIds[4])
                        .OverallTrustLevel
                        ));
            }

            //rating2.CurrentValue.Should().Be(user4Rating2UserRatingValue);
            //rating2.LastTrustedValue.Should().Be(user4Rating2UserRatingValue);

            UserInteraction user2User4Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x =>
                x.User.UserID == _testHelper.UserIds[2] &&
                x.User1.UserID == _testHelper.UserIds[4]);
            List<UserInteractionStat> user2User4InteractionStats = user2User4Interaction.UserInteractionStats.ToList();

            decimal basisRatingValue1 = user1Rating1UserRatingValue;
            decimal ratingValue1 = user2Rating1UserRatingValue;
            decimal currentRatingValue1 = user4Rating1UserRatingValue;
            //float adjustmentPercentage1 = PMAdjustmentPercentages.GetAdjustmentPctFromRatings(currentRating1, enteredRating1, lastTrustedRating1, null);
            //float ratingMagnitude1 = PMAdjustmentPercentages.GetRatingMagnitude(lastTrustedRating1, enteredRating1, null, minRating, maxRating);
            float adjustmentPercentage1 = PMAdjustmentFactor.CalculateAdjustmentFactor(
                laterValue: currentRatingValue1, 
                enteredValue: ratingValue1, 
                basisValue: basisRatingValue1, 
                logBase: null);
            float ratingMagnitude1 = PMAdjustmentFactor.CalculateRelativeMagnitude(value: ratingValue1, basisValue: basisRatingValue1, minValue: minRating, maxValue: maxRating, logBase: null);

            decimal basisRatingValue2 = user1Rating2UserRatingValue;
            decimal ratingValue2 = user2Rating2UserRatingValue;
            decimal currentRatingValue2 = user4Rating2UserRatingValue;
            //float adjustmentPercentage2 = PMAdjustmentPercentages.GetAdjustmentPctFromRatings(currentRating2, enteredRating2, lastTrustedRating2, null);
            //float ratingMagnitude2 = PMAdjustmentPercentages.GetRatingMagnitude(lastTrustedRating2, enteredRating2, null, minRating, maxRating);
            float adjustmentPercentage2 = PMAdjustmentFactor.CalculateAdjustmentFactor(
                laterValue: currentRatingValue2, 
                enteredValue: ratingValue2, 
                basisValue: basisRatingValue2, 
                logBase: null);
            float ratingMagnitude2 = PMAdjustmentFactor.CalculateRelativeMagnitude(value: ratingValue2, basisValue: basisRatingValue2, minValue: minRating, maxValue: maxRating, logBase: null);

            // Test the NoExtraWeightingStat

            float noExtraWeightingStat1 = 1 * ratingMagnitude1;
            float noExtraWeightingStat2 = 1 * ratingMagnitude2;

            float SumAdjustmentPercentageTimesWeightsNoExtraWeightingStat2 =
                0 + noExtraWeightingStat1 * adjustmentPercentage1 + noExtraWeightingStat2 * adjustmentPercentage2;
            float sumWeightNoExtraWeightingStat2 =
                0 + noExtraWeightingStat1 + noExtraWeightingStat2;
            float averageAdjustmentPercentageWeightedByNoExtraWeightingStat2 =
                SumAdjustmentPercentageTimesWeightsNoExtraWeightingStat2 /
                    sumWeightNoExtraWeightingStat2;


            user2User4InteractionStats[0].AvgAdjustmentPctWeighted
                .Should().BeApproximately(averageAdjustmentPercentageWeightedByNoExtraWeightingStat2, Precision);

            // Test the LargeDeltaRating Stat

            float largeDeltaRatingStat1 = (float)Math.Pow(ratingMagnitude1, 2) * ratingMagnitude1;
            float largeDeltaRatingStat2 = (float)Math.Pow(ratingMagnitude2, 2) * ratingMagnitude2;
            float SumAdjustmentPercentageTimesWeightsLargeDeltaRatingStat2 =
                0 + largeDeltaRatingStat1 * adjustmentPercentage1 + largeDeltaRatingStat2 * adjustmentPercentage2;
            float sumWeightLargeDeltaRatingStat2 = 0 + largeDeltaRatingStat1 + largeDeltaRatingStat2;

            user2User4InteractionStats[1].SumAdjustPctTimesWeight
                .Should().BeApproximately(SumAdjustmentPercentageTimesWeightsLargeDeltaRatingStat2, Precision);
            user2User4InteractionStats[1].SumWeights.Should().BeApproximately(sumWeightLargeDeltaRatingStat2, Precision);
            user2User4InteractionStats[1].AvgAdjustmentPctWeighted
                .Should().BeApproximately(SumAdjustmentPercentageTimesWeightsLargeDeltaRatingStat2 / sumWeightLargeDeltaRatingStat2, Precision);

            float ratingMagnitudesSumWeights = ((10F - 1F) + (9.5F - 1F)) / (float)(maxRating - minRating);
            user2User4Interaction.WeightInCalculatingTrustTotal
                .Should().BeApproximately(
                    (ratingMagnitudesSumWeights / 2) * (float)Math.Log(1 + 2, 10.0) * user2User4Interaction.LatestUserEgalitarianTrust,
                    Precision,
                    "because the more transactions we have, the greater the weight, by we increase this only logarithmically.");
        }

        [TestMethod]
        public void WeightInCalculatingTrustTotalForTwoUsersWhoEachRateTwoRatingsShouldEqualPredictedValue()
        {
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;

            const decimal minRating = 0M;
            const decimal maxRating = 10M;

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(5);

            UserRatingResponse theResponse = new UserRatingResponse();

            decimal user1Rating1UserRatingValue = 1M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1Rating1UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user2Rating1UserRatingValue = 10M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2Rating1UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user3Rating1UserRatingValue = 2M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3Rating1UserRatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();

            /* Have another user replace user 3 as the latest-rating user */
            decimal user4Rating1UserRatingValue = 6M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user4Rating1UserRatingValue, _testHelper.UserIds[4], ref theResponse);
            _testHelper.WaitIdleTasks();

            /* Now let's add other ratings that will make for another user interaction between users 2 and 4 */

            TestableDateTime.SleepOrSkipTime(50000);
            _testHelper.WaitIdleTasks();
            //theTestHelper.WaitIdleTasks(); // There's no reason to wait twice on idle tasks, is there?
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, 1);
            _testHelper.WaitIdleTasks();

            Rating rating2 = _dataManipulation.DataContext.GetTable<Rating>().OrderByDescending(x => x.RatingID).First(); // TODO this is an example of where TestHelper is not being useful; relying upon the highest ID rating to be the most recent is bad form.  It's probably always correct, but it would make for one heck of a bug if for some reason it were not.  It would be much better if TestHelper had something like TestHelper.CreateRating(...) returning the Rating.
            int rating2Id = rating2.RatingID;

            decimal user1Rating2UserRatingValue = 9.5M;
            _testHelper.ActionProcessor.UserRatingAdd(rating2Id, user1Rating2UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            rating2 = _dataManipulation.DataContext.GetTable<Rating>().Single(x => x.RatingID == rating2.RatingID); // reload it

            rating2.CurrentValue.Should().Be(user1Rating2UserRatingValue);
            rating2.LastTrustedValue.Should().Be(user1Rating2UserRatingValue);

            decimal user2Rating2UserRatingValue = 1.0M;
            _testHelper.ActionProcessor.UserRatingAdd(rating2Id, user2Rating2UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            rating2 = _dataManipulation.DataContext.GetTable<Rating>().Single(x => x.RatingID == rating2.RatingID); // reload it

            rating2.CurrentValue.Should().Be(user2Rating2UserRatingValue);
            rating2.LastTrustedValue.Should().Be(user2Rating2UserRatingValue);

            decimal user4Rating2UserRatingValue = 10M;
            _testHelper.ActionProcessor.UserRatingAdd(rating2Id, user4Rating2UserRatingValue, _testHelper.UserIds[4], ref theResponse); // -200%
            _testHelper.WaitIdleTasks();

            //rating2.CurrentValue.Should().Be(user4Rating2UserRatingValue);
            //rating2.LastTrustedValue.Should().Be(user4Rating2UserRatingValue);

            UserInteraction user2User4Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x =>
                x.User.UserID == _testHelper.UserIds[2] &&
                x.User1.UserID == _testHelper.UserIds[4]);
            List<UserInteractionStat> user2User4InteractionStats = user2User4Interaction.UserInteractionStats.ToList();

            decimal basisRatingValue1 = user1Rating1UserRatingValue;
            decimal ratingValue1 = user2Rating1UserRatingValue;
            decimal currentRatingValue1 = user4Rating1UserRatingValue;
            //float adjustmentPercentage1 = PMAdjustmentPercentages.GetAdjustmentPctFromRatings(currentRating1, enteredRating1, lastTrustedRating1, null);
            //float ratingMagnitude1 = PMAdjustmentPercentages.GetRatingMagnitude(lastTrustedRating1, enteredRating1, null, minRating, maxRating);
            float adjustmentPercentage1 = PMAdjustmentFactor.CalculateAdjustmentFactor(
                laterValue: currentRatingValue1,
                enteredValue: ratingValue1,
                basisValue: basisRatingValue1,
                logBase: null);
            float ratingMagnitude1 = PMAdjustmentFactor.CalculateRelativeMagnitude(value: ratingValue1, basisValue: basisRatingValue1, minValue: minRating, maxValue: maxRating, logBase: null);

            decimal basisRatingValue2 = user1Rating2UserRatingValue;
            decimal ratingValue2 = user2Rating2UserRatingValue;
            decimal currentRatingValue2 = user4Rating2UserRatingValue;
            //float adjustmentPercentage2 = PMAdjustmentPercentages.GetAdjustmentPctFromRatings(currentRating2, enteredRating2, lastTrustedRating2, null);
            //float ratingMagnitude2 = PMAdjustmentPercentages.GetRatingMagnitude(lastTrustedRating2, enteredRating2, null, minRating, maxRating);
            float adjustmentPercentage2 = PMAdjustmentFactor.CalculateAdjustmentFactor(
                laterValue: currentRatingValue2,
                enteredValue: ratingValue2,
                basisValue: basisRatingValue2,
                logBase: null);
            float ratingMagnitude2 = PMAdjustmentFactor.CalculateRelativeMagnitude(value: ratingValue2, basisValue: basisRatingValue2, minValue: minRating, maxValue: maxRating, logBase: null);

            float ratingMagnitudesSumWeights = (float)(
                // I'm not 100% sure that these are the correct variables to use in this calculation; they just fit the constants I replaced them with.
                // Originally was: ((10F - 1F) + (9.5F - 1F)) / (float)(maxRating - minRating);
                ((user4Rating2UserRatingValue - user2Rating2UserRatingValue) + (user1Rating2UserRatingValue - user2Rating2UserRatingValue)) /
                    (maxRating - minRating));
            user2User4Interaction.WeightInCalculatingTrustTotal
                .Should().BeApproximately(
                    (ratingMagnitudesSumWeights / 2) * (float)Math.Log(2 + 1, 10.0) * user2User4Interaction.LatestUserEgalitarianTrust,
                    Precision,
                    "because the more transactions we have, the greater the weight, by we increase this only logarithmically.");
        }

        /// <summary>
        /// I think the idea is that the test is being applied to user 3 (because we need to have an easy way to see whether the previous rating 
        /// is questionable, so we need this to be at least the third rating). So, I think that lastTrustedRating = 7, currentRating = 8, and 
        /// enteredRating = 7.5. But I think for this to work, we need to be sure that user 1's rating will be trusted and user 2's rating will 
        /// not be trusted, and I don't see anything in this code that does it. So that may be something that I neglected, or maybe there is 
        /// something that I'm not thinking of (perhaps I ensured that in the initialization routines for the table). Feel free to change as 
        /// necessary so that the test is actually testing correctly.
        /// 
        /// It does look right to me. But a couple of ideas: (1) You might look carefully at the number that you do get, and figure out how 
        /// you get that number from the numbers that you have by playing with the math. If you can't figure it out that way, of course you 
        /// could put a break point at the relevant points of the code to see where it's getting its numbers from, and then we need to figure
        ///  out if the discrepancy is a problem with the test or with the code. (2) One possibility is that it's not trusting the users we 
        /// think -- I can't remember why we think that users 1, 3, and 4 are trusted, but not 2. (Ideally, that would be clearer in the test.) 
        /// (3) relativeVolatility still looks wrong, though I don't think that's causing the result.
        /// </summary>
        [TestMethod]
        public void TestTrustTracker_CalculatesUserInteractionCorrectly_WhereAdjPctIsGreaterThan1()
        {
            PMTrustCalculations.NumPerfectScoresToGiveNewUser = 0; // for purpose of this test, do not start user with extra trust

            const decimal minRating = 0M;
            const decimal maxRating = 10M;

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(10);
            // we don't care about user 1 but will add a user rating just to get things started at a particular rating value
            UserRatingResponse theResponse = new UserRatingResponse();
            decimal user1RatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1RatingValue, _testHelper.UserIds[1], ref theResponse);
            /* move the rating back and forth to create some volatility -- use users 4 and 5, whom we're not focusing on */
            decimal user4RatingValue = 8M;
            decimal user5RatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user4RatingValue, _testHelper.UserIds[4], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user5RatingValue, _testHelper.UserIds[5], ref theResponse);
            _testHelper.WaitIdleTasks();
            TestableDateTime.SleepOrSkipTime(1000 * 60 * 60 * 2); // move forward 2 hours so that we have volatility for the day but not the hour as of user 2's rating
            _testHelper.WaitIdleTasks();
            _testHelper.Rating.CurrentValue.Should().Be(7M);

            /* see what the volatility values are expected and are actually as of just before user 2's rating */
            float volatilityObservedDay = (float)_testHelper.Rating.RatingGroup.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneDay).Volatility;
            float volatilityObservedHour = (float)_testHelper.Rating.RatingGroup.VolatilityTrackers.Single(x => x.DurationType == (int)VolatilityDuration.oneHour).Volatility;

            decimal user2RatingValue = 8M;
            decimal user3RatingValue = 9M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2RatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3RatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();

            UserInteraction user2User3Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x =>
                x.User.UserID == _testHelper.UserIds[2] &&
                x.User1.UserID == _testHelper.UserIds[3]);
            List<UserInteractionStat> user2User3InteractionStats = user2User3Interaction.UserInteractionStats.ToList();

            decimal basisRatingValue = user1RatingValue;
            decimal ratingValue = user2RatingValue;
            decimal currentRatingValue = user3RatingValue;
            float unconstrainedAdjustmentFactor = PMAdjustmentFactor.CalculateAdjustmentFactor(currentRatingValue, ratingValue, basisRatingValue, null, constrainForRetrospectiveAssessment: false);
            unconstrainedAdjustmentFactor.Should().BeApproximately(2.0F, 0.01F, "because user 3 moved the rating twice as far as user 1 moved it");
            float expectedAdjustmentFactor = PMAdjustmentFactor.CalculateAdjustmentFactor(currentRatingValue, ratingValue, basisRatingValue, null, constrainForRetrospectiveAssessment:true);
            expectedAdjustmentFactor.Should().BeGreaterThan(1.0F, "because this test is specifically intended to test adjustment factors greater than 1F");
            if (PMAdjustmentFactor.MaximumRetrospectiveAdjustmentFactor < 2.0)
                expectedAdjustmentFactor.Should().BeApproximately(PMAdjustmentFactor.MaximumRetrospectiveAdjustmentFactor, 0.01F, "because a constrained adjustment factor should be no more than the maximum retrospective adjustment factor");

            float ratingMagnitude = PMAdjustmentFactor.CalculateRelativeMagnitude(ratingValue, basisRatingValue, minRating, maxRating, null);

            List<float> expectedStats = new List<float>(TrustTrackerStatManager.NumStats);
            float expectedNoExtraWeightingStat1 = (1 * ratingMagnitude);
            expectedStats.Add(expectedNoExtraWeightingStat1);
            float expectedLargeDeltaRatingStat1 = (float)Math.Pow(ratingMagnitude, 2) * ratingMagnitude;
            expectedStats.Add(expectedLargeDeltaRatingStat1);
            expectedStats.Add((float)Math.Pow(1 - ratingMagnitude, 2) * ratingMagnitude);

            expectedStats.Add(volatilityObservedDay); // last day volatility
            expectedStats.Add(volatilityObservedHour); // last hour volatility

            float expectedExtremeness = PMAdjustmentFactor.CalculateRelativeMagnitude(ratingValue, (maxRating - minRating) / 2, minRating, maxRating, null) * ratingMagnitude;
            expectedStats.Add(expectedExtremeness);
            
            expectedStats.Add(0); // current rating questionable is 0 because the last rating = last trusted rating, since everyone is trusted
            expectedStats.Add(0); // percent previous ratings -- user 2 hasn't rated beore

            List<float> expectedAverageAdjustmentPercentageWeightedByStats = new List<float>();
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
            {
                expectedAverageAdjustmentPercentageWeightedByStats.Add(expectedStats[i] == 0 ? 0 : (0 + expectedAdjustmentFactor * expectedStats[i]) / (0 + expectedStats[i])); // there is only one weight, i.e. the relevant statistic, per statistic, so this should just be the expected adjustment factor, but the whole equation is included for clarity
                double.IsNaN(expectedAverageAdjustmentPercentageWeightedByStats[i]).Should().BeFalse();
            }

            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
                user2User3InteractionStats[i].AvgAdjustmentPctWeighted
                    .Should().BeApproximately(expectedAverageAdjustmentPercentageWeightedByStats[i], 0F, 
                        String.Format("because stat {0} should have the expected value.", i));

            TrustTracker theTrustTracker = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(x => x.UserID == _testHelper.UserIds[2]);
            theTrustTracker.OverallTrustLevel.Should().BeApproximately(PMAdjustmentFactor.MaximumRetrospectiveAdjustmentFactor, 0.0F, "because trust levels are constrained to being between " + PMAdjustmentFactor.MinimumRetrospectiveAdjustmentFactor + " and " + PMAdjustmentFactor.MaximumRetrospectiveAdjustmentFactor);
        }

        [TestMethod]
        public void TestTrustTracker_LatestEgalitarianTrustUpdatesCorrectly()
        {
            Initialize();
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(10);
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;

            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, 10);
            _testHelper.WaitIdleTasks();
            var ratings = _dataManipulation.DataContext.GetTable<Rating>().ToArray();

            ratings[0].CurrentValue = ratings[0].LastTrustedValue = 5M;
            ratings[1].CurrentValue = ratings[1].LastTrustedValue = 5M;
            _testHelper.WaitIdleTasks();

            UserRatingResponse theResponse = new UserRatingResponse();
            Func<decimal> ran = () => (decimal) (RandomGenerator.GetRandom() * 10.0);

            // users 1 and 2 are rerated by user 3, and user 1 is separately rerated by user 9
            _testHelper.ActionProcessor.UserRatingAdd(ratings[0].RatingID, 5M, _testHelper.UserIds[6], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[0].RatingID, 6M, _testHelper.UserIds[1], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[0].RatingID, 7M, _testHelper.UserIds[3], ref theResponse);

            _testHelper.ActionProcessor.UserRatingAdd(ratings[1].RatingID, 5M, _testHelper.UserIds[6], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[1].RatingID, 4M, _testHelper.UserIds[2], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[1].RatingID, 2M, _testHelper.UserIds[3], ref theResponse);

            _testHelper.ActionProcessor.UserRatingAdd(ratings[9].RatingID, 5M, _testHelper.UserIds[6], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[9].RatingID, 4M, _testHelper.UserIds[1], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[9].RatingID, 4.5M, _testHelper.UserIds[9], ref theResponse);

            _testHelper.WaitIdleTasks();

            // get the original LatestUserEgalitarianTrust
            UserInteraction theUserInteraction1 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[3]);
            float originalLatestUserEgalitarianTrust1 = theUserInteraction1.LatestUserEgalitarianTrust;
            originalLatestUserEgalitarianTrust1.Should().BeApproximately(1.0F, 0.01F);
            UserInteraction theUserInteraction2 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[2] && x.User1.UserID == _testHelper.UserIds[3]);
            float originalLatestUserEgalitarianTrust2 = theUserInteraction2.LatestUserEgalitarianTrust;
            originalLatestUserEgalitarianTrust2.Should().BeApproximately(1.0F, 0.01F);
            float originalOverallTrust1 = theUserInteraction1.User.TrustTrackers.Single().OverallTrustLevel;
            float originalOverallTrust2 = theUserInteraction2.User.TrustTrackers.Single().OverallTrustLevel;

            // user 3 gets rerated on some new ratings
            _testHelper.ActionProcessor.UserRatingAdd(ratings[2].RatingID, 5M, _testHelper.UserIds[6], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[2].RatingID, 3M, _testHelper.UserIds[3], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[2].RatingID, 4M, _testHelper.UserIds[4], ref theResponse); // sent half-way back

            _testHelper.ActionProcessor.UserRatingAdd(ratings[3].RatingID, 5M, _testHelper.UserIds[6], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[3].RatingID, 4M, _testHelper.UserIds[3], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[3].RatingID, 5M, _testHelper.UserIds[5], ref theResponse); // sent all the way back
            _testHelper.WaitIdleTasks();

            // see if LatestUserEgalitarianTrust has changed
            theUserInteraction1 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[3]);
            TrustTracker user3 = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(x => x.User.UserID == _testHelper.UserIds[3]);
            user3.EgalitarianTrustLevel.Should().BeApproximately(0.25F, 0.01F);
            float revisedLatestUserEgalitarianTrust1 = theUserInteraction1.LatestUserEgalitarianTrust;
            revisedLatestUserEgalitarianTrust1.Should().BeApproximately(0.25F, 0.01F);
            theUserInteraction2 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[2] && x.User1.UserID == _testHelper.UserIds[3]);
            float revisedLatestUserEgalitarianTrust2 = theUserInteraction2.LatestUserEgalitarianTrust;
            revisedLatestUserEgalitarianTrust2.Should().BeApproximately(0.25F, 0.01F);
            float revisedOverallTrust1 = theUserInteraction1.User.TrustTrackers.Single().OverallTrustLevel;
            (revisedOverallTrust1 == originalOverallTrust1).Should().BeFalse(); // because user 3's egalitarian trust has changed and user 3 is only one of users who rerated 1
            float revisedOverallTrust2 = theUserInteraction2.User.TrustTrackers.Single().OverallTrustLevel;
            (revisedOverallTrust2 == originalOverallTrust2).Should().BeTrue(); // because user 3 is still only rerater of user 2

            // set the EgalitarianTrustOverride, and see if that changes things
            user3.EgalitarianTrustLevelOverride = 0.9F;
            user3.MustUpdateUserInteractionEgalitarianTrustLevel = true;
            _testHelper.WaitIdleTasks(); 
            user3 = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(x => x.User.UserID == _testHelper.UserIds[3]);
            user3.MustUpdateUserInteractionEgalitarianTrustLevel.Should().BeFalse(); // flag should have reset
            theUserInteraction1 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[3]);
            revisedLatestUserEgalitarianTrust1 = theUserInteraction1.LatestUserEgalitarianTrust;
            revisedLatestUserEgalitarianTrust1.Should().BeApproximately(0.9F, 0.01F);
            theUserInteraction2 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[2] && x.User1.UserID == _testHelper.UserIds[3]);
            revisedLatestUserEgalitarianTrust2 = theUserInteraction2.LatestUserEgalitarianTrust;
            revisedLatestUserEgalitarianTrust2.Should().BeApproximately(0.9F, 0.01F);
            float revisedOverallTrust1a = theUserInteraction1.User.TrustTrackers.Single().OverallTrustLevel;
            (revisedOverallTrust1 == revisedOverallTrust1a).Should().BeFalse(); // because user 3's egalitarian trust has changed and user 3 is only one of users who rerated 1
            float revisedOverallTrust2a = theUserInteraction2.User.TrustTrackers.Single().OverallTrustLevel;
            revisedOverallTrust2.Should().BeApproximately(revisedOverallTrust2a, 0.01F); // because user 3 is still only rerater of user 2

            // now change the EgalitarianTrustOverride only slightly. That should change LatestUserEgalitarianTrust, but not the overall trust level of the earlier users
            user3 = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(x => x.User.UserID == _testHelper.UserIds[3]);
            user3.EgalitarianTrustLevelOverride = 0.91F;
            user3.MustUpdateUserInteractionEgalitarianTrustLevel = true;
            _testHelper.WaitIdleTasks();
            theUserInteraction1 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[3]);
            revisedLatestUserEgalitarianTrust1 = theUserInteraction1.LatestUserEgalitarianTrust;
            revisedLatestUserEgalitarianTrust1.Should().BeApproximately(0.91F, 0.001F);
            float latestUserEgalitarianTrustAtTimeOfLastUpdate1 = (float) theUserInteraction1.LatestUserEgalitarianTrustAtLastWeightUpdate;
            latestUserEgalitarianTrustAtTimeOfLastUpdate1.Should().BeApproximately(0.90F, 0.001F); // this should not change
            theUserInteraction2 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[2] && x.User1.UserID == _testHelper.UserIds[3]);
            revisedLatestUserEgalitarianTrust2 = theUserInteraction2.LatestUserEgalitarianTrust;
            revisedLatestUserEgalitarianTrust2.Should().BeApproximately(0.91F, 0.001F);
            float revisedOverallTrust1b = theUserInteraction1.User.TrustTrackers.Single().OverallTrustLevel;
            (revisedOverallTrust1a == revisedOverallTrust1b).Should().BeTrue(); // because the override value did not change by enough to change the weighting
            float revisedOverallTrust2b = theUserInteraction2.User.TrustTrackers.Single().OverallTrustLevel;
            revisedOverallTrust2a.Should().BeApproximately(revisedOverallTrust2b, 0.01F); // for the same reason and because user 3 is still only rerater of user 2
        }

        [TestMethod]
        public void TestTrustTracker_CalculatesUserInteractionStatAndTrustTrackerStatCorrectlyWhenOneUserIsReratedByTwoDifferentUsers()
        {
            // For each separate rating, the first user rating can be higher or lower than the base level of 5.0.
            // The second rating can be slightly higher, slightly lower, considerably higher, or considerably lower than the first.
            foreach (var firstSequenceFirstUserRating in new decimal[] { 6M, 4M })
                foreach (var secondSequenceFirstUserRating in new decimal[] { 7M, 3M })
                    foreach (var firstSequenceRelativeSecondRating in new decimal[] { 0.3M, -0.4M, 0.0M, 1.4M, -2.3M })
                        foreach (var secondSequenceRelativeSecondRating in new decimal[] { 0.3M, -0.4M, 1.4M, -2.3M })
                        {

                            decimal[] firstSequence = new decimal[] { firstSequenceFirstUserRating, firstSequenceFirstUserRating + firstSequenceRelativeSecondRating};
                            decimal[] secondSequence = new decimal[] { secondSequenceFirstUserRating, secondSequenceFirstUserRating + secondSequenceRelativeSecondRating };
                            TestTrustTracker_CalculatesUserInteractionStatAndTrustTrackerStatCorrectly_WhenOneUserIsReratedByTwoDifferentUsers_Helper(firstSequence, secondSequence);
                        }
        }

        private void TestTrustTracker_CalculatesUserInteractionStatAndTrustTrackerStatCorrectly_WhenOneUserIsReratedByTwoDifferentUsers_Helper(decimal[] firstSequence, decimal[] secondSequence)
        {
            Initialize();
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(10);

            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, 1);
            _testHelper.WaitIdleTasks();
            var ratings = _dataManipulation.DataContext.GetTable<Rating>().ToArray();

            ratings[0].CurrentValue = ratings[0].LastTrustedValue = 5M;
            ratings[1].CurrentValue = ratings[1].LastTrustedValue = 5M;
            _testHelper.WaitIdleTasks();

            UserRatingResponse theResponse = new UserRatingResponse();
            Func<decimal> ran = () => (decimal) (RandomGenerator.GetRandom() * 10.0);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[0].RatingID, firstSequence[0], _testHelper.UserIds[1], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[0].RatingID, ran(), _testHelper.UserIds[9], ref theResponse); // Add an irrelevant intermediary user rating by someone else
            _testHelper.ActionProcessor.UserRatingAdd(ratings[0].RatingID, firstSequence[1], _testHelper.UserIds[2], ref theResponse);

            _testHelper.ActionProcessor.UserRatingAdd(ratings[1].RatingID, secondSequence[0], _testHelper.UserIds[1], ref theResponse);
            _testHelper.ActionProcessor.UserRatingAdd(ratings[1].RatingID, ran(), _testHelper.UserIds[9], ref theResponse); // Add an irrelevant intermediary user rating by someone else
            _testHelper.ActionProcessor.UserRatingAdd(ratings[1].RatingID, ran(), _testHelper.UserIds[3], ref theResponse); // Add an irrelevant intermediary user rating by the eventual latest user rater
            _testHelper.ActionProcessor.UserRatingAdd(ratings[1].RatingID, secondSequence[1], _testHelper.UserIds[3], ref theResponse);

            TestableDateTime.SleepOrSkipTime(1000 * 60 * 61);
            _testHelper.WaitIdleTasks();

            var theUserInteraction1 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[2]);
            float correctWeightInCalculatingTrustTotal1 = PMTrustCalculations.GetLastUpdatedUserInteractionWeightInCalculatingTrustTotal(theUserInteraction1.UserInteractionStats[0], theUserInteraction1);
            theUserInteraction1.WeightInCalculatingTrustTotal.Should().BeApproximately(correctWeightInCalculatingTrustTotal1, 0.01F);
            var theUserInteraction2 = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[3]);
            float correctWeightInCalculatingTrustTotal2 = PMTrustCalculations.GetLastUpdatedUserInteractionWeightInCalculatingTrustTotal(theUserInteraction2.UserInteractionStats[0], theUserInteraction2);
            theUserInteraction2.WeightInCalculatingTrustTotal.Should().BeApproximately(correctWeightInCalculatingTrustTotal2, 0.01F);
            var theUserInteraction3 = _dataManipulation.DataContext.GetTable<UserInteraction>().SingleOrDefault(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[9]);
            theUserInteraction3.Should().BeNull(); // because the idle task should eliminate it

            int numStats = TrustTrackerStatManager.NumStats;
            for (int i = 0; i < numStats; i++)
            {
                TrustTrackerStat theTrustTrackerStat = _dataManipulation.DataContext.GetTable<TrustTrackerStat>().Single(x => x.TrustTracker.UserID == _testHelper.UserIds[1] && x.StatNum == i);
                UserInteractionStat theUserInteractionStat1 = theUserInteraction1.UserInteractionStats.Single(x => x.StatNum == i);
                UserInteractionStat theUserInteractionStat2 = theUserInteraction2.UserInteractionStats.Single(x => x.StatNum == i);
                float avgAdjustPctFromUserInteractionStat1 = theUserInteractionStat1.AvgAdjustmentPctWeighted;
                float avgAdjustPctFromUserInteractionStat2 = theUserInteractionStat2.AvgAdjustmentPctWeighted;
                float trustNumerator = theUserInteraction1.WeightInCalculatingTrustTotal * avgAdjustPctFromUserInteractionStat1 + theUserInteraction2.WeightInCalculatingTrustTotal * avgAdjustPctFromUserInteractionStat2;
                float trustDenominator = theUserInteraction1.WeightInCalculatingTrustTotal + theUserInteraction2.WeightInCalculatingTrustTotal;
                if (theUserInteractionStat1.SumWeights > 0 || theUserInteractionStat2.SumWeights > 0)
                    (trustDenominator > 0).Should().BeTrue();
                float trustValue = (trustDenominator == 0) ? 1F : trustNumerator / trustDenominator;
                theTrustTrackerStat.TrustValue.Should().BeApproximately(trustValue, 0.01F);
            }
        }

        [TestMethod]
        public void TestTrustTracker_CalculatesUserInteractionStatAndTrustTrackerStatCorrectly_WhenOneUserIsReratedBySeveralDifferentUsers()
        {
            int numRepetitions = 10;
            for (int r = 0; r < numRepetitions; r++)
            {
                Func<decimal> ran = () => (decimal)(RandomGenerator.GetRandom() * 10.0);
                Func<decimal> ran2 = () =>
                    {
                        decimal x = ran();
                        if (x < 5.0M)
                            return x * 2.0M;
                        return 6.0M + ran() / 10.0M; // make a lot of ratings in the vicinity of 6 - 7.
                    };
                int numUsers = 5;
                List<decimal[]> sequence = new List<decimal[]>();
                for (int u = 0; u < numUsers; u++)
                    sequence.Add(new decimal[] { ran2(), ran2() });
                TestTrustTracker_CalculatesUserInteractionStatAndTrustTrackerStatCorrectly_WhenOneUserIsReratedBySeveralDifferentUsers_Helper(sequence);
            }            
        }

        private void TestTrustTracker_CalculatesUserInteractionStatAndTrustTrackerStatCorrectly_WhenOneUserIsReratedBySeveralDifferentUsers_Helper(List<decimal[]> sequence)
        {
            Initialize();
            _testHelper.CreateSimpleTestTable(true);

            int numSequences = sequence.Count(); // one sequence per rating, multiple user ratings per sequence
            _testHelper.CreateUsers(numSequences + 5);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, numSequences);
            _testHelper.WaitIdleTasks();
            var ratings = _dataManipulation.DataContext.GetTable<Rating>().ToArray();
            for (int s = 0; s < numSequences; s++)
                ratings[s].CurrentValue = ratings[s].LastTrustedValue = 5M;
            _testHelper.WaitIdleTasks();

            UserRatingResponse theResponse = new UserRatingResponse();
            Func<decimal> ran = () => (decimal)((new Random((int)DateTime.Now.Ticks)).NextDouble() * 10.0);

            // later rating users are 2 through 2 + numSequences - 1
            for (int s = 0; s < numSequences; s++)
            {
                _testHelper.ActionProcessor.UserRatingAdd(ratings[s].RatingID, sequence[s][0], _testHelper.UserIds[1], ref theResponse);
                _testHelper.ActionProcessor.UserRatingAdd(ratings[s].RatingID, ran(), _testHelper.UserIds[numSequences + 2], ref theResponse); // Add an irrelevant intermediary user rating by someone else
                _testHelper.ActionProcessor.UserRatingAdd(ratings[s].RatingID, ran(), _testHelper.UserIds[s + 2], ref theResponse); // Add an irrelevant intermediary user rating by the eventual latest user rater
                _testHelper.ActionProcessor.UserRatingAdd(ratings[s].RatingID, sequence[s][1], _testHelper.UserIds[s + 2], ref theResponse);
            }

            TestableDateTime.SleepOrSkipTime(1000 * 60 * 61);
            _testHelper.WaitIdleTasks();

            List<UserInteraction> userInteractions = new List<UserInteraction>();
            for (int s = 0; s < numSequences; s++)
            {
                UserInteraction uiToAdd = _dataManipulation.DataContext.GetTable<UserInteraction>().Single(x => x.User.UserID == _testHelper.UserIds[1] && x.User1.UserID == _testHelper.UserIds[2 + s]);
                userInteractions.Add(uiToAdd);
                uiToAdd.WeightInCalculatingTrustTotal.Should().BeApproximately(PMTrustCalculations.GetLastUpdatedUserInteractionWeightInCalculatingTrustTotal(uiToAdd.UserInteractionStats[0], uiToAdd), 0.01F);
            }

            int numStats = TrustTrackerStatManager.NumStats;
            for (int i = 0; i < numStats; i++)
            {
                TrustTrackerStat theTrustTrackerStat = _dataManipulation.DataContext.GetTable<TrustTrackerStat>().Single(x => x.TrustTracker.UserID == _testHelper.UserIds[1] && x.StatNum == i);
                List<UserInteractionStat> userInteractionStats = userInteractions.Select(x => x.UserInteractionStats.Single(y => y.StatNum == i)).ToList();

                List<float> avgAdjustPctFromUserInteractionStats = userInteractionStats.Select(x => x.AvgAdjustmentPctWeighted).ToList();
                float trustNumerator = Enumerable.Range(0, numSequences).Sum(s => userInteractions[s].WeightInCalculatingTrustTotal * avgAdjustPctFromUserInteractionStats[s]);
                float trustDenominator = Enumerable.Range(0, numSequences).Sum(s => userInteractions[s].WeightInCalculatingTrustTotal);
                if (userInteractionStats.Any(z => z.SumWeights > 0))
                    (trustDenominator > 0).Should().BeTrue();
                float trustValue = (trustDenominator == 0) ? 1F : trustNumerator / trustDenominator;
                theTrustTrackerStat.TrustValue.Should().BeApproximately(trustValue, 0.01F);
            }
        }

        [TestMethod]
        public void Test_TrustTracking_ProperlyCalculatesAdjustmentPercentagesBasedOnUserSuccess()
        {
            PMTrustCalculations.NumPerfectScoresToGiveNewUser = 0;

            int? randomSeed = null;
            Test_TrustTracking_ProperlyCalculatesAdjustmentPercentagesBasedOnUserSuccess_Helper(100, 0.6F, 0.6F, false, true, randomSeed);
            Test_TrustTracking_ProperlyCalculatesAdjustmentPercentagesBasedOnUserSuccess_Helper(100, 0.6F, 0.8F, false, true, randomSeed);
            Test_TrustTracking_ProperlyCalculatesAdjustmentPercentagesBasedOnUserSuccess_Helper(100, 0.6F, 0.8F, true, false, randomSeed);
            Test_TrustTracking_ProperlyCalculatesAdjustmentPercentagesBasedOnUserSuccess_Helper(100, 0.6F, 0.6F, true, false, randomSeed);
        }

        public void Test_TrustTracking_ProperlyCalculatesAdjustmentPercentagesBasedOnUserSuccess_Helper(
            int numTblRows, 
            float baselineAdjustmentFactorToApply, 
            float specialCaseAdjustmentFactorToApply, 
            bool applySpecialCaseAdjustmentFactorToHighMagnitudeRatings, 
            bool applySpecialCaseAdjustmentFactorToSpecifiedChoiceFieldValue, 
            int? randomSeed = null)
        {
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true; // we want all the userratings to have adjustment factors of 1 for testing purposes. That is, if a user enters a userrating, the rating will be set to that userrating. But we can still see what the trust levels (i.e., retrospective adjustment factors are).
            TrustTrackerStatManager.MinAdjustmentFactorToCreditUserRating = 0.0F; // this allows us to make calculations without adjusting for the fact that trust will be a little lower than it otherwise would be

            const decimal minRatingValue = 0M;
            const decimal maxRatingValue = 10M;
            const decimal otherUserRatingValueOffset = 0.001M;

            Initialize();

            if (applySpecialCaseAdjustmentFactorToHighMagnitudeRatings && applySpecialCaseAdjustmentFactorToSpecifiedChoiceFieldValue)
                throw new Exception("Can't apply special case to high magnitude ratings and specified choice field value.");

            decimal initialValue = 4M;
            // different even/odd values so that we have two classes of ratings
            // The idea is that we might have ratings with different ChoiceInGroupIDs, and we want to make sure there is a difference.
            decimal correctValueEvens = 4.2M;
            decimal correctValueOdds = 3.5M;
            decimal correctValueHighMagnitude = 8M;

            if (randomSeed != null)
                RandomGenerator.SeedOverride = randomSeed;
            Initialize();

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(15);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, numTblRows - 1); // -1 because the TestHelper has already created one?
            _testHelper.WaitIdleTasks();

            TblRow[] tblRows = _dataManipulation.DataContext.GetTable<TblRow>().ToArray();
            Rating[] ratings = _dataManipulation.DataContext.GetTable<Rating>().ToArray();

            ratings.Count().Should().Be(numTblRows, "because we started with one TblRow and added numTblRows-1 TblRows, so we should end up with numTblRows TblRows."); 
            tblRows.Count().Should().Be(numTblRows);

            
            if (applySpecialCaseAdjustmentFactorToSpecifiedChoiceFieldValue)
            {
                RaterooTestEnvironmentCreator testEnv = new RaterooTestEnvironmentCreator(_testHelper);
                PointsManager pm = _dataManipulation.DataContext.GetTable<PointsManager>().OrderByDescending(x => x.PointsManagerID).First();
                testEnv.CreateChoiceGroups(pm.PointsManagerID);
                ChoiceGroup theChoiceGroup = _dataManipulation.DataContext.GetTable<ChoiceGroup>().Single(x => x.Name == "ChoiceGroup single"); // this is just a test choice group that was added where one can only select a single choice from the group
                var choiceInGroups = theChoiceGroup.ChoiceInGroups.ToList();
                int choiceInGroupsCount = choiceInGroups.Count();
                int fieldDefinitionID = _testHelper.ActionProcessor.FieldDefinitionCreate(_testHelper.Tbl.TblID, "SimpChoice", FieldTypes.ChoiceField, true, theChoiceGroup.ChoiceGroupID, null, true, true, _testHelper.SuperUserId, null);
                for (int rowNum = 0; rowNum < numTblRows; rowNum++)
                {
                    int? theChoiceId = null;
                    // Every tenth rating gets a the first choice
                    if (rowNum % 10 == 3)
                        theChoiceId = choiceInGroups[0].ChoiceInGroupID; // these are the ones we are going to focus on.
                    // Otherwise there's a 50% chance that we will set theChoiceId to a random choice other than the first one
                    else if (RandomGenerator.GetRandom() > 0.5)
                        theChoiceId = choiceInGroups[RandomGenerator.GetRandom(1, choiceInGroupsCount - 1)].ChoiceInGroupID;
                    TblRow theTblRow = _dataManipulation.DataContext.GetTable<TblRow>().OrderBy(x => x.TblRowID).ToList().Skip(rowNum).First();
                    // If we leave theChoice == null, then this means that there is no choice for this field.
                    _testHelper.ActionProcessor.ChoiceFieldWithSingleChoiceCreateOrReplace(theTblRow, fieldDefinitionID, theChoiceId, _testHelper.SuperUserId, null);
                }
            }

            // Initialize, each tbl row to the initial value.
            for (int rowNum = 0; rowNum < numTblRows; rowNum++)
            {
                Rating theRating = _dataManipulation.DataContext.GetTable<Rating>().OrderBy(x => x.RatingID).ToList()
                    .Skip(rowNum).First();
                theRating.CurrentValue = initialValue;
                theRating.LastTrustedValue = initialValue;
            }

            decimal valueEnteredHighValue = initialValue + 
                (correctValueHighMagnitude - initialValue) / (decimal)specialCaseAdjustmentFactorToApply;

            // Now, for each table row, have user # 1 enter a value reflecting the adjustment factor that it applies
            // and then have two other users enter very close to the correct value.
            for (int rowNum = 0; rowNum < numTblRows; rowNum++)
            {
                decimal correctValue = (rowNum % 2 == 0) ? correctValueEvens : correctValueOdds;
                float adjustmentFactorToApply = baselineAdjustmentFactorToApply;
                if (baselineAdjustmentFactorToApply != specialCaseAdjustmentFactorToApply && rowNum % 10 == 3) // this is arbitrary, but we check for this below
                {
                    if (applySpecialCaseAdjustmentFactorToHighMagnitudeRatings)
                        correctValue = correctValueHighMagnitude;
                    adjustmentFactorToApply = specialCaseAdjustmentFactorToApply;
                }

                //decimal valueToEnterByFirstUser = PMAdjustmentPercentages.GetRatingToAcceptFromAdjustmentPct(initialValue, correctValue, adjPctToApply, null);
                decimal valueToEnterByFirstUser = initialValue + (correctValue - initialValue) / (decimal)adjustmentFactorToApply;
                valueToEnterByFirstUser = PMTrustCalculations.Constrain(valueToEnterByFirstUser, minRatingValue,
                    maxRatingValue);

                UserRatingResponse theResponse = new UserRatingResponse();
                bool isLastEntry = rowNum == numTblRows - 1; // useful for setting conditional breakpoint
                _testHelper.ActionProcessor.UserRatingAdd(ratings[rowNum].RatingID, valueToEnterByFirstUser, _testHelper.UserIds[1], ref theResponse);

                int otherUser1Num = RandomGenerator.GetRandom(2, 10);
                _testHelper.ActionProcessor.UserRatingAdd(ratings[rowNum].RatingID, correctValue + otherUserRatingValueOffset, _testHelper.UserIds[otherUser1Num], ref theResponse);
                
                int otherUser2Num;
                do otherUser2Num = RandomGenerator.GetRandom(2, 10);
                while (otherUser2Num == otherUser1Num);
                _testHelper.ActionProcessor.UserRatingAdd(ratings[rowNum].RatingID, correctValue - otherUserRatingValueOffset, _testHelper.UserIds[otherUser2Num], ref theResponse);

                if (rowNum % 5 == 0) // periodically, do the idle tasks so that trust will be updated
                {
                    _testHelper.WaitIdleTasks();
                    TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(10).GetTotalWholeMilliseconds());
                }
            }
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(10).GetTotalWholeMilliseconds());
            _testHelper.WaitIdleTasks();

            // Check the trust tracker
            TrustTracker trustTracker = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(x => x.TrustTrackerUnit.TrustTrackerUnitID == _testHelper.Tbl.PointsManager.TrustTrackerUnit.TrustTrackerUnitID && 
x.UserID == _testHelper.UserIds[1]);
            TrustTrackerStat noExtraWeightingTrustStat = trustTracker.TrustTrackerStats.Single(x => x.StatNum == 0);
            float tolerance = baselineAdjustmentFactorToApply == specialCaseAdjustmentFactorToApply ?
                0.01F : 0.15F;

            /* What it is supposed to be doing in those lines is making sure that when the ratings do adjust a certain amount, 
             * the user's trust tracking should reflect that. So, if, for example, whenever a user makes a userrating, it almost 
             * moves back 50%, then we should end up with a number close to 0.50.
             * 
             * In words what is this comparison checking?
             * If noExtraWeightingStat used to be about equal to baselineAdjustmentFactorToApply, and we changed noExtraWeightStat to be multiplied by
             * RatingMagnitude, then baselineAdjustmentFactorToApply should be multiplied by RatingMagnitude.
             */
            noExtraWeightingTrustStat.TrustValue.Should().BeInRange(
                baselineAdjustmentFactorToApply - tolerance, 
                baselineAdjustmentFactorToApply + tolerance);

            if (baselineAdjustmentFactorToApply != specialCaseAdjustmentFactorToApply && 
                applySpecialCaseAdjustmentFactorToHighMagnitudeRatings)
            {
                var highMagnitude = trustTracker.TrustTrackerStats.Single(x => x.StatNum == 1); 
                //var uis = DataAccess.RaterooDB.GetTable<UserInteractionStat>().Where(x => x.UserInteraction.User.UserID == theTestHelper.theUsers[1] && x.StatNum == 2).ToList();
                //foreach (var debugui in uis.Where(x => x.SumWeights > 0))
                //    Trace.TraceInformation("Adj pct " + debugui.SumAdjustPctTimesWeight / debugui.SumWeights + " weights: " + debugui.SumWeights);
                Math.Abs(noExtraWeightingTrustStat.TrustValue - specialCaseAdjustmentFactorToApply)
                    .Should().BeGreaterThan(Math.Abs(highMagnitude.TrustValue - specialCaseAdjustmentFactorToApply));
            }

            // Check the last rating entered
            int ratingIndexToUse = numTblRows - 1;
            Rating theRating2 = _dataManipulation.DataContext.GetTable<Rating>().OrderBy(x => x.RatingID).Skip(ratingIndexToUse).First();
            UserRating lastUserRating = theRating2.UserRatings.Single(x => x.UserID == _testHelper.UserIds[1]);
            float adjPctApplied = PMAdjustmentFactor.CalculateAdjustmentFactor((decimal)lastUserRating.NewUserRating, lastUserRating.EnteredUserRating, lastUserRating.PreviousRatingOrVirtualRating, null);
            if (baselineAdjustmentFactorToApply == specialCaseAdjustmentFactorToApply)
                adjPctApplied.Should().BeInRange(
                    baselineAdjustmentFactorToApply - tolerance, 
                    baselineAdjustmentFactorToApply + tolerance);

            // Check the last special user rating
            if (baselineAdjustmentFactorToApply != specialCaseAdjustmentFactorToApply)
            {
                while (ratingIndexToUse % 10 != 3)
                    ratingIndexToUse--;
                Rating theRatingToAssess = _dataManipulation.DataContext.GetTable<Rating>().OrderBy(x => x.RatingID).Skip(ratingIndexToUse).First();
                lastUserRating = theRatingToAssess.UserRatings.Single(x => x.UserID == _testHelper.UserIds[1]);
                adjPctApplied = PMAdjustmentFactor.CalculateAdjustmentFactor((decimal)lastUserRating.NewUserRating, lastUserRating.EnteredUserRating, lastUserRating.PreviousRatingOrVirtualRating, null);
                (specialCaseAdjustmentFactorToApply < adjPctApplied == adjPctApplied < baselineAdjustmentFactorToApply)
                    .Should().BeTrue("because the applied adjustment percentage should be somewhere in the middle");
            }
        }

        /// <summary>
        /// First, we will have User 1 add a rating and no one else rates it. Then, we will have User 1 add a rating and
        /// some other users set the rating to around the correct value based on the adjustment percentage specified, and we will
        /// repeat this several times. Now, we check back to that first rating and see if the adjustment percentage has been
        /// applied after the idle tasks.
        /// </summary>
        [TestMethod]
        public void TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel()
        {
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(0.6F, true, false);
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(0.6F, true, true);
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(1.5F, true, false);
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(-0.2F, true, false);
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(0.6F, false, false);
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(0.6F, false, true);
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(1.5F, false, false);
            TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(-0.2F, false, false);
        }

        public void TestTrustTracking_RatingValuesAdjustBasedOnChangesInTrustLevel_Helper(
            float adjustmentFactorToApply, 
            bool initializeRowsToNonNullValue, 
            bool allowMonthToPass)
        {

            PMTrustCalculations.NumPerfectScoresToGiveNewUser = 0;
            TrustTrackerStatManager.MinAdjustmentFactorToCreditUserRating = 0;

            if (adjustmentFactorToApply == 0)
                throw new Exception("This test won't work with applying adjustment percentage of 0, because the initial user must apply some wrong number that will yield the correct number after the adjustment percentage is applied.");
                
            Initialize();

            const int numTblRows = 10;
            decimal initialValue = 4M;
            const decimal correctValue = 7M;
            const int numUsers = 15;

            Initialize();
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(numUsers);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, numTblRows - 1);
            _testHelper.WaitIdleTasks();
            var tblRows = _dataManipulation.DataContext.GetTable<TblRow>().ToArray();
            tblRows.Count().Should().Be(numTblRows);
            var ratings = _dataManipulation.DataContext.GetTable<Rating>().ToArray();
            ratings.Count().Should().Be(numTblRows);

            // Initialize, each tbl row to the initial value.
            if (initializeRowsToNonNullValue)
            {
                for (int rowNum = 0; rowNum < numTblRows; rowNum++)
                {
                    ratings[rowNum].CurrentValue = initialValue;
                    ratings[rowNum].LastTrustedValue = initialValue;
                }
            }
            else
                initialValue = 5M;

            #region debug
            //var user = _dataManipulation.DataContext.GetTable<User>().Single(u => u.UserID ==  _testHelper.UserIds[0]);
            //Debug.WriteLine(String.Format("User {0} is SuperUser: {1}", user.UserID, user.SuperUser));
            #endregion

            // Rate the first rating
            UserRatingResponse aResponse = new UserRatingResponse();
            _testHelper.ActionProcessor.UserRatingAdd(ratings[0].RatingID, correctValue, _testHelper.UserIds[0], ref aResponse);
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(1).GetTotalWholeMilliseconds()); // so that trust will be updated, but not so far that short term resolution will be reflected
            _testHelper.WaitIdleTasks();
            //#region debug
            //{
            //    Debug.WriteLine("***********************************************************");
            //    Debug.WriteLine("******** Adjustment Factor (Before Any Rows Rated) ********");
                
            //    Rating r = _dataManipulation.DataContext.GetTable<Rating>().OrderBy(x => x.RatingID).First();
            //    UserRating ur = r.UserRatings.Single(x => x.UserID == _testHelper.UserIds[0]);

            //    float adjFac = PMAdjustmentFactor.CalculateAdjustmentFactor((decimal)ur.NewUserRating, 
            //        ur.EnteredUserRating, ur.PreviousRatingOrVirtualRating);
            //    Debug.WriteLine(String.Format("{1} = {2} - {4} / {3} - {4}", 
            //        null, adjFac, 
            //        ur.NewUserRating, ur.EnteredUserRating, ur.PreviousRatingOrVirtualRating));
            //}
            //#endregion
            
            if (allowMonthToPass)
            {
                _testHelper.WaitIdleTasks();
                TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(31).GetTotalWholeMilliseconds()); // wait a month
                _testHelper.WaitIdleTasks();
            }

            // For ratings other than the first one, have the first user enter a value that is off from the "correct value"
            //  by the adjustment factor, and have two other users rate the correct value.
            for (int rowNum = 1; rowNum < numTblRows; rowNum++)
            {
                decimal valueToEnterByFirstUser = (decimal)((float)initialValue + (float)(correctValue - initialValue) / adjustmentFactorToApply ); // PMAdjustmentPercentages.GetRatingToAcceptFromAdjustmentPct(initialValue, correctValue, adjPctToApply, null);
                valueToEnterByFirstUser = PMTrustCalculations.Constrain(valueToEnterByFirstUser, 0, 10);
                UserRatingResponse theResponse = new UserRatingResponse();
                _testHelper.ActionProcessor.UserRatingAdd(ratings[rowNum].RatingID, valueToEnterByFirstUser, _testHelper.UserIds[0], ref theResponse);

                // Instead of just two users, rate the rating with the correct value using all other users.
                foreach (int userNum in Enumerable.Range(1, numUsers - 1))
                {
                    _testHelper.ActionProcessor.UserRatingAdd(ratings[rowNum].RatingID, correctValue, _testHelper.UserIds[userNum], ref theResponse);
                }

                Debug.WriteLine(String.Format("{0} RowNum {1} Rated.", TestableDateTime.Now, rowNum));

                //int userNum1 = RandomGenerator.GetRandom(2, 10);
                //_testHelper.ActionProcessor.UserRatingAdd(ratings[rowNum].RatingID, correctValue + 0.001M, _testHelper.UserIds[userNum1], ref theResponse);
                //int userNum2;
                //do 
                //{
                //    userNum2 = RandomGenerator.GetRandom(2, 10);
                //} while (userNum2 == userNum1);
                //_testHelper.ActionProcessor.UserRatingAdd(ratings[rowNum].RatingID, correctValue - 0.001M, _testHelper.UserIds[userNum2], ref theResponse);

                if (true) // (rowNum % 5 == 0) // periodically, do the idle tasks
                {
                    // So that trust will be updated, but not so far that short term resolution will be reflected
                    TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(1).GetTotalWholeMilliseconds()); 
                    _testHelper.WaitIdleTasks();
                }
                //#region debug
                //Debug.WriteLine(String.Format("******** User Trusts (After Row Row {0} Rated) ********", rowNum));
                //foreach (int userNum in Enumerable.Range(0, numUsers))
                //{
                //    var trustTracker = _dataManipulation.DataContext.GetTable<TrustTracker>()
                //        .Single(tt => tt.UserID == _testHelper.UserIds[userNum]);
                //    Debug.WriteLine(String.Format("<User {0}> Overall/Skeptical Trust: {1}/{2}", userNum, 
                //        trustTracker.OverallTrustLevel, trustTracker.SkepticalTrustLevel));
                //}
                //#endregion
                //#region debug
                //{
                //    Debug.WriteLine(String.Format("******** Adjustment Factor (After Row {0} Rated) ********", rowNum));
                    
                //    Rating r = _dataManipulation.DataContext.GetTable<Rating>().OrderBy(x => x.RatingID).First();
                //    UserRating ur = r.UserRatings.Single(x => x.UserID == _testHelper.UserIds[0]);

                //    float adjFac = PMAdjustmentFactor.CalculateAdjustmentFactor((decimal)ur.NewUserRating, 
                //        ur.EnteredUserRating, ur.PreviousRatingOrVirtualRating);
                //    Debug.WriteLine(String.Format("{1} = {2} - {4} / {3} - {4}", 
                //        null, adjFac, 
                //        ur.NewUserRating, ur.EnteredUserRating, ur.PreviousRatingOrVirtualRating));
                //}
                //#endregion
            }
            for (int i = 0; i <= 1; i++)
            { // we do this twice because ratings review won't happen for 20 minutes
                TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(1).GetTotalWholeMilliseconds());
                _testHelper.WaitIdleTasks();
            }            

            // Check the first rating entered to make sure it has been updated
            int ratingIndexToUse = 0;
            Rating rating = _dataManipulation.DataContext.GetTable<Rating>().OrderBy(x => x.RatingID).Skip(ratingIndexToUse).First();
            UserRating userRating = rating.UserRatings.Single(x => x.UserID == _testHelper.UserIds[0]);
            TrustTracker tt = userRating.User.TrustTrackers.Single(x => x.TrustTrackerUnit.PointsManagers.First() == rating.RatingGroup.RatingGroupAttribute.PointsManager);
            if (adjustmentFactorToApply < 1.0)  
                tt.OverallTrustLevel.Should().BeLessThan(1.0F);
            float appliedAdjustmentFactor = PMAdjustmentFactor.CalculateAdjustmentFactor((decimal)rating.CurrentValue, userRating.EnteredUserRating, userRating.PreviousRatingOrVirtualRating);
            float expectedAdjustmentFactor = adjustmentFactorToApply;
            if (allowMonthToPass)
                expectedAdjustmentFactor = 1.0F; // b/c the userratings will be too old to adjust
            expectedAdjustmentFactor = PMTrustCalculations.Constrain(expectedAdjustmentFactor, 0, 1);

            float tolerance = 0.02F;
            appliedAdjustmentFactor.Should().BeInRange(expectedAdjustmentFactor - tolerance, expectedAdjustmentFactor + tolerance);

            // Check the second rating entered to make sure it has not changed
            ratingIndexToUse = 1;
            rating = _dataManipulation.DataContext.GetTable<Rating>().OrderBy(x => x.RatingID).Skip(ratingIndexToUse).First();
            userRating = rating.UserRatings.SingleOrDefault(x => x.User.Username == "admin");
            userRating.Should().BeNull();
        }



        /// <summary>
        /// One user rates a rating using a random value.  Other users rate the rating using values identical to the first
        /// user's rating.  The User's trust should increase.
        /// </summary>
        //[TestMethod]
        public void UserExactlySupportedByOtherUsersRatingsBecomesTrusted()
        {
            const decimal maxRatingValue = 10M;
            const decimal minRatingValue = 0M;
            
            int otherUserCount = 10;
            // The number of times each other user will perform a rating
            int timesOtherUsersShouldRate = 10;

            Random random = new Random();

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(1 + otherUserCount);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, 1);
            _testHelper.WaitIdleTasks();

            User user = _dataManipulation.DataContext.GetTable<User>().Single(u => u.UserID == 2); // I think the first user is the super user, created by TestHelper
            IEnumerable<User> otherUsers = _dataManipulation.DataContext.GetTable<User>().Where(u => u.UserID > 2 && u.UserID < 2 + otherUserCount);

            decimal targetRating = (decimal)((decimal)random.NextDouble() * (maxRatingValue - minRatingValue));
            Debug.WriteLine(String.Format("Target Rating: {0}", targetRating));

            UserRatingResponse response = new UserRatingResponse();
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, targetRating, user.UserID, ref response);
            _testHelper.WaitIdleTasks();

            TrustTracker trustTracker = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(tt => tt.User.UserID == user.UserID);
            Debug.WriteLine(String.Format("Initial: User's Trust = {0} (Overall) {1} (Skeptical)",
                trustTracker.OverallTrustLevel, trustTracker.SkepticalTrustLevel));

            trustTracker.OverallTrustLevel.Should().BeApproximately(0f, 0f, "because a User whose rating on a table that has not been supported by other users' ratings should not be trusted.");
            trustTracker.SkepticalTrustLevel.Should().BeApproximately(0f, 0f, "because a User whose rating on a table that has not been supported by other users' ratings should not be trusted.");

            foreach (int i in Enumerable.Range(0, timesOtherUsersShouldRate))
            {
                foreach (User otherUser in otherUsers)
                {
                    UserRatingResponse otherResponse = new UserRatingResponse();
                    _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, targetRating, otherUser.UserID, ref otherResponse);
                    Debug.WriteLine(String.Format("Other User UserRating: {0}", targetRating));
                }
                _testHelper.WaitIdleTasks();

                trustTracker = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(tt => tt.User.UserID == user.UserID);
                Debug.WriteLine(String.Format("Iteration {0}: User's Trust = {1} (Overall) {2} (Skeptical)",
                    i, trustTracker.OverallTrustLevel, trustTracker.SkepticalTrustLevel));
                trustTracker.OverallTrustLevel
                    .Should().BeGreaterOrEqualTo(trustTracker.OverallTrustLevelAtLastReview, 
                        "because as other users perform ratings confirming a user's rating, the user's trust should increase.");
            }
        }

        /// <summary>
        /// One user rates a rating using a random value.  Other users rate the rating using values close to the first
        /// user's rating.  The User's trust should increase.
        /// </summary>
        //[TestMethod]
        public void UserSupportedByOtherUsersRatingsBecomesTrusted()
        {
            const decimal maxRatingValue = 10M;
            const decimal minRatingValue = 0M;

            int otherUserCount = 10;
            // The number of times each other user will perform a rating
            int timesOtherUsersShouldRate = 10;
            // The maximum amount by which an other user's rating will differ from the target rating
            decimal otherUsersMaxError = 0.1m;

            Random random = new Random();

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(1 + otherUserCount);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, 1);
            _testHelper.WaitIdleTasks();

            User user = _dataManipulation.DataContext.GetTable<User>().Single(u => u.UserID == 2); // I think the first user is the super user, created by TestHelper
            IEnumerable<User> otherUsers = _dataManipulation.DataContext.GetTable<User>().Where(u => u.UserID > 2 && u.UserID < 2 + otherUserCount);

            decimal targetRating = (decimal)((decimal)random.NextDouble() * (maxRatingValue - minRatingValue));
            Debug.WriteLine(String.Format("Target Rating: {0}", targetRating));

            UserRatingResponse response = new UserRatingResponse();
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, targetRating, user.UserID, ref response);
            _testHelper.WaitIdleTasks();

            TrustTracker trustTracker = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(tt => tt.User.UserID == user.UserID);
            Debug.WriteLine(String.Format("Initial: User's Trust = {0} (Overall) {1} (Skeptical)",
                trustTracker.OverallTrustLevel, trustTracker.SkepticalTrustLevel));

            trustTracker.OverallTrustLevel.Should().BeApproximately(0f, 0f, "because a User whose rating on a table that has not been supported by other users' ratings should not be trusted.");
            trustTracker.SkepticalTrustLevel.Should().BeApproximately(0f, 0f, "because a User whose rating on a table that has not been supported by other users' ratings should not be trusted.");

            foreach (int i in Enumerable.Range(0, timesOtherUsersShouldRate))
            {
                foreach (User otherUser in otherUsers)
                {
                    decimal error = -1 * otherUsersMaxError + (decimal)random.NextDouble() * (2 * otherUsersMaxError);
                    decimal erroredRating = targetRating + error;

                    UserRatingResponse otherResponse = new UserRatingResponse();
                    _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, erroredRating, otherUser.UserID, ref otherResponse);
                    Debug.WriteLine(String.Format("Other User UserRating: {0}", erroredRating));
                }
                _testHelper.WaitIdleTasks();

                trustTracker = _dataManipulation.DataContext.GetTable<TrustTracker>().Single(tt => tt.User.UserID == user.UserID);
                Debug.WriteLine(String.Format("Iteration {0}: User's Trust = {1} (Overall) {2} (Skeptical)",
                    i, trustTracker.OverallTrustLevel, trustTracker.SkepticalTrustLevel));

                // Fashion some test that checks that the user's trust is increasing, allowing for the fact that it may 
                //   fluctuate a little since other users are not rating exactly equal to the user's rating.
                (
                    trustTracker.OverallTrustLevel > trustTracker.OverallTrustLevelAtLastReview
                    )
                    .Should().BeTrue("because as other users perform ratings confirming a user's rating, the user's trust should increase.");
            }
        }


        /// <summary>
        /// Since it may be important to test different scenarios to ensure that Rateroo's trust system works, we will
        /// enapsulate parameters in a class for ease of reporting and recording.  Mostly we just want to be able to
        /// use this class's ToString method so that the Debug output for a test can easily report the parameters
        /// its using.
        /// </summary>
        class RaterooTrustTestParameters
        {
            public int NumUsers;
            public int NumUsersWhoTargetWrongValues;
            public int NumTblRows;
            public int NumBatchesOfUserRatings;
            public int NumUserRatingsPerBatch;
            
            // The closer these are to 1, the more likely a user is to make a rating that the database will accept.
            public float MinDesiredAdjustmentFactor = 0.8F;
            public float MaxDesiredAdjustmentFactor = 1.2F;

            // Use rating values in the middle of the possible range [0,10] so that htere is room to over/undershoot
            public decimal MinRatingValue = 4M;
            public decimal MaxRatingValue = 6M;

            // How close to the "correct" value a Rating value must be in order to be considered correct
            public decimal Tolerance = 0.5M;
            
            public float RequiredProportionOfRatingsWithinTolerance = 0.9F;
            public int? RandomSeed = null;

            public override string ToString()
            {
                Reflection.FieldInfo[] fieldInfos = this.GetType().GetFields();
                Reflection.PropertyInfo[] propertyInfos = this.GetType().GetProperties();
                IEnumerable<string> fieldNamesAndValues = fieldInfos.Select(fi => String.Format("{0}={1}", fi.Name, fi.GetValue(this) ?? "null"));
                IEnumerable<string> propertyNamesAndValues = propertyInfos.Select(pi => String.Format("{0}={1}", pi.Name, pi.GetValue(this, null) ?? "null"));
                return String.Format("{0}: {1}, {2}", this.GetType().Name, 
                    String.Join(", ", fieldNamesAndValues.ToArray()), String.Join(", ", propertyNamesAndValues.ToArray()));
            }
        }


        /// <summary>
        /// Overall test to see if things get to right number.
        /// Have some number of users, each of whom has a different adjustment factor (a few are negative)
        /// and also a special adjustment factors for some value of the ChoiceInFields.
        /// We have a large number of table rows and continuously add many user ratings. We should then see
        /// that a high percentage are near the goal.
        /// </summary>
        [TestMethod]
        public void RateroosRatingsShouldConvergeWhenAPopulationOfUsersPerformsRatings()
        {
            rateroosRatingsShouldConvergeWhenAPopulationOfUsersPerformsRatings_Helper(new RaterooTrustTestParameters {
                NumUsers = 10,
                NumUsersWhoTargetWrongValues = 2, 
                NumTblRows = 40, 
                NumBatchesOfUserRatings = 40, 
                NumUserRatingsPerBatch = 200, 
                MinDesiredAdjustmentFactor = 0.8f,
                MaxDesiredAdjustmentFactor = 1.2f,
                // Use rating values in the middle of the possible range [0,10] so that htere is room to over/undershoot
                MinRatingValue = 4m,
                MaxRatingValue = 6m,
                Tolerance = 0.2M, 
                RequiredProportionOfRatingsWithinTolerance = 0.99F,
            });
            //Test_OverallTrustTest_Helper(20, 500, 100, 10, 3, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numBatchesOfUserRatings"></param>
        /// <param name="numUserRatingsPerBatch"></param>
        /// <param name="numTblRows"></param>
        /// <param name="numUsers"></param>
        /// <param name="proportionOfUsersWhoTargetWrongValues"></param>
        /// <param name="tolerance">The magnitude of deviation allowed by <paramref name="requiredProportionOfRatingsWithinTolerance"/> percent of ratin</param>
        /// <param name="requiredProportionOfRatingsWithinTolerance"></param>
        /// <param name="randomSeed"></param>
        void rateroosRatingsShouldConvergeWhenAPopulationOfUsersPerformsRatings_Helper(RaterooTrustTestParameters parameters)
        {
            Debug.WriteLine(String.Format("Parameters: {0}", parameters.ToString()));

            if (parameters.RandomSeed != null)
                RandomGenerator.SeedOverride = parameters.RandomSeed;
            Initialize();

            float[] baseAdjustmentFactorToApply = new float[parameters.NumUsers];
            for (int i = 0; i < parameters.NumUsers; i++)
                baseAdjustmentFactorToApply[i] = RandomGenerator.GetRandom(parameters.MinDesiredAdjustmentFactor, parameters.MaxDesiredAdjustmentFactor);

            decimal[] correctValues = new decimal[parameters.NumTblRows];
            decimal[] wrongValues = new decimal[parameters.NumTblRows];
            for (int i = 0; i < parameters.NumTblRows; i++)
            {
                correctValues[i] = (decimal)RandomGenerator.GetRandom(parameters.MinRatingValue, parameters.MaxRatingValue);
                wrongValues[i] = (decimal)RandomGenerator.GetRandom(parameters.MinRatingValue, parameters.MaxRatingValue);
            }

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(parameters.NumUsers);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, parameters.NumTblRows - 1);
            _testHelper.WaitIdleTasks();

            var testEnv = new RaterooTestEnvironmentCreator(_testHelper);
            PointsManager pointsManager = _dataManipulation.DataContext.GetTable<PointsManager>().OrderByDescending(x => x.PointsManagerID).First();
            testEnv.CreateChoiceGroups(pointsManager.PointsManagerID);
            ChoiceGroup choiceGroup = _dataManipulation.DataContext.GetTable<ChoiceGroup>().Single(x => x.Name == "ChoiceGroup single");
            var choiceInGroups = choiceGroup.ChoiceInGroups.ToList();
            int choiceInGroupsCount = choiceInGroups.Count();
            int fieldDefinitionId = _testHelper.ActionProcessor.FieldDefinitionCreate(_testHelper.Tbl.TblID, "SimpChoice",
                FieldTypes.ChoiceField, true, choiceGroup.ChoiceGroupID, null, true, true, _testHelper.SuperUserId, null);
            int[] randomChoiceInGroupIds = new int[parameters.NumTblRows];
            for (int rowNum = 0; rowNum < parameters.NumTblRows; rowNum++)
            {
                randomChoiceInGroupIds[rowNum] = choiceInGroups[RandomGenerator.GetRandom(1, choiceInGroupsCount - 1)].ChoiceInGroupID;
                TblRow tblRow = _dataManipulation.DataContext.GetTable<TblRow>().OrderBy(x => x.TblRowID).ToList().Skip(rowNum).First();
                _testHelper.ActionProcessor.ChoiceFieldWithSingleChoiceCreateOrReplace(tblRow, fieldDefinitionId, randomChoiceInGroupIds[rowNum],
                    _testHelper.SuperUserId, null);
            }
            #region Debug
            {
                Debug.WriteLine("******************");

                float proportionWithinTolerance = calculateProportionWithinTolerance(_dataManipulation.DataContext.GetTable<Rating>().ToArray(),
                parameters.NumTblRows, correctValues, parameters.Tolerance);
                Debug.WriteLine(String.Format("Before Batches, {0}% within tolerance.", proportionWithinTolerance * 100));

                IEnumerable<TrustTracker> tts = _dataManipulation.DataContext.GetTable<TrustTracker>().OrderBy(u => u.UserID);
                if (tts.Any())
                {
                    foreach (TrustTracker tt in tts)
                    {
                        Debug.WriteLine(String.Format("User {0} Trust: {1}/{2}", tt.UserID, tt.OverallTrustLevel, tt.SkepticalTrustLevel));
                    }
                }
                else
                {
                    Debug.WriteLine("No TrustTrackers Yet.");
                }
            }
            #endregion
            //int minUserId = _dataManipulation.DataContext.GetTable<User>().Min(u => u.UserID);
            //int userIdsUnderWhichUserWillTargetWrongValues = minUserId + numUsersWhoTargetWrongValues;

            for (int batchNum = 0; batchNum < parameters.NumBatchesOfUserRatings; batchNum++)
            {
                for (int userRatingNum = 0; userRatingNum < parameters.NumUserRatingsPerBatch; userRatingNum++)
                {
                    int randomUserNum = RandomGenerator.GetRandom(0, parameters.NumUsers - 1);
                    int randomRowNum = RandomGenerator.GetRandom(0, parameters.NumTblRows - 1);
                    float desiredAdjustmentFactor = baseAdjustmentFactorToApply[randomUserNum];
                    bool userTargetsWrongValue = randomUserNum < parameters.NumUsersWhoTargetWrongValues;
                    decimal valueForUserToTarget = userTargetsWrongValue ?
                        wrongValues[randomRowNum] :
                        correctValues[randomRowNum];
                    Rating theRating = _dataManipulation.DataContext.GetTable<Rating>().Single(x => x.RatingID == randomRowNum + 1);
                    decimal currentValue = theRating.CurrentValue ?? (parameters.MaxRatingValue - parameters.MinRatingValue) / 2;
                    // AdjustmentFactor is defined as:
                    //  (adjustedRating - basisRating) / 
                    //  (rating         - basisRating)
                    // I think the following is trying to fix the ratingValue so that the adjustment factor comes out correctly
                    decimal ratingValue = currentValue + (valueForUserToTarget - currentValue) * (decimal)(1 / desiredAdjustmentFactor);
                    ratingValue = PMTrustCalculations.Constrain(ratingValue, 0M, 10m);
                    UserRatingResponse theResponse = new UserRatingResponse();
                    _testHelper.ActionProcessor.UserRatingAdd(theRating.RatingID, ratingValue, _testHelper.UserIds[randomUserNum], ref theResponse);
                }
                TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(1).GetTotalWholeMilliseconds()); // so that trust will be updated
                _testHelper.WaitIdleTasks();

                float proportionWithinTolerance = calculateProportionWithinTolerance(_dataManipulation.DataContext.GetTable<Rating>().ToArray(),
                    parameters.NumTblRows, correctValues, parameters.Tolerance);
                #region Debug
                {
                    Debug.WriteLine("******************");

                    Debug.WriteLine(String.Format("Batch {0}, {1}% within tolerance.", batchNum + 1, proportionWithinTolerance * 100));

                    IEnumerable<TrustTracker> tts = _dataManipulation.DataContext.GetTable<TrustTracker>().OrderBy(u => u.UserID);
                    foreach (TrustTracker tt in tts)
                    {
                        int userNum = _testHelper.UserIds.ToList().IndexOf(tt.UserID);
                        Debug.WriteLine(String.Format("<User {0}> Trust: {1}/{2} AdjustmentFactor: {3}", tt.UserID, tt.OverallTrustLevel, tt.SkepticalTrustLevel,
                            baseAdjustmentFactorToApply[userNum]));
                    }
                }
                #endregion
                if (proportionWithinTolerance > parameters.RequiredProportionOfRatingsWithinTolerance)
                    break;
            }
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(1).GetTotalWholeMilliseconds());
            _testHelper.WaitIdleTasks();

            float proportionWithinToleranceOfCorrectRating = calculateProportionWithinTolerance(_dataManipulation.DataContext.GetTable<Rating>().ToArray(),
                parameters.NumTblRows, correctValues, parameters.Tolerance);
            proportionWithinToleranceOfCorrectRating.Should().BeGreaterThan(parameters.RequiredProportionOfRatingsWithinTolerance,
                String.Format("because the parameters indicate that {0}% of the ratings should approach the correct value to within a tolerance of ±{1}.",
                    parameters.RequiredProportionOfRatingsWithinTolerance * 100, parameters.Tolerance));
        }

        float calculateProportionWithinTolerance(Rating[] ratings, int numTblRows, decimal[] correctValues, decimal tolerance)
        {
            int numWithinTolerance = 0;
            for (int i = 0; i < numTblRows; i++)
            {
                if (ratings[i].CurrentValue != null &&
                    Math.Abs(ratings[i].CurrentValue.Value - correctValues[i]) < tolerance)
                    numWithinTolerance++;
            }
            return numWithinTolerance / (float)numTblRows;
        }

        /// <summary>
        /// Overall test to see if things get to right number.
        /// Have some number of users, each of whom has a different adjustment factor (a few are negative)
        /// and also a special adjustment factors for some value of the ChoiceInFields.
        /// We have a large number of table rows and continuously add many user ratings. We should then see
        /// that a high percentage are near the goal.
        /// 
        /// If i recall, we are testing whether rateroo correctly recognizes
        /// that user should be trusted more for some types of ratings than
        /// others, based on choiceinfields. Eg, maybe user is better at rating
        /// chinese than indian restaurants. Here we assume a difference in ability
        /// and see if rateroo picks up on it.
        /// </summary>
        [TestMethod]
        public void RateroosRatingsShouldConvergeWhenAPopulationOfUsersPerformsRatingstIncludingSpecialCaseAdjustmentFactor()
        {
            RateroosRatingsShouldConvergeWhenAPopulationOfUsersPerformsRatingstIncludingSpecialCaseAdjustmentFactor_Helper(20, 50, 40, 10, 0.01F, 0.5M, 0.9F);
            //Test_OverallTrustTest_Helper(20, 500, 100, 10, 3, 1);
        }

        /// <summary>
        /// Tests Rateroos's trust system.  Specialist's are users who are particularly good
        /// at rating one type of thing.  Either one Tbl among all tables or one ChoiceInField 
        /// among other ChoiceInFields in the same table.
        /// </summary>
        /// <param name="numBatchesOfUserRatings"></param>
        /// <param name="numUserRatingsPerBatch"></param>
        /// <param name="numTblRows"></param>
        /// <param name="numUsers"></param>
        /// <param name="proportionOfUsersTargettingWrongValues"></param>
        /// <param name="tolerance">The magnitude of deviation allowed by <paramref name="requiredProportionOfRatingsWithinTolerance"/> percent of ratin</param>
        /// <param name="requiredProportionOfRatingsWithinTolerance"></param>
        /// <param name="randomSeed"></param>
        public void RateroosRatingsShouldConvergeWhenAPopulationOfUsersPerformsRatingstIncludingSpecialCaseAdjustmentFactor_Helper(
            int numBatchesOfUserRatings, 
            int numUserRatingsPerBatch, 
            int numTblRows, 
            int numUsers, 
            float proportionOfUsersTargettingWrongValues,
            decimal tolerance, 
            float requiredProportionOfRatingsWithinTolerance,
            int? randomSeed = null)
        {
            const decimal maxRatingValue = 10M;
            const decimal minRatingValue = 0M;

            if (randomSeed != null)
                RandomGenerator.SeedOverride = randomSeed;
            Initialize();

            float[] adjustmentFactorToApply = new float[numUsers];
            float[] specialistAdjustmentFactorToApply = new float[numUsers];
            for (int i = 0; i < numUsers; i++)
            {
                adjustmentFactorToApply[i] = RandomGenerator.GetRandom(0.5F, 0.8F);
                specialistAdjustmentFactorToApply[i] = RandomGenerator.GetRandom(0.5F, 0.8F);
            }

            decimal[] correctValues = new decimal[numTblRows];
            decimal[] wrongValues = new decimal[numTblRows];
            for (int i = 0; i < numTblRows; i++)
            {
                correctValues[i] = (decimal)RandomGenerator.GetRandom(minRatingValue, maxRatingValue);
                wrongValues[i] = (decimal)RandomGenerator.GetRandom(minRatingValue, maxRatingValue);
            }

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(numUsers);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, numTblRows - 1);
            _testHelper.WaitIdleTasks();

            var testEnv = new RaterooTestEnvironmentCreator(_testHelper);
            PointsManager pointsManager = _dataManipulation.DataContext.GetTable<PointsManager>().OrderByDescending(x => x.PointsManagerID).First();
            testEnv.CreateChoiceGroups(pointsManager.PointsManagerID);
            ChoiceGroup choiceGroup = _dataManipulation.DataContext.GetTable<ChoiceGroup>().Single(x => x.Name == "ChoiceGroup single");
            var choiceInGroups = choiceGroup.ChoiceInGroups.ToList();
            int choiceInGroupsCount = choiceInGroups.Count();
            int fieldDefinitionId = _testHelper.ActionProcessor.FieldDefinitionCreate(_testHelper.Tbl.TblID, "SimpChoice", 
                FieldTypes.ChoiceField, true, choiceGroup.ChoiceGroupID, null, true, true, _testHelper.SuperUserId, null);
            int[] randomChoiceIds = new int[numTblRows];
            for (int rowNum = 0; rowNum < numTblRows; rowNum++)
            {
                randomChoiceIds[rowNum] = choiceInGroups[RandomGenerator.GetRandom(1, choiceInGroupsCount - 1)].ChoiceInGroupID;
                TblRow tblRow = _dataManipulation.DataContext.GetTable<TblRow>().OrderBy(x => x.TblRowID).ToList().Skip(rowNum).First();
                _testHelper.ActionProcessor.ChoiceFieldWithSingleChoiceCreateOrReplace(tblRow, fieldDefinitionId, randomChoiceIds[rowNum], 
                    _testHelper.SuperUserId, null);
            }

            // The ChoiceInGroups that will trigger a different adj factor for this user
            int[] choiceInGroupIdThatTriggersSpecialAdjustmentFactor = new int[numUsers];
            for (int i = 0; i < numUsers; i++)
                choiceInGroupIdThatTriggersSpecialAdjustmentFactor[i] = RandomGenerator.GetRandom(0, choiceInGroupsCount - 1); 

            for (int batchNum = 0; batchNum < numBatchesOfUserRatings; batchNum++)
            {
                for (int userRatingNum = 0; userRatingNum < numUserRatingsPerBatch; userRatingNum++)
                {
                    int randomUserNum = RandomGenerator.GetRandom(0, numUsers - 1);
                    int randomRowNum = RandomGenerator.GetRandom(0, numTblRows - 1);
                    bool doUseSpecialCaseAdjustmentFactor = randomChoiceIds[randomRowNum] == choiceInGroupIdThatTriggersSpecialAdjustmentFactor[randomUserNum];
                    float desiredAdjustmentFactor = doUseSpecialCaseAdjustmentFactor ?
                        specialistAdjustmentFactorToApply[randomUserNum] : 
                        adjustmentFactorToApply[randomUserNum];
                    decimal valueForUserToTarget = RandomGenerator.GetRandom() < proportionOfUsersTargettingWrongValues ?
                        wrongValues[randomRowNum] :
                        correctValues[randomRowNum];
                    Rating theRating = _dataManipulation.DataContext.GetTable<Rating>().Single(x => x.RatingID == randomRowNum + 1);
                    decimal currentValue = theRating.CurrentValue ?? (maxRatingValue - minRatingValue) / 2;
                    // Since AdjustmentFactor is defined as:
                    //  (adjustedRating - basisRating) / 
                    //  (rating         - basisRating)
                    // The following should achieve the desired AdjustmentFactor, I think.
                    decimal ratingValue = currentValue + (valueForUserToTarget - currentValue) * (decimal)(1 / desiredAdjustmentFactor);
                    ratingValue = Math.Min(Math.Max(ratingValue, 0M), 10m);
                    UserRatingResponse theResponse = new UserRatingResponse();
                    _testHelper.ActionProcessor.UserRatingAdd(theRating.RatingID, ratingValue, _testHelper.UserIds[randomUserNum], ref theResponse);
                }
                TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds()); // so that trust will be updated
                _testHelper.WaitIdleTasks();
            }
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(2).GetTotalWholeMilliseconds()); // 2 days/
            _testHelper.WaitIdleTasks();

            Rating[] theRatings = _dataManipulation.DataContext.GetTable<Rating>().ToArray();
            int numWithinTolerance = 0;
            for (int i = 0; i < numTblRows; i++)
            {
                if (theRatings[i].CurrentValue != null &&
                    Math.Abs(theRatings[i].CurrentValue.Value - correctValues[i]) < tolerance)
                    numWithinTolerance++;
            }
            float proportionWithinToleranceOfCorrectRating = numWithinTolerance / (float)numTblRows;
            proportionWithinToleranceOfCorrectRating.Should().BeGreaterThan(requiredProportionOfRatingsWithinTolerance, 
                String.Format("because the parameters indicate that {0}% of the ratings should approach the correct value to within a tolerance of ±{1}.",
                    proportionWithinToleranceOfCorrectRating * 100, tolerance));
        }

        [TestMethod]
        public void RaterooDeletesUserInteractionsWhenAnotherUserBecomesTheLatestRatingUser()
        {
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(4);

            UserRatingResponse theResponse = new UserRatingResponse();
            decimal user1RatingValue = 7M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1RatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();

            decimal user2RatingValue = 8M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2RatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();
            
            UserInteraction user1User2Interaction = _dataManipulation.DataContext.GetTable<UserInteraction>()
                .Single(ui =>
                    ui.OrigRatingUserID == _testHelper.UserIds[1] &&
                    ui.LatestRatingUserID == _testHelper.UserIds[2]);
            user1User2Interaction.Should().NotBeNull();

            decimal user3RatingValue = 5M;
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user3RatingValue, _testHelper.UserIds[3], ref theResponse);
            _testHelper.WaitIdleTasks();
            
            _dataManipulation.DataContext.GetTable<UserInteraction>()
                .SingleOrDefault(ui =>
                    ui.OrigRatingUserID == _testHelper.UserIds[1] &&
                    ui.LatestRatingUserID == _testHelper.UserIds[3])
                .Should().NotBeNull();
            
           _dataManipulation.DataContext.GetTable<UserInteraction>()
                .SingleOrDefault(ui =>
                    ui.OrigRatingUserID == _testHelper.UserIds[1] &&
                    ui.LatestRatingUserID == _testHelper.UserIds[2])
                .Should().BeNull();
        }
    }
}
