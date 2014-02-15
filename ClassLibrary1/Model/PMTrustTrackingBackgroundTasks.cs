using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClassLibrary1.Misc;
using System.Data.Linq;
using System.Diagnostics;

namespace ClassLibrary1.Model
{
    public static class PMTrustTrackingBackgroundTasks
    {
        public const float MinSkepticalTrustNeededForTrustedOnInitialRating = 0.4F;

        static int _NumBackgroundTasks = 2;
        static int _CurrentTask = 1;

        internal static void MoveToNextTask()
        {
            _CurrentTask++; 
            if (_CurrentTask == _NumBackgroundTasks + 1)
                _CurrentTask = 1;
        }

        internal static Func<IRaterooDataContext, bool> GetTask()
        {
            // Trace.TraceInformation("Currently executing trust tracking task: " + currentTask); 

            switch (_CurrentTask)
            {
                case 1:
                    return FindAndCorrectUserInteractionLatestUserEgalitarianTrust;
                case 2:
                    return DeleteZereodUserInteractions;
            }

            throw new Exception("Internal error: Unrecognized trust tracking background task.");
        }

        public static bool DoTrustTrackingBackgroundTasks(IRaterooDataContext RaterooDB)
        {
            int tasksToDo = _NumBackgroundTasks; // this might be the same background task six times, or each background task separately. this function will return false (no more work to do) only if each background task returns this. (That is why the number is equal to the total number of background tasks, even though some may not be executed in situations in which at least one background task returns true.)
            bool moreWorkToDoOnSomeTask = false;
            for (int i = 1; i <= tasksToDo; i++)
            {
                Func<IRaterooDataContext, bool> theTask = GetTask();
                bool moreWorkToDoOnThisTask = theTask(RaterooDB);
                RaterooDB.SubmitChanges();
                if (moreWorkToDoOnThisTask)
                    moreWorkToDoOnSomeTask = true;
                else
                    MoveToNextTask(); // move on to next task only when we've finished previous one
            }
            return moreWorkToDoOnSomeTask;
        }

        internal class TrustTrackerAndUserInteraction
        {
#pragma warning disable 0649
            public TrustTracker TrustTracker;
            public UserInteraction UserInteraction;
#pragma warning restore 0649
        }

        internal static bool FindAndCorrectUserInteractionLatestUserEgalitarianTrust(IRaterooDataContext RaterooDB)
        {
            const int numToDoAtOnceEachQuery = 1000;

            // Looking for UserInteractions where the LatestUserEgalitarianTrust is not up to date.
            var query = 
                from x in RaterooDB.GetTable<UserInteraction>()
                let trustTracker = x.TrustTrackerUnit.TrustTrackers.SingleOrDefault(y => y.UserID == x.LatestRatingUserID)
                where trustTracker.MustUpdateUserInteractionEgalitarianTrustLevel
                select new TrustTrackerAndUserInteraction { TrustTracker = trustTracker, UserInteraction = x };

            var data = query.Take(numToDoAtOnceEachQuery).ToArray();

            CorrectLatestUserEgalitarianTrust(data);

            return data.Count() == numToDoAtOnceEachQuery;
        }

        public static float MinChangeToLatestUserEgalitarianTrustBeforeUpdatingWeightInCalculatingTrustTotal = 0.1F;

        internal static void CorrectLatestUserEgalitarianTrust(TrustTrackerAndUserInteraction[] items)
        {
            foreach (var item in items)
            {
                double egalitarianTrustLevelToUse = TrustTrackerTrustEveryone.LatestUserEgalitarianTrustAlways1 ? 1.0 : item.TrustTracker.EgalitarianTrustLevelOverride ?? item.TrustTracker.EgalitarianTrustLevel;
                item.UserInteraction.LatestUserEgalitarianTrust = (float) egalitarianTrustLevelToUse;
                item.TrustTracker.MustUpdateUserInteractionEgalitarianTrustLevel = false;
                if (item.UserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate == null || Math.Abs(item.UserInteraction.LatestUserEgalitarianTrust - (float)item.UserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate) > MinChangeToLatestUserEgalitarianTrustBeforeUpdatingWeightInCalculatingTrustTotal)
                {
                    double originalWeightInCalculatingTrustTotal = item.UserInteraction.WeightInCalculatingTrustTotal;
                    UpdateWeightInCalculatingTrustTotal(item.UserInteraction, egalitarianTrustLevelToUse);
                    UpdateEarlierUserTrustTrackerStatsandEgalitarianTrustAfterUpdatingUserInteraction(item.UserInteraction, null, null, originalWeightInCalculatingTrustTotal);
                    item.UserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate = item.UserInteraction.LatestUserEgalitarianTrust;
                }
            }
        }

