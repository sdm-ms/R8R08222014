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
using ClassLibrary1.Misc;
using System.Diagnostics;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        const float MinChangePromptingReview = 0.2F;
        const int NumUsersWhoseRatingsShouldBeFlaggedAtOnce = 30;
        const int MaxRatingsToFlagPerUser = 200;
        const int MaxNumDaysToReview = 30;
        const int NumRatingsToReviewAtOnce = 30;

        public bool IdleTaskFlagRatingsNeedingReview()
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
                                                    .Where(x => x.ReviewRecentUserRatingsAfter == null) // since we allow 20 minutes before this should start, we decrease the risk that we will end up in an endless loop where we flag some set of ratings and those are updated before we flag the remaining ratings for a particular user
                                                    .Distinct()
                                                    .Take(MaxRatingsToFlagPerUser)
                                    };
            var trustTrackerWithRatings = ratingsQuery.ToList();
            bool moreWorkToDo = trustTrackerWithRatings.Count() == NumUsersWhoseRatingsShouldBeFlaggedAtOnce; // see also below
            foreach (var item in trustTrackerWithRatings)
            {
                foreach (Rating r in item.Ratings)
                {
                    r.ReviewRecentUserRatingsAfter = TestableDateTime.Now + TimeSpan.FromMinutes(20); // see above for why we delay this
                    Debug.WriteLine("Set rating " + r.RatingID + " for review at " + r.ReviewRecentUserRatingsAfter.ToString()); // DEBUG
                }
                item.TrustTracker.OverallTrustLevelAtLastReview = item.TrustTracker.OverallTrustLevel;
                if (item.Ratings.Count() < MaxRatingsToFlagPerUser)
                    item.TrustTracker.DeltaOverallTrustLevel = 0;
                else
                    moreWorkToDo = true; // could be more ratings for this user
            }
            return moreWorkToDo;
        }

        public static Tuple<float, float> HypotheticalAdjFactorsNotWorthImplementing = new Tuple<float, float>(0.8F, 1.2F);

        public bool IdleTaskReviewRecentUserRatings()
        {
            DateTime cutoffTime = TestableDateTime.Now - TimeSpan.FromDays(MaxNumDaysToReview); // for userratings
            DateTime currentTime = TestableDateTime.Now; // to prevent prematurely reviewing ratings
            var userRatingsInitialQuery =
                                   (
                                       from r in DataContext.GetTable<Rating>()
                                       where r.ReviewRecentUserRatingsAfter < currentTime
                                       select r
                                   )
                                   .Take(NumRatingsToReviewAtOnce)
                                   .SelectMany(x => x.UserRatings.Where(y => y.UserRatingGroup.WhenMade > cutoffTime));
            var userRatingsGrouped = from ur in userRatingsInitialQuery
                                     group ur by ur.Rating into grouped
                                     let pointsManager = grouped.Key.RatingGroup.RatingGroupAttribute.PointsManager
                                     let adminPointsTotals = pointsManager.PointsTotals.SingleOrDefault(x => x.User.Username == "admin")
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
                                         RatingPhaseStatus = rating.RatingPhaseStatus.OrderByDescending(x => x.RatingPhaseStatusID).FirstOrDefault(),
                                         UserRatings = grouped.OrderBy(x => x.UserRatingGroup.WhenMade),
                                         TrustTrackerUnit = pointsManager.TrustTrackerUnit,
                                         AdminAccount = adminUserAccount,
                                         AdminPointsTotals = adminPointsTotals,
                                         NewTrustLevel = grouped.Select(x => 
                                             x.User.TrustTrackers.SingleOrDefault(tt => 
                                                 tt.TrustTrackerUnit.PointsManagers.Any() && tt.TrustTrackerUnit.PointsManagers.First() == pointsManager)
                                                 .OverallTrustLevelAtLastReview
                                            )
                                     };
            var urSets = userRatingsGrouped.ToList();
            bool moreWorkToDo = userRatingsInitialQuery.Count() == NumRatingsToReviewAtOnce;
            foreach (var urSet in urSets)
            {
                Debug.WriteLine("DEBUG reviewing rating " + urSet.Rating.RatingID);
                decimal? trackIdealRatingValue = null;
                int numUserRatings = urSet.UserRatings.Count();
                List<UserRating> urs = urSet.UserRatings.ToList();

                List<float> newTrusts = urSet.NewTrustLevel.ToList();
                for (int u = 0; u < numUserRatings; u++)
                {
                    UserRating ur = urs[u];
                    if (trackIdealRatingValue == null)
                        trackIdealRatingValue = ur.PreviousRatingOrVirtualRating;
                    decimal newAdjustmentFactor = ur.OriginalAdjustmentPct * ((decimal) newTrusts[u] / ur.OriginalTrustLevel);
                    if (newAdjustmentFactor > 1.0M)
                        newAdjustmentFactor = 1.0M;
                    trackIdealRatingValue = PMAdjustmentFactor.GetRatingToAcceptFromAdjustmentFactor((decimal) trackIdealRatingValue, ur.EnteredUserRating, (float) newAdjustmentFactor, ur.LogarithmicBase);
                }
                decimal actualFinalRating = (decimal) urSet.Rating.CurrentValue;
                if (Math.Abs(actualFinalRating - urs[numUserRatings - 1].NewUserRating) > 0.01M)
                    throw new Exception("Internal error. Rating CurrentValue should equal most recent UserRating NewUserRating");
                // is this a big enough difference?
                float hypotheticalAdjustmentFactorForLastUser = PMAdjustmentFactor.CalculateAdjustmentFactor((decimal)trackIdealRatingValue, actualFinalRating, urs[numUserRatings - 1].PreviousRatingOrVirtualRating);
                bool bigEnoughDifference = (hypotheticalAdjustmentFactorForLastUser < HypotheticalAdjFactorsNotWorthImplementing.Item1 || hypotheticalAdjustmentFactorForLastUser > HypotheticalAdjFactorsNotWorthImplementing.Item2);
                if (bigEnoughDifference)
                { // add the new UserRating
                    UserRatingGroup urg = AddUserRatingGroup(urSet.Rating.RatingGroup2);
                    User adminAccount = urSet.AdminAccount;
                    if (adminAccount == null)
                        adminAccount = DataContext.GetTable<User>().Single(x => x.Username == "admin");
                    UserRatingHierarchyAdditionalInfo additionalInfo = new UserRatingHierarchyAdditionalInfo(1.0F, 1.0F, 0,0,0, new List<TrustTrackerChoiceSummary>(), new List<int>()); // not all info is accurate but it doesn't matter since we ignore UserInteractions where earlier user is the admin account

                    UserRating newUr = AddUserRating(adminAccount, urSet.Rating, urg, urSet.AdminPointsTotals, urSet.RatingPhaseStatus, new List<RatingGroup>() { urSet.Rating.RatingGroup }, (decimal)trackIdealRatingValue, (decimal)trackIdealRatingValue, actualFinalRating, true, additionalInfo);
                }

                urSet.Rating.ReviewRecentUserRatingsAfter = null;
            }
            return moreWorkToDo;
        }

    }
}