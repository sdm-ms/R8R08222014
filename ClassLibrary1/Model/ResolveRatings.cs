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



namespace ClassLibrary1.Model
{
    ///// <summary>
    ///// Summary description for RaterooSupport
    ///// </summary>
    //public partial class RaterooSupport
    //{
    //    /// <summary>
    //    /// Resolves unresolved predictions in a rating at a specified value.
    //    /// </summary>
    //    /// <param name="ratingID">The rating to resolve trading in</param>
    //    /// <param name="resolutionValue">The resolution value, or null to set points to 0</param>
    //    public void ResolveUserRatings(int ratingID, decimal? resolutionValue, DateTime? effectiveTime)
    //    {
    //        UpdateUserRatingStatus(ratingID, resolutionValue, true);
    //        RaterooDB.SubmitChanges();
    //    }

    //    /// <summary>
    //    /// Ends all ratings in a Tbl at current values.
    //    /// </summary>
    //    /// <param name="TblID">The Tbl</param>
    //    public void EndRatingsForTblAtCurrentValues(int TblID)
    //    {
    //        var theTblRows = RaterooDB.GetTable<TblRow>().Where(e => e.TblID == TblID && e.Status == (Byte)StatusOfObject.Active).Select(e => e.TblRowID);
    //        foreach (int entityID in theTblRows)
    //            EndRatingsForTblRowAtCurrentValues(entityID);
    //    }

    //    /// <summary>
    //    /// Adds ratings to a Tbl for all entities, other than those
    //    /// that already have non-ended ratings. It can then start trading.
    //    /// </summary>
    //    /// <param name="TblID"></param>
    //    /// <param name="startTrading"></param>
    //    public void AddRatingsForTbl(int TblID, bool startTrading)
    //    {
    //        bool someRatingsAlreadyCreated = RaterooDB.GetTable<RatingGroup>().Where(mg => mg.TblRow.TblID == TblID && mg.TradingStatus != (Byte)TradingStatus.Ended).Count() > 0;
    //        var theTblRows = RaterooDB.GetTable<TblRow>().Where(e => e.TblID == TblID && e.Status == (Byte)StatusOfObject.Active).Select(e => e.TblRowID);
    //        foreach (int theTblRowID in theTblRows)
    //        {
    //            bool addRatings = true;
    //            if (someRatingsAlreadyCreated)
    //                addRatings = !RaterooDB.GetTable<RatingGroup>().Any(mg => mg.TblRowID == theTblRowID && mg.TradingStatus != (Byte)TradingStatus.Ended); // add ratings if there are no active or about to be created ones
    //            if (addRatings)
    //                AddRatingsForTblRow(theTblRowID);
    //        }
    //        if (startTrading)
    //            SetTradingStatusHierarchical(TblID, TypeOfObject.Tbl, TradingStatus.Active);
    //    }

    //    /// <summary>
    //    /// Restart all ratings in a Tbl (end at current values and restart).
    //    /// </summary>
    //    /// <param name="TblID"></param>
    //    public void RestartTbl(int TblID)
    //    {
    //        EndRatingsForTblAtCurrentValues(TblID);
    //        AddRatingsForTbl(TblID, true);
    //    }

    //    /// <summary>
    //    /// Ends the ratings for an entity based on the current prediction.
    //    /// </summary>
    //    /// <param name="entityID">The entity</param>
    //    public void EndRatingsForTblRowAtCurrentValues(int entityID)
    //    {
           
    //        TblRow theTblRow = ObjDataAccess.GetTblRow(entityID);
    //        var TblTabs = RaterooDB.GetTable<TblTab>().Where(cg => cg.TblID == theTblRow.TblID && cg.Status == (Byte)StatusOfObject.Active);
    //        foreach (TblTab theTblTab in TblTabs)
    //        {
    //            var theCategories = RaterooDB.GetTable<TblColumn>().Where(c => c.TblTabID == theTblTab.TblTabID && c.Status == (Byte)StatusOfObject.Active);
    //            foreach (TblColumn theCategory in theCategories)
    //            {
    //                bool ratingGroupExists = RaterooDB.GetTable<RatingGroup>().Any(mg => mg.TblRowID == entityID && mg.TblColumnID == theCategory.TblColumnID);
    //                if (ratingGroupExists)
    //                {
    //                    int theRatingGroup = (int)GetTopRatingGroupForTblRowCategory(entityID, theCategory.TblColumnID);
    //                    EndRatingGroupAndSubgroupsAtCurrentValues(theRatingGroup);
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Ends a rating group and hierarchical subgroups based on the value of the current prediction.
    //    /// </summary>
    //    /// <param name="ratingGroupID">The rating group</param>
    //    public void EndRatingGroupAndSubgroupsAtCurrentValues(int ratingGroupID)
    //    {

            
    //        RatingGroup theGroup = ObjDataAccess.GetRatingGroup(ratingGroupID);
    //        var theRatings = RaterooDB.GetTable<Rating>().Where(m => m.RatingGroupID == ratingGroupID).Select(m => m.RatingID);
    //        foreach (int ratingID in theRatings)
    //        {
    //            EndRatingAtCurrentValue(ratingID);
    //        }
    //    }

