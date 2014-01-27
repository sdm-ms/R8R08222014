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
using ClassLibrary1.Model;
using ClassLibrary1.Misc;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {



        public int AddPointsManagerNewSettings(int pointsManagerID, decimal? currentPeriodDollarSubsidy, DateTime? endOfDollarSubsidyPeriod, decimal? nextPeriodDollarSubsidy, int? nextPeriodLength, short? numPrizes, decimal? minimumPayment)
        {
            // Duplicate the original universe object, making changes as necessary.
            PointsManager originalPointsManager = DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == pointsManagerID);
            int newPointsManagerID = AddPointsManager(originalPointsManager.DomainID, originalPointsManager.Name, originalPointsManager.Creator);
            PointsManager newPointsManager = DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == newPointsManagerID);


            if (currentPeriodDollarSubsidy != null)
                newPointsManager.CurrentPeriodDollarSubsidy = (decimal)currentPeriodDollarSubsidy;
            if (endOfDollarSubsidyPeriod != null)
                newPointsManager.EndOfDollarSubsidyPeriod = (DateTime)endOfDollarSubsidyPeriod;
            if (nextPeriodDollarSubsidy != null)
                newPointsManager.NextPeriodDollarSubsidy = (decimal)nextPeriodDollarSubsidy;
            if (nextPeriodLength != null)
                newPointsManager.NextPeriodLength = (int)nextPeriodLength;
            if (numPrizes != null)
                newPointsManager.NumPrizes = (short)numPrizes;
            if (minimumPayment != null)
                newPointsManager.MinimumPayment = (decimal)minimumPayment;
            newPointsManager.TotalUserPoints = originalPointsManager.TotalUserPoints;
            newPointsManager.CurrentUserPoints = originalPointsManager.CurrentUserPoints;
            newPointsManager.Status = (Byte)StatusOfObject.Proposed;
            DataContext.SubmitChanges();
            PMCacheManagement.InvalidateCacheDependency("DomainID" + newPointsManager.DomainID);

            return newPointsManager.PointsManagerID;
        }

        // Methods related to executing changes

        /// <summary>
        /// Check on any change groups that may be linked to the specified rating, updating them if necessary.
        /// </summary>
        /// <param name="theRating">The rating (which may or may not be linked to changes groups)</param>
        public void CheckChangeGroupsLinkedToRating(Rating theRating)
        {
            // Currently disabled, because we're implementing all changes right away.
            //var theChangesGroups = theRating.ChangesGroups;
            //foreach (ChangesGroup theChangesGroup in theChangesGroups)
            //{
            //    UpdateChangesGroupStatus(theChangesGroup);
            //    CheckChangesGroup(theChangesGroup);
            //}
        }

        /// <summary>
        /// Get the proposal settings corresponding to the Tbl or universe that a ChangesGroup
        /// may effect.
        /// </summary>
        /// <param name="theGroup"></param>
        /// <returns></returns>
        public ProposalSetting GetProposalSettingsForChangesGroup(ChangesGroup theGroup)
        {
            ProposalSetting theSettings = null;
            if (theGroup.TblID != null)
                theSettings = GetProposalSettingForTbl((int)theGroup.TblID);//GetTbl((int)theGroup.TblID).ProposalSetting;
            if (theSettings == null)
                theSettings = GetProposalSettingForPointsManager((int)theGroup.PointsManagerID);//GetPointsManager((int)theGroup.PointsManagerID).ProposalSetting;
            if (theSettings == null)
                throw new Exception("Internal error -- proposal settings undefined for ChangeGroup " + theGroup.ChangesGroupID.ToString());
            return theSettings;
        }
        /// <summary>
        /// Get the proposal setting corresponding to the universe
        /// </summary>
        /// <param name="PointsManagerID"></param>
        /// <returns></returns>
        public ProposalSetting GetProposalSettingForPointsManager(int PointsManagerID)
        {
            ProposalSetting TheSetting = null;
            var ProposalSettings = DataContext.GetTable<ProposalSetting>().Where(m => m.PointsManagerID == PointsManagerID && m.Status == Convert.ToByte(StatusOfObject.Active));
            if (ProposalSettings.Count() == 1)
            {
                TheSetting = (ProposalSetting)ProposalSettings;
            }
            if (ProposalSettings.Count() == 0)
            {
                throw new Exception("Internal error--Proposal setting does not exist for the universe");
            }
            if (ProposalSettings.Count() > 1)
            {
                throw new Exception("Internal error--more than one proposal settings exist for universe");
            }
            return TheSetting;
        }
        /// <summary>
        /// Get the proposal setting corresponding to the Tbl
        /// </summary>
        /// <param name="TblID"></param>
        /// <returns></returns>
        public ProposalSetting GetProposalSettingForTbl(int TblID)
        {
            ProposalSetting TheSetting = null;
            var ProposalSettings = DataContext.GetTable<ProposalSetting>().Where(m => m.TblID == TblID && m.Status == Convert.ToByte(StatusOfObject.Active));
            if (ProposalSettings.Count() == 1)
            {
                TheSetting = (ProposalSetting)ProposalSettings;
            }
            if (ProposalSettings.Count() == 0)
            {
                throw new Exception("Internal error--Proposal setting does not exist for the Tbl");
            }
            if (ProposalSettings.Count() > 1)
            {
                throw new Exception("Internal error--more than one proposal settings exist for Tbl");
            }
            return TheSetting;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changesStatusOfObjectID"></param>
        /// <param name="changesResolveRatingOrGroupID"></param>
        public void UpdateChangesGroupMembershipInfo(int? changesStatusOfObjectID)
        {

            
            int? objectID = null;
            TypeOfObject theObjectType;
            int? TblID = null;
            int? pointsManagerID = null;
            ChangesGroup theChangesGroup = null;

            if (changesStatusOfObjectID != null)
            {
                ChangesStatusOfObject theChange = DataContext.GetTable<ChangesStatusOfObject>().Single(x=>x.ChangesStatusOfObjectID ==(int)changesStatusOfObjectID);
                theChangesGroup = theChange.ChangesGroup;
                if (theChange.AddObject || theChange.ReplaceObject)
                    objectID = theChange.NewObject;
                else if (theChange.DeleteObject || theChange.ChangeOther || theChange.ChangeName)
                    objectID = theChange.ExistingObject;
                theObjectType = (TypeOfObject)theChange.ObjectType;
                GetTblAndPointsManagerForObject(objectID, theObjectType, out TblID, out pointsManagerID);
            }
            else throw new Exception("Internal error -- must specify changesStatusOfObjectID in UpdateChangesGroupMembershipInfo.");

            if (TblID != null)
            {
                if (theChangesGroup.TblID != null && theChangesGroup.TblID != TblID)
                    throw new Exception("Internal error -- can't group multiple changes across different Tbls.");
                theChangesGroup.TblID = TblID;
            }
            if (pointsManagerID != null)
            {
                if (theChangesGroup.PointsManagerID != null && theChangesGroup.PointsManagerID != pointsManagerID)
                    throw new Exception("Internal error -- can't group multiple changes across different universes.");
                theChangesGroup.PointsManagerID = pointsManagerID;
            }
        }
        /// <summary>
        /// Updates the changes group status for proposed changes, determining whether it is on track
        /// for acceptance or rejection, and when acceptance or rejection should occur if so.
        /// </summary>
        /// <param name="theGroup">The changes group. No changes will be made if the status is not proposed/accepted/rejected</param>
        //public void UpdateChangesGroupStatus(ChangesGroup theGroup)
        //{
        //    if (theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackAccept ||
        //            theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackReject ||
        //            theGroup.StatusOfChanges == (Byte)StatusOfChanges.Proposed)
        //    { // We need to see if this change group needs updating
        //        ProposalSetting theSettings = GetProposalSettingsForChangesGroup(theGroup);

        //        decimal? currentUserRating = theGroup.Rating.CurrentValue;

        //        if (currentUserRating == null)
        //        {
        //            theGroup.StatusOfChanges = (Byte)StatusOfChanges.Proposed;
        //            theGroup.ScheduleApprovalOrRejection = null;
        //        }
        //        else if ((decimal)currentUserRating > theSettings.MaxValueToReject && (decimal)currentUserRating < theSettings.MinValueToApprove)
        //        { // Change group can't be on track if its prediction is in between the thresholds
        //            theGroup.StatusOfChanges = (Byte)StatusOfChanges.Proposed;
        //            theGroup.ScheduleApprovalOrRejection = null;
        //        }
        //        else
        //        { // We're on track for something
        //            if ((theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackAccept && (decimal)currentUserRating > theSettings.MinValueToApprove)
        //                || (theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackReject && (decimal)currentUserRating < theSettings.MaxValueToReject))
        //                return; // nothing could have changed

        //            // Let's figure out what we're on track for
        //            if ((decimal)currentUserRating < theSettings.MaxValueToReject)
        //                theGroup.StatusOfChanges = (Byte)StatusOfChanges.OnTrackReject;
        //            else
        //                theGroup.StatusOfChanges = (Byte)StatusOfChanges.OnTrackAccept;

        //            // And now we must figure out the time at which being on track will be realized.
        //            DateTime currentTime = TestableDateTime.Now;
        //            DateTime beginningOfPeriod = TestableDateTime.Now - new TimeSpan(0, 0, theSettings.MinTimePastThreshold);
        //            var predictionsInTimePeriod = RaterooDB.GetTable<UserRating>().Where(p => p.RatingID == theGroup.MakeChangeRatingID
        //                                                               && p.UserRatingGroup.WhenMade >= beginningOfPeriod)
        //                                                        .Select(p => new
        //                                                        {
        //                                                            PreviousUserRating = p.PreviousUserRating,
        //                                                            NewUserRating = p.NewUserRating,
        //                                                            WhenMade = p.UserRatingGroup.WhenMade
        //                                                        })
        //                                                        .OrderBy(p => p.WhenMade);


        //            int totalUserRatingsInTimePeriod = predictionsInTimePeriod.Count() + 1; // include prediction pending at start of time period

        //            // Calculate different pieces of information for all predictions (including prediction pending at start of time period)
        //            bool[] predictionOnTrack = new bool[totalUserRatingsInTimePeriod]; // is this prediction on track in the same direction as the current prediction?
        //            TimeSpan[] predictionDuration = new TimeSpan[totalUserRatingsInTimePeriod]; // duration of this prediction alone
        //            TimeSpan[] predictionToCurrentDuration = new TimeSpan[totalUserRatingsInTimePeriod]; // duration of this prediction and subsequent ones
        //            TimeSpan[] predictionToCurrentOnTrackDuration = new TimeSpan[totalUserRatingsInTimePeriod]; // duration of this prediction and later ones, but only when they're on track


        //            // Determine whether each prediction is on track and the duration of the prediction itself.
        //            for (int i = 0; i < totalUserRatingsInTimePeriod; i++)
        //            {
        //                decimal? theUserRating = 0;
        //                if (i == 0)
        //                {   // Use prediction pending at beginning of time period, measured from that point to the time of the next prediction
        //                    theUserRating = predictionsInTimePeriod.First().PreviousUserRating;
        //                    predictionDuration[i] = predictionsInTimePeriod.First().WhenMade - beginningOfPeriod;
        //                }
        //                else
        //                {   // Use prediction actually made.
        //                    theUserRating = predictionsInTimePeriod.Skip(i - 1).First().NewUserRating;
        //                    DateTime thisUserRatingTime, nextUserRatingTime;
        //                    thisUserRatingTime = predictionsInTimePeriod.Skip(i - 1).First().WhenMade;
        //                    if (i < totalUserRatingsInTimePeriod - 1)
        //                        nextUserRatingTime = predictionsInTimePeriod.Skip(i).First().WhenMade;
        //                    else
        //                        nextUserRatingTime = currentTime;
        //                    predictionDuration[i] = nextUserRatingTime - thisUserRatingTime;
        //                }
        //                predictionOnTrack[i] = ((theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackReject && theUserRating < theSettings.MaxValueToReject)
        //                    || (theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackAccept && theUserRating > theSettings.MinValueToApprove));
        //            }

        //            // Now, calculate the duration between the prediction and now, and the amount of that duration that is on track.
        //            TimeSpan cumDurationToCurrent = new TimeSpan(0, 0, 0);
        //            TimeSpan cumOnTrackDurationToCurrent = new TimeSpan(0, 0, 0);
        //            for (int i = totalUserRatingsInTimePeriod - 1; i >= 0; i--)
        //            {
        //                cumDurationToCurrent += predictionDuration[i];
        //                if (predictionOnTrack[i])
        //                    cumOnTrackDurationToCurrent += predictionDuration[i];
        //                predictionToCurrentDuration[i] = cumDurationToCurrent;
        //                predictionToCurrentOnTrackDuration[i] = cumOnTrackDurationToCurrent;

        //            }

        //            // For each on track prediction, figure out the earliest we would need to meet the criteria.
        //            // So, if we're currently below the minimum proportion, (ontrack + timeNeeded) / (totalduration + timeNeeded) >= minproportion.
        //            // It follows timeNeeded = (ontrack - minproportion*totalduration) / (minproportion - 1).
        //            // But timeNeeded >= mintimeneeded - totalduration.
        //            TimeSpan[] timeNeededToMeetThresholdFromUserRating = new TimeSpan[totalUserRatingsInTimePeriod]; // how much more time is needed to meet the threshold requirements
        //            DateTime earliestTime = currentTime; // this will be overriden but need to assign to something
        //            for (int i = 0; i < totalUserRatingsInTimePeriod; i++)
        //            {
        //                if (predictionToCurrentOnTrackDuration[i].TotalSeconds / predictionToCurrentDuration[i].TotalSeconds >= (double)theSettings.MinProportionOfThisTime)
        //                    timeNeededToMeetThresholdFromUserRating[i] = new TimeSpan(0, 0, theSettings.MinTimePastThreshold) - predictionToCurrentDuration[i];
        //                else
        //                    timeNeededToMeetThresholdFromUserRating[i] = new TimeSpan(0, 0, (int)Math.Max((predictionToCurrentOnTrackDuration[i].TotalSeconds - (double)theSettings.MinProportionOfThisTime * predictionToCurrentDuration[i].TotalSeconds) / (double)(theSettings.MinProportionOfThisTime - 1), theSettings.MinTimePastThreshold - predictionToCurrentDuration[i].TotalSeconds));
        //                if (i == 0)
        //                    earliestTime = currentTime + timeNeededToMeetThresholdFromUserRating[i];
        //                else
        //                {
        //                    DateTime newTime = currentTime + timeNeededToMeetThresholdFromUserRating[i];
        //                    if (newTime < earliestTime)
        //                        earliestTime = newTime;
        //                }
        //            }
        //            theGroup.ScheduleApprovalOrRejection = earliestTime;

        //        } // we're on track region
        //        RaterooDB.SubmitChanges();


        //    }
        //}

        ///// <summary>
        ///// Check a changes groups, to see if it is time to implement or to accept/reject the changes.
        ///// Note that the status of the changes group is not changed by this procedure (unless the time
        ///// specified in the UpdateChangesGroupStatus procedure has been reached).
        ///// </summary>
        ///// <param name="theGroup">The changes group.</param>
        //public void CheckChangesGroup(ChangesGroup theGroup)
        //{
        //    if (theGroup.ScheduleImplementation != null)
        //    {
        //        if (theGroup.ScheduleImplementation < TestableDateTime.Now)
        //            ImplementChangesGroup(theGroup);
        //    }
        //    else if (theGroup.ScheduleApprovalOrRejection != null)
        //    {
        //        if (theGroup.ScheduleApprovalOrRejection < TestableDateTime.Now)
        //        {
        //            if (theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackAccept)
        //                AcceptChangesGroup(theGroup);
        //            else if (theGroup.StatusOfChanges == (Byte)StatusOfChanges.OnTrackReject)
        //                RejectChangesGroup(theGroup);
        //            else
        //                throw new Exception("Internal error -- ChangesGroup scheduled but not on track");
        //        }
        //    }
        //}

        /// <summary>
        /// Find a changes groups that need implementing or acceptance or rejection
        /// </summary>
        //public bool IdleTaskCheckAllChangesGroups()
        //{
        //    throw new Exception("IdleTaskCheckAllChangesGroups not currently implemented.");

        //    bool didWork = false;

        //    ChangesGroup theGroup = RaterooDB.GetTable<ChangesGroup>().Where(g => g.ScheduleImplementation != null)
        //                                            .FirstOrDefault(g => g.ScheduleImplementation < TestableDateTime.Now);
        //    if (theGroup != null)
        //    {
        //        ImplementChangesGroup(theGroup);
        //        didWork = true;
        //    }

        //    theGroup = RaterooDB.GetTable<ChangesGroup>().Where(g => g.ScheduleApprovalOrRejection != null)
        //                                            .FirstOrDefault(g => g.ScheduleApprovalOrRejection < TestableDateTime.Now);
        //    if (theGroup != null)
        //    {
        //        CheckChangesGroup(theGroup);
        //    }

        //    return didWork;
        //}

        /// <summary>
        /// Accept a changes group, scheduling it for implementation if appropriate
        /// </summary>
        /// <param name="theGroup"></param>
        public void AcceptChangesGroup(ChangesGroup theGroup)
        {
            theGroup.StatusOfChanges = (Byte)StatusOfChanges.Accepted;
            theGroup.ScheduleImplementation = null;
            theGroup.ScheduleApprovalOrRejection = null;
            ProposalSetting theSettings = GetProposalSettingsForChangesGroup(theGroup);
            bool implementImmediately = true;
            if (theSettings.HalfLifeForResolvingAtFinalValue > 0)
            {
                // See if this change group includes a change to a rating, in which case we'll schedule it.
                int directResolution = 0; // This feature currently disabled. To enable, change to: RaterooDB.GetTable<ChangesResolveRatingOrGroup>().Where(c => c.ChangeGroupID == theGroup.ChangesGroupID).Count();
                if (directResolution > 0)
                    implementImmediately = false;
                else
                {
                    // If a change may affect running ratings, then also don't implement it right away.
                    int indirectResolution = DataContext.GetTable<ChangesStatusOfObject>().Where(c => c.ChangesGroupID == theGroup.ChangesGroupID && c.MayAffectRunningRating == true).Count();
                    if (indirectResolution > 0)
                        implementImmediately = false;
                }
            };
            if (implementImmediately)
                ImplementChangesGroup(theGroup);
            else
            {
                int secondsToResolution = (int)((0-theSettings.HalfLifeForResolvingAtFinalValue) * Math.Log(RandomGenerator.GetRandom()));
                theGroup.ScheduleImplementation = TestableDateTime.Now + new TimeSpan(0, 0, secondsToResolution);
            }
            // Add scheduling here if applicable or implement immediately
            DataContext.SubmitChanges();

        }

        /// <summary>
        /// Reject a changes group
        /// </summary>
        /// <param name="theGroup">The group to reject</param>
        public void RejectChangesGroup(ChangesGroup theGroup)
        {
            theGroup.StatusOfChanges = (Byte)StatusOfChanges.Rejected;
            theGroup.ScheduleApprovalOrRejection = null;
            theGroup.ScheduleImplementation = null;
            DataContext.SubmitChanges();
        }

        /// <summary>
        /// Implement a changes group. We wrap the changes in a transaction, so that the changes either all succeed
        /// or all fail.
        /// </summary>
        /// <param name="changesGroupID">The changes group (as an id)</param>
        public void ImplementChangesGroup(int changesGroupID)
        {

            
            ImplementChangesGroup(DataContext.GetTable<ChangesGroup>().Single(x=>x.ChangesGroupID ==changesGroupID));
        }

        /// <summary>
        /// Implement a changes group. We wrap the changes in a transaction, so that the changes either all succeed
        /// or all fail.
        /// </summary>
        /// <param name="theGroup">The changes group (as an object)</param>
        public void ImplementChangesGroup(ChangesGroup theGroup)
        {
            bool debugMode = false; // if debugging, don't wrap in transaction (so that we can see error more easily)

            if (debugMode)
            {
                // We make a list of all the objects since the datacontext might change in ImplementChangeStatusOfObject,
                // and therefore we can't just enumerate in the usual way.
                var theStatusChanges = DataContext.GetTable<ChangesStatusOfObject>().Where(csoo => csoo.ChangesGroupID == theGroup.ChangesGroupID).ToList();
                foreach (ChangesStatusOfObject theChange in theStatusChanges)
                    ImplementChangeStatusOfObject(theChange);


                theGroup.StatusOfChanges = (Byte)StatusOfChanges.Implemented;
                theGroup.ScheduleApprovalOrRejection = null;
                theGroup.ScheduleImplementation = null;
                DataContext.SubmitChanges();
            }
            else
            {
                try
                {
                    int numTries = 0;
                    TryLabel:
                    try
                    {
                        numTries++;
                        DataContext.SubmitChanges();
                        ResetDataContexts();
                        var theStatusChanges = DataContext.GetTable<ChangesStatusOfObject>().Where(csoo => csoo.ChangesGroupID == theGroup.ChangesGroupID).ToList();
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 10, 0)))
                        {

                            foreach (ChangesStatusOfObject theChange in theStatusChanges)
                            {
                                ImplementChangeStatusOfObject(theChange);
                            }


                            theGroup.StatusOfChanges = (Byte)StatusOfChanges.Implemented;
                            theGroup.ScheduleApprovalOrRejection = null;
                            theGroup.ScheduleImplementation = null;
                            DataContext.SubmitChanges();
                            ts.Complete();

                        }
                    }
                    catch (System.Data.Linq.ChangeConflictException)
                    {
                        if (numTries < 5)
                        {
                            System.Diagnostics.Trace.TraceError("Change conflict exception on changes group try " + numTries);
                            System.Threading.Thread.Sleep(4000); // wait one second
                            goto TryLabel;
                        }
                        else throw new Exception("Change conflict exception raised too many times.");
                    }
                }
                catch (Exception e)
                {
                    theGroup.StatusOfChanges = (Byte)StatusOfChanges.Failed;
                    DataContext.SubmitChanges();
                    String theExceptionString = "An error occurred while attempting to implement proposed changes";
                    if (e.InnerException != null)
                        theExceptionString += ": " + e.InnerException.ToString();
                    throw new Exception(theExceptionString);
                }
            }

            DataContext.SubmitChanges();
            DeleteChangesGroup(theGroup.ChangesGroupID);
        }

        /// <summary>
        /// Checks a change group ID and determines whether that corresponds to a valid change group
        /// that has not yet been proposed (and thus can be augmented).
        /// </summary>
        /// <param name="changeGroupID"></param>
        /// <returns></returns>
        public bool ChangesGroupCanBeAugmented(int changesGroupID)
        {
            var theChangesGroups = DataContext.GetTable<ChangesGroup>().Where(cg => cg.ChangesGroupID == changesGroupID);
            if (theChangesGroups.Count() != 1)
                return false;
            return theChangesGroups.First().StatusOfChanges == (Byte)StatusOfChanges.NotYetProposed;
        }

        /// <summary>
        /// Determines whether a change can be made.
        /// </summary>
        /// <param name="changesGroupID">The changesGroupID, or null for an immediate change, in which case a new
        /// changes group will be created for the change (assuming this isn't a proposal)</param>
        /// <param name="userID">The user creating the change</param>
        /// <param name="theAction">The action to the database</param>
        /// <param name="proposalOnly">True if this is only a proposal </param>
        /// <param name="pointsManagerID">The universe to which the changes will apply, or null (e.g., for changes to domain)</param>
        /// <param name="TblID">The Tbl to which the changes will apply, or null (e.g., for changes to universe)</param>
        /// <returns>True if we can proceed with the change, based on the users' rights and the status of the changes group</returns>
        public bool ProceedWithChange(ref int? changesGroupID, int? userID, UserActionOldList theAction, bool proposalOnly, int? pointsManagerID, int? TblID, bool createChangesGroupIfNoneProvided)
        {
            Tbl theTbl = null;
            string key = "Tbl" + TblID;
            if (TblID != null)
            {
                theTbl = DataContext.TempCacheGet(key) as Tbl;
                if (theTbl == null)
                {
                    theTbl = DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID);
                    DataContext.TempCacheAdd(key, theTbl);
                }
            }
            bool userRightsOK = ObjDataAccess.CheckUserRights(userID, theAction, proposalOnly, pointsManagerID, TblID);
            if (!userRightsOK)
                return false;

            if (TblID != null && theAction != UserActionOldList.Predict && theAction != UserActionOldList.View)
            {
                if (theTbl.Name == "Changes")
                {
                    User theUser = DataContext.GetTable<User>().SingleOrDefault(u => u.UserID == userID);
                    if (theUser == null || theUser.SuperUser == false)
                        return false;
                }
            }

            if (!createChangesGroupIfNoneProvided)
                return true;

            if (changesGroupID == null)
            {
                //Will be Uncommented later
                //if (!proposalOnly)
                //{
                //    if (!ObjDataAccess.GetUser((int)userID).SuperUser)
                //        return false;
                //}
                changesGroupID = AddChangesGroup(pointsManagerID, TblID, userID, null, null, StatusOfChanges.NotYetProposed, null, null);
            }

            if (changesGroupID == null)
                throw new Exception("Internal error -- changes group should have been created but wasn't.");

            return ChangesGroupCanBeAugmented((int)changesGroupID);
        }


        /// <summary>
        /// Implement change status of object
        /// </summary>
        /// <param name="theChange"></param>
        public void ImplementChangeStatusOfObject(ChangesStatusOfObject theChange)
        {

           
            bool changeMade = false;
            if (theChange.AddObject)
            {
                if (ObjectTypeIsNamed((TypeOfObject)theChange.ObjectType))
                {
                    // We must make sure to avoid duplicatively named objects where those are prohibited.
                    // When we originally add the proposed object, we attach a name to it. Now, we make sure that
                    // name is unique, and if not, we change the name.
                    string theName = "";
                    int? theCreator = null;
                    GetNameAndCreatorOfObject((int)theChange.NewObject, (TypeOfObject)theChange.ObjectType, ref theName, ref theCreator);
                    ChangeNameOfObject((int)theChange.NewObject, (TypeOfObject)theChange.ObjectType, theCreator, theName);
                }
                SetStatusOfObject((int)theChange.NewObject, (TypeOfObject)theChange.ObjectType, StatusOfObject.Active);
                changeMade = true;

                switch ((TypeOfObject)theChange.ObjectType)
                {
                    case TypeOfObject.TblColumn:
                        AddRatingsAfterAddingCategoryIfTblIsActive((int)theChange.NewObject);
                        break;
                    case TypeOfObject.RatingGroupResolution:
                        break;
                    case TypeOfObject.PointsAdjustment:
                        PointsAdjustment thePointsAdjustment = DataContext.GetTable<PointsAdjustment>().Single(x=>x.PointsAdjustmentID ==(int)theChange.NewObject);
                        UpdateUserPointsAndStatus(thePointsAdjustment.UserID, thePointsAdjustment.PointsManagerID, PointsChangesReasons.AdministrativeChange, thePointsAdjustment.TotalAdjustment, thePointsAdjustment.CurrentAdjustment, 0, 0, 0, 0, true);
                        break;
                    case TypeOfObject.PointsManager:
                        SetTradingStatus((int)theChange.NewObject, TypeOfObject.PointsManager, TradingStatus.Active);
                        break;
                    default:
                        break;
                }
            }
            if (theChange.ChangeName)
            {
                if (theChange.ExistingObject == null)
                    throw new Exception("Internal error -- trying to change name of null object.");
                ChangeNameOfObject((int)theChange.ExistingObject, (TypeOfObject)theChange.ObjectType, theChange.ChangesGroup.Creator, theChange.NewName);
                changeMade = true;
            }
            if (theChange.ChangeOther)
            {
                if (theChange.ExistingObject == null)
                    throw new Exception("Internal error -- trying to change attribute of null object.");
                switch ((TypeOfObject)theChange.ObjectType)
                {
                    case TypeOfObject.TblColumn:
                        if (theChange.ChangeSetting1 && !theChange.ChangeSetting2)
                        {
                            if (theChange.NewValueText.Length > 10)
                                throw new Exception("The abbreviation must be no longer than 10 characters.");
                            var theTblColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == (int)theChange.ExistingObject);
                            theTblColumn.Abbreviation = theChange.NewValueText;
                            PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theTblColumn.TblTab.TblID);
                            SQLFastAccess.PlanDropTbl(DataContext, theTblColumn.TblTab.Tbl);
                            changeMade = true;
                        }
                        else if (theChange.ChangeSetting1 && theChange.ChangeSetting2)
                        {     
                            if (theChange.NewValueText.Length > 50)
                                throw new Exception("The abbreviation must be no longer than 50 characters.");
                            var theTblColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == (int)theChange.ExistingObject);
                            theTblColumn.Name = theChange.NewValueText;
                            PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theTblColumn.TblTab.TblID);
                            SQLFastAccess.PlanDropTbl(DataContext, theTblColumn.TblTab.Tbl);
                            changeMade = true;
                        }
                        else if (!theChange.ChangeSetting1 && !theChange.ChangeSetting2)
                        {
                            var theTblColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == (int)theChange.ExistingObject);
                            theTblColumn.Explanation = theChange.NewValueText;
                            PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theTblColumn.TblTab.TblID);
                            SQLFastAccess.PlanDropTbl(DataContext, theTblColumn.TblTab.Tbl);
                            changeMade = true;
                        }

                        else if (!theChange.ChangeSetting1 && theChange.ChangeSetting2)
                        {
                            var theTblColumn = DataContext.GetTable<TblColumn>().Single(cd => cd.TblColumnID == theChange.ExistingObject);
                            PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theTblColumn.TblTab.TblID);
                            SQLFastAccess.PlanDropTbl(DataContext, theTblColumn.TblTab.Tbl);
                            bool useAsFilter = false, sortable = false, defaultSortOrderAscending = false;
                            int theNewValue = (int)theChange.NewValueInteger;
                            if (theNewValue >= 4)
                            {
                                defaultSortOrderAscending = true;
                                theNewValue -= 4;
                            }
                            if (theNewValue >= 2)
                            {
                                sortable = true;
                                theNewValue -= 2;
                            }
                            if (theNewValue >= 1)
                            {
                                useAsFilter = true;
                                theNewValue -= 1;
                            }
                            ChangeTblColumnSortOptions((int)theChange.ExistingObject, useAsFilter, sortable, defaultSortOrderAscending);

                            changeMade = true;
                        }
                        break;
                    case TypeOfObject.TblTab:
                        ChangeTblTabDefaultSort((int)theChange.ExistingObject, theChange.NewValueInteger);
                        var theTblTab = DataContext.GetTable<TblTab>().Single(cg => cg.TblTabID == theChange.ExistingObject);
                        PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theTblTab.TblID);
                        SQLFastAccess.PlanDropTbl(DataContext, theTblTab.Tbl);
                        changeMade = true;
                        break;
                    case TypeOfObject.ChoiceInGroup:
                        ChoiceInGroup theChoiceInGroup = DataContext.GetTable<ChoiceInGroup>().Single(x=>x.ChoiceInGroupID==(int)theChange.ExistingObject);
                        if (theChange.ChangeSetting1 && !theChange.ChangeSetting2)
                        {
                            if (theChange.NewValueInteger == null)
                                throw new Exception("The choice number cannot be null.");
                            theChoiceInGroup.ChoiceNum = (int)theChange.NewValueInteger;
                        }
                        else if (!theChange.ChangeSetting1 && theChange.ChangeSetting2)
                            theChoiceInGroup.ActiveOnDeterminingGroupChoiceInGroupID = theChange.NewValueInteger;
                        else if (theChange.ChangeSetting1 && theChange.ChangeSetting2)
                        {
                            theChoiceInGroup.ChoiceText = theChange.NewValueText;
                            var entities = theChoiceInGroup.ChoiceInFields.Select(x => x.ChoiceField).Select(x => x.Field).Select(x => x.TblRow).Distinct().ToList();
                            foreach (var entity in entities)
                                ResetTblRowFieldDisplay(entity);
                            SetSearchWordsForChoiceInGroup(theChoiceInGroup, false);
                        }
                        else if (!theChange.ChangeSetting1 && !theChange.ChangeSetting2)
                        {
                            if (theChange.NewValueBoolean == null)
                                throw new Exception("The availability status cannot be changed to null.");
                            if ((bool)theChange.NewValueBoolean)
                                theChoiceInGroup.Status = (Byte)StatusOfObject.Active;
                            else
                                theChoiceInGroup.Status = (Byte)StatusOfObject.Unavailable;
                        }
                        PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + theChoiceInGroup.ChoiceGroup.PointsManagerID);
                        SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == theChoiceInGroup.ChoiceGroup.PointsManagerID));
                        changeMade = true;
                        break;

                    case TypeOfObject.Tbl:
                        if (theChange.NewValueText == "TblDimension")
                        {
                            DataContext.GetTable<Tbl>().Single(x => x.TblID == (int)theChange.ExistingObject).TblDimensionsID = theChange.NewValueInteger;
                            changeMade = true;
                        }
                        else if (theChange.NewValueText == "ChangeCommentSetting")
                        {
                            bool AllowUsersToAddComment = false, LimitComments = false;
                            int NewValue = (int)theChange.NewValueInteger;
                            if (NewValue >= 2)
                            {
                                LimitComments = true;
                                NewValue -= 2;
                            }
                            if (NewValue >= 1)
                            {
                                AllowUsersToAddComment = true;
                                NewValue -= 1;
                            }
                            DataContext.GetTable<Tbl>().Single(x => x.TblID == (int)theChange.ExistingObject).AllowUsersToAddComments = AllowUsersToAddComment;
                            DataContext.GetTable<Tbl>().Single(x => x.TblID == (int)theChange.ExistingObject).LimitCommentsToUsersWhoCanMakeUserRatings = LimitComments;
                            changeMade = true;
                        }
                        else if (theChange.NewValueText != ""  && theChange.ChangeSetting1 == true && theChange.ChangeSetting2 == false)
                        {
                            Tbl theTbl = DataContext.GetTable<Tbl>().Single(x => x.TblID == (int)theChange.ExistingObject);
                            theTbl.TblTabWord = theChange.NewValueText;
                            PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theChange.ExistingObject);
                            SQLFastAccess.PlanDropTbl(DataContext, theTbl);
                            changeMade = true;
                        }
                        else if (theChange.NewValueText != "" && theChange.ChangeSetting1 == false && theChange.ChangeSetting2 == true)
                        {
                            DataContext.GetTable<Tbl>().Single(x => x.TblID == (int)theChange.ExistingObject).TypeOfTblRow = theChange.NewValueText;
                            changeMade = true;
                        }
                        else if (theChange.NewValueText != "" && theChange.ChangeSetting1 == true && theChange.ChangeSetting2 == true)
                        {
                            try
                            {
                                string[] theStrings = theChange.NewValueText.Split('&');
                                Tbl theTbl = DataContext.GetTable<Tbl>().Single(x => x.TblID == (int)theChange.ExistingObject);
                                theTbl.SuppStylesHeader = theStrings[0];
                                theTbl.SuppStylesMain = theStrings[1];
                                PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theChange.ExistingObject);
                                SQLFastAccess.PlanDropTbl(DataContext, theTbl);
                            }
                            catch
                            {
                            }
                            changeMade = true;
                        }                            
                        //else
                        //{
                        //    SetTradingStatus((int)theChange.ExistingObject, TypeOfObject.Tbl, TradingStatus.Active);
                        //    changeMade = true;
                        //}
                        break;

                    case TypeOfObject.Domain:
                        if (theChange.ChangeSetting1)
                        {
                            bool activeRatingWebsite = false;
                            int theNewValue = (int)theChange.NewValueInteger;
                            if (theNewValue >= 4)
                            {
                                //activeBuyingWebsite = true;
                                //theNewValue -= 4;
                            }
                            if (theNewValue >= 2)
                            {
                                activeRatingWebsite = true;
                                theNewValue -= 2;
                            }
                            if (theNewValue >= 1)
                            {
                                //activeUserRatingWebsite = true;
                                //theNewValue -= 1;
                            }
                            Domain theDomain = DataContext.GetTable<Domain>().Single(x => x.DomainID == (int)theChange.ExistingObject);
                            theDomain.ActiveRatingWebsite = activeRatingWebsite;
                            PMCacheManagement.InvalidateCacheDependency("DomainID" + theDomain.DomainID);

                            changeMade = true;
                        }
                        else if (theChange.ChangeSetting2)
                        {
                            PMCacheManagement.InvalidateCacheDependency("DomainID" + theChange.ExistingObject);
                            if (theChange.NewValueText == "TblDimension")
                            {
                                DataContext.GetTable<Domain>().Single(x => x.DomainID == (int)theChange.ExistingObject).TblDimensionsID = theChange.NewValueInteger;
                                changeMade = true;
                            }
                        }
                        break;
                    case TypeOfObject.InsertableContent:
                        PMCacheManagement.InvalidateCacheDependency("InsertableContent");
                        changeMade = true;
                        break;
                    case TypeOfObject.FieldDefinition:
                        if (theChange.ChangeSetting1)
                        {
                            var theFieldDefinition = DataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == (int)theChange.ExistingObject);
                            if (theChange.NewValueText != "")
                            {
                                theFieldDefinition.FieldName = theChange.NewValueText;
                                ResetTblRowFieldDisplay(theFieldDefinition.Tbl);
                                changeMade = true;
                            }
                            else if (theChange.NewValueBoolean != null)
                            {
                                theFieldDefinition.UseAsFilter = (bool)theChange.NewValueBoolean;
                                changeMade = true;
                            }
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + theFieldDefinition.Tbl.PointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == theFieldDefinition.Tbl.PointsManagerID));
                        }
                        else if (theChange.ChangeSetting2)
                        {
                            FieldDefinition theFieldDefinition = DataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == theChange.ExistingObject);
                            var thePointsManagerID = theFieldDefinition.Tbl.PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + thePointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == thePointsManagerID));
                            if (theChange.NewValueText == "DisplayInTableSetting")
                            {
                                DataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == (int)theChange.ExistingObject).DisplayInTableSettings = (int)theChange.NewValueInteger;
                                ResetTblRowFieldDisplay(theFieldDefinition.Tbl);
                                changeMade = true;
                            }
                            if (theChange.NewValueText == "DisplayInPopUpSetting")
                            {
                                DataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == (int)theChange.ExistingObject).DisplayInPopupSettings = (int)theChange.NewValueInteger;
                                ResetTblRowFieldDisplay(theFieldDefinition.Tbl);
                                changeMade = true;
                            }
                            if (theChange.NewValueText == "DisplayInTblRowPageSetting")
                            {
                                DataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == (int)theChange.ExistingObject).DisplayInTblRowPageSettings = (int)theChange.NewValueInteger;
                                ResetTblRowFieldDisplay(theFieldDefinition.Tbl);
                                changeMade = true;
                            }
                        }
                        break;

                    case TypeOfObject.User:
                        if (theChange.NewValueBoolean != null)
                        {
                            DataContext.GetTable<UserInfo>().Single(user => user.UserID == (int)theChange.ExistingObject).IsVerified = (bool)theChange.NewValueBoolean;
                            changeMade = true;
                             
                        }
                        break;

                    default:
                        throw new Exception("Internal error -- change other information for object not provided for.");
                }
                
            }
            if (theChange.DeleteObject)
            {
                if (theChange.ChangeSetting1 == true) // we're deleting
                    SetStatusOfObject((int)theChange.ExistingObject, (TypeOfObject)theChange.ObjectType, StatusOfObject.Unavailable);
                else // we're undeleting
                    SetStatusOfObject((int)theChange.ExistingObject, (TypeOfObject)theChange.ObjectType, StatusOfObject.Active);
                switch ((TypeOfObject)theChange.ObjectType)
                {
                    case TypeOfObject.AddressField:
                        {
                            int entityID = DataContext.GetTable<AddressField>().Single(a => a.AddressFieldID == theChange.ExistingObject).Field.TblRowID;
                            PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entityID);
                        }
                        break;
                    case TypeOfObject.TblColumn:
                        {
                            TblTab theTab = DataContext.GetTable<TblColumn>().Single(a => a.TblColumnID == theChange.ExistingObject).TblTab;
                            int TblID = theTab.TblID;
                            PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + TblID);
                            SQLFastAccess.PlanDropTbl(DataContext, theTab.Tbl);

                        }
                        break;
                    case TypeOfObject.TblTab:
                        {
                            TblTab theTab = DataContext.GetTable<TblColumn>().Single(a => a.TblColumnID == theChange.ExistingObject).TblTab;
                            int TblID = theTab.TblID;
                            PMCacheManagement.InvalidateCacheDependency("CategoriesForTblID" + TblID);
                            SQLFastAccess.PlanDropTbl(DataContext, theTab.Tbl);
                        }
                        break;
                    case TypeOfObject.ChoiceField:
                        {
                            int entityID = DataContext.GetTable<ChoiceField>().Single(a => a.ChoiceFieldID == theChange.ExistingObject).Field.TblRowID;
                            PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entityID);
                        }
                        break;
                    case TypeOfObject.ChoiceGroup:
                        {
                            int pointsManagerID = DataContext.GetTable<ChoiceGroup>().Single(a => a.ChoiceGroupID== theChange.ExistingObject).PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == pointsManagerID));
                        }
                        break;

                    case TypeOfObject.ChoiceGroupFieldDefinition:
                        {
                            int pointsManagerID = DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(a => a.ChoiceGroupFieldDefinitionID == theChange.ExistingObject).FieldDefinition.Tbl.PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == pointsManagerID));
                        }
                        break;
                    case TypeOfObject.ChoiceInField:
                        {
                            int entityID = DataContext.GetTable<ChoiceInField>().Single(a => a.ChoiceInFieldID == theChange.ExistingObject).ChoiceField.Field.TblRowID;
                            PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entityID);
                        }
                        break;
                    case TypeOfObject.ChoiceInGroup:
                        {
                            int pointsManagerID = DataContext.GetTable<ChoiceInGroup>().Single(a => a.ChoiceInGroupID == theChange.ExistingObject).ChoiceGroup.PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
                        }
                        break;
                    case TypeOfObject.Tbl:
                        {
                            Tbl theTbl = DataContext.GetTable<Tbl>().Single(a => a.TblID == theChange.ExistingObject);
                            PMCacheManagement.InvalidateCacheDependency("DomainID" + theTbl.PointsManager.DomainID);
                            PMCacheManagement.InvalidateCacheDependency("TopicsMenu");
                        }
                        break;
                    case TypeOfObject.Comment:
                        {
                            Comment theComment = DataContext.GetTable<Comment>().Single(a => a.CommentsID == theChange.ExistingObject);
                            PMCacheManagement.InvalidateCacheDependency("CommentForTblRowID" + theComment.TblRowID);
                        }
                        break;
                    case TypeOfObject.DateTimeField:
                        {
                            int entityID = DataContext.GetTable<DateTimeField>().Single(a => a.DateTimeFieldID == theChange.ExistingObject).Field.TblRowID;
                            PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entityID);
                        }
                        break;
                    case TypeOfObject.DateTimeFieldDefinition:
                        {
                            int pointsManagerID = DataContext.GetTable<DateTimeFieldDefinition>().Single(a => a.DateTimeFieldDefinitionID == theChange.ExistingObject).FieldDefinition.Tbl.PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == pointsManagerID));
                        }
                        break;
                    case TypeOfObject.Domain:
                        {
                            PMCacheManagement.InvalidateCacheDependency("DomainID" + theChange.ExistingObject);
                            PMCacheManagement.InvalidateCacheDependency("TopicsMenu");
                        }
                        break;
                    case TypeOfObject.TblRow:
                        {
                            int TblID = DataContext.GetTable<TblRow>().Single(a => a.TblRowID == theChange.ExistingObject).TblID;
                            PMCacheManagement.InvalidateCacheDependency("TblRowForTblID" + TblID);
                        }
                        break;
                    case TypeOfObject.Field:
                        {
                            int entityID = DataContext.GetTable<Field>().Single(a => a.FieldID == theChange.ExistingObject).TblRowID;
                            PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entityID);
                        }
                        break;
                    case TypeOfObject.FieldDefinition:
                        {
                            int pointsManagerID = DataContext.GetTable<FieldDefinition>().Single(a => a.FieldDefinitionID == theChange.ExistingObject).Tbl.PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == pointsManagerID));
                        }
                        break;
                    case TypeOfObject.NumberField:
                        {
                            int entityID = DataContext.GetTable<NumberField>().Single(a => a.NumberFieldID == theChange.ExistingObject).Field.TblRowID;
                            PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entityID);
                        }
                        break;
                    case TypeOfObject.NumberFieldDefinition:
                        {
                            int pointsManagerID = DataContext.GetTable<NumberFieldDefinition>().Single(a => a.NumberFieldDefinitionID == theChange.ExistingObject).FieldDefinition.Tbl.PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == pointsManagerID));
                        }
                        break;
                    case TypeOfObject.TextField:
                        {
                            int entityID = DataContext.GetTable<TextField>().Single(a => a.TextFieldID == theChange.ExistingObject).Field.TblRowID;
                            PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entityID);
                        }
                        break;
                    case TypeOfObject.TextFieldDefinition:
                        {
                            int pointsManagerID = DataContext.GetTable<TextFieldDefinition>().Single(a => a.TextFieldDefinitionID == theChange.ExistingObject).FieldDefinition.Tbl.PointsManagerID;
                            PMCacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + pointsManagerID);
                            SQLFastAccess.PlanDropTbls(DataContext, DataContext.GetTable<PointsManager>().Single(f => f.PointsManagerID == pointsManagerID));
                        }
                        break;
                    case TypeOfObject.PointsManager:
                        {
                            PointsManager thePointsManager = DataContext.GetTable<PointsManager>().Single(a => a.PointsManagerID == theChange.ExistingObject);
                            PMCacheManagement.InvalidateCacheDependency("DomainID" + thePointsManager.DomainID);
                            PMCacheManagement.InvalidateCacheDependency("TopicsMenu");
                        }
                        break;
                    default:
                        break;
                };
                changeMade = true;
            }
            if (theChange.ReplaceObject)
            {
                if (theChange.ObjectType == (Byte)TypeOfObject.RatingGroupAttributes)
                { // Changing rating group attributes -- maybe for a running rating, maybe not.
                    if (theChange.NewObject == null)
                        throw new Exception("Internal error -- trying to replace object from null object.");
                    // Are we just changing the default? ChangeSetting1 indicates true if this is so.
                    if (theChange.ChangeSetting1)
                    {
                        if (theChange.ChangesGroup.Tbl != null)
                            theChange.ChangesGroup.Tbl.DefaultRatingGroupAttributesID = (int)theChange.NewObject;
                        else
                            throw new Exception("Internal error: No default rating group attributes assigned when replacing object.");

                        // now, change the status of the new object
                        SetStatusOfObject((int)theChange.NewObject, (TypeOfObject)theChange.ObjectType, StatusOfObject.Active);
                        changeMade = true;
                    }
                    else
                    { // We want to restart the ratings with these default rating group attributes.
                        throw new Exception("Changing default rating group attributes of running ratings is not currently supported.");
                        //// First, stop trading and set the default attributes. Note that this will stop all trading,
                        //// even where we have an override on the default rating group attributes.
                        //if (theChange.ChangesGroup.Tbl != null)
                        //{
                        //    EndRatingsForTblAtCurrentValues((int)theChange.ChangesGroup.TblID);
                        //    theChange.ChangesGroup.Tbl.DefaultRatingGroupAttributesID = (int)theChange.NewObject;
                        //}
                        //else
                        //{
                        //    var theTbls = RaterooDB.GetTable<Tbl>().Where(c => c.PointsManagerID == theChange.ChangesGroup.PointsManagerID && c.Status == (Byte)StatusOfObject.Active && c.TradingStatus != (Byte)TradingStatus.Ended);
                        //    foreach (Tbl theTbl in theTbls)
                        //        EndRatingsForTblAtCurrentValues(theTbl.TblID);
                        //    theChange.ChangesGroup.PointsManager.DefaultRatingGroupAttributesID = (int)theChange.NewObject;
                        //}
                        //// Now, add the new default attributes.
                        //SetStatusOfObject((int)theChange.NewObject, (TypeOfObject)theChange.ObjectType, StatusOfObject.Active);
                        //// Now, restart trading.
                        //SetTradingStatusHierarchical((int)theChange.NewObject, (TypeOfObject)theChange.ObjectType, TradingStatus.Active);
                        //changeMade = true;
                    }
                }
                else
                {
                    if (theChange.ExistingObject == null)
                        throw new Exception("Internal error -- trying to replace null object.");
                    if (theChange.NewObject == null)
                        throw new Exception("Internal error -- trying to replace object from null object.");
                    if ((TypeOfObject)theChange.ObjectType != TypeOfObject.PointsManager)
                    {
                        SetStatusOfObject((int)theChange.ExistingObject, (TypeOfObject)theChange.ObjectType, StatusOfObject.Unavailable);
                        SetStatusOfObject((int)theChange.NewObject, (TypeOfObject)theChange.ObjectType, StatusOfObject.Active);
                        changeMade = true;
                    }
                    switch ((TypeOfObject)theChange.ObjectType)
                    {
                        case TypeOfObject.PointsManager:
                            PointsManager originalPointsManager = DataContext.GetTable<PointsManager>().Single(x=>x.PointsManagerID ==(int)theChange.ExistingObject);
                            PointsManager newPointsManager = DataContext.GetTable<PointsManager>().Single(x=>x.PointsManagerID==(int)theChange.NewObject);
                            if (newPointsManager.CurrentPeriodDollarSubsidy > 0 && newPointsManager.EndOfDollarSubsidyPeriod == null)
                                throw new Exception("An end time to the prize period must be specified.");

                            originalPointsManager.DomainID = newPointsManager.DomainID;
                            originalPointsManager.CurrentPeriodDollarSubsidy = newPointsManager.CurrentPeriodDollarSubsidy;
                            originalPointsManager.EndOfDollarSubsidyPeriod = newPointsManager.EndOfDollarSubsidyPeriod;
                            originalPointsManager.NextPeriodDollarSubsidy = newPointsManager.NextPeriodDollarSubsidy;
                            originalPointsManager.NextPeriodLength = newPointsManager.NextPeriodLength;
                            originalPointsManager.NumPrizes = newPointsManager.NumPrizes;
                            originalPointsManager.MinimumPayment = newPointsManager.MinimumPayment;
                            originalPointsManager.TotalUserPoints = newPointsManager.TotalUserPoints;
                            originalPointsManager.CurrentUserPoints = newPointsManager.CurrentUserPoints;
                            originalPointsManager.Name = newPointsManager.Name;
                            originalPointsManager.Creator = newPointsManager.Creator;
                            changeMade = true;
                            PMCacheManagement.InvalidateCacheDependency("DomainID" + newPointsManager.DomainID);
                            UpdateAutomaticInsertableContentForPointsManager(originalPointsManager.PointsManagerID, originalPointsManager.CurrentPeriodDollarSubsidy, originalPointsManager.NumPrizes, originalPointsManager.EndOfDollarSubsidyPeriod);

                            // we won't change the trading status, since that's done elsewhere.
                            break;
                        case TypeOfObject.UsersRights:
                            break;
                        default:
                            throw new Exception("Internal error -- replace object undefined.");
                    } // switch on object type
                } // we're dealing with something other than rating group attributes
            } // we're doing a replace object

            // TO DO: The following should be moved to ChangeOther above and make sure that it works. 

            //if (theChange.ChangeSetting1 && ((TypeOfObject)theChange.ObjectType != TypeOfObject.ChoiceInGroup))
            //{

            //    if (theChange.ExistingObject == null)
            //        throw new Exception("Internal error -- trying to change null object.");
                
            //    if(theChange.NewValueInteger==null)
            //        throw new Exception("Internal error -- must specify a value for NewValueInteger");
            //    if (theChange.NewValueInteger < 5)
            //    {
            //        TradingStatus newTradingStatus = TradingStatus.Active;//must initialize
            //        if (theChange.NewValueInteger == 1)
            //            newTradingStatus = TradingStatus.Active;
            //        if (theChange.NewValueInteger == 2)
            //            newTradingStatus = TradingStatus.SuspendedDirectly;
            //        if (theChange.NewValueInteger == 3)
            //            newTradingStatus = TradingStatus.Ended;
            //        SetTradingStatus((int)theChange.ExistingObject, (TypeOfObject)theChange.ObjectType, newTradingStatus);
                   
            //    }
            //    if (theChange.NewValueInteger >= 5)
            //    {
            //        StatusOfObject theNewStatus=StatusOfObject.Active ;//must initialize
            //        if(theChange.NewValueInteger==5)
            //        {
            //            theNewStatus=StatusOfObject.Active;
            //        }
            //        if(theChange.NewValueInteger==6)
            //        {
            //            theNewStatus=StatusOfObject.Proposed ;
            //        }
            //       SetStatusOfObject((int)theChange.ExistingObject, (TypeOfObject)theChange.ObjectType,theNewStatus);
            //    }
            //    changeMade = true;
              
            //}

            
            if (changeMade)
                DataContext.SubmitChanges();
            else
                throw new Exception("Internal error -- unknown change type");
        }
    }
}
