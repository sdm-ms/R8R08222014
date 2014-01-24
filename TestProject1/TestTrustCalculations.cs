using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;
using FluentAssertions;
using FluentAssertions.Assertions;

namespace TestProject1
{
    [TestClass]
    public class TestTrustCalculations
    {
        const decimal MaxRating = 10M;
        const decimal MinRating = 0M;
        const float Precision = 0.0001f;

        RaterooDataManipulation _dataManipulation;
        TestHelper _testHelper;
        Random _random;

        decimal nextRandomRating()
        {
            return Math.Round((decimal)
                (_random.NextDouble() * (double)(MaxRating - MinRating) + (double)MinRating),
                4);
        }

        [TestInitialize()]
        public void Initialize()
        {
            GetIRaterooDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            UseFasterSubmitChanges.Set(false);
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds());
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            _testHelper = new TestHelper();
            _dataManipulation = new RaterooDataManipulation();

            _random = new Random();
        }

        #region AdjustmentFactor
        float naivelyCalculateAdjustmentFactor(
            decimal adjustedValue, 
            decimal value, 
            decimal basisValue)
        {
            return (float)((adjustedValue - basisValue) / (value - basisValue));
        }
        float naivelyCalculateAdjustmentFactor(
            decimal adjustedValue,
            decimal value,
            decimal basisValue, 
            decimal logBase)
        {
            return (float)(adjustedValue - basisValue) / (float)(value - basisValue);
        }
        [TestMethod]
        public void AdjustmentFactorDoesNotDivideByZero()
        {
            decimal adjustedValue = 1m;
            decimal value, basisValue;
            value = basisValue = 0.5m; // When these values are equal, the naive calculation would cause division by zero

            // Make sure the naive calculation throws
            // Actually, the naive calculation will not throw, since a non-zero real divided by a zero real is equal to infinity
            //Action divideByZero = () => { float adj = CalculateAdjustmentFactor(adjustedValue, value, basisValue); };
            //divideByZero.ShouldThrow<DivideByZeroException>();

            // Now make sure that it doesn't
            Action avoidDivideByZero = () => { float adj = PMAdjustmentFactor.CalculateAdjustmentFactor(adjustedValue, value, basisValue); };
            avoidDivideByZero.ShouldNotThrow();

            // And make sure that the value returned when otherwise division by zero would ensue, is correct.
            PMAdjustmentFactor.CalculateAdjustmentFactor(adjustedValue, value, basisValue).Should().BeApproximately(1F, 0F);
        }
        //[TestMethod]
        //public void AdjustmentFactorCalculationConstrainsMaximumValue()
        //{
        //    decimal adjustedValue = 1.0m;
        //    decimal value = 0.5m;
        //    decimal basisValue = 0.0m;
        //    float unconstrainedAdjustmentFactor = (float)(
        //        (adjustedValue - basisValue) / 
        //        (value - basisValue));

        //    unconstrainedAdjustmentFactor.Should().BeGreaterThan(PMAdjustmentFactor.MaximumAdjustmentFactor);

        //    PMAdjustmentFactor.CalculateAdjustmentFactor(adjustedValue, value, basisValue)
        //        .Should().BeApproximately(PMAdjustmentFactor.MaximumAdjustmentFactor, 0F);
        //}
        //[TestMethod]
        //public void AdjustmentFactorCalculationConstrainsMinimumValue()
        //{
        //    decimal adjustedValue = 0.3m;
        //    decimal value = 0.6m;
        //    decimal basisValue = 0.5m;
        //    float unconstrainedAdjustmentFactor = (float)(
        //        (adjustedValue - basisValue) /
        //        (value - basisValue));

        //    unconstrainedAdjustmentFactor.Should().BeLessThan(PMAdjustmentFactor.MinimumAdjustmentFactor);

        //    PMAdjustmentFactor.CalculateAdjustmentFactor(adjustedValue, value, basisValue)
        //        .Should().BeApproximately(PMAdjustmentFactor.MinimumAdjustmentFactor, 0F);
        //}
        #endregion

        #region WeightedAverageAdjustmentFactor
        public void blah()
        {
            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(2); // Must create one more user than needed...Not sure why.  Maybe to make room for the SuperUser?  But why doesn't the SuperUser creation make its own room?
        }
        //public void WeightedAverageAdjustmentFactorCalculatesCorrectlyForRandomRatingByTwoUsers()
        //{
        //    RaterooDataManipulation dataManipulation = new RaterooDataManipulation();

        //    RatingGroup ratingGroup = dataManipulation.AddRatingGroup()
        //    RatingPlan ratingPlan;
        //    RatingGroupPhaseStatus ratingGroupPhaseStatus;
        //    Rating rating = dataManipulation.AddRating(ratingGroup, ratingPlan, ratingGroupPhaseStatus);

        //    User user1 = dataManipulation.AddUser("name1", "email1@email.com", "password1");

        //    const decimal user1RatingValue = 1m;
        //    dataManipulation.AddUserRatingSimple(user1, rating, user1RatingValue);

        //    User user2 = dataManipulation.AddUser("name2", "email2@email.com", "password2");
        //}

        [TestMethod]
        public void WeightedAverageAdjustmentFactorCalculatesCorrectlyForRandomRatingByTwoUsers()
        {

            _testHelper.CreateSimpleTestTable(true);
            _testHelper.CreateUsers(3); // Must create one more user than needed...Not sure why.  Maybe to make room for the SuperUser?  But why doesn't the SuperUser creation make its own room?

            UserRatingResponse theResponse = new UserRatingResponse();
            decimal user1UserRatingValue = nextRandomRating();
            // Must ensure that this rating is trusted
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user1UserRatingValue, _testHelper.UserIds[1], ref theResponse);
            _testHelper.WaitIdleTasks();
            _testHelper.Rating.CurrentValue.Should().Be(user1UserRatingValue);

            decimal user2UserRatingValue = nextRandomRating();
            _testHelper.ActionProcessor.UserRatingAdd(_testHelper.Rating.RatingID, user2UserRatingValue, _testHelper.UserIds[2], ref theResponse);
            _testHelper.WaitIdleTasks();

            UserInteraction userInteraction = _dataManipulation.DataContext.GetTable<UserInteraction>()
                .Single(x =>
                    x.User.UserID == _testHelper.UserIds[1] &&
                    x.User1.UserID == _testHelper.UserIds[2]);
            
            List<UserInteractionStat> userInteractionStats = userInteraction.UserInteractionStats.ToList();

            decimal basisRatingValue = user1UserRatingValue;
            decimal ratingValue = user2UserRatingValue;
            decimal currentRatingValue = user2UserRatingValue;
            float expectedAdjustmentFactor = PMAdjustmentFactor.CalculateAdjustmentFactor(currentRatingValue, ratingValue, basisRatingValue, null);

        }
        #endregion

        #region TrustValue
        #endregion
    }
}
