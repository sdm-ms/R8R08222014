using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using System.Diagnostics;
using System.Threading;

namespace TestProject1
{
    /// <summary>
    /// These users rate 
    /// </summary>
    class HeterogeneousUser
    {
        TestHelper TestHelper { get; set; }
        public Guid userID { get; private set; }
        public HeterogeneousUserType Type { get; private set; }

        /// <summary>
        /// This value ranges from 0.0 to 1.0 and affects 
        /// how close to _userRatingValueTarget the actual
        /// user rating value will be.
        /// </summary>
        public double Quality { get; private set; }
        /// <summary>
        /// This value  is
        /// a measure of the degree to which the user will
        /// update its user rating value away from UserRatingEstimate
        /// based upon the HeterogeneousUser's observation of previous
        /// UserRatings on the same Rating.
        /// 
        /// It must be a positive number and its interpretation is that
        /// when the count of previous UserRatings is equal to it, then
        /// the User's UserRating value is an average of UserRatingEstimate
        /// and the average of the previous UserRating values.
        /// 
        /// Otherwise, the User's UserRating value is a weighted average
        /// of UserRatingEstimate and the average of previous UserRating
        /// values, where the weights are UserRatingEstimateWeight and 
        /// the count of previous UserRatings.
        /// 
        /// Zero will make the HeterogeneousUser always use the average of
        /// previous UserRating values.
        /// MaxValue will make the HeterogeneousUser never consider previous
        /// UserRatings
        /// </summary>
        public int UserRatingEstimateWeight { get; private set; }

        public HeterogeneousUser(
            TestHelper testHelper, 
            Guid userID,
            HeterogeneousUserType type,
            double quality,
            int userRatingEstimateWeight)
        {
            TestHelper = testHelper;
            UserId = userId;
            Type = type;
            Quality = quality;
            UserRatingEstimateWeight = userRatingEstimateWeight;
        }

        public void PrintOutInfo()
        {
            R8RDataManipulation dc = new R8RDataManipulation();
            User theUser = dc.DataContext.GetTable<User>().Single(x => x.UserID == UserId);
            TrustTracker theTrustTracker = theUser.TrustTrackers.First();
            Debug.WriteLine("UserID: " + UserId + " Subversive: " + (Type == HeterogeneousUserType.Subversive) + " Quality: " + Quality + " EstimateWeight: " + UserRatingEstimateWeight + " Overall trust: " + theTrustTracker.OverallTrustLevel + " at last review: " + theTrustTracker.OverallTrustLevelAtLastReview);
        }

        /// <summary>
        /// Returns a weighted average of _userRatingTargetValue and
        /// a random value between the minimum and maximum user rating
        /// value.  The weight of the former is _quality and the latter
        /// 1 - _quality.
        /// </summary>
        public decimal GetUserRatingEstimate(decimal userRatingValueTarget)
        {
            return 
                TrustCalculations.Constrain(
                (decimal) userRatingValueTarget + 
                (decimal)(1 - Quality) * (decimal) RandomGenerator.GetRandom(-2.0, 2.0), // this is a somewhat simplified approach, but if we just weigh in random noise, we will end up with a downward bias
                0, 10);
        }

        public void Rate(Rating rating, decimal userRatingValueTarget, bool subversiveUserIgnoresPreviousRatings)
        {
            decimal userRatingValue;

            decimal userRatingEstimate = GetUserRatingEstimate(userRatingValueTarget);

            List<UserRating> previousUserRatings = TestHelper.ActionProcessor.DataContext.GetTable<UserRating>().Where(ur => ur.UserID != this.UserId && ur.Rating.RatingID == rating.RatingID).ToList();
            int previousUserRatingCount = previousUserRatings.Count;
            if (UserRatingEstimateWeight == Int32.MaxValue ||
                previousUserRatingCount < 1 ||
                (subversiveUserIgnoresPreviousRatings && Type == HeterogeneousUserType.Subversive))
            {
                userRatingValue = userRatingEstimate;
            }
            else
            {
                double? theMedian = previousUserRatings
                    .Median(ur => ur.EnteredUserRating);
                decimal previousUserRatingValueAverage = (decimal)theMedian; // there must be at least 1
                userRatingValue = 
                    (UserRatingEstimateWeight * userRatingEstimate + 
                        previousUserRatingCount * previousUserRatingValueAverage) 
                    /
                    (UserRatingEstimateWeight + previousUserRatingCount);
            }
            
            UserEditResponse theResponse = new UserEditResponse();
            TestHelper.ActionProcessor.UserRatingAdd(rating.RatingID, userRatingValue, UserId, ref theResponse);
            TestHelper.FinishUserRatingAdd(TestHelper.ActionProcessor.DataManipulation);
            TestHelper.ActionProcessor.ResetDataContexts();
        }
    }
}
