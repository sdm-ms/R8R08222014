using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MoreStrings;

using StringEnumSupport;
using System.Diagnostics;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public enum RewardableUserAction
    {
        [StringValue("Add row")]
        AddRow = 1,
        [StringValue("Change fields")]
        ChangeFields,
        [StringValue("Resolve table cell")]
        ResolveTableCell,
        [StringValue("Cancel resolution of table cell")]
        CancelResolve,
        [StringValue("Delete row")]
        DeleteRow,
        [StringValue("Undelete row")]
        UndeleteRow,
        [StringValue("Propose comment")]
        ProposeComment,
        [StringValue("Add comment")]
        AddComment,
        [StringValue("Delete comment")]
        DeleteComment,
        [StringValue("Approve proposed comment")]
        ApproveComment,
        [StringValue("Undelete comment")]
        UndeleteComment
    }

        /// <summary>
        /// Summary description for R8RSupport
        /// </summary>
        public partial class R8RDataManipulation
        {

            internal decimal GetNoviceRewardRatingProbability(PointsManager thePointsManager, PointsTotal thePointsTotal)
            {
                return GetNoviceHighStakesProbability(thePointsManager, thePointsTotal) * thePointsManager.DatabaseChangeSelectHighStakesNoviceNumPct;
            }

            internal bool ShouldCreateRewardRating(int originalTblID, int userID, out decimal? baseMultiplierOverride)
            {
                baseMultiplierOverride = null;
                User theUser = DataContext.GetTable<User>().Single(u => u.UserID == userID);
                if (theUser.SuperUser)
                    return false;
                Tbl theTbl = DataContext.GetTable<Tbl>().Single(c => c.TblID == originalTblID);
                PointsManager thePointsManager = theTbl.PointsManager;
                if (theTbl.Name == "Changes")
                    return false;
                PointsTotal thePointsTotal = theUser.PointsTotals.SingleOrDefault(x => x.PointsManagerID == thePointsManager.PointsManagerID);
                if (thePointsTotal == null)
                    thePointsTotal = AddPointsTotal(theUser, DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == thePointsManager.PointsManagerID));
                AddUserCheckIn(theUser, TestableDateTime.Now, thePointsTotal); // let's record that user is making a change at this time that potentially could bring a reward.

                RewardRatingSetting theSetting = DataContext.GetTable<RewardRatingSetting>().SingleOrDefault(x => x.PointsManagerID == theTbl.PointsManagerID);
                if (theSetting == null)
                    return false;
                decimal probabilityThreshold = 0;
                decimal noviceRewardRatingProbability = GetNoviceRewardRatingProbability(thePointsManager, thePointsTotal);
                if (noviceRewardRatingProbability > 0)
                {
                    probabilityThreshold = noviceRewardRatingProbability;
                    baseMultiplierOverride = (theSetting.Multiplier ?? 1M) * (theSetting.ProbOfRewardEvaluation / noviceRewardRatingProbability);
                }
                else
                    probabilityThreshold = theSetting.ProbOfRewardEvaluation;
                decimal randomNumber = RandomGenerator.GetRandom(0M, 1M);
                return (probabilityThreshold > randomNumber);
            }


            public FieldSetDataInfo CreateFieldSetDataForRewardRating(Tbl originalTbl, Tbl userChangesTbl, RewardableUserAction changeType, decimal? baseMultiplierOverride, decimal? supplementalMultiplier, int userID, string changeName, string changeDescription)
            {

                R8RDataAccess theDataAccess = new R8RDataAccess();
                FieldSetDataInfo theFieldSetDataInfo = new FieldSetDataInfo(null, userChangesTbl, theDataAccess);
                theFieldSetDataInfo.theEntityName = changeName;
                User theUser = DataContext.GetTable<User>().Single(u => u.UserID == userID);
                RewardRatingSetting theRMS = DataContext.GetTable<RewardRatingSetting>().Single(rms => rms.PointsManagerID == userChangesTbl.PointsManagerID && rms.Status == (Byte)(StatusOfObject.Active));
                decimal totalMultiplier = (baseMultiplierOverride ?? theRMS.Multiplier ?? 1M) * (supplementalMultiplier ?? 1M);

                List<FieldDefinition> theRewardFieldDefinitions = DataContext.GetTable<FieldDefinition>().Where(fd => fd.TblID == userChangesTbl.TblID).ToList();
                FieldDefinition changeTypeFieldDefinition = theRewardFieldDefinitions.Single(x => x.FieldName == "Change Type");
                FieldDefinition tableFieldDefinition = theRewardFieldDefinitions.Single(x => x.FieldName == "Table");
                FieldDefinition usernameFieldDefinition = theRewardFieldDefinitions.Single(x => x.FieldName == "Username");
                FieldDefinition dateFieldDefinition = theRewardFieldDefinitions.Single(x => x.FieldName == "Date");
                FieldDefinition descriptionFieldDefinition = theRewardFieldDefinitions.Single(x => x.FieldName == "Description");
                FieldDefinition multiplierFieldDefinition = theRewardFieldDefinitions.Single(x => x.FieldName == "Multiplier");

                int changeChoicesGroupID = DataContext.GetTable<ChoiceGroup>().Single(cg => cg.Name.StartsWith("Change choices") && cg.PointsManagerID == originalTbl.PointsManagerID).ChoiceGroupID;
                string theChangeString = StringEnum.GetStringValue(changeType);
                ChoiceInGroup choiceInGroup = DataContext.GetTable<ChoiceInGroup>().Single(cig => cig.ChoiceGroupID == changeChoicesGroupID && cig.ChoiceText == theChangeString);
                ChoiceFieldDataInfo theChangeType = new ChoiceFieldDataInfo(changeTypeFieldDefinition, choiceInGroup, theFieldSetDataInfo, theDataAccess);
                TextFieldDataInfo theTable = new TextFieldDataInfo(tableFieldDefinition, originalTbl.Name, "", theFieldSetDataInfo, theDataAccess);
                TextFieldDataInfo theUsername = new TextFieldDataInfo(usernameFieldDefinition, theUser.Username, "", theFieldSetDataInfo, theDataAccess);
                DateTimeFieldDataInfo theDate = new DateTimeFieldDataInfo(dateFieldDefinition, TestableDateTime.Now, theFieldSetDataInfo, theDataAccess);
                TextFieldDataInfo theDescription = new TextFieldDataInfo(descriptionFieldDefinition, changeDescription, "", theFieldSetDataInfo, theDataAccess);
                NumericFieldDataInfo theMultiplier = new NumericFieldDataInfo(multiplierFieldDefinition, totalMultiplier, theFieldSetDataInfo, theDataAccess);

                theFieldSetDataInfo.AddFieldDataInfo(theChangeType);
                theFieldSetDataInfo.AddFieldDataInfo(theTable);
                theFieldSetDataInfo.AddFieldDataInfo(theUsername);
                theFieldSetDataInfo.AddFieldDataInfo(theDate);
                theFieldSetDataInfo.AddFieldDataInfo(theDescription);
                theFieldSetDataInfo.AddFieldDataInfo(theMultiplier);

                return theFieldSetDataInfo;
            }

            public bool TblIsRewardTbl(Tbl theTbl)
            {
                return theTbl.Name == "Changes";
            }

            public bool TblIsRewardTbl(int TblID)
            {
                Tbl theTbl = DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID);
                return TblIsRewardTbl(theTbl);
            }

            public int? GetUserWhoMadeDatabaseChange(TblRow theTblRow)
            {
                if (!TblIsRewardTbl(theTblRow.Tbl))
                    return null;
                TextField theField = DataContext.GetTable<TextField>().SingleOrDefault(x => x.Field.TblRowID == theTblRow.TblRowID && x.Field.FieldDefinition.FieldName == "Username");
                if (theField == null)
                    return null;
                User theUser = DataContext.GetTable<User>().SingleOrDefault(u => u.Username == theField.Text);
                if (theUser == null)
                    return null;
                return theUser.UserID;
            }

            public decimal GetMultiplierForRewardRatingTblRow(TblRow theRewardRatingTblRow)
            {
                NumberField theField = DataContext.GetTable<NumberField>().SingleOrDefault(x => x.Field.TblRowID == theRewardRatingTblRow.TblRowID && x.Field.FieldDefinition.FieldName == "Multiplier");
                if (theField == null || theField.Number == null)
                    return 1M;
                return (decimal)theField.Number;
            }

            public void UpdateRewardPointsBasedOnUpdatedRating(RewardPendingPointsTracker theTracker, DateTime timeOfRating, decimal? theRating)
            {
                if (theRating == null) // still no trusted rating
                    theRating = 0;

                if (theTracker.PendingRating == null || theTracker.TimeOfPendingRating < timeOfRating)
                {
                    if (theTracker.PendingRating == null)
                        UpdateRewardPointsPotentialMaxLossNotYetPending(theTracker.UserID, theTracker.TblRow.Tbl.PointsManagerID, theTracker.TblRow, false);
                    UpdateRewardPoints(theTracker.UserID, theTracker.TblRow.Tbl.PointsManagerID, theTracker.TblRow, 0, (decimal)theRating - (theTracker.PendingRating ?? 0));
                    theTracker.PendingRating = theRating;
                    theTracker.TimeOfPendingRating = timeOfRating;
                }
            }

            public void UpdateRewardPoints(int userID, int pointsManagerID, TblRow theRewardRatingTblRow, decimal rawFinalPointsDelta, decimal rawPendingPointsDelta)
            {
                if (rawFinalPointsDelta == 0 && rawPendingPointsDelta == 0)
                    return;
                RewardRatingSetting theSettings = DataContext.GetTable<RewardRatingSetting>().SingleOrDefault(rms => rms.PointsManagerID == pointsManagerID);
                decimal multiplier = GetMultiplierForRewardRatingTblRow(theRewardRatingTblRow);
                PointsAdjustmentReason theReason = PointsAdjustmentReason.RatingsUpdate;
                if (rawFinalPointsDelta != 0)
                    theReason = PointsAdjustmentReason.RewardForUserDatabaseChange;
                Trace.TraceInformation("UpdateRewardPoints final: " + rawFinalPointsDelta * multiplier + " pending: " + rawPendingPointsDelta * multiplier);
                UpdateUserPointsAndStatus(userID, pointsManagerID, theReason, rawFinalPointsDelta * multiplier, rawFinalPointsDelta * multiplier, rawPendingPointsDelta * multiplier, 0, 0, 0, true);
            }

            public void UpdateRewardPointsPotentialMaxLossNotYetPending(int userID, int pointsManagerID, TblRow theRewardRatingTblRow, bool startingRewardRating)
            {
                RewardRatingSetting theSettings = DataContext.GetTable<RewardRatingSetting>().SingleOrDefault(rms => rms.PointsManagerID == pointsManagerID);
                decimal multiplier = GetMultiplierForRewardRatingTblRow(theRewardRatingTblRow);
                decimal worstCaseScenario = multiplier * (0 - theSettings.RatingGroupAttribute.RatingCharacteristic.MinimumUserRating);

                Trace.TraceInformation("UpdateRewardPointsPotentialMaxLossNotYetPending " + ((startingRewardRating) ? worstCaseScenario : 0 - worstCaseScenario));
                UpdateUserPointsAndStatus(userID, pointsManagerID, PointsAdjustmentReason.RatingsUpdate, 0, 0, 0, 0, (startingRewardRating) ? worstCaseScenario : 0 - worstCaseScenario, 0, true);
            }

            internal void FinalizeReward(int userID, int pointsManagerID, TblRow theRewardRatingTblRow, decimal rawFinalPoints, bool isBeingUnresolved)
            {
                Trace.TraceInformation("Finalize reward");
                if (isBeingUnresolved)
                    rawFinalPoints = 0 - rawFinalPoints;
                UpdateRewardPointsBasedOnUpdatedRating(theRewardRatingTblRow.RewardPendingPointsTrackers.First(), TestableDateTime.Now, rawFinalPoints); // make pending points equal to final points, and adjust UpdateRewardPointsPotentialMaxLossNotYetPending if not already adjusted
                UpdateRewardPoints(userID, pointsManagerID, theRewardRatingTblRow, rawFinalPoints, 0 - rawFinalPoints); // now deduct all pending points and add to final
                theRewardRatingTblRow.Status = (int)StatusOfObject.Unavailable;
                theRewardRatingTblRow.Tbl.NumTblRowsActive--;
                theRewardRatingTblRow.Tbl.NumTblRowsDeleted++;
                //SQLFastAccess.IdentifyRowRequiringUpdate(DataContext, theRewardRatingTblRow.Tbl, theRewardRatingTblRow, false, false);
                AddTblRowStatusRecord(theRewardRatingTblRow, TestableDateTime.Now, true, false);
            }

            internal void ApplyRewardsForRatingBeingResolved(RatingGroup topRatingGroup, bool isBeingUnresolved)
            {
                if (TblIsRewardTbl(topRatingGroup.TblRow.Tbl))
                {
                    decimal currentRating = topRatingGroup.CurrentValueOfFirstRating ?? 0; // only rating here
                    int? theUser = GetUserWhoMadeDatabaseChange(topRatingGroup.TblRow);
                    if (theUser == null)
                        return;
                    FinalizeReward((int)theUser, topRatingGroup.TblRow.Tbl.PointsManagerID, topRatingGroup.TblRow, currentRating, isBeingUnresolved);
                }
            }

        }
}