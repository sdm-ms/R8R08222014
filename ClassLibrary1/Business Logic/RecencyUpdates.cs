using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class RecencyUpdates
    {
        const int numThresholdProportions = 4;
        static float[] thresholdProportions = new float[] { 0.1F, 0.3F, 0.7F, 0.9F };

        public class RecencyUpdateInfo
        {
            public int? TotalUserRatingsAtNextUpdate;
            public int? NextUpdateIndex;
        }

        public static RecencyUpdateInfo GetNextRecencyUpdate(int userRatingNumber, int totalUserRatingsSoFar)
        {
            float[] thresholds = new float[numThresholdProportions];
            for (int i = 0; i < numThresholdProportions; i++)
            {
                int threshold = GetThresholdForRecencyUpdate(userRatingNumber, thresholdProportions[i]);
                if (totalUserRatingsSoFar < threshold) // if it's less than or equal go to next threshold
                    return new RecencyUpdateInfo() { NextUpdateIndex = i, TotalUserRatingsAtNextUpdate = threshold };
            }
            return new RecencyUpdateInfo() { NextUpdateIndex = null, TotalUserRatingsAtNextUpdate = null }; // no more updates
        }

        internal static int GetThresholdForRecencyUpdate(int userRatingNumber, float pctThreshold)
        {
            // p = pct threshold
            // n = user rating number at time of user rating
            // a = additional user ratings since needed to reach threshold
            // (n + a) - p * (n + a) >= n
            // (n + a) * (1 - p) >= n
            // a >= (n / (1 - p)) - n
            // a = Math.Ceil((n / (1 - p)) - n)
            // t = a + n = Math.Ceil((n / (1 - p))
            return ((int)Math.Ceiling(((double)userRatingNumber / (1.0 - (double)pctThreshold))));
        }

        public static bool UpdateRecency(IR8RDataContext raterooDB)
        {
            int maxToUpdate = 1000;
            var updateQuery = from ur in raterooDB.GetTable<UserRating>()
                              let trustTrackerUnit = ur.TrustTrackerUnit
                              let mostRecentUserRatingRecordedInUserRating = ur.UserRating1 // this previously was the latest user rating
                              let user = ur.User
                              let pmID = ur.Rating.RatingGroup.TblRow.Tbl.PointsManagerID
                              let pointsTotal = user.PointsTotals.FirstOrDefault(pt => pt.PointsManagerID == pmID)
                              let currentlyRecordedUserInteraction = ur.User.UserInteractions.FirstOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit && mostRecentUserRatingRecordedInUserRating != null && y.OriginalRatingUser == user && y.LatestRatingUser == mostRecentUserRatingRecordedInUserRating.User)
                              let originalUserTrustTracker = ur.User.TrustTrackers.FirstOrDefault(y => y.TrustTrackerUnit == trustTrackerUnit)
                              where ur.NextRecencyUpdateAtUserRatingNum != null
                                        && ur.NextRecencyUpdateAtUserRatingNum <= pointsTotal.NumUserRatings
                              select new 
                              { 
                                  UserRating = ur, 
                                  RatingCharacteristic = ur.Rating.RatingCharacteristic,
                                  PointsTotal = pointsTotal, 
                                  UserInteraction = currentlyRecordedUserInteraction, 
                                  RecencyUserInteractionStats = currentlyRecordedUserInteraction.UserInteractionStats
                                    .Where(x => 
                                        x.StatNum == (int) TrustStat.IsMostRecent10PercentOfUsersUserRatings || 
                                        x.StatNum == (int) TrustStat.IsMostRecent30PercentOfUsersUserRatings || 
                                        x.StatNum == (int) TrustStat.IsMostRecent70PercentOfUsersUserRatings || 
                                        x.StatNum == (int) TrustStat.IsMostRecent90PercentOfUsersUserRatings),
                                  TrustTracker = originalUserTrustTracker,
                                  TrustTrackerStats = originalUserTrustTracker.TrustTrackerStats,
                                  CurrentValueBasedOnMostRecentUserRating = mostRecentUserRatingRecordedInUserRating.NewUserRating
                              };
            var itemsToUpdate = updateQuery.Take(maxToUpdate).ToList();
            foreach (var item in itemsToUpdate)
            {
                UserRating ur = item.UserRating;
                RecencyUpdateInfo rui = GetNextRecencyUpdate(ur.UserRatingNumberForUser, item.PointsTotal.NumUserRatings);
                bool originalIsMostRecent10Pct = ur.IsMostRecent10Pct;
                bool originalIsMostRecent30Pct = ur.IsMostRecent30Pct;
                bool originalIsMostRecent70Pct = ur.IsMostRecent70Pct;
                bool originalIsMostRecent90Pct = ur.IsMostRecent90Pct;
                ur.IsMostRecent10Pct = rui.NextUpdateIndex == 0;
                ur.IsMostRecent30Pct = rui.NextUpdateIndex <= 1;
                ur.IsMostRecent70Pct = rui.NextUpdateIndex <= 2;
                ur.IsMostRecent90Pct = rui.NextUpdateIndex <= 3;
                ur.NextRecencyUpdateAtUserRatingNum = rui.TotalUserRatingsAtNextUpdate;
                float ratingMagnitude = AdjustmentFactorCalc.CalculateRelativeMagnitude(ur.EnteredUserRating, ur.PreviousRatingOrVirtualRating, item.RatingCharacteristic.MinimumUserRating, item.RatingCharacteristic.MaximumUserRating, ur.LogarithmicBase);
                float adjustFactor = AdjustmentFactorCalc.CalculateAdjustmentFactor(
                 laterValue: item.CurrentValueBasedOnMostRecentUserRating,
                 enteredValue: ur.EnteredUserRating,
                 basisValue: ur.PreviousRatingOrVirtualRating,
                 logBase: ur.LogarithmicBase,
                 constrainForRetrospectiveAssessment: true);
                if (originalIsMostRecent10Pct && !ur.IsMostRecent10Pct)
                    RemoveUserRatingFromRecencyUserInteractionStat(
                        ur, 
                        item.RecencyUserInteractionStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent10PercentOfUsersUserRatings),
                        item.TrustTrackerStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent10PercentOfUsersUserRatings),
                        ratingMagnitude, adjustFactor);
                if (originalIsMostRecent30Pct && !ur.IsMostRecent10Pct)
                    RemoveUserRatingFromRecencyUserInteractionStat(
                        ur,
                        item.RecencyUserInteractionStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent30PercentOfUsersUserRatings),
                        item.TrustTrackerStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent30PercentOfUsersUserRatings),
                        ratingMagnitude, adjustFactor);
                if (originalIsMostRecent70Pct && !ur.IsMostRecent10Pct)
                    RemoveUserRatingFromRecencyUserInteractionStat(
                        ur,
                        item.RecencyUserInteractionStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent70PercentOfUsersUserRatings),
                        item.TrustTrackerStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent70PercentOfUsersUserRatings),
                        ratingMagnitude, adjustFactor);
                if (originalIsMostRecent90Pct && !ur.IsMostRecent90Pct)
                    RemoveUserRatingFromRecencyUserInteractionStat(
                        ur,
                        item.RecencyUserInteractionStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent90PercentOfUsersUserRatings),
                        item.TrustTrackerStats.FirstOrDefault(x => x.StatNum == (int)TrustStat.IsMostRecent90PercentOfUsersUserRatings),
                        ratingMagnitude, adjustFactor);
            }
            return itemsToUpdate.Count() == maxToUpdate; // more work to do
        }

        internal static void RemoveUserRatingFromRecencyUserInteractionStat(UserRating userRating, UserInteractionStat userInteractionStat, TrustTrackerStat trustTrackerStat, float ratingMagnitude, float adjustmentPct)
        {
            double weightInCalculatingTrustTotal = userInteractionStat.UserInteraction.WeightInCalculatingTrustTotal;
            double previousWeightForIndividualUserInteraction = ratingMagnitude; // the statistic is the same as the NoExtraWeighting statistic as long as the user rating is still recent
            double ratingMagnitudeTimesAdjustmentPct = previousWeightForIndividualUserInteraction * adjustmentPct; 
            double originalAverageAdjustmentPct = userInteractionStat.AvgAdjustmentPctWeighted;
            double originalSumWeights = userInteractionStat.SumWeights;
            userInteractionStat.SumAdjustPctTimesWeight -= ratingMagnitudeTimesAdjustmentPct;
            userInteractionStat.SumWeights -= previousWeightForIndividualUserInteraction;
            if (userInteractionStat.UserInteraction.NumTransactions == 0 || userInteractionStat.SumWeights == 0)
            {
                userInteractionStat.AvgAdjustmentPctWeighted = 0;
                userInteractionStat.SumWeights = 0;
            }
            else
                userInteractionStat.AvgAdjustmentPctWeighted = userInteractionStat.SumAdjustPctTimesWeight / userInteractionStat.SumWeights;
            TrustTrackingBackgroundTasks.AdjustTrustTrackerStatsTrustLevel(userInteractionStat, trustTrackerStat, weightInCalculatingTrustTotal, weightInCalculatingTrustTotal, originalAverageAdjustmentPct, originalSumWeights);
        }
    }
}
