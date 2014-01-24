using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using FluentAssertions;
using Debug = System.Diagnostics.Debug;

namespace TestProject1
{
    [TestClass]
    public class HeterogeneousUserTests
    {
        static float _CalculateProportionWithinTolerance(IEnumerable<Rating> ratings, decimal target, decimal tolerance)
        {
            int numWithinTolerance = ratings
                .Where(r =>
                    r.CurrentValue != null &&
                    Math.Abs(r.CurrentValue.Value - target) < tolerance)
                .Count();
            float proportionWithinToleranceOfTarget = numWithinTolerance / (float)ratings.Count(); // Floating-point division
            return proportionWithinToleranceOfTarget;
        }

        private decimal _CalculateAverageAbsoluteError(IEnumerable<Rating> ratings, decimal correctRatingValue)
        {
            return ratings.Average(r => Math.Abs(r.CurrentValue.Value - correctRatingValue));
        }

        TestHelper _testHelper;
        RaterooDataManipulation _dataManipulation;

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
        public void HeterogeneousTestWorksWithPerfectConditions()
        {
            RatingsShouldConvergeWhenAPopulationOfHeterogeneousUsersPerformRatings_Helper(
                tblRowCount: 20,
                userCount: 20,
                subversivePercentage: 0f,
                quality: 1,
                userRatingEstimateWeight: Int32.MaxValue,
                correctRatingValue: 8m,
                subversiveUserRatingvalue: 2m,
                rounds: 10,
                userRatingsPerRating: 20,
                tolerance: 0.5m,
                requiredProportionOfRatingsWithinTolerance: 0.95f,
                breakUponSuccess: false,
                subversiveUserIgnoresPreviousRatings: false);
        }
        
        [TestMethod]
        public void HeterogeneousTestWorksWithPerfectConditionsPlusSubversiveUsers()
        {
            RatingsShouldConvergeWhenAPopulationOfHeterogeneousUsersPerformRatings_Helper(
                tblRowCount: 20,
                userCount: 20,
                subversivePercentage: 0.1f,
                quality: 1,
                userRatingEstimateWeight: Int32.MaxValue,
                correctRatingValue: 8m,
                subversiveUserRatingvalue: 2m,
                rounds: 20,
                userRatingsPerRating: 20,
                tolerance: 0.5m,
                requiredProportionOfRatingsWithinTolerance: 0.95f,
                breakUponSuccess: false,
                subversiveUserIgnoresPreviousRatings: false);
        }

        [TestMethod]
        public void HeterogeneousTestWorksWithRealisticConditions()
        {
            RatingsShouldConvergeWhenAPopulationOfHeterogeneousUsersPerformRatings_Helper(
                tblRowCount: 20,
                userCount: 20,  
                subversivePercentage: 0.1f,
                quality: 0.8f,
                userRatingEstimateWeight: Int32.MaxValue,
                correctRatingValue: 8m,
                subversiveUserRatingvalue: 2m,
                rounds: 40,
                userRatingsPerRating: 8,
                tolerance: 1m,
                requiredProportionOfRatingsWithinTolerance: 0.90f,
                breakUponSuccess:true,
                subversiveUserIgnoresPreviousRatings: false);
        }

        public void RatingsShouldConvergeWhenAPopulationOfHeterogeneousUsersPerformRatings_Helper(
            int tblRowCount,
            int userCount,
            float subversivePercentage,
            double quality,
            int userRatingEstimateWeight,
            decimal correctRatingValue,
            decimal subversiveUserRatingvalue,
            int rounds,
            int userRatingsPerRating,
            decimal tolerance,
            float requiredProportionOfRatingsWithinTolerance,
            bool breakUponSuccess,
            bool subversiveUserIgnoresPreviousRatings
        )
        {
            /* 
             * Setup
             */
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(userCount);
            _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, tblRowCount);
            _testHelper.WaitIdleTasks();

            Action skip1Hour = () => TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(1).GetTotalWholeMilliseconds());
            HeterogeneousUserPool pool = new HeterogeneousUserPool(_testHelper, quality,
                userRatingEstimateWeight, subversivePercentage, afterEachRatingAction: skip1Hour);
            
            /*
             * Perform Ratings
             */
            foreach (int i in Enumerable.Range(0, rounds))
            {
                pool.PerformRatings(correctRatingValue, subversiveUserRatingvalue, _testHelper.Tbl, userRatingsPerRating,
                    subversiveUserIgnoresPreviousRatings);
                _testHelper.WaitIdleTasks();
                IEnumerable<Rating> interRatings = _testHelper.ActionProcessor.DataContext.GetTable<Rating>()
                    .Where(r => r.RatingGroup.TblRow.Tbl.Equals(_testHelper.Tbl));
                float interProportion = _CalculateProportionWithinTolerance(interRatings, correctRatingValue, tolerance);
                #region Debug
                Debug.WriteLine(String.Format("Round {0}: {1}% within tolerance", i, interProportion * 100));
                #endregion
                if (breakUponSuccess && 
                    interProportion > requiredProportionOfRatingsWithinTolerance)
                    break;
            }

