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

using System.Diagnostics;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{

    public enum VolatilityDuration
    {
        oneHour,
        oneDay,
        oneWeek,
        oneYear
    }

    public static class VolatilityTracking
    {
        public static TimeSpan GetTimeSpanForVolatilityTiming(VolatilityDuration theTiming)
        {
            switch (theTiming)
            {
                case VolatilityDuration.oneHour:
                    return new TimeSpan(1, 0, 0);

                case VolatilityDuration.oneDay:
                    return new TimeSpan(1, 0, 0, 0);

                case VolatilityDuration.oneWeek:
                    return new TimeSpan(7, 0, 0, 0);

                case VolatilityDuration.oneYear:
                    return new TimeSpan(365, 0, 0, 0);
            }

            throw new Exception("Internal error: Unknown volatility timing span.");
        }

        public static void AddVolatilityTracking(R8RDataManipulation theDataAccessModule, TblRow theTblRow)
        {
            theDataAccessModule.AddVolatilityTblRowTracker(theTblRow, VolatilityDuration.oneHour);
            theDataAccessModule.AddVolatilityTblRowTracker(theTblRow, VolatilityDuration.oneDay);
            theDataAccessModule.AddVolatilityTblRowTracker(theTblRow, VolatilityDuration.oneWeek);
            theDataAccessModule.AddVolatilityTblRowTracker(theTblRow, VolatilityDuration.oneYear);
        }

        public static void AddVolatilityTracking(R8RDataManipulation theDataAccessModule, RatingGroup theRatingGroup)
        {
            theDataAccessModule.AddVolatilityTracker(theRatingGroup, VolatilityDuration.oneHour);
            theDataAccessModule.AddVolatilityTracker(theRatingGroup, VolatilityDuration.oneDay);
            theDataAccessModule.AddVolatilityTracker(theRatingGroup, VolatilityDuration.oneWeek);
            theDataAccessModule.AddVolatilityTracker(theRatingGroup, VolatilityDuration.oneYear);
        }

        public static bool UpdateTrackers(IR8RDataContext theDataContext)
        {
            bool[] moreWorkToDo = new bool[4];
            moreWorkToDo[0] = UpdateTrackers(theDataContext, VolatilityDuration.oneHour);
            moreWorkToDo[1] = UpdateTrackers(theDataContext, VolatilityDuration.oneDay);
            moreWorkToDo[2] = UpdateTrackers(theDataContext, VolatilityDuration.oneWeek);
            moreWorkToDo[3] = UpdateTrackers(theDataContext, VolatilityDuration.oneYear);
            return moreWorkToDo.Any(x => x == true);
        }

        internal class TrackerChanges
        {
#pragma warning disable 0649
            public VolatilityTracker Tracker;
            public decimal? ExpiredVolatility;
            public decimal? NewVolatility;
            public decimal MaximumVolatility;
#pragma warning restore 0649
        }

        public static void AddVolatilityForUserRating(UserRating theUserRating)
        {
            var volatilityTrackers = theUserRating.Rating.RatingGroup.VolatilityTrackers;
            foreach (var volatilityTracker in volatilityTrackers)
                ChangeVolatilityForUserRating(theUserRating, volatilityTracker, true);
        }

        public static void ChangeVolatilityForUserRating(UserRating theUserRating, VolatilityTracker theTracker, bool addToVolatility)
        {
            if (theTracker == null)
                return;
            decimal userRatingMovement = (decimal)theUserRating.NewUserRating - (decimal)theUserRating.PreviousRatingOrVirtualRating;
            decimal userRatingAbsMovement = Math.Abs(userRatingMovement);
            decimal maximumVolatility = theUserRating.Rating.RatingCharacteristic.MaximumUserRating - theUserRating.Rating.RatingCharacteristic.MinimumUserRating;
            decimal userRatingMovementRelativeToDistance = userRatingMovement / maximumVolatility;
            decimal userRatingAbsMovementRelativeToDistance = Math.Abs(userRatingMovementRelativeToDistance);
            decimal multiplier = (addToVolatility) ? 1.0M : -1.0M;
            theTracker.TotalMovement += userRatingAbsMovementRelativeToDistance * multiplier;
            theTracker.DistanceFromStart += userRatingMovementRelativeToDistance * multiplier; // note that this is not absolute value, so two moves in opposite directions will cancel one another out
            decimal originalPushback = theTracker.Pushback;
            decimal originalPushbackProportion = theTracker.PushbackProportion;
            theTracker.Pushback = (theTracker.TotalMovement - Math.Abs(theTracker.DistanceFromStart)); // So, if we've moved 100 but are back where we started, this is 100, meaning we're doing a lot of back and forth. 
            theTracker.PushbackProportion = theTracker.TotalMovement == 0 ? 0 : theTracker.Pushback / theTracker.TotalMovement;

            theTracker.VolatilityTblRowTracker.TotalMovement += userRatingAbsMovementRelativeToDistance * multiplier;
            theTracker.VolatilityTblRowTracker.DistanceFromStart += userRatingMovementRelativeToDistance * multiplier;
            theTracker.VolatilityTblRowTracker.Pushback += theTracker.Pushback - originalPushback;
            theTracker.VolatilityTblRowTracker.PushbackProportion += theTracker.PushbackProportion - originalPushbackProportion;

            new FastAccessVolatilityUpdateInfo() { TimeFrame = (VolatilityDuration)theTracker.DurationType, Value = 0.2M * theTracker.VolatilityTblRowTracker.DistanceFromStart + 0.8M * theTracker.VolatilityTblRowTracker.Pushback }.AddToTblRow(theUserRating.Rating.RatingGroup.TblRow);
        }

        public static bool UpdateTrackers(IR8RDataContext theDataContext, VolatilityDuration theTimeFrame)
        {

            var cutoffTime = TestableDateTime.Now - GetTimeSpanForVolatilityTiming(theTimeFrame);

            var ratingsWhoseVolatilityMustBeRemoved = 
                from y in 
                    ((from x in theDataContext.GetTable<UserRating>()
                    where x.VolatilityTrackingNextTimeFrameToRemove == (byte)theTimeFrame && x.UserRatingGroup.WhenMade < cutoffTime
                    select x)
                    .Take(1000))
                let volatilityTracker = y.Rating.RatingGroup.VolatilityTrackers.FirstOrDefault(z => z.DurationType == (byte) theTimeFrame)
                where volatilityTracker != null
                /* some of these must be projected so that we won't hit the database multiple times, even though we don't access the fields directly */
                let rating = y.Rating
                let ratingCharacteristic = rating.RatingCharacteristic
                let ratingGroup = rating.RatingGroup
                let tblRow = ratingGroup.TblRow
                select new { 
                    UserRating = y, 
                    Rating = rating,
                    RatingCharacteristic = y.Rating.RatingCharacteristic,
                    RatingGroup = ratingGroup,
                    TblRow = tblRow,
                    VolatilityTracker = volatilityTracker, 
                    VolatilityTblRowTracker = volatilityTracker.VolatilityTblRowTracker
                };
            //var ratingsWhoseVolatilityMustBeRemoved =
            //    (from x in theDataContext.GetTable<UserRating>()
            //     where x.VolatilityTrackingNextTimeFrameToRemove == (byte)theTimeFrame && x.UserRatingGroup.WhenMade < cutoffTime
            //     let volatilityTracker = x.Rating.RatingGroup.VolatilityTrackers.FirstOrDefault(y => y.DurationType == (byte)theTimeFrame)
            //     where volatilityTracker != null
            //     select new
            //     {
            //         UserRating = x,
            //         VolatilityTracker = volatilityTracker,
            //         VolatilityTblRowTracker = volatilityTracker.VolatilityTblRowTracker
            //     })
            //    .Take(1000);
                
            bool moreWork = false;
            foreach (var rating in ratingsWhoseVolatilityMustBeRemoved)
            {
                moreWork = true;
                ChangeVolatilityForUserRating(rating.UserRating, rating.VolatilityTracker, false);
                rating.UserRating.VolatilityTrackingNextTimeFrameToRemove++;
            }
            return moreWork;
        }

        public static bool UpdateTrackersOldMethod(IR8RDataContext theDataContext, VolatilityDuration theTimeFrame)
        {
            DateTime newEndTime = TestableDateTime.Now;
            DateTime newStartTime = newEndTime - GetTimeSpanForVolatilityTiming(theTimeFrame);
            var theTrackersWithInfo = theDataContext.GetTable<VolatilityTracker>()
                .Where(x => x.DurationType == (byte)theTimeFrame 
                    && x.RatingGroup.UserRatingGroups.Any(pg => 
                        (pg.WhenMade >= x.StartTime && pg.WhenMade < x.EndTime && pg.WhenMade < newStartTime) /* predictions that no longer should count toward volatility) */ 
                        || 
                        (pg.WhenMade >= x.EndTime && pg.WhenMade >= newStartTime && pg.WhenMade < newEndTime))) /* predictions that now should count toward volatility */
                .Take(1000)
                .Select(t => new TrackerChanges
                    {
                        Tracker = t, 
                        ExpiredVolatility = t.RatingGroup.UserRatingGroups
                                            .Where(pg => pg.WhenMade >= t.StartTime && pg.WhenMade < t.EndTime && pg.WhenMade < newStartTime)
                                            .SelectMany(pg => pg.UserRatings)
                                            .Where(p => p.NewUserRating != null && p.PreviousDisplayedRating != null)
                                            .Sum(p => Math.Abs(((decimal?)p.NewUserRating - p.PreviousRatingOrVirtualRating) ?? 0)),
                        NewVolatility =     t.RatingGroup.UserRatingGroups
                                            .Where(pg => pg.WhenMade >= t.EndTime && pg.WhenMade >= newStartTime && pg.WhenMade < newEndTime)
                                            .SelectMany(pg => pg.UserRatings)
                                            .Where(p => p.NewUserRating != null && p.PreviousDisplayedRating != null)
                                            .Sum(p => Math.Abs(((decimal?)p.NewUserRating - p.PreviousRatingOrVirtualRating) ?? 0)),
                        MaximumVolatility = t.RatingGroup.Ratings.FirstOrDefault().RatingCharacteristic.MaximumUserRating - t.RatingGroup.Ratings.FirstOrDefault().RatingCharacteristic.MinimumUserRating
                    });
            if (!theTrackersWithInfo.Any())
                return false;
            foreach (var theTracker in theTrackersWithInfo)
            {
                decimal originalValue = theTracker.Tracker.TotalMovement;
                if (theTracker.NewVolatility == null)
                    theTracker.NewVolatility = 0;
                if (theTracker.ExpiredVolatility == null)
                    theTracker.ExpiredVolatility = 0;
                theTracker.Tracker.TotalMovement = ((theTracker.Tracker.TotalMovement * theTracker.MaximumVolatility) + ((decimal)theTracker.NewVolatility) - ((decimal)theTracker.ExpiredVolatility)) / theTracker.MaximumVolatility;
                theTracker.Tracker.StartTime = newStartTime;
                theTracker.Tracker.EndTime = newEndTime;
                theTracker.Tracker.VolatilityTblRowTracker.TotalMovement += theTracker.Tracker.TotalMovement - originalValue;
                Trace.TraceInformation("Time frame " + theTimeFrame + " " + theTracker.Tracker.RatingGroup.TblRow.Name + " new volatility " + theTracker.Tracker.VolatilityTblRowTracker.TotalMovement);
            }
            return true;
        }


    }
}