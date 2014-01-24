using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;

namespace TestProject1
{
    /// <summary>
    /// These users rate 
    /// </summary>
    class HeterogeneousUser
    {
        TestHelper TestHelper { get; set; }
        public int UserId { get; private set; }
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
            int userId,
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

        /// <summary>
        /// Returns a weighted average of _userRatingTargetValue and
        /// a random value between the minimum and maximum user rating
        /// value.  The weight of the former is _quality and the latter
        /// 1 - _quality.
        /// </summary>
        public decimal GetUserRatingEstimate(decimal userRatingValueTarget)
        {
            return 
                (decimal)Quality * userRatingValueTarget + 
                (decimal)(1 - Quality) * RandomGenerator.GetRandom(Rating.MinRatingValue, Rating.MaxRatingValue);
        }

        public void Rate(Rating rating, decimal userRatingValueTarget, bool subversiveUserIgnoresPreviousRatings)
        {
            decimal userRatingValue;

            decimal userRatingEstimate = GetUserRatingEstimate(userRatingValueTarget);

            if (UserRatingEstimateWeight == Int32.MaxValue ||
                rating.UserRatings.Count < 1 ||
                subversiveUserIgnoresPreviousRatings)
            {
                userRatingValue = userRatingEstimate;
            }
            else
            {
                decimal previousUserRatingValueAverage = rating.UserRatings
                    .Where(ur => ur.UserID != this.UserId)
                    .Average(ur => ur.EnteredUserRating);
                int previousUserRatingCount = rating.UserRatings.Count;
                userRatingValue = 
                    (UserRatingEstimateWeight * userRatingEstimate + 
                        previousUserRatingCount * previousUserRatingValueAverage) 
                    /
                    (UserRatingEstimateWeight + previousUserRatingCount);
            }
            
            UserRatingResponse theResponse = new UserRatingResponse();
            TestHelper.ActionProcessor.UserRatingAdd(rating.RatingID, userRatingValue, UserId, ref theResponse);
        }
    }
}
