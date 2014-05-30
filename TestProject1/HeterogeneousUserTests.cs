using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using FluentAssertions;
using Debug = System.Diagnostics.Debug;
using System.Reflection;
using System.Data.SqlClient;

namespace TestProject1
{
    [TestClass]
    public class HeterogeneousUserTests
    {

        TestHelper _testHelper;
        R8RDataManipulation _dataManipulation;

        [TestInitialize()]
        public void Initialize()
        {
            GetIR8RDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds()); // go to next day
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            _testHelper = new TestHelper();
            _dataManipulation = new R8RDataManipulation();
        }

        static float CalculateProportionWithinTolerance(IEnumerable<Rating> ratings, decimal target, decimal tolerance)
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
            var nonNull = ratings.Where(r => r.CurrentValue != null);
            if (nonNull.Any())
                return nonNull.Average(r => Math.Abs(r.CurrentValue.Value - correctRatingValue));
            else
                return decimal.MaxValue;
        }


        [TestMethod]
        [Category("Convergence")]
        public void HeterogeneousTestWorksWithNoSubversiveUsers()
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
                userRatingsPerRating: 4,
                tolerance: 0.5m,
                requiredProportionOfRatingsWithinTolerance: 0.95f,
                breakUponSuccess: true,
                subversiveUserIgnoresPreviousRatings: false);
        }

        [TestMethod]
        [Category("Convergence")]
        public void HeterogeneousTestWorksWithSubversiveUsersAccountingForPreviousRatings()
        {
            RatingsShouldConvergeWhenAPopulationOfHeterogeneousUsersPerformRatings_Helper(
                tblRowCount: 20,
                userCount: 20,
                subversivePercentage: 0.1f,
                quality: 1,
                userRatingEstimateWeight: 12,
                correctRatingValue: 8m,
                subversiveUserRatingvalue: 2m,
                rounds: 5,
                userRatingsPerRating: 4,
                tolerance: 0.5m,
                requiredProportionOfRatingsWithinTolerance: 0.98f,
                breakUponSuccess: true,
                subversiveUserIgnoresPreviousRatings: false);
        }

        [TestMethod]
        [Category("Convergence")]
        public void HeterogeneousTestWorksWithSubversiveUsersIgnoringPreviousRatings()
        {
            RatingsShouldConvergeWhenAPopulationOfHeterogeneousUsersPerformRatings_Helper(
                tblRowCount: 40,
                userCount: 20,  
                subversivePercentage: 0.1f,
                quality: 0.6f,
                userRatingEstimateWeight: 6,
                correctRatingValue: 8m,
                subversiveUserRatingvalue: 2m,
                rounds: 30,
                userRatingsPerRating: 5,
                tolerance: 1m,
                requiredProportionOfRatingsWithinTolerance: 0.95f,
                breakUponSuccess:true,
                subversiveUserIgnoresPreviousRatings: true);
            BackgroundThread.Instance.ExitAsSoonAsPossible();
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

            // This is not needed: TrustTrackerStatManager.UseOverallTrustValueOnly = true;
            TrustCalculations.NumPerfectScoresToGiveNewUser = 2; // reduce this to get faster convergence

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
                float interProportion = CalculateProportionWithinTolerance(interRatings, correctRatingValue, tolerance);
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
            float proportion = CalculateProportionWithinTolerance(ratings, correctRatingValue, tolerance);
            #region Debug
            pool.PrintOutUsersInfo();
            IEnumerable<Rating> ratings2 = _testHelper.ActionProcessor.DataContext.GetTable<Rating>()
                .Where(r => r.RatingGroup.TblRow.Tbl.Equals(_testHelper.Tbl));
            foreach (Rating rating in ratings)
            {
                if (Math.Abs(rating.CurrentValue.Value - correctRatingValue) > tolerance)
                {
                    Debug.WriteLine(String.Format("<Rating {0}> CurrentValue={1} LastTrustedValue={2}", rating.RatingID,
                        rating.CurrentValue, rating.LastTrustedValue));
                    foreach (UserRating userRating in rating.UserRatings)
                    {
                        TrustTracker trustTracker = userRating.User.TrustTrackers.Single();
                        Debug.Write(String.Format("\t<Rating {0}> <UserRating {1}> PreviousUserRating={2} EnteredUserRating={3} NewUserRating={4}", rating.RatingID, userRating.UserRatingID,
                            userRating.PreviousDisplayedRating,
                            userRating.EnteredUserRating, userRating.NewUserRating));
                        Debug.WriteLine(String.Format(
                            "\t\t<User {0}> OverallTrustLevel={1} SkepticalTrustLevel={2}",
                            userRating.User.UserID, trustTracker.OverallTrustLevel, trustTracker.SkepticalTrustLevel));
                    }
                }
            }
            #endregion
            proportion.Should().BeGreaterThan(requiredProportionOfRatingsWithinTolerance,
                String.Format(
                    "because the parameters indicate that {0}% of the ratings should approach the correct value to within a tolerance of ±{1}.",
                    requiredProportionOfRatingsWithinTolerance * 100, tolerance));
        }


        [TestMethod]
        [Category("Convergence")]
        public void AbsoluteErrorWithinBoundsWhenHeterogeneousUsersRateIterationsofTblRows()
        {
            AbsoluteErrorWithinBoundsWhenHeterogeneousUsersRateIterationsofTblRows_Helper(
                tblRowsPerIteration: 20, 
                userCount: 20,  
                subversivePercentage: 0.1f,
                quality: 0.9f,
                userRatingEstimateWeight: 2,
                correctRatingValue: 8m,
                subversiveUserRatingvalue: 6m,
                iterations: 20, 
                roundsPerIteration: 1,
                userRatingsPerRating: 5,
                maximumAllowableAverageAbsoluteErrorForFinalIteration: 0.15m,
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
        /// <param name="userRatingsPerRating">The number of users that will make a single UserRating per round</param>
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
            int userRatingsPerRating,
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
                int countCurrentTblRows = _testHelper.ActionProcessor.DataContext.GetTable<TblRow>().Select(x => x).Count();
                List<TblRow> tblRows = _testHelper.AddTblRowsToTbl(_testHelper.Tbl.TblID, tblRowsPerIteration).ToList();
                _testHelper.WaitIdleTasks();
                List<int> tblRowIDs = _testHelper.ActionProcessor.DataContext.GetTable<TblRow>().Where(x => !(x.RatingGroups.SelectMany(y => y.Ratings.SelectMany(z => z.UserRatings))).Any()).Select(x => x.TblRowID).ToList(); // TblRows without any user ratings
                Func<List<Rating>> reload_ratings = () => _testHelper.ActionProcessor.DataContext.GetTable<Rating>()
                        .Where(r => tblRowIDs.Contains(r.RatingGroup.TblRowID)).ToList(); // load ratings from the most recent set of tbl rows -- must do this after each background task
                List<Rating> ratings = reload_ratings();
                foreach (int round in Enumerable.Range(0, roundsPerIteration))
                {

                    ratings = reload_ratings();
                    pool.PerformRatings(ratings, correctRatingValue, subversiveUserRatingvalue, userRatingsPerRating,
                        subversiveUserIgnoresPreviousRatings);
                    _testHelper.WaitIdleTasks();
                    ratings = reload_ratings();

                    /*
                     * Calculate the intermediate absolute error.
                     */
                    decimal averageAbsError2 = _CalculateAverageAbsoluteError(ratings, correctRatingValue);
                    if (averageAbsError2 < maximumAllowableAverageAbsoluteErrorForFinalIteration)
                        return; // success
                    #region Debug
                    Debug.WriteLine(String.Format("Iteration {0} Round {1} Average Absolute Error: {2}", iteration, round, averageAbsError2));
                    #endregion
                }

                ratings = reload_ratings();
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
