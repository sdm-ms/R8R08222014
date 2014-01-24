using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ClassLibrary1.Model
{
    public static class PMAdjustmentFactor
    {
        /// <summary>
        /// The maximum and minimum to which we will constrain AdjustmentFactors when we are looking back at them
        /// to see what effect UserInteractions have upon our trust of the User who has been re-rated. We restrict
        /// this range so as not to give too much weight to outliers. For example, if the original rating is 5 and
        /// a user enters 6, the fact that the end result is 10 does not mean that the user is an even more powerful
        /// predictor than if the end result was 6.5. This is important given that some users may be useful at making
        /// many small changes to user ratings; we then effectively want to examine what proportion of these changes
        /// were in the correct direction, rather than the extent to which the user ratings kept moving in that direction.
        /// </summary>
        public const float MinimumRetrospectiveAdjustmentFactor = -1.25F;
        public const float MaximumRetrospectiveAdjustmentFactor = 1.25F;
        /// <summary>
        /// The maximum and minimum to which we will constrain AdjustmentFactors when we are deciding the extent
        /// to which Rateroo should trust a new UserRating. Given a range of 0 to 1, that means that Rateroo will
        /// never decide to move in the opposite direction of a new user rating, or to move even further than the
        /// user rating. So, if the rating was 5 and the user rating is 6, Rateroo's result will be between 5 and 6.
        /// </summary>
        public const float MinimumPredictiveAdjustmentFactor = 0F;
        public const float MaximumPredictiveAdjustmentFactor = 1F;



        public static float CalculateRelativeMagnitude(
            decimal value, 
            decimal basisValue, 
            decimal minValue, 
            decimal maxValue, 
            decimal? logBase = null)
        {
            if (logBase == null)
                return (float)Math.Abs((value - basisValue) / (maxValue - minValue));
            else
                return Math.Abs((PMTrustCalculations.LogBase(value, (decimal) logBase) - PMTrustCalculations.LogBase(basisValue, (decimal) logBase) ) / (PMTrustCalculations.LogBase(maxValue, (decimal) logBase) - PMTrustCalculations.LogBase(minValue, (decimal) logBase)));
        }

        public static float CalculateExtremeness(decimal value, decimal? logBase, decimal minRating, decimal maxRating)
        {
            decimal midpointRating;
            if (logBase == null)
                midpointRating = (maxRating - minRating)/2;
            else
                midpointRating = (decimal) Math.Pow((double) logBase, (double) ((PMTrustCalculations.LogBase(maxRating, (decimal) logBase) - PMTrustCalculations.LogBase(minRating, (decimal) logBase))/2));
            return CalculateRelativeMagnitude(value, midpointRating, minRating, maxRating, logBase);
        }

        /// <summary>
        /// An AdjustmentFactor measures the extent to which a rating has moved proportionately from an original rating (the basis)
        /// to a user rating.
        /// that Rateroo has not moved the Rating's value at all towards the UserRating's Value.  1.0 indicates that Rateroo thinks that
        /// the User's rating is correct.  ±∞ indicates that the User was completely wrong, but we have a constraint option that will
        /// limit this to a range (currently -1.25, 1.25).
        /// 
        /// Mathematically, it is: 
        ///     (laterValue - basisRating) / 
        ///     (enteredValue         - basisRating)
        ///         when enteredValue =/= basisRating
        /// and
        ///     1
        ///         when enteredValue == basisRating.
        ///     
        /// Where rating was what a user originally entered; basisRating was Rateroo's best guess for the rating value at the time the 
        /// user entered the rating value; and laterValue is Rateroo's guess for the rating value at a later time.
        /// 
        /// If the user enters the same rating as the basis, the denominator would be zero, so instead the factor is defined to be 1.
        /// 
        /// For example let's say on January 1 the rating value is 6.0 (and hypothetically it has no asterisk -- it is trusted). On 
        /// January 2, Carl enters 7.0; the system moves the userrating (hypothetically) to 6.4. On February 1, someone else enters 
        /// 6.6, and the system mostly trusts that person and moves the underlying rating to 6.5. Then, the adjustment percentage for 
        /// the user rating that Carl enters on January 2 should be 0.5 = (6.5 - 6.0) / (7.0 - 6.0)
        /// 
        /// </summary>
        /// <param name="laterValue"></param>
        /// <param name="enteredValue"></param>
        /// <param name="basisValue">The value relative to which the adjustment is calculated</param>
        /// <param name="logBase"></param>
        /// <returns></returns>
        public static float CalculateAdjustmentFactor(
            decimal laterValue, 
            decimal enteredValue,
            decimal basisValue, 
            decimal? logBase = null,
            bool constrainForRetrospectiveAssessment = false)
        {
            float factor;
            if (enteredValue == basisValue)
                factor = 1F;
            else if (logBase == null)
                factor =
                    (float)(laterValue - basisValue) / 
                    (float)(enteredValue - basisValue);
            else
                factor = (PMTrustCalculations.LogBase(laterValue, logBase.Value) - PMTrustCalculations.LogBase(basisValue, logBase.Value)) / 
                    (PMTrustCalculations.LogBase(enteredValue, logBase.Value) - PMTrustCalculations.LogBase(basisValue, logBase.Value));

            if (constrainForRetrospectiveAssessment)
                factor = PMTrustCalculations.Constrain(factor, MinimumRetrospectiveAdjustmentFactor, MaximumRetrospectiveAdjustmentFactor);

            return factor;
        }

        /// <summary>
        /// Retrospective means looking backward, how much of an adjustment have we seen to a user's rating by other
        /// users.      
        /// </summary>
        /// <param name="adjustedValue"></param>
        /// <param name="value"></param>
        /// <param name="basisValue"></param>
        /// <param name="logBase"></param>
        /// <returns></returns>
        //public static float CalculateEmpiricalAdjustmentFactor(
        //    decimal adjustedValue,
        //    decimal value,
        //    decimal basisValue,
        //    decimal? logBase = null)
        //{
        //    float adj = CalculateAdjustmentFactor(adjustedValue, value, basisValue, logBase);
        //    return Constrain(adj, MinimumEmpiricalAdjustmentFactor, MaximumEmpiricalAdjustmentFactor);
        //}

        /// <summary>
        /// Prospective means looking forward, how much of an affect should we allow a user to have on adjusting a rating
        /// </summary>
        /// <param name="adjustedValue"></param>
        /// <param name="value"></param>
        /// <param name="basisValue"></param>
        /// <param name="logBase"></param>
        /// <returns></returns>
        //public static float CalculatePredictiveAdjustmentFactor(
        //    decimal adjustedValue,
        //    decimal value,
        //    decimal basisValue,
        //    decimal? logBase = null)
        //{
        //    float adj = CalculateAdjustmentFactor(adjustedValue, value, basisValue, logBase);
        //    return Constrain(adj, MinimumPredictiveAdjustmentFactor, MaximumPredictiveAdjustmentFactor);
        //}

        public static decimal GetRatingToAcceptFromAdjustmentFactor(
            decimal basisValue, 
            decimal value, 
            float adjustmentFactor, 
            decimal? logBase)
        {
            adjustmentFactor = PMTrustCalculations.Constrain(adjustmentFactor, MinimumPredictiveAdjustmentFactor, MaximumPredictiveAdjustmentFactor);

            if (logBase == null)
                return (decimal) adjustmentFactor * (value - basisValue) + basisValue;
            else
                return (decimal)Math.Pow(
                    (double)logBase.Value, 
                    adjustmentFactor * (PMTrustCalculations.LogBase(value, logBase.Value) - PMTrustCalculations.LogBase(basisValue, logBase.Value)) + 
                        PMTrustCalculations.LogBase(basisValue, logBase.Value));
        }
    }
}
