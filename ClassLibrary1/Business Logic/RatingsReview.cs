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

using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Nonmodel_Code;
using System.Diagnostics;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for R8RSupport
    /// </summary>
    public partial class R8RDataManipulation
    {
        public static float MinChangePromptingReview = 0.2F;
        const int NumUsersWhoseRatingsShouldBeFlaggedAtOnce = 30;
        const int MaxRatingsToFlagPerUser = 200;
        const int MaxNumDaysToReview = 30;
        const int NumRatingsToReviewAtOnce = 30;

        public bool IdleTaskFlagRatingsNeedingReviewBasedOnChangeInTrust()
        {
            var trustTrackerQuery = 
                from tt in DataContext.GetTable<TrustTracker>()
                where tt.DeltaOverallTrustLevel > MinChangePromptingReview
                select tt;
            var someUsersTrustTrackers = trustTrackerQuery.Take(NumUsersWhoseRatingsShouldBeFlaggedAtOnce);
            var ratingsQuery = from tt in someUsersTrustTrackers
                                    let ur = tt.User.UserRatings
                                    select new { 
                                        TrustTracker = tt,
                                        Ratings = ur.Select(x => x.Rating)
                                                    .Where(x => x.RatingGroup.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || x.RatingGroup.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || x.RatingGroup.RatingGroupAttribute.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber) // we are updating only single-number types of ratings
                                                    .Where(x => x.ReviewRecentUserRatingsAfter == null) // since we allow 20 minutes before this should start, we decrease the risk that we will end up in an endless loop where we flag some set of ratings and those are updated before we flag the remaining ratings for a particular user; when this is null, it means that we have already flagged this one
                                                    .Distinct()
                                                    .Take(MaxRatingsToFlagPerUser) // this shows that we are updating this only incompletely right now; we'll update more later
                                    };
            var trustTrackerWithRatings = ratingsQuery.ToList();
            bool moreWorkToDo = trustTrackerWithRatings.Count() == NumUsersWhoseRatingsShouldBeFlaggedAtOnce; // see also below
            foreach (var item in trustTrackerWithRatings)
            {
                foreach (Rating r in item.Ratings)
                {
                    // this is the flag -- it tells us that pretty soon we're going to review the user ratings for this rating
                    r.ReviewRecentUserRatingsAfter = TestableDateTime.Now + TimeSpan.FromMinutes(20); // see above for why we delay this
                    //Debug.WriteLine("Set rating " + r.RatingID + " for review at " + r.ReviewRecentUserRatingsAfter.ToString());
                }
                item.TrustTracker.OverallTrustLevelAtLastReview = item.TrustTracker.OverallTrustLevel;
                Debug.WriteLine("DEBUG Trust level for " + item.TrustTracker.User.UserID + ": " + item.TrustTracker.OverallTrustLevelAtLastReview);
                if (item.Ratings.Count() < MaxRatingsToFlagPerUser)
                    item.TrustTracker.DeltaOverallTrustLevel = 0;
                else
                    moreWorkToDo = true; // could be more ratings for this user
            }
            return moreWorkToDo;
        }

        public static Tuple<float, float> HypotheticalAdjFactorsNotWorthImplementing = new Tuple<float, float>(0.8F, 1.2F);

        public static Guid DEBUGPrintOut = new Guid();

        public bool IdleTaskReviewRecentUserRatings()
        {
            DateTime cutoffTime = TestableDateTime.Now - TimeSpan.FromDays(MaxNumDaysToReview); // for userratings
            DateTime currentTime = TestableDateTime.Now; // to prevent prematurely reviewing ratings
            var userRatingsInitialQuery =
                                   (
                                       from r in DataContext.GetTable<Rating>()
                                       where r.ReviewRecentUserRatingsAfter < currentTime
                                       orderby r.ReviewRecentUserRatingsAfter
                                       select r
                                   )
                                   .Take(NumRatingsToReviewAtOnce)
                                   .SelectMany(x => x.UserRatings.Where(y => y.UserRatingGroup.WhenCreated > cutoffTime));
            var userRatingsGrouped = from ur in userRatingsInitialQuery
                                     group ur by ur.Rating into grouped
                                     let pointsManager = grouped.Key.RatingGroup.RatingGroupAttribute.PointsManager
                                     let adminPointsTotals = pointsManager.PointsTotals.FirstOrDefault(x => x.User.Username == "admin")
                                     let adminUserAccount = adminPointsTotals == null ? null : adminPointsTotals.User
                                     let rating = grouped.Key
                                     let ratingGroup = rating.RatingGroup
                                     select new 
                                     { 
                                         Rating = rating, 
                                         RatingGroup = ratingGroup,
                                         TblRow = ratingGroup.TblRow,
                                         TblColumn = ratingGroup.TblColumn,
                                         RatingCharacteristic = rating.RatingCharacteristic,
                                         RatingGroupAttribute = ratingGroup.RatingGroupAttribute,
                                         RatingPhaseStatus = rating.RatingPhaseStatus.OrderByDescending(x => x.RatingGroupPhaseStatus.WhenCreated).FirstOrDefault(),
                                         UserRatings = grouped.OrderBy(x => x.UserRatingGroup.WhenCreated),
                                         TrustTrackerUnit = pointsManager.TrustTrackerUnit,
                                         AdminAccount = adminUserAccount,
                                         AdminPointsTotals = adminPointsTotals,
                                         NewTrustLevel = grouped.OrderBy(x => x.UserRatingGroup.WhenCreated).Select(x =>
                                             x.User.TrustTrackers.FirstOrDefault(tt => 
                                                 tt.TrustTrackerUnit.PointsManagers.Any() && tt.TrustTrackerUnit.PointsManagers.FirstOrDefault() == pointsManager)
                                                 .OverallTrustLevelAtLastReview
                                            ),
                                        Users = grouped.Select(x => x.User)
                                     };
            var urSets = userRatingsGrouped.ToList();
            bool moreWorkToDo = userRatingsInitialQuery.Count() == NumRatingsToReviewAtOnce;
            foreach (var urSet in urSets)
            {
                User adminAccount = urSet.AdminAccount;
                if (adminAccount == null)
                    adminAccount = DataContext.GetTable<User>().Single(x => x.Username == "admin");

                decimal? trackIdealRatingValue = null;
                int numUserRatings = urSet.UserRatings.Count();
                List<UserRating> urs = urSet.UserRatings.ToList();

                List<Guid> users = urSet.Users.Select(x => x.UserID).ToList();
                List<double> newTrusts = urSet.NewTrustLevel.ToList();
                int numAdminUserRatingsAlreadyAtEnd = 0;
                for (int u = numUserRatings - 1; u >= 0; u--)
                    if (urs[u].UserID == adminAccount.UserID)
                        numAdminUserRatingsAlreadyAtEnd++;
                    else
                        break;
                bool DEBUGscrutiny = false;
                for (int u = 0; u < numUserRatings - numAdminUserRatingsAlreadyAtEnd; u++)
                {
                    UserRating ur = urs[u];
                    if (ur.RatingID == DEBUGPrintOut && u == 0)
                    {
                        Debug.WriteLine("DEBUG -- reconsidering UserRating " + ur.UserRatingID);
                        DEBUGscrutiny = true;
                    }
                    else if (DEBUGscrutiny)
                        Debug.WriteLine("     UR" + u +  ": " + ur.PreviousDisplayedRating + " --> " + ur.EnteredUserRating + " --> " + ur.NewUserRating);
                    if (trackIdealRatingValue == null)
                        trackIdealRatingValue = ur.PreviousRatingOrVirtualRating;
                    decimal newAdjustmentFactor = ur.OriginalTrustLevel == 0 ? (decimal) newTrusts[u] : ur.OriginalAdjustmentPct * ((decimal) newTrusts[u] / ur.OriginalTrustLevel);
                    if (newAdjustmentFactor > 1.0M)
                        newAdjustmentFactor = 1.0M;
                    trackIdealRatingValue = AdjustmentFactorCalc.GetRatingToAcceptFromAdjustmentFactor((decimal)trackIdealRatingValue, ur.EnteredUserRating, (float)newAdjustmentFactor, ur.LogarithmicBase);
                }
                decimal actualFinalRating = (decimal) urSet.Rating.CurrentValue;
                if (Math.Abs(actualFinalRating - urs[numUserRatings - 1].NewUserRating) > 0.01M)
                    throw new Exception("Internal error. Rating CurrentValue should equal most recent UserRating NewUserRating");
                // is this a big enough difference?
                float hypotheticalAdjustmentFactorForLastUser = AdjustmentFactorCalc.CalculateAdjustmentFactor((decimal)trackIdealRatingValue, actualFinalRating, urs[numUserRatings - 1].PreviousRatingOrVirtualRating);
                bool bigEnoughDifference = (hypotheticalAdjustmentFactorForLastUser < HypotheticalAdjFactorsNotWorthImplementing.Item1 || hypotheticalAdjustmentFactorForLastUser > HypotheticalAdjFactorsNotWorthImplementing.Item2);
                if (DEBUGscrutiny)
                    Debug.WriteLine("DEBUG -- bigEnoughDifference? " + bigEnoughDifference);
                //Debug.WriteLine(" reviewing rating " + urSet.Rating.RatingID + " entered " + urs.Last().EnteredUserRating + " final " + actualFinalRating + " ideal " + trackIdealRatingValue + " hypo adj " + hypotheticalAdjustmentFactorForLastUser + " big enough? " + bigEnoughDifference);
                if (bigEnoughDifference)
                { // add the new UserRating
                    UserRatingGroup urg = AddUserRatingGroup(urSet.Rating.TopRatingGroup);
                    UserRatingHierarchyAdditionalInfo additionalInfo = new UserRatingHierarchyAdditionalInfo(1.0F, 1.0F, 0,0,0, new List<TrustTrackerChoiceSummary>(), new List<Guid>()); // not all info is accurate but it doesn't matter since we ignore UserInteractions where earlier user is the admin account

                    UserRating newUr = AddUserRating(adminAccount, urSet.Rating, urg, urSet.AdminPointsTotals, urSet.RatingPhaseStatus, new List<RatingGroup>() { urSet.Rating.RatingGroup }, (decimal)trackIdealRatingValue, (decimal)trackIdealRatingValue, actualFinalRating, true, additionalInfo);
                    if (DEBUGscrutiny && (double) trackIdealRatingValue > 6.9 && (double) trackIdealRatingValue < 7.1)
                    {
                        var DEBUG2 = 0;
                    }
                    if (DEBUGscrutiny)
                        Debug.WriteLine("DEBUG -- admin added " + trackIdealRatingValue + " replacing: " + actualFinalRating);
                }

                urSet.Rating.ReviewRecentUserRatingsAfter = null;
            }
            return moreWorkToDo;
        }

    }
}