            /*
             * Analyze Results
             */
            IEnumerable<Rating> ratings = _testHelper.ActionProcessor.DataContext.GetTable<Rating>()
                .Where(r => r.RatingGroup.TblRow.Tbl.Equals(_testHelper.Tbl));
            float proportion = _CalculateProportionWithinTolerance(ratings, correctRatingValue, tolerance);
            #region Debug
            //IEnumerable<Rating> ratings2 = _testHelper.ActionProcessor.DataContext.GetTable<Rating>()
            //    .Where(r => r.RatingGroup.TblRow.Tbl.Equals(_testHelper.Tbl));
            //foreach (Rating rating in ratings)
            //{
            //    Debug.WriteLine(String.Format("<Rating {0}> CurrentValue={1} LastTrustedValue={2}", rating.RatingID,
            //        rating.CurrentValue, rating.LastTrustedValue));
            //    foreach (UserRating userRating in rating.UserRatings)
            //    {
            //        Debug.WriteLine(String.Format("\t<UserRating {0}> EnteredUserRating={1}", userRating.RatingID, 
            //            userRating.EnteredUserRating));
            //        TrustTracker trustTracker = userRating.User.TrustTrackers.Single();
            //        Debug.WriteLine(String.Format(
            //            "\t\t<User {0}> OverallTrustLevel={1} SkepticalTrustLevel={2}", 
            //            userRating.User.UserID, trustTracker.OverallTrustLevel, trustTracker.SkepticalTrustLevel));
            //    }
            //}
            #endregion
            proportion.Should().BeGreaterThan(requiredProportionOfRatingsWithinTolerance,
                String.Format(
                    "because the parameters indicate that {0}% of the ratings should approach the correct value to within a tolerance of ±{1}.",
                    requiredProportionOfRatingsWithinTolerance * 100, tolerance));
        }

        [TestMethod]
        public void AbsoluteErrorWithinBoundsWhenHeterogeneousUsersRateIterationsofTblRows()
        {
            AbsoluteErrorWithinBoundsWhenHeterogeneousUsersRateIterationsofTblRows_Helper(
                tblRowsPerIteration: 20,
                userCount: 20,  
                subversivePercentage: 0.1f,
                quality: 0.9f,
                userRatingEstimateWeight: 5,
                correctRatingValue: 8m,
                subversiveUserRatingvalue: 6m,
                iterations: 40,
                roundsPerIteration: 1,
                userRatingsPerRatingPerRound: 20,
                maximumAllowableAverageAbsoluteErrorForFinalIteration: 1m,
                subversiveUserIgnoresPreviousRatings: false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tblRowsPerIteration">The number of TblRows that will be created
        /// each iteration, and upon only which UserRatings will be made.</param>
        /// <param name="userCount">The number of users making UserRatings</param>
        /// <param name="subversivePercentage">The percentage of userCount that is subversive</param>
        /// <param name="quality">The quality of any user</param>
        /// <param name="userRatingEstimateWeight">The degree to which users pay attention to previous UserRatings in making their rating</param>
        /// <param name="correctRatingValue"></param>
        /// <param name="subversiveUserRatingvalue"></param>
        /// <param name="iterations">The number of times new TblRows will be created and rounds of UserRatings created on them.</param>
        /// <param name="roundsPerIteration">The number of rating-rounds that will occur each iteration.  multiple rating
        /// rounds allow for the same users to rate a rating more than once.</param>
        /// <param name="userRatingsPerRatingPerRound">The number of users that will make a single UserRating per round</param>
        /// <param name="maximumAllowableAverageAbsoluteErrorForFinalIteration">
        /// The maximum absolute error that ratings for the last iteration of TblRows can have on average to pass the test.</param>
        public void AbsoluteErrorWithinBoundsWhenHeterogeneousUsersRateIterationsofTblRows_Helper(
            int tblRowsPerIteration,
            int userCount,
            float subversivePercentage,
            double quality,
            int userRatingEstimateWeight,
            decimal correctRatingValue,
            decimal subversiveUserRatingvalue,
            int iterations,
            int roundsPerIteration,
            int userRatingsPerRatingPerRound,
            decimal maximumAllowableAverageAbsoluteErrorForFinalIteration,
            bool subversiveUserIgnoresPreviousRatings)
        {
            /* 
             * Setup
             */
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(userCount);

            Action skip1Hour = () => TestableDateTime.SleepOrSkipTime(TimeSpan.FromHours(1).GetTotalWholeMilliseconds());
            HeterogeneousUserPool pool = new HeterogeneousUserPool(_testHelper, quality,
                userRatingEstimateWeight, subversivePercentage, afterEachRatingAction: skip1Hour);

            /*
             * Perform Ratings
             */
            foreach (int iteration in Enumerable.Range(0, iterations))
            {
                IEnumerable<TblRow> tblRows = _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, tblRowsPerIteration);
                _testHelper.WaitIdleTasks();
                IEnumerable<Rating> ratings =  _testHelper.ActionProcessor.DataContext.GetTable<Rating>()
                    .Where(r => tblRows.Contains(r.RatingGroup.TblRow));

                foreach (int round in Enumerable.Range(0, roundsPerIteration))
                {
                    pool.PerformRatings(ratings, correctRatingValue, subversiveUserRatingvalue, userRatingsPerRatingPerRound,
                        subversiveUserIgnoresPreviousRatings);
                    _testHelper.WaitIdleTasks();

                    /*
                     * Calculate the intermediate absolute error.
                     */
                    decimal averageAbsError2 = _CalculateAverageAbsoluteError(ratings, correctRatingValue);
                    #region Debug
                    Debug.WriteLine(String.Format("Iteration {0} Round {1} Average Absolute Error: {2}", iteration, round, averageAbsError2));
                    #endregion
                }

                /*
                 * After the final iteration, 
                 */
                if (iteration == iterations - 1)
                {
                    /*
                     * Analyze Results
                     */
                    decimal averageAbsError = _CalculateAverageAbsoluteError(ratings, correctRatingValue);
                    #region Debug
                    Debug.WriteLine(String.Format("Final Average Absolute Error: {0}", averageAbsError));
                    #endregion
                    averageAbsError.Should().BeLessOrEqualTo(maximumAllowableAverageAbsoluteErrorForFinalIteration);
                }
            }

        }
    }
}