        public static void UpdateWeightInCalculatingTrustTotal(
            UserInteraction theUserInteraction,
            double newLatestUserEgalitarianTrust)
        {
            UserInteractionStat ratingMagnitudeStat = theUserInteraction.UserInteractionStats.Single(x => x.StatNum == 0);
            theUserInteraction.WeightInCalculatingTrustTotal = PMTrustCalculations.GetUserInteractionWeightInCalculatingTrustTotal(ratingMagnitudeStat, theUserInteraction);
        }

        public static TrustTracker AddTrustTracker(IRaterooDataContext RaterooDB, User theUser, TrustTrackerUnit theTrustTrackerUnit)
        {
            TrustTracker theTrustTracker = new TrustTracker
            {
                TrustTrackerUnit = theTrustTrackerUnit,
                User = theUser,
                //SkepticalTrustLevel = theTrustTrackerUnit.SkepticalTrustThreshhold == 0 ? 1.0F : 0F, // new users on a new table will be trusted
                OverallTrustLevel = 1,
                EgalitarianTrustLevel = 1,
                EgalitarianTrustLevelOverride = null,
                OverallTrustLevelAtLastReview = 1,
                DeltaOverallTrustLevel = 0,
                SumUserInteractionWeights = 0
            };
            RaterooDB.GetTable<TrustTracker>().InsertOnSubmit(theTrustTracker);
            RaterooDB.RegisterObjectToBeInserted(theTrustTracker);
            AddTrustTrackerStatsForTrustTracker(RaterooDB, theTrustTracker);
            return theTrustTracker;
        }

        internal static TrustTracker[] AddTrustTrackerStatsForTrustTracker(IRaterooDataContext RaterooDB, TrustTracker theTrustTracker)
        {
            TrustTracker[] tt = new TrustTracker[TrustTrackerStatManager.NumStats];
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
            {
                TrustTrackerStat theTrustTrackerStat = new TrustTrackerStat
                {
                    TrustTracker = theTrustTracker,
                    StatNum = (short)i,
                    TrustValue = 1,
                    Trust_Numer = 0,
                    Trust_Denom = 0
                };
                RaterooDB.GetTable<TrustTrackerStat>().InsertOnSubmit(theTrustTrackerStat);
                RaterooDB.RegisterObjectToBeInserted(theTrustTrackerStat);
            }
            return tt;
        }

        internal static UserInteractionStat[] AddUserInteractionStatsForUserInteraction(IRaterooDataContext RaterooDB, UserInteraction theUserInteraction, TrustTrackerStat[] originalUserTrustTrackerStats)
        {
            UserInteractionStat[] uis = new UserInteractionStat[TrustTrackerStatManager.NumStats];
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
            {
                UserInteractionStat theUserInteractionStat = new UserInteractionStat
                {
                    UserInteraction = theUserInteraction,
                    TrustTrackerStat = originalUserTrustTrackerStats[i],
                    StatNum = (short)i,
                    SumAdjustPctTimesWeight = 0,
                    SumWeights = 0,
                    AvgAdjustmentPctWeighted = 0
                };
                // we won't change the Egalitarian_Denom yet -- only when the SumWeights > 0.
                RaterooDB.GetTable<UserInteractionStat>().InsertOnSubmit(theUserInteractionStat);
                RaterooDB.RegisterObjectToBeInserted(theUserInteractionStat);
            }
            return uis;
        }

