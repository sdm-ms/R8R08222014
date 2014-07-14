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
using System.Diagnostics;
using MoreStrings;

using System.Threading;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{
    

    public enum UserRatingUpdatingReason
    {
        pendingPointsRecalculate,
        shortTermExpiration,
        resolution,
        undoResolution
    }

    /// <summary>
    /// Summary description for R8RSupport
    /// </summary>
    public partial class R8RDataManipulation
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
                .OrderBy(x => x.ExecutionTime).ThenBy(x => x.RatingGroupResolutionID) // OK to order by ID just to get a consistent ordering
                .Take(maxAtOnce))
                let RatingResolution = x
                let RatingGroup = x.RatingGroup
                let Ratings = RatingGroup.Ratings2
                let UserRatings = Ratings.SelectMany(y => y.UserRatings)
                let TblRow = RatingGroup.TblRow
                let TblColumn = RatingGroup.TblColumn
                let Tbl = TblRow.Tbl
                let Users = UserRatings.Select(y => y.User).Distinct()
                let PointsTotals = Users.SelectMany(y => y.PointsTotals.Where(z => z.PointsManager == x.RatingGroup.TblRow.Tbl.PointsManager)).Distinct()
                select new 
                    {
                        RatingResolution = RatingResolution,
                        RatingGroup = RatingGroup,
                        Ratings = Ratings.ToList(),
                        //PreviousRatingResolution = Ratings.First().RatingGroup2.RatingGroupResolutions
                        //    .Where(rgr => rgr.ExecutionTime < x.ExecutionTime && rgr.Status == (int) StatusOfObject.Active)
                        //    .OrderByDescending(rgr => rgr.ExecutionTime)
                        //    .ThenByDescending(rgr => rgr.RatingGroupResolutionID)
                        //    .FirstOrDefault(),
                        UserRatings = UserRatings.ToList(),
                        TblRow = TblRow,
                        TblColumn = TblColumn,
                        Tbl = Tbl,
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
                    TblColumn = resolution.TblColumn,
                    Tbl = resolution.Tbl,
                    ReferenceUserRating = x.UserRatings
                        .Where(y => y.UserRatingGroup.WhenMade < resolution.RatingResolution.EffectiveTime) 
                        .OrderByDescending(y => y.UserRatingGroup.WhenMade)
                        .FirstOrDefault(),
                    ReferenceLastTrustedUserRating = x.UserRatings
                        .Where(y => y.IsTrusted && y.UserRatingGroup.WhenMade < resolution.RatingResolution.EffectiveTime)
                        .OrderByDescending(y => y.UserRatingGroup.WhenMade)
                        .FirstOrDefault()
                });
                foreach (var z in ratingsWithlastUserRatings)
                {
                    decimal? previousValue = z.Rating.CurrentValue;
                    z.Rating.CurrentValue = (resolution.RatingResolution.ResolveByUnwinding || z.ReferenceUserRating == null) ? (decimal?) null : z.ReferenceUserRating.NewUserRating;
                    if (previousValue != z.Rating.CurrentValue)
                    {
                        if (RatingGroupTypesList.singleItemNotDate.Contains(resolution.RatingGroup.TypeOfRatingGroup))
                        {
                            int tblColID = resolution.RatingGroup.TblColumnID;
                            var farui = new FastAccessRatingUpdatingInfo()
                            {
                                TblColumnID = tblColID,
                                NewValue = z.Rating.CurrentValue,
                                StringRepresentation = NumberandTableFormatter.FormatAsSpecified(z.Rating.CurrentValue, z.Rating.RatingCharacteristic.DecimalPlaces, tblColID),
                                RecentlyChanged = true,
                                CountNonNullEntries = resolution.TblRow.CountNonnullEntries,
                                CountUserPoints = resolution.TblRow.CountUserPoints
                            };
                            farui.AddToTblRow(resolution.TblRow);
                            if (z.Rating.CurrentValue == null)
                                z.TblColumn.NumNonNull--;
                            else
                                z.TblColumn.NumNonNull++;
                            z.TblColumn.ProportionNonNull = (double)z.TblColumn.NumNonNull / ((double)z.Tbl.NumTblRowsActive + (double)z.Tbl.NumTblRowsDeleted);
                        }
                        else
                            throw new NotImplementedException(); // must implement copying to fast access for dates
                    }

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

        public bool IdleTaskUpdatePointsAndUserInteractionsInResponseToRatingPhaseStatusTrigger()
        {
            DateTime currentTime = TestableDateTime.Now;
            const int maxToTake = 50;

            DataContext.LoadStatsWithTrustTrackersAndUserInteractions(); // we can't seem to do this in a projection without getting a linq to sql unable to translate error, so we're going to use loading options.

            var rpsInitialQuery = DataContext.GetTable<RatingPhaseStatus>()
                .Where(rps => rps.TriggerUserRatingsUpdate)
                .Take(maxToTake)
                .SelectMany(x => x.UserRatings);

            var userRatingInfoQuery =
                                      from ur in rpsInitialQuery
                                      let trustTrackerUnit = ur.TrustTrackerUnit
                                      let mostRecentUserRatingRecordedInUserRating = ur.UserRating1 // this previously was the latest user rating
                                      let mostRecentUserRatingRecordedInRating = ur.Rating.UserRating // this now is the latest user rating
                                      let user = ur.User
                                      let pointsTotal = user.PointsTotals.Single(pt => pt.PointsManagerID == ur.Rating.RatingGroup.RatingGroupAttribute.PointsManagerID)
                                      let currentlyRecordedUserInteraction = ur.User.UserInteractions.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit && mostRecentUserRatingRecordedInUserRating != null && y.User == user && y.User1 == mostRecentUserRatingRecordedInUserRating.User)
                                      let replacementUserInteraction = ur.User.UserInteractions.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit && y.User == user && y.User1 == mostRecentUserRatingRecordedInRating.User)
                                      let originalUserTrustTracker = ur.User.TrustTrackers.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit)
                                      let mostRecentUserTrustTracker = mostRecentUserRatingRecordedInRating.User.TrustTrackers.SingleOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit)
                                      select new
                                      {
                                          UserRating = ur,
                                          UserRatingGroup = ur.UserRatingGroup,
                                          Rating = ur.Rating,
                                          RatingPhaseStatus = ur.RatingPhaseStatus,
                                          MostRecentUserRatingInUserRating = ur.UserRating1,
                                          MostRecentUserRatingInRating = ur.Rating.UserRating,
                                          RatingGroup = ur.UserRatingGroup.RatingGroup,
                                          RatingGroupAttribute = ur.UserRatingGroup.RatingGroup.RatingGroupAttribute,
                                          RatingCharacteristic = ur.UserRatingGroup.RatingGroup.RatingGroupAttribute.RatingCharacteristic,
                                          PointsTotal = ur.User.PointsTotals.SingleOrDefault(y => y.PointsManager == ur.Rating.RatingGroup.TblRow.Tbl.PointsManager),
                                          OriginalUserTrustTracker = originalUserTrustTracker,
                                          MostRecentUserTrustTracker = mostRecentUserTrustTracker,
                                          CurrentlyRecordedUserInteraction = currentlyRecordedUserInteraction,
                                          ReplacementUserInteraction = replacementUserInteraction,
                                          TrustTrackerForChoiceInGroups = ur.TrustTrackerForChoiceInGroupsUserRatingLinks.Select(y => y.TrustTrackerForChoiceInGroup)
                                      }
                   ;

            var userRatingInfosGrouped = from ur in userRatingInfoQuery
                                         group ur by ur.RatingPhaseStatus into grouped
                                         select new { RatingPhaseStatus = grouped.Key, UserRatingInfos = grouped.OrderBy(x => x.UserRating.UserRatingGroup.WhenMade) };

            var userRatingInfoGroups = userRatingInfosGrouped.ToList(); // remember, take is already done above
            bool moreWorkToDo = userRatingInfoGroups.Count() == maxToTake;

            foreach (var userRatingInfoGroup in userRatingInfoGroups)
            {
                SetPointsPumpingProportion(userRatingInfoGroup.UserRatingInfos.Select(x => x.UserRating).ToList(), userRatingInfoGroup.UserRatingInfos.Select(x => x.PointsTotal).ToList());
                foreach (var userRatingInfo in userRatingInfoGroup.UserRatingInfos)
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
                    TrustTrackingBackgroundTasks.UpdateUserInteractionsAfterNewUserRatingIsEntered(DataContext,
                        userRatingInfo.CurrentlyRecordedUserInteraction, userRatingInfo.ReplacementUserInteraction,
                        userRatingInfo.UserRating, originalUserTrustTrackerStats, userRatingInfo.MostRecentUserRatingInUserRating,
                        userRatingInfo.MostRecentUserRatingInRating, userRatingInfo.UserRatingGroup.WhenMade,
                        userRatingInfo.RatingGroupAttribute, userRatingInfo.RatingCharacteristic, userRatingInfo.MostRecentUserTrustTracker);
                    userRatingInfo.RatingPhaseStatus.TriggerUserRatingsUpdate = false;
                }
            }
            return moreWorkToDo;
        }

        public bool IdleTaskUpdatePointsBecauseOfSomethingOtherThanNewUserRating()
        {
            DateTime currentTime = TestableDateTime.Now;
            const int maxToTake = 400;
            // DEBUG: Now that this is not called in response to a new UserRating -- only in response to some other need to update points -- 
            // we probably don't need to load all the trust-related data here.
            var userRatingInfoQuery = from x in DataContext.GetTable<UserRating>()
                                      where
                                          (!x.PointsHaveBecomePending && x.WhenPointsBecomePending < currentTime) // triggered when time for points to become pending arrives
                                          || (!x.ShortTermResolutionReflected && x.RatingPhaseStatus.ShortTermResolutionValue != null) // triggered when there has been a short term resolution
                                          // || (x.LastModifiedTime < x.Rating.LastModifiedResolutionTimeOrCurrentValue) // triggered when another UserRating has been added -- now we are doing that in IdleTaskRespondToUpdatePointsTriggers
                                          || (x.ForceRecalculate) // triggered in unusual circumstances, primarily for high stakes ratings, database transitions, and testing purposes
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
                                          PointsTotal = x.User.PointsTotals.SingleOrDefault(y => y.PointsManager == x.Rating.RatingGroup.TblRow.Tbl.PointsManager)
                                      }
                   ;
            var userRatingInfos = userRatingInfoQuery.Take(maxToTake).ToList();
            bool moreWorkToDo = userRatingInfos.Count() == maxToTake;

            foreach (var userRatingInfo in userRatingInfos)
            {
                UpdatePointsForUserRating(userRatingInfo.UserRating, userRatingInfo.PointsTotal, currentTime);
                // we have not loaded userinteractions or trust, so we don't need to update those
            }
            return moreWorkToDo;
        }

        private class PointsMovementSegment
        {
            public int FromPointIndex;
            public int ToPointIndex;
            public decimal DistanceBetweenPoints;
            public int IndexOfUserWhoMovedIt;
            public bool UserWhoMovedItHadPointsToLose;
            public bool MoveRepresentsPointsPumping;

            public PointsMovementSegment DeepCopy()
            {
                return new PointsMovementSegment() { FromPointIndex = this.FromPointIndex, ToPointIndex = this.ToPointIndex, DistanceBetweenPoints = this.DistanceBetweenPoints, IndexOfUserWhoMovedIt = this.IndexOfUserWhoMovedIt, UserWhoMovedItHadPointsToLose = this.UserWhoMovedItHadPointsToLose, MoveRepresentsPointsPumping = this.MoveRepresentsPointsPumping };
            }

            public void SetDistance(List<decimal> points, decimal? logarithmicBase)
            {
                if (logarithmicBase == null)
                    DistanceBetweenPoints = Math.Abs(points[ToPointIndex] - points[FromPointIndex]);
                else
                {
                    Func<decimal, decimal> lg = x => (decimal) TrustCalculations.LogBase(x, (decimal)logarithmicBase);
                    DistanceBetweenPoints = Math.Abs(lg(points[ToPointIndex]) - lg(points[FromPointIndex]));
                }
            }

            public List<PointsMovementSegment> SplitAtIndex(int splitIndex, List<decimal> points, decimal? logarithmicBase)
            {
                PointsMovementSegment s1 = this.DeepCopy();
                PointsMovementSegment s2 = this.DeepCopy();
                s1.ToPointIndex = splitIndex;
                s2.FromPointIndex = splitIndex;
                s1.SetDistance(points, logarithmicBase);
                s2.SetDistance(points, logarithmicBase);
                return new List<PointsMovementSegment>() { s1, s2 };
            }
        }

        public static void SetPointsPumpingProportion(List<UserRating> userRatingsForRatingInChronologicalOrder, List<PointsTotal> correspondingPointsTotals)
        {
            // a point-pumping scheme is an attempt to create a new user account that has nothing to lose, move ratings with that account, and then benefit by fixing the ratings with another account. We are tracking the proportion of points that may be due to points pumping for each user on this table. If it is relatively high for a particular user, then we will reduce all the users' points proportionately. 
            // we measure this by looking only at the current rating phase (though there may be some possibility of long-term points pumping).
            // Points pumping occurs only when one is challenging an earlier user without points who moved along the same segment in the opposite direction.
            // Thus, a movement from value A to B is NOT points pumping if: (1) there were no earlier movement from B to A by a user without points to lose; or (2) there was an earlier movement from B to A but it was by a user with points to lose.
            // A movement by User i becomes irrelevant after subsequent user j if User j reversed the movement by User i, AND either User i or User j had points to lose. If User i had points to lose, that can save only User j from points pumping. If User j had points to lose, then that user is the new user who can be challenged, not User i, because one user's movement from A to B cannot allow an unlimited number of users to move from B to A.
            // A movement by User i also becomes irrelevant after subsequent user j with points to lose moves the user rating in the same direction, because then user K will be challenging user j, not user i. Note that user i's move might still represent points pumping, but it won't be relevant for future users.
            // Note that if we have many movements back and forth by users without points to lose, they will all be points pumping, because they will stay relevant under these criteria.
            int count = userRatingsForRatingInChronologicalOrder.Count();
            if (count == 0)
                return;
            // create a list of the points at the end of moves
            List<decimal> points = userRatingsForRatingInChronologicalOrder.Select(x => x.NewUserRating).ToList();
            points.Add(userRatingsForRatingInChronologicalOrder[0].PreviousRatingOrVirtualRating);
            points = points.Distinct().OrderBy(x => x).ToList();
            Dictionary<decimal, int> pointIndices = new Dictionary<decimal,int>();
            for (int p = 0; p < points.Count(); p++)
                pointIndices.Add(points[p], p);
            
            //  break down each user rating movement into PointsMovementSegments, so that we can keep track of which segments remain relevant and potentially indicate points pumping.
            List<PointsMovementSegment> segmentsAlreadyExaminedAndStillRelevant = new List<PointsMovementSegment>();
            for (int u = 0; u < count; u++)
            {
                // Add new segments for this user
                UserRating ur = userRatingsForRatingInChronologicalOrder[u];
                PointsTotal pointsTotalOfMovingUser = correspondingPointsTotals[u];
                List<PointsMovementSegment> newSegmentsForThisUser = new List<PointsMovementSegment>();
                int startingIndex = pointIndices[ur.PreviousRatingOrVirtualRating];
                int endingIndex = pointIndices[ur.NewUserRating];
                if (startingIndex != endingIndex)
                {
                    int i = startingIndex;
                    bool ratingIsIncreased = endingIndex > startingIndex;
                    while (i != endingIndex)
                    {
                        int orig_i = i;
                        bool goFurther = true;
                        while (goFurther)
                        {
                            if (ratingIsIncreased)
                                i++;
                            else
                                i--;
                            goFurther = i != endingIndex && !segmentsAlreadyExaminedAndStillRelevant.Any(x => x.FromPointIndex == i || x.ToPointIndex == i); // So, if user 1 goes from point 0 to point 5, and user 2 moves from point 5 to point 0, we should have a full segment all the way back -- despite the fact that user 3 may move from 0 to 1.
                        }
                        // any past points that straddle the new ending index need to be split up
                        List<PointsMovementSegment> segmentsNeedingSplitting = segmentsAlreadyExaminedAndStillRelevant.Where(x =>
                            (x.FromPointIndex < i && i < x.ToPointIndex) ||
                            (x.ToPointIndex < i && i < x.FromPointIndex)
                            ).ToList();
                        foreach (PointsMovementSegment straddler in segmentsNeedingSplitting)
                        {
                            List<PointsMovementSegment> replacements = straddler.SplitAtIndex(i, points, ur.LogarithmicBase);
                            int indexOfStraddler = segmentsAlreadyExaminedAndStillRelevant.IndexOf(straddler);
                            segmentsAlreadyExaminedAndStillRelevant.RemoveAt(indexOfStraddler);
                            segmentsAlreadyExaminedAndStillRelevant.Insert(indexOfStraddler, replacements[0]);
                            segmentsAlreadyExaminedAndStillRelevant.Insert(indexOfStraddler + 1, replacements[1]);
                        }

                        PointsMovementSegment pms = new PointsMovementSegment()
                        {
                            FromPointIndex = orig_i,
                            ToPointIndex = i,
                            IndexOfUserWhoMovedIt = u,
                            UserWhoMovedItHadPointsToLose = (pointsTotalOfMovingUser.PendingPoints + pointsTotalOfMovingUser.NotYetPendingPoints + pointsTotalOfMovingUser.TotalPoints) > userRatingsForRatingInChronologicalOrder[u].MaxGain * 10.0M,
                            MoveRepresentsPointsPumping = segmentsAlreadyExaminedAndStillRelevant.Any(x =>
                                            x.FromPointIndex == i && x.ToPointIndex == orig_i // same segment in opposite direction
                                            && !x.UserWhoMovedItHadPointsToLose // by someone without points to lose
                                            )
                        }; // by user without points
                        pms.SetDistance(points, ur.LogarithmicBase);
                        newSegmentsForThisUser.Add(pms);
                        // We save memory and speed up queries by removing irrelevant items rather than simply marking them as irrelevant. This adds time to remove them from the list, and makes the algorithm a bit less transparent. But the memory savings could be significant if we have a very large number of user ratings, because each might be divided across a large number of segments. 
                        List<PointsMovementSegment> segmentsNowIrrelevant = segmentsAlreadyExaminedAndStillRelevant.Where(x =>
                            x.FromPointIndex == i && x.ToPointIndex == orig_i // same segment in opposite direction
                            && (x.UserWhoMovedItHadPointsToLose || pms.UserWhoMovedItHadPointsToLose)).ToList();
                        foreach (PointsMovementSegment segment in segmentsNowIrrelevant)
                            segmentsAlreadyExaminedAndStillRelevant.Remove(segment);
                        if (pms.UserWhoMovedItHadPointsToLose)
                        {
                            List<PointsMovementSegment> additionalSegmentsNowIrrelevant = segmentsAlreadyExaminedAndStillRelevant.Where(x =>
                                x.FromPointIndex == orig_i && x.ToPointIndex == i // same segment in same direction
                                ).ToList();
                            foreach (PointsMovementSegment segment in additionalSegmentsNowIrrelevant)
                                segmentsAlreadyExaminedAndStillRelevant.Remove(segment);
                        }
                    }
                }
                decimal pointsPumpingNumerator = newSegmentsForThisUser.Where(x => x.MoveRepresentsPointsPumping).Sum(x => x.DistanceBetweenPoints);
                decimal pointsPumpingDenominator = newSegmentsForThisUser.Sum(x => x.DistanceBetweenPoints);
                decimal pointsPumpingProportion = pointsPumpingDenominator == 0 ? 0.0M : pointsPumpingNumerator / pointsPumpingDenominator;
                decimal? oldValue = ur.PointsPumpingProportion;
                ur.PointsPumpingProportion = pointsPumpingProportion;
                pointsTotalOfMovingUser.PointsPumpingProportionAvg_Numer += (float)(ur.MaxGain * (pointsPumpingProportion - (oldValue ?? 0)));
                pointsTotalOfMovingUser.PointsPumpingProportionAvg_Denom += oldValue == null ? (float) ur.MaxGain : 0F; // if there already was a value for points pumping proportion, then there is no change to the denominator, because we've already taken the MaxGain into account.
                pointsTotalOfMovingUser.PointsPumpingProportionAvg = R8RDataManipulation.CalculatePointsPumpingProportionAvg(pointsTotalOfMovingUser.PointsPumpingProportionAvg_Numer, pointsTotalOfMovingUser.PointsPumpingProportionAvg_Denom, pointsTotalOfMovingUser.NumUserRatings);

                segmentsAlreadyExaminedAndStillRelevant.AddRange(newSegmentsForThisUser);
            }
        }

        protected void UpdateTrustTrackersForChoiceInGroups(UserRating userRating, RatingCharacteristic theRatingCharacteristic, UserRating previousLatestUserRating, UserRating newLatestUserRating, IEnumerable<TrustTrackerForChoiceInGroup> choiceInGroupTrustTrackers)
        {
            float? previousAdjustmentPct = null;
            if (previousLatestUserRating != null && previousLatestUserRating != userRating)
                previousAdjustmentPct = AdjustmentFactorCalc.CalculateAdjustmentFactor(previousLatestUserRating.NewUserRating, userRating.EnteredUserRating, userRating.PreviousRatingOrVirtualRating, userRating.LogarithmicBase, true);
            float newAdjustmentPct = AdjustmentFactorCalc.CalculateAdjustmentFactor(newLatestUserRating.NewUserRating, userRating.EnteredUserRating, userRating.PreviousRatingOrVirtualRating, userRating.LogarithmicBase, true);
            float ratingMagnitude = AdjustmentFactorCalc.CalculateRelativeMagnitude(userRating.EnteredUserRating, userRating.PreviousRatingOrVirtualRating, theRatingCharacteristic.MinimumUserRating, theRatingCharacteristic.MaximumUserRating, userRating.LogarithmicBase);
            float deltaNumerator = newAdjustmentPct * ratingMagnitude - (previousAdjustmentPct ?? 0) * ratingMagnitude;
            float deltaDenominator = previousAdjustmentPct == null ? ratingMagnitude : 0; // no change in denominator when there was already a previous user rating recorded.
            foreach (TrustTrackerForChoiceInGroup ttcing in choiceInGroupTrustTrackers)
            {
                ttcing.SumAdjustmentPctTimesRatingMagnitude += deltaNumerator;
                ttcing.SumRatingMagnitudes += deltaDenominator;
                ttcing.TrustLevelForChoice = deltaNumerator / deltaDenominator;
            }
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
                theUserRating.ShortTermResolutionReflected = false; // we've resolved BEFORE the short term resolution time, so we need to undo the short term resolution.

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
            CalculatePointsInfo(theRating, theTopRatingGroup, theUserRating.RatingPhaseStatus.RatingGroupPhaseStatus, whenMade, basisForRating, (decimal) theUserRating.NewUserRating, theUserRating.ShortTermResolutionValueOrLastTrustedValueIfNotResolved, true, longTermPointsWeight, theUserRating.HighStakesMultiplierOverride, theUserRating.PastPointsPumpingProportion, out maxLossShortTerm, out maxGainShortTerm, out profitShortTerm, out profitShortTermUnweighted);

            CalculatePointsInfo(theRating, theTopRatingGroup, theUserRating.RatingPhaseStatus.RatingGroupPhaseStatus, whenMade, basisForRating, (decimal)theUserRating.NewUserRating, theUserRating.LongTermResolutionValueOrLastTrustedValueIfNotResolved, false, longTermPointsWeight, theUserRating.HighStakesMultiplierOverride, theUserRating.PastPointsPumpingProportion, out maxLossLongTerm, out maxGainLongTerm, out profitLongTerm, out profitLongTermUnweighted);

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

            UpdateUserPointsAndStatus(theUserRating.User, theUserRating.Rating.RatingGroup.TblRow.Tbl.PointsManager, PointsAdjustmentReason.RatingsUpdate, totalPointsAdjustment, totalPointsAdjustment, pendingPointsAdjustment, notYetPendingAdjustment, notYetPendingMaxLossAdjustment, longTermUnweightedAdjustment, false, thePointsTotal);
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
