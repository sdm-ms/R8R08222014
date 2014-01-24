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

        static int _NumBackgroundTasks = 5;
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
                    return UpdateUserRatingsThatAreStillMostRecentWhereTrustHasChanged; // DEBUG: Will change this to something where Rateroo assigns ratings on its own.
                case 3:
                    return UpdateSkepticalTrustThreshhold; // DEBUG: Won't be necessary any more
                case 4:
                    return UpdateSkepticalTrust; // DEBUG: Won't be necessary any more
                case 5:
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

        const float MinChangeToLatestUserEgalitarianTrustBeforeUpdatingWeightInCalculatingTrustTotal = 0.1F;

        internal static void CorrectLatestUserEgalitarianTrust(TrustTrackerAndUserInteraction[] items)
        {
            foreach (var item in items)
            {
                float egalitarianTrustLevelToUse = item.TrustTracker.EgalitarianTrustLevelOverride ?? item.TrustTracker.EgalitarianTrustLevel;
                item.UserInteraction.LatestUserEgalitarianTrust = egalitarianTrustLevelToUse;
                item.TrustTracker.MustUpdateUserInteractionEgalitarianTrustLevel = false;
                if (item.UserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate == null || Math.Abs(item.UserInteraction.LatestUserEgalitarianTrust - (float)item.UserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate) > MinChangeToLatestUserEgalitarianTrustBeforeUpdatingWeightInCalculatingTrustTotal)
                {
                    float originalWeightInCalculatingTrustTotal = item.UserInteraction.WeightInCalculatingTrustTotal;
                    UpdateWeightInCalculatingTrustTotal(item.UserInteraction, egalitarianTrustLevelToUse);
                    UpdateEarlierUserTrustTrackerStatsandEgalitarianTrustAfterUpdatingUserInteraction(item.UserInteraction, null, null, originalWeightInCalculatingTrustTotal);
                    item.UserInteraction.LatestUserEgalitarianTrustAtLastWeightUpdate = item.UserInteraction.LatestUserEgalitarianTrust;
                }
            }
        }

        public static void UpdateWeightInCalculatingTrustTotal(
            UserInteraction theUserInteraction,
            float newLatestUserEgalitarianTrust)
        {
            UserInteractionStat ratingMagnitudeStat = theUserInteraction.UserInteractionStats.Single(x => x.StatNum == 0);
            theUserInteraction.WeightInCalculatingTrustTotal = PMTrustCalculations.GetUserInteractionWeightInCalculatingTrustTotal(ratingMagnitudeStat, theUserInteraction);
        }


        internal class TrustTrackerAndUserRatings
        {
#pragma warning disable 0649
            public TrustTracker TrustTracker;
            public TrustTrackerStat[] TrustTrackerStats;
            public UserRating[] UserRatings;
            public IEnumerable<TrustTrackerForChoiceInField>[] TrustTrackerForChoiceInFields;
            public IEnumerable<ChoiceInField>[] ChoiceInFields;
            public Rating[] Ratings;
            public RatingCharacteristic[] RatingCharacteristics;
            public TblRow[] TblRows;
            public Tbl[] Tbls;
#pragma warning restore 0649
        }

        internal static bool UpdateUserRatingsThatAreStillMostRecentWhereTrustHasChanged(IRaterooDataContext RaterooDB)
        {
            if (TrustTrackerTrustEveryone.AllAdjustmentFactorsAre1ForTestingPurposes)
                return false; // no more work to do

            /* This is a pretty intense query because it looks up all the data relevant
             * to recalculating each pending user rating. But this is still a relatively finite amount of data, that doesn't depend on the number of users. */


            const int numToDoAtOnceEachQuery = 50;
            var query = from tt in RaterooDB.GetTable<TrustTracker>()
                        where tt.DeltaOverallTrustLevel > 0.05F || tt.MustUpdateIsTrustedOnNotSubsequentlyRated /* trust has changed a lot or we have a flag to respond to */
                        orderby tt.DeltaOverallTrustLevel descending
                        let userRatings = tt.User.UserRatings.Where(ur =>
                                !ur.SubsequentlyRated &&
                                !ur.ShortTermResolutionReflected &&
                                ur.RatingPhaseStatus.ShortTermResolutionValue == null &&
                                !ur.LongTermResolutionReflected &&
                                tt.TrustTrackerUnitID == ur.TrustTrackerUnitID)
                        let trustTrackerForChoiceInFields = userRatings.Select(ur =>
                            ur.TrustTrackerForChoiceInFieldsUserRatingLinks.Select(t => t.TrustTrackerForChoiceInField))
                        let choiceInFields = trustTrackerForChoiceInFields.Select(ttfcif => ttfcif.Select(t => t.ChoiceInField))
                        let ratings = userRatings.Select(ur => ur.Rating)
                        let ratingCharacteristics = ratings.Select(r => r.RatingGroup.RatingGroupAttribute.RatingCharacteristic)
                        let tblRows = ratings.Select(r => r.RatingGroup.TblRow)
                        let tbls = tblRows.Select(tr => tr.Tbl)
                        select new TrustTrackerAndUserRatings
                        {
                            TrustTracker = tt,
                            TrustTrackerStats = tt.TrustTrackerStats.ToArray(),
                            UserRatings = userRatings.ToArray(),
                            TrustTrackerForChoiceInFields = trustTrackerForChoiceInFields.ToArray(),
                            ChoiceInFields = choiceInFields.ToArray(),
                            Ratings = ratings.ToArray(),
                            RatingCharacteristics = ratingCharacteristics.ToArray(),
                            TblRows = tblRows.ToArray(),
                            Tbls = tbls.ToArray()
                        };

            var data = query.Take(numToDoAtOnceEachQuery).ToArray();

            UpdateUserRatingsThatAreStillMostRecentBasedOnTrustTrackers(RaterooDB, data);

            return data.Count() == numToDoAtOnceEachQuery;
        }

        internal static void UpdateUserRatingsThatAreStillMostRecentBasedOnTrustTrackers(IRaterooDataContext dataContext, TrustTrackerAndUserRatings[] trustTrackersAndUserRatings)
        {
            foreach (var trustTrackerAndUserRatings in trustTrackersAndUserRatings)
            {
                trustTrackerAndUserRatings.TrustTracker.MustUpdateIsTrustedOnNotSubsequentlyRated = false; // will do below
                int userRatingsCount = trustTrackerAndUserRatings.UserRatings.Count();
                for (int i = 0; i <  userRatingsCount; i++)
                {
                    UserRating userRating = trustTrackerAndUserRatings.UserRatings[i];
                    Rating rating = trustTrackerAndUserRatings.Ratings[i];
                    TblRow tblRow = trustTrackerAndUserRatings.TblRows[i];
                    Tbl tbl = trustTrackerAndUserRatings.Tbls[i];
                    RatingCharacteristic ratingCharacteristic = trustTrackerAndUserRatings.RatingCharacteristics[i];

                    TrustTrackerStatManager manager = new TrustTrackerStatManager(userRating, ratingCharacteristic,
                        trustTrackerAndUserRatings.TrustTrackerStats);
                    userRating.NewUserRating = manager.GetNewUserRatingValueToUse(userRating);

                    bool rowRequiresUpdate = false;
                    if (userRating.NewUserRating != null && rating.CurrentValue != userRating.NewUserRating)
                    {
                        rating.CurrentValue = userRating.NewUserRating;
                        rowRequiresUpdate = true;
                        rating.LastModifiedResolutionTimeOrCurrentValue = TestableDateTime.Now; // trigger recalculation of points for all previous user ratings for the same rating
                    }
                    bool previousIsTrusted = userRating.IsTrusted;
                    userRating.IsTrusted =
                        // if there is a previous rating, then if we trust you even a little bit, we won't give you an asterisk, because we will already be adjusting your user rating based on the degree to which we trust you
                        (userRating.PreviousDisplayedRating != null && trustTrackerAndUserRatings.TrustTracker.SkepticalTrustLevel > 0) ||
                        // but if this is the very first rating, then we don't know what the baseline is, so we need to indicate that this is untrusted unless you are very trustworthy
                        (userRating.PreviousDisplayedRating == null && trustTrackerAndUserRatings.TrustTracker.SkepticalTrustLevel > MinSkepticalTrustNeededForTrustedOnInitialRating);    
                    if (userRating.NewUserRating != null && previousIsTrusted != userRating.IsTrusted)
                    {
                        if (userRating.IsTrusted)
                        {
                            rating.LastTrustedValue = userRating.NewUserRating;
                        }
                        else
                        {
                            if (userRating.PreviousDisplayedRating == null)
                                rating.LastTrustedValue = null;
                            else
                                rating.LastTrustedValue = userRating.PreviousRatingOrVirtualRating;
                        }
                        rowRequiresUpdate = true;
                        rating.LastModifiedResolutionTimeOrCurrentValue = TestableDateTime.Now; // trigger recalculation of points for all previous user ratings
                    }
                    if (rowRequiresUpdate)
                        SQLFastAccess.IdentifyRowRequiringUpdate(dataContext, tbl, tblRow.TblRowID, true, false);
                }
                trustTrackerAndUserRatings.TrustTracker.LastOverallTrustLevel = trustTrackerAndUserRatings.TrustTracker.OverallTrustLevel;
                trustTrackerAndUserRatings.TrustTracker.DeltaOverallTrustLevel = 0;
            }
        }



        internal static TrustTracker AddTrustTracker(IRaterooDataContext RaterooDB, User theUser, TrustTrackerUnit theTrustTrackerUnit)
        {
            TrustTracker theTrustTracker = new TrustTracker
            {
                TrustTrackerUnit = theTrustTrackerUnit,
                User = theUser,
                SkepticalTrustLevel = theTrustTrackerUnit.SkepticalTrustThreshhold == 0 ? 1.0F : 0F, // new users on a new table will be trusted
                OverallTrustLevel = 1,
                EgalitarianTrustLevel = 1,
                EgalitarianTrustLevelOverride = null,
                LastOverallTrustLevel = 1,
                DeltaOverallTrustLevel = 0,
                SumUserInteractionWeights = 0,
                MustUpdateIsTrustedOnNotSubsequentlyRated = false
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
            if (originalUserRating == null || latestUserRating == null || latestUserRating.NewUserRating == null || ratingCharacteristic == null)
                return;
            if (originalUserRating.User == latestUserRating.User)
                return;

            if (theUserInteraction == null)
            { // it could have been created earlier in this background process
                theUserInteraction = RaterooDB.RegisteredToBeInserted.OfType<UserInteraction>().SingleOrDefault(ui => ui.User == originalUserRating.User && ui.User1 == latestUserRating.User && ui.TrustTrackerUnit == latestUserRating.TrustTrackerUnit);
            }

            if (theUserInteraction == null)
            {
                theUserInteraction = new UserInteraction 
                { 
                    User = originalUserRating.User, 
                    User1 = latestUserRating.User, 
                    TrustTrackerUnit = latestUserRating.TrustTrackerUnit, 
                    LatestUserEgalitarianTrust = mostRecentUserTrustTracker.EgalitarianTrustLevel,
                    LatestUserEgalitarianTrustAtLastWeightUpdate = mostRecentUserTrustTracker.EgalitarianTrustLevel
                };
                RaterooDB.GetTable<UserInteraction>().InsertOnSubmit(theUserInteraction);
                RaterooDB.RegisterObjectToBeInserted(theUserInteraction);
                AddUserInteractionStatsForUserInteraction(RaterooDB, theUserInteraction, originalUserTrustTrackerStats);
            }

            // We don't want any one instance to have a huge effect on the adjustment percentage, so we need to constrain the adjustment factor to a range of -1.25, 1.25
            float adjustFactor = PMAdjustmentFactor.CalculateAdjustmentFactor(
                laterValue: latestUserRating.NewUserRating.Value, 
                enteredValue: originalUserRating.EnteredUserRating, 
                basisValue: originalUserRating.PreviousRatingOrVirtualRating, 
                logBase: latestUserRating.LogarithmicBase,
                constrainForRetrospectiveAssessment: true);
            UpdateUserInteractionStats(theUserInteraction, originalUserRating, originalUserTrustTrackerStats, ratingCharacteristic, adjustFactor, subtractFromUserInteraction);
        }

        public static void UpdateUserInteractionStats(
            UserInteraction theUserInteraction, 
            UserRating originalUserRating, 
            TrustTrackerStat[] trustTrackerStats, 
            RatingCharacteristic ratingCharacteristic, 
            float changeInIndividualUserInteractionAdjustmentFactor, 
            bool subtractFromUserInteraction)
        {
            //Trace.TraceInformation("UpdateUserInteractionStats original user: " + theUserInteraction.User.UserID + " later user: " + theUserInteraction.User1.UserID + " Original user rating: " + originalUserRating.EnteredUserRating + " adjust% " + adjustPercentage + " subtract: " + subtractFromUserInteraction.ToString());
            TrustTrackerStatManager manager = new TrustTrackerStatManager(originalUserRating, ratingCharacteristic, trustTrackerStats);
            float positiveOrNegative = subtractFromUserInteraction ? -1F : 1F;
            float[] originalAvgAdjustmentPct = new float[TrustTrackerStatManager.NumStats], originalSumWeights = new float[TrustTrackerStatManager.NumStats];
            UserInteractionStat noExtraWeightingStat = null;
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
            {
                UserInteractionStat theStat = theUserInteraction.UserInteractionStats.Single(x => x.StatNum == i);
                if (i == 0)
                    noExtraWeightingStat = theStat;
                float theStatFloat = manager.GetStat(i);
                originalAvgAdjustmentPct[i] = theStat.AvgAdjustmentPctWeighted;
                originalSumWeights[i] = theStat.SumWeights;
                theStat.SumAdjustPctTimesWeight += theStatFloat * changeInIndividualUserInteractionAdjustmentFactor * positiveOrNegative;
                theStat.SumWeights += theStatFloat * positiveOrNegative;
                theStat.AvgAdjustmentPctWeighted = (theStat.SumWeights == 0) ? 0 : theStat.SumAdjustPctTimesWeight / theStat.SumWeights;
                //Trace.TraceInformation(String.Format("Stat {0}: {1}", i, theStat.SumWeights == 0 ? 0 : theStat.SumAdjustPctTimesWeight / theStat.SumWeights));
            }
            theUserInteraction.NumTransactions += 1 * (int)positiveOrNegative;
            float originalWeightInCalculatingTrustTotal = theUserInteraction.WeightInCalculatingTrustTotal;
            if (theUserInteraction.NumTransactions == 0)
                theUserInteraction.WeightInCalculatingTrustTotal = 0;
            else
                theUserInteraction.WeightInCalculatingTrustTotal = PMTrustCalculations.GetUserInteractionWeightInCalculatingTrustTotal(noExtraWeightingStat, theUserInteraction);
            UpdateEarlierUserTrustTrackerStatsandEgalitarianTrustAfterUpdatingUserInteraction(theUserInteraction, originalAvgAdjustmentPct, originalSumWeights, originalWeightInCalculatingTrustTotal);
        }

        private static void UpdateEarlierUserTrustTrackerStatsandEgalitarianTrustAfterUpdatingUserInteraction(UserInteraction theUserInteraction, float[] originalAvgAdjustmentPctOrNullIfNoChange, float[] originalSumWeightsOrNullIfNoChange, float originalWeightInCalculatingTrustTotal)
        {
            bool noChangeToOriginalAvgAdjustmentPct = originalAvgAdjustmentPctOrNullIfNoChange == null;
            bool noChangeToOriginalSumWeights = originalSumWeightsOrNullIfNoChange == null;

            // Make the appropriate changes to the TrustTrackerStat and TrustTracker
            for (int i = 0; i < TrustTrackerStatManager.NumStats; i++)
            {
                UserInteractionStat theStat = theUserInteraction.UserInteractionStats.Single(x => x.StatNum == i);
                float originalAverageAdjustmentPct = noChangeToOriginalAvgAdjustmentPct ? theStat.AvgAdjustmentPctWeighted : originalAvgAdjustmentPctOrNullIfNoChange[i];
                float previousContributionToTrustNumerator = originalAverageAdjustmentPct * originalWeightInCalculatingTrustTotal;
                float newContributionToTrustNumerator = theStat.AvgAdjustmentPctWeighted * theStat.UserInteraction.WeightInCalculatingTrustTotal;
                if (previousContributionToTrustNumerator != newContributionToTrustNumerator || theUserInteraction.WeightInCalculatingTrustTotal != originalWeightInCalculatingTrustTotal)
                {
                    theStat.TrustTrackerStat.Trust_Numer += newContributionToTrustNumerator - previousContributionToTrustNumerator;
                    theStat.TrustTrackerStat.Trust_Denom += theUserInteraction.WeightInCalculatingTrustTotal - originalWeightInCalculatingTrustTotal;
                    theStat.TrustTrackerStat.TrustValue = (theStat.TrustTrackerStat.Trust_Denom == 0) ? 1F : theStat.TrustTrackerStat.Trust_Numer / theStat.TrustTrackerStat.Trust_Denom;
                    if (!noChangeToOriginalSumWeights)
                        theStat.TrustTrackerStat.SumUserInteractionStatWeights += theStat.SumWeights - originalSumWeightsOrNullIfNoChange[i];
                }
                if (i == 0)
                {
                    TrustTracker tt = theStat.TrustTrackerStat.TrustTracker;
                    float originalTrustLevel = tt.OverallTrustLevel;
                    tt.OverallTrustLevel = theStat.TrustTrackerStat.TrustValue;
                    tt.DeltaOverallTrustLevel = Math.Abs(originalTrustLevel - tt.OverallTrustLevel);
                    float originalEgalitarianTrustLevel = tt.EgalitarianTrustLevel;
                    float originalWeight = originalWeightInCalculatingTrustTotal > 0 ? 1.0F : 0.0F;
                    float newWeight = theUserInteraction.WeightInCalculatingTrustTotal > 0 ? 1.0F : 0.0F;
                    float previousContributionToEgalitarianTrustNumerator = originalAverageAdjustmentPct * originalWeight;
                    float newContributionToEgalitarianTrustNumerator = theStat.AvgAdjustmentPctWeighted * newWeight;
                    if (previousContributionToEgalitarianTrustNumerator != newContributionToEgalitarianTrustNumerator || theUserInteraction.WeightInCalculatingTrustTotal != originalWeightInCalculatingTrustTotal)
                    {
                        tt.Egalitarian_Num += newContributionToEgalitarianTrustNumerator - previousContributionToEgalitarianTrustNumerator;
                        tt.Egalitarian_Denom += newWeight - originalWeight;
                        tt.EgalitarianTrustLevel = tt.Egalitarian_Denom == 0 ? 1F : tt.Egalitarian_Num / tt.Egalitarian_Denom;
                        // Note: We don't calculate the EgalitarianTrustLevelOverride, but this will be used to determine other users' trusts if it is not null
                    }
                    if (tt.EgalitarianTrustLevel != originalEgalitarianTrustLevel && tt.EgalitarianTrustLevelOverride == null)
                        tt.MustUpdateUserInteractionEgalitarianTrustLevel = true;
                }
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

        // Skeptical trust: A user's overall trust level depends on all the user's UserInteractionStats. However, where a user is relatively new, we might not want to give full weight to that overall trust level; that is, we want to put an asterisk next to the user's user ratings when the data producing the overall trust level is relatively unreliable. The user in that case can still receive points, but the asterisk provides an indication and an incentive for other users to validate whether that user rating is accurate. The question is how much data we need before we trust a user enough to set the skeptical trust equal to the overall trust level, instead of equal to zero. When there are very few users, we need to trust more, and so we have a very low skeptical trust threshold, but as the user base grows, we demand more user ratings that have been rerated by distinct users, and so the skeptical trust threshold increases, but never to more than 25.

        internal static short GetSkepticalTrustThreshhold(IEnumerable<TrustTracker> TrustTrackers)
        {
            int skepticallyTrusted = TrustTrackers.Count(x => x.SkepticalTrustLevel > 0);
            int numToReturn = (int)Math.Sqrt(skepticallyTrusted) - 3;
            if (numToReturn < 0)
                numToReturn = 0;
            else if (numToReturn > 25)
                numToReturn = 25;
            return (short) numToReturn;
        }

        internal class TrustTrackerUnitAndTrustTrackers
        {
#pragma warning disable 0649
            public TrustTrackerUnit TrustTrackerUnit;
            public IEnumerable<TrustTracker> TrustTrackers;
#pragma warning restore 0649
        }

        static bool skepticalTrustThreshholdUpdateComplete = true;
        static int numCompletedSkepticalTrustThreshhold = 0;
        static DateTime? lastUpdateOfSkepticalTrustThreshhold;
        internal static bool UpdateSkepticalTrustThreshhold(IRaterooDataContext RaterooDB)
        {
            if (skepticalTrustThreshholdUpdateComplete && (lastUpdateOfSkepticalTrustThreshhold == null || ((DateTime)lastUpdateOfSkepticalTrustThreshhold) + TimeSpan.FromMinutes(32) < TestableDateTime.Now))
            {
                skepticalTrustThreshholdUpdateComplete = false;
                numCompletedSkepticalTrustThreshhold = 0;
            }

            const int numToDoAtOnceEachQuery = 100;
            
            bool moreToDoSkepticalTrustThreshholdUpdate = !skepticalTrustThreshholdUpdateComplete;
            if (moreToDoSkepticalTrustThreshholdUpdate)
            {
                var query =
                        from x in RaterooDB.GetTable<TrustTrackerUnit>()
                        select new TrustTrackerUnitAndTrustTrackers { TrustTrackerUnit = x, TrustTrackers = x.TrustTrackers }
                 ;
                var data = query.Skip(numCompletedSkepticalTrustThreshhold).Take(numToDoAtOnceEachQuery).ToArray();
                foreach (var datum in data)
                {
                    short skepticalTrustThreshhold = GetSkepticalTrustThreshhold(datum.TrustTrackers);
                    datum.TrustTrackerUnit.SkepticalTrustThreshhold = skepticalTrustThreshhold;
                }
                skepticalTrustThreshholdUpdateComplete = data.Count() != numToDoAtOnceEachQuery;
                if (skepticalTrustThreshholdUpdateComplete)
                {
                    moreToDoSkepticalTrustThreshholdUpdate = false;
                    lastUpdateOfSkepticalTrustThreshhold = TestableDateTime.Now;
                }
                else
                    numCompletedSkepticalTrustThreshhold += numToDoAtOnceEachQuery;
            }

            return moreToDoSkepticalTrustThreshholdUpdate;

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

        static bool updateSkepticalTrustComplete = true;
        static int numCompletedSkepticalTrust = 0;
        static DateTime? lastUpdateOfSkepticalTrust;
        internal static bool UpdateSkepticalTrust(IRaterooDataContext RaterooDB)
        {
            if (updateSkepticalTrustComplete)
            {
                updateSkepticalTrustComplete = 
                    (lastUpdateOfSkepticalTrust == null || 
                    lastUpdateOfSkepticalTrust.Value + TimeSpan.FromMinutes(10) < TestableDateTime.Now);
                numCompletedSkepticalTrust = 0;
            }

            const int numToDoAtOnceEachQuery = 1000;
            bool moreToDo = !updateSkepticalTrustComplete;

            if (moreToDo)
            {
                var query = from tt in RaterooDB.GetTable<TrustTracker>()
                            where (
                                // skeptical trust not yet at trust level -- we'll see if that's changed
                                (tt.SkepticalTrustLevel != tt.OverallTrustLevel) ||
                                // the threshhold has changed, so we had better check things even where SkepticalTrustLevel == TrustLevel, because we could demote SkepticalTrust back to zero.
                                (tt.TrustTrackerUnit.LastSkepticalTrustThreshhold != tt.TrustTrackerUnit.SkepticalTrustThreshhold) 
                                )
                            let numUserInteractionsWithTrustedLatestUsers =
                                tt.User.UserInteractions.Count(y => y.TrustTrackerUnit == tt.TrustTrackerUnit && y.LatestUserEgalitarianTrust > 0)
                            select new { TrustTrackerUnit = tt.TrustTrackerUnit, TrustTracker = tt, NumUserInteractionsWithTrustedLatestUsers = numUserInteractionsWithTrustedLatestUsers };
                var data = query.Skip(numCompletedSkepticalTrust).Take(numToDoAtOnceEachQuery).ToArray();
                foreach (var datum in data)
                {
                    float previousSkepticalTrustLevel = datum.TrustTracker.SkepticalTrustLevel;
                    if (datum.NumUserInteractionsWithTrustedLatestUsers >= datum.TrustTrackerUnit.SkepticalTrustThreshhold)
                        datum.TrustTracker.SkepticalTrustLevel = datum.TrustTracker.OverallTrustLevel; // could be negative
                    else
                        datum.TrustTracker.SkepticalTrustLevel = 0;
                    datum.TrustTrackerUnit.LastSkepticalTrustThreshhold = datum.TrustTrackerUnit.SkepticalTrustThreshhold;
                    if ((previousSkepticalTrustLevel > MinSkepticalTrustNeededForTrustedOnInitialRating != datum.TrustTracker.SkepticalTrustLevel > MinSkepticalTrustNeededForTrustedOnInitialRating) || 
                        (previousSkepticalTrustLevel > 0 != datum.TrustTracker.SkepticalTrustLevel > 0))
                        // we've changed the side of the threshold we're on, so we must update userratings and ratings where there have been no subsequent ratings, i.e. unchallenged user ratings
                        datum.TrustTracker.MustUpdateIsTrustedOnNotSubsequentlyRated = true; 
                }

                updateSkepticalTrustComplete = data.Count() != numToDoAtOnceEachQuery;
                if (updateSkepticalTrustComplete)
                {
                    lastUpdateOfSkepticalTrust = TestableDateTime.Now;
                    moreToDo = false;
                }
                else
                    numCompletedSkepticalTrust += numToDoAtOnceEachQuery;

            }
            return moreToDo;
        }

    }
}
