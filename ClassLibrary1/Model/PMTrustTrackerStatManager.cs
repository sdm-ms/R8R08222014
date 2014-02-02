using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
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

        internal float NoExtraWeighting;
        internal float LargeDeltaRatings;
        internal float SmallDeltaRatings;
        internal float LastDayVolatility;
        internal float LastHourVolatility;
        internal float Extremeness;
        internal float CurrentRatingQuestionable;
        internal float PercentPreviousRatings;
        internal List<TrustTrackerChoiceSummary> TrustTrackerChoiceSummary;

        /// <summary>
        /// Use this constructor before we have created the UserRating object. This allows us to calculate the adjustment percentage that we
        /// should give for a new UserRating.
        /// </summary>
        /// <param name="RaterooDB"></param>
        /// <param name="theRating"></param>
        /// <param name="theUser"></param>
        /// <param name="enteredRating"></param>
        /// <param name="trustTrackerForChoiceFieldsSummary"></param>
        /// <param name="otherChoiceInFieldIDs"></param>
        /// <param name="additionalInfo"></param>
        public TrustTrackerStatManager(
            IRaterooDataContext RaterooDB, 
            Rating theRating, User theUser,
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
                currentRatingOrBasisOfCalc = RaterooDataManipulation.GetAlternativeBasisForCalcIfNoPreviousUserRating(RaterooDB, theRating, theRating.RatingGroup.RatingGroupAttribute);
            VolatilityTracker oneDayVolatility = theRating.RatingGroup.VolatilityTrackers.FirstOrDefault(x => x.DurationType == (int) VolatilityDuration.oneDay);
            decimal oneDayVolatilityDecimal = (oneDayVolatility == null) ? 0 : oneDayVolatility.Volatility;
            VolatilityTracker oneHourVolatility = theRating.RatingGroup.VolatilityTrackers.FirstOrDefault(x => x.DurationType == (int)VolatilityDuration.oneHour);
            decimal oneHourVolatilityDecimal = (oneHourVolatility == null) ? 0 : oneHourVolatility.Volatility;

            float pctPreviousRatings = 0;
            if (theRating.TotalUserRatings > 0)
                pctPreviousRatings = ((float) theUser.UserRatings.Count(x => x.Rating == theRating)) / ((float) theRating.TotalUserRatings); // DEBUG -- too slow to pull all this data in. We could keep track of it in a separate data structure.

            TrustTracker theTrustTracker = theUser.TrustTrackers.FirstOrDefault(x => x.TrustTrackerUnit == (theRating.RatingGroup.TblColumn.TrustTrackerUnit ?? theRating.RatingGroup.TblRow.Tbl.PointsManager.TrustTrackerUnit));
            if (theTrustTracker == null)
                theTrustTracker = PMTrustTrackingBackgroundTasks.AddTrustTracker(RaterooDB, theUser, theRating.RatingGroup.TblColumn.TrustTrackerUnit ?? theRating.RatingGroup.TblRow.Tbl.PointsManager.TrustTrackerUnit);
            TrustTrackerStat[] theTrustTrackerStats = theTrustTracker.TrustTrackerStats.ToArray();
            SetStats(theRating.LastTrustedValue ?? currentRatingOrBasisOfCalc, theRating.CurrentValue, currentRatingOrBasisOfCalc, enteredRating, oneDayVolatilityDecimal, oneHourVolatilityDecimal, theRating.RatingGroup.RatingGroupAttribute.RatingCharacteristic, theRating.RatingGroup.RatingGroupAttribute.RatingCharacteristic.SubsidyDensityRangeGroup == null ? null : theRating.RatingGroup.RatingGroupAttribute.RatingCharacteristic.SubsidyDensityRangeGroup.UseLogarithmBase, (float) pctPreviousRatings);
            SetAdjustmentFactor(theRating.CurrentValue, theTrustTrackerStats);
            bool isTrusted = true; // we are disabling the approach of trusting only some users
                //(theRating.CurrentValue != null && theTrustTracker.SkepticalTrustLevel > 0) ||
                //(theRating.CurrentValue == null && theTrustTracker.SkepticalTrustLevel > PMTrustTrackingBackgroundTasks.MinSkepticalTrustNeededForTrustedOnInitialRating) || 
                //TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes; 
            additionalInfo = new UserRatingHierarchyAdditionalInfo(AdjustmentFactorConservative, theTrustTracker.OverallTrustLevel, isTrusted, pctPreviousRatings, oneHourVolatilityDecimal, oneDayVolatilityDecimal, trustTrackerForChoiceFieldsSummary, otherChoiceInFieldIDs);
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
            SetStats(theUserRating.PreviousRatingOrVirtualRating, theUserRating.PreviousDisplayedRating, theUserRating.PreviousDisplayedRating ?? theUserRating.PreviousRatingOrVirtualRating, theUserRating.EnteredUserRating, theUserRating.OneDayVolatility ?? 0, theUserRating.OneHourVolatility ?? 0, theRatingCharacteristic, theUserRating.LogarithmicBase, (float) theUserRating.PercentPreviousRatings);
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
            decimal oneDayVolatility, 
            decimal oneHourVolatility, 
            RatingCharacteristic theRatingCharacteristic, 
            decimal? logBase, 
            float pctPreviousRatings)
        {
            RatingMagnitude = PMAdjustmentFactor.CalculateRelativeMagnitude(enteredUserRating, lastTrustedRatingOrBasisOfCalc,
                theRatingCharacteristic.MinimumUserRating, theRatingCharacteristic.MaximumUserRating, logBase);
            
            NoExtraWeighting = 1.0F * RatingMagnitude;
            if (RatingMagnitude < MinThresholdToBeConsideredHighMagnitudeRating)
                LargeDeltaRatings = 0; // setting 0 for most cases eliminates effects of weird rounding errors
            else
                LargeDeltaRatings = (float)Math.Pow(RatingMagnitude, 2) * RatingMagnitude; // This makes bigger ratings matter more. Note that even for numbers between 0 and 1, 4^2/9^2 is lower than 4/9. 
            if (RatingMagnitude > MaxThresholdToBeConsideredLowMagnitudeRating)
                SmallDeltaRatings = 0;
            else
                SmallDeltaRatings = (float)Math.Pow((1F - RatingMagnitude), 2) * RatingMagnitude;
            LastDayVolatility = (float)oneDayVolatility * RatingMagnitude;
            //Debug.WriteLine("One day volatility: " + oneDayVolatility + " time: " + TestableDateTime.Now.ToShortTimeString()); // DEBUG
            LastHourVolatility = (float)oneHourVolatility * RatingMagnitude;
            Extremeness = (float)PMAdjustmentFactor.CalculateExtremeness(enteredUserRating, logBase, 
                theRatingCharacteristic.MinimumUserRating, theRatingCharacteristic.MaximumUserRating) * RatingMagnitude;
            CurrentRatingQuestionable = (float)PMAdjustmentFactor.CalculateRelativeMagnitude(currentRatingOrBasisOfCalc, lastTrustedRatingOrBasisOfCalc, theRatingCharacteristic.MinimumUserRating, theRatingCharacteristic.MaximumUserRating, logBase) *
                RatingMagnitude; // DEBUG -- no longer useful. Maybe swap in something about whether it is a first rating
            PercentPreviousRatings = pctPreviousRatings * RatingMagnitude;
        }

        private void SetAdjustmentFactor(decimal? currentRating, TrustTrackerStat[] trustTrackerStats)
        {
            if (currentRating == null)
                AdjustmentFactor = 1F;
            else
                CalculateAdjustmentFactorForUserRating(trustTrackerStats);
        }

        public static int NumStats = 8; // see PMTrustStat.cs for the list

        /// <summary>
        /// Gets the statistic corresponding to the statistic number. For example, if statNum is 1,
        /// this will be the NoExtraWeighting statistic, i.e. how much the user is trying to move
        /// the rating.
        /// </summary>
        /// <param name="statNum"></param>
        /// <returns></returns>
        public float GetStat(int statNum)
        {
            switch (statNum)
            {
                case (int) TrustStat.NoExtraWeighting:
                    return NoExtraWeighting;
                case (int)TrustStat.LargeDeltaRatings:
                    return LargeDeltaRatings;
                case (int)TrustStat.SmallDeltaRatings:
                    return SmallDeltaRatings;
                case (int)TrustStat.LastDayVolatility:
                    return LastDayVolatility;
                case (int)TrustStat.LastHourVolatility:
                    return LastHourVolatility;
                case (int)TrustStat.Extremeness:
                    return Extremeness;
                case (int)TrustStat.CurrentRatingQuestionable:
                    return CurrentRatingQuestionable;
                case (int)TrustStat.PercentPreviousRatings:
                    return PercentPreviousRatings;
            }
            throw new Exception("Internal error: Unknown Trust Tracker Stat.");
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
            public float PortionOfPool
            { 
                get 
                {
                    return TrustTrackerStat.SumUserInteractionStatWeights == 0 ? 0 : StatValue / TrustTrackerStat.SumUserInteractionStatWeights;
                } 
            }

            /// <summary>
            /// This will be set to a value that averages the trust value for this statistic with a trust value for the simplest statistic.
            /// The simplest statistic will receive no weight when PortionOfPool is sufficiently small, i.e. we have a lot of data for this situation.
            /// When PortionOfPool is large (i.e., there is little data for this statistic), then we regress back to the trust value for the simplest statistic.
            /// </summary>
            public float RegressedMeanTrustValue;

            /// <summary>
            /// absolute difference between trust value and the simple trust tracker stat's value, so we weigh more
            /// heavily trust trackers that indicate a greater difference in how much the user can be trusted.
            /// </summary>
            public float Weight;
        }

        internal void CalculateAdjustmentFactorForUserRating(TrustTrackerStat[] trustTrackerStats)
        {
            TrustTrackerStat simpleTrustTrackerStat = trustTrackerStats.Single(tts => tts.StatNum == 0);
            if (simpleTrustTrackerStat.SumUserInteractionStatWeights == 0)
            {
                AdjustmentFactor = simpleTrustTrackerStat.TrustValue;
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
                if (!alternativeTrustTrackerStatWithValues.Any())
                {
                    AdjustmentFactor = simpleTrustTrackerStat.TrustValue;
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
                    float sumOfWeights = alternativeTrustTrackerStatWithValues.Sum(ttswv => ttswv.Weight);
                    if (sumOfWeights == 0)
                    {
                        AdjustmentFactor = simpleTrustTrackerStat.TrustValue;
                    }
                    else
                    {
                        AdjustmentFactor = 0;
                        // Calculate a weighted average of the trust trackers to use.
                        foreach (var alternativeCandidatePool in alternativeTrustTrackerStatWithValues)
                            AdjustmentFactor += alternativeCandidatePool.RegressedMeanTrustValue * (alternativeCandidatePool.Weight / sumOfWeights);
                    }
                }
                AdjustmentFactor = PMTrustCalculations.Constrain(AdjustmentFactor,
                    PMAdjustmentFactor.MinimumPredictiveAdjustmentFactor,
                    PMAdjustmentFactor.MaximumPredictiveAdjustmentFactor);
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
            return PMAdjustmentFactor.GetRatingToAcceptFromAdjustmentFactor(theUserRating.PreviousRatingOrVirtualRating, theUserRating.EnteredUserRating, AdjustmentFactorConservative, theUserRating.LogarithmicBase);
        }

    }

}
