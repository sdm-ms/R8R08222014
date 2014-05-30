using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public enum TrustStat
    {
        NoExtraWeighting = 0, // just include the rating magnitude (which is multiplied by all the other statistics)
        LargeDeltaRatings = 1, // is this an attempt to move the rating a large distance?
        SmallDeltaRatings = 2, // is this an attempt to move the rating a small distance only?
        Extremeness = 3, // is this a movement to near the end of the probability continuum?
        LastWeekDistanceFromStart = 4, // has this on net moved much lately?
        LastWeekPushback = 5, // has there been recent controversy?
        LastYearPushback = 6, // has there been controversy generally?
        IsFirstUserRating = 7, // is this something that hasn't previously been rated?
        IsUsersFirstWeek = 8, // is this the user's first week?
        IsMostRecent10PercentOfUsersUserRatings = 9, // 1 if most recent 10% of user ratings; will need to be updated as user does more user ratings
        IsMostRecent30PercentOfUsersUserRatings = 10, // 1 if most recent half of ratings; will need to be updated as user does more user ratings
        IsMostRecent70PercentOfUsersUserRatings = 11, // 1 if most recent 70% of ratings; will need to be updated as user does more user ratings
        IsMostRecent90PercentOfUsersUserRatings = 12 // 1 if most recent 90% of ratings; will need to be updated as user does more user ratings
        // NumStats must be set below to the total number
    }

    [Serializable()]
    public class TrustTrackerChoiceSummary
    {
        public int ChoiceInGroupID;
        public float SumAdjustmentPctTimesRatingMagnitude;
        public float SumRatingMagnitudes;
        public float TrustValueForChoice;
    }

    public static class TrustTrackerTrustEveryone
    {
        internal static bool OverrideAdjustmentFactors = false;
        public static bool AllAdjustmentFactorsAre1ForTestingPurposes { get { return OverrideAdjustmentFactors; } set { OverrideAdjustmentFactors = value; } }
        public static bool LatestUserEgalitarianTrustAlways1 = false;
    }

    public class TrustTrackerStatManager
    {

        public static int NumStats = 13; 

        float _adjustmentFactor;
        public float AdjustmentFactor
        {
            get
            {
                if (TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes)
                    return 1F;
                return _adjustmentFactor;
            }
            set
            {
                _adjustmentFactor = value;
            }

        }

        public static float MinAdjustmentFactorToCreditUserRating = 0.30F;
        public float AdjustmentFactorConservative
        {
            get
            {
                if (AdjustmentFactor < MinAdjustmentFactorToCreditUserRating)
                    return 0;
                return (AdjustmentFactor - MinAdjustmentFactorToCreditUserRating) / (1.0F - MinAdjustmentFactorToCreditUserRating);

            }
        }

        internal bool SingleNumberRating;
        internal float RatingMagnitude;
        internal float[] StatValueUnadjusted = new float[NumStats];
        internal List<TrustTrackerChoiceSummary> TrustTrackerChoiceSummary;

        /// <summary>
        /// Use this constructor before we have created the UserRating object. This allows us to calculate the adjustment percentage that we
        /// should give for a new UserRating.
        /// </summary>
        /// <param name="R8RDB"></param>
        /// <param name="theRating"></param>
        /// <param name="theUser"></param>
        /// <param name="enteredRating"></param>
        /// <param name="trustTrackerForChoiceFieldsSummary"></param>
        /// <param name="otherChoiceInFieldIDs"></param>
        /// <param name="additionalInfo"></param>
        public TrustTrackerStatManager(
            IR8RDataContext R8RDB, 
            Rating theRating, User theUser, PointsTotal pointsTotal,
            decimal enteredRating, 
            List<TrustTrackerChoiceSummary> trustTrackerForChoiceFieldsSummary, 
            List<int> otherChoiceInFieldIDs, 
            out UserRatingHierarchyAdditionalInfo additionalInfo)
        {
            TrustTrackerChoiceSummary = trustTrackerForChoiceFieldsSummary;
            SingleNumberRating = theRating.RatingGroup.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || 
                theRating.RatingGroup.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || 
                theRating.RatingGroup.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber;
            decimal currentRatingOrBasisOfCalc;
            if (theRating.CurrentValue != null)
                currentRatingOrBasisOfCalc = (decimal)theRating.CurrentValue;
            else
                currentRatingOrBasisOfCalc = R8RDataManipulation.GetAlternativeBasisForCalcIfNoPreviousUserRating(R8RDB, theRating, theRating.RatingGroup.RatingGroupAttribute);
            VolatilityTracker oneWeekVolatility = theRating.RatingGroup.VolatilityTrackers.FirstOrDefault(x => x.DurationType == (int) VolatilityDuration.oneWeek);
            VolatilityTracker oneYearVolatility = theRating.RatingGroup.VolatilityTrackers.FirstOrDefault(x => x.DurationType == (int)VolatilityDuration.oneYear);
            bool isFirstUserRating = theRating.TotalUserRatings == 0; // this may turn out to be wrong once we finally process it, but it doesn't matter; right now, we're just making a forecast
            bool isUsersFirstWeek = pointsTotal == null || pointsTotal.FirstUserRating > TestableDateTime.Now - TimeSpan.FromDays(7);

            TrustTracker theTrustTracker = theUser.TrustTrackers.FirstOrDefault(x => x.TrustTrackerUnit == (theRating.RatingGroup.TblColumn.TrustTrackerUnit ?? theRating.RatingGroup.TblRow.Tbl.PointsManager.TrustTrackerUnit));
            if (theTrustTracker == null)
                theTrustTracker = TrustTrackingBackgroundTasks.AddTrustTracker(R8RDB, theUser, theRating.RatingGroup.TblColumn.TrustTrackerUnit ?? theRating.RatingGroup.TblRow.Tbl.PointsManager.TrustTrackerUnit);
            TrustTrackerStat[] theTrustTrackerStats = theTrustTracker.TrustTrackerStats.ToArray();
            SetStats(theRating.LastTrustedValue ?? currentRatingOrBasisOfCalc, theRating.CurrentValue, currentRatingOrBasisOfCalc, enteredRating, oneWeekVolatility.DistanceFromStart, oneWeekVolatility.Pushback, oneYearVolatility.Pushback, isFirstUserRating, isUsersFirstWeek, theRating.RatingGroup.RatingGroupAttribute.RatingCharacteristic, theRating.RatingGroup.RatingGroupAttribute.RatingCharacteristic.SubsidyDensityRangeGroup == null ? null : theRating.RatingGroup.RatingGroupAttribute.RatingCharacteristic.SubsidyDensityRangeGroup.UseLogarithmBase);
            SetAdjustmentFactor(theRating.CurrentValue, theTrustTrackerStats);
            additionalInfo = new UserRatingHierarchyAdditionalInfo(AdjustmentFactorConservative, theTrustTracker.OverallTrustLevel, (float) oneWeekVolatility.DistanceFromStart, (float) oneWeekVolatility.Pushback, (float) oneYearVolatility.Pushback, trustTrackerForChoiceFieldsSummary, otherChoiceInFieldIDs);
        }

        /// <summary>
        /// Use this constructor after a UserRating already exists. This allows change to the adjustment percentage (where the user's trust has changed sufficiently).
        /// </summary>
        /// <param name="theUserRating"></param>
        /// <param name="theRatingCharacteristic"></param>
        /// <param name="trustTrackerStats"></param>
        public TrustTrackerStatManager(UserRating theUserRating, RatingCharacteristic theRatingCharacteristic)
        {
            TrustTrackerChoiceSummary = theUserRating.TrustTrackerForChoiceInGroupsUserRatingLinks.Select(x => new TrustTrackerChoiceSummary { ChoiceInGroupID = x.TrustTrackerForChoiceInGroup.ChoiceInGroupID, SumAdjustmentPctTimesRatingMagnitude = x.TrustTrackerForChoiceInGroup.SumAdjustmentPctTimesRatingMagnitude, SumRatingMagnitudes = x.TrustTrackerForChoiceInGroup.SumRatingMagnitudes }).ToList();
            SetStats(theUserRating.PreviousRatingOrVirtualRating, theUserRating.PreviousDisplayedRating, theUserRating.PreviousDisplayedRating ?? theUserRating.PreviousRatingOrVirtualRating, theUserRating.EnteredUserRating, theUserRating.LastWeekDistanceFromStart, theUserRating.LastWeekPushback, theUserRating.LastYearPushback, theUserRating.UserRatingNumberForUser == 1, theUserRating.IsUsersFirstWeek, theRatingCharacteristic, theUserRating.LogarithmicBase);
        }

        public static float MinThresholdToBeConsideredHighMagnitudeRating = 0.3F; // a userrating with less than a 30% change will get a 0 in this category
        public static float MaxThresholdToBeConsideredLowMagnitudeRating = 0.7F; // a userrating with more than a 70% change will get a 0 in this category

        /// <summary>
        /// Set the Stat Manager's statistics based on data reflecting the situation at the time the user entered the UserRating.
        /// </summary>
        /// <param name="lastTrustedRatingOrBasisOfCalc"></param>
        /// <param name="currentRating"></param>
        /// <param name="currentRatingOrBasisOfCalc"></param>
        /// <param name="enteredUserRating"></param>
        /// <param name="oneDayVolatility"></param>
        /// <param name="oneHourVolatility"></param>
        /// <param name="theRatingCharacteristic"></param>
        /// <param name="logBase"></param>
        /// <param name="pctPreviousRatings"></param>
        /// <param name="trustTrackerStats"></param>
        void SetStats(
            decimal lastTrustedRatingOrBasisOfCalc, 
            decimal? currentRating, 
            decimal currentRatingOrBasisOfCalc, 
            decimal enteredUserRating, 
            decimal lastWeekDistanceFromStart,
            decimal lastWeekPushback,
            decimal lastYearPushback,
            bool isFirstUserRating,
            bool isUsersFirstWeek,
            RatingCharacteristic theRatingCharacteristic, 
            decimal? logBase)
        {
            RatingMagnitude = AdjustmentFactorCalc.CalculateRelativeMagnitude(enteredUserRating, lastTrustedRatingOrBasisOfCalc,
                theRatingCharacteristic.MinimumUserRating, theRatingCharacteristic.MaximumUserRating, logBase);
            
            StatValueUnadjusted[(int) TrustStat.NoExtraWeighting] = 1.0F;
            if (RatingMagnitude < MinThresholdToBeConsideredHighMagnitudeRating)
                StatValueUnadjusted[(int) TrustStat.LargeDeltaRatings] = 0; // setting 0 for most cases eliminates effects of weird rounding errors
            else
                StatValueUnadjusted[(int)TrustStat.LargeDeltaRatings] = (float)Math.Pow(RatingMagnitude, 2); // This makes bigger ratings matter more. Note that even for numbers between 0 and 1, 4^2/9^2 is lower than 4/9. 
            if (RatingMagnitude > MaxThresholdToBeConsideredLowMagnitudeRating)
                StatValueUnadjusted[(int)TrustStat.SmallDeltaRatings] = 0;
            else
                StatValueUnadjusted[(int)TrustStat.SmallDeltaRatings] = (float)Math.Pow((1F - RatingMagnitude), 2);
            StatValueUnadjusted[(int)TrustStat.Extremeness] = (float)AdjustmentFactorCalc.CalculateExtremeness(enteredUserRating, logBase,
                theRatingCharacteristic.MinimumUserRating, theRatingCharacteristic.MaximumUserRating);
            StatValueUnadjusted[(int)TrustStat.LastWeekDistanceFromStart] = (float)lastWeekDistanceFromStart;
            StatValueUnadjusted[(int)TrustStat.LastWeekPushback] = (float)lastWeekPushback;
            StatValueUnadjusted[(int)TrustStat.LastYearPushback] = (float)lastYearPushback;
            StatValueUnadjusted[(int)TrustStat.IsFirstUserRating] = isFirstUserRating ? 1.0F : 0.0F;
            StatValueUnadjusted[(int)TrustStat.IsUsersFirstWeek] = isUsersFirstWeek ? 1.0F : 0.0F;
            StatValueUnadjusted[(int)TrustStat.IsMostRecent10PercentOfUsersUserRatings] = 1.0F; // a new user rating is always recent
            StatValueUnadjusted[(int)TrustStat.IsMostRecent30PercentOfUsersUserRatings] = 1.0F; // a new user rating is always recent
            StatValueUnadjusted[(int)TrustStat.IsMostRecent70PercentOfUsersUserRatings] = 1.0F; // a new user rating is always recent
            StatValueUnadjusted[(int)TrustStat.IsMostRecent90PercentOfUsersUserRatings] = 1.0F; // a new user rating is always recent
        }

        private void SetAdjustmentFactor(decimal? currentRating, TrustTrackerStat[] trustTrackerStats)
        {
            if (currentRating == null)
                AdjustmentFactor = 1F;
            else
                CalculateAdjustmentFactorForUserRating(trustTrackerStats);
        }


        /// <summary>
        /// Gets the statistic corresponding to the statistic number. For example, if statNum is 1,
        /// this will be the NoExtraWeighting statistic, i.e. how much the user is trying to move
        /// the rating.
        /// </summary>
        /// <param name="statNum"></param>
        /// <returns></returns>
        public float GetStat(int statNum)
        {
            return StatValueUnadjusted[statNum] * RatingMagnitude;
        }

        /// <summary>
        /// Convert TrustTrackerChoiceSummary objects to TrustTrackerStats, so that all can be considered together in determining an adjustment percentage.
        /// </summary>
        /// <returns></returns>
        internal TrustTrackerStat[] ConvertTrustTrackerChoiceSummaryToTrustTrackerStats()
        {
            return TrustTrackerChoiceSummary.Select(x => new TrustTrackerStat { StatNum = -1, Trust_Numer = x.SumAdjustmentPctTimesRatingMagnitude, Trust_Denom = x.SumRatingMagnitudes, TrustValue = x.TrustValueForChoice }).ToArray();
        }

        /// <summary>
        /// Combine a TrustTrackerStat with information relevant to determining how to weigh it with other TrustTrackerStats given
        /// a statistic.
        /// </summary>
        internal class TrustTrackerStatWithValue
        {
            /// <summary>
            /// The trust tracker statistic for the user. This keeps track of the degree to which the user is trusted, as weighted by 
            /// this statistic.
            /// </summary>
            public TrustTrackerStat TrustTrackerStat;
            /// <summary>
            /// The value of this statistic for this user rating. For example, if the statistic is extremeness of rating, this will
            /// reflect whether the rating is to one of the ends of the rating continuum.
            /// </summary>
            public float StatValue;
            /// <summary>
            /// What is the contribution of this statistic value relative to the pool of all statistics of this type?
            /// The "pool" is the sum of the all StatValues of this type, and the portion is this stat value divided by that sum.
            /// This is useful only in comparison to other TrustTrackerStatWithValues. If the PortionOfPool is relatively high,
            /// especially as measured in contrast to the PortionOfPool for the statistic with no special weighting,
            /// it means that, as measured by the relevant statistic, this user rating is relatively similar to those in the pool.
            /// For example, if the statistic is one day volatility, then it would indicate that this is a user rating with relatively
            /// high one-day volatility.
            /// </summary>
            public double PortionOfPool
            { 
                get 
                {
                    return TrustTrackerStat.SumUserInteractionStatWeights == 0 ? 0 : StatValue / TrustTrackerStat.SumUserInteractionStatWeights;
                } 
            }
            /// <summary>
            /// The statistic being used or null for choices.
            /// </summary>
            public TrustStat? StatNum;

            /// <summary>
            /// This will be set to a value that averages the trust value for this statistic with a trust value for the simplest statistic.
            /// The simplest statistic will receive no weight when PortionOfPool is sufficiently small, i.e. we have a lot of data for this situation.
            /// When PortionOfPool is large (i.e., there is little data for this statistic), then we regress back to the trust value for the simplest statistic.
            /// </summary>
            public double RegressedMeanTrustValue;

            /// <summary>
            /// absolute difference between trust value and the simple trust tracker stat's value, so we weigh more
            /// heavily trust trackers that indicate a greater difference in how much the user can be trusted.
            /// </summary>
            public double Weight;
        }

        public static bool UseOverallTrustValueOnly = false;

        internal void CalculateAdjustmentFactorForUserRating(TrustTrackerStat[] trustTrackerStats)
        {
            TrustTrackerStat simpleTrustTrackerStat = trustTrackerStats.Single(tts => tts.StatNum == 0);
            if (simpleTrustTrackerStat.SumUserInteractionStatWeights == 0 || UseOverallTrustValueOnly)
            {
                AdjustmentFactor = (float) simpleTrustTrackerStat.TrustValue;
            }
            else
            {
                TrustTrackerStatWithValue simpleTrustTrackerStatWithValue = new TrustTrackerStatWithValue {
                    TrustTrackerStat = simpleTrustTrackerStat,
                    StatValue = GetStat(simpleTrustTrackerStat.StatNum)
                };
                List<TrustTrackerStatWithValue> nonChoiceTrustTrackerStatWithValues = trustTrackerStats
                    .Where(tts => tts.StatNum != 1 && SingleNumberRating)
                    .Select(tts => new TrustTrackerStatWithValue() {
                        TrustTrackerStat = tts,
                        StatValue = GetStat(tts.StatNum)
                    }).ToList();
                List<TrustTrackerStatWithValue> choiceTrustTrackerStatWithValues = ConvertTrustTrackerChoiceSummaryToTrustTrackerStats()
                    .Select(tts => new TrustTrackerStatWithValue() {
                        TrustTrackerStat = tts,
                        StatValue = 1 // for now, we only keep track of 1 statistic for the ChoiceInFields.
                    }).ToList();
                // Select only those TrustTrackerStats which are particularly significant for this rating (Portion of Pool > Portion of Main Pool)
                List<TrustTrackerStatWithValue> alternativeTrustTrackerStatWithValues = 
                    nonChoiceTrustTrackerStatWithValues.Concat(choiceTrustTrackerStatWithValues)
                        .Where(ttswsv => ttswsv.PortionOfPool > simpleTrustTrackerStatWithValue.PortionOfPool).ToList();
                KeepOnlyMostInfluentialOfMutuallyExclusiveTrustStats(alternativeTrustTrackerStatWithValues);
                if (!alternativeTrustTrackerStatWithValues.Any())
                {
                    AdjustmentFactor = (float) simpleTrustTrackerStat.TrustValue;
                }
                else
                {
                    // Weight any alternative candidate pools that made the cut for being significant by how much data they contain
                    // Log(1 / PortionOfPool) increases as the Portion of Pool decreases.  Portion of Pool decreasing indicates that there is a more data relative to this one user rating.
                    foreach (TrustTrackerStatWithValue alternativeTrustTrackerStatWithValue in alternativeTrustTrackerStatWithValues)
                    {
                        alternativeTrustTrackerStatWithValue.Weight = 
                            Math.Abs(alternativeTrustTrackerStatWithValue.TrustTrackerStat.TrustValue - simpleTrustTrackerStat.TrustValue);
                        // proportionOfRegressedMeanValue varies from 0 to 1, with 0 meaning that we shouldn't weigh this 
                        // alternative trust tracker stat at all (because there is so little data in the pool) and 1 meaning that we should
                        // give it full weight (because this trust tracker stat is small relative to the pool, and thus the pool should be given
                        // a high amount of weight)
                        float proportionOfRegressedMeanValue = (float)
                            Math.Log(1 / (double)alternativeTrustTrackerStatWithValue.PortionOfPool, 10.0);
                        // so if portion of pool is 1/3, we have log(3); if it's 1/30, we have log(30), 
                        // but because of the next line, once the stat is less than 1/10 of the pool, we count
                        // the pool fully. When it's more than 1/10 of the pool, we count it only partially.
                        if (proportionOfRegressedMeanValue > 1)
                            proportionOfRegressedMeanValue = 1;
                        alternativeTrustTrackerStatWithValue.RegressedMeanTrustValue = 
                            proportionOfRegressedMeanValue * alternativeTrustTrackerStatWithValue.TrustTrackerStat.TrustValue + 
                            (1 - proportionOfRegressedMeanValue) * simpleTrustTrackerStat.TrustValue;
                    }
                    double sumOfWeights = alternativeTrustTrackerStatWithValues.Sum(ttswv => ttswv.Weight);
                    if (sumOfWeights == 0)
                    {
                        AdjustmentFactor = (float) simpleTrustTrackerStat.TrustValue;
                    }
                    else
                    {
                        AdjustmentFactor = 0;
                        // Calculate a weighted average of the trust trackers to use.
                        foreach (var alternativeCandidatePool in alternativeTrustTrackerStatWithValues)
                            AdjustmentFactor += (float) (alternativeCandidatePool.RegressedMeanTrustValue * (alternativeCandidatePool.Weight / sumOfWeights));
                    }
                }
            }
            AdjustmentFactor = TrustCalculations.Constrain(AdjustmentFactor,
    AdjustmentFactorCalc.MinimumPredictiveAdjustmentFactor,
    AdjustmentFactorCalc.MaximumPredictiveAdjustmentFactor);

        }

        private static void KeepOnlyMostInfluentialOfMutuallyExclusiveTrustStats(List<TrustTrackerStatWithValue> alternativeTrustTrackerStatWithValues)
        {
            List<List<TrustStat>> mutuallyExclusiveTrustStatsList = new List<List<TrustStat>>() { 
                    new List<TrustStat>() { TrustStat.IsMostRecent10PercentOfUsersUserRatings, TrustStat.IsMostRecent30PercentOfUsersUserRatings, TrustStat.IsMostRecent70PercentOfUsersUserRatings, TrustStat.IsMostRecent90PercentOfUsersUserRatings },
                    new List<TrustStat>() { TrustStat.LastWeekDistanceFromStart, TrustStat.LastWeekPushback, TrustStat.LastYearPushback },
                };
            foreach (List<TrustStat> mutuallyExclusiveTrustStats in mutuallyExclusiveTrustStatsList)
            {
                var matching = alternativeTrustTrackerStatWithValues.Where(x => x.StatNum != null && mutuallyExclusiveTrustStats.Contains((TrustStat)x.StatNum));
                if (matching.Count() > 1)
                {
                    var highestPortion = matching.Max(x => x.PortionOfPool);
                    TrustTrackerStatWithValue theHighest = matching.First(x => x.PortionOfPool == highestPortion);
                    foreach (var match in matching)
                        if (match != theHighest)
                            alternativeTrustTrackerStatWithValues.Remove(match);
                }
            }
        }

        //internal void CalculateAdjustmentPercentageForUserRatingOldMethod(TrustTrackerStat[] trustTrackerStats)
        //{
        //    if (trustTrackerStats.Count() != NumStats)
        //        throw new Exception("Internal error: Incorrect number of trust tracker stats.");

        //    TrustTrackerStat[] pseudoTrustTrackerStats = ConvertTrustTrackerChoiceSummaryToTrustTrackerStats();
            
        //    int trustTrackerChoiceSummariesToUse = TrustTrackerChoiceSummary == null ? 0 : TrustTrackerChoiceSummary.Count();
        //    float[] weighingFactors = new float[NumStats + trustTrackerChoiceSummariesToUse];
        //    float sumWeighingFactors = 0;
        //    float[] normalizedWeighingFactors = new float[NumStats + trustTrackerChoiceSummariesToUse];
        //    AdjustmentFactor = 0;
        //    int numStatsToUse = NumStats;
        //    if (!SingleNumberRating)
        //        numStatsToUse = 1; // just use the basic aggregation -- we don't want to look at factors like rating magnitude, since that will vary from one rating in a rating group to another
        //    for (int i = 0; i < numStatsToUse; i++)
        //    {
        //        if (trustTrackerStats[i].SumUserInteractionStatWeights == 0)
        //            weighingFactors[i] = 0;
        //        else
        //            weighingFactors[i] = Math.Min((float)0.1, GetStat(i) / trustTrackerStats[i].SumUserInteractionStatWeights);
        //        sumWeighingFactors += weighingFactors[i];
        //    }
        //    for (int i = NumStats; i < NumStats + trustTrackerChoiceSummariesToUse; i++)
        //    {
        //        if (TrustTrackerChoiceSummary[i].SumRatingMagnitudes == 0)
        //            weighingFactors[i] = 0;
        //        else
        //            weighingFactors[i] = Math.Min(0.1F, 1 / TrustTrackerChoiceSummary[i].SumRatingMagnitudes);
        //        sumWeighingFactors += weighingFactors[i];
        //    }
        //    if (sumWeighingFactors == 0)
        //    {
        //        AdjustmentFactor = 1F;
        //        return;
        //    }
        //    for (int i = 0; i < numStatsToUse + trustTrackerChoiceSummariesToUse; i++)
        //    {
        //        normalizedWeighingFactors[i] = weighingFactors[i] / sumWeighingFactors;
        //        AdjustmentFactor += normalizedWeighingFactors[i] * ((i < numStatsToUse) ? trustTrackerStats[i].TrustValue : TrustTrackerChoiceSummary[i].TrustValueForChoice) ;
        //    }
        //}

        public decimal GetNewUserRatingValueToUse(UserRating theUserRating)
        {
            return AdjustmentFactorCalc.GetRatingToAcceptFromAdjustmentFactor(theUserRating.PreviousRatingOrVirtualRating, theUserRating.EnteredUserRating, AdjustmentFactorConservative, theUserRating.LogarithmicBase);
        }

    }

}
