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


namespace ClassLibrary1.Model
{

    public enum VolatilityDuration
    {
        oneHour,
        oneDay,
        oneWeek
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
            }

            throw new Exception("Internal error: Unknown volatility timing span.");
        }

        public static void AddVolatilityTracking(IRaterooDataContext theDataContext, TblRow theTblRow)
        {
            RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
            theDataAccessModule.AddVolatilityTblRowTracker(theTblRow, VolatilityDuration.oneHour);
            theDataAccessModule.AddVolatilityTblRowTracker(theTblRow, VolatilityDuration.oneDay);
            theDataAccessModule.AddVolatilityTblRowTracker(theTblRow, VolatilityDuration.oneWeek);
            int[] excludedRatingGroupTypes = { (int) RatingGroupTypes.hierarchyNumbersBelow, (int) RatingGroupTypes.probabilityHierarchyBelow, (int) RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy };
            foreach (var theRatingGroup in theTblRow.RatingGroups)
            {
                if (!excludedRatingGroupTypes.Contains(theRatingGroup.TypeOfRatingGroup))
                    AddVolatilityTracking(theDataContext, theRatingGroup);
            }
        }

        public static void AddVolatilityTracking(IRaterooDataContext theDataContext, RatingGroup theRatingGroup)
        {
            RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
            theDataAccessModule.AddVolatilityTracker(theRatingGroup, VolatilityDuration.oneHour);
            theDataAccessModule.AddVolatilityTracker(theRatingGroup, VolatilityDuration.oneDay);
            theDataAccessModule.AddVolatilityTracker(theRatingGroup, VolatilityDuration.oneWeek);
        }

        public static bool UpdateTrackers(IRaterooDataContext theDataContext)
        {
            bool[] moreWorkToDo = new bool[3];
            moreWorkToDo[0] = UpdateTrackers(theDataContext, VolatilityDuration.oneHour);
            moreWorkToDo[1] = UpdateTrackers(theDataContext, VolatilityDuration.oneDay);
            moreWorkToDo[2] = UpdateTrackers(theDataContext, VolatilityDuration.oneWeek);
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
            if (theUserRating.NewUserRating == null || theTracker == null)
                return;
            decimal absoluteVolatility = Math.Abs((decimal) theUserRating.NewUserRating - (decimal) theUserRating.PreviousRatingOrVirtualRating);
            decimal maximumVolatility = theUserRating.Rating.RatingCharacteristic.MaximumUserRating - theUserRating.Rating.RatingCharacteristic.MinimumUserRating;
            decimal relativeVolatility = Math.Round(absoluteVolatility / maximumVolatility,3);
            if (!addToVolatility)
                relativeVolatility = 0 - relativeVolatility;
            theTracker.Volatility += relativeVolatility;
            theTracker.VolatilityTblRowTracker.Volatility += relativeVolatility;
        }

        public static bool UpdateTrackers(IRaterooDataContext theDataContext, VolatilityDuration theTimeFrame)
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
                select new { 
                    UserRating = y, 
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

        public static bool UpdateTrackersOldMethod(IRaterooDataContext theDataContext, VolatilityDuration theTimeFrame)
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
                        MaximumVolatility = t.RatingGroup.Ratings.First().RatingCharacteristic.MaximumUserRating - t.RatingGroup.Ratings.First().RatingCharacteristic.MinimumUserRating
                    });
            if (!theTrackersWithInfo.Any())
                return false;
            foreach (var theTracker in theTrackersWithInfo)
            {
                decimal originalValue = theTracker.Tracker.Volatility;
                if (theTracker.NewVolatility == null)
                    theTracker.NewVolatility = 0;
                if (theTracker.ExpiredVolatility == null)
                    theTracker.ExpiredVolatility = 0;
                theTracker.Tracker.Volatility = ((theTracker.Tracker.Volatility * theTracker.MaximumVolatility) + ((decimal) theTracker.NewVolatility) - ((decimal) theTracker.ExpiredVolatility)) / theTracker.MaximumVolatility;
                theTracker.Tracker.StartTime = newStartTime;
                theTracker.Tracker.EndTime = newEndTime;
                theTracker.Tracker.VolatilityTblRowTracker.Volatility += theTracker.Tracker.Volatility - originalValue;
                Trace.TraceInformation("Time frame " + theTimeFrame + " " + theTracker.Tracker.RatingGroup.TblRow.Name + " new volatility " + theTracker.Tracker.VolatilityTblRowTracker.Volatility);
            }
            return true;
        }


    }
}