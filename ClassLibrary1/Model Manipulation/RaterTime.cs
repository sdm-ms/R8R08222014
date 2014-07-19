using ClassLibrary1.EFModel;
using ClassLibrary1.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace ClassLibrary1.Model
{
    public static class RaterTime
    {
        internal class TimeRange
        {
            public DateTime? fromTime = null;
            public DateTime? toTime = null;
        }


        internal static TimeSpan aggregateTimeWithinTimeSpan = new TimeSpan(0, 10, 0); // if a second time is within this length of a first time, then the entire time range will be counted as time spent.
        internal static TimeSpan extraTimeToAddBeforeFirstRating = new TimeSpan(0, 5, 0); // must be less than aggregateTimeWithinTimeSpan or we'll have overlapping checkin periods

        public static void UpdateTimeForUser(PointsTotal thePointsTotal, DateTime newCheckInTime)
        {
            if (thePointsTotal.LastCheckIn == newCheckInTime)
                return;

            if (thePointsTotal.CurrentCheckInPeriodStart != null && thePointsTotal.LastCheckIn != null && newCheckInTime - (DateTime)thePointsTotal.LastCheckIn < aggregateTimeWithinTimeSpan)
            {
                ExtendTimePeriods(thePointsTotal, newCheckInTime - (DateTime)thePointsTotal.LastCheckIn);
                thePointsTotal.LastCheckIn = newCheckInTime;
            }
            else
            {
                ExtendTimePeriods(thePointsTotal, extraTimeToAddBeforeFirstRating);
                thePointsTotal.CurrentCheckInPeriodStart = newCheckInTime - extraTimeToAddBeforeFirstRating;
                thePointsTotal.LastCheckIn = newCheckInTime;
            }
        }

        internal static void ExtendTimePeriods(PointsTotal thePointsTotal, TimeSpan extensionOfPeriod)
        {
            thePointsTotal.TotalTimeThisCheckInPeriod = thePointsTotal.TotalTimeThisCheckInPeriod + (decimal)extensionOfPeriod.TotalHours;
            thePointsTotal.TotalTimeThisRewardPeriod = thePointsTotal.TotalTimeThisRewardPeriod + (decimal)extensionOfPeriod.TotalHours;
            thePointsTotal.TotalTimeEver = thePointsTotal.TotalTimeEver + (decimal)extensionOfPeriod.TotalHours;
            PaymentGuarantees.CheckSatisfactionOfConditionalGuarantee(thePointsTotal);
        }


        public static TimeSpan GetTimeForUser(Guid userID, DateTime fromTime, DateTime toTime)
        {
            R8RDataAccess dataAccess = new R8RDataAccess();
            List<DateTime> ratingTimes = dataAccess.R8RDB
                                .GetTable<UserRating>()
                                    .Where(x => x.UserID == userID && x.UserRatingGroup.WhenCreated > fromTime && x.UserRatingGroup.WhenCreated < toTime)
                                    .Select(x => x.UserRatingGroup.WhenCreated)
                                .Concat(
                                    dataAccess.R8RDB.GetTable<UserCheckIn>()
                                    .Where(x => x.UserID == userID && x.CheckInTime > fromTime && x.CheckInTime < toTime)
                                    .Select(x => x.CheckInTime)
                                 )
                                .OrderBy(x => x)
                                .Distinct()
                                .ToList();
            List<TimeRange> timeRangeList = new List<TimeRange>();
            DateTime? lastDateTime = null;
            TimeRange currentRange = null;
            foreach (DateTime time in ratingTimes)
            {
                if (lastDateTime == null || time - lastDateTime < aggregateTimeWithinTimeSpan)
                {
                    if (currentRange == null)
                        currentRange = new TimeRange() { fromTime = time - extraTimeToAddBeforeFirstRating, toTime = time };
                    else
                        currentRange.toTime = time;
                }
                else
                {
                    timeRangeList.Add(currentRange);
                    currentRange = new TimeRange() { fromTime = time - extraTimeToAddBeforeFirstRating, toTime = time };
                }
                lastDateTime = time;
            }
            if (currentRange != null)
                timeRangeList.Add(currentRange);
            TimeSpan totalTime = new TimeSpan(0, 0, 0);
            foreach (var range in timeRangeList)
                totalTime += DateTimeManip.RoundDateTime((DateTime)range.toTime, 1, DateTimeManip.eRoundingDirection.nearest) - DateTimeManip.RoundDateTime((DateTime)range.fromTime, 1, DateTimeManip.eRoundingDirection.nearest);
            return totalTime;
        }

    }
}
