﻿using System;
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
using System.Diagnostics;
////using PredRatings;
using MoreStrings;

using System.Threading;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;

namespace ClassLibrary1.Model
{
    public partial class UserRating : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public bool Resolved { get { return this.LongTermResolutionReflected; } }

        public bool ResolvedShortTerm { get { return Resolved || this.ShortTermResolutionReflected; } }

        public decimal? ShortTermResolutionValueOrLastTrustedValueIfNotResolved
        {
            get 
            {
                if (ResolvedShortTerm || Resolved)
                    return ShortTermResolutionValue;
                return this.Rating.LastTrustedValue;
            }
        }

        public decimal? ShortTermResolutionValue
        {
            get
            {
                if (!ResolvedShortTerm)
                    return null;
                if (!Resolved)
                    return this.RatingPhaseStatus.ShortTermResolutionValue;
                if (this.Rating.RatingGroup.ResolutionTime < this.RatingPhaseStatus.RatingGroupPhaseStatus.ShortTermResolveTime)
                    return LongTermResolutionValue;
                else
                    return this.RatingPhaseStatus.ShortTermResolutionValue;
            }
        }

        public decimal? LongTermResolutionValueOrLastTrustedValueIfNotResolved
        {
            get
            {
                if (Resolved && this.Rating.RatingGroup.ResolutionTime != null && this.Rating.RatingGroup.ResolutionTime < this.UserRatingGroup.WhenMade)
                    return null;
                return this.Rating.LastTrustedValue;
            }
        }

        public decimal? LongTermResolutionValue
        {
            get
            {
                if (!Resolved)
                    return null;
                if (this.Rating.RatingGroup.ResolutionTime < this.UserRatingGroup.WhenMade)
                    return null;
                return this.Rating.LastTrustedValue;
            }
        }

        public decimal PointsEarnedShortTerm { get { return GetPointsOrMaxLoss(true, false, false, false, true); } }

        public decimal PointsEarnedLongTerm { get { return GetPointsOrMaxLoss(false, true, false, false, true); } }

        public decimal PointsEarned { get { return GetPointsOrMaxLoss(true, true, false, false, true); } }

        public decimal PendingPointsShortTerm { get { return GetPointsOrMaxLoss(true, false, false, true, false); } }

        public decimal PendingPointsLongTerm { get { return GetPointsOrMaxLoss(false, true, false, true, false); } }

        public decimal PendingPoints { get { return GetPointsOrMaxLoss(true, true, false, true, false); } }

        public decimal PendingOrEarnedPointsShortTerm { get { return GetPointsOrMaxLoss(true, false, false, true, true); } }

        public decimal PendingOrEarnedPointsLongTerm { get { return GetPointsOrMaxLoss(false, true, false, true, true); } }

        public decimal PendingOrEarnedPoints { get { return GetPointsOrMaxLoss(true, true, false, true, true); } }

        public decimal NotYetPendingPointsShortTerm { get { return GetPointsOrMaxLoss(true, false, true, false, false); } }

        public decimal NotYetPendingPointsLongTerm { get { return GetPointsOrMaxLoss(false, true, true, false, false); } }

        public decimal NotYetPendingPoints { get { return GetPointsOrMaxLoss(true, true, true, false, false); } }

        public decimal PendingMaxLossShortTerm { get { return GetPointsOrMaxLoss(true, false, false, true, false, true); } }

        public decimal PendingMaxLossLongTerm { get { return GetPointsOrMaxLoss(false, true, false, true, false, true); } }

        public decimal PendingMaxLoss { get { return GetPointsOrMaxLoss(true, true, false, true, false, true); } }

        public decimal NotYetPendingMaxLossShortTerm { get { return GetPointsOrMaxLoss(true, false, true, false, false, true); } }

        public decimal NotYetPendingMaxLossLongTerm { get { return GetPointsOrMaxLoss(false, true, true, false, false, true); } }

        public decimal NotYetPendingMaxLoss { get { return GetPointsOrMaxLoss(true, true, true, false, false, true); } }
        public decimal PotentialMaxLossShortTerm { get { return GetPointsOrMaxLoss(true, false, true, true, false, true); } }

