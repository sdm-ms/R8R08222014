using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
////using PredRatings;
using MoreStrings;

using ClassLibrary1.Model;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        //  Methods related to calculating points for predictions

        /// <summary>
        /// Returns the current subsidy level for a rating, taking into account any subsidy adjustment factor(s)
        /// for the rating, as well as any high stakes adjustments.
        /// </summary>
        /// <param name="ratingID"></param>
        /// <returns></returns>
        public decimal GetSubsidyLevelAtTime(Rating theRating, RatingGroup topmostRatingGroup, RatingGroupPhaseStatus theRatingGroupPhaseStatus, DateTime theTime)
        {
            
            decimal baseSubsidyLevel = theRatingGroupPhaseStatus.RatingPhase.SubsidyLevel;
            var neverEndingAdjustments = theRatingGroupPhaseStatus.SubsidyAdjustments.Where(sa => sa.EffectiveTime < theTime && sa.EndingTime == null && sa.Status == (Byte)StatusOfObject.Active);
            var endingAdjustments = theRatingGroupPhaseStatus.SubsidyAdjustments.Where(sa => sa.EffectiveTime < theTime && sa.EndingTime != null && sa.Status == (Byte)StatusOfObject.Active).Where(sa => sa.EndingTime > theTime);
            var subsidyLevel = baseSubsidyLevel;
            foreach (var adj1 in neverEndingAdjustments)
                subsidyLevel *= adj1.SubsidyAdjustmentFactor;
            foreach (var adj1 in endingAdjustments)
                subsidyLevel *= adj1.SubsidyAdjustmentFactor;
            return subsidyLevel;
        }

        /// <summary>
        /// Returns the cumulative density of the subsidy density ranges up to a specified point.
        /// </summary>
        /// <param name="theSubsidyDensityRangeGroup">The group of subsidy density ranges</param>
        /// <param name="thePoint">The point (should be from 0 to 1)</param>
        /// <returns>The cumulative density up to that point</returns>
        public decimal GetSubsidyDensityRangeCumDensity(SubsidyDensityRangeGroup theSubsidyDensityRangeGroup, decimal thePoint)
        {
            // See if we have an exact match.
            SubsidyDensityRange theRange = DataContext.GetTable<SubsidyDensityRange>().
                                        SingleOrDefault(r => r.SubsidyDensityRangeGroupID == theSubsidyDensityRangeGroup.SubsidyDensityRangeGroupID && r.RangeTop == thePoint);
            if (theRange != null)
                return (decimal)theRange.CumDensityTop;
            else
            {
                // Find the liquidity range surrounding this one
                SubsidyDensityRange theSurroundingRange = DataContext.GetTable<SubsidyDensityRange>().
                                      Where(r => r.SubsidyDensityRangeGroupID == theSubsidyDensityRangeGroup.SubsidyDensityRangeGroupID).
                                      SingleOrDefault(r => r.RangeBottom <= thePoint && r.RangeTop > thePoint);
                if (theSurroundingRange == null)
                {
                    decimal minimumValue = (decimal)DataContext.GetTable<SubsidyDensityRange>().
                                                Where(r => r.SubsidyDensityRangeGroupID == theSubsidyDensityRangeGroup.SubsidyDensityRangeGroupID).
                                                Min(r => r.RangeBottom);
                    if (thePoint < minimumValue)
                        return GetSubsidyDensityRangeCumDensity(theSubsidyDensityRangeGroup, minimumValue);
                    else
                    {
                        decimal maximumValue = (decimal)DataContext.GetTable<SubsidyDensityRange>().
                                                Where(r => r.SubsidyDensityRangeGroupID == theSubsidyDensityRangeGroup.SubsidyDensityRangeGroupID).
                                                Max(r => r.RangeTop);
                        if (thePoint <= maximumValue)
                            throw new System.Exception("Internal error -- GetSubsidyDensityRangeCumDensity");
                        return GetSubsidyDensityRangeCumDensity(theSubsidyDensityRangeGroup, maximumValue);
                    }
                }
                else
                {
                    decimal theRatio = (decimal)((thePoint - theSurroundingRange.RangeBottom) / (theSurroundingRange.RangeTop - theSurroundingRange.RangeBottom));
                    return theSurroundingRange.CumDensityBottom + theRatio * (theSurroundingRange.CumDensityTop - theSurroundingRange.CumDensityBottom);
                }

            }
        }

        /// <summary>
        /// Returns the distance between two points, adjusted for the density of the subsidy range between those points
        /// In effect, this is equal to the proportion of the total density from 0 to 1 covered by these two points.
        /// </summary>
        /// <param name="theSubsidyDensityRangeGroup">The subsidy density ranges</param>
        /// <param name="startPoint">The starting point (should be 0 to 1)</param>
        /// <param name="endPoint">The ending point (should be 0 to 1)</param>
        /// <returns>The density-adjusted distance</returns>
        public decimal GetDensityAdjustedDistance(SubsidyDensityRangeGroup theSubsidyDensityRangeGroup, decimal startPoint, decimal endPoint)
        {
            if (theSubsidyDensityRangeGroup == null)
                return Math.Abs(endPoint - startPoint);
            else if (theSubsidyDensityRangeGroup.UseLogarithmBase != null)
            { // we don't look for individual subsidy ranges, but just do a linear transformation
                return (decimal) (Math.Log(Math.Abs((double) endPoint - (double) startPoint)) / Math.Log((double)theSubsidyDensityRangeGroup.UseLogarithmBase));
            }
            else 
            { // now we look for specific subsidy ranges
                decimal cumDensityStart = GetSubsidyDensityRangeCumDensity(theSubsidyDensityRangeGroup, startPoint);
                decimal cumDensityEnd = GetSubsidyDensityRangeCumDensity(theSubsidyDensityRangeGroup, endPoint);

                decimal densityCovered = (cumDensityEnd - cumDensityStart);
                decimal totalCumDensity = theSubsidyDensityRangeGroup.CumDensityTotal;

                return Math.Abs(densityCovered / totalCumDensity);
            }
        }

        /// <summary>
        /// Specifies the relative density of a subsidy density range between two points. If the subsidy density range
        /// is 2 between those points and 1 elsewhere, this will return 2.
        /// </summary>
        /// <param name="theSubsidyDensityRangeGroup">The subsidy density range group</param>
        /// <param name="startPoint">The start point (should be between 0 and 1)</param>
        /// <param name="endPoint">The end point (should be between 0 and 1)</param>
        /// <returns>The density adjustment factor</returns>
        public decimal GetDensityAdjustmentFactor(SubsidyDensityRangeGroup theSubsidyDensityRangeGroup, decimal startPoint, decimal endPoint)
        {
            // The adjustment is the proportion of density covered, divided by proportion that it would be covered if uniform
            if (endPoint == startPoint)
                return (decimal)1;
            else
                return Math.Abs(GetDensityAdjustedDistance(theSubsidyDensityRangeGroup, startPoint, endPoint) / (endPoint - startPoint));
        }

        /// <summary>
        /// Applies a specified scoring rule to a prediction. 
        /// </summary>
        /// <param name="theScoringRule">The scoring rule</param>
        /// <param name="scaledUserRating">The prediction (0 to 1)</param>
        /// <param name="scaledFinalResult">The actual result (that is, what was being predicted) (0 to 1)</param>
        /// <returns>The outcome of the scoring rule.</returns>
        public static decimal ApplyScoringRuleFunction(ScoringRules theScoringRule, decimal scaledUserRating, decimal scaledFinalResult)
        {
            if (theScoringRule == ScoringRules.Logarithmic)
            {
                decimal payoffIfOccurs = BoundedLogarithm(scaledUserRating);
                decimal payoffIfDoesntOccur = BoundedLogarithm((decimal)1 - scaledUserRating);
                return scaledFinalResult * payoffIfOccurs + ((decimal)1 - scaledFinalResult) * payoffIfDoesntOccur;
            }
            else if (theScoringRule == ScoringRules.Quadratic)
            {
                //return scaledFinalResult * (scaledUserRating * scaledUserRating - scaledUserRating + (decimal) 0.5) 
                //    + (1 - scaledFinalResult) * ((decimal) 0.5 - scaledUserRating * scaledUserRating);
                decimal sumris = scaledUserRating * scaledUserRating + (1 - scaledUserRating) * (1 - scaledUserRating);
                decimal payoffIfOccurs = 2 * scaledUserRating - sumris;
                decimal payoffIfDoesntOccur = 2 * (1 - scaledUserRating) - sumris;
                return scaledFinalResult * payoffIfOccurs + (1 - scaledFinalResult) * payoffIfDoesntOccur;
            }
            else
            {
                double absoluteDifference = (double) Math.Abs(scaledUserRating - scaledFinalResult);
                double exponent;
                switch (theScoringRule)
                {
                    case ScoringRules.FourthPower:
                        exponent = 4;
                        break;
                    case ScoringRules.Cubic:
                        exponent = 3;
                        break;
                    case ScoringRules.Square:
                        exponent = 2;
                        break;
                    case ScoringRules.Linear:
                        exponent = 1;
                        break;
                    case ScoringRules.SquareRoot:
                        exponent = 0.5;
                        break;
                    case ScoringRules.CubicRoot:
                        exponent = 0.33333333333;
                        break;
                    case ScoringRules.FourthRoot:
                        exponent = 0.25;
                        break;
                    default:
                        throw new Exception("Scoring rule not implemented.");

                }
                return (decimal) (0 - Math.Pow(absoluteDifference, exponent));
            }
        }

        /// <summary>
        /// Returns the maximum raw scoring rule score that can be achieved for a particular scoring rule.
        /// </summary>
        /// <param name="theScoringRule">The scoring rule</param>
        /// <returns>The maximum score that can be achieved</returns>
        public decimal GetMaximumRatingScoringRuleRawScore(ScoringRules theScoringRule)
        {
            const decimal LogScoringRuleMaxRawScore = (decimal)9.21034037;

            if (theScoringRule == ScoringRules.Linear || theScoringRule == ScoringRules.SquareRoot 
                || theScoringRule == ScoringRules.CubicRoot || theScoringRule == ScoringRules.FourthRoot
                || theScoringRule == ScoringRules.Square || theScoringRule == ScoringRules.Cubic
                || theScoringRule == ScoringRules.FourthPower)
                return 1;
            else if (theScoringRule == ScoringRules.Quadratic)
                return (decimal)2;
            else if (theScoringRule == ScoringRules.Logarithmic)
                return LogScoringRuleMaxRawScore;
            else
                throw new Exception("Internal error -- Seeking maximum of unknown scoring rule.");
        }

        /// <summary>
        /// Applies the rating scoring rule -- that is, determines how much a particular prediction improved
        /// the score. The maximum value is 1 and the minimum value is -1. This is then used elsewhere to determine
        /// points adjustments.
        /// </summary>
        /// <param name="theScoringRule">The scoring rule</param>
        /// <param name="theSubsidyDensityRangeGroup">The subsidy density range group</param>
        /// <param name="originalUserRating">The original prediction</param>
        /// <param name="newUserRating">The new prediction</param>
        /// <param name="finalValue">The final value being predicted</param>
        /// <returns>A score (where -1 is worst possible and 1 is best possible)</returns>
        public decimal GetRatingScoringRuleScore(ScoringRules theScoringRule, SubsidyDensityRangeGroup theSubsidyDensityRangeGroup, decimal originalUserRating, decimal newUserRating, decimal finalValue)
        {
            decimal originalUnadjustedScore, newUnadjustedScore;
            decimal originalAdjustedScore, newAdjustedScore;
            decimal ratingScoreRaw, ratingScoreFixed;

            // Apply the scoring rule functions to the various predictions.
            originalUnadjustedScore = ApplyScoringRuleFunction(theScoringRule, originalUserRating, finalValue);
            newUnadjustedScore = ApplyScoringRuleFunction(theScoringRule, newUserRating, finalValue);

            // Adjust the scores based on the density adjustment factor.
            originalAdjustedScore = originalUnadjustedScore * GetDensityAdjustmentFactor(theSubsidyDensityRangeGroup, originalUserRating, finalValue);
            newAdjustedScore = newUnadjustedScore * GetDensityAdjustmentFactor(theSubsidyDensityRangeGroup, newUserRating, finalValue);
            ratingScoreRaw = newAdjustedScore - originalAdjustedScore;

            // Now, adjust so that the score would be 1 if the original prediction was 0 and the new prediction and final value were 1.
            ratingScoreFixed = ratingScoreRaw / GetMaximumRatingScoringRuleRawScore(theScoringRule);

            // System.Diagnostics.Trace.TraceInformation(String.Format("original {0} new {1} final {2} score {3} rule {4}", originalUserRating, newUserRating, finalValue, ratingScoreFixed, theScoringRule.ToString()));

            return ratingScoreFixed;
        }

        /// <summary>
        /// Returns a "bounded logarithm" for values between 0 and 1 -- so that the lowest output of the function = ln(0.0001).
        /// This is useful in practice for applying the logarithmic scoring rule.
        /// </summary>
        /// <param name="thePoint">The point to calculate the logarithm for</param>
        /// <returns>The logarithm, but no lower than ln(0.0001)</returns>
        public static decimal BoundedLogarithm(decimal thePoint)
        {
            const decimal LogScoringRuleBottomOfRange = (decimal)0.0001;
            // const decimal LogScoringRuleLnOfBottomOfRange = (decimal) -9.21034037;

            if (thePoint <= LogScoringRuleBottomOfRange)
                thePoint = LogScoringRuleBottomOfRange;
            else if (thePoint > 1)
                thePoint = 1;

            return ((decimal)System.Math.Log((double)thePoint));
        }

        /// <summary>
        /// Calculates a scaled prediction (0 to 1) from an unscaled prediction (from a minimum to a maximum value)
        /// </summary>
        /// <param name="unscaledUserRating">The unscaled prediction</param>
        /// <param name="predictionMinimum">The minimum prediction</param>
        /// <param name="predictionMaximum">The maximum prediction</param>
        /// <returns></returns>
        public static decimal CalculateScaledUserRating(decimal unscaledUserRating, decimal predictionMinimum, decimal predictionMaximum)
        {
            if (predictionMaximum == 1 && predictionMinimum == 0)
                return unscaledUserRating;
            else
                return (unscaledUserRating - predictionMinimum) / (predictionMaximum - predictionMinimum);
        }

        /// <summary>
        /// A helper routine, returns the profit for an actual or hypothetical prediction and result with a particular scoring rule.
        /// It applies the scoring rule specified (without changing it for the last period).
        /// </summary>
        /// <param name="theScoringRule"></param>
        /// <param name="theSubsidyDensityRangeGroup"></param>
        /// <param name="originalPredictino"></param>
        /// <param name="newUserRating"></param>
        /// <param name="finalUserRating"></param>
        /// <param name="subsidyLevel"></param>
        /// <param name="minVal"></param>
        /// <param name="maxVal"></param>
        /// <returns></returns>
        public decimal CalculateProfit(ScoringRules theScoringRule, SubsidyDensityRangeGroup theSubsidyDensityRangeGroup, decimal originalUserRating, decimal newUserRating, decimal finalUserRating, decimal subsidyLevel, decimal minVal, decimal maxVal)
        {
            decimal scaledOriginalUserRating = CalculateScaledUserRating(originalUserRating, minVal, maxVal);
            decimal scaledNewUserRating = CalculateScaledUserRating(newUserRating, minVal, maxVal);
            decimal scaledFinalUserRating = CalculateScaledUserRating(finalUserRating, minVal, maxVal);

            // with this scoring rule, score is abs(new - final) - abs(original - final)
            return subsidyLevel * GetRatingScoringRuleScore(theScoringRule, theSubsidyDensityRangeGroup, scaledOriginalUserRating, scaledNewUserRating, scaledFinalUserRating);
        }

        /// <summary>
        /// If there is no previous prediction (i.e., this is first trusted one), then we need an alternative to the previous prediction
        /// as the basis of calculating a score for the rating. We use an average value for the current prediction in ratings of
        /// that type (typically within the same category descriptor), or the mean of the lowest and highest values if that isn't
        /// available.
        /// </summary>
        /// <param name="ratingID"></param>
        /// <returns></returns>
        public static decimal GetAlternativeBasisForCalcIfNoPreviousUserRating(IRaterooDataContext theDataContext, Rating theRating, RatingGroupAttribute theRatingGroupAttribute)
        {
            TblColumn theCD = theRating.RatingGroup.TblColumn;

            OverrideCharacteristic theOverride = theDataContext.GetTable<OverrideCharacteristic>().SingleOrDefault(o => o.TblColumnID == theCD.TblColumnID && o.TblRowID == theRating.RatingGroup.TblRowID);
            if (theOverride != null) // this is unique, so we just go by the middle possible rating.
                return (theRating.RatingCharacteristic.MinimumUserRating + theRating.RatingCharacteristic.MaximumUserRating) / 2;

            string cacheKey = "AlternativeBasisFor" + theCD.TblColumnID.ToString() + theRatingGroupAttribute.RatingGroupAttributesID;
            object theAlternativeBasisObject = PMCacheManagement.GetItemFromCache(cacheKey);
            decimal? theAlternativeBasis = (decimal?) theAlternativeBasisObject;
            if (theAlternativeBasis != null)
                return (decimal)theAlternativeBasis;

            var theQuery = theDataContext.GetTable<Rating>().Where(m => 
                m.RatingGroup.TblColumnID == theCD.TblColumnID &&
                m.RatingGroup.RatingGroupAttributesID == theRatingGroupAttribute.RatingGroupAttributesID && 
                m.CurrentValue != null &&
                m.Name == theRating.Name);
            if (theQuery.Count() > 4)
            { // get average based on 100 ratings
                theAlternativeBasis = theQuery.OrderByDescending(m => m.RatingID).Select(x => x.CurrentValue).Where(x => x != null).Take(100).Average(x => (decimal)x);
            }
            else // not enough data
            {
                if (theRating.RatingCharacteristic.SubsidyDensityRangeGroup != null && theRating.RatingCharacteristic.SubsidyDensityRangeGroup.UseLogarithmBase != null)
                    theAlternativeBasis = (decimal)Math.Pow((double)theRating.RatingCharacteristic.SubsidyDensityRangeGroup.UseLogarithmBase, (double)((PMTrustCalculations.LogBase(theRating.RatingCharacteristic.MinimumUserRating, (decimal)theRating.RatingCharacteristic.SubsidyDensityRangeGroup.UseLogarithmBase) + PMTrustCalculations.LogBase(theRating.RatingCharacteristic.MaximumUserRating, (decimal)theRating.RatingCharacteristic.SubsidyDensityRangeGroup.UseLogarithmBase)) / 2));
                else
                    theAlternativeBasis = (theRating.RatingCharacteristic.MinimumUserRating + theRating.RatingCharacteristic.MaximumUserRating) / 2;
            }
           PMCacheManagement.AddItemToCache(cacheKey, new string[] { }, (object)theAlternativeBasis, new TimeSpan(0, 10, 0));
           return (decimal) theAlternativeBasis;
        }


        /// <summary>
        /// Calculates the points that should or would be awarded for changing a prediction.
        /// </summary>
        /// <param name="ratingID">The rating</param>
        /// <param name="originalUserRating">The original prediction</param>
        /// <param name="newUserRating">The new prediction</param>
        /// <param name="finalUserRating">The final prediction</param>
        /// <param name="cost">This is the maximum loss associated with the prediction (can be cost if payment must be up front)</param>
        /// <param name="winnings">The amount that the prediction is now worth</param>
        /// <param name="profit">Winnings minus cost</param>
        public void CalculatePointsInfo(Rating theRating, RatingGroup topmostRatingGroup, RatingGroupPhaseStatus theRatingGroupPhaseStatus, DateTime timeOfUserRating, decimal originalUserRatingOrAltBasisForCalc, decimal newUserRating, decimal? finalUserRating, bool shortTerm, decimal longTermPointsWeight, decimal? highStakesMultiplierOverride, out decimal maxLoss, out decimal maxGain, out decimal profit, out decimal profitIfAllWeightOnThisTerm)
        {
            RatingCharacteristic theCharacteristics = theRating.RatingCharacteristic;
            SubsidyDensityRangeGroup theSubsidyDensityRangeGroup = theCharacteristics.SubsidyDensityRangeGroup;
            ScoringRules theScoringRule = (ScoringRules)(theRatingGroupPhaseStatus.RatingPhase.ScoringRule);

            // For the last period, if ending on an extreme, we use the last period scoring rule.
            ScoringRules scoringRuleForExtremeResults = theScoringRule;
            if (theScoringRule != ScoringRules.Logarithmic)
                scoringRuleForExtremeResults = ScoringRules.Quadratic;

            decimal subsidyLevel = GetSubsidyLevelAtTime(theRating, topmostRatingGroup, theRatingGroupPhaseStatus, timeOfUserRating);
            if (shortTerm)
            {
                subsidyLevel *= highStakesMultiplierOverride ?? HighStakesMultiplier(theRatingGroupPhaseStatus, topmostRatingGroup, timeOfUserRating);
            }

            // Calculate actual profit
            // Also, assign zero profit if the rating conditions aren't met, if the final prediction is null (unwinding),
            // or if the prediction was made after the current resolution time of the rating.
            if (finalUserRating == null)
            { // No points to be awarded.
                profit = 0;
            }
            else
            {
                ScoringRules scoringRuleToUse;
                if (finalUserRating == theCharacteristics.MinimumUserRating || finalUserRating == theCharacteristics.MaximumUserRating)
                    scoringRuleToUse = scoringRuleForExtremeResults;
                else
                    scoringRuleToUse = theScoringRule;
                profit = CalculateProfit(scoringRuleToUse, theSubsidyDensityRangeGroup, originalUserRatingOrAltBasisForCalc, newUserRating, (decimal)finalUserRating, subsidyLevel, theCharacteristics.MinimumUserRating, theCharacteristics.MaximumUserRating);
            }

            // Now, calculate the best and worse case scenario under both scoring rules.
            decimal bestCaseFinalUserRating;
            decimal worstCaseFinalUserRating;
            decimal bestCaseProfitGivenScoringRule;
            decimal bestCaseProfitQuadraticScoringRule;
            decimal worstCaseProfitGivenScoringRule;
            decimal worstCaseProfitQuadraticScoringRule;
            if (newUserRating > originalUserRatingOrAltBasisForCalc)
            {
                bestCaseFinalUserRating = theCharacteristics.MaximumUserRating;
                worstCaseFinalUserRating = theCharacteristics.MinimumUserRating;
            }
            else
            {
                bestCaseFinalUserRating = theCharacteristics.MinimumUserRating;
                worstCaseFinalUserRating = theCharacteristics.MaximumUserRating;
            }
            bestCaseProfitGivenScoringRule = CalculateProfit(theScoringRule, theSubsidyDensityRangeGroup, originalUserRatingOrAltBasisForCalc, newUserRating, bestCaseFinalUserRating, subsidyLevel, theCharacteristics.MinimumUserRating, theCharacteristics.MaximumUserRating);
            if (theScoringRule == scoringRuleForExtremeResults)
                bestCaseProfitQuadraticScoringRule = bestCaseProfitGivenScoringRule;
            else
                bestCaseProfitQuadraticScoringRule = CalculateProfit(scoringRuleForExtremeResults, theSubsidyDensityRangeGroup, originalUserRatingOrAltBasisForCalc, newUserRating, bestCaseFinalUserRating, subsidyLevel, theCharacteristics.MinimumUserRating, theCharacteristics.MaximumUserRating);
            maxGain = Math.Max(bestCaseProfitGivenScoringRule, bestCaseProfitQuadraticScoringRule);
            
            worstCaseProfitGivenScoringRule = CalculateProfit(theScoringRule, theSubsidyDensityRangeGroup, originalUserRatingOrAltBasisForCalc, newUserRating, worstCaseFinalUserRating, subsidyLevel, theCharacteristics.MinimumUserRating, theCharacteristics.MaximumUserRating);
            if (theScoringRule == scoringRuleForExtremeResults)
                worstCaseProfitQuadraticScoringRule = worstCaseProfitGivenScoringRule;
            else
                worstCaseProfitQuadraticScoringRule = CalculateProfit(scoringRuleForExtremeResults, theSubsidyDensityRangeGroup, originalUserRatingOrAltBasisForCalc, newUserRating, worstCaseFinalUserRating, subsidyLevel, theCharacteristics.MinimumUserRating, theCharacteristics.MaximumUserRating);
            maxLoss = 0 - Math.Min(worstCaseProfitGivenScoringRule, worstCaseProfitQuadraticScoringRule);

            decimal pointsWeight = longTermPointsWeight;
            if (shortTerm)
                pointsWeight = (decimal)1 - pointsWeight;
            maxLoss *= pointsWeight;
            maxGain *= pointsWeight;
            profitIfAllWeightOnThisTerm = profit;
            profit *= pointsWeight;

            maxLoss = Math.Round(maxLoss, 4);
            maxGain = Math.Round(maxGain, 4);
            profit = Math.Round(profit, 4);
        }

    }
}
