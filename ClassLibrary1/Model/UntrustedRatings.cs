using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public partial class RaterooDataManipulation
    {
        public bool IdleTaskRevertLongUntrustedRatings()
        {
            return false; // Now that we have a new trust traking mechanism, we do not need to add additional user ratings automatically.
            //const int numToTake = 100;
            //DateTime cutoffPoint = TestableDateTime.Now - new TimeSpan(72, 0, 0);
            //var longUntrusteds = (from x in RaterooDB.GetTable<Rating>()
            //                      where x.LastTrustedValue != null && x.LastTrustedValue != x.CurrentValue &&
            //                          (x.RatingGroup.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || x.RatingGroup.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || x.RatingGroup.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber)
            //                      let lastUserRating = x.UserRatings.OrderByDescending(y => y.UserRatingID).FirstOrDefault()
            //                      where lastUserRating != null && lastUserRating.UserRatingGroup.WhenMade < cutoffPoint
            //                      select x).Take(numToTake).ToList();
            //;
            //User admin = RaterooDB.GetTable<User>().Single(x => x.Username == "admin");
            //bool moreWorkToDo = longUntrusteds.Count() == numToTake;
            //foreach (var theRating in longUntrusteds)
            //{
            //    PointsTotal adminPointsTotal = RaterooDB.GetTable<PointsTotal>().Single(x => x.User == admin && x.PointsManagerID == theRating.RatingGroup.TblRow.Tbl.PointsManagerID);
            //    UserRatingGroup theUserRatingGroup = AddUserRatingGroup(theRating.RatingGroup);
            //    RatingPhaseStatus theRatingPhaseStatus = GetRatingPhaseStatus(theRating);
            //    UserRating theUserRating = AddUserRating(theUserRatingGroup, adminPointsTotal, theRating, theRatingPhaseStatus, new List<RatingGroup> { theRating.RatingGroup }, admin, (decimal)theRating.LastTrustedValue, true, true);
            //}
            //return moreWorkToDo;
        }

        private void RevertTrustedRatings(int userIDOfOriginalRater, int userIDOfNewRater)
        {
            //var theNewRater = RaterooDB.GetTable<User>().Single(x => x.UserID == userIDOfNewRater);
            //if (!(theNewRater.SuperUser))
            //    throw new Exception("Internal error: Revert trusted ratings button should be available only to super users.");

            //const int numToTake = 300;

            //var theUserRatings = (from ur in RaterooDB.GetTable<UserRating>()
            //                      where ur.UserID == userIDOfOriginalRater
            //                      where ur.Rating.CurrentValue == ur.EnteredUserRating && ur.EnteredUserRating != null
            //                      select ur).Take(numToTake);
            //foreach (var userRating in theUserRatings)
            //{
            //    decimal newValueToEnter = userRating.PreviousRatingOrVirtualRating;
            //    CompleteRevertOrMimic(theNewRater, newValueToEnter, userRating.Rating);
            //}
            //RaterooDB.SubmitChanges();
        }


        private void RevertOrMimicUntrustedRatings(int userIDOfOriginalRater, int userIDOfNewRater, bool mimic)
        {

            //const int numToTake = 300;

            //var narrowDownRatings = (from ur in RaterooDB.GetTable<UserRating>()
            //                         where ur.UserID == userIDOfOriginalRater
            //                         where ur.Rating.CurrentValue == ur.EnteredUserRating && ur.EnteredUserRating != null
            //                         select ur.Rating).Distinct();
            //var untrustedRatings = (from x in narrowDownRatings /* RaterooDB.GetTable<Rating>() */
            //                        where ((x.LastTrustedValue == null && x.CurrentValue != null) || x.LastTrustedValue != x.CurrentValue) /* last clause sometimes returns false where intuitively should return true b/c of sql-clr type mismatch */ &&
            //                            (x.RatingGroup.TypeOfRatingGroup == (int)RatingGroupTypes.probabilitySingleOutcome || x.RatingGroup.TypeOfRatingGroup == (int)RatingGroupTypes.singleDate || x.RatingGroup.TypeOfRatingGroup == (int)RatingGroupTypes.singleNumber)
            //                        let lastUserRating = x.UserRatings.OrderByDescending(y => y.UserRatingID).FirstOrDefault()
            //                        where lastUserRating != null && lastUserRating.UserID == userIDOfOriginalRater
            //                        select new
            //                        {
            //                            Rating = x,
            //                            LastUserRating = lastUserRating
            //                        }
            //                      ).Take(numToTake).ToList();
            //;
            //User newRater = RaterooDB.GetTable<User>().Single(x => x.UserID == userIDOfNewRater);
            //foreach (var theRatingInfo in untrustedRatings)
            //{
            //    PointsTotal adminPointsTotal = RaterooDB.GetTable<PointsTotal>().Single(x => x.User == newRater && x.PointsManagerID == theRatingInfo.Rating.RatingGroup.TblRow.Tbl.PointsManagerID);
            //    //bool userIsTrusted = UserIsTrusted(theRatingInfo.Rating.RatingGroup.TblRow.Tbl.PointsManager, newRater);
            //    //if (userIsTrusted)
            //    if (newRater.SuperUser)
            //    {
            //        UserRatingGroup theUserRatingGroup = AddUserRatingGroup(theRatingInfo.Rating.RatingGroup);
            //        RatingPhaseStatus theRatingPhaseStatus = GetRatingPhaseStatus(theRatingInfo.Rating);

            //        decimal newRating = 0;
            //        if (mimic)
            //            newRating = (decimal)theRatingInfo.Rating.CurrentValue;
            //        else
            //        {
            //            if (theRatingInfo.Rating.LastTrustedValue == null) // revert not by setting to null, but by setting to basis of previous rating
            //                newRating = theRatingInfo.LastUserRating.PreviousRatingOrVirtualRating;
            //            else
            //                newRating = (decimal)theRatingInfo.Rating.LastTrustedValue;
            //        }
            //        Rating rating = theRatingInfo.Rating;

            //        CompleteRevertOrMimic(newRater, newRating, rating);
            //    }

            //}
            //RaterooDB.SubmitChanges();
        }

        private void CompleteRevertOrMimic(User newRater, decimal newRating, Rating rating)
        {
            //if (!newRater.SuperUser)
            //    return; 
            //// We are no longer offering this functionality for most users, since we no longer need it, given that bad ratings
            //// will be automatically reverted once a few of them are rerated using the new trust tracking functionality.

            //UserRatingHierarchyData theData = new UserRatingHierarchyData();
            //GetUserRatingHierarchyBasedOnUserRatings(
            //    new List<RatingAndUserRating> { 
            //                new RatingAndUserRating { 
            //                    ratingID = rating.RatingID, 
            //                    theUserRating = newRating
            //                } 
            //            },
            //    new List<Rating> { rating },
            //    new List<RatingGroup> { rating.RatingGroup },
            //    rating.RatingGroup.RatingGroupAttribute.ConstrainedSum,
            //    ref theData);

            //AddUserRatingsToAdd(newRater, true, theData);
        }

        public void RevertOrMimicRatings(int userIDOfOriginalRater, int userIDOfNewRater, bool mimic, bool untrustedOnly = true)
        {
            //if (untrustedOnly)
            //    RevertOrMimicUntrustedRatings(userIDOfOriginalRater, userIDOfNewRater, mimic);
            //else
            //{
            //    if (!mimic)
            //        RevertTrustedRatings(userIDOfOriginalRater, userIDOfNewRater);
            //    else
            //        throw new Exception("Internal error: Mimicing of trusted ratings is not implemented.");
            //}

        }

    }
}