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
////using PredRatings;
using MoreStrings;

using ClassLibrary1.Nonmodel_Code;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for R8RSupport
    /// </summary>
    public partial class R8RDataManipulation
    {
        /// <summary>
        /// Sets the rating group attributes corresponding to a rating group below a rating plan in the hierarchy.
        /// For example, if we are planning to have a NL East rating, then we should have a plan for that rating,
        /// and rating group attributes for the hierarchically below rating group consisting of the NL East teams.
        /// Note that this is used for a rating plan, while the RelateRatingAndGroup is used for the actual rating.
        /// </summary>
        /// <param name="ownedRatingGroupAttributesID">The rating group attributes for the hierarchically below group</param>
        /// <param name="ownerRatingPlanID">The rating plan for the hierarchically above rating</param>
        public void RelateRatingPlanAndGroupAttributes(Guid ownedRatingGroupAttributesID, Guid ownerRatingPlanID)
        {
            RatingPlan thePlan = DataContext.GetTable<RatingPlan>().Single(x=>x.RatingPlanID==ownerRatingPlanID);
            RatingGroupAttribute theAttributes = DataContext.GetTable<RatingGroupAttribute>().Single(x=>x.RatingGroupAttributesID==ownedRatingGroupAttributesID);

            thePlan.OwnedRatingGroupAttributesID = ownedRatingGroupAttributesID;
            Guid ownerRatingGroupAttributesID = thePlan.RatingGroupAttributesID;

            RatingGroupTypes parentRatingGroupType = (RatingGroupTypes) DataContext.GetTable<RatingGroupAttribute>().Single(mga => mga.RatingGroupAttributesID == ownerRatingGroupAttributesID).TypeOfRatingGroup;
            RatingGroupTypes subordinateRatingGroupTypes = parentRatingGroupType;
            if (parentRatingGroupType == RatingGroupTypes.hierarchyNumbersTop)
                subordinateRatingGroupTypes = RatingGroupTypes.hierarchyNumbersBelow;
            else if (parentRatingGroupType == RatingGroupTypes.probabilityHierarchyTop)
                subordinateRatingGroupTypes = RatingGroupTypes.probabilityHierarchyBelow;
            else if (parentRatingGroupType == RatingGroupTypes.probabilityMultipleOutcomes)
                subordinateRatingGroupTypes = RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy;
            theAttributes.TypeOfRatingGroup = (Byte)subordinateRatingGroupTypes;
        }

        /// <summary>
        /// Identifies a rating group as being owned by a particular rating (that is, beneath in the hierarchy).
        /// So, a NL East rating might be related to a group of all the teams in the NL East.
        /// </summary>
        /// <param name="ownedRatingGroupID">The hierarchically below rating group </param>
        /// <param name="ownerRatingID">The hierarchically above rating</param>
        public void RelateRatingAndGroup(RatingGroup theOwnedGroup, Rating ownerRating)
        {
            RatingGroup theOwnerGroup = ownerRating.RatingGroup;

            ownerRating.OwnedRatingGroup = theOwnedGroup;

            RatingGroupTypes parentRatingGroupType = (RatingGroupTypes) theOwnerGroup.TypeOfRatingGroup;
            RatingGroupTypes subordinateRatingGroupTypes = parentRatingGroupType;
            if (parentRatingGroupType == RatingGroupTypes.hierarchyNumbersTop)
                subordinateRatingGroupTypes = RatingGroupTypes.hierarchyNumbersBelow;
            else if (parentRatingGroupType == RatingGroupTypes.probabilityHierarchyTop)
                subordinateRatingGroupTypes = RatingGroupTypes.probabilityHierarchyBelow;
            else if (parentRatingGroupType == RatingGroupTypes.probabilityMultipleOutcomes)
                subordinateRatingGroupTypes = RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy;
            theOwnedGroup.TypeOfRatingGroup = (Byte)subordinateRatingGroupTypes;
        }

        /// <summary>
        /// Returns the ratings in a rating group
        /// </summary>
        /// <param name="ratingGroupID">The rating group</param>
        /// <returns>A queryable group of ratings</returns>
        public IQueryable<Rating> GetRatingsForGroup(RatingGroup ratingGroup)
        {
            //RatingGroup theGroup = ObjDataAccess.GetRatingGroup(ratingGroupID);
            return DataContext.GetTable<Rating>().Where(m => m.RatingGroup.RatingGroupID == ratingGroup.RatingGroupID);
        }

        /// <summary>
        /// Returns the rating that is hierarchically above a rating group (or null, if none)
        /// </summary>
        /// <param name="ratingGroupID">The rating group</param>
        /// <returns>The rating above the group, or null</returns>
        public Rating GetRatingGroupOwner(RatingGroup ratingGroup)
        {
            Rating theRating = DataContext.NewOrSingleOrDefault<Rating>(m => m.OwnedRatingGroup.RatingGroupID == ratingGroup.RatingGroupID);
            return theRating;
        }

        /// <summary>
        /// Returns the prediction that is hierarchically above the specified prediction in a prediction group, or null if none.
        /// </summary>
        /// <param name="predictionID">The starting prediction</param>
        /// <returns>The prediction above the starting prediction, or null</returns>
        public UserRating GetHierarchicallyHigherUserRating(UserRating prediction)
        {
            RatingGroup theCurrentRatingGroup = prediction.Rating.RatingGroup;
            UserRatingGroup theUserRatingGroup = prediction.UserRatingGroup;
            Rating theRatingOwner = ObjDataAccess.R8RDB.NewOrSingle<Rating>(m => m.OwnedRatingGroup.RatingGroupID == theCurrentRatingGroup.RatingGroupID);
            if (theRatingOwner == null)
                return null;
            else
            {
                UserRating higherUserRating = DataContext.NewOrSingleOrDefault<UserRating>(
                                                p => p.UserRatingGroup.UserRatingGroupID == theUserRatingGroup.UserRatingGroupID
                                                && p.Rating.RatingID == theRatingOwner.RatingID);
                if (higherUserRating == null)
                    return null;
                else
                    return higherUserRating;
            }
        }

        /// <summary>
        /// Determines whether a prediction is a "leaf node" at the bottom of the prediction hierarchy.
        /// Will always be true if the rating group is not hierarchical.
        /// </summary>
        /// <param name="predictionID">The prediction</param>
        /// <returns>True if this prediction is at the bottom of the hierarchy</returns>
        public bool UserRatingIsBottomOfHierarchy(UserRating prediction)
        {
            return prediction.Rating.OwnedRatingGroup == null;
        }

        /// <summary>
        /// Returns the topmost rating group for a particular tblRow and column
        /// </summary>
        /// <param name="TblColumnID">The table column object</param>
        /// <returns>The topmost rating group</returns>
        public RatingGroup GetTopRatingGroupForTblRowAndColumn(TblRow tblRow, TblColumn TblColumn)
        {
            var firstRating = DataContext.NewOrFirstOrDefault<Rating>(m => m.RatingGroup.TblRow.TblRowID == tblRow.TblRowID && m.RatingGroup.TblColumn.TblColumnID == TblColumn.TblColumnID);
            if (firstRating != null)
                return firstRating.TopRatingGroup;

            // otherwise, use longer method.
            var theRatingGroups = DataContext.GetTable<RatingGroup>().Where(mg => mg.TblRow.TblRowID == tblRow.TblRowID && mg.TblColumn.TblColumnID == TblColumn.TblColumnID && mg.Status == (Byte)StatusOfObject.Active);
            foreach (RatingGroup aGroup in theRatingGroups)
            {
                if (GetRatingGroupOwner(aGroup) == null)
                    return aGroup;
            }
            return null;
        }

        /// <summary>
        /// Returns the topmost rating group, starting at the specified rating group.
        /// </summary>
        /// <param name="ratingGroupID">A rating group that may or may not be topmost</param>
        /// <returns>The topmost rating group</returns>
        public RatingGroup GetTopRatingGroup(RatingGroup ratingGroup)
        {
            var firstRating = DataContext.NewOrFirstOrDefault<Rating>(m => m.RatingGroup.RatingGroupID == ratingGroup.RatingGroupID);
            if (firstRating != null)
                return firstRating.TopRatingGroup;

            Rating theOwnerRating =  DataContext.NewOrSingleOrDefault<Rating>(m => m.OwnedRatingGroup.RatingGroupID == ratingGroup.RatingGroupID);
            if (theOwnerRating == null)
                return ratingGroup;
            else
                return GetTopRatingGroup(theOwnerRating.RatingGroup);
        }

        ///// <summary>
        ///// Sets the trading status of a universe, Tbl, TblRow, rating group, or rating. This routine does not
        ///// ensure hierarchical integrity, but is called by SetTradingStatus below. It does take appropriate action
        ///// when a rating is started. Note that this is called by the FinalizeRating function (not the other way around)
        ///// </summary>
        ///// <param name="objectID">The id of the object</param>
        ///// <param name="theType">The type of object (e.g., TypeOfObject.Tbl)</param>
        ///// <param name="theStatus">The new status to set this to. </param>
        //internal void SetTradingStatusNonhierarchical(Guid objectID, TypeOfObject theType, TradingStatus newStatus)
        //{
        //    TradingStatus currentStatus = (TradingStatus)TradingStatus.Active; // must set to something to avoid compiler error
        //    switch (theType)
        //    {
        //        case TypeOfObject.Rating:
        //            currentStatus = (TradingStatus)ObjDataAccess.GetRating(objectID).RatingStatus.TradingStatus;
        //            break;
        //        case TypeOfObject.RatingGroup:
        //            currentStatus = (TradingStatus)ObjDataAccess.GetRatingGroup(objectID).TradingStatus;
        //            break;
        //        case TypeOfObject.TblRow:
        //            currentStatus = (TradingStatus)ObjDataAccess.GetTblRow(objectID).TradingStatus;
        //            break;
        //        case TypeOfObject.Tbl:
        //            currentStatus = (TradingStatus)ObjDataAccess.GetTbl(objectID).TradingStatus;
        //            break;
        //        case TypeOfObject.PointsManager:
        //            currentStatus = (TradingStatus)ObjDataAccess.GetPointsManager(objectID).TradingStatus;
        //            break;
        //    }
        //    bool makeAChange = true;
        //    if (newStatus == currentStatus)
        //        makeAChange = false;
        //    if (newStatus == TradingStatus.SuspendedHigherLevel && currentStatus != TradingStatus.Active)
        //        makeAChange = false; // Suspension should be noted as higher level only if the trading status otherwise would be active.
        //    if (makeAChange)
        //    {
        //        switch (theType)
        //        {
        //            case TypeOfObject.Rating:
        //                 R8RDB.NewOrSingle<Rating>(x=>x.RatingID==objectID).RatingStatus.TradingStatus = (Byte)newStatus;
        //                break;
        //            case TypeOfObject.RatingGroup:
        //                R8RDB.GetTable<RatingGroup>().Single(x=>x.RatingGroupID==objectID).TradingStatus = (Byte)newStatus;
        //                break;
        //            case TypeOfObject.TblRow:
        //               R8RDB.GetTable<TblRow>().Single(x=>x.TblRowID==objectID).TradingStatus = (Byte)newStatus;
        //                break;
        //            case TypeOfObject.Tbl:
        //               R8RDB.GetTable<Tbl>().Single(x=>x.TblID==objectID).TradingStatus = (Byte)newStatus;
        //                break;
        //            case TypeOfObject.PointsManager:
        //                R8RDB.GetTable<PointsManager>().Single(x=>x.PointsManagerID==objectID).TradingStatus = (Byte)newStatus;
        //                break;
        //        }
        //        R8RDB.SubmitChanges();
        //    }
        //}

        ///// <summary>
        ///// Returns the trading status of a rating, rating group, TblRow, Tbl.
        ///// </summary>
        ///// <param name="objectID">The object id</param>
        ///// <param name="theType">The type of object</param>
        ///// <returns>The trading status</returns>
        //public TradingStatus GetTradingStatus(Guid objectID, TypeOfObject theType)
        //{
        //    switch (theType)
        //    {
        //        case TypeOfObject.Rating:
        //            return (TradingStatus)ObjDataAccess.GetRatingGroup(ObjDataAccess.GetRating(objectID).RatingGroupID).TradingStatus;
        //        case TypeOfObject.RatingGroup:
        //            return (TradingStatus)ObjDataAccess.GetRatingGroup(objectID).TradingStatus;
        //        case TypeOfObject.TblRow:
        //            return (TradingStatus)ObjDataAccess.GetTblRow(objectID).TradingStatus;
        //        case TypeOfObject.Tbl:
        //            return (TradingStatus)ObjDataAccess.GetTbl(objectID).TradingStatus;
        //        case TypeOfObject.PointsManager:
        //            return (TradingStatus)ObjDataAccess.GetPointsManager(objectID).TradingStatus;
        //        default:
        //            throw new Exception("Internal error -- sought trading status of an object that doesn't have one.");
        //    }
        //}

        //public void SetRatingGroupTradingStatus(RatingGroup theTopRatingGroup, TradingStatus newStatus)
        //{
        //    theTopRatingGroup.TradingStatus = (Byte) newStatus;
        //    // Invalidate the cache for the individual table cell and for the row of table cells.
        //    CacheManagement.InvalidateCacheDependency("RatingGroupID" + theTopRatingGroup.RatingGroupID.ToString());
        //    CacheManagement.InvalidateCacheDependency("RatingsForTblRowIDAndTblTabID" + theTopRatingGroup.TblRowID.ToString() + "," + theTopRatingGroup.TblColumn.TblTabID.ToString());
        //}

        public void SetTradingStatus(Guid objectID, TypeOfObject theType, TradingStatus newStatus)
        {
            // This more general functionality is currently not in the project, so we have commented out the 
            // routine, with the body below.
        }

        ///// <summary>
        ///// Sets the trading status of a universe, Tbl, TblRow, rating group, or rating, considering
        ///// the trading status of whatever is immediately higher in the hierarchy. Note that right now,
        ///// this does not directly affect whatever is beneath it in the hierarchy. That will be accomplished
        ///// through a separate routine that runs as a background process noting inconsistencies.
        ///// </summary>
        ///// <param name="objectID">The id of the object</param>
        ///// <param name="theType">The type of object (e.g., TypeOfObject.Rating)</param>
        ///// <param name="newStatus">The new status of the rating</param>
        //public void SetTradingStatus(Guid objectID, TypeOfObject theType, TradingStatus newStatus)
        //{
        //    int? ratingGroupOwner = null;
        //    TradingStatus oldStatus = TradingStatus.Ended; // set below to correct value if necessary

        //    if (newStatus == TradingStatus.Active)
        //    {
        //        // Check immediately higher level to see if trading is active. 
        //        // If not, then we'll change the status to temporarily suspended.
        //        bool allowActive = true;
        //        switch (theType)
        //        {
        //            case TypeOfObject.Rating:
        //                allowActive = R8RDB.GetTable<RatingGroup>().Single(x=>x.RatingGroupID==ObjDataAccess.GetRating(objectID).RatingGroupID).TradingStatus == (Byte)TradingStatus.Active;
        //                break;
        //            case TypeOfObject.RatingGroup:
        //                RatingGroup theRatingGroup = ObjDataAccess.GetRatingGroup(objectID);
        //                oldStatus = (TradingStatus) theRatingGroup.TradingStatus;
        //                ratingGroupOwner = GetRatingGroupOwner(theRatingGroup.RatingGroupID);
        //                if (ratingGroupOwner != null)
        //                    allowActive = R8RDB.NewOrSingle<Rating>(x=>x.RatingID==(Guid)ratingGroupOwner).RatingStatus.TradingStatus == (Byte)TradingStatus.Active;
        //                else
        //                    allowActive = R8RDB.GetTable<RatingGroup>().Single(x => x.RatingGroupID == objectID).TblRow.TradingStatus == (Byte)TradingStatus.Active;
        //                break;
        //            case TypeOfObject.TblRow:
        //                allowActive = R8RDB.GetTable<TblRow>().Single(x=>x.TblRowID==objectID).Tbl.TradingStatus == (Byte)TradingStatus.Active;
        //                break;
        //            case TypeOfObject.Tbl:
        //                allowActive = R8RDB.GetTable<Tbl>().Single(x=>x.TblID==objectID).PointsManager.TradingStatus == (Byte)TradingStatus.Active;
        //                oldStatus = (TradingStatus) R8RDB.GetTable<Tbl>().Single(x => x.TblID == objectID).TradingStatus;
        //                break;
        //        }
        //        if (!allowActive)
        //            newStatus = TradingStatus.SuspendedHigherLevel;
        //    }
        //    // Set the flag for this level.
        //    SetTradingStatusNonhierarchical(objectID, theType, newStatus);
        //    // Now, start trading if this is the topmost rating group and we're changing from not yet started to active.
        //    if (theType == TypeOfObject.RatingGroup
        //            && ratingGroupOwner == null
        //            && newStatus == TradingStatus.Active
        //            && oldStatus == TradingStatus.NotYetStarted)
        //        AdvanceRatingGroupToNextRatingPhase(objectID);
        //    // If we're just starting off a Tbl, go ahead and start trading. Note that when we're adding an TblRow to 
        //    // a Tbl that's already started, we'll add the ratings for that entity separately.
        //    if (theType == TypeOfObject.Tbl && newStatus == TradingStatus.Active && oldStatus == TradingStatus.NotYetStarted)
        //        StartAddingMissingRatingsForTbl(objectID);
        //    //// Do the next level lower hierarchically
        //    //switch (theType)
        //    //{
        //    //    case TypeOfObject.Rating:
        //    //        RatingGroup ownedRatingGroup = ObjDataAccess.GetRating(objectID).RatingGroup1;
        //    //        if (ownedRatingGroup != null)
        //    //            SetTradingStatusHierarchical(ownedRatingGroup.RatingGroupID, TypeOfObject.RatingGroup, newStatus);
        //    //        break;
        //    //    case TypeOfObject.RatingGroup:
        //    //        var theRatings = GetRatingsForGroup(objectID);
        //    //        foreach (Rating theRating in theRatings)
        //    //            SetTradingStatusHierarchical(theRating.RatingID, TypeOfObject.Rating, newStatus);
        //    //        break;
        //    //    case TypeOfObject.TblRow:
        //    //        TblRow theTblRow = ObjDataAccess.GetTblRow(objectID);
        //    //        var TblTabs = R8RDB.GetTable<TblTab>().Where(cg => cg.TblID == theTblRow.TblID && cg.Status == (Byte)StatusOfObject.Active);
        //    //        foreach (TblTab theTblTab in TblTabs)
        //    //        {
        //    //            var theTblColumns = R8RDB.GetTable<TblColumn>().Where(c => c.TblTabID == theTblTab.TblTabID && c.Status == (Byte)StatusOfObject.Active);
        //    //            foreach (TblColumn theTblColumn in theTblColumns)
        //    //            {
        //    //                Guid? theRatingGroupID = GetTopRatingGroupForTblRowAndColumn(objectID, theTblColumn.TblColumnID);
        //    //                if (theRatingGroupID != null)
        //    //                    SetTradingStatusHierarchical((Guid) theRatingGroupID, TypeOfObject.RatingGroup, newStatus);
        //    //            }
        //    //        }
        //    //        break;
        //    //    case TypeOfObject.Tbl:  
                    
        //    //        var theTblRows = R8RDB.GetTable<TblRow>().Where(e => e.TblID == objectID && e.Status == (Byte)StatusOfObject.Active).Select(e => e.TblRowID);
        //    //        foreach (Guid tblRowID in theTblRows)
        //    //            SetTradingStatusHierarchical(tblRowID, TypeOfObject.TblRow, newStatus);
        //    //        break;

        //    //    case TypeOfObject.PointsManager:
                    
        //    //        var theTbls = R8RDB.GetTable<Tbl>().Where(c => c.PointsManagerID == objectID && c.Status == (Byte)StatusOfObject.Active).Select(c => c.TblID);
        //    //        foreach (Guid TblID in theTbls)
        //    //            SetTradingStatusHierarchical(TblID, TypeOfObject.Tbl, newStatus);
        //    //        break;
        //    //}
        //}

        //public bool FixTradingStatusInconsistencies()
        //{
        //    bool moreToDo = false;

        //    var TblsToPause = R8RDB.GetTable<Tbl>().Where(x => 
        //            x.TradingStatus == (Byte) TradingStatus.Active
        //            && x.PointsManager.TradingStatus != (Byte) TradingStatus.Active);
        //    if (TblsToPause.Any())
        //    {
        //        var TblsToFix = TblsToPause.Take(20);
        //        foreach (var theTblToFix in TblsToFix)
        //            SetTradingStatus(theTblToFix.TblID, TypeOfObject.Tbl, TradingStatus.SuspendedHigherLevel);
        //        return true; // may be more work to do
        //    }

        //    //var TblColumnsToPause = R8RDB.GetTable<TblColumn>().Where(x =>
        //    //        x.TradingStatus == (Byte)TradingStatus.Active
        //    //        && x.PointsManager.TradingStatus != (Byte)TradingStatus.Active);
        //    //if (TblColumnsToPause.Any())
        //    //{
        //    //    var TblColumnsToFix = TblColumnsToPause.Take(20);
        //    //    foreach (var theTblColumnToFix in TblColumnsToFix)
        //    //        SetTradingStatus(theTblColumnToFix.TblColumnID, TypeOfObject.TblColumn, TradingStatus.SuspendedHigherLevel);
        //    //    return true; // may be more work to do
        //    //}

        //    // Incomplete code, even before being commented out

        //    return moreToDo;

        //}

        public bool AdvanceRatingGroupsNeedingAdvancing()
        {
            const int numAtOnce = 100;
            DateTime now = TestableDateTime.Now;
            var allRatingGroupsToAdvance = from rg in DataContext.GetTable<RatingGroup>()
                                           where rg.TypeOfRatingGroup != (int)RatingGroupTypes.hierarchyNumbersBelow && rg.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityHierarchyBelow && rg.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy
                                           let activeResolution = rg.RatingGroupResolutions.OrderByDescending(y => y.ExecutionTime).ThenByDescending(y => y.WhenCreated).FirstOrDefault()
                                           where activeResolution == null || activeResolution.CancelPreviousResolutions
                                           let ratingGroupPhaseStatusLast = rg.RatingGroupPhaseStatuses.OrderByDescending(x => x.ActualCompleteTime).FirstOrDefault()
                                           where ratingGroupPhaseStatusLast != null
                                           let currentPhase = ratingGroupPhaseStatusLast.RatingPhase
                                           where !currentPhase.RepeatIndefinitely // when repeating indefinitely, we don't want to create excess phases in the database, so we wait for the user to do it
                                           where now > ratingGroupPhaseStatusLast.ShortTermResolveTime || now > ratingGroupPhaseStatusLast.ActualCompleteTime
                                           select rg;
            var someRatingGroupsToAdvance = allRatingGroupsToAdvance.Take(numAtOnce).ToArray();
            foreach (var ratingGroupToAdvance in someRatingGroupsToAdvance)
                AdvanceRatingGroupIfNeeded(ratingGroupToAdvance);
            return someRatingGroupsToAdvance.Any();
        }

        public void AdvanceRatingGroupIfNeeded(RatingGroup topmostRatingGroup)
        {
            bool didAdvance;
            bool ratingExpired;
            RatingGroupPhaseStatus currentRatingGroupPhaseStatus;
            AdvanceRatingGroupIfNeeded(topmostRatingGroup, out didAdvance, out ratingExpired, out currentRatingGroupPhaseStatus);
        }

        public void AdvanceRatingGroupIfNeeded(RatingGroup topmostRatingGroup, out bool didAdvance, out bool ratingExpired, out RatingGroupPhaseStatus currentRatingGroupPhaseStatus)
        {
            RatingGroupPhaseStatus lastRatingGroupPhaseStatus = DataContext.RegisteredToBeInserted.OfType<RatingGroupPhaseStatus>().Where(mps => mps.RatingGroup == topmostRatingGroup).OrderByDescending(x => x.RoundNum).FirstOrDefault();
            if (lastRatingGroupPhaseStatus == null)
            {
                lastRatingGroupPhaseStatus = GetRatingGroupPhaseStatus(topmostRatingGroup);
            }
            currentRatingGroupPhaseStatus = lastRatingGroupPhaseStatus;
            if (lastRatingGroupPhaseStatus.ShortTermResolveTime < TestableDateTime.Now || lastRatingGroupPhaseStatus.ActualCompleteTime < TestableDateTime.Now)
            {
                currentRatingGroupPhaseStatus = AdvanceRatingGroupToNextRatingPhase(topmostRatingGroup);
                ratingExpired = currentRatingGroupPhaseStatus == null;
                if (ratingExpired)
                {
                    didAdvance = false;
                    return;
                }
                else
                {
                    didAdvance = true;
                    return;
                }
            }
            else
            {
                didAdvance = false;
                ratingExpired = false;
            }
        }

        public RatingGroupPhaseStatus AdvanceRatingGroupToNextRatingPhase(RatingGroup aTopmostRatingGroup)
        {
            // We want to synchronize all the ratings for a table row using the same rating phase group.
            // We do this by passing the same new rating group phase status for each rating in the row. 
            //ProfileSimple.Start("AdvanceRatingGroupToNextRatingPhase");
            //ProfileSimple.Start("AllTopmostRatingGroups");
            var allTopmostRatingGroups = DataContext.WhereFromNewOrDatabase<RatingGroup>(rg => 
                rg.TblRow.TblRowID == aTopmostRatingGroup.TblRow.TblRowID
                && rg.RatingGroupAttribute.RatingCharacteristic.RatingPhaseGroup.RatingPhaseGroupID == aTopmostRatingGroup.RatingGroupAttribute.RatingCharacteristic.RatingPhaseGroup.RatingPhaseGroupID 
                && rg.TypeOfRatingGroup != (int)RatingGroupTypes.hierarchyNumbersBelow && rg.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityHierarchyBelow && rg.TypeOfRatingGroup != (int)RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy).ToList();
            //ProfileSimple.End("AllTopmostRatingGroups");
            RatingGroupPhaseStatus newRatingGroupPhaseStatus = null;
            //ProfileSimple.Start("Foreachloop");
            foreach (var topmostRatingGroup in allTopmostRatingGroups)
            {
                //ProfileSimple.Start("topmostRatingGroup");
                //ProfileSimple.Start("LoadTheRatings");
                var theRatings = DataContext.WhereFromNewOrDatabase<Rating>(m => m.TopRatingGroup.RatingGroupID == topmostRatingGroup.RatingGroupID).ToList();
                //ProfileSimple.End("LoadTheRatings");
                //ProfileSimple.Start("GetRatingGroupPhaseStatus");
                RatingGroupPhaseStatus theRatingGroupPhaseStatus = GetRatingGroupPhaseStatus(topmostRatingGroup); 
                //ProfileSimple.End("GetRatingGroupPhaseStatus");
                foreach (var theRating in theRatings)
                {
                    //ProfileSimple.Start("AdvanceRatingGroup2");
                    bool canAdvance = AdvanceRatingGroupToNextRatingPhase(theRating.TopRatingGroup, theRatingGroupPhaseStatus, ref newRatingGroupPhaseStatus);
                    //ProfileSimple.End("AdvanceRatingGroup2");
                    if (!canAdvance)
                        return null;
                    //ProfileSimple.Start("AddRatingPhaseStatus");
                    AddRatingPhaseStatus(theRating, newRatingGroupPhaseStatus);
                    //ProfileSimple.End("AddRatingPhaseStatus");
                    // if (newRatingGroupPhaseStatus != null)
                    //      Trace.TraceInformation("Round num is now " + newRatingGroupPhaseStatus.RoundNum);
                }
                //ProfileSimple.End("topmostRatingGroup");
            }
            //ProfileSimple.End("Foreachloop");
            //ProfileSimple.End("AdvanceRatingGroupToNextRatingPhase");
            return newRatingGroupPhaseStatus;
        }


        public void ResolveRatingGroupIfNeeded(RatingGroup topRatingGroup)
        {

            RatingGroupPhaseStatus thePhaseStatus = GetRatingGroupPhaseStatus(topRatingGroup);
            RatingPhase thePhase = thePhaseStatus.RatingPhase;

            if (!thePhase.RepeatIndefinitely
                && !(thePhaseStatus.RoundNumThisPhase < (thePhase.RepeatNTimes ?? 1))
                && thePhase.NumberInGroup == thePhase.RatingPhaseGroup.NumPhases
                && thePhaseStatus.ActualCompleteTime < TestableDateTime.Now
                && !RatingGroupIsResolved(topRatingGroup))
            {
                bool resolveByUnwinding = topRatingGroup.CurrentValueOfFirstRating == null;
                User theSuperUser = DataContext.GetTable<User>().Single(u => u.Username == "admin");
                RatingGroupResolution ratingResolution = AddRatingGroupResolution(topRatingGroup, false, resolveByUnwinding, thePhaseStatus.ActualCompleteTime, theSuperUser.UserID);
                // Actual resolution will take place following ResolveRatingsNeedingResolving
            }
        }

        public RatingPhaseStatus GetRatingPhaseStatus(Rating theRating)
        {
            var alreadyEntered = DataContext.RegisteredToBeInserted.OfType<RatingPhaseStatus>().Where(x => x.Rating == theRating).OrderByDescending(x => x.RatingGroupPhaseStatus.RoundNum).FirstOrDefault();
            if (alreadyEntered != null)
                return alreadyEntered;

            if (theRating.RatingPhaseStatus.Any())
                return theRating.RatingPhaseStatus.OrderByDescending(x => x.RatingGroupPhaseStatus.WhenCreated).FirstOrDefault();

            return DataContext.GetTable<RatingPhaseStatus>().Where(x => x.Rating.RatingID == theRating.RatingID).OrderByDescending(x => x.RatingGroupPhaseStatus.WhenCreated).First();
        }

        public RatingGroupPhaseStatus GetRatingGroupPhaseStatus(RatingGroup topRatingGroup)
        {
            var alreadyEntered = DataContext.RegisteredToBeInserted.OfType<RatingGroupPhaseStatus>().Where(x => x.RatingGroup == topRatingGroup).OrderByDescending(x => x.RoundNum).FirstOrDefault();
            if (alreadyEntered != null)
                return alreadyEntered;

            if (topRatingGroup.RatingGroupPhaseStatuses.Any())
                return topRatingGroup.RatingGroupPhaseStatuses.OrderByDescending(x => x.WhenCreated).FirstOrDefault();

            return DataContext.GetTable<RatingGroupPhaseStatus>().Where(x => x.RatingGroup.RatingGroupID == topRatingGroup.RatingGroupID).OrderByDescending(x => x.WhenCreated).First();
        }

        /// <summary>
        /// Advances a rating to the next phase of the rating. Called at the start of trading,
        /// and after a successful check for concluded rounds. Ends the rating if we're at the
        /// last phase. If newRatingGroupPhaseStatus is null, then a new rating phase
        /// status object will be created (this should be done only for the first rating).
        /// Returns false if and only if the rating must be resolved rather than advanced.
        /// </summary>
        /// <param name="ratingID">The rating to advance</param>
        public bool AdvanceRatingGroupToNextRatingPhase(RatingGroup topRatingGroup, RatingGroupPhaseStatus theRatingGroupPhaseStatus, ref RatingGroupPhaseStatus newRatingGroupPhaseStatus)
        {
            RatingPhase thePhase = null;
            if (theRatingGroupPhaseStatus != null)
                thePhase = theRatingGroupPhaseStatus.RatingPhase;
            RatingPhase newPhase;
            int newRoundNumThisPhase = -1;

            if (theRatingGroupPhaseStatus.RoundNum == 0)
            { // Just getting started
                newRoundNumThisPhase = 1;
                string key = "RatingPhaseFirst" + thePhase.RatingPhaseGroup.GetHashCode();
                newPhase = DataContext.TempCacheGet(key) as RatingPhase;
                if (newPhase == null)
                {
                    newPhase = DataContext.NewOrSingle<RatingPhase>(p => p.RatingPhaseGroup.RatingPhaseGroupID == thePhase.RatingPhaseGroup.RatingPhaseGroupID
                        && p.NumberInGroup == 1
                        && p.Status == (Byte)StatusOfObject.Active);
                    DataContext.TempCacheAdd(key, newPhase);
                }


            }
            else /* thePhaseStatus.RoundNum > 0 */
            { // Switch to next phase

                bool doRepeatCurrentPhase = false;
                if (thePhase.RepeatIndefinitely)
                    doRepeatCurrentPhase = true;
                else if (theRatingGroupPhaseStatus.RoundNumThisPhase < thePhase.RepeatNTimes)
                    doRepeatCurrentPhase = true;
                if (!doRepeatCurrentPhase && thePhase.NumberInGroup == thePhase.RatingPhaseGroup.NumPhases)
                { // Time to end the rating.
                    ResolveRatingGroupIfNeeded(topRatingGroup);
                    return false;
                }

                newRoundNumThisPhase = theRatingGroupPhaseStatus.RoundNumThisPhase;
                if (doRepeatCurrentPhase)
                {
                    newRoundNumThisPhase++;
                    string cacheKey = "RatingPhase" + thePhase.RatingPhaseGroup.GetHashCode() + thePhase.NumberInGroup.ToString();
                    newPhase = DataContext.TempCacheGet(cacheKey) as RatingPhase;
                    if (newPhase == null)
                    {
                        newPhase = DataContext.NewOrSingle<RatingPhase>(
                            p => p.RatingPhaseGroup.RatingPhaseGroupID == thePhase.RatingPhaseGroup.RatingPhaseGroupID
                            && p.NumberInGroup == thePhase.NumberInGroup
                            && p.Status == (Byte)StatusOfObject.Active);
                        DataContext.TempCacheAdd(cacheKey, newPhase);
                    }
                }
                else
                {
                    newPhase = DataContext.NewOrSingle<RatingPhase>(
                        p => p.RatingPhaseGroup.RatingPhaseGroupID == thePhase.RatingPhaseGroup.RatingPhaseGroupID
                        && p.NumberInGroup == thePhase.NumberInGroup + 1
                        && p.Status == (Byte)StatusOfObject.Active);
                }

                thePhase = newPhase;

            }

            if (newRatingGroupPhaseStatus != null && newRatingGroupPhaseStatus.RatingGroup != topRatingGroup && GetTopRatingGroup(newRatingGroupPhaseStatus.RatingGroup) != topRatingGroup)
            {   // this is a new table cell. copy the values from the previous.
                RatingGroupPhaseStatus copyRatingGroupPhaseStatus = newRatingGroupPhaseStatus;
                newRatingGroupPhaseStatus = AddRatingGroupPhaseStatus(theRatingGroupPhaseStatus.RatingPhaseGroup, topRatingGroup);
                newRatingGroupPhaseStatus.ActualCompleteTime = copyRatingGroupPhaseStatus.ActualCompleteTime;
                newRatingGroupPhaseStatus.EarliestCompleteTime = copyRatingGroupPhaseStatus.EarliestCompleteTime;
                newRatingGroupPhaseStatus.HighStakesBecomeKnown = copyRatingGroupPhaseStatus.HighStakesBecomeKnown;
                newRatingGroupPhaseStatus.HighStakesKnown = copyRatingGroupPhaseStatus.HighStakesKnown;
                newRatingGroupPhaseStatus.HighStakesReflected = copyRatingGroupPhaseStatus.HighStakesReflected;
                newRatingGroupPhaseStatus.HighStakesSecret = copyRatingGroupPhaseStatus.HighStakesSecret;
                newRatingGroupPhaseStatus.HighStakesNoviceUser = copyRatingGroupPhaseStatus.HighStakesNoviceUser;
                newRatingGroupPhaseStatus.HighStakesNoviceUserAfter = copyRatingGroupPhaseStatus.HighStakesNoviceUserAfter;
                newRatingGroupPhaseStatus.RatingPhase = copyRatingGroupPhaseStatus.RatingPhase;
                newRatingGroupPhaseStatus.RatingPhaseGroup = copyRatingGroupPhaseStatus.RatingPhaseGroup;
                newRatingGroupPhaseStatus.RoundNum = copyRatingGroupPhaseStatus.RoundNum;
                newRatingGroupPhaseStatus.RoundNumThisPhase = copyRatingGroupPhaseStatus.RoundNumThisPhase;
                newRatingGroupPhaseStatus.ShortTermResolveTime = copyRatingGroupPhaseStatus.ShortTermResolveTime;
                newRatingGroupPhaseStatus.StartTime = copyRatingGroupPhaseStatus.StartTime;
            }
            else if (newRatingGroupPhaseStatus == null)
            {   // this is the first time through --> we'll use this until we get to a new table cell
                newRatingGroupPhaseStatus = AddRatingGroupPhaseStatus(theRatingGroupPhaseStatus.RatingPhaseGroup, topRatingGroup);

                newRatingGroupPhaseStatus.RoundNum = theRatingGroupPhaseStatus.RoundNum + 1;
                newRatingGroupPhaseStatus.RoundNumThisPhase = newRoundNumThisPhase;
                newRatingGroupPhaseStatus.RatingPhase = newPhase;

                if (newPhase.Timed)
                {
                    if (newPhase.BaseTimingOnSpecificTime)
                        newRatingGroupPhaseStatus.EarliestCompleteTime = (DateTime)thePhase.EndTime;
                    else
                        newRatingGroupPhaseStatus.EarliestCompleteTime = TestableDateTime.Now + new TimeSpan(0, 0, 0, (int)newPhase.RunTime);
                }
                else
                    newRatingGroupPhaseStatus.EarliestCompleteTime = TestableDateTime.Now + new TimeSpan(1000000, 0, 0, 0, 0);

                int secondsDelay = (int)((0 - newPhase.HalfLifeForResolution) * Math.Log(RandomGenerator.GetRandom()));

                if (topRatingGroup.RatingGroupAttribute.RatingEndingTimeVaries)
                {
                    newRatingGroupPhaseStatus.ActualCompleteTime = newRatingGroupPhaseStatus.EarliestCompleteTime + new TimeSpan(0, 0, secondsDelay);
                    newRatingGroupPhaseStatus.ShortTermResolveTime = newRatingGroupPhaseStatus.ActualCompleteTime;
                }
                else
                {
                    newRatingGroupPhaseStatus.ActualCompleteTime = newRatingGroupPhaseStatus.EarliestCompleteTime;
                    newRatingGroupPhaseStatus.ShortTermResolveTime = newRatingGroupPhaseStatus.ActualCompleteTime + new TimeSpan(0, 0, secondsDelay);
                }

                if (theRatingGroupPhaseStatus.HighStakesSecret && !topRatingGroup.RatingGroupAttribute.RatingEndingTimeVaries)
                {
                    HighStakesImplementPlanForNewRatingGroupPhaseStatus(topRatingGroup, newRatingGroupPhaseStatus, theRatingGroupPhaseStatus);
                }
                else
                {   // Determine whether to make a high stakes rating phase.
                    HighStakesRandomize(topRatingGroup, newRatingGroupPhaseStatus);
                }
            }

            return true;
        }

        
    }
}