        public static void AdjustUserInteraction(
            IRaterooDataContext RaterooDB, 
            UserInteraction theUserInteraction, 
            UserRating originalUserRating, 
            TrustTrackerStat[] originalUserTrustTrackerStats, 
            UserRating latestUserRating, 
            RatingCharacteristic ratingCharacteristic, 
            bool subtractFromUserInteraction,
            TrustTracker mostRecentUserTrustTracker)
        {
            if (originalUserRating == null || latestUserRating == null || ratingCharacteristic == null)
                return;
            if (originalUserRating.User == latestUserRating.User)
                return;

            if (theUserInteraction == null)
            { // it could have been created earlier in this background process
                theUserInteraction = RaterooDB.RegisteredToBeInserted.OfType<UserInteraction>().SingleOrDefault(ui => ui.User == originalUserRating.User && ui.User1 == latestUserRating.User && ui.TrustTrackerUnit == latestUserRating.TrustTrackerUnit);
            }

            if (theUserInteraction == null)
            {
                if (mostRecentUserTrustTracker == null) // usually trust trackers are added before the userrating is added, but this might not happen in some circumstances, such as when the user is a superuser
                    mostRecentUserTrustTracker = AddTrustTracker(RaterooDB, latestUserRating.User, latestUserRating.TrustTrackerUnit);

                theUserInteraction = new UserInteraction 
                { 
                    User = originalUserRating.User, 
                    User1 = latestUserRating.User, 
                    TrustTrackerUnit = latestUserRating.TrustTrackerUnit, 
                    LatestUserEgalitarianTrust = (float) (TrustTrackerTrustEveryone.LatestUserEgalitarianTrustAlways1 ? 1.0F : mostRecentUserTrustTracker.EgalitarianTrustLevel),
                    LatestUserEgalitarianTrustAtLastWeightUpdate = (float) mostRecentUserTrustTracker.EgalitarianTrustLevel
                };
                RaterooDB.GetTable<UserInteraction>().InsertOnSubmit(theUserInteraction);
                RaterooDB.RegisterObjectToBeInserted(theUserInteraction);
                AddUserInteractionStatsForUserInteraction(RaterooDB, theUserInteraction, originalUserTrustTrackerStats);
            }

            // We don't want any one instance to have a huge effect on the adjustment percentage, so we need to constrain the adjustment factor to a range of -1.25, 1.25
            float adjustFactor = PMAdjustmentFactor.CalculateAdjustmentFactor(
                laterValue: latestUserRating.NewUserRating, 
                enteredValue: originalUserRating.EnteredUserRating, 
                basisValue: originalUserRating.PreviousRatingOrVirtualRating,
                logBase: originalUserRating.LogarithmicBase,
                constrainForRetrospectiveAssessment: true);
            UpdateUserInteractionStats(theUserInteraction, originalUserRating, ratingCharacteristic, adjustFactor, subtractFromUserInteraction);
//            if (theUserInteraction != null && theUserInteraction.User.UserID == 2)
 //               Debug.WriteLine("AdjustUserInteraction " + theUserInteraction.User.UserID + " " + theUserInteraction.User1.UserID + " subtract: " + subtractFromUserInteraction + " latest user rating: " + (latestUserRating == null ? -1 : latestUserRating.UserID) + " adjust: " + adjustFactor + " latest entered " + latestUserRating.EnteredUserRating + " latest new " + latestUserRating.NewUserRating);
        }

