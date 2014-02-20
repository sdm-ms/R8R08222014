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

        public void CheckPointsPumpingProportionSetting(decimal? previousRatingValue, List<decimal> userRatings, List<bool> usersHaveSufficientPoints, decimal expectedContributionToPointsPumpingNumerator, decimal expectedContributionToPointsPumpingDenominator)
        {

        }
    }
}