    //    /// <summary>
    //    /// Ends a rating (and hierarchically below ratings) at their current values.
    //    /// </summary>
    //    /// <param name="ratingID">The rating</param>
    //    public void EndRatingAtCurrentValue(int ratingID)
    //    {

            
    //        Rating theRating = ObjDataAccess.GetRating(ratingID);
    //        FinalizeRatingBeforeEnding(theRating.RatingID, theRating.CurrentValue);
    //        if (theRating.OwnedRatingGroupID != null)
    //            EndRatingGroupAndSubgroupsAtCurrentValues((int)theRating.OwnedRatingGroupID);
    //    }

    //    /// <summary>
    //    /// Ends a rating at a specified value.
    //    /// </summary>
    //    /// <param name="ratingID"></param>
    //    /// <param name="finalValue"></param>
    //    public void EndRatingAtSpecifiedValue(int ratingID, decimal finalValue)
    //    {

           
    //        Rating theRating = ObjDataAccess.GetRating(ratingID);
    //        FinalizeRatingBeforeEnding(theRating.RatingID, finalValue);
    //    }

    //    /// <summary>
    //    /// Ends all ratings in a Tbl while unwinding all unresolved predictions.
    //    /// </summary>
    //    /// <param name="TblID">The Tbl to unwind</param>
    //    public void EndRatingsForTblWithUnwinding(int TblID)
    //    {
    //        var theTblRows = RaterooDB.GetTable<TblRow>().Where(e => e.TblID == TblID && e.Status == (Byte)StatusOfObject.Active).Select(e => e.TblRowID);
    //        foreach (int entityID in theTblRows)
    //            EndRatingsForTblRowWithUnwinding(entityID);
    //    }

    //    /// <summary>
    //    /// Ends the ratings for a particular entity while unwinding all unresolved predictions (i.e., setting 
    //    /// points to zero).
    //    /// </summary>
    //    /// <param name="entityID"></param>
    //    public void EndRatingsForTblRowWithUnwinding(int entityID)
    //    {

            
    //        TblRow theTblRow = ObjDataAccess.GetTblRow(entityID);
    //        var TblTabs = RaterooDB.GetTable<TblTab>().Where(cg => cg.TblID == theTblRow.TblID && cg.Status == (Byte)StatusOfObject.Active);
    //        foreach (TblTab theTblTab in TblTabs)
    //        {
    //            var theCategories = RaterooDB.GetTable<TblColumn>().Where(c => c.TblTabID == theTblTab.TblTabID && c.Status == (Byte)StatusOfObject.Active);
    //            foreach (TblColumn theCategory in theCategories)
    //            {
    //                int theRatingGroup = (int)GetTopRatingGroupForTblRowCategory(entityID, theCategory.TblColumnID);
    //                EndRatingGroupAndSubgroupsWithUnwinding(theRatingGroup);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Ends a particular rating groups and subgroups while unwinding all unresolved predictions (i.e., setting
    //    /// points to zero).
    //    /// </summary>
    //    /// <param name="ratingGroupID">The rating group</param>
    //    public void EndRatingGroupAndSubgroupsWithUnwinding(int ratingGroupID)
    //    {

           
    //        RatingGroup theGroup = ObjDataAccess.GetRatingGroup(ratingGroupID);
    //        var theRatings = RaterooDB.GetTable<Rating>().Where(m => m.RatingGroupID == ratingGroupID).Select(m => m.RatingID);
    //        foreach (int theRatingID in theRatings)
    //            EndRatingWithUnwinding(theRatingID);
    //    }

    //    /// <summary>
    //    /// Ends a rating (and hierarchically below ratings) with unwinding.
    //    /// </summary>
    //    /// <param name="ratingID"></param>
    //    public void EndRatingWithUnwinding(int ratingID)
    //    {