        public static void UpdateUserInteractionStats(
            UserInteraction theUserInteraction, 
            UserRating originalUserRating, 
            RatingCharacteristic ratingCharacteristic, 
            float changeInIndividualUserInteractionAdjustmentFactor, 
            bool subtractFromUserInteraction)
        {
            //Trace.TraceInformation("UpdateUserInteractionStats original user: " + theUserInteraction.User.UserID + " later user: " + theUserInteraction.User1.UserID + " Original user rating: " + originalUserRating.EnteredUserRating + " adjust% " + adjustPercentage + " subtract: " + subtractFromUserInteraction.ToString());
            TrustTrackerStatManager manager = new TrustTrackerStatManager(originalUserRating, ratingCharacteristic);
            double positiveOrNegative = subtractFromUserInteraction ? -1 : 1;
            double[] originalAvgAdjustmentPct = new double[TrustTrackerStatManager.NumStats], originalSumWeights = new double[TrustTrackerStatManager.NumStats];
            UserInteractionStat noExtraWeightingStat = null;
            theUserInteraction.NumTransactions += 1 * (int)positiveOrNegative;
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
            {
                UserInteractionStat userInteractionStat = theUserInteraction.UserInteractionStats.Single(x => x.StatNum == i);
                if (i == 0)
                    noExtraWeightingStat = userInteractionStat;
                double userRatingStat = manager.GetStat(i);
                originalAvgAdjustmentPct[i] = userInteractionStat.AvgAdjustmentPctWeighted;
                originalSumWeights[i] = userInteractionStat.SumWeights;
                double sumAdjustPctTimesWeightDelta = changeInIndividualUserInteractionAdjustmentFactor * userRatingStat * positiveOrNegative;
                double sumWeightsDelta = userRatingStat * positiveOrNegative;
                // bool revertingToCloseToZero = Math.Abs((sumWeightsDelta + userInteractionStat.SumWeights) / userInteractionStat.SumWeights) < 0.00001;
                double originalSumAdjustPctTimesWeight = userInteractionStat.SumAdjustPctTimesWeight;
                userInteractionStat.SumAdjustPctTimesWeight += sumAdjustPctTimesWeightDelta;
                if (theUserInteraction.NumTransactions == 0)
                { // avoid misleading user interaction stats from situations in which there are very small rounding errors
                    userInteractionStat.SumWeights = 0;
                    userInteractionStat.AvgAdjustmentPctWeighted = 0;
                }
                else
                {
                    userInteractionStat.SumWeights += sumWeightsDelta;
                    userInteractionStat.AvgAdjustmentPctWeighted = (userInteractionStat.SumWeights == 0) ? 0 : userInteractionStat.SumAdjustPctTimesWeight / userInteractionStat.SumWeights;
                }
                userInteractionStat.AvgAdjustmentPctWeighted = PMTrustCalculations.Constrain(userInteractionStat.AvgAdjustmentPctWeighted, PMAdjustmentFactor.MinimumRetrospectiveAdjustmentFactor, PMAdjustmentFactor.MaximumRetrospectiveAdjustmentFactor);
                //if (i == 1 && theUserInteraction.User.UserID == 43 && theUserInteraction.User1.UserID == 20)
                //{
                //    Debug.WriteLine("-------------------------------");
                //    manager.GetStat(i);
                //    Debug.WriteLine(String.Format("changeInIndividualUserInteractionAdjustmentFactor {0} * userRatingStat {1} * positiveOrNegative {2} = sumAdjustPctTimesWeightDelta {3}", changeInIndividualUserInteractionAdjustmentFactor, userRatingStat, positiveOrNegative, sumAdjustPctTimesWeightDelta));
                //    Debug.WriteLine(String.Format("OriginalSumAdjustPctTimesWeight {0} + sumAdjustPctTimesWeightDelta {1} = SumAdjustPctTimesWeight {2}", originalSumAdjustPctTimesWeight, sumAdjustPctTimesWeightDelta, userInteractionStat.SumAdjustPctTimesWeight));
                //    Debug.WriteLine(String.Format("SumAdjustPctTimesWeight {0} / userInteractionStat.SumWeights {1} = userInteractionStat.AvgAdjustmentPctWeighted {2}", userInteractionStat.SumAdjustPctTimesWeight, userInteractionStat.SumWeights, userInteractionStat.AvgAdjustmentPctWeighted));
                //    Debug.WriteLine("-------------------------------");
                //}
                //Trace.TraceInformation(String.Format("Stat {0}: {1}", i, theStat.SumWeights == 0 ? 0 : theStat.SumAdjustPctTimesWeight / theStat.SumWeights));
            }
            double originalWeightInCalculatingTrustTotal = theUserInteraction.WeightInCalculatingTrustTotal;
            if (theUserInteraction.NumTransactions == 0)
                theUserInteraction.WeightInCalculatingTrustTotal = 0;
            else
                theUserInteraction.WeightInCalculatingTrustTotal = PMTrustCalculations.GetUserInteractionWeightInCalculatingTrustTotal(noExtraWeightingStat, theUserInteraction);
            UpdateEarlierUserTrustTrackerStatsandEgalitarianTrustAfterUpdatingUserInteraction(theUserInteraction, originalAvgAdjustmentPct, originalSumWeights, originalWeightInCalculatingTrustTotal);
        }

