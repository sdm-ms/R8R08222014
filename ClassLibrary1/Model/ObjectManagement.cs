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


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        /// <summary>
        /// Generates an exception if a specified object doesn't exist.
        /// </summary>
        /// <param name="objectID">The object id</param>
        /// <param name="theObjectType">The object type</param>
        public void ConfirmObjectExists(int? objectID, TypeOfObject theObjectType)
        {
            if (!ObjectExists(objectID, theObjectType))
                throw new Exception("A specified object does not exist. Objectid = " + objectID.ToString() + " ; ObjectType = " + theObjectType.ToString());
        }

        /// <summary>
        /// Determines whether an object exists.
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="theObjectType"></param>
        /// <returns></returns>
        public bool ObjectExists(int? objectID, TypeOfObject theObjectType)
        {
           
            int objectID2;
            if (objectID == null)
                return false;
            else
                objectID2 = (int)objectID;

            bool returnVal = true;
            bool unknownObjectType = false;
            try
            {
                switch (theObjectType)
                {
                    case TypeOfObject.AddressField:
                        ObjDataAccess.GetAddressField(objectID2);
                        break;
                    case TypeOfObject.TblColumn:
                        ObjDataAccess.GetTblColumn(objectID2);
                        break;
                    case TypeOfObject.TblColumnFormatting:
                        ObjDataAccess.GetTblColumnFormatting(objectID2);
                        break;
                    case TypeOfObject.TblTab:
                        ObjDataAccess.GetTblTab(objectID2);
                        break;
                    case TypeOfObject.ChoiceField:
                        ObjDataAccess.GetChoiceField(objectID2);
                        break;
                    case TypeOfObject.ChoiceGroup:
                        ObjDataAccess.GetChoiceGroup(objectID2);
                        break;
                    case TypeOfObject.ChoiceGroupFieldDefinition:
                        ObjDataAccess.GetChoiceGroupFieldDefinition(objectID2);
                        break;
                    case TypeOfObject.ChoiceInField:
                        ObjDataAccess.GetChoiceInField(objectID2);
                        break;
                    case TypeOfObject.ChoiceInGroup:
                        ObjDataAccess.GetChoiceInGroup(objectID2);
                        break;
                    case TypeOfObject.Tbl:
                        ObjDataAccess.GetTbl(objectID2);
                        break;
                    case TypeOfObject.Comment:
                        ObjDataAccess.GetComment(objectID2);
                        break;
                    case TypeOfObject.DateTimeField:
                        ObjDataAccess.GetDateTimeField(objectID2);
                        break;
                    case TypeOfObject.DateTimeFieldDefinition:
                        ObjDataAccess.GetDateTimeFieldDefinition(objectID2);
                        break;
                    case TypeOfObject.Domain:
                        ObjDataAccess.GetDomain(objectID2);
                        break;
                    case TypeOfObject.TblRow:
                        ObjDataAccess.GetTblRow(objectID2);
                        break;
                    case TypeOfObject.Field:
                        ObjDataAccess.GetField(objectID2);
                        break;
                    case TypeOfObject.FieldDefinition:
                        ObjDataAccess.GetFieldDefinition(objectID2);
                        break;
                    case TypeOfObject.InsertableContent:
                        ObjDataAccess.GetInsertableContents(objectID2);
                        break;
                    case TypeOfObject.Rating:
                        ObjDataAccess.GetRating(objectID2);
                        break;
                    case TypeOfObject.RatingCharacteristics:
                        ObjDataAccess.GetRatingCharacteristic(objectID2);
                        break;
                    case TypeOfObject.RatingCondition:
                        ObjDataAccess.GetRatingCondition(objectID2);
                        break;
                    case TypeOfObject.RatingGroup:
                        ObjDataAccess.GetRatingGroup(objectID2);
                        break;
                    case TypeOfObject.RatingGroupAttributes:
                        ObjDataAccess.GetRatingGroupAttributes(objectID2);
                        break;
                    case TypeOfObject.RatingPhase:
                        ObjDataAccess.GetRatingPhase(objectID2);
                        break;
                    case TypeOfObject.RatingPhaseGroup:
                        ObjDataAccess.GetRatingPhaseGroup(objectID2);
                        break;
                    case TypeOfObject.RatingPlan:
                        ObjDataAccess.GetRatingPlan(objectID2);
                        break;
                    case TypeOfObject.NumberField:
                        ObjDataAccess.GetNumberField(objectID2);
                        break;
                    case TypeOfObject.NumberFieldDefinition:
                        ObjDataAccess.GetNumberFieldDefinition(objectID2);
                        break;
                    case TypeOfObject.OverrideCharacteristics:
                        ObjDataAccess.GetOverrideCharacteristics(objectID2);
                        break;
                    case TypeOfObject.PointsAdjustment:
                        ObjDataAccess.GetPointsAdjustment(objectID2);
                        break;
                    case TypeOfObject.ProposalSettings:
                        ObjDataAccess.GetProposalSettings(objectID2);
                        break;
                    case TypeOfObject.SubsidyAdjustment:
                        ObjDataAccess.GetSubsidyAdjustment(objectID2);
                        break;
                    case TypeOfObject.SubsidyDensityRange:
                        ObjDataAccess.GetSubsidyDensityRange(objectID2);
                        break;
                    case TypeOfObject.SubsidyDensityRangeGroup:
                        ObjDataAccess.GetSubsidyDensityRangeGroup(objectID2);
                        break;
                    case TypeOfObject.TextField:
                        ObjDataAccess.GetTextField(objectID2);
                        break;
                    case TypeOfObject.PointsManager:
                        ObjDataAccess.GetPointsManager(objectID2);
                        break;
                    case TypeOfObject.User:
                        ObjDataAccess.GetUser(objectID2);
                        break;
                    case TypeOfObject.UsersRights:
                        ObjDataAccess.GetUsersRights(objectID2);
                        break;
                    case TypeOfObject.UsersActions: 
                        ObjDataAccess.GetUserAction(objectID2);
                        break;
                    case TypeOfObject.ProposalEvaluationRatingSettings: 
                        ObjDataAccess.GetProposalEvaluationRatingSetting(objectID2);
                        break;
                    case TypeOfObject.RewardRatingSettings: 
                        ObjDataAccess.GetRewardRatingSetting(objectID2);
                        break;
                    default:
                        unknownObjectType = true;
                        throw new Exception("Internal error -- Trying to assess the existence of an unknown object type");
                }
            }
            catch
            {
                returnVal = false;
                if (unknownObjectType)
                    throw; // We haven't really handled the error.
            }

            return returnVal;
        }

        /// <summary>
        /// Return the Tbl and/or universe for an object, if it is definitively associated with one.
        /// Note that many objects are not; for example, rating group attributes can be used in different universes.
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="theObjectType"></param>
        /// <param name="TblID"></param>
        /// <param name="pointsManagerID"></param>
        public void GetTblAndPointsManagerForObject(int? objectID, TypeOfObject theObjectType, out int? TblID, out int? pointsManagerID)
        {
           
            TblID = null;
            pointsManagerID = null;
            if (objectID == null)
                return;
            int objectID2 = (int)objectID;
            switch (theObjectType)
            {
                case TypeOfObject.AddressField:
                    TblID = ObjDataAccess.GetAddressField(objectID2).Field.TblRow.TblID;
                    break;
                case TypeOfObject.TblColumn:
                    TblID = ObjDataAccess.GetTblColumn(objectID2).TblTab.TblID;
                    break;
                case TypeOfObject.TblColumnFormatting:
                    TblID = ObjDataAccess.GetTblColumnFormatting(objectID2).TblColumn.TblTab.TblID;
                    break;
                case TypeOfObject.TblTab:
                    TblID = ObjDataAccess.GetTblTab(objectID2).TblID;
                    break;
                case TypeOfObject.ChoiceField:
                    TblID = ObjDataAccess.GetChoiceField(objectID2).Field.TblRow.TblID;
                    break;
                case TypeOfObject.ChoiceGroup:
                    pointsManagerID = ObjDataAccess.GetChoiceGroup(objectID2).PointsManagerID;
                    break;
                case TypeOfObject.ChoiceGroupFieldDefinition:
                    TblID = ObjDataAccess.GetChoiceGroupFieldDefinition(objectID2).FieldDefinition.TblID;
                    break;
                case TypeOfObject.ChoiceInField:
                    TblID = ObjDataAccess.GetChoiceInField(objectID2).ChoiceField.Field.TblRow.TblID;
                    break;
                case TypeOfObject.ChoiceInGroup:
                    pointsManagerID = ObjDataAccess.GetChoiceInGroup(objectID2).ChoiceGroup.PointsManagerID;
                    break;
                case TypeOfObject.Tbl:
                    TblID = ObjDataAccess.GetTbl(objectID2).TblID;
                    break;
                case TypeOfObject.DateTimeField:
                    TblID = ObjDataAccess.GetDateTimeField(objectID2).Field.TblRow.TblID;
                    break;
                case TypeOfObject.DateTimeFieldDefinition:
                    TblID = ObjDataAccess.GetDateTimeFieldDefinition(objectID2).FieldDefinition.TblID;
                    break;
                case TypeOfObject.Domain:
                    break;
                case TypeOfObject.TblRow:
                    TblID = ObjDataAccess.GetTblRow(objectID2).TblID;
                    break;
                case TypeOfObject.Field:
                    TblID = ObjDataAccess.GetField(objectID2).TblRow.TblID;
                    break;
                case TypeOfObject.FieldDefinition:
                    TblID = ObjDataAccess.GetFieldDefinition(objectID2).TblID;
                    break;
                case TypeOfObject.Rating:
                    TblID = ObjDataAccess.GetRating(objectID2).RatingGroup.TblRow.TblID;
                    break;
                case TypeOfObject.RatingCharacteristics:
                    break;
                case TypeOfObject.RatingCondition:
                    break;
                case TypeOfObject.RatingGroup:
                    TblID = ObjDataAccess.GetRatingGroup(objectID2).TblRow.TblID;
                    break;
                case TypeOfObject.RatingGroupAttributes:
                    break;
                case TypeOfObject.RatingPhase:
                    break;
                case TypeOfObject.RatingPhaseGroup:
                    break;
                case TypeOfObject.RatingPlan:
                    break;
                case TypeOfObject.NumberField:
                    TblID = ObjDataAccess.GetNumberField(objectID2).Field.TblRow.TblID;
                    break;
                case TypeOfObject.NumberFieldDefinition:
                    TblID = ObjDataAccess.GetNumberFieldDefinition(objectID2).FieldDefinition.TblID;
                    break;
                case TypeOfObject.OverrideCharacteristics:
                    TblID = ObjDataAccess.GetOverrideCharacteristics(objectID2).TblColumn.TblTab.TblID;
                    break;
                case TypeOfObject.PointsAdjustment:
                    pointsManagerID = ObjDataAccess.GetPointsAdjustment(objectID2).PointsManagerID;
                    break;
                case TypeOfObject.ProposalSettings:
                    TblID = ObjDataAccess.GetProposalSettings(objectID2).TblID;
                    break;
                case TypeOfObject.SubsidyAdjustment:
                    break;
                case TypeOfObject.SubsidyDensityRange:
                    break;
                case TypeOfObject.SubsidyDensityRangeGroup:
                    break;
                case TypeOfObject.TextField:
                    TblID = ObjDataAccess.GetTextField(objectID2).Field.TblRow.TblID;
                    break;
                case TypeOfObject.PointsManager:
                    pointsManagerID = ObjDataAccess.GetPointsManager(objectID2).PointsManagerID;
                    break;
                case TypeOfObject.User:
                    break;
                case TypeOfObject.UsersRights:
                    pointsManagerID = ObjDataAccess.GetUsersRights(objectID2).PointsManagerID;
                    break;
                default:
                    throw new Exception("Trying to find membership for an object of an unknown object type");
            }
            if (TblID != null)
                pointsManagerID = ObjDataAccess.GetTbl((int)TblID).PointsManagerID;
        }

        /// <summary>
        /// Set the status of the specified object to the specified status. If we're activating, we make
        /// sure to avoid conflict with other objects that may have been added since.
        /// </summary>
        /// <param name="objectID">The object</param>
        /// <param name="theObjectType">The object's type</param>
        /// <param name="theStatus">The status of the object</param>
        private void SetStatusOfObject(int objectID, TypeOfObject theObjectType, StatusOfObject theStatus)
        {
           
            Byte newValue = (Byte)theStatus;
            switch (theObjectType)
            {
                case TypeOfObject.AddressField:
                    AddressField theAddressField = DataContext.GetTable<AddressField>().Single(x => x.AddressFieldID == objectID);
                    theAddressField.Status = newValue;
                    ResetTblRowFieldDisplay(theAddressField.Field.TblRow);
                     break;
                case TypeOfObject.TblColumn:
                    TblColumn theTblColumn = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == objectID);
                    
                    theTblColumn.Status = newValue;
                    DataContext.SubmitChanges();
                    StartAddingMissingRatingsForTbl(theTblColumn.TblTab.TblID);
                    break;
                case TypeOfObject.TblColumnFormatting:
                    TblColumnFormatting theTblColumnFormatting = DataContext.GetTable<TblColumnFormatting>().Single(x => x.TblColumnFormattingID == objectID);
                    theTblColumnFormatting.Status = newValue;
                    break;
                case TypeOfObject.TblTab:
                    TblTab theTblTab = DataContext.GetTable<TblTab>().Single(x => x.TblTabID== objectID);
                   
                    theTblTab.Status = newValue;
                    break;
                case TypeOfObject.ChoiceField:
                    ChoiceField theChoiceField = DataContext.GetTable<ChoiceField>().Single(x => x.ChoiceFieldID == objectID);
                    theChoiceField.Status = newValue;
                    ResetTblRowFieldDisplay(theChoiceField.Field.TblRow);
                    break;
                case TypeOfObject.ChoiceGroup:
                    ChoiceGroup theChoiceGroup = DataContext.GetTable<ChoiceGroup>().Single(x => x.ChoiceGroupID == objectID);
                    theChoiceGroup.Status = newValue;
                    var entities1 = theChoiceGroup.ChoiceGroups.SelectMany(x => x.ChoiceInGroups).SelectMany(y => y.ChoiceInFields).Select(z => z.ChoiceField.Field.TblRow).Distinct().ToList();
                    foreach (var entity1 in entities1)
                        ResetTblRowFieldDisplay(entity1);
                    break;
                case TypeOfObject.ChoiceGroupFieldDefinition:
                    ChoiceGroupFieldDefinition theCGFD = DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.ChoiceGroupFieldDefinitionID== objectID);
                    theCGFD.Status = newValue;
                    ResetTblRowFieldDisplay(theCGFD.FieldDefinition.Tbl);
                    if (newValue != (byte)StatusOfObject.Active)
                    {
                        var dependentChoiceGroupFieldDefinitions = DataContext.GetTable<ChoiceGroupFieldDefinition>().Where(cgfd => cgfd.DependentOnChoiceGroupFieldDefinitionID == objectID);
                        foreach (var dependentCGFD in dependentChoiceGroupFieldDefinitions)
                        {
                            dependentCGFD.Status = newValue;
                        }
                    }
                    break;
                case TypeOfObject.ChoiceInField:
                    ChoiceInField theChoiceInField = DataContext.GetTable<ChoiceInField>().Single(x => x.ChoiceInFieldID== objectID);
                    theChoiceInField.Status = newValue;
                    ResetTblRowFieldDisplay(theChoiceInField.ChoiceField.Field.TblRow);
                    break;
                case TypeOfObject.ChoiceInGroup:
                    ChoiceInGroup theChoiceInGroup = DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceInGroupID == objectID);
                    theChoiceInGroup.Status = newValue;
                    var entities = theChoiceInGroup.ChoiceInFields.Select(x => x.ChoiceField).Select(x => x.Field.TblRow).Distinct().ToList();
                    foreach (var entity in entities)
                        ResetTblRowFieldDisplay(entity);
                    break;
                case TypeOfObject.Tbl:
                    DataContext.GetTable<Tbl>().Single(x => x.TblID == objectID).Status =newValue;

                    break;
                case TypeOfObject.Comment:
                    Comment theComment = DataContext.GetTable<Comment>().Single(x => x.CommentsID == objectID);
                    theComment.Status = newValue;
                    if (newValue == (byte) StatusOfObject.Unavailable)
                        theComment.LastDeletedDate = TestableDateTime.Now;
                    else
                        theComment.LastDeletedDate = null;
                    break;
                case TypeOfObject.DateTimeField:
                    DateTimeField theField = DataContext.GetTable<DateTimeField>().Single(x => x.DateTimeFieldID== objectID);
                    theField.Status = newValue;
                    ResetTblRowFieldDisplay(theField.Field.TblRow);
                    break;
                case TypeOfObject.DateTimeFieldDefinition:
                    DateTimeFieldDefinition theFD = DataContext.GetTable<DateTimeFieldDefinition>().Single(x => x.DateTimeFieldDefinitionID == objectID);
                    theFD.Status = newValue;
                    ResetTblRowFieldDisplay(theFD.FieldDefinition.Tbl);
                    break;
                case TypeOfObject.Domain:
                    DataContext.GetTable<Domain>().Single(x => x.DomainID == objectID).Status = newValue;
                    break;
                case TypeOfObject.TblRow:
                    TblRow theTblRow = DataContext.GetTable<TblRow>().Single(x => x.TblRowID == objectID);
                    StatusOfObject originalStatus = (StatusOfObject)theTblRow.Status;
                    theTblRow.Status = newValue;
                    DataContext.SubmitChanges();
                    if (theStatus == StatusOfObject.Active) 
                    {
                        AddTblRowStatusRecord(theTblRow, TestableDateTime.Now, false, false);
                    }
                    else
                    {
                        AddTblRowStatusRecord(theTblRow, TestableDateTime.Now, true, false);	                        
                    }

                    break;
                case TypeOfObject.Field:
                    Field theField2 = DataContext.GetTable<Field>().Single(x => x.FieldID == objectID);
                    theField2.Status = newValue;
                    ResetTblRowFieldDisplay(theField2.TblRow);
                    break;
                case TypeOfObject.FieldDefinition:
                    FieldDefinition theFD2 = DataContext.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == objectID);
                    theFD2.Status = newValue;
                    ResetTblRowFieldDisplay(theFD2.Tbl);
                    break;
                case TypeOfObject.InsertableContent:
                    DataContext.GetTable<InsertableContent>().Single(x => x.InsertableContentID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingCharacteristics:
                    DataContext.GetTable<RatingCharacteristic>().Single(x => x.RatingCharacteristicsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingCondition:
                    DataContext.GetTable<RatingCondition>().Single(x => x.RatingConditionID== objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingGroup:
                    DataContext.GetTable<RatingGroup>().Single(x => x.RatingGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingGroupAttributes:
                    DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingPhase:
                    DataContext.GetTable<RatingPhase>().Single(x => x.RatingPhaseID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingPhaseGroup:
                    DataContext.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingPlan:
                    DataContext.GetTable<RatingPlan>().Single(x => x.RatingPlansID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RatingGroupResolution:
                    DataContext.GetTable<RatingGroupResolution>().Single(x => x.RatingGroupResolutionID == objectID).Status = newValue;
                    break;
                case TypeOfObject.NumberField:
                    NumberField theNumberField = DataContext.GetTable<NumberField>().Single(x => x.NumberFieldID== objectID);
                    theNumberField.Status = newValue;
                    ResetTblRowFieldDisplay(theNumberField.Field.TblRow);
                    break;
                case TypeOfObject.NumberFieldDefinition:
                    NumberFieldDefinition theNumberFD = DataContext.GetTable<NumberFieldDefinition>().Single(x => x.NumberFieldDefinitionID == objectID);
                    theNumberFD.Status = newValue;
                    ResetTblRowFieldDisplay(theNumberFD.FieldDefinition.Tbl);
                    break;
                case TypeOfObject.OverrideCharacteristics:
                    DataContext.GetTable<OverrideCharacteristic>().Single(x => x.OverrideCharacteristicsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.PointsAdjustment:
                    DataContext.GetTable<PointsAdjustment>().Single(x => x.PointsAdjustmentID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ProposalSettings:
                    DataContext.GetTable<ProposalSetting>().Single(x => x.ProposalSettingsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.SubsidyAdjustment:
                    DataContext.GetTable<SubsidyAdjustment>().Single(x => x.SubsidyAdjustmentID == objectID).Status = newValue;
                    break;
                case TypeOfObject.SubsidyDensityRange:
                    DataContext.GetTable<SubsidyDensityRange>().Single(x => x.SubsidyDensityRangeID == objectID).Status = newValue;
                    break;
                case TypeOfObject.SubsidyDensityRangeGroup:
                    DataContext.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.TextField:
                    TextField theTextField = DataContext.GetTable<TextField>().Single(x => x.TextFieldID== objectID);
                    theTextField.Status = newValue;
                    ResetTblRowFieldDisplay(theTextField.Field.TblRow);
                    break;
                case TypeOfObject.TextFieldDefinition:
                    TextFieldDefinition theTextFD = DataContext.GetTable<TextFieldDefinition>().Single(x => x.TextFieldDefinitionID == objectID);
                    theTextFD.Status = newValue;
                    ResetTblRowFieldDisplay(theTextFD.FieldDefinition.Tbl);
                    break;
                case TypeOfObject.PointsManager:
                    DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == objectID).Status = newValue;
                    break;
                case TypeOfObject.User:
                    DataContext.GetTable<User>().Single(x => x.UserID == objectID).Status = newValue;
                    break;
                case TypeOfObject.RewardRatingSettings: 
                    DataContext.GetTable<RewardRatingSetting>().Single(x => x.RewardRatingSettingsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.ProposalEvaluationRatingSettings: 
                    DataContext.GetTable<ProposalEvaluationRatingSetting>().Single(x => x.ProposalEvaluationRatingSettingsID == objectID).Status = newValue;
                    break;
                case TypeOfObject.UsersAdministrationRightsGroup: 
                    DataContext.GetTable<UsersAdministrationRightsGroup>().Single(x => x.UsersAdministrationRightsGroupID == objectID).Status = newValue;
                    break;
                case TypeOfObject.UsersActions:
                    break;
                case TypeOfObject.UsersRights:
                    DataContext.GetTable<UsersRight>().Single(x => x.UsersRightsID == objectID).Status = newValue;             
                    break;
                default:
                    throw new Exception("Internal error -- trying to activate or deactivate unknown object type");
            }
            DataContext.SubmitChanges();
        }

        public bool FixStatusInconsistencies()
        {
            // We look for objects that are Active but should be set to DerivativelyUnavailable because
            // the immediately higher object in the hierarchy is not Active, and also we look for objects that are DerivativelyUnavailable
            // but should be set to Active because the immediate superior is now Active again.

            // The hierarchy is as follows: Domain, PointsManager, Tbl, TblRow OR TblTab/TblColumn, RatingGroup.
            // This means that TblRow and TblTab both depend on Tbl, and RatingGroup depends on both TblRow and TblColumn.
            // We only bother to inactivate the top rating group. When a top rating group is inactive, there should be no short term expiration
            // of that rating group (or any ratings in it).

            // We go through the hierarchy from top to bottom, and at any step first look for things that need to be inactivated and then
            // for things that need to be activated.

            bool moreWorkToDo = false;

            // As soon as there's an indication that we've done something, we'll return so that this can be called again soon.
            for (int hierarchyComparisonToMake = 1; hierarchyComparisonToMake <= 6 && !moreWorkToDo; hierarchyComparisonToMake++)
            {
                moreWorkToDo = FixStatusInconsistenciesHelper(true, hierarchyComparisonToMake);
                if (!moreWorkToDo)
                    FixStatusInconsistenciesHelper(false, hierarchyComparisonToMake);
            }

            return moreWorkToDo;
        }


        //public class StatusInconsistencyExistsArgs
        //{
        //    public bool checkingForImproperlyActive;
        //    public StatusOfObject higherObject;
        //    public StatusOfObject? additionalHigherObject;
        //    public StatusOfObject lowerObject;

        //    public StatusInconsistencyExistsArgs(bool checking, StatusOfObject higher, StatusOfObject? additional, StatusOfObject lower)
        //    {
        //        checkingForImproperlyActive = checking;
        //        higherObject = higher;
        //        additionalHigherObject = additional;
        //        lowerObject = lower;
        //    }
        //}


        //public static bool StatusInconsistencyExistsOriginal()
        //{
        //    if (checkingForImproperlyActive)
        //    {
        //        return (lowerObject == StatusOfObject.Active && ( (higherObject != StatusOfObject.Active) || (additionalHigherObject != null && additionalHigherObject != StatusOfObject.Active)));
        //    }
        //    else
        //    { // We are checking for improperly derivatively inactive
        //        return (lowerObject == StatusOfObject.DerivativelyUnavailable && higherObject == StatusOfObject.Active && (additionalHigherObject == null || additionalHigherObject == StatusOfObject.Active));
        //    }
        //}

        public bool FixStatusInconsistenciesHelper(bool checkingForImproperlyActive, int hierarchyComparison)
        {
            
            //Expression<Func<StatusInconsistencyExistsArgs, bool>> StatusInconsistencyExistsExpression = x =>
            //(
            //    x.checkingForImproperlyActive ?
            //        ((x.lowerObject == StatusOfObject.Active && ((x.higherObject != StatusOfObject.Active) || (x.additionalHigherObject != null && x.additionalHigherObject != StatusOfObject.Active))))
            //    :
            //        ((x.lowerObject == StatusOfObject.DerivativelyUnavailable && x.higherObject == StatusOfObject.Active && (x.additionalHigherObject == null || x.additionalHigherObject == StatusOfObject.Active)))
            //);

            switch (hierarchyComparison)
            {
                
                case 1:
                    var PointsManagersToCheck = DataContext.GetTable<PointsManager>().Where(x => checkingForImproperlyActive ?
                        ((StatusOfObject)x.Status == StatusOfObject.Active && (StatusOfObject)x.Domain.Status != StatusOfObject.Active)
                        :
                        ((StatusOfObject)x.Status == StatusOfObject.DerivativelyUnavailable && (StatusOfObject)x.Domain.Status == StatusOfObject.Active)
                        );
                    if (PointsManagersToCheck.Any())
                    {
                        var PointsManagersToFix = PointsManagersToCheck.Take(5);
                        foreach (var PointsManagerToFix in PointsManagersToFix)
                            SetStatusOfObject(PointsManagerToFix.PointsManagerID, TypeOfObject.PointsManager, checkingForImproperlyActive ? StatusOfObject.DerivativelyUnavailable : StatusOfObject.Active);
                        return true;
                    }
                    break;
                
                case 2:
                    var TblsToCheck = DataContext.GetTable<Tbl>().Where(x => checkingForImproperlyActive ?
                        ((StatusOfObject)x.Status == StatusOfObject.Active && (StatusOfObject)x.PointsManager.Status != StatusOfObject.Active)
                        :
                        ((StatusOfObject)x.Status == StatusOfObject.DerivativelyUnavailable && (StatusOfObject)x.PointsManager.Status == StatusOfObject.Active)
                        );
                    if (TblsToCheck.Any())
                    {
                        var TblsToFix = TblsToCheck.Take(5);
                        foreach (var TblToFix in TblsToFix)
                            SetStatusOfObject(TblToFix.TblID, TypeOfObject.Tbl, checkingForImproperlyActive ? StatusOfObject.DerivativelyUnavailable : StatusOfObject.Active);
                        return true;
                    }
                    break;


                case 3:
                    var TblRowsToCheck = DataContext.GetTable<TblRow>().Where(x => checkingForImproperlyActive ?
                        ((StatusOfObject)x.Status == StatusOfObject.Active && (StatusOfObject)x.Tbl.Status != StatusOfObject.Active)
                        :
                        ((StatusOfObject)x.Status == StatusOfObject.DerivativelyUnavailable && (StatusOfObject)x.Tbl.Status == StatusOfObject.Active)
                        );
                    if (TblRowsToCheck.Any())
                    {
                        var TblRowsToFix = TblRowsToCheck.Take(5);
                        foreach (var TblRowToFix in TblRowsToFix)
                            SetStatusOfObject(TblRowToFix.TblRowID, TypeOfObject.TblRow, checkingForImproperlyActive ? StatusOfObject.DerivativelyUnavailable : StatusOfObject.Active);
                        return true;
                    }
                    break;

                case 4:
                    var TblTabsToCheck = DataContext.GetTable<TblTab>().Where(x => checkingForImproperlyActive ?
                        ((StatusOfObject)x.Status == StatusOfObject.Active && (StatusOfObject)x.Tbl.Status != StatusOfObject.Active)
                        :
                        ((StatusOfObject)x.Status == StatusOfObject.DerivativelyUnavailable && (StatusOfObject)x.Tbl.Status == StatusOfObject.Active)
                        );
                    if (TblTabsToCheck.Any())
                    {
                        var TblTabsToFix = TblTabsToCheck.Take(5);
                        foreach (var TblTabToFix in TblTabsToFix)
                            SetStatusOfObject(TblTabToFix.TblTabID, TypeOfObject.TblTab, checkingForImproperlyActive ? StatusOfObject.DerivativelyUnavailable : StatusOfObject.Active);
                        return true;
                    }
                    break;

                case 5:
                    var TblColumnsToCheck = DataContext.GetTable<TblColumn>().Where(x => checkingForImproperlyActive ?
                        ((StatusOfObject)x.Status == StatusOfObject.Active && (StatusOfObject)x.TblTab.Status != StatusOfObject.Active)
                        :
                        ((StatusOfObject)x.Status == StatusOfObject.DerivativelyUnavailable && (StatusOfObject)x.TblTab.Status == StatusOfObject.Active)
                        );
                    if (TblColumnsToCheck.Any())
                    {
                        var TblColumnsToFix = TblColumnsToCheck.Take(5);
                        foreach (var TblColumnToFix in TblColumnsToFix)
                            SetStatusOfObject(TblColumnToFix.TblColumnID, TypeOfObject.TblColumn, checkingForImproperlyActive ? StatusOfObject.DerivativelyUnavailable : StatusOfObject.Active);
                        return true;
                    }
                    break;

                case 6:
                    var RatingGroupsToCheck = DataContext.GetTable<RatingGroup>().Where(x => checkingForImproperlyActive ?
                        ((StatusOfObject)x.Status == StatusOfObject.Active && ( (StatusOfObject)x.TblColumn.Status != StatusOfObject.Active || (StatusOfObject)x.TblRow.Status != StatusOfObject.Active ) )
                        :
                        ((StatusOfObject)x.Status == StatusOfObject.DerivativelyUnavailable && (StatusOfObject)x.TblColumn.Status == StatusOfObject.Active && (StatusOfObject)x.TblRow.Status == StatusOfObject.Active)
                        );
                    if (RatingGroupsToCheck.Any())
                    {
                        var RatingGroupsToFix = RatingGroupsToCheck.Take(5);
                        foreach (var RatingGroupToFix in RatingGroupsToFix)
                            SetStatusOfObject(RatingGroupToFix.RatingGroupID, TypeOfObject.RatingGroup, checkingForImproperlyActive ? StatusOfObject.DerivativelyUnavailable : StatusOfObject.Active);
                        return true;
                    }
                    break;

            }

            return false;
        }


        /// <summary>
        /// Finds an available name for an object. If the given name is unavailable, a digit is added
        /// until the name is OK. If there is already a digit, that digit is incremented as necessary.
        /// </summary>
        /// <param name="objectID">The object id</param>
        /// <param name="theObjectType">The type of the object</param>
        /// <param name="userID">The id of the user who wishes to create the object (or null for system-created)</param>
        /// <param name="theName">The proposed name</param>
        public void FindAvailableNameForObject(int objectID, TypeOfObject theObjectType, int? userID, ref String theName)
        {
            if (theName == "")
                theName = "Unnamed 1";
            while (!NameIsAvailableForObject(objectID, theObjectType, userID, theName))
                theName = theName.IncrementNumAtEndOfString();
        }

        /// <summary>
        /// Determines whether a name is available for a particular object. For some fields, this depends on whether
        /// the user (or a system user) has created an object with that name, while for other fields, it depends only
        /// on whether there exists an object with that name in conflicting scope (e.g., within the same Tbl).
        /// This function ignores the names of objects that are merely proposed.
        /// </summary>
        /// <param name="objectID">The object id</param>
        /// <param name="theObjectType">The type of the object</param>
        /// <param name="userID">The id of the user who wishes to create the object (or null for system-created)</param>
        /// <param name="theName">The proposed name</param>
        /// <returns>True if and only if the name has not been used for the object</returns>
        public bool NameIsAvailableForObject(int objectID, TypeOfObject theObjectType, int? userID, String theName)
        {
            
            switch (theObjectType)
            {
                case TypeOfObject.TblColumn:
                    int TblTabID = ObjDataAccess.GetTblColumn(objectID).TblTabID;
                    return !DataContext.GetTable<TblColumn>().Where(cd => cd.TblTab.TblTabID == TblTabID && cd.Name == theName && cd.Status != (byte)StatusOfObject.Proposed && cd.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.TblTab:
                    int TblID = ObjDataAccess.GetTblTab(objectID).TblID;
                    return !DataContext.GetTable<TblTab>().Where(cg => cg.TblID == TblID && cg.Name == theName && cg.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.ChoiceGroup:
                    return !DataContext.GetTable<ChoiceGroup>().Where(cg => (cg.Creator == userID || cg.Creator == null) && cg.Name == theName && cg.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.Tbl:
                    int pointsManagerID = ObjDataAccess.GetTbl(objectID).PointsManagerID;
                    return !DataContext.GetTable<Tbl>().Where(c => c.PointsManagerID == pointsManagerID && c.Name == theName && c.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.Domain:
                    return !DataContext.GetTable<Domain>().Where(dom => dom.Name == theName && dom.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.TblRow:
                    int TblID2 = ObjDataAccess.GetTblRow(objectID).TblID;
                    return !DataContext.GetTable<TblRow>().Where(e => e.TblID == TblID2 && e.Name == theName && e.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.InsertableContent:
                    
                    return !DataContext.GetTable<InsertableContent>().Where(ins => ins.Name == theName && ins.Status != (byte)StatusOfObject.Proposed).Any();

                case TypeOfObject.RatingCharacteristics:
                    return !DataContext.GetTable<RatingCharacteristic>().Where(mc => (mc.Creator == userID || mc.Creator == null) && mc.Name == theName && mc.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.RatingGroupAttributes:
                    return !DataContext.GetTable<RatingGroupAttribute>().Where(mga => (mga.Creator == userID || mga.Creator == null) && mga.Name == theName && mga.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.RatingPhaseGroup:
                    return !DataContext.GetTable<RatingPhaseGroup>().Where(mpg => (mpg.Creator == userID || mpg.Creator == null) && mpg.Name == theName && mpg.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.RatingPlan:
                    return true; // We can have multiple ratings (and thus rating plans) with same name.
                case TypeOfObject.RewardRatingSettings:
                    return !DataContext.GetTable<RewardRatingSetting>().Any(rms => (rms.Creator == userID || rms.Creator == null) && rms.Name == theName && rms.Status != (byte)StatusOfObject.Proposed);
                case TypeOfObject.SubsidyDensityRangeGroup:
                    return !DataContext.GetTable<SubsidyDensityRangeGroup>().Where(sdrg => (sdrg.Creator == userID || sdrg.Creator == null) && sdrg.Name == theName && sdrg.Status != (byte)StatusOfObject.Proposed).Any();
                case TypeOfObject.PointsManager:
                    int domainID = ObjDataAccess.GetPointsManager(objectID).DomainID;
                    return !DataContext.GetTable<PointsManager>().Where(u => u.DomainID == domainID && u.Name == theName && u.Status != (byte)StatusOfObject.Proposed).Any();
                default:
                    throw new Exception("Internal error -- trying to change system name of field without a system name");
            }
        }

        /// <summary>
        /// Returns the name of a named object.
        /// </summary>
        /// <param name="objectID">The id of the object</param>
        /// <param name="theObjectType">The type of the object</param>
        /// <param name="theName">Passes back the name of the object</param>
        public void GetNameOfObject(int objectID, TypeOfObject theObjectType, ref String theName)
        {
            if (!ObjectExists(objectID, theObjectType))
                throw new Exception("Internal error -- trying to get name of nonexistent object.");
            switch (theObjectType)
            {
                case TypeOfObject.TblColumn:
                    theName = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == objectID).Name;
                    break;
                case TypeOfObject.TblTab:
                    theName = DataContext.GetTable<TblTab>().Single(x => x.TblTabID == objectID).Name;
                    break;
                case TypeOfObject.ChoiceGroup:
                    theName = DataContext.GetTable<ChoiceGroup>().Single(x => x.ChoiceGroupID == objectID).Name;
                    break;
                case TypeOfObject.Tbl:
                    theName = DataContext.GetTable<Tbl>().Single(x => x.TblID == objectID).Name;
                    break;
                case TypeOfObject.Domain:
                    theName = DataContext.GetTable<Domain>().Single(x => x.DomainID == objectID).Name;
                    break;
                case TypeOfObject.TblRow:
                    theName = DataContext.GetTable<TblRow>().Single(x => x.TblRowID == objectID).Name;
                    break;
                case TypeOfObject.InsertableContent:
                    theName = DataContext.GetTable<InsertableContent>().Single(x => x.InsertableContentID == objectID).Name;
                    break;
                case TypeOfObject.RatingCharacteristics:
                    theName = DataContext.GetTable<RatingCharacteristic>().Single(x => x.RatingCharacteristicsID == objectID).Name;
                    break;
                case TypeOfObject.RatingGroupAttributes:
                    theName = DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == objectID).Name;
                    break;
                case TypeOfObject.RatingPhaseGroup:
                    theName = DataContext.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID == objectID).Name;
                    break;
                case TypeOfObject.RatingPlan:
                    theName = DataContext.GetTable<RatingPlan>().Single(x => x.RatingPlansID == objectID).Name;
                    break;
                case TypeOfObject.RewardRatingSettings:
                    theName = DataContext.GetTable<RewardRatingSetting>().Single(x => x.RewardRatingSettingsID == objectID).Name;
                    break;
                case TypeOfObject.SubsidyDensityRangeGroup:
                    theName = DataContext.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID == objectID).Name;
                    break;
                case TypeOfObject.PointsManager:
                    theName = DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == objectID).Name;
                    break;
                default:
                    throw new Exception("Internal error -- trying to get the name of an unnamed object type");
            }
        }

        public bool ObjectTypeIsNamed(TypeOfObject theObjectType)
        {
            switch (theObjectType)
            {
                case TypeOfObject.TblColumn:
                case TypeOfObject.TblTab:
                case TypeOfObject.ChoiceGroup:
                case TypeOfObject.Tbl:
                case TypeOfObject.Domain:
                case TypeOfObject.TblRow:
                case TypeOfObject.InsertableContent:
                case TypeOfObject.RatingCharacteristics:
                case TypeOfObject.RatingGroupAttributes:
                case TypeOfObject.RatingPhaseGroup:
                case TypeOfObject.RatingPlan:
                case TypeOfObject.SubsidyDensityRangeGroup:
                case TypeOfObject.PointsManager:
                case TypeOfObject.AdministrationRightsGroup:
                case TypeOfObject.ProposalEvaluationRatingSettings: 
                case TypeOfObject.RewardRatingSettings: 
                    return true;
                default:
                    return false;
            }

        }

        /// <summary>
        /// Returns the name and creator of a named object.
        /// </summary>
        /// <param name="objectID">The id of the object</param>
        /// <param name="theObjectType">The type of the object</param>
        /// <param name="theName">Passes back the name of the object</param>
        /// <param name="theCreator">Passes back the creator of the object</param>
        public void GetNameAndCreatorOfObject(int objectID, TypeOfObject theObjectType, ref String theName, ref int? theCreator)
        {
            if (!ObjectExists(objectID, theObjectType))
                throw new Exception("Internal error -- trying to get name of nonexistent object.");
            switch (theObjectType)
            {
                case TypeOfObject.TblColumn:
                    theName = DataContext.GetTable<TblColumn>().Single(x => x.TblColumnID == objectID).Name;
                    break;
                case TypeOfObject.TblTab:
                    theName = DataContext.GetTable<TblTab>().Single(x => x.TblTabID == objectID).Name;
                    break;
                case TypeOfObject.ChoiceGroup:
                    theName = DataContext.GetTable<ChoiceGroup>().Single(x => x.ChoiceGroupID == objectID).Name;
                    break;
                case TypeOfObject.Tbl:
                    theName = DataContext.GetTable<Tbl>().Single(x => x.TblID == objectID).Name;
                    break;
                case TypeOfObject.Domain:
                    theName = DataContext.GetTable<Domain>().Single(x => x.DomainID == objectID).Name;
                    break;
                case TypeOfObject.TblRow:
                    theName = DataContext.GetTable<TblRow>().Single(x => x.TblRowID == objectID).Name;
                    break;
                case TypeOfObject.InsertableContent:
                    theName = DataContext.GetTable<InsertableContent>().Single(x => x.InsertableContentID == objectID).Name;
                    break;
                case TypeOfObject.RatingCharacteristics:
                    theName = DataContext.GetTable<RatingCharacteristic>().Single(x => x.RatingCharacteristicsID == objectID).Name;
                    break;
                case TypeOfObject.RatingGroupAttributes:
                    theName = DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == objectID).Name;
                    break;
                case TypeOfObject.RatingPhaseGroup:
                    theName = DataContext.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID == objectID).Name;
                    break;
                case TypeOfObject.RatingPlan:
                    theName = DataContext.GetTable<RatingPlan>().Single(x => x.RatingPlansID == objectID).Name;
                    break;
                case TypeOfObject.RewardRatingSettings:
                    theName = DataContext.GetTable<RewardRatingSetting>().Single(x => x.RewardRatingSettingsID == objectID).Name;
                    break;
                case TypeOfObject.SubsidyDensityRangeGroup:
                    theName = DataContext.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID == objectID).Name;
                    break;
                case TypeOfObject.PointsManager:
                    theName = DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == objectID).Name;
                    break;
                default:
                    throw new Exception("Internal error -- trying to get the name of an unnamed object type");
            }
            switch (theObjectType)
            {
                case TypeOfObject.TblColumn:
                    theCreator = null;
                    break;
                case TypeOfObject.TblTab:
                    theCreator = null;
                    break;
                case TypeOfObject.ChoiceGroup:
                    theCreator = DataContext.GetTable<ChoiceGroup>().Single(x => x.ChoiceGroupID == objectID).Creator;
                    break;
                case TypeOfObject.Tbl:
                    theCreator = DataContext.GetTable<Tbl>().Single(x => x.TblID == objectID).Creator;
                    break;
                case TypeOfObject.Domain:
                    theCreator = DataContext.GetTable<Domain>().Single(x => x.DomainID == objectID).Creator;
                    break;
                case TypeOfObject.TblRow:
                    theCreator = null;
                    break;
                case TypeOfObject.InsertableContent :
                    theCreator = null;
                    break;
                case TypeOfObject.RatingCharacteristics:
                    theCreator = DataContext.GetTable<RatingCharacteristic>().Single(x => x.RatingCharacteristicsID == objectID).Creator;
                    break;
                case TypeOfObject.RatingGroupAttributes:
                    theCreator = DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID == objectID).Creator;
                    break;
                case TypeOfObject.RatingPhaseGroup:
                    theCreator = DataContext.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID == objectID).Creator;
                    break;
                case TypeOfObject.RatingPlan:
                    theCreator = DataContext.GetTable<RatingPlan>().Single(x => x.RatingPlansID == objectID).Creator;
                    break;
                case TypeOfObject.RewardRatingSettings:
                    theCreator = DataContext.GetTable<RewardRatingSetting>().Single(x => x.RewardRatingSettingsID == objectID).Creator;
                    break;
                case TypeOfObject.SubsidyDensityRangeGroup:
                    theCreator = DataContext.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID == objectID).Creator;
                    break;
                case TypeOfObject.PointsManager:
                    theCreator = DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == objectID).Creator;
                    break;
                default:
                    throw new Exception("Internal error -- trying to get the Creator of an unCreatord object type");
            }
        }

        /// <summary>
        /// Changes the name of an object to the specified name. If that name is taken, we'll find a new
        /// name to change it to (e.g., Object 4).
        /// </summary>
        /// <param name="objectID">The object</param>
        /// <param name="theObjectType">The type of the object</param>
        /// <param name="userID">The user who created the object</param>
        /// <param name="theName">The name of the object</param>
        public void ChangeNameOfObject(int objectID, TypeOfObject theObjectType, int? userID, String theName)
        {
            // We allow duplicative names for entities, but not other objects.
            if (!NameIsAvailableForObject(objectID, theObjectType, userID, theName) && theObjectType != TypeOfObject.TblRow)
                FindAvailableNameForObject(objectID, theObjectType, userID, ref theName);
            switch (theObjectType)
            {
                case TypeOfObject.TblColumn:
                    var theTblColumn = DataContext.GetTable<TblColumn>().Single(x=>x.TblColumnID==objectID);
                    theTblColumn.Name = theName;
                    CacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theTblColumn.TblTab.TblID);
                    break;
                case TypeOfObject.TblTab:
                    var theTblTab = DataContext.GetTable<TblTab>().Single(x => x.TblTabID==objectID);
                    theTblTab.Name = theName;
                    CacheManagement.InvalidateCacheDependency("CategoriesForTblID" + theTblTab.TblID);
                    break;
                case TypeOfObject.ChoiceGroup:
                    var theChoiceGroup = DataContext.GetTable<ChoiceGroup>().Single(x => x.ChoiceGroupID==objectID);
                    theChoiceGroup.Name = theName;
                    CacheManagement.InvalidateCacheDependency("FieldInfoForPointsManagerID" + theChoiceGroup.PointsManagerID);
                    break;
                case TypeOfObject.Tbl:
                    var theTbl = DataContext.GetTable<Tbl>().Single(x => x.TblID==objectID);
                    theTbl.Name = theName;
                    CacheManagement.InvalidateCacheDependency("DomainID" + theTbl.PointsManager.DomainID);
                    CacheManagement.InvalidateCacheDependency("TopicsMenu");
                    break;
                case TypeOfObject.Domain:
                    Domain theDomain = DataContext.GetTable<Domain>().Single(x => x.DomainID==objectID);
                    theDomain.Name = theName;
                    CacheManagement.InvalidateCacheDependency("TopicsMenu");
                    
                    break;
                case TypeOfObject.TblRow:
                    TblRow theTblRow = DataContext.GetTable<TblRow>().Single(x => x.TblRowID ==objectID);
                    theTblRow.Name = theName;
                    SetSearchWordsForEntityName(theTblRow, false);
                    CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + objectID);
                    break;
                case TypeOfObject.HierarchyItem:
                    HierarchyItem theHierarchyItem = DataContext.GetTable<HierarchyItem>().Single(x => x.HierarchyItemID == objectID);
                    theHierarchyItem.HierarchyItemName = theName;
                    SetSearchWordsForHierarchyItem(theHierarchyItem, false);
                    CacheManagement.InvalidateCacheDependency("TopicsMenu");
                    break;
                case TypeOfObject.InsertableContent:
                    DataContext.GetTable<InsertableContent>().Single(x => x.InsertableContentID==objectID).Name = theName;
                    break;
                case TypeOfObject.RatingCharacteristics:
                    DataContext.GetTable<RatingCharacteristic>().Single(x => x.RatingCharacteristicsID==objectID).Name = theName;
                    break;
                case TypeOfObject.RatingGroupAttributes:
                    DataContext.GetTable<RatingGroupAttribute>().Single(x => x.RatingGroupAttributesID==objectID).Name = theName;
                    break;
                case TypeOfObject.RatingPhaseGroup:
                    DataContext.GetTable<RatingPhaseGroup>().Single(x => x.RatingPhaseGroupID==objectID).Name = theName;
                    break;
                case TypeOfObject.RatingPlan:
                    DataContext.GetTable<RatingPlan>().Single(x => x.RatingPlansID==objectID).Name = theName;
                    break;
                case TypeOfObject.RewardRatingSettings:
                    DataContext.GetTable<RewardRatingSetting>().Single(x => x.RewardRatingSettingsID == objectID).Name = theName;
                    break;
                case TypeOfObject.SubsidyDensityRangeGroup:
                    DataContext.GetTable<SubsidyDensityRangeGroup>().Single(x => x.SubsidyDensityRangeGroupID==objectID).Name = theName;
                    break;
                case TypeOfObject.PointsManager:
                    var thePointsManager = DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID==objectID);
                    thePointsManager.Name = theName;
                    CacheManagement.InvalidateCacheDependency("DomainID" + thePointsManager.DomainID);
                    CacheManagement.InvalidateCacheDependency("TopicsMenu");
                    break;
                default:
                    throw new Exception("Internal error -- trying to change system name of field without a system name");
            }
        }
       

       
    }
}