        public decimal PotentialMaxLossLongTerm { get { return GetPointsOrMaxLoss(false, true, true, true, false, true); } }

        public decimal PotentialMaxLoss { get { return GetPointsOrMaxLoss(true, true, true, true, false, true); } }

        public decimal MaxLossShortTerm { get { return MaxLoss * (1 - LongTermPointsWeight); } }

        public decimal MaxLossLongTerm { get { return MaxLoss * LongTermPointsWeight; } }

        public decimal PointsOrPendingPointsLongTermUnweighted { get { if (!PointsHaveBecomePending) return 0; return PotentialPointsLongTermUnweighted; } }

        public decimal GetPointsOrMaxLoss(bool includeShortTerm, bool includeLongTerm, bool includeNotYetPending, bool includePending, bool includeEarned, bool getMaxLoss = false)
        {
            decimal total = 0;
            if (includeShortTerm && (
                (includeNotYetPending && !this.PointsHaveBecomePending && !this.ResolvedShortTerm) ||
                (includePending && this.PointsHaveBecomePending && !this.ResolvedShortTerm) ||
                (includeEarned && this.ResolvedShortTerm)
                ))
            {
                if (getMaxLoss)
                    total += this.MaxLossShortTerm;
                else
                    total += this.PotentialPointsShortTerm;
            }

            if (includeLongTerm && (
                (includeNotYetPending && !this.PointsHaveBecomePending && !this.ResolvedShortTerm) ||
                (includePending && this.PointsHaveBecomePending && !this.Resolved) ||
                (includeEarned && this.Resolved)
                ))
            {
                if (getMaxLoss)
                    total += this.MaxLossLongTerm;
                else
                    total += this.PotentialPointsLongTerm;
            }
            return total;
        }
    }

    public enum UserRatingUpdatingReason
    {
        pendingPointsRecalculate,
        shortTermExpiration,
        resolution,
        undoResolution
    }

    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {


        // Returns true if a rating group is resolved (or set to be resolved), regardless
        // of whether the predictions have been processed. 
        public bool RatingGroupIsResolved(RatingGroup theTopRatingGroup)
        {
            RatingGroupResolution activeResolution = theTopRatingGroup.RatingGroupResolutions.Where(x => x.RatingGroup == theTopRatingGroup).OrderByDescending(y => y.ExecutionTime).ThenByDescending(y => y.RatingGroupResolutionID).FirstOrDefault();
            if (activeResolution == null)
                return false;
            return !activeResolution.CancelPreviousResolutions;
        }

        public bool IdleTaskImplementResolutions()
        {
            const int maxAtOnce = 10;
            var resolutions = (from x in (DataContext.GetTable<RatingGroupResolution>()
                .Where(x => x.Status == (int)StatusOfObject.Proposed)
                .OrderBy(x => x.ExecutionTime).ThenBy(x => x.RatingGroupResolutionID)
                .Take(maxAtOnce))
                let RatingResolution = x
                let Ratings = x.RatingGroup.Ratings2
                let UserRatings = Ratings.SelectMany(y => y.UserRatings)
                let Users = UserRatings.Select(y => y.User).Distinct()
                let PointsTotals = Users.SelectMany(y => y.PointsTotals.Where(z => z.PointsManager == x.RatingGroup.TblRow.Tbl.PointsManager)).Distinct()
                select new 
                    {
                        RatingResolution = RatingResolution,
                        Ratings = Ratings.ToList(),
                        //PreviousRatingResolution = Ratings.First().RatingGroup2.RatingGroupResolutions
                        //    .Where(rgr => rgr.ExecutionTime < x.ExecutionTime && rgr.Status == (int) StatusOfObject.Active)
                        //    .OrderByDescending(rgr => rgr.ExecutionTime)
                        //    .ThenByDescending(rgr => rgr.RatingGroupResolutionID)
                        //    .FirstOrDefault(),
                        UserRatings = UserRatings.ToList(),
                        Users = Users.ToList(),
                        PointsTotals = PointsTotals.ToList()
                    })
                .ToList();
            DateTime currentTime = TestableDateTime.Now;
            foreach (var resolution in resolutions)
            {
                /* ratings must be up to date BEFORE the resolution occurs. Otherwise the UpdatePointsForUserRating may think there has been no change */
                /* in the rare event that there are multiple simultaneous resolutions */
                foreach (var z in resolution.UserRatings)
                    UpdatePointsForUserRating(z, resolution.PointsTotals.Single(w => w.User == z.User), currentTime);
                var ratingsWithlastUserRatings = resolution.RatingResolution.RatingGroup.Ratings2.Select(x => new { 
                    Rating = x, 
                    ReferenceUserRating = x.UserRatings
                        .Where(y => y.UserRatingGroup.WhenMade < resolution.RatingResolution.EffectiveTime) // DEBUG added
                        .OrderByDescending(y => y.UserRatingGroup.WhenMade)
                        .FirstOrDefault(),
                    ReferenceLastTrustedUserRating = x.UserRatings
                        .Where(y => y.IsTrusted && y.UserRatingGroup.WhenMade < resolution.RatingResolution.EffectiveTime) // DEBUG added
                        .OrderByDescending(y => y.UserRatingGroup.WhenMade)
                        .FirstOrDefault()
                });
                foreach (var z in ratingsWithlastUserRatings)
                {
                    z.Rating.CurrentValue = (resolution.RatingResolution.ResolveByUnwinding || z.ReferenceUserRating == null) ? null : z.ReferenceUserRating.NewUserRating;
                    //Trace.TraceInformation("3Setting rating to " + z.Rating.CurrentValue);
                    if (resolution.RatingResolution.CancelPreviousResolutions)
                    {
                        if (resolution.RatingResolution.ResolveByUnwinding || z.ReferenceLastTrustedUserRating == null)
                            z.Rating.LastTrustedValue = null;
                        else
                            z.Rating.LastTrustedValue = z.ReferenceLastTrustedUserRating.NewUserRating;
                    }
                    else
                        z.Rating.LastTrustedValue = z.Rating.CurrentValue;
                    z.Rating.RatingGroup.ResolutionTime = resolution.RatingResolution.CancelPreviousResolutions ? (DateTime?) null : resolution.RatingResolution.EffectiveTime;
                    z.Rating.LastModifiedResolutionTimeOrCurrentValue = currentTime;
                }
                /* and we'll update the ratings again to reflect this resolution */
                foreach (var z in resolution.UserRatings)
                    UpdatePointsForUserRating(z, resolution.PointsTotals.Single(w => w.User == z.User), currentTime);
                // mark this as active so we know that it's been processed
                resolution.RatingResolution.Status = (int)StatusOfObject.Active;
                // apply rewards for ratings being resolved
                Rating firstRating = resolution.Ratings.FirstOrDefault();
                if (firstRating != null)
                    ApplyRewardsForRatingBeingResolved(firstRating.RatingGroup2, resolution.RatingResolution.CancelPreviousResolutions);
            }
            return resolutions.Count() == maxAtOnce;
        }


        public bool IdleTaskShortTermResolve()
        {
            DateTime currentTime = TestableDateTime.Now;
            const int maxToTake = 100;
            var shortTermExpireds = DataContext.GetTable<RatingPhaseStatus>().Where(x => x.ShortTermResolutionValue == null && currentTime > x.RatingGroupPhaseStatus.ShortTermResolveTime && x.Rating.LastTrustedValue != null).Take(maxToTake).ToList();
            foreach (var ratingPhase in shortTermExpireds)
                ratingPhase.ShortTermResolutionValue = ratingPhase.Rating.LastTrustedValue;
            return shortTermExpireds.Count() == maxToTake;
        }

        public bool IdleTaskUpdatePoints()
        {
            DateTime currentTime = TestableDateTime.Now;
            const int maxToTake = 400;

            DataContext.LoadStatsWithTrustTrackersAndUserInteractions(); // we can't seem to do this in a projection without getting a linq to sql unable to translate error, so we're going to use loading options.

            var userRatingInfoQuery = from x in DataContext.GetTable<UserRating>()
                                      where
                                          (!x.PointsHaveBecomePending && x.WhenPointsBecomePending < currentTime)
                                          || (!x.ShortTermResolutionReflected && x.RatingPhaseStatus.ShortTermResolutionValue != null)
                                          || (x.LastModifiedTime < x.Rating.LastModifiedResolutionTimeOrCurrentValue)
                                          || (x.ForceRecalculate)
                                        let trustTrackerUnit = x.TrustTrackerUnit
                                        let mostRecentUserRatingRecordedInUserRating = x.UserRating1 // this previously was the latest user rating
                                        let mostRecentUserRatingRecordedInRating = x.Rating.UserRating // this now is the latest user rating
                                        let user = x.User
                                      let currentlyRecordedUserInteraction = x.User.UserInteractions.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit && mostRecentUserRatingRecordedInUserRating != null && y.User == user && y.User1 == mostRecentUserRatingRecordedInUserRating.User)
                                      let replacementUserInteraction = x.User.UserInteractions.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit && y.User == user && y.User1 == mostRecentUserRatingRecordedInRating.User)
                                      let originalUserTrustTracker = x.User.TrustTrackers.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit)
                                      let mostRecentUserTrustTracker = mostRecentUserRatingRecordedInRating.User.TrustTrackers.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit)
                                      select new
                                      {
                                          UserRating = x,
                                          UserRatingGroup = x.UserRatingGroup,
                                          Rating = x.Rating,
                                          MostRecentUserRatingInUserRating = x.UserRating1,
                                          MostRecentUserRatingInRating = x.Rating.UserRating,
                                          RatingGroup = x.UserRatingGroup.RatingGroup,
                                          RatingGroupAttribute = x.UserRatingGroup.RatingGroup.RatingGroupAttribute,
                                          RatingCharacteristic = x.UserRatingGroup.RatingGroup.RatingGroupAttribute.RatingCharacteristic,
                                          PointsTotal = x.User.PointsTotals.SingleOrDefault(y => y.PointsManager == x.Rating.RatingGroup.TblRow.Tbl.PointsManager),
                                          OriginalUserTrustTracker = originalUserTrustTracker,
                                          MostRecentUserTrustTracker = mostRecentUserTrustTracker,
                                          CurrentlyRecordedUserInteraction = currentlyRecordedUserInteraction,
                                          ReplacementUserInteraction = replacementUserInteraction
                                      }
                   ;
            //var userRatingInfoQueryOLD = from x in RaterooDB.GetTable<UserRating>()
            //    where
            //        (!x.PointsHaveBecomePending && x.WhenPointsBecomePending < currentTime)
            //        || (!x.ShortTermResolutionReflected && x.RatingPhaseStatus.ShortTermResolutionValue != null)
            //        || (x.LastModifiedTime < x.Rating.LastModifiedResolutionTimeOrCurrentValue)
            //        || (x.ForceRecalculate)
            //    let currentlyRecordedUserInteraction = x.User.UserInteractions.SingleOrDefault(y => y.TrustTrackerUnit == x.TrustTrackerUnit && x.UserRating1 != null && y.User == x.User && y.User1 == x.UserRating1.User)
            //    let replacementUserInteraction = x.User.UserInteractions.SingleOrDefault(y => y.TrustTrackerUnit == x.TrustTrackerUnit && y.User == x.User && y.User1 == x.Rating.UserRating.User)
            //    let originalUserTrustTracker = x.User.TrustTrackers.SingleOrDefault(y => y.TrustTrackerUnit == x.TrustTrackerUnit)
            //    select new 
            //        { 
            //            UserRating = x, 
            //            UserRatingGroup = x.UserRatingGroup,
            //            Rating = x.Rating,
            //            MostRecentUserRatingInUserRating = x.UserRating1,
            //            MostRecentUserRatingInRating = x.Rating.UserRating,
            //            RatingGroup = x.UserRatingGroup.RatingGroup,
            //            //MostRecentResolution = x.UserRatingGroup.RatingGroup.RatingGroupResolutions.Where(rgr => rgr.Status == (int) StatusOfObject.Active).OrderByDescending(rgr => rgr.ExecutionTime).FirstOrDefault(),
            //            RatingGroupAttribute = x.UserRatingGroup.RatingGroup.RatingGroupAttribute,
            //            RatingCharacteristic = x.UserRatingGroup.RatingGroup.RatingGroupAttribute.RatingCharacteristic,
            //            PointsTotal = x.User.PointsTotals.SingleOrDefault(y => y.PointsManager == x.Rating.RatingGroup.TblRow.Tbl.PointsManager),
            //            OriginalUserTrustTrackingStats = originalUserTrustTracker == null ? null : originalUserTrustTracker.TrustTrackerStats,
            //            CurrentlyRecordedUserInteraction = 
            //                currentlyRecordedUserInteraction,
            //            ReplacementUserInteraction = 
            //                replacementUserInteraction,
            //            CurrentlyRecordedUserInteractionStats = currentlyRecordedUserInteraction == null ? null : currentlyRecordedUserInteraction.UserInteractionStats.ToArray(), // to ensure that it loads
            //            ReplacementUserInteractionStats =
            //                replacementUserInteraction == null ? null : replacementUserInteraction.UserInteractionStats.ToArray() // to ensure that it loads

            //        }
            //       ;
            var userRatingInfos = userRatingInfoQuery.Take(maxToTake).ToList();
            bool moreWorkToDo = userRatingInfos.Count() == maxToTake;

            foreach (var userRatingInfo in userRatingInfos)
            {
                //if (userRatingInfo.CurrentlyRecordedUserInteraction != null && 
                //    userRatingInfo.ReplacementUserInteraction != null && 
                //    userRatingInfo.CurrentlyRecordedUserInteraction != userRatingInfo.ReplacementUserInteraction)
                //    Trace.TraceInformation(
                //        "Updating currently recorded interaction " + userRatingInfo.CurrentlyRecordedUserInteraction.User.UserID + ", " + 
                //        userRatingInfo.CurrentlyRecordedUserInteraction.User1.UserID + " replacement user interaction " +
                //        userRatingInfo.ReplacementUserInteraction.User.UserID + "," + userRatingInfo.ReplacementUserInteraction.User1.UserID + 
                //        " Most recent user rating in user rating (older) " + userRatingInfo.MostRecentUserRatingInUserRating.EnteredUserRating + 
                //        " Most recent user rating in rating (newer) " + userRatingInfo.MostRecentUserRatingInRating.EnteredUserRating);

                UpdatePointsForUserRating(userRatingInfo.UserRating, userRatingInfo.PointsTotal, currentTime);
                TrustTrackerStat[] originalUserTrustTrackerStats = /* userRatingInfo.OriginalUserTrustTracker.TrustTrackerStats == null ? new TrustTrackerStat[0] : */ userRatingInfo.OriginalUserTrustTracker.TrustTrackerStats.ToArray();
                PMTrustTrackingBackgroundTasks.UpdateUserInteractionsAfterNewUserRatingIsEntered(DataContext, 
                    userRatingInfo.CurrentlyRecordedUserInteraction, userRatingInfo.ReplacementUserInteraction, 
                    userRatingInfo.UserRating, originalUserTrustTrackerStats, userRatingInfo.MostRecentUserRatingInUserRating, 
                    userRatingInfo.MostRecentUserRatingInRating, userRatingInfo.UserRatingGroup.WhenMade, 
                    userRatingInfo.RatingGroupAttribute, userRatingInfo.RatingCharacteristic, userRatingInfo.MostRecentUserTrustTracker);
            }
            return moreWorkToDo;
        }

        protected void UpdatePointsForUserRating(UserRating theUserRating, PointsTotal thePointsTotal, DateTime currentTime)
        {
            decimal previousNotYetPendingMaxLoss = theUserRating.NotYetPendingMaxLoss;
            decimal previousPendingPointsLongTerm = theUserRating.PendingPointsLongTerm;
            decimal previousPendingPointsShortTerm = theUserRating.PendingPointsShortTerm;
            decimal previousPointsEarnedLongTerm = theUserRating.PointsEarnedLongTerm;
            decimal previousPointsEarnedShortTerm = theUserRating.PointsEarnedShortTerm;
            decimal previousPointsNotYetPendingShortTerm = theUserRating.NotYetPendingPointsShortTerm;
            decimal previousPointsNotYetPendingLongTerm = theUserRating.NotYetPendingPointsLongTerm;
            decimal previousPointsOrPendingPointsLongTermUnweighted = theUserRating.PointsOrPendingPointsLongTermUnweighted;

            if (theUserRating.ForceRecalculate)
            {
                theUserRating.ForceRecalculate = false;
                UpdateUserRatingHighStakesKnownFields(theUserRating, theUserRating.UserRatingGroup.RatingGroupPhaseStatus, theUserRating.UserRatingGroup.WhenMade);
            }

            if (!theUserRating.PointsHaveBecomePending && theUserRating.WhenPointsBecomePending < currentTime)
                theUserRating.PointsHaveBecomePending = true;
            if (!theUserRating.ShortTermResolutionReflected && theUserRating.RatingPhaseStatus.ShortTermResolutionValue != null)
                theUserRating.ShortTermResolutionReflected = true;
            if (!theUserRating.LongTermResolutionReflected && theUserRating.Rating.RatingGroup.ResolutionTime != null)
                theUserRating.LongTermResolutionReflected = true;
            else if (theUserRating.LongTermResolutionReflected && theUserRating.Rating.RatingGroup.ResolutionTime == null)
                theUserRating.LongTermResolutionReflected = false; /* resolution was undone */
            if (theUserRating.Rating.RatingGroup.ResolutionTime != null && theUserRating.ShortTermResolutionReflected && theUserRating.LongTermResolutionReflected && theUserRating.ShortTermResolutionValue != null && theUserRating.LongTermResolutionValue != null && theUserRating.Rating.RatingGroup.ResolutionTime < theUserRating.RatingPhaseStatus.RatingGroupPhaseStatus.ShortTermResolveTime)
                theUserRating.ShortTermResolutionReflected = false; // we've resolved BEFORE the short term resolution time

            theUserRating.LastModifiedTime = currentTime;

            decimal maxLossShortTerm, maxGainShortTerm, profitShortTerm;
            decimal maxLossLongTerm = 0, maxGainLongTerm = 0, profitLongTerm = 0;

            DateTime whenMade = theUserRating.UserRatingGroup.WhenMade;

            decimal longTermPointsWeight = theUserRating.Rating.RatingGroup.RatingGroupAttribute.LongTermPointsWeight;
            Rating theRating = theUserRating.Rating;
            RatingGroup theTopRatingGroup = theRating.RatingGroup2;

            decimal basisForRating = theUserRating.PreviousRatingOrVirtualRating;
            // if we want to give zero points for untrusted ratings, we could uncomment the following lines, but it may be better
            // just to ensure that untrusted ratings don't count in determining points, so initially untrusted ratings are expected
            // to lose points.
            //if (theRating.CurrentValue == theUserRating.EnteredUserRating && theRating.LastTrustedValue != theUserRating.EnteredUserRating && theRating.LastTrustedValue == theUserRating.PreviousRatingOrVirtualRating)
            //    basisForRating = theUserRating.EnteredUserRating;

            decimal profitShortTermUnweighted, profitLongTermUnweighted;
            CalculatePointsInfo(theRating, theTopRatingGroup, theUserRating.RatingPhaseStatus.RatingGroupPhaseStatus, whenMade, basisForRating, (decimal) theUserRating.NewUserRating, theUserRating.ShortTermResolutionValueOrLastTrustedValueIfNotResolved, true, longTermPointsWeight, theUserRating.HighStakesMultiplierOverride, out maxLossShortTerm, out maxGainShortTerm, out profitShortTerm, out profitShortTermUnweighted);

            CalculatePointsInfo(theRating, theTopRatingGroup, theUserRating.RatingPhaseStatus.RatingGroupPhaseStatus, whenMade, basisForRating, (decimal) theUserRating.NewUserRating, theUserRating.LongTermResolutionValueOrLastTrustedValueIfNotResolved, false, longTermPointsWeight, theUserRating.HighStakesMultiplierOverride, out maxLossLongTerm, out maxGainLongTerm, out profitLongTerm, out profitLongTermUnweighted);

            theUserRating.PotentialPointsShortTerm = profitShortTerm;
            theUserRating.PotentialPointsLongTerm = profitLongTerm;
            if (longTermPointsWeight != 0 || theUserRating.UserRatingGroup.WhenMade + TimeSpan.FromDays(theUserRating.UserRatingGroup.RatingGroup.RatingGroupAttribute.MinimumDaysToTrackLongTerm) > TestableDateTime.Now) // either long term is changing or we're still within the number of days that we should track unweighted long-term. Note that this is a minimum because if longtermpointsweight > 0, we track it forever
                theUserRating.PotentialPointsLongTermUnweighted = profitLongTermUnweighted;

            if (theUserRating.RewardPendingPointsTracker != null)
                UpdateRewardPointsBasedOnUpdatedRating(theUserRating.RewardPendingPointsTracker, TestableDateTime.Now, theUserRating.NewUserRating);

            decimal totalPointsAdjustment = 0, pendingPointsAdjustment = 0, notYetPendingMaxLossAdjustment = 0, notYetPendingAdjustment = 0, longTermUnweightedAdjustment = 0;

            totalPointsAdjustment = theUserRating.PointsEarned - (previousPointsEarnedLongTerm + previousPointsEarnedShortTerm);
            pendingPointsAdjustment = theUserRating.PendingPoints - (previousPendingPointsLongTerm + previousPendingPointsShortTerm);
            notYetPendingMaxLossAdjustment = theUserRating.NotYetPendingMaxLoss - previousNotYetPendingMaxLoss;
            notYetPendingAdjustment = (theUserRating.NotYetPendingPointsLongTerm + theUserRating.NotYetPendingPointsShortTerm) - (previousPointsNotYetPendingLongTerm + previousPointsNotYetPendingShortTerm);
            longTermUnweightedAdjustment = theUserRating.PointsOrPendingPointsLongTermUnweighted - previousPointsOrPendingPointsLongTermUnweighted;

            UpdateUserPointsAndStatus(theUserRating.User, theUserRating.Rating.RatingGroup.TblRow.Tbl.PointsManager, PointsChangesReasons.RatingsUpdate, totalPointsAdjustment, totalPointsAdjustment, pendingPointsAdjustment, notYetPendingAdjustment, notYetPendingMaxLossAdjustment, longTermUnweightedAdjustment, false, thePointsTotal);
        }

        public void UpdateUserRatingHighStakesKnownFields(UserRating theUserRating, RatingGroupPhaseStatus rgps, DateTime whenMade)
        {
            DateTime currentTime = TestableDateTime.Now;
            if (rgps.HighStakesKnown && (rgps.HighStakesBecomeKnown == null || (whenMade > rgps.HighStakesBecomeKnown && currentTime > rgps.HighStakesBecomeKnown)))
                theUserRating.HighStakesKnown = true;
            else if (rgps.HighStakesSecret && (rgps.HighStakesBecomeKnown == null || (whenMade < rgps.HighStakesBecomeKnown && currentTime > rgps.HighStakesBecomeKnown)))
                theUserRating.HighStakesPreviouslySecret = true;
            else if (rgps.HighStakesNoviceUser && whenMade > rgps.HighStakesNoviceUserAfter && currentTime > rgps.HighStakesNoviceUserAfter)
                theUserRating.HighStakesKnown = true; // we set to this mode for a novice user only if not appropriate for some other reason.
        }

    }
}