        private static void UpdateEarlierUserTrustTrackerStatsandEgalitarianTrustAfterUpdatingUserInteraction(UserInteraction theUserInteraction, double[] originalAvgAdjustmentPctOrNullIfNoChange, double[] originalSumWeightsOrNullIfNoChange, double originalWeightInCalculatingTrustTotal)
        {
            bool noChangeToOriginalAvgAdjustmentPct = originalAvgAdjustmentPctOrNullIfNoChange == null;
            bool noChangeToOriginalSumWeights = originalSumWeightsOrNullIfNoChange == null;

            // Make the appropriate changes to the TrustTrackerStat and TrustTracker
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
            {
                UserInteractionStat theStat = theUserInteraction.UserInteractionStats.Single(x => x.StatNum == i);
                double originalAverageAdjustmentPct = noChangeToOriginalAvgAdjustmentPct ? theStat.AvgAdjustmentPctWeighted : originalAvgAdjustmentPctOrNullIfNoChange[i];
                TrustTrackerStat ttStat = theStat.TrustTrackerStat;
                double? originalSumWeightsThisStatOrNullIfNoChange = noChangeToOriginalSumWeights ? (double?)null : originalSumWeightsOrNullIfNoChange[i];
                AdjustTrustTrackerStatsTrustLevel(theStat, ttStat, originalWeightInCalculatingTrustTotal, theUserInteraction.WeightInCalculatingTrustTotal, originalAverageAdjustmentPct, originalSumWeightsThisStatOrNullIfNoChange);
                if (i == 0)
                {
                    TrustTracker tt = theStat.TrustTrackerStat.TrustTracker;
                    double originalEgalitarianTrustLevel = tt.EgalitarianTrustLevel;
                    double originalWeight = originalWeightInCalculatingTrustTotal > 0 ? 1.0F : 0.0F;
                    double newWeight = theUserInteraction.WeightInCalculatingTrustTotal > 0 ? 1.0F : 0.0F;
                    double previousContributionToEgalitarianTrustNumerator = originalAverageAdjustmentPct * originalWeight;
                    double newContributionToEgalitarianTrustNumerator = theStat.AvgAdjustmentPctWeighted * newWeight;
                    if (previousContributionToEgalitarianTrustNumerator != newContributionToEgalitarianTrustNumerator || theUserInteraction.WeightInCalculatingTrustTotal != originalWeightInCalculatingTrustTotal)
                    {
                        tt.Egalitarian_Num += newContributionToEgalitarianTrustNumerator - previousContributionToEgalitarianTrustNumerator;
                        tt.Egalitarian_Denom += newWeight - originalWeight;
                        tt.EgalitarianTrustLevel = tt.Egalitarian_Denom == 0 ? 1F : tt.Egalitarian_Num / tt.Egalitarian_Denom;
                        // Note: We don't calculate the EgalitarianTrustLevelOverride, but this will be used to determine other users' trusts if it is not null
                    }
                    tt.DeltaOverallTrustLevel = Math.Abs(tt.OverallTrustLevelAtLastReview - tt.OverallTrustLevel);
                    tt.OverallTrustLevel = PMTrustCalculations.GetOverallTrustLevelWithNewUserCredit(theStat.TrustTrackerStat, (int) Math.Round(tt.Egalitarian_Denom));
                    tt.SkepticalTrustLevel = theStat.TrustTrackerStat.TrustValue; // we keep track of the "real" skeptical trust value, but won't use it in deciding adjustment factors.
                    if (tt.EgalitarianTrustLevel != originalEgalitarianTrustLevel && tt.EgalitarianTrustLevelOverride == null)
                        tt.MustUpdateUserInteractionEgalitarianTrustLevel = true;
                }
            }
        }

        public static void AdjustTrustTrackerStatsTrustLevel(UserInteractionStat theStat, TrustTrackerStat ttStat, double originalWeightInCalculatingTrustTotal, double newWeightInCalculatingTrustTotal, double originalAverageAdjustmentPct, double? originalSumWeightsThisStatOrNullIfNoChange)
        {
            double previousContributionToTrustNumerator = originalAverageAdjustmentPct * originalWeightInCalculatingTrustTotal;
            double newContributionToTrustNumerator = theStat.AvgAdjustmentPctWeighted * theStat.UserInteraction.WeightInCalculatingTrustTotal;
            if (previousContributionToTrustNumerator != newContributionToTrustNumerator || newWeightInCalculatingTrustTotal != originalWeightInCalculatingTrustTotal)
            {
                double originalTrustValue = theStat.TrustTrackerStat.TrustValue;
                ttStat.Trust_Numer += newContributionToTrustNumerator - previousContributionToTrustNumerator;
                ttStat.Trust_Denom += newWeightInCalculatingTrustTotal - originalWeightInCalculatingTrustTotal;
                ttStat.TrustValue = (theStat.TrustTrackerStat.Trust_Denom == 0) ? 1F : ttStat.Trust_Numer / ttStat.Trust_Denom;

                //if (theUserInteraction != null && theUserInteraction.User.UserID == 43 && theUserInteraction.User1.UserID == 20 && i == 0)
                //{
                //    //Debug.WriteLine("Adjusting 43, " + theUserInteraction.User1.UserID + " based on previous stat " + i + " of " + previousContributionToTrustNumerator + "/" + originalWeightInCalculatingTrustTotal + "=" + originalTrustValue + " and new of " + newContributionToTrustNumerator + "/" + theUserInteraction.WeightInCalculatingTrustTotal + "=" + theStat.TrustTrackerStat.TrustValue);
                //}
                if (originalSumWeightsThisStatOrNullIfNoChange != null)
                    ttStat.SumUserInteractionStatWeights += theStat.SumWeights - (float)originalSumWeightsThisStatOrNullIfNoChange;
            }
        }


        public static void UpdateUserInteractionsAfterNewUserRatingIsEntered(IRaterooDataContext RaterooDB, UserInteraction oldUserInteraction, 
            UserInteraction newUserInteraction, UserRating originalUserRating, TrustTrackerStat[] originalUserTrustTrackerStats, 
            UserRating previousLatestUserRating, UserRating latestUserRating, DateTime whenOriginalMade, RatingGroupAttribute theRatingGroupAttribute, 
            RatingCharacteristic ratingCharacteristic, TrustTracker mostRecentUserTrustTracker)
        {
            bool keepTrackingThis = theRatingGroupAttribute.LongTermPointsWeight > 0 || 
                whenOriginalMade + TimeSpan.FromDays(theRatingGroupAttribute.MinimumDaysToTrackLongTerm) <= TestableDateTime.Now;
            if (keepTrackingThis)
            {
                if (oldUserInteraction != newUserInteraction || previousLatestUserRating != latestUserRating)
                {
                    AdjustUserInteraction(RaterooDB, oldUserInteraction, originalUserRating, originalUserTrustTrackerStats, previousLatestUserRating, ratingCharacteristic, true, mostRecentUserTrustTracker);
                    AdjustUserInteraction(RaterooDB, newUserInteraction, originalUserRating, originalUserTrustTrackerStats, latestUserRating, ratingCharacteristic, false, mostRecentUserTrustTracker);
                    originalUserRating.UserRating1 = latestUserRating;
                }
            }
        }



        internal class TrustTrackerUnitAndTrustTrackers
        {
#pragma warning disable 0649
            public TrustTrackerUnit TrustTrackerUnit;
            public IEnumerable<TrustTracker> TrustTrackers;
#pragma warning restore 0649
        }


        /// <summary>
        /// Although we only use the latest user interaction for the purposes of trust calculation,
        /// UserInteractions in the database still exist even when it is no longer the case that there are any user ratings
        /// where user A entered a user rating and user B has made the latest rating.  This task should remove all
        /// UserInteractions that are no longer be used for trust calcuation.  These UserInteractions should be the 
        /// same as those with NumTransactions == 0, because we rely upon methods elsewhere to adjust NumTransactions
        /// when a UserInteraction's LatestRatingUser becomes no longer the latest rating user.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        internal static bool DeleteZereodUserInteractions(IRaterooDataContext dataContext)
        {
            const int numToDoAtOnce = 1000;

            bool moreToDo = false;

            IQueryable<UserInteraction> zeroedUserInteractionsQuery = 
                dataContext.GetTable<UserInteraction>().Where(ui => ui.NumTransactions == 0);
            if (zeroedUserInteractionsQuery.Count() > numToDoAtOnce)
                moreToDo = true;

            UserInteraction[] zeroedUserInteractions = zeroedUserInteractionsQuery.Take(numToDoAtOnce).ToArray();

            // If we are using our home-made in-memory database, there is not on-delete-cascade behavior,
            // so we have delete any dependent records ourselves.  It's possible that we could add this
            // behavior to some kind of OnDelete event of the DataContext
            if (!dataContext.IsRealDatabase())
            {
                IEnumerable<UserInteractionStat> userInteractionStats = zeroedUserInteractions.SelectMany(ui => ui.UserInteractionStats);
                dataContext.GetTable<UserInteractionStat>().DeleteAllOnSubmit(userInteractionStats);
            }

            dataContext.GetTable<UserInteraction>().DeleteAllOnSubmit(zeroedUserInteractions);

            return moreToDo;
        }


    }
}
