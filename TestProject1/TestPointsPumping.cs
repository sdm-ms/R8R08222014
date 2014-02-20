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

using ClassLibrary1.Model;
using ClassLibrary1.Misc;
using FluentAssertions;

namespace TestProject1
{
    [TestClass]
    public class TestPointsPumping
    {
        const decimal MaxRating = 10M;
        const decimal MinRating = 0M;
        const float Precision = 0.0001f;

        RaterooDataManipulation _dataManipulation;
        TestHelper _testHelper;
        Random _random;

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

        [TestMethod]
        public void CheckPointsPumpingProportionSetting()
        {
            CheckPointsPumpingProportionSetting_Helper(5, new List<decimal>() { 6 }, new List<bool>() { false }, new List<float> { 0.0F }); // moving user rating without help should not be points pumping
            CheckPointsPumpingProportionSetting_Helper(5, new List<decimal>() { 6, 7 }, new List<bool>() { false, false }, new List<float> { 0.0F, 0.0F }); // moving user rating further without help should not be points pumping
            CheckPointsPumpingProportionSetting_Helper(5, new List<decimal>() { 6, 7, 6 }, new List<bool>() { false, false, false }, new List<float> { 0.0F, 0.0F, 1.0F }); // here, second user pushed to 7 without points so moving back to 6 is points pumping
        }

        public void CheckPointsPumpingProportionSetting_Helper(decimal previousRatingOrVirtualRating, List<decimal> userRatings, List<bool> usersHaveSufficientPoints, List<float> expectedContributionToPointsPumpingNumeratorWithMaxGainOf1)
        {
            int n = userRatings.Count();
            List<UserRating> urList = new List<UserRating>();
            List<PointsTotal> ptList = new List<PointsTotal>();
            // Setup 
            for (int i = 0; i < n; i++)
            {
                UserRating ur = new UserRating();
                if (i == 0)
                    ur.PreviousRatingOrVirtualRating = previousRatingOrVirtualRating;
                else
                    ur.PreviousRatingOrVirtualRating = userRatings[i - 1];
                ur.NewUserRating = userRatings[i];
                if (ur.PreviousRatingOrVirtualRating == ur.NewUserRating)
                    ur.MaxGain = 0;
                else
                    ur.MaxGain = 1.0M; // we just need an assumption 
                urList.Add(ur);
                PointsTotal pt = new PointsTotal();
                if (usersHaveSufficientPoints[i])
                    pt.TotalPoints = 1000000; // gives user sufficient points
                // assuming this is the first user rating
                pt.PointsPumpingProportionAvg_Numer = 0;
                pt.PointsPumpingProportionAvg_Denom = 0;
                ptList.Add(pt);
            }
            RaterooDataManipulation.SetPointsPumpingProportion(urList, ptList);
            for (int i = 0; i < n; i++)
            {
                if (urList[i].MaxGain == 0)
                {
                    ptList[i].PointsPumpingProportionAvg_Numer.Should().BeApproximately(0, 0.01F, "because the user rating did not move");
                    ptList[i].PointsPumpingProportionAvg_Denom.Should().BeApproximately(0, 0.01F, "because the user rating did not move");
                }
                else
                {
                    ptList[i].PointsPumpingProportionAvg_Numer.Should().BeApproximately(expectedContributionToPointsPumpingNumeratorWithMaxGainOf1[i], 0.01F, "based on expected calculation provided");
                    ptList[i].PointsPumpingProportionAvg_Denom.Should().BeApproximately(1.0F, 0.01F, "based max gain was 1.0");
                }
            }

        }
    }
}
