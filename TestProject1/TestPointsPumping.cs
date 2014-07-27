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
using ClassLibrary1.EFModel;
using ClassLibrary1.Nonmodel_Code;
using FluentAssertions;

namespace TestProject1
{
    [TestClass]
    public class TestPointsPumping
    {
        const float Precision = 0.0001f;

        R8RDataManipulation _dataManipulation;
        TestHelper theTestHelper;
        Random _random;

        public void Initialize()
        {
            GetIR8RDataContext.UseRealDatabase = Test_UseRealDatabase.UseReal();
            TestableDateTime.UseFakeTimes();
            TestableDateTime.SleepOrSkipTime(TimeSpan.FromDays(1).GetTotalWholeMilliseconds());
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = false;
            theTestHelper = new TestHelper();
            _dataManipulation = new R8RDataManipulation();

            _random = new Random();
        }

        [TestMethod]
        [Category("Long")]
        public void SimplePointsPumpingInitiativeCounteracted()
        {
            Initialize();
            theTestHelper.CreateSimpleTestTable(true);
            int numFakeUsers = 100;
            theTestHelper.CreateUsers(100 + 1);
            TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes = true;

            UserEditResponse theResponse = new UserEditResponse();
            int fakeUserNumber = 1;

            Guid user0 = theTestHelper.UserIds[0];
            for (int i = 1; i <= numFakeUsers * 2; i++)
            {
                if (i % 2 == 1)
                    theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, 4.0M, user0, ref theResponse);
                else
                {
                    theTestHelper.ActionProcessor.UserRatingAdd(theTestHelper.Rating.RatingID, 5.0M, theTestHelper.UserIds[fakeUserNumber], ref theResponse);
                    fakeUserNumber++;
                }
                theTestHelper.FinishUserRatingAdd(theTestHelper.ActionProcessor.DataManipulation); // must do this, otherwise next user rating will get an adjustment factor of 0.
                if (i % 10 == 0)
                    theTestHelper.WaitIdleTasks();
            }
            theTestHelper.WaitIdleTasks();

            decimal? ppp = theTestHelper.ActionProcessor.DataContext.GetTable<UserRating>().Where(x => x.UserID == user0).FirstOrDefault().PointsPumpingProportion;
            ppp.Should().Equals(1.0M);
        }

        [TestMethod]
        [Category("UnitTest")]
        public void CheckPointsPumpingProportionSetting()
        {
            CheckPointsPumpingProportionSetting_Helper(
                5, 
                // index up/down     0U     1U     2D     3D     4U     5D    6U    7D    8U     9D     10U   11U    12D
                new List<decimal>() 
                                    { 6,     7,     6,     5,     5.5M,  4.5M, 7.0M, 4.0M, 7.0M,  4.0M,  5.0M, 7.0M,  3.0M }, 
                new List<bool>() 
                                    { false, false, false, false, true, false, true, true, false, false, false, true, false }, 
                new List<float> 
                { 
                    0.0F, // 0: moving user rating without help should not be points pumping
                    0.0F, // 1: moving user rating further without help should not be points pumping
                    1.0F, // 2: here, user 1 pushed to 7 without points so moving back to 6 is points pumping
                    1.0F, // 3: 5 to 6 was covered by user 0 without points, so points pumping
                    1.0F, // 4: since user 3 without points moved from 6 to 5, moving back to 5.5 is points pumping; it doesn't matter that new user has points
                    0.0F, // 5: moving from 5.5M to 5.0M is not points pumping because user 4 had points (and user 0's move was made irrelevant by user 4's move), and 5.0M to 4.5M is not because that's pushing beyond
                    1.0F, // 6: user 4's move from 5 to 5.5 was already challenged by user 5, so it and everything else is points pumping
                    0.0F, // 7: 4.5M to 7.0M was covered by user 6, and 4.0 is beyond 4.5, so no points pumping
                    0.0F, // 8: user 7 covered this in reverse; there are now no creditable unchallenged segments
                    1.0F, // 9: all points pumping since witihn range traversed
                    1.0F, // 10: entire range from 7 to 4 was covered including 5 to 4
                    1.0F, // 11: entire range from 7 to 4 was covered including 7 to 5
                    0.25F, // 12: 5.0 to 7.0 was covered by user 11, but 4.0 to 5.0 was traveled previously and not challenged; 3.0 to 4.0 is beyond previous range
                }
                    );


            //Arguably, this is a bad approach, because once you've seen that there is controversy, you might have a profitable strategy (if you assume that the current value is more likely to represent the true value) to move it away from the current value and back, but it's at least risky given the existence of controversy. 
        }

        public void CheckPointsPumpingProportionSetting_Helper(decimal previousRatingOrVirtualRating, List<decimal> userRatings, List<bool> usersHaveSufficientPoints, List<float> expectedPointsPumpingProportion)
        {
            int n = userRatings.Count();
            List<UserRating> urList = new List<UserRating>();
            List<PointsTotal> ptList = new List<PointsTotal>();
            const decimal assumedMaxGain = 10.0M;
            // Setup 
            for (int i = 0; i < n; i++)
            {
                UserRating ur = new UserRating() { UserRatingID = Guid.NewGuid() };
                if (i == 0)
                    ur.PreviousRatingOrVirtualRating = previousRatingOrVirtualRating;
                else
                    ur.PreviousRatingOrVirtualRating = userRatings[i - 1];
                ur.NewUserRating = userRatings[i];
                if (ur.PreviousRatingOrVirtualRating == ur.NewUserRating)
                    ur.MaxGain = 0;
                else
                    ur.MaxGain = assumedMaxGain; // we just need an assumption 
                urList.Add(ur);
                PointsTotal pt = new PointsTotal() { PointsTotalID = Guid.NewGuid() };
                if (usersHaveSufficientPoints[i])
                    pt.TotalPoints = 1000000; // gives user sufficient points
                // assuming this is the first user rating
                pt.PointsPumpingProportionAvg_Numer = 0;
                pt.PointsPumpingProportionAvg_Denom = 0;
                ptList.Add(pt);
            }
            R8RDataManipulation.SetPointsPumpingProportion(urList, ptList);
            for (int i = 0; i < n; i++)
            {
                if (urList[i].MaxGain == 0)
                {
                    ptList[i].PointsPumpingProportionAvg_Numer.Should().BeApproximately(0, 0.01F, "because the user rating did not move");
                    ptList[i].PointsPumpingProportionAvg_Denom.Should().BeApproximately(0, 0.01F, "because the user rating did not move");
                }
                else
                {
                    ((float)urList[i].PointsPumpingProportion).Should().BeApproximately(expectedPointsPumpingProportion[i], 0.01F);
                    ptList[i].PointsPumpingProportionAvg_Numer.Should().BeApproximately(expectedPointsPumpingProportion[i] * (float) assumedMaxGain, 0.01F, "based on expected calculation provided");
                    ptList[i].PointsPumpingProportionAvg_Denom.Should().BeApproximately(1.0F * (float)assumedMaxGain, 0.01F, "based max gain was 1.0");
                }
            }

        }
    }
}