    //        Rating theRating = ObjDataAccess.GetRating(ratingID);
    //        FinalizeRatingBeforeEnding(ratingID, null);
    //        if (theRating.OwnedRatingGroupID != null)
    //            EndRatingGroupAndSubgroupsWithUnwinding((int)theRating.OwnedRatingGroupID);
    //    }

    //    /// <summary>
    //    /// Ends a rating group based on a "winning rating," which is set to the maximum,
    //    /// while all other ratings are set to a minimum. Usually used with probability ratings.
    //    /// </summary>
    //    /// <param name="winningRatingID">The id of the rating winner</param>
    //    public void EndRatingsInGroupRecursivelyBasedOnWinner(int winningRatingID)
    //    {

           
    //        Rating theRating = ObjDataAccess.GetRating(winningRatingID);
    //        RatingGroup theGroup = theRating.RatingGroup;
    //        decimal minimumUserRating = theGroup.RatingGroupAttribute.RatingCharacteristic.MinimumUserRating;

    //        // Now, end all ratings in group
    //        var theRatingsInGroup = RaterooDB.GetTable<Rating>().Where(m => m.RatingGroupID == theGroup.RatingGroupID);
    //        foreach (Rating theRatingInGroup in theRatingsInGroup)
    //        {
    //            if (theRatingInGroup.RatingID == winningRatingID)
    //                FinalizeRatingBeforeEnding(winningRatingID, theGroup.RatingGroupAttribute.ConstrainedSum + minimumUserRating);
    //            else
    //                FinalizeRatingBeforeEnding(theRatingInGroup.RatingID, minimumUserRating);
    //        }

    //        // Now, recurse upwards if necessary.
    //        int? ownerRatingID = GetRatingGroupOwner(theGroup.RatingGroupID);
    //        if (ownerRatingID != null)
    //            EndRatingsInGroupRecursivelyBasedOnWinner((int)ownerRatingID);
    //    }

    //    /// <summary>
    //    /// Ends each rating in a rating group based on a set of values
    //    /// </summary>
    //    /// <param name="ratingGroupID">The rating group</param>
    //    /// <param name="finalResolution">An array of final rating values (or null for unwinding)
    //    /// for each rating (ordered consecutively) in the rating group</param>
    //    public void FinalizeRatingGroupBeforeEnding(int ratingGroupID, decimal?[] finalResolution)
    //    {

            
    //        RatingGroup theGroup = ObjDataAccess.GetRatingGroup(ratingGroupID);
    //        var theRatings = RaterooDB.GetTable<Rating>().Where(m => m.RatingGroupID == ratingGroupID);
    //        foreach (Rating theRating in theRatings)
    //        {
    //            FinalizeRatingBeforeEnding(theRating.RatingID, finalResolution[theRating.NumInGroup - 1]);
    //        }
    //        // Check whether we need to end the rating corresponding to the group
    //        int? ownerRatingID = GetRatingGroupOwner(theGroup.RatingGroupID);
    //        if (ownerRatingID != null)
    //        {
    //            decimal? sum = finalResolution.Sum();
    //            FinalizeRatingBeforeEnding((int)ownerRatingID, sum);
    //        };
    //    }
    //    /// <summary>
    //    /// Ends a particular rating at a specified value
    //    /// </summary>
    //    /// <param name="ratingID">The rating to end</param>
    //    /// <param name="finalResolution">The final value (or null to unwind)</param>
    //    public void FinalizeRatingBeforeEnding(int ratingID, decimal? finalResolution)
    //    {
    //        // Resolve the predictions for the previous round based on the current price if the rating
    //        // needs to be concluded.
    //        Rating theRating = ObjDataAccess.GetRating(ratingID);
    //        CheckRatingGroupForConcludedRounds(theRating.RatingGroupID);

    //        // Resolve the predictions for the current round

          
    //        RatingStatus theRatingStatus = theRating.RatingStatus;
    //        RatingGroupPhaseStatus thePhaseStatus = theRatingStatus.RatingGroupPhaseStatus;
    //        ResolveUserRatings(ratingID, finalResolution);
    //        thePhaseStatus.LongTermResolvedByResolutionOfRatings = true;
    //        UpdateUserRatingStatusForRating(ratingID, true, false); // Will see that rounds are resolved, and will just note that rating is up to date
    //        RaterooDB.SubmitChanges();

    //        // Now, end the rating.
    //        SetTradingStatusNonhierarchical(ratingID, TypeOfObject.Rating, TradingStatus.Ended);
    //    }

    //}
}
