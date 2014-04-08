using System;
using System.Data;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
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



using MoreStrings;
using GoogleGeocoder;
using System.Diagnostics;
using StringEnumSupport;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{    
    public class PMActionProcessor
    {
        RaterooDataManipulation _dataManipulation = null;

        public RaterooDataManipulation DataManipulation
        {
            get
            {
                if (null == _dataManipulation)
                {
                    _dataManipulation = new RaterooDataManipulation();
                }

                return _dataManipulation;
            }
        }


        RaterooDataAccess _dataAccess = null;
        public RaterooDataAccess DataAccess
        {
            get
            {
                if (null == _dataAccess)
                {
                    _dataAccess = new RaterooDataAccess();
                }
                return _dataAccess;
            }
        }


        public IRaterooDataContext DataContext
        {
            get
            {
                if (null == _dataManipulation)
                {
                    _dataManipulation = new RaterooDataManipulation();
                }
                return _dataManipulation.DataContext;
            }
            
        }
        
        public int DomainCreate(bool activeUserRatingWebsite, bool activeRatingWebsite, bool activeBuyingWebsite, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID, String name)
        {
            int? domainId = null;

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.Other, !makeActiveNow, null, null, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                domainId = DataManipulation.AddDomain(activeRatingWebsite, name, theUser);
                if (makeActive)
                {
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Domain, true, false, false, false, false, false, false, false, "", domainId, null, null, null, null, "", null);
                    if (makeActiveNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            else
                throw new Exception("Insufficient privileges");
            return (int) domainId;
        }

        public void DomainChangeSettings(int domainID, bool activeUserRatingWebsite, bool activeRatingWebsite, bool activeBuyingWebsite, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(domainID, TypeOfObject.Domain);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.Other, !doItNow, null, null, true))
            {
                int theNewSetting = 0;
                if (activeUserRatingWebsite)
                    theNewSetting += 1;
                if (activeRatingWebsite)
                    theNewSetting += 2;
                if (activeBuyingWebsite)
                    theNewSetting += 4;

                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Domain, false, false, false, false, true, false, false, false, "", null, domainID, null, theNewSetting, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public int PointsManagerCreate(int domainID, int? specializedSiteNum, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID, String name)
        {
            
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.Other, !makeActiveNow, null, null, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddPointsManager(domainID, name, theUser);

                if (makeActive)
                {
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.PointsManager, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);
                    if (makeActiveNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
                //Creating default user right for universe
                UsersRightsCreate(null, newObjectID, true, true, false, name+" Right", makeActive, makeActiveNow, userID, null);
            }
            else
            {
                throw new Exception("Insufficient privileges");
            }
            return (int) newObjectID;
        }

        public int RewardTblCreate(int pointsManagerID, decimal worstCasePenalty, decimal bestCaseReward, int runTime, int halfLife, decimal probOfRewardEvaluation, decimal? multiplier, decimal subsidyLevel, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, null, true))
            {
                int ratingPhaseGroup = DataManipulation.AddRatingPhaseGroup("Rewards Phases",userID);
                int ratingPhase1 = DataManipulation.AddRatingPhase(ratingPhaseGroup, subsidyLevel/5, ScoringRules.SquareRoot,true,false,null,runTime/5,0,false,null);
                int ratingPhase2 = DataManipulation.AddRatingPhase(ratingPhaseGroup, subsidyLevel/2, ScoringRules.SquareRoot,true,false,null,runTime/5,0,false,null);
                int ratingPhase3 =DataManipulation.AddRatingPhase(ratingPhaseGroup, subsidyLevel, ScoringRules.SquareRoot,true,false,null,3*runTime/5,halfLife,false,null);
                int ratingCharacteristics = DataManipulation.AddRatingCharacteristics(ratingPhaseGroup, null, worstCasePenalty, bestCaseReward, 1, "Reward or penalty", userID);
                int ratingGroupAttributes = DataManipulation.AddRatingGroupAttributes(ratingCharacteristics, null, null, "Reward or penalty attributes", RatingGroupTypes.singleNumber, "A table cell for assessing rewards or penalties", userID, pointsManagerID, true, true, 1.0M);
                int ratingPlan = DataManipulation.AddRatingPlan(ratingGroupAttributes, 1, 0, "0 default", "Rating", userID);
                int Tbl = DataManipulation.AddTbl((int)pointsManagerID,ratingGroupAttributes,"Group","Changes",null,false,true,"User Change","",true,true,"wf250","wf35");
                int TblTab = DataManipulation.AddTblTab(Tbl, 1, "Reward group");
                int  TblColumn = DataManipulation.AddTblColumn(TblTab, ratingGroupAttributes, 1, "Quality", "Quality of Change", "wv10", "A positive number indicates that the user made a good change, and a negative number indicates that the user made a bad change.<br> The rating should reflect the importance, extent, and accuracy of the change, relative to other changes of the same type.<br> The user will receive a reward or penalty based on this rating.",false);

                ChoiceGroupData theData = new ChoiceGroupData();
                theData.AddChoiceToGroup(StringEnum.GetStringValue(RewardableUserAction.AddRow));
                theData.AddChoiceToGroup(StringEnum.GetStringValue(RewardableUserAction.DeleteRow));
                theData.AddChoiceToGroup(StringEnum.GetStringValue(RewardableUserAction.UndeleteRow));
                theData.AddChoiceToGroup(StringEnum.GetStringValue(RewardableUserAction.ChangeInfo));
                theData.AddChoiceToGroup(StringEnum.GetStringValue(RewardableUserAction.ResolveTableCell));
                theData.AddChoiceToGroup(StringEnum.GetStringValue(RewardableUserAction.CancelResolve));
                int changeTypeChoiceGroup = ChoiceGroupCreate((int) pointsManagerID, theData, ChoiceGroupSettingsMask.GetStandardSetting(), null, true, true, userID, null, "Change choices"); 
                int FieldDefinition1 = FieldDefinitionCreate(Tbl, "Change Type", FieldTypes.ChoiceField, true, changeTypeChoiceGroup,null, true, false, userID, changesGroupID);
                int FieldDefinition2 = FieldDefinitionCreate(Tbl, "Table", FieldTypes.TextField, true, true, true, false, true, false, userID, changesGroupID);
                int FieldDefinition3 = FieldDefinitionCreate(Tbl, "Username", FieldTypes.TextField, true, true, false, false, true, false, userID, changesGroupID);
                int FieldDefinition4 = FieldDefinitionCreate(Tbl, "Date", FieldTypes.DateTimeField, true, true, true, true, false, userID, changesGroupID);
                int FieldDefinition5 = FieldDefinitionCreate(Tbl, "Description", FieldTypes.TextField, false, true, false, false, true, false, userID, changesGroupID);
                int FieldDefinition6 = FieldDefinitionCreate(Tbl, "Multiplier", FieldTypes.NumberField, false, 0, 1000M, 2, true, false, userID, changesGroupID);
                int rewardRatingSettings = DataManipulation.AddRewardRatingSettings((int) pointsManagerID,null,ratingGroupAttributes,probOfRewardEvaluation,multiplier,"Reward Settings",userID);

                DataManipulation.AddChangesStatusOfObject((int) changesGroupID, TypeOfObject.RatingPhaseGroup, true, false, false, false, false, false, false, false, "", ratingPhaseGroup, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPhase, true, false, false, false, false, false, false, false, "", ratingPhase1, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPhase, true, false, false, false, false, false, false, false, "", ratingPhase2, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPhase, true, false, false, false, false, false, false, false, "", ratingPhase3, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingCharacteristics, true, false, false, false, false, false, false, false, "", ratingCharacteristics, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingGroupAttributes, true, false, false, false, false, false, false, false, "", ratingGroupAttributes, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPlan, true, false, false, false, false, false, false, false, "", ratingPlan, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, true, false, false, false, false, false, false, false, "", Tbl, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblTab, true, false, false, false, false, false, false, false, "", TblTab, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, true, false, false, false, false, false, false, false, "", TblColumn, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceGroup, true, false, false, false, false, false, false, false, "", changeTypeChoiceGroup, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, true, false, false, false, false, false, false, false, "", FieldDefinition1, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, true, false, false, false, false, false, false, false, "", FieldDefinition2, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, true, false, false, false, false, false, false, false, "", FieldDefinition3, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, true, false, false, false, false, false, false, false, "", FieldDefinition4, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, true, false, false, false, false, false, false, false, "", FieldDefinition5, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, true, false, false, false, false, false, false, false, "", FieldDefinition6, null, null, null, null, "", null);
                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RewardRatingSettings, true, false, false, false, false, false, false, false, "", rewardRatingSettings, null, null, null, null, "", null);

                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);

                return Tbl;
            }
            else
                throw new Exception("Insufficient privileges");
        }


        public void RewardRatingCreate(int originalTblID, RewardableUserAction changeType, decimal? baseMultiplierOverride, decimal? supplementalMultiplier, int userID, string changeName, string changeDescription)
        {
            Tbl originalTbl = DataContext.GetTable<Tbl>().Single(c => c.TblID == originalTblID);
            Tbl userChangesTbl = DataContext.GetTable<Tbl>().SingleOrDefault(c => c.PointsManagerID == originalTbl.PointsManagerID && c.Name == "Changes");
            if (userChangesTbl == null)
                return;

            FieldSetDataInfo theData = DataManipulation.CreateFieldSetDataForRewardRating(originalTbl, userChangesTbl, changeType, baseMultiplierOverride, supplementalMultiplier, userID, changeName, changeDescription);
            theData.theEntityName = changeName;

            RewardRatingSetting theSetting = DataContext.GetTable<RewardRatingSetting>().SingleOrDefault(rms => rms.PointsManagerID == originalTbl.PointsManagerID && rms.Status == (Byte)StatusOfObject.Active);
            User theSuperUser = DataContext.GetTable<User>().SingleOrDefault(u => u.Username == "admin"); // this is created by admin, but a field above tracks the creating user for purpose of giving credit
            TblRow theRewardTblRow = TblRowCreateWithFields(theData, theSuperUser.UserID);

            if (theRewardTblRow != null && theSetting != null)
            {
                DataManipulation.UpdateRewardPointsPotentialMaxLossNotYetPending(userID, originalTbl.PointsManagerID, theRewardTblRow, true);
                DataManipulation.AddRewardPendingPointsTracker(theRewardTblRow, userID);
            }
        }

        public void RewardRatingSettingChange(int pointsManagerID, decimal probOfEvaluation, decimal multiplier, int userID)
        {
            PointsManager ptsManager = DataContext.GetTable<PointsManager>().Single(x => x.PointsManagerID == pointsManagerID);
            RewardRatingSetting rewardRatingSetting = ptsManager.RewardRatingSettings.FirstOrDefault(x => x.Status == (int) (StatusOfObject.Active));
            int? changesGroupID = null;
            if (!DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, false, pointsManagerID, null, false))
                throw new Exception("Insufficient user rights to change reward rating setting.");
            if (probOfEvaluation < 0 || probOfEvaluation > 1 || multiplier < 0)
                throw new Exception("Reward rating setting invalid numbers.");
            if (rewardRatingSetting != null)
            {
                rewardRatingSetting.ProbOfRewardEvaluation = probOfEvaluation;
                rewardRatingSetting.Multiplier = multiplier;
                DataContext.SubmitChanges();
            }
        }

        public void PointsManagerChangeSettings(int pointsManagerID, decimal? currentPeriodDollarSubsidy, DateTime? endOfDollarSubsidyPeriod, decimal? nextPeriodDollarSubsidy, int? nextPeriodLength, short? numPrizes, decimal? minimumPayment, bool doItNow, int userID, int? changesGroupID)
        {

            DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, null, true))
            {

                int newPointsManagerID = DataManipulation.AddPointsManagerNewSettings(pointsManagerID, currentPeriodDollarSubsidy, endOfDollarSubsidyPeriod, nextPeriodDollarSubsidy, nextPeriodLength, numPrizes, minimumPayment);

                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.PointsManager, false, false, true, false, false, false, false, false, "", newPointsManagerID, pointsManagerID, null, null, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
                PMCacheManagement.InvalidateCacheDependency("InsertableContent");
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void PointsManagerGuaranteeSettings(int pointsManagerID, decimal dollarValuePerPoint, decimal discountForGuarantees, decimal maximumTotalGuarantees, bool allowApplicationsWhenNoConditionalGuaranteesAvailable, bool conditionalGuaranteesAvailableForNewUsers, bool conditionalGuaranteesAvailableForExistingUsers, int conditionalGuaranteeTimeBlockInHours, decimal maximumGuaranteePaymentPerHour, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, null, true))
            {

                DataManipulation.GuaranteeSettings(pointsManagerID, dollarValuePerPoint, discountForGuarantees, maximumTotalGuarantees, allowApplicationsWhenNoConditionalGuaranteesAvailable, conditionalGuaranteesAvailableForNewUsers, conditionalGuaranteesAvailableForExistingUsers, conditionalGuaranteeTimeBlockInHours, maximumGuaranteePaymentPerHour);

                if (!doItNow)
                    throw new Exception("Points manager guarantee settings currently must be implemented immediately.");

                DataManipulation.DataContext.SubmitChanges();
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void PointsManagerHighStakesSettings(int pointsManagerID, decimal highStakesProbability, decimal highStakesMultiplierSecret, decimal highStakesMultiplierKnown, bool highStakesNoviceOn, int highStakesNoviceNumAutomatic, int highStakesNoviceNumOneThird, int highStakesNoviceNumOneTenth, int highStakesNoviceTargetNum, decimal databaseChangeSelectHighStakesNoviceNumPct, bool doItNow, int userID, int? changesGroupID)
        {

            DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, null, true))
            {

                DataManipulation.HighStakesSettings(pointsManagerID, highStakesProbability, highStakesMultiplierSecret, highStakesMultiplierKnown, highStakesNoviceOn, highStakesNoviceNumAutomatic, highStakesNoviceNumOneThird, highStakesNoviceNumOneTenth, highStakesNoviceTargetNum, databaseChangeSelectHighStakesNoviceNumPct);

                if (!doItNow)
                    throw new Exception("High stakes settings currently must be implemented immediately.");


                DataManipulation.DataContext.SubmitChanges();
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public int TblCreate(int pointsManagerID, int? defaultRatingGroupAttributesID, string TblTabWord, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID, String name, bool AllowOverrideOfRatingCharacterstics, bool OneRatingPerRatingGroup, string TypeOfTblRow, string entityAdditionCriteria, bool AllowUsersToAddComments, bool LimitCommentsToUsersWhoCanMakeUserRatings, string widthStyleEntityCol, string widthStyleNumCol)
        {
            
            int? newObjectID = null;
            if (name == "Changes")
                throw new Exception("The name 'Changes' is reserved and cannot be used.");
            DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !makeActiveNow, pointsManagerID, null, true))
            {
                //DataAccessModule.ConfirmObjectExists(defaultRatingGroupAttributesID, TypeOfObject.RatingGroupAttributes);
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddTbl(pointsManagerID, defaultRatingGroupAttributesID, TblTabWord, name, theUser, AllowOverrideOfRatingCharacterstics, OneRatingPerRatingGroup, TypeOfTblRow, entityAdditionCriteria, AllowUsersToAddComments, LimitCommentsToUsersWhoCanMakeUserRatings, widthStyleEntityCol, widthStyleNumCol);
                if (makeActive)
                {
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);
                    if (makeActiveNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            else
                throw new Exception("Insufficient privileges");

            return (int)newObjectID;
        }

        public void TblChangeTblTabWord(int TblID, string TblTabWord, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            int pointsManagerID = DataManipulation.DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID).PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, TblID, true))
            {
                if (TblTabWord.Length >= 50)
                    throw new Exception("The word to describe the category group must be less than 50 characters.");

                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, false, false, false, false, true, true, false, false, "", null, TblID, null, null, null, TblTabWord, null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void TblChangeStyles(int TblID, string suppStylesHeader, string suppStylesMain, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            int pointsManagerID = DataManipulation.DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID).PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, TblID, true))
            {
                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, false, false, false, false, true, true, true, false, "", null, TblID, null, null, null, suppStylesHeader + "&" + suppStylesMain, null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void TblChangeTypeOfTblRow(int TblID, string typeOfTblRow, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            int pointsManagerID = DataManipulation.DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID).PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, TblID, true))
            {
                if (typeOfTblRow.Length >= 50)
                    throw new Exception("The word to describe the type of entity must be less than 50 characters.");

                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, false, false, false, false, false, false, true, false, "", null, TblID, null, null, null, typeOfTblRow, null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }





        public int ChoiceGroupCreate(int pointsManagerID, ChoiceGroupData theChoiceGroupData, int choiceGroupSettings,int? DependentOnChoiceGroupID, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID, String name)
        {
            
            int? newChoiceGroupID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeChoiceGroups, !makeActiveNow, pointsManagerID, null, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;

                // first, add the choice group
                newChoiceGroupID = DataManipulation.AddChoiceGroup(pointsManagerID, choiceGroupSettings,DependentOnChoiceGroupID, name);
                if (makeActive)
                    DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceGroup, true, false, false, false, false, false, false, false, "", newChoiceGroupID, null, null, null, null, "", null);

                // now, add the contents
                int numItems = theChoiceGroupData.Count;
                for (int i = 1; i <= numItems; i++)
                {
                    int newChoiceInGroupID = DataManipulation.AddChoiceInGroup((int)newChoiceGroupID, i, theChoiceGroupData.TheData[i-1].text, theChoiceGroupData.TheData[i-1].determiningGroupValue);
                    if (makeActive)
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceInGroup, true, false, false, false, false, false, false, false, "", newChoiceInGroupID, null, null, null, null, "", null);
                }

                // now, activate.
                if (makeActiveNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
            return (int)newChoiceGroupID;
        }

        public void ChoiceGroupChange(int choiceGroupID, ChoiceGroupData newChoiceGroup, bool doItNow, int userID, int? changesGroupID)
        {

            DataManipulation.ConfirmObjectExists(choiceGroupID, TypeOfObject.ChoiceGroup);
            int pointsManagerID = DataAccess.GetChoiceGroup(choiceGroupID).PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeChoiceGroups, !doItNow, pointsManagerID, null, true))
            {

                // Load the entire original choice group and see if the changes are acceptable.
                ChoiceGroupData originalChoiceGroup = DataAccess.GetChoiceGroupData(choiceGroupID, false, null);
                newChoiceGroup.Sort();
                if (!originalChoiceGroup.ChangesAreValid(newChoiceGroup))
                    throw new Exception("Internal error -- invalid changes to choice group. All previous elements must be kept.");

                // Now, let's go through each element of the new choice group, and see when we need to make a change.
                foreach (ChoiceInGroupData newChoiceInGroup in newChoiceGroup.TheData)
                {
                    if (newChoiceInGroup.choiceInGroupID == null)
                    { // This one is new -- let's add it to the database.
                        int newChoiceInGroupID = DataManipulation.AddChoiceInGroup((int)choiceGroupID, newChoiceInGroup.choiceNum, newChoiceInGroup.text, newChoiceInGroup.determiningGroupValue);
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceInGroup, true, false, false, false, false, false, false, false, "", newChoiceInGroupID, null, null, null, null, "", null);
                    }
                    else
                    { // This one is old -- but let's see if there are any changes to it, though.
                        ChoiceInGroupData originalChoiceInGroup = originalChoiceGroup.TheData.Single(cigd => cigd.choiceInGroupID == newChoiceInGroup.choiceInGroupID);

                        if (newChoiceInGroup.choiceNum != originalChoiceInGroup.choiceNum)
                        {
                            int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceInGroup, false, false, false, false, true, true, false, false, "", null, originalChoiceInGroup.choiceInGroupID, null, newChoiceInGroup.choiceNum, null, "", null);
                        }
                        if (newChoiceInGroup.determiningGroupValue != originalChoiceInGroup.determiningGroupValue)
                        {
                            int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceInGroup, false, false, false, false, true, false, true, false, "", null, originalChoiceInGroup.choiceInGroupID, null, newChoiceInGroup.determiningGroupValue, null, "", null);
                        }
                        if (newChoiceInGroup.text != originalChoiceInGroup.text)
                        {
                            int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceInGroup, false, false, false, false, true, true, true, false, "", null, originalChoiceInGroup.choiceInGroupID, null, null, null, newChoiceInGroup.text, null);

                        }
                        if (newChoiceInGroup.isAvailable != originalChoiceInGroup.isAvailable)
                        {
                            int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceInGroup, false, false, false, false, true, false, false, false, "", null, originalChoiceInGroup.choiceInGroupID, newChoiceInGroup.isAvailable, null, null, "", null);
                        }
                    }
                }
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public int FieldDefinitionCreate(int TblID,  String fieldName, FieldTypes fieldType, bool useAsFilter, int? choiceGroupID, int? dependentOnChoiceGroupFieldDefinitionID, decimal? minimum, decimal? maximum, short? decimalPlaces, bool? includeDate, bool? includeTime, bool? includeText, bool? includeLink, bool? searchableTextField, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            
            int? FieldDefinitionID = null;
            DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, !makeActiveNow, null, TblID, true))
            {
                if (fieldName.Length > 50)
                    throw new Exception("Field name too long -- Maximum length is 50 characters.");
                if (fieldType == FieldTypes.ChoiceField)
                {
                    DataManipulation.ConfirmObjectExists(choiceGroupID, TypeOfObject.ChoiceGroup);
                    if (dependentOnChoiceGroupFieldDefinitionID != null)
                        DataManipulation.ConfirmObjectExists(dependentOnChoiceGroupFieldDefinitionID, TypeOfObject.ChoiceGroupFieldDefinition);
                }
                else if (fieldType == FieldTypes.NumberField)
                {
                    if (minimum != null && maximum != null && (decimal)minimum >= (decimal)maximum)
                        throw new Exception("Maximum must be greater than minimum.");
                    if (decimalPlaces == null || decimalPlaces < 0 || decimalPlaces > 4)
                        throw new Exception("Number of decimal places must be between 0 and 4.");
                }
                else if (fieldType == FieldTypes.DateTimeField)
                {
                    if (includeDate == null || includeTime == null)
                        throw new Exception("Must specify whether to include the date and/or time.");
                }
                else if (!(fieldType == FieldTypes.AddressField || fieldType == FieldTypes.TextField))
                    throw new Exception("Internal error -- unknown field type.");

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                var existingFields = DataManipulation.DataContext.GetTable<FieldDefinition>().Where(fd => fd.TblID == TblID);
                int fieldNum;
                if (existingFields.Any())
                    fieldNum = DataManipulation.DataContext.GetTable<FieldDefinition>().Where(fd => fd.TblID == TblID).Max(fd => fd.FieldNum) + 1;
                else
                    fieldNum = 1;
                FieldDefinitionID = DataManipulation.AddFieldDefinition(TblID, fieldNum, fieldName, fieldType, useAsFilter);
                if (makeActive)
                    DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, true, false, false, false, false, false, false, false, "", FieldDefinitionID, null, null, null, null, "", null);
                if (fieldType == FieldTypes.ChoiceField)
                {
                    int choiceGroupFieldDefinitionID = DataManipulation.AddChoiceGroupFieldDefinition((int) choiceGroupID,(int)FieldDefinitionID,dependentOnChoiceGroupFieldDefinitionID);
                    if (makeActive)
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ChoiceGroupFieldDefinition, true, false, false, false, false, false, false, false, "",choiceGroupFieldDefinitionID, null, null, null, null, "", null);
                }
                else if (fieldType == FieldTypes.NumberField)
                {
                    int numberFieldDefinitionID = DataManipulation.AddNumberFieldDefinition((int)FieldDefinitionID, minimum, maximum, (short) decimalPlaces);
                    if (makeActive)
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.NumberFieldDefinition, true, false, false, false, false, false, false, false, "", numberFieldDefinitionID, null, null, null, null, "", null);
                }
                else if (fieldType == FieldTypes.DateTimeField)
                {
                    int dateTimeFieldDefinitionID = DataManipulation.AddDateTimeFieldDefinition((int)FieldDefinitionID, (bool)includeDate, (bool)includeTime);
                    if (makeActive)
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.DateTimeFieldDefinition, true, false, false, false, false, false, false, false, "", dateTimeFieldDefinitionID, null, null, null, null, "", null);
                }
                else if (fieldType == FieldTypes.TextField)
                {
                    int textFieldDefinitionID = DataManipulation.AddTextFieldDefinition((int)FieldDefinitionID, (bool)includeText, (bool)includeLink, (bool)searchableTextField);
                    if (makeActive)
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TextFieldDefinition, true, false, false, false, false, false, false, false, "", textFieldDefinitionID, null, null, null, null, "", null);
                }

                if (makeActive && makeActiveNow)
                {
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            else
                throw new Exception("Insufficient privileges");
            return (int)FieldDefinitionID;
        }

        public int FieldDefinitionCreate(int TblID, String fieldName, FieldTypes fieldType, bool useAsFilter, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            return FieldDefinitionCreate(TblID, fieldName, fieldType, useAsFilter, null, null, null, null, null, null, null, null, null, null, makeActive, makeActiveNow, userID, changesGroupID);
        }

        public int FieldDefinitionCreate(int TblID, String fieldName, FieldTypes fieldType, bool useAsFilter, int? choiceGroupID, int? dependentOnChoiceGroupFieldDefinitionID, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            return FieldDefinitionCreate(TblID, fieldName, fieldType, useAsFilter, choiceGroupID, dependentOnChoiceGroupFieldDefinitionID, null, null, null, null, null, null, null, null, makeActive, makeActiveNow, userID, changesGroupID);
        }

        public int FieldDefinitionCreate(int TblID, String fieldName, FieldTypes fieldType, bool useAsFilter, decimal? minimum, decimal? maximum, short? decimalPlaces, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            return FieldDefinitionCreate(TblID, fieldName, fieldType, useAsFilter, null, null, minimum, maximum, decimalPlaces, null, null, null, null, null, makeActive, makeActiveNow, userID, changesGroupID);
        }

        public int FieldDefinitionCreate(int TblID, String fieldName, FieldTypes fieldType, bool useAsFilter, bool? includeDate, bool? includeTime, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            return FieldDefinitionCreate(TblID, fieldName, fieldType, useAsFilter, null, null, null, null, null, includeDate, includeTime, null, null, null, makeActive, makeActiveNow, userID, changesGroupID);
        }

        public int FieldDefinitionCreate(int TblID, String fieldName, FieldTypes fieldType, bool useAsFilter, bool? includeText, bool? includeLink, bool? searchable, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            return FieldDefinitionCreate(TblID, fieldName, fieldType, useAsFilter, null, null, null, null, null, null, null, includeText, includeLink, searchable, makeActive, makeActiveNow, userID, changesGroupID);
        }

        public void FieldDefinitionChangeSettings(int FieldDefinitionID, string fieldName, bool? useAsFilter, bool doItNow, int userID, int? changesGroupID)
        {
            
            DataManipulation.ConfirmObjectExists(FieldDefinitionID, TypeOfObject.FieldDefinition);
            int TblID = DataAccess.GetFieldDefinition(FieldDefinitionID).TblID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, null, TblID, true))
            {
                if (fieldName.Length >= 50)
                    throw new Exception("The field name must be less than 50 characters.");

                if (fieldName != "")
                {
                    int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, false, false, false, false, true, false, false, false, "", null, FieldDefinitionID, null, null, null, fieldName, null);
                    if (doItNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
                if (useAsFilter != null)
                {
                    int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, false, false, false, false, true, false, false, false, "", null, FieldDefinitionID, useAsFilter, null, null, "", null);
                    if (doItNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void FieldDefinitionChangeDisplaySettings(int FieldDefinitionID, int displayInTableSetting, int displayInPopUpSetting, int displayInTblRowPageSetting, bool doItNow, int userID, int? changesGroupID)
        {
            FieldDefinitionChangeDisplaySettings(new List<int> { FieldDefinitionID }, new List<int> { displayInTableSetting }, new List<int> { displayInPopUpSetting }, new List<int> { displayInTblRowPageSetting }, doItNow, userID, changesGroupID);
        }

        public void FieldDefinitionChangeDisplaySettings(List<int> FieldDefinitionID, List<int> displayInTableSetting, List<int> displayInPopUpSetting, List<int> displayInTblRowPageSetting, bool doItNow, int userID, int? changesGroupID)
        {
            if (FieldDefinitionID.Count() == 0)
                return;

            foreach (int FID in FieldDefinitionID)
            {
                DataManipulation.ConfirmObjectExists(FID, TypeOfObject.FieldDefinition);
            }

            int TblID = DataAccess.GetFieldDefinition((int)FieldDefinitionID[0]).TblID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, !doItNow, null, TblID, true))
            {
                int? changesStatusObjectID = null;
                for (int i = 0; i < FieldDefinitionID.Count; i++)
                {
                    if ((int)displayInTableSetting[i] != -1)
                    {
                        changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, false, false, false, false, true, false, true, false, "", null, (int)FieldDefinitionID[i], null, (int)displayInTableSetting[i], null, "DisplayInTableSetting", null);
                    }
                    if ((int)displayInPopUpSetting[i] != -1)
                    {
                        changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, false, false, false, false, true, false, true, false, "", null, (int)FieldDefinitionID[i], null, (int)displayInPopUpSetting[i], null, "DisplayInPopUpSetting", null);
                    }
                    if ((int)displayInTblRowPageSetting[i] != -1)
                    {
                        changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, false, false, false, false, true, false, true, false, "", null, (int)FieldDefinitionID[i], null, (int)displayInTblRowPageSetting[i], null, "DisplayInTblRowPageSetting", null);
                    }

                }

                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");

        }
        public void FieldDefinitionDeleteOrUndelete(int FieldDefinitionID, bool delete, bool doItNow, int userID, int? changesGroupID)
        {
            
            DataManipulation.ConfirmObjectExists(FieldDefinitionID, TypeOfObject.FieldDefinition);
            int TblID = DataAccess.GetFieldDefinition(FieldDefinitionID).TblID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, !doItNow, null, TblID, true))
            {
                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.FieldDefinition, false, true, false, false, false, delete, false, false, "", null, FieldDefinitionID, null, null, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
        }

        public TblRow TblRowCreate(int TblID, int userID, int? changesGroupID, String name, List<UserSelectedRatingInfo> theRatingTypeOverrides = null)
        {
            // Note that the creation of an entity is not part of a changes group, since we need to create them before assigning IDs.
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, false, null, TblID, false))
            {
                Tbl theTbl = DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID);
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;

                TblRow theTblRow = DataManipulation.AddTblRow(theTbl, name, theRatingTypeOverrides);
                theTblRow.Status = (int) (StatusOfObject.Active);
                return theTblRow;
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public TblRow TblRowCreateWithFields(FieldSetDataInfo theFieldSetDataInfo, int userID, List<UserSelectedRatingInfo> theRatingTypeOverrides = null)
        {
            if (theFieldSetDataInfo.theTbl == null)
                throw new Exception("Tbl must be specified before creating entity.");
             int? changesGroupID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, false, null, theFieldSetDataInfo.theTbl.TblID, false))
            {

                //ProfileSimple.Start("TblRowCreate");
                TblRow theTblRow = TblRowCreate(theFieldSetDataInfo.theTbl.TblID, userID, changesGroupID, theFieldSetDataInfo.theEntityName, theRatingTypeOverrides);
                theFieldSetDataInfo.theTblRow = theTblRow;
                //ProfileSimple.End("TblRowCreate");
                //ProfileSimple.Start("FieldSetImplement");
                FieldSetImplement(theFieldSetDataInfo, userID, false, false);
                //ProfileSimple.End("FieldSetImplement");
                //ProfileSimple.Start("RewardInTblRowCreateWithFields");
                decimal? baseMultiplierOverride;
                var shouldCreateReward = DataManipulation.ShouldCreateRewardRating((int)theFieldSetDataInfo.theTbl.TblID, userID, out baseMultiplierOverride);
                if (shouldCreateReward)
                    RewardRatingCreate((int)theFieldSetDataInfo.theTbl.TblID, RewardableUserAction.AddRow, baseMultiplierOverride, 1M, userID, "Added row: " + theFieldSetDataInfo.theEntityName, theFieldSetDataInfo.GetDescription());

                //ProfileSimple.End("RewardInTblRowCreateWithFields");
                return theTblRow;
            }
            return null;
        }

        public void FieldSetImplement(FieldSetDataInfo theSet, int userID, bool activeFieldDefinitionsOnly, bool considerCreatingRewardRating)
        {
            //ProfileSimple.Start("FieldSetImplement");
            //ProfileSimple.Start("Verify");
            theSet.VerifyCanBeAdded();
            //ProfileSimple.End("Verify");
            int? changesGroupID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, false, null, theSet.theTbl.TblID, false))
            {
                theSet.theTblRow.Name = theSet.theEntityName;
                theSet.IdentifyEmptyFields(activeFieldDefinitionsOnly); // Identify the empty fields (which need to be deleted if they exist)
                //ProfileSimple.Start("FieldDelete and FieldChange");
                foreach (var theEmptyFieldDefinition in theSet.theEmptyFieldDefinitions)
                {
                    FieldDelete(theSet.theTblRow, theEmptyFieldDefinition, userID);
                }
                foreach (FieldDataInfo theFieldData in theSet.theFieldDataInfos)
                {
                    FieldChange(theFieldData, userID);
                }

                if (theSet.theTblRow.TblRowID != 0)
                {
                    FieldsDisplayCreator theFieldsDisplayCreator = new FieldsDisplayCreator();
                    theFieldsDisplayCreator.SetFieldDisplayHtml(theSet.theTblRow);      
                    FastAccessTablesMaintenance.IdentifyRowRequiringUpdate(DataContext, theSet.theTbl, theSet.theTblRow, theSet.defaultTblVals != null, true); 
                }
                else
                {
                    theSet.theTblRow.TblRowFieldDisplay.ResetNeeded = true;
                    // Once this reset is processed, the row will be updated again. This means that initially, we will move to the FastSQL the fields
                    // without the field display htmls, and then later recopy all the fields, plus the field display htmls that have been set.
                }

                if (theSet.defaultTblVals != null)
                { // set the tbl vals to these values
                    var ratingsList = theSet.theTblRow.RatingGroups.SelectMany(x => x.Ratings).ToList();
                    foreach (TblVal tblVal in theSet.defaultTblVals)
                    {
                        Rating theRating = ratingsList.SingleOrDefault(x => x.RatingGroup.TblColumn.Name == tblVal.TblCol && x.NumInGroup == tblVal.NumInGroup);
                        if (theRating != null)
                        {
                            //Trace.TraceInformation("1Setting current value to " + tblVal.CurrentValue);
                            theRating.CurrentValue = theRating.LastTrustedValue = tblVal.CurrentValue;
                            if (theRating.NumInGroup == 1)
                                theRating.RatingGroup.CurrentValueOfFirstRating = theRating.CurrentValue;
                            theRating.LastModifiedResolutionTimeOrCurrentValue = TestableDateTime.Now;
                        }
                    }
                }

                //ProfileSimple.End("FieldDelete and FieldChange");
            }
            //ProfileSimple.Start("RewardRating");
            decimal? baseMultiplierOverride;
            if (considerCreatingRewardRating && DataManipulation.ShouldCreateRewardRating(theSet.theTbl.TblID, userID, out baseMultiplierOverride))
            {
                FieldSetDataInfo oldSettings = new FieldSetDataInfo(theSet.theTblRow, theSet.theTbl, new RaterooDataAccess());
                oldSettings.LoadFromDatabase();
                string changesList = theSet.GetComparison(oldSettings);
                RewardRatingCreate((int)theSet.theTbl.TblID, RewardableUserAction.ChangeInfo, baseMultiplierOverride, 0.75M, userID, "Changed info: " + theSet.theEntityName, changesList);
            }
            //ProfileSimple.End("RewardRating");
            //ProfileSimple.End("FieldSetImplement");
        }

        internal void EntityNameChange(int entityID, string newName, int userID, int changesGroupID)
        {
            // Assumes that change has already been approved. Will not implement the change.
            string currentName = DataManipulation.DataContext.GetTable<TblRow>().Single(e => e.TblRowID == entityID).Name;
            if (currentName != newName)
                DataManipulation.AddChangesStatusOfObject(changesGroupID, TypeOfObject.TblRow, false, false, false, true, false, false, false, false, newName, null, entityID, null, null, null, "", null);
        }

        internal void FieldChange(FieldDataInfo theFieldData, int userID)
        {
            // Assumes that change has already been approved. Will not implement the change.
            if (!theFieldData.MatchesDatabase())
            {
                Field theField = null;
                theField = FieldClearSubfield(true, theFieldData.TheGroup.theTblRow, theFieldData.TheFieldDefinition.FieldDefinitionID, userID);
                if (theField == null)
                    theField = DataManipulation.GetFieldForTblRow(theFieldData.TheGroup.theTblRow, theFieldData.TheFieldDefinition);
                if (theFieldData is AddressFieldDataInfo)
                {
                    decimal? latitude = null;
                    decimal? longitude = null;
                    string theAddress = ((AddressFieldDataInfo)theFieldData).TheAddress;
                    //Disable immediate geocoding, we'll geocode later
                    //Geocode theGeocoder = new Geocode();
                    //Coordinate theCoordinate = Geocode.GetCoordinatesAndReformatAddress(ref theAddress);
                    //if (theCoordinate.Latitude != 0 || theCoordinate.Longitude != 0)
                    //{
                    //    latitude = theCoordinate.Latitude;
                    //    longitude = theCoordinate.Longitude;
                    //}
                    AddressField addressField = DataManipulation.AddAddressField(theField, theAddress, latitude, longitude);

                }
                else if (theFieldData is TextFieldDataInfo)
                {
                    TextField textField = DataManipulation.AddTextField(theField, ((TextFieldDataInfo)theFieldData).TheText, ((TextFieldDataInfo)theFieldData).TheLink, ((TextFieldDataInfo)theFieldData).TheFieldDefinition.TextFieldDefinitions.Single().Searchable);
                }
                else if (theFieldData is ChoiceFieldDataInfo)
                {
                    ChoiceField choiceField = DataManipulation.AddChoiceField(theField);

                    foreach (ChoiceInGroup choiceInGroup in ((ChoiceFieldDataInfo)theFieldData).TheChoices)
                    {
                        DataManipulation.AddChoiceInField(choiceField, choiceInGroup);
                    }
                }
                else if (theFieldData is DateTimeFieldDataInfo)
                {
                    DataManipulation.AddDateTimeField(theField, ((DateTimeFieldDataInfo)theFieldData).TheDateTime);
                }
                else if (theFieldData is NumericFieldDataInfo)
                {
                    DataManipulation.AddNumberField(theField, ((NumericFieldDataInfo)theFieldData).TheNumber);
                }
            }

        }

        internal void FieldDelete(TblRow entity, FieldDefinition theFieldDefinition, int userID)
        {
            if (entity.TblRowID == 0)
                return; // Shouldn't be a field to delete for entity that hasn't been added yet.
            Field theField = DataManipulation.GetFieldForTblRow(entity, theFieldDefinition);
            if (theField != null)
            {
                FieldClearSubfield(false, entity, theFieldDefinition.FieldDefinitionID, userID);
                theField.Status = (int)StatusOfObject.Unavailable;
            }
        }

        internal Field FieldClearSubfield(bool addFieldIfNotExists, TblRow entity, int FieldDefinitionID, int userID)
        {
            // We assume that change has already been approved and the changes group will be implemented by caller.
            Field field = null;
            object subfield = null;
            FieldTypes theFieldType = FieldTypes.AddressField; // must initialize before passing as ref below
            if (entity.TblRowID != 0)
                DataManipulation.GetFieldForTblRow(entity, FieldDefinitionID, ref field, ref subfield, ref theFieldType);
            if (field == null)
            {
                if (addFieldIfNotExists) // Create a new field
                {
                    FieldDefinition theFieldDefinition = DataContext.GetTable<FieldDefinition>().Single(fd => fd.FieldDefinitionID == FieldDefinitionID);
                    field = DataManipulation.AddField(entity, theFieldDefinition);
                }
            }
            else
            { // Delete the subfield
                if (subfield != null)
                {
                    switch (theFieldType)
                    {
                        case FieldTypes.AddressField:
                            ((AddressField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.ChoiceField:
                            ((ChoiceField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.DateTimeField:
                            ((DateTimeField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.NumberField:
                            ((NumberField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.TextField:
                            ((TextField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                    }

                }
            }
            return field;
        }

        public Field FieldCreateOrReplace(TblRow entity, int FieldDefinitionID, String textValue, String linkValue, int? singleChoice, List<int> multipleChoices, decimal? numericValue, decimal? latitude, decimal? longitude, DateTime? dateTimeValue, int userID, int? changesGroupID)
        {
            Field theField = null;
            int? TblID = entity.TblID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, false, null, TblID, false))
            {
                FieldDefinition theFieldDefinition = DataManipulation.DataContext.GetTable<FieldDefinition>().SingleOrDefault(fd => fd.FieldDefinitionID == FieldDefinitionID);
                if (theFieldDefinition == null)
                    throw new Exception("Error -- field definition does not exist for entity to be added.");

                NumberFieldDefinition theNumberFieldDefinition = null;
                ChoiceGroupFieldDefinition theChoiceGroupFieldDefinition = null;
                DateTimeFieldDefinition theDateTimeFieldDefinition = null;
                TextFieldDefinition theTextFieldDefinition = null;

                if ((FieldTypes) theFieldDefinition.FieldType == FieldTypes.NumberField)
                {
                    theNumberFieldDefinition = DataManipulation.DataContext.GetTable<NumberFieldDefinition>().Single(nfd => nfd.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                    if (numericValue != null && (numericValue > theNumberFieldDefinition.Maximum || numericValue < theNumberFieldDefinition.Minimum))
                        throw new Exception ("Number does not fall within required range.");
                }
                else if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.ChoiceField)
                {
                    theChoiceGroupFieldDefinition = DataManipulation.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(cfd => cfd.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                    if (!theChoiceGroupFieldDefinition.ChoiceGroup.AllowMultipleSelections)
                    {
                        if (singleChoice != null)
                            ((List<int>) multipleChoices).Add((int) singleChoice);
                    }
                    if (multipleChoices != null)
                    {
                        foreach (int theChoice in ((List<int>) multipleChoices))
                            if (DataManipulation.DataContext.GetTable<ChoiceInGroup>().Where(cig => cig.ChoiceGroupID == theChoiceGroupFieldDefinition.ChoiceGroup.ChoiceGroupID && cig.ChoiceInGroupID == theChoice).Count() != 1)
                                throw new Exception("Nonexistent choice selected.");
                    }
                }
                else if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.DateTimeField)
                {
                    theDateTimeFieldDefinition = DataManipulation.DataContext.GetTable<DateTimeFieldDefinition>().Single(dtfd => dtfd.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                }
                else if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.TextField)
                {
                    theTextFieldDefinition = DataManipulation.DataContext.GetTable<TextFieldDefinition>().Single(tfd => tfd.FieldDefinitionID == theFieldDefinition.FieldDefinitionID);
                }

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;

                bool fieldAlreadyExists = true; // if this stays true, we must delete the existing field
                object subfield = null;
                FieldTypes theFieldType = FieldTypes.AddressField; // must initialize
                DataManipulation.GetFieldForTblRow(entity, theFieldDefinition.FieldDefinitionID, ref theField, ref subfield, ref theFieldType);             
                if (theField == null)
                {
                    fieldAlreadyExists = false;
                    theField = DataManipulation.AddField(entity, theFieldDefinition);
                }
                
                if (fieldAlreadyExists)
                { // We'll delete the subfield (e.g., the AddressField) so that we can add a new subfield below.
                    switch (theFieldType)
                    {
                        case FieldTypes.AddressField:
                            ((AddressField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.ChoiceField:
                            ((ChoiceField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.DateTimeField:
                            ((DateTimeField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.NumberField:
                            ((NumberField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                        case FieldTypes.TextField:
                            ((TextField)subfield).Status = (int)StatusOfObject.Unavailable;
                            break;
                    }
                    
                }

                if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.AddressField)
                {
                    DataManipulation.AddAddressField(theField, textValue, latitude, longitude);
                }
                else if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.ChoiceField)
                {
                    ChoiceField theChoiceField = DataManipulation.AddChoiceField(theField);
                    
                    foreach (int choiceInGroupID in ((List<int>) multipleChoices))
                    {
                        //int choiceInGroupID = DataAccessModule.RaterooDB.GetTable<ChoiceInGroup>().Single(cig => cig.ChoiceGroupID == theChoiceGroupFieldDefinition.ChoiceGroup.ChoiceGroupID && cig.ChoiceNum == theChoice).ChoiceInGroupID;
                        ChoiceInGroup choiceInGroup = DataContext.GetTable<ChoiceInGroup>().Single(x => x.ChoiceInGroupID == choiceInGroupID);
                        ChoiceInField choiceInField = DataManipulation.AddChoiceInField(theChoiceField, choiceInGroup);
                    }
                }
                else if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.DateTimeField)
                {
                     DataManipulation.AddDateTimeField(theField, dateTimeValue);
                }
                else if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.NumberField)
                {
                     DataManipulation.AddNumberField(theField, numericValue);
                }
                else if ((FieldTypes)theFieldDefinition.FieldType == FieldTypes.TextField)
                {
                    if (!theTextFieldDefinition.IncludeText)
                        textValue = "Link";
                    else if (!theTextFieldDefinition.IncludeLink)
                        linkValue = "";
                     DataManipulation.AddTextField(theField, textValue, linkValue, theTextFieldDefinition.Searchable);
                }
                else
                    throw new Exception("Unknown field type.");

                DataManipulation.ResetTblRowFieldDisplay(entity);
                PMCacheManagement.InvalidateCacheDependency("FieldForTblRowID" + entity.TblRowID);
            }
            else
                throw new Exception("Insufficient privileges");

            return theField;
        }

        public Field AddressFieldCreateOrReplace(TblRow entity, int FieldDefinitionID, String textValue, decimal latitude, decimal longitude, int userID, int? changesGroupID)
        {
            List<int> multipleChoicesList = new List<int>();
            return FieldCreateOrReplace(entity, FieldDefinitionID, textValue, "", null, multipleChoicesList, null, latitude, longitude, null,  userID, changesGroupID);
        }

        public Field TextFieldCreateOrReplace(TblRow entity, int FieldDefinitionID, String textValue, int userID, int? changesGroupID)
        {
            List<int> multipleChoicesList = new List<int>();
            return FieldCreateOrReplace(entity, FieldDefinitionID, textValue, "", null, multipleChoicesList, null, null, null, null,  userID, changesGroupID);
        }

        public Field TextWithLinkFieldCreateOrReplace(TblRow entity, int FieldDefinitionID, String textValue, String linkValue,  int userID, int? changesGroupID)
        {
            List<int> multipleChoicesList = new List<int>();
            return FieldCreateOrReplace(entity, FieldDefinitionID, textValue, linkValue, null, multipleChoicesList, null, null, null, null,  userID, changesGroupID);
        }
        public Field TextFieldLinkOnlyCreateOrReplace(TblRow entity, int FieldDefinitionID, String linkValue, int userID, int? changesGroupID)
        {
            List<int> multipleChoicesList = new List<int>();
            return FieldCreateOrReplace(entity, FieldDefinitionID, "Link", linkValue, null, multipleChoicesList, null, null, null, null, userID, changesGroupID);
        }

        public Field ChoiceFieldWithSingleChoiceCreateOrReplace(TblRow entity, int FieldDefinitionID, int? singleChoice, int userID, int? changesGroupID)
        {
            List<int> multipleChoicesList = new List<int>();
            return FieldCreateOrReplace(entity, FieldDefinitionID, null, null, singleChoice, multipleChoicesList, null, null, null, null, userID, changesGroupID);

        }
       public Field ChoiceFieldWithMultipleChoicesCreateOrReplace(TblRow entity, int FieldDefinitionID, List<int> multipleChoices, int userID, int? changesGroupID)
        {
            return FieldCreateOrReplace(entity, FieldDefinitionID, null, null, null, multipleChoices, null, null, null, null,  userID, changesGroupID);
        }

        public Field NumericFieldCreateOrReplace(TblRow entity, int FieldDefinitionID, decimal? numericValue, int userID, int? changesGroupID)
        {
            List<int> multipleChoicesList = new List<int>();
            return FieldCreateOrReplace(entity, FieldDefinitionID, null, null, null, multipleChoicesList, numericValue, null, null, null, userID, changesGroupID);
        }

        public Field DateTimeFieldCreateOrReplace(TblRow entity, int FieldDefinitionID, DateTime? dateTimeValue,  int userID, int? changesGroupID)
        {
            List<int> multipleChoicesList = new List<int>();
            return FieldCreateOrReplace(entity, FieldDefinitionID, null, null, null, multipleChoicesList, null, null, null, dateTimeValue, userID, changesGroupID);

        }

        public int TblTabCreate(int TblID, String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            Tbl tbl = DataManipulation.DataContext.GetTable<Tbl>().Single(x => x.TblID == TblID);
            if (tbl.AllowOverrideOfRatingGroupCharacterstics)
                throw new NotImplementedException();

            
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !makeActiveNow, null, TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                var existingTblTabs = DataManipulation.DataContext.GetTable<TblTab>().Where(cg => cg.TblID == TblID);
                int numInTbl;
                if (existingTblTabs.Any())
                    numInTbl = existingTblTabs.Max(cg => cg.NumInTbl) + 1;
                else
                    numInTbl = 1;
                newObjectID = DataManipulation.AddTblTab(TblID, numInTbl, name);
                if (makeActive)
                {
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblTab, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);
                    if (makeActiveNow)
                    {
                        //start of modification
                        bool? AllowOverrideofRatingGroupCharacterstics = DataAccess.GetTbl(TblID).AllowOverrideOfRatingGroupCharacterstics;
                        int NumofTblTab = DataManipulation.DataContext.GetTable<TblTab>().Where(x => x.TblID == TblID && x.Status == (byte)StatusOfObject.Active).Count();
                        if (AllowOverrideofRatingGroupCharacterstics == true && NumofTblTab > 1)
                        {
                            DataAccess.GetTbl(TblID).AllowOverrideOfRatingGroupCharacterstics = false;
                           
                        }
                        // End of modification
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                      
                    }
                }
            }
            else
                throw new Exception("Insufficient privileges");



            return (int)newObjectID;
        }

        public void TblTabDeleteOrUndelete(int TblTabID, bool delete, bool doItNow, int userID, int? changesGroupID)
        {
           
            DataManipulation.ConfirmObjectExists(TblTabID, TypeOfObject.TblTab);
            TblTab theTblTab = DataAccess.GetTblTab(TblTabID);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, null, theTblTab.TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblTab, false, true, false, false, false, delete, false, false, "", null, TblTabID, null, null, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }
        public void TblTabChangeDefaultSort(int TblTabID, int? defaultSortTblColumnID, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblTabID, TypeOfObject.TblTab);
            TblTab theTblTab = DataAccess.GetTblTab(TblTabID);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, null, theTblTab.TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblTab, false, false, false, false, true, false, false, false, "", null, TblTabID, null, defaultSortTblColumnID, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");

        }

        public int TblColumnFormattingCreate(int TblColumnID, string prefix, string suffix, bool omitLeadingZero, decimal? extraDecimalPlaceAbove, decimal? extraDecimalPlace2Above, decimal? extraDecimalPlace3Above, string suppStylesHeader, string suppStylesMain, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            int TblID = DataAccess.RaterooDB.GetTable<TblColumn>().Single(c => c.TblColumnID == TblColumnID).TblTab.TblID;
            DataManipulation.ConfirmObjectExists(TblColumnID, TypeOfObject.TblColumn);
            int newObjectID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !makeActiveNow, null, TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                TblColumnFormatting oldObject = DataManipulation.DataContext.GetTable<TblColumnFormatting>().SingleOrDefault(cdf => cdf.TblColumnID == TblColumnID);
                newObjectID = DataManipulation.AddTblColumnFormatting(TblColumnID,prefix,suffix,omitLeadingZero,extraDecimalPlaceAbove,extraDecimalPlace2Above,extraDecimalPlace3Above,suppStylesHeader,suppStylesMain);
                if (makeActive)
                {
                    int theChange;
                    if (oldObject != null)
                        theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumnFormatting, false, true, false, false, false, false, false, false, "", null, oldObject.TblColumnFormattingID, null, null, null, "", null);
                    theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumnFormatting, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);

                    if (makeActiveNow)
                    {
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);

                    }
                }
            }
            else
                throw new Exception("Insufficient privileges");
            return (int)newObjectID;
        }

        public int TblColumnCreate(int TblTabID, int defaultRatingGroupAttributesID, String abbreviation, String name, String explanation, string widthStyle, bool trackTrustWithinTableColumn, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            Tbl tbl = DataManipulation.DataContext.GetTable<TblTab>().Single(x => x.TblTabID == TblTabID).Tbl;
            int tblID = tbl.TblID;
            if (tbl.AllowOverrideOfRatingGroupCharacterstics)
                throw new NotImplementedException();

            int? newObjectID = null;
            DataManipulation.ConfirmObjectExists(TblTabID, TypeOfObject.TblTab);

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !makeActiveNow, null, tblID, true))
            {
                int? theUser = userID;
                if ( DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                int categoryNum = DataManipulation.DataContext.GetTable<TblColumn>().Where(cd => cd.TblTabID == TblTabID).Count() + 1;
                newObjectID = DataManipulation.AddTblColumn(TblTabID, defaultRatingGroupAttributesID, categoryNum, abbreviation, name, widthStyle, explanation, trackTrustWithinTableColumn);
                if (makeActive)
                {                    
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);
                    if (makeActiveNow)
                    {
                        //start of modification
                        bool? AllowOverrideofRatingGroupCharacterstics = DataAccess.GetTbl(tblID).AllowOverrideOfRatingGroupCharacterstics;
                        int NumofCategoryDesc = DataManipulation.DataContext.GetTable<TblColumn>().Where(x => x.TblTab.TblID == tblID && x.Status == (byte)StatusOfObject.Active).Count();
                        if (AllowOverrideofRatingGroupCharacterstics == true && NumofCategoryDesc > 1)
                        {
                            DataAccess.GetTbl(tblID).AllowOverrideOfRatingGroupCharacterstics = false;
                           
                        }
                        // End of modification
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                   
                    }
                }
            }
            else
                throw new Exception("Insufficient privileges");
            return (int) newObjectID;
        }

        public void TblColumnDeleteOrUndelete(int TblColumnID, bool delete, bool doItNow, int userID, int? changesGroupID)
        {
          
            DataManipulation.ConfirmObjectExists(TblColumnID, TypeOfObject.TblColumn);
            TblColumn theTblColumn = DataAccess.GetTblColumn(TblColumnID);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, null, theTblColumn.TblTab.TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, false, true, false, false, false, delete, false, false, "", null, TblColumnID, null, null, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }
        
        public void TblColumnChangeAbbreviation(int TblColumnID, string abbreviation, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblColumnID, TypeOfObject.TblColumn);
            int TblID = DataManipulation.DataContext.GetTable<TblColumn>().Single(c => c.TblColumnID == TblColumnID).TblTab.Tbl.TblID;
            int pointsManagerID = DataManipulation.DataContext.GetTable<TblColumn>().Single(c => c.TblColumnID == TblColumnID).TblTab.Tbl.PointsManager.PointsManagerID;

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, pointsManagerID, TblID, true))
            {
                if (abbreviation.Length > 10)
                    throw new Exception("The abbreviation must be no longer than 10 characters.");
                

                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, false, false, false, false, true, true, false, false, "", null, TblColumnID, null, null, null, abbreviation, null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void TblColumnChangeName(int TblColumnID, string Name, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblColumnID, TypeOfObject.TblColumn);
            int TblID = DataManipulation.DataContext.GetTable<TblColumn>().Single(c => c.TblColumnID == TblColumnID).TblTab.Tbl.TblID;
            int pointsManagerID = DataManipulation.DataContext.GetTable<TblColumn>().Single(c => c.TblColumnID == TblColumnID).TblTab.Tbl.PointsManager.PointsManagerID;

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, pointsManagerID, TblID, true))
            {
                if (Name.Length > 50)
                    throw new Exception("The Name must be no longer than 50 characters.");


                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, false, false, false, false, true, true, true, false, "", null, TblColumnID, null, null, null, Name, null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void TblColumnChangeExplanation(int TblColumnID, string Explanation, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblColumnID, TypeOfObject.TblColumn);
            int TblID = DataManipulation.DataContext.GetTable<TblColumn>().Single(c => c.TblColumnID == TblColumnID).TblTab.Tbl.TblID;
            int pointsManagerID = DataManipulation.DataContext.GetTable<TblColumn>().Single(c => c.TblColumnID == TblColumnID).TblTab.Tbl.PointsManager.PointsManagerID;

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, pointsManagerID, TblID, true))
            {


                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, false, false, false, false, true, false, false, false, "", null, TblColumnID, null, null, null, Explanation, null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }


        public void TblColumnChangeSortOptions(int TblColumnID, bool useAsFilter,bool sortable,bool defaultSortOrderDescending, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblColumnID, TypeOfObject.TblColumn);
            TblColumn theTblColumn = DataAccess.GetTblColumn(TblColumnID);

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCategories, !doItNow, null, theTblColumn.TblTab.TblID, true))
            {

                int theNewSetting = 0;
                if (useAsFilter)
                    theNewSetting += 1;
                if (sortable)
                    theNewSetting += 2;
                if (defaultSortOrderDescending)
                    theNewSetting += 4;

                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, false, false, false, false, true, false,true, false, "", null, TblColumnID, null, theNewSetting, null,"", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");

        }
        
        public void TblRowDeleteOrUndelete(int entityID, bool delete, bool doItNow, int userID, int? changesGroupID)
        {
            TblRow theTblRow = DataAccess.GetTblRow(entityID); 
            if (DataManipulation.TblIsRewardTbl(theTblRow.Tbl))
                throw new Exception("You cannot delete or undelete a table cell in Changes.");

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, !doItNow, null, theTblRow.TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;

                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblRow, false, true, false, false, false, delete, false, false, "", null,entityID, null, null, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);

                decimal? baseMultiplierOverride;
                if (DataManipulation.ShouldCreateRewardRating(theTblRow.TblID, userID, out baseMultiplierOverride))
                {
                    RewardRatingCreate(theTblRow.TblID, delete ? RewardableUserAction.DeleteRow : RewardableUserAction.UndeleteRow, baseMultiplierOverride, 0.25M, userID, (delete ? "Deleted row " : "Undeleted row ") + theTblRow.Name,"");
                }
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void TblDeleteOrUndelete(int TblID, bool delete, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, null, TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, false, true, false, false, false, delete, false, false, "", null, TblID, null, null, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public int RatingPhaseGroupCreate(List<RatingPhaseData> thePhases, String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCharacteristics, !makeActiveNow, null, null, true))
            {
                int numPhases = thePhases.Count;
                if (numPhases < 1)
                    throw new Exception("At least one rating phase must be defined.");

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddRatingPhaseGroup(name, userID);

                if (makeActive)
                    DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPhaseGroup, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);

                foreach (RatingPhaseData thePhase in thePhases)
                {
                    int ratingPhaseId=DataManipulation.AddRatingPhase((int)newObjectID, thePhase.SubsidyLevel, thePhase.ScoringRule, thePhase.Timed, thePhase.BaseTimingOnSpecificTime, thePhase.EndTime, thePhase.RunTime, thePhase.HalfLifeForResolution, thePhase.RepeatIndefinitely, thePhase.RepeatNTimes);
                    if (makeActive)
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPhase, true, false, false, false, false, false, false, false, "", ratingPhaseId, null, null, null, null, "", null);
                }

                if (makeActiveNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");

            return (int) newObjectID;
        }

        public int SubsidyDensityRangeGroupLogarithmicCreate(decimal theBase, String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCharacteristics, !makeActiveNow, null, null, true))
            {

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddSubsidyDensityRangeGroup(name, userID, theBase);
                if (makeActive)
                    DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.SubsidyDensityRangeGroup, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);

                if (makeActiveNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");

            return (int)newObjectID;
        }

        public int SubsidyDensityRangeGroupCreate(List<SubsidyDensityRangeData> theRanges, String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCharacteristics, !makeActiveNow, null, null, true))
            {
                int numRanges = theRanges.Count;
                if (numRanges < 1)
                    throw new Exception("At least one rating Range must be defined.");

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddSubsidyDensityRangeGroup(name,userID, null);
                if (makeActive)
                    DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.SubsidyDensityRangeGroup, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);

                foreach (SubsidyDensityRangeData theRange in theRanges)
                {
                    int SubsidyDensityRangeId= DataManipulation.AddSubsidyDensityRange((int)newObjectID, theRange.RangeBottom, theRange.RangeTop, theRange.LiquidityFactor, true);
                    if (makeActive)
                        DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.SubsidyDensityRange, true, false, false, false, false, false, false, false, "", SubsidyDensityRangeId, null, null, null, null, "", null);
                }

                if (makeActiveNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");

            return (int) newObjectID;
        }

        public int RatingCharacteristicsCreate(int ratingPhaseGroupID, int? subsidyDensityRangeGroupID, decimal minimumUserRating, decimal maximumUserRating, short decimalPlaces, String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCharacteristics, !makeActiveNow, null, null, true))
            {
                if (minimumUserRating >= maximumUserRating)
                    throw new Exception("Maximum prediction must be greater than minimum prediction.");
                if (decimalPlaces < 0 || decimalPlaces > 4)
                    throw new Exception("Number of decimal places must be between 0 and 4.");

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddRatingCharacteristics(ratingPhaseGroupID, subsidyDensityRangeGroupID,minimumUserRating,maximumUserRating,decimalPlaces,name,userID);
                if (makeActive)
                {
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingCharacteristics, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);
                    if (makeActiveNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            else
                throw new Exception("Insufficient privileges");
            return (int)newObjectID;
        }

        // This version assumes that the rating ending time does not vary (i.e., the reference time varies),
        // that points are evenly split between short term and long term, and that ratings can be autocalculated
        // (i.e., where user rates one or more items in the group).
        public int RatingGroupAttributesCreate(int ratingCharacteristicsID, int? RatingConditionID, decimal? constrainedSum, RatingHierarchyData theHierarchy, String name, RatingGroupTypes RatingType, String ratingGroupDescription, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID, int pointsManagerID)
        {
            return RatingGroupAttributesCreate(ratingCharacteristicsID, null, RatingConditionID, constrainedSum, theHierarchy, name, RatingType, ratingGroupDescription, false, true, null, (decimal)0.5, makeActive, makeActiveNow, userID, changesGroupID, pointsManagerID);
        }

        public class RatingCharacteristicsHierarchyOverride
        {
            public RatingHierarchyEntry theEntryForRatingGroupWhoseMembersWillHaveDifferentCharacteristics { get; set; }
            public int theReplacementCharacteristicsID { get; set; }
        }

        internal int GetRatingCharacteristicsForSpotInHierarchy(int defaultCharacteristics, RatingHierarchyEntry theEntry, List<RatingCharacteristicsHierarchyOverride> theReplacementCharacteristics)
        {
            if (theReplacementCharacteristics == null)
                return defaultCharacteristics;
            var replacement = theReplacementCharacteristics.FirstOrDefault(x => 
                x.theEntryForRatingGroupWhoseMembersWillHaveDifferentCharacteristics == theEntry);
            if (replacement == null)
                return defaultCharacteristics;
            return replacement.theReplacementCharacteristicsID;
        }

        public int RatingGroupAttributesCreate(int ratingCharacteristicsID, List<RatingCharacteristicsHierarchyOverride> theReplacementCharacteristics, int? RatingConditionID, decimal? constrainedSum, RatingHierarchyData theHierarchy, String name, RatingGroupTypes RatingType, String ratingGroupDescription, bool ratingEndingTimeVaries, bool topGroupRatingsCanBeAutoCalculated, List<RatingHierarchyEntry> suppressAutoCalculationForGroupsBeneath, decimal longTermPointsWeight, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID, int pointsManagerID)
        {
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCharacteristics, !makeActiveNow, null, null, true))
            {
                int numInHierarchy = theHierarchy.RatingHierarchyEntries.Count;
                if (numInHierarchy == 0)
                    throw new Exception("A rating group must contain at least one rating.");
                if (numInHierarchy == 1 && constrainedSum != null)
                    throw new Exception("To constrain the sum, a rating group must contain at least two ratings.");
                if (constrainedSum < 0)
                    throw new Exception("The constrained sum must be greater than 0.");

                RatingCharacteristic theRatingChars = DataManipulation.DataContext.GetTable<RatingCharacteristic>().Single(mc => mc.RatingCharacteristicsID == ratingCharacteristicsID);
                theHierarchy.CalculateDefaultValuesForPlannedHierarchy(constrainedSum, theRatingChars.MinimumUserRating, theRatingChars.MaximumUserRating);

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddRatingGroupAttributes(ratingCharacteristicsID, RatingConditionID, constrainedSum, name, RatingType, ratingGroupDescription, userID, pointsManagerID, ratingEndingTimeVaries, topGroupRatingsCanBeAutoCalculated, longTermPointsWeight);
                if (makeActive)
                    DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingGroupAttributes, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);

                // Go through the hierarchy, creating rating plans grouped by superior and adding those plans to the appropriate rating group.
                // After creating a rating plan, see if we need to create a rating group for its descendants.
                // Use a dictionary to keep track of how entryNums get translated to RatingGroupAttributesID's.

                Dictionary<int, int> entryNumToRatingGroupAttributesID = new Dictionary<int,int>();
                int maxHierarchyLevel = theHierarchy.RatingHierarchyEntries.Max(d => d.HierarchyLevel);
                for (int currentHierarchyLevel = 1; currentHierarchyLevel <= maxHierarchyLevel; currentHierarchyLevel++)
                {
                    var theSuperiors = theHierarchy.RatingHierarchyEntries.Where(d => d.HierarchyLevel == currentHierarchyLevel).Select(d => d.Superior).Distinct();
                    foreach (int? superior in theSuperiors)
                    { // Each of these represents a separate rating group.
                        
                        // Let's remember the ratingGroupID, so that we can add the new rating plans
                        // to this rating group.
                        int ratingGroupAttributesID = 0;
                        if (currentHierarchyLevel == 1)
                            ratingGroupAttributesID = (int)newObjectID;
                        else
                            ratingGroupAttributesID = entryNumToRatingGroupAttributesID[(int)superior];

                        // Now, let's add the rating plans, and see if we need to add an additional rating group 
                        // for the next lower level of the hierarchy.
                        var theEntries = theHierarchy.RatingHierarchyEntries.Where(d => d.HierarchyLevel == currentHierarchyLevel && d.Superior == superior);
                        int numInGroup = 0;
                        RatingGroupTypes subordinateRatingGroupTypes = RatingType;
                        if (subordinateRatingGroupTypes == RatingGroupTypes.hierarchyNumbersTop)
                            subordinateRatingGroupTypes = RatingGroupTypes.hierarchyNumbersBelow;
                        else if (subordinateRatingGroupTypes == RatingGroupTypes.probabilityHierarchyTop)
                            subordinateRatingGroupTypes = RatingGroupTypes.probabilityHierarchyBelow;
                        else if (subordinateRatingGroupTypes == RatingGroupTypes.probabilityMultipleOutcomes)
                            subordinateRatingGroupTypes = RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy;

                        foreach (var theEntry in theEntries)
                        {
                            numInGroup++;

                            // Add the rating plan.
                            int newRatingPlanID = DataManipulation.AddRatingPlan(ratingGroupAttributesID,numInGroup,theEntry.Value,theEntry.RatingName,theEntry.Description,userID);
                            if (makeActive)
                                DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPlan, true, false, false, false, false, false, false, false, "", newRatingPlanID, null, null, null, null, "", null);

                            // Check to see if this has any descendants.
                            if (theHierarchy.RatingHierarchyEntries.Where(d => d.Superior == theEntry.EntryNum).Any())
                            { // We need to add a rating group attributes for this entry, and to relate the new rating group attributes to the rating plan we just created.
                                // We will allow autocalculation of ratings within this rating group, unless the entry containing the rating group
                                // is included in suppressAutoCalculationForGroupsBeneath
                                int newRatingGroupID = DataManipulation.AddRatingGroupAttributes(GetRatingCharacteristicsForSpotInHierarchy(ratingCharacteristicsID,theEntry,theReplacementCharacteristics), null, null, theEntry.RatingName, subordinateRatingGroupTypes, theEntry.Description, userID, pointsManagerID, ratingEndingTimeVaries, suppressAutoCalculationForGroupsBeneath == null || !suppressAutoCalculationForGroupsBeneath.Contains(theEntry), longTermPointsWeight);
                                if (makeActive)
                                    DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingGroupAttributes, true, false, false, false, false, false, false, false, "", newRatingGroupID, null, null, null, null, "", null);
                                DataManipulation.RelateRatingPlanAndGroupAttributes(newRatingGroupID,newRatingPlanID);
                                entryNumToRatingGroupAttributesID.Add(theEntry.EntryNum,newRatingGroupID);
                            } // adding rating group for descendants
                        } // for each entry 
                    } // for this superior
                } // for this level of the hierarchy
                    
                if (makeActiveNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
            return (int) newObjectID;
        }

        // This is for a rating group attributes with just a single rating.
        public int RatingGroupAttributesCreate(int ratingCharacteristicsID, int? RatingConditionID, String name, RatingGroupTypes RatingType, 
            decimal? defaultUserRating, String ratingGroupDescription, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID, 
            int pointsManagerID)
        {
            RatingHierarchyData ratingHierarchyData = new RatingHierarchyData();
            ratingHierarchyData.Add(name, defaultUserRating, 1, "");
            return RatingGroupAttributesCreate(ratingCharacteristicsID, RatingConditionID, null, ratingHierarchyData, name,
                                               RatingType, ratingGroupDescription, makeActive, makeActiveNow, userID,
                                               changesGroupID, pointsManagerID);
        }

        public int RatingGroupAttributesCreate(int ratingCharacteristicsID, int? RatingConditionID, String name, RatingGroupTypes RatingType, 
            decimal? defaultUserRating, String ratingGroupDescription, bool ratingEndingTimeVaries, decimal longTermPointsWeight, bool makeActive,
            bool makeActiveNow, int userID, int? changesGroupID, int pointsManagerID)
        {
            RatingHierarchyData theHierarchy = new RatingHierarchyData();
            theHierarchy.Add(name, defaultUserRating, 1, "");
            return RatingGroupAttributesCreate(ratingCharacteristicsID, null, RatingConditionID, null, theHierarchy, name, RatingType, ratingGroupDescription, ratingEndingTimeVaries, true, null, longTermPointsWeight, makeActive, makeActiveNow, userID, changesGroupID, pointsManagerID);
        }


        public RatingCondition RatingConditionCreate(Rating conditionRating, decimal? greaterThan, decimal? lessThan, int userID, int? changesGroupID, String name)
        {
            int? TblID = null;
           
            if (conditionRating == null)
                throw new Exception("Internal error -- condition rating must be specified.");
                                    

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeCharacteristics, false, null, TblID, false))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                RatingCondition theRatingCondition = DataManipulation.AddRatingCondition(conditionRating, greaterThan, lessThan);
                return theRatingCondition;
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public PointsAdjustment PointsAdjustmentCreate(int userToAdjustID, int pointsManagerID, decimal adjustmentToTotal, decimal adjustmentToCurrent, int userID, int? changesGroupID)
        {
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AdjustPoints, false, pointsManagerID, null, false))
            {
                DataManipulation.ConfirmObjectExists(userToAdjustID, TypeOfObject.User);
                DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;

                User theUserToAdjust = DataContext.GetTable<User>().Single(u => u.UserID == userToAdjustID);
                PointsManager thePointsManager = DataContext.GetTable<PointsManager>().Single(p => p.PointsManagerID == pointsManagerID);
                PointsAdjustment thePointsAdjustment = DataManipulation.AddPointsAdjustment(theUserToAdjust, thePointsManager, PointsAdjustmentReason.AdministrativeChange, adjustmentToTotal, adjustmentToCurrent, null);
                return thePointsAdjustment;
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public int ProposalSettingsCreate(int? pointsManagerID, int? TblID, bool usersMayProposeAddingTbls, bool usersMayProposeResolvingRatings, bool usersMayProposeChangingTblRows,
            bool usersMayProposeChangingChoiceGroups, bool usersMayProposeChangingCharacteristics, bool usersMayProposeChangingCategories, bool usersMayProposeChangingUsersRights, bool usersMayProposeAdjustingPoints,
            bool usersMayProposeChangingProposalSettings, decimal minValueToApprove, decimal maxValueToReject, int minTimePastThreshold, decimal minProportionOfThisTime, int minAdditionalTimeForRewardRating,
            int halfLifeForRewardRating, decimal maxBonusForProposal, decimal maxPenaltyForRejection, decimal subsidyForApprovalRating, decimal subsidyForRewardRating,
            int halfLifeForResolvingAtFinalValue, decimal requiredPointsToMakeProposal,
            String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeProposalSettings, !makeActiveNow, pointsManagerID, TblID, true))
            {
                if (pointsManagerID == null && TblID == null)
                    throw new Exception("Internal error -- must specify universe or Tbl to change proposal settings.");
                if (pointsManagerID != null && TblID != null)
                    pointsManagerID = null; // apply changes only to Tbl
                if (pointsManagerID != null)
                    DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);
                if (TblID != null)
                    DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddProposalSettings(pointsManagerID, TblID, usersMayProposeAddingTbls, usersMayProposeResolvingRatings, usersMayProposeChangingTblRows, 
                                    usersMayProposeChangingChoiceGroups, usersMayProposeChangingCharacteristics, usersMayProposeChangingCategories, usersMayProposeChangingUsersRights, usersMayProposeAdjustingPoints, 
                                    usersMayProposeChangingProposalSettings, minValueToApprove, maxValueToReject, minTimePastThreshold, minProportionOfThisTime, minAdditionalTimeForRewardRating,
                                    halfLifeForRewardRating, maxBonusForProposal, maxPenaltyForRejection, subsidyForApprovalRating, subsidyForRewardRating,
                                    halfLifeForResolvingAtFinalValue,  requiredPointsToMakeProposal,
                                    name, theUser);
                if (makeActive)
                {
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.ProposalSettings, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);
                    if (makeActiveNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            else
                throw new Exception("Insufficient privileges");
            return (int)newObjectID;
        }

        public int UsersRightsCreate(int? userToAffectID, int? pointsManagerID, bool mayView, bool mayPredict, bool mayAddTbls,
            bool mayResolveRatings, bool mayChangeTblRows, bool mayChangeChoiceGroups, bool mayChangeCharacteristics,
            bool mayChangeCategories, bool mayChangeUsersRights, bool mayAdjustPoints, bool mayChangeProposalSettings,
            String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            int? newObjectID = null;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeUsersRights, !makeActiveNow, pointsManagerID, null, true))
            {
                if (userToAffectID != null)
                    DataManipulation.ConfirmObjectExists(userToAffectID, TypeOfObject.User);
                if (pointsManagerID != null)
                    DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);

                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                newObjectID = DataManipulation.AddUsersRights(userToAffectID, pointsManagerID, mayView, mayPredict, mayAddTbls,
                                mayResolveRatings, mayChangeTblRows, mayChangeChoiceGroups, mayChangeCharacteristics,
                                mayChangeCategories, mayChangeUsersRights, mayAdjustPoints, mayChangeProposalSettings,
                                name, theUser);
                if (makeActive)
                {
                    int? ExistingObjectId = null;

                    if (userToAffectID == null)
                    {
                        var ExistingObject = DataContext.GetTable<UsersRight>().SingleOrDefault(x => x.PointsManagerID == pointsManagerID && x.UserID == null && x.Status == (byte)StatusOfObject.Active);
                        if (ExistingObject != null)
                        {
                            ExistingObjectId = ExistingObject.UsersRightsID;
                        }
                        
                    }
                    else
                    {
                        var ExistingObject = DataContext.GetTable<UsersRight>().SingleOrDefault(x => x.PointsManagerID == pointsManagerID && x.UserID == userToAffectID && x.Status == (byte)StatusOfObject.Active);
                        if (ExistingObject != null)
                        {
                            ExistingObjectId = ExistingObject.UsersRightsID;
                        }
                    }

                    if (ExistingObjectId == null)
                    {
                        int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.UsersRights, true, false, false, false, false, false, false, false, "", newObjectID, null, null, null, null, "", null);
                        if (makeActiveNow)
                            DataManipulation.ImplementChangesGroup((int)changesGroupID);
                    }
                    else
                    {
                        int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.UsersRights, false, false, true, false, false, false, false, false, "", newObjectID, ExistingObjectId, null, null, null, "", null);
                        if (makeActiveNow)
                            DataManipulation.ImplementChangesGroup((int)changesGroupID);
                    }
                }
            }
            else
                throw new Exception("Insufficient privileges");
            return (int)newObjectID;
        }

        public int UsersRightsCreate(int? userToAffectID, int? pointsManagerID, bool mayView, bool mayPredict, bool mayAdminister, String name, bool makeActive, bool makeActiveNow, int userID, int? changesGroupID)
        {
            return UsersRightsCreate(userToAffectID, pointsManagerID, mayView, mayPredict, mayAdminister, mayAdminister, mayAdminister, mayAdminister, mayAdminister, mayAdminister, mayAdminister, mayAdminister, mayAdminister, name, makeActive, makeActiveNow, userID, changesGroupID);
        }

        public void ChangeNameOfObject(int objectID, TypeOfObject theObjectType, String theName, bool doItNow, int userID, int? changesGroupID)
        {
            UserActionOldList theAction;
            switch (theObjectType)
            {
                case TypeOfObject.TblColumn:
                case TypeOfObject.TblTab:
                    theAction = UserActionOldList.ChangeCategories;
                    break;
                case TypeOfObject.ChoiceGroup:
                    theAction = UserActionOldList.ChangeChoiceGroups;
                    break;
                case TypeOfObject.Tbl:
                case TypeOfObject.PointsManager:
                case TypeOfObject.Domain:
                    theAction = UserActionOldList.Other;
                    break;
                case TypeOfObject.TblRow:
                    theAction = UserActionOldList.ChangeTblRows;
                    break;
                case TypeOfObject.RatingCharacteristics:
                case TypeOfObject.RatingGroupAttributes:
                case TypeOfObject.RatingPhaseGroup:
                case TypeOfObject.RatingPlan:
                case TypeOfObject.SubsidyDensityRangeGroup:
                    theAction = UserActionOldList.ChangeCharacteristics;
                    break;
                default:
                    throw new Exception("Internal error -- trying to change system name of field without a system name");
            }

            int? TblID;
            int? pointsManagerID;
            DataManipulation.ConfirmObjectExists(objectID, theObjectType);
            DataManipulation.GetTblAndPointsManagerForObject(objectID, theObjectType, out TblID, out pointsManagerID);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, theAction, !doItNow, pointsManagerID, TblID, true))
            {
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, theObjectType, false, false, false, true, false, false, false, false, theName, null, objectID, null, null, null, "", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
        }

        public void ResolveRatingInGroupAtZero(int ratingID, bool doItNow, int userID, int? changesGroupID)
        {
            throw new Exception("No longer implemented.");
        }

        public void ResolveRatingInGroupAtSpecifiedValue(int ratingID, decimal finalValue, bool doItNow, int userID, int? changesGroupID)
        {
            throw new Exception("No longer implemented.");
        }

        public void UnresolveRatingGroupFromRound(int ratingGroupID, int fromRoundNum, bool doItNow, int userID, int? changesGroupID)
        {
            throw new Exception("No longer implemented.");
        }

        public void UnresolveRatingFromRound(int ratingID, int fromRoundNum, bool doItNow, int userID, int? changesGroupID)
        {
            throw new Exception("No longer implemented.");
        }

        public void ResolveRatingGroupByUnwinding(RatingGroup ratingGroup, bool doItNow, int userID, int? changesGroupID)
        {
            int? TblID;
            int? pointsManagerID;
            TblID = ratingGroup.TblRow.TblID;
            pointsManagerID = ratingGroup.TblRow.Tbl.PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ResolveRatings, !doItNow, pointsManagerID, TblID, false))
            {
                RatingGroupResolution theResolution = DataManipulation.AddRatingGroupResolution(ratingGroup,false,true,null,userID);
                decimal? baseMultiplierOverride;
                if (TblID != null && DataManipulation.ShouldCreateRewardRating((int)TblID, userID, out baseMultiplierOverride))
                {
                    RewardRatingCreate((int)TblID, RewardableUserAction.ResolveTableCell, baseMultiplierOverride, 0.1M, userID, "Resolved table cell (assigning zero to unresolved ratings) " + ratingGroup.TblRow.Name + " " + ratingGroup.TblColumn.Name, "");
                }
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
        }

        public void ResolveRatingGroup(RatingGroup ratingGroup, bool doItNow, bool cancelPreviousResolutions, bool resolveByUnwinding, DateTime? effectiveTime, int userID, int? changesGroupID)
        {
            if (DataManipulation.TblIsRewardTbl(ratingGroup.TblRow.Tbl))
                throw new Exception("You cannot resolve a table cell in Changes.");
            RatingGroupResolution currentRatingGroupResolution = 
                DataAccess.RaterooDB.GetTable<RatingGroupResolution>().Where
                    (mr => mr.RatingGroup == ratingGroup)
                    .OrderByDescending(mr => mr.ExecutionTime)
                    .ThenByDescending(mr => mr.RatingGroupResolutionID)
                    .FirstOrDefault();

            if (currentRatingGroupResolution == null && cancelPreviousResolutions)
                return;
            else if (currentRatingGroupResolution != null)
            {
                if (currentRatingGroupResolution.CancelPreviousResolutions && cancelPreviousResolutions)
                    return;
                else if (!currentRatingGroupResolution.CancelPreviousResolutions && !cancelPreviousResolutions)
                {
                    // First, we need to cancel previous resolutions
                    ResolveRatingGroup(ratingGroup, doItNow, true, false, effectiveTime, userID, changesGroupID);
                    // Now, start again at the beginning of this routine and then return.
                    ResolveRatingGroup(ratingGroup, doItNow,cancelPreviousResolutions, resolveByUnwinding, effectiveTime, userID, changesGroupID);
                    return;
                }
            }
            int? TblID;
            int? pointsManagerID;
            TblID = ratingGroup.TblRow.TblID;
            pointsManagerID = ratingGroup.TblRow.Tbl.PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ResolveRatings, !doItNow, pointsManagerID, TblID, false))
            {
                RatingGroupResolution theResolution = DataManipulation.AddRatingGroupResolution(ratingGroup, cancelPreviousResolutions, resolveByUnwinding, effectiveTime, userID); 
                //Trace.TraceInformation("ResolveRatingGroup effective time" + effectiveTime);
                decimal? baseMultiplierOverride;
                if (TblID != null && DataManipulation.ShouldCreateRewardRating((int) TblID, userID, out baseMultiplierOverride))
                {
                    RewardRatingCreate((int)TblID, cancelPreviousResolutions ? RewardableUserAction.CancelResolve : RewardableUserAction.ResolveTableCell, baseMultiplierOverride, 0.1M, userID, (cancelPreviousResolutions ? "Canceled resolution of table cell" : "Resolved table cell ") + ratingGroup.TblRow.Name + " " + ratingGroup.TblColumn.Name,"Effective: " + (effectiveTime == null ? TestableDateTime.Now.ToString() : effectiveTime.ToString()));
                }
            }
        }

        public void UserRatingAdd(int aRatingID, decimal aUserRating, int userID, ref UserRatingResponse theResponse)
        {
            User theUser = DataContext.GetTable<User>().Single(u => u.UserID == userID);
            UserRatingAdd(aRatingID, aUserRating, theUser, ref theResponse);
        }

        public void UserRatingAdd(int aRatingID, decimal aUserRating, User theUser, ref UserRatingResponse theResponse)
        {
            List<RatingIdAndUserRatingValue> theList = new List<RatingIdAndUserRatingValue>();
            theList.Add(new RatingIdAndUserRatingValue { RatingID = aRatingID, UserRatingValue = aUserRating });
            UserRatingsAdd(theList, theUser, ref theResponse);
        }

        public void UserRatingsAdd(List<RatingIdAndUserRatingValue> theUserRatings, User theUser, ref UserRatingResponse theResponse)
        {
            List<int> theRatingIDs = theUserRatings.Select(p => p.RatingID).ToList();
            int firstRatingID = theRatingIDs.First();
            Rating firstRating = DataContext.GetTable<Rating>().Single(m => m.RatingID == firstRatingID);
            int topmostRatingGroupID = firstRating.TopmostRatingGroupID;
            RatingGroup theTopRatingGroup = DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == topmostRatingGroupID);
            List<Rating> theRatings = DataContext.GetTable<Rating>().Where(m => m.TopmostRatingGroupID == theTopRatingGroup.RatingGroupID).ToList();
            List<RatingGroup> theRatingGroups = theRatings.Select(x => x.RatingGroup).Distinct().ToList();
            UserRatingsAdd(theUserRatings, theRatings, theRatingGroups, theUser, ref theResponse);
        }

        public void UserRatingsAdd(List<RatingIdAndUserRatingValue> theUserRatings, List<Rating> theRatings, List<RatingGroup> theRatingGroups, User theUser, ref UserRatingResponse theResponse)
        {
            theResponse = new UserRatingResponse();
            if (!theUserRatings.Any())
            {
                theResponse.result = new UserRatingResult("You have not entered any ratings.");
                return;
            }

            int theUserRatingCount = theUserRatings.Count();
            RatingGroup topRatingGroup = null;
            int topMostRatingID = -1;
            for (int i = 0; i < theUserRatingCount; i++)
            {
                theUserRatings[i].UserRatingValue = Math.Round(theUserRatings[i].UserRatingValue, 4);
                Rating theRating = theRatings.SingleOrDefault(m => m.RatingID == theUserRatings[i].RatingID);
                if (theRating == null)
                    throw new Exception("The specified table cell could not be found.");
                if (theUserRatings[i].UserRatingValue > theRating.RatingCharacteristic.MaximumUserRating)
                {
                    theResponse.result = new UserRatingResult("The maximum value is " + Math.Round((double)theRating.RatingCharacteristic.MaximumUserRating, theRating.RatingCharacteristic.DecimalPlaces, MidpointRounding.ToEven));
                    return;
                }
                if (theUserRatings[i].UserRatingValue < theRating.RatingCharacteristic.MinimumUserRating)
                {
                    theResponse.result = new UserRatingResult("The minimum value is " + Math.Round((double)theRating.RatingCharacteristic.MinimumUserRating, theRating.RatingCharacteristic.DecimalPlaces, MidpointRounding.ToEven));
                    return;
                }
                if (i == 0)
                {
                    topRatingGroup = theRating.RatingGroup2;
                    topMostRatingID = theRating.TopmostRatingGroupID;
                    if (theRatingGroups.Single(mg => mg.RatingGroupID == topMostRatingID).Status != (Byte)StatusOfObject.Active)
                    {
                        theResponse.result = new UserRatingResult("Entering ratings is currently disabled for this table cell.");
                    }
                }
                else if (topMostRatingID != theRating.TopmostRatingGroupID)
                    throw new Exception("Only ratings from a single table cell can be entered at one time.");
                //if (DataAccessModule.GetTradingStatus(theUserRatings[i].ratingID, TypeOfObject.Rating) != TradingStatus.Active)
                //    return new UserRatingResult("Entering ratings is currently disabled for this table cell.");
            }

            theResponse.result = DataAccess.ConfirmUserRatingRightsForTableCell(theUser.UserID, topRatingGroup);
            if (theResponse.result.success)
                DataManipulation.AddUserRatingsBasedOnOneOrMore(theUserRatings, theRatings, theRatingGroups, theUser, ref theResponse);
        }

        public void UserRatingsAddFromService(List<Rating> allRatingsIfOneRatingPerCell, List<RatingAndUserRatingString> theUserRatingsString, User theUser, ref UserRatingResponse theResponse)
        {
            theResponse = new UserRatingResponse();
            int? topRatingGroupID = -1;
            int firstRaterooID = 0;

            int numTries = 0;
            TryLabel:
            try
            {
                numTries++;

                if (!theUserRatingsString.Any())
                {
                    theResponse.result = new UserRatingResult("You did not enter a new rating.");
                    return;
                }


                // convert string data to a numeric format
                List<RatingIdAndUserRatingValue> theUserRatings = new List<RatingIdAndUserRatingValue>();
                List<int> theRatingIDs = new List<int>();
                bool ratingIDsProperlyFormatted = true;
                ratingIDsProperlyFormatted = RatingAndUserRatingStringConverter.AddRatingIDsToList(theUserRatingsString, theRatingIDs);
                if (!ratingIDsProperlyFormatted)
                {
                    theResponse.result = new UserRatingResult("Internal error: table cell must be represented by numeric data.");
                    return;
                }

                Rating firstRating;
                if (allRatingsIfOneRatingPerCell == null)
                    firstRating = DataContext.GetTable<Rating>().SingleOrDefault(m => m.RatingID == theRatingIDs.First());
                else
                    firstRating = allRatingsIfOneRatingPerCell.SingleOrDefault(m => m.RatingID == theRatingIDs.First());
                RatingCharacteristic theRatingCharacteristic = firstRating.RatingCharacteristic;
                RatingGroup theTopRatingGroup = firstRating.RatingGroup2;
                TblRow theTableRow = theTopRatingGroup.TblRow;
                if (theTableRow.Status == (Byte)StatusOfObject.Unavailable)
                {
                    theResponse.result = new UserRatingResult("Sorry, you cannot enter a rating in a table row that has been deleted.");
                    return;
                }
                RatingGroupTypes theTypeForFirstRating = (RatingGroupTypes) theTopRatingGroup.RatingGroupAttribute.TypeOfRatingGroup;
                List<Rating> theRatings = new List<Rating>();
                if (theTypeForFirstRating != RatingGroupTypes.probabilitySingleOutcome && theTypeForFirstRating != RatingGroupTypes.singleDate && theTypeForFirstRating != RatingGroupTypes.singleNumber)
                {
                    theRatings = DataContext.GetTable<Rating>().Where(x => x.RatingGroup2 == theTopRatingGroup).ToList(); // all ratings sharing same top rating group
                }
                else
                    theRatings.Add(firstRating);

                if (firstRating == null)
                    throw new UserRatingDataException("An internal error occurred. The table cell that you specified could not be found in the database.");
                List<RatingGroup> theRatingGroups = theRatings.Select(x => x.RatingGroup).Distinct().ToList();
                if (theRatings.Count < theUserRatingsString.Count)
                {
                    theResponse.result = new UserRatingResult("Sorry, an internal error occurred. The table cell you specified could not be found in the database.");
                    return;
                }
                for (int i = 0; i < theRatingIDs.Count; i++)
                {
                    int aRatingID = theRatingIDs[i];
                    double aUserRating = (double)-1;
                    if (i == 0)
                    {
                        firstRaterooID = aRatingID;
                        Rating theRating = theRatings.Single(m => m.RatingID == firstRaterooID);
                        if (DataManipulation.TblIsRewardTbl(theRating.RatingGroup.TblRow.Tbl))
                        {
                            if (theUser.UserID == DataManipulation.GetUserWhoMadeDatabaseChange(theRating.RatingGroup.TblRow))
                            {
                                theResponse.result = new UserRatingResult("You may not rate a database change that you made.");
                                return;
                            }
                        }
                        topRatingGroupID = theRating.TopmostRatingGroupID;
                    }
                    if (theUserRatingsString[i].theUserRating.Trim() == "")
                    {
                        if (theRatings.Where(m => m.TopmostRatingGroupID == topRatingGroupID).Count() > 1)
                            theResponse.result = new UserRatingResult("Please do not leave a rating blank. If you want a rating to be automatically calculated on the basis of other ratings, please leave it at its original value.");
                        else
                            theResponse.result = new UserRatingResult("Please enter a rating.");
                        return;
                    }

                    TblColumnFormatting
                                theFormatting = PMNumberandTableFormatter.GetFormattingForTblColumn(theRatingGroups.Single(mg => mg.RatingGroupID == topRatingGroupID).TblColumnID);
                    string withoutPrefixSuffix = PMNumberandTableFormatter.RemovePrefixAndSuffix(theUserRatingsString[i].theUserRating, theFormatting);
                    if (!MoreStringManip.IsNumeric(withoutPrefixSuffix, ref aUserRating))
                    {
                        theResponse.result = new UserRatingResult("Please enter numeric data.");
                        return;
                    }
                    if (theRatings.Count() > 1 && aUserRating != Math.Round(aUserRating, theRatingCharacteristic.DecimalPlaces))
                    {
                        theResponse.result = new UserRatingResult("The maximum number of decimal places is " + theRatingCharacteristic.DecimalPlaces.ToString() + ".");
                        return;
                    }
                    theUserRatings.Add(new RatingIdAndUserRatingValue { RatingID = aRatingID, UserRatingValue = (decimal)aUserRating });
                }

                if (topRatingGroupID == null)
                {
                    if (theUserRatings.Count() == 1)
                        theResponse.result = new UserRatingResult("An error occurred in submitting the rating.");
                    else
                        theResponse.result = new UserRatingResult("An error occurred in submitting the ratings.");
                    return;
                }

                UserRatingsAdd(theUserRatings, theRatings, theRatingGroups, theUser, ref theResponse);

                DataContext.SubmitChanges();
            }
            catch (UserRatingDataException ex)
            {
                theResponse.result = new UserRatingResult(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                if (ex is RoutineMaintenanceException)
                    theResponse.result = new UserRatingResult(ex.Message);
                else
                {
                    if (numTries < 5)
                    {
                        System.Diagnostics.Trace.TraceError("Exception in predictionsaddfromservice " + ex.Message + numTries);
                        BackgroundThread.Instance.RequestBriefPauseAndWaitForPauseToBegin();
                        goto TryLabel;
                    }
                    else
                    {
                        theResponse.result = new UserRatingResult("Sorry, an error occurred. Rateroo may be busy processing other ratings. Please try again.");
                        return;
                    }
                }
            }
        }

        

        public UserRatingResponse GetUpdatedRatings(string ratingGroupIDString)
        {
            UserRatingResponse theResponse = new UserRatingResponse();
            TblColumnFormatting theFormatting = null;
            try
            {
                int topRatingGroupID = Convert.ToInt32(ratingGroupIDString);
                theFormatting = PMNumberandTableFormatter.GetFormattingForTblColumn(DataManipulation.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == topRatingGroupID).TblColumnID);
                theResponse.result = new UserRatingResult();
                List<RatinCurrentValueAndDecimalPlaces> theRatingIDsAndRatings = DataManipulation.DataContext.GetTable<Rating>()
                    .Where(m => m.TopmostRatingGroupID == topRatingGroupID)
                    .Select(m => new RatinCurrentValueAndDecimalPlaces
                    {
                        ratingID = m.RatingID,
                        theValue = m.CurrentValue,
                        decimalPlaces = m.RatingCharacteristic.DecimalPlaces
                    }).ToList();
                theResponse.currentValues = theRatingIDsAndRatings.Select(x => new RatingAndUserRatingString
                    {
                        ratingID = x.ratingID.ToString(),
                        theUserRating = PMNumberandTableFormatter.FormatAsSpecified(x.theValue,x.decimalPlaces,theFormatting)
                    }).ToList();
            }
            catch
            {
                theResponse.result = new UserRatingResult("An error occurred in accessing the database.");
            }
            return theResponse;
        }

        public UserRatingResponse GetUpdatedRatingsMultiple(List<string> ratingIDStrings)
        {
            UserRatingResponse theResponse = new UserRatingResponse();
            TblColumnFormatting theFormatting = null;
            try
            {
                List<RatingAndUserRatingString> theList = new List<RatingAndUserRatingString>();
                theResponse.result = new UserRatingResult();
                foreach (string ratingIDString in ratingIDStrings)
                {
                    int ratingID = Convert.ToInt32(ratingIDString);
                    Rating theRating = DataManipulation.DataContext.GetTable<Rating>()
                        .Single(m => m.RatingID == ratingID);
                    decimal? theUserRating = theRating.CurrentValue;
                    int decimalPlaces = theRating.RatingCharacteristic.DecimalPlaces;
                    int topRatingGroupID = theRating.TopmostRatingGroupID;
                    theFormatting = PMNumberandTableFormatter.GetFormattingForTblColumn(DataManipulation.DataContext.GetTable<RatingGroup>().Single(mg => mg.RatingGroupID == topRatingGroupID).TblColumnID);
                    string formattedResult = PMNumberandTableFormatter.FormatAsSpecified(theUserRating,decimalPlaces,theFormatting);
                    RatingAndUserRatingString theRatingAndUserRating = new RatingAndUserRatingString();
                    theRatingAndUserRating.ratingID = ratingIDString;
                    theRatingAndUserRating.theUserRating = formattedResult;
                    theList.Add(theRatingAndUserRating);
                }
                theResponse.currentValues = theList;
            }
            catch
            {
                theResponse.result = new UserRatingResult("An error occurred in accessing the database.");
            }
            return theResponse;
        }

        //public void TradingStatusSet(int objectID, TypeOfObject theObjectType, TradingStatus newStatus, int userID,int? changesGroupID)
        //{
        //    DataAccessModule.ConfirmObjectExists(objectID, theObjectType);
        //    int? TblID;
        //    int? pointsManagerID;
        //    DataAccessModule.GetTblAndPointsManagerForObject(objectID, theObjectType, out TblID, out pointsManagerID);
        //    if (DataAccessModule.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, false, pointsManagerID, TblID))
        //    {
        //        int? newValueInteger = null;
        //        if(newStatus==TradingStatus.Active)
        //        {
        //            newValueInteger = 1;
        //        }
        //        if (newStatus == TradingStatus.SuspendedDirectly)
        //        {
        //            newValueInteger = 2;
        //        }
        //        if(newStatus==TradingStatus.Ended)
        //        {
        //              newValueInteger = 3;
        //        }
               
        //        int theChange = DataAccessModule.AddChangesStatusOfObject((int)changesGroupID, theObjectType, false, false, false, false, false,true, false, false, "", null, objectID, null, newValueInteger, null, "", null);
               
        //        DataAccessModule.ImplementChangesGroup((int)changesGroupID);
        //    }
        //    //if (DataAccessModule.CheckUserRights(userID, UserActionOldList.AddTblsAndChangePointsManagers, false, pointsManagerID, TblID))
        //    //{
        //    //    DataAccessModule.SetTradingStatusHierarchical(objectID, theObjectType, newStatus);
        //    //}
        //    else throw new Exception("You don't have privileges to set the trading status.");
        //}
        public void TradingStatusSet(int objectID, TypeOfObject theObjectType, TradingStatus newStatus, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(objectID, theObjectType);
            int? TblID;
            int? pointsManagerID;
            DataManipulation.GetTblAndPointsManagerForObject(objectID, theObjectType, out TblID, out pointsManagerID);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, false, pointsManagerID, TblID, true))
            {
                int? newValueInteger = null;
                if (newStatus == TradingStatus.Active)
                {
                    newValueInteger = 1;
                }
                if (newStatus == TradingStatus.SuspendedDirectly)
                {
                    newValueInteger = 2;
                }
                if (newStatus == TradingStatus.Ended)
                {
                    newValueInteger = 3;
                }

                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, theObjectType, false, false, false, false, false, true, false, false, "", null, objectID, null, newValueInteger, null, "", null);
                if (doItNow)
                {
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            //if (DataAccessModule.CheckUserRights(userID, UserActionOldList.AddTblsAndChangePointsManagers, false, pointsManagerID, TblID))
            //{
            //    DataAccessModule.SetTradingStatusHierarchical(objectID, theObjectType, newStatus);
            //}
            else throw new Exception("You don't have privileges to set the trading status.");
        }

        //public void TblCreateRatingsAndBeginTrading(int TblID, int userID,int? changesGroupID)
        //{
        //    DataAccessModule.ConfirmObjectExists(TblID, TypeOfObject.Tbl);

        //    if (DataAccessModule.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, false, null, TblID) || DataAccessModule.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, false, null, TblID))
        //    {
        //        int theChange = DataAccessModule.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, false, false, false, false,true, false, false, false, "", null,TblID, null, null, null, "", null);
        //        DataAccessModule.ImplementChangesGroup((int)changesGroupID);
        //    }
          
        //    else throw new Exception("You don't have privileges to launch trading on a Tbl.");
        //}

        public int CommentForTblRowCreate(int entityId, string commentTitle, string commentText, DateTime date, int userID, bool proposeOnly, bool considerCreatingRewardRating, int? changesGroupID)
        {
            int? theCommentId = null;
            DataManipulation.ConfirmObjectExists(entityId, TypeOfObject.TblRow);
            Tbl theTbl = DataAccess.GetTblRow(entityId).Tbl;
            int? PointsManagerID = theTbl.PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, false, PointsManagerID, null, true))
            {
                theCommentId = DataManipulation.AddComment(entityId, userID, commentTitle, commentText, date, proposeOnly ? StatusOfObject.Proposed : StatusOfObject.Active);
                DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
            decimal? baseMultiplierOverride;
            if (considerCreatingRewardRating && DataManipulation.ShouldCreateRewardRating(theTbl.TblID, userID, out baseMultiplierOverride))
            {
                RewardableUserAction theAction = proposeOnly ? RewardableUserAction.ProposeComment : RewardableUserAction.AddComment;
                string theActionString = theAction.ToString() + ": " + commentTitle.Left(200);
                string detailString = commentTitle + ": " + commentText;
                RewardRatingCreate(theTbl.TblID, theAction, baseMultiplierOverride, 1M, userID, theActionString, detailString);
            }


            return (int)theCommentId;
        }

        public void CommentForTblRowDeleteOrUndelete(int CommentId, bool delete, int userID, bool considerCreatingRewardRating, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(CommentId, TypeOfObject.Comment);
            Comment theComment = DataAccess.GetComment(CommentId);
            Tbl theTbl = theComment.TblRow.Tbl;
            int? PointsManagerID = theTbl.PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.ChangeTblRows, false, PointsManagerID, null, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Comment, false, true, false, false, false, delete, false, false, "", null, CommentId, null, null, null, "", null);
                DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");

            decimal? baseMultiplierOverride;
            if (considerCreatingRewardRating && DataManipulation.ShouldCreateRewardRating(theTbl.TblID, userID, out baseMultiplierOverride))
            {
                RewardableUserAction theAction = delete ? RewardableUserAction.DeleteComment : (theComment.Status == (int)StatusOfObject.Proposed ? RewardableUserAction.ApproveComment : RewardableUserAction.DeleteComment);
                string theActionString = theAction.ToString() + ": " + theComment.CommentTitle.Left(200);
                string detailString = theComment.CommentTitle + ": " + theComment.CommentText;
                RewardRatingCreate(theTbl.TblID, theAction, baseMultiplierOverride, 1M, userID, theActionString, detailString);
            }

        }

        public void TblChangeCommentSetting(int TblID, bool allowUsersToAddComments, bool limitComments, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            int pointsManagerID = DataManipulation.DataContext.GetTable<Tbl>().Single(c => c.TblID == TblID).PointsManagerID;
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, TblID, true))
            {
                int theNewSetting = 0;
                if (allowUsersToAddComments)
                    theNewSetting += 1;
                if (limitComments)
                    theNewSetting += 2;
                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, false, false, false, false, false, false, true, false, "", null, TblID, null, theNewSetting, null, "ChangeCommentSetting", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public int UserAdd(string userName, bool superUser, string email, string password, bool profileAlreadyAdded)
        {
            int? newObjectID = null;

            newObjectID = DataManipulation.AddUserReturnId(userName, superUser, email, password, profileAlreadyAdded);
           
            return (int)newObjectID;     
        }
       
        public void ConditionalRatingForTblCreate(int TblColumnID,int? conditionTblColumnID, decimal? greaterThan,decimal? lessThan,int? userID,int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblColumnID, TypeOfObject.TblColumn);
            int? TblID=DataAccess.GetTblColumn(TblColumnID).TblTab.TblID;

            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, false, null, TblID, true))
            {
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.TblColumn, false, false, false, false, false, true, false, false, "", null, TblID, null, null, null, "", null);
                DataManipulation.AddConditionalRatingForTbl(TblColumnID, conditionTblColumnID, greaterThan, lessThan);
               
            }
        }
        public void InsertableContentCreate(int? domainID, int? pointsManagerID,int? TblID,string name,string content,bool isTextOnly,bool isOverridable,InsertableLocation location,bool isActivate,int userID,int? changesGroupID)
        {
            int? insertableContentID = null;
            
            if((domainID !=null && pointsManagerID !=null)|| (domainID!=null && TblID!=null) || (pointsManagerID!=null && TblID!=null))
            {
                throw new Exception("From the parameters domainID, pointsManagerID, TblID only one can be non null");
            }
            if (domainID == null && pointsManagerID == null && TblID == null)
            {
                int? theUser = userID;
                if (!DataAccess.GetUser(userID).SuperUser)
                {
                    throw new Exception("You don't have privileges to make announcements for current domain.");
                }
            }
            if (domainID != null)
            {
                DataManipulation.ConfirmObjectExists(domainID, TypeOfObject.Domain);
                int? theUser = userID;
                if (!DataAccess.GetUser(userID).SuperUser)
                {
                    throw new Exception("You don't have privileges to make announcements.");
                }
            }
            if (pointsManagerID != null)
            {
                DataManipulation.ConfirmObjectExists(pointsManagerID,TypeOfObject.PointsManager);
            }
            if (TblID != null)
            {
                DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            }
            if (DataManipulation.ProceedWithChange(ref changesGroupID,userID,UserActionOldList.AddTblsAndChangePointsManagers,false,pointsManagerID,TblID, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
               

               
               // Deleting the active contents for selected domain or universe or Tbl or everywhere 
                //var ObjInsertableContent = RaterooDB.GetTable<InsertableContent>().Where(x => int.Equals(x.DomainID,domainID)  && int.Equals(x.TblID, TblID) && int.Equals(x.PointsManagerID, pointsManagerID) && x.Location == location && x.Status==(byte)StatusOfObject.Active).Select(X => new { InsertableContentID=X.InsertableContentID });
                
                //if (ObjInsertableContent.Count() > 0)
                //{
                //    foreach (var vi in ObjInsertableContent)
                //    {
                //        DataAccessModule.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.InsertableContent, false, true,false,false, false, false, false, false, "", null,vi.InsertableContentID, null, null, null, "", null);
                //    }
                    
                //}
                                
                insertableContentID = DataManipulation.AddInsertableContents(name, domainID, pointsManagerID, TblID, content, isTextOnly, isOverridable, location);
                
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.InsertableContent, true, false, false, false, false, false, false, false, "", insertableContentID, null, null, null, null, "", null);
                    if (isActivate)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
            }
            PMCacheManagement.InvalidateCacheDependency("InsertableContent");
        }

        public HierarchyItem HierarchyItemCreate(HierarchyItem higherItemForMenu, HierarchyItem higherItemForRouting, Tbl correspondingTblIfAny, bool includeInMenu, string name, int userID, int? changesGroupID)
        {
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, false, null, null, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;
                return DataManipulation.AddHierarchyItem(higherItemForMenu, higherItemForRouting, correspondingTblIfAny, includeInMenu, name);
            }
            else
                throw new Exception("Insufficient privileges");
        }

        public void InsertableContentChange(int insertableContentID, string name, string content, bool isTextOnly, bool isOverridable, InsertableLocation location, bool isActivate, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(insertableContentID, TypeOfObject.InsertableContent);
            InsertableContent theInsertableContent = DataAccess.GetInsertableContents(insertableContentID);
            int? TopicId = theInsertableContent.DomainID;
            int? pointsManagerID = theInsertableContent.PointsManagerID;
            int? TableId = theInsertableContent.TblID;
           
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, false, pointsManagerID, TableId, true))
            {
                int? theUser = userID;
                if (DataAccess.GetUser(userID).SuperUser)
                    theUser = null;

                //InsertableContent originalAnnouncement = ObjDataAccess.GetInsertableContents(insertableContentID);
                //int newInsertableContentID = DataAccessModule.AddInsertableContents(name, originalAnnouncement.DomainID, originalAnnouncement.PointsManagerID, originalAnnouncement.TblID, content, isTextOnly, isOverridable, location,isActivate);
                DataManipulation.ChangeInsertableContents(insertableContentID, name, content, isTextOnly, isOverridable, location, isActivate);
                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.InsertableContent, false, false, false, false, true, false, false, false, "",null, insertableContentID, null, null, null, "", null);
               if(isActivate)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);
               PMCacheManagement.InvalidateCacheDependency("InsertableContent");             
            }
            else
                throw new Exception("You don't have privileges to change announcements.");
     

        }
        public bool CheckUserRights(int? userID, UserActionOldList theAction, bool proposalOnly, int? pointsManagerID, int? TblID)
        {
            return DataAccess.CheckUserRights(userID, theAction, proposalOnly, pointsManagerID, TblID);
        }
        public int CreateRatingPhaseGroup(String name,bool makeActive,bool makeActiveNow,int? creator,int? changesGroupID)
        {
            int? theRatingPhaseGroupId = null;
            if(DataManipulation.ProceedWithChange(ref changesGroupID,creator,UserActionOldList.ChangeCharacteristics,false,null,null, true))
            {
                theRatingPhaseGroupId = DataManipulation.AddRatingPhaseGroup(name, creator);
                if (makeActive)
                {
                    int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.RatingPhaseGroup, true, false, false, false, false, false, false, false, "", theRatingPhaseGroupId, null, null, null, null, "", null);
                    if (makeActiveNow)
                        DataManipulation.ImplementChangesGroup((int)changesGroupID);
                }
            }
            return (int)theRatingPhaseGroupId;
            
        }

        //public void CommentForTblRowCreate(int entityId, int userId, string commentTitle, string commentText, DateTime date, int? changesGroupID)
        //{
        //    DataAccessModule.ConfirmObjectExists(entityId, TypeOfObject.TblRow);
        //    TblRow theTblRow = ObjDataAccess.GetTblRow(entityId);
        //    int? TableId = theTblRow.TblID;

        //    if (DataAccessModule.ProceedWithChange(ref changesGroupID,userId,UserActionOldList.ChangeTblRows,false,null,TableId))
        //    {
        //        int newObjectID = DataAccessModule.AddComment(entityId, userId, commentTitle, commentText, date);
        //        int theChange = DataAccessModule.AddChangesStatusOfObject((int)changesGroupID,TypeOfObject.Comment,true,false,false,false,false,false,false,false,"",newObjectID,null,null,null,null,"",null);
        //        DataAccessModule.ImplementChangesGroup((int)changesGroupID);
        //    }
            
        //}

        //public void CommentForTblRowDelete(int CommentId, int userId, ref int? changesGroupID)
        //{
        //    DataAccessModule.ConfirmObjectExists(CommentId, TypeOfObject.Comment);
        //    int? TableId = ObjDataAccess.RaterooDB.GetTable<Comment>().Single(c => c.CommentsID == CommentId).TblRow.TblID;
        //    if (DataAccessModule.ProceedWithChange(ref changesGroupID,userId,UserActionOldList.ChangeTblRows,false,null,TableId))
        //    {
        //        int theChange = DataAccessModule.AddChangesStatusOfObject((int)changesGroupID,TypeOfObject.Comment,false,true,false,false,false,false,false,false,"",CommentId,null,null,null,null,"",null);
        //        DataAccessModule.ImplementChangesGroup((int)changesGroupID);
        //    }
        //}

        public ChoiceGroupData GetChoiceGroupData(int choiceGroupID, bool availableChoicesOnly, int? determiningGroupValue)
        {
            return DataAccess.GetChoiceGroupData(choiceGroupID, availableChoicesOnly, determiningGroupValue);
        }
        public int AddInvitedUser(string emailId, bool mayView, bool mayPredict, bool mayAddTbls,
           bool mayResolveRatings, bool mayChangeTblRows, bool mayChangeChoiceGroups, bool mayChangeCharacteristics,
           bool mayChangeCategories, bool mayChangeUsersRights, bool mayAdjustPoints, bool mayChangeProposalSettings)
        {
            return DataManipulation.AddInvitedUser(emailId,mayView,mayPredict,mayAddTbls,mayResolveRatings,mayChangeTblRows,mayChangeChoiceGroups,mayChangeCharacteristics,mayChangeCategories,mayChangeUsersRights,mayAdjustPoints,mayChangeProposalSettings);

        }
        public UserLoginResult CheckValidUser(string username, string password)
        {
            return DataAccess.CheckValidUser(username, password);
        }
        public void SetUserVerificationStatus(int userID, bool isVerified,int? changesGroupID)
        {

            DataManipulation.ConfirmObjectExists(userID, TypeOfObject.User);

            changesGroupID = DataManipulation.AddChangesGroup(null, null, userID, null, null, StatusOfChanges.NotYetProposed, null, null);
          
               // DataAccessModule.SetUserVerificationStatus(userID, isVerified);
                int theChange = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.User, false, false, false,false,true, false, false, false, "",null,userID,isVerified, null, null, "", null); 
                DataManipulation.ImplementChangesGroup((int)changesGroupID); 
          
          
        }
        public bool PointsManagerAdministrationLinkVisible(int userID, int pointsManagerID)
        {
            return DataAccess.PointsManagerAdministrationLinkVisible(userID, pointsManagerID);
        }
        public bool TblAdministrationLinkVisible(int userID, int TblID)
        {
            return DataAccess.TblAdministrationLinkVisible(userID, TblID);
        }
        public void TblChangeAppearance(int TblID, int? tableDimesionID, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(TblID, TypeOfObject.Tbl);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, null, TblID, true))
            {
                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Tbl, false, false, false, false, false, false, true, false, "", null, TblID, null, tableDimesionID, null, "TblDimension", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);


            }
        }
        public void PointsManagerChangeAppearance(int pointsManagerID, int? cssTblID, int? tableDimesionID, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(pointsManagerID, TypeOfObject.PointsManager);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.AddTblsAndChangePointsManagers, !doItNow, pointsManagerID, null, true))
            {

                PointsManagerChangeSettings(pointsManagerID, null, null, null, null, null, null, doItNow, userID, changesGroupID);


            }
        }

        public void DomainChangeAppearance(int domainID, int? tableDimesionID, bool doItNow, int userID, int? changesGroupID)
        {
            DataManipulation.ConfirmObjectExists(domainID, TypeOfObject.Domain);
            if (DataManipulation.ProceedWithChange(ref changesGroupID, userID, UserActionOldList.Other, !doItNow, null, null, true))
            {
                int changesStatusObjectID = DataManipulation.AddChangesStatusOfObject((int)changesGroupID, TypeOfObject.Domain, false, false, false, false, true, false, true, false, "", null, domainID, null, tableDimesionID, null, "TblDimension", null);
                if (doItNow)
                    DataManipulation.ImplementChangesGroup((int)changesGroupID);

            }
        }

        public int TblDimensionCreate(int widthOfRowHeaderColumn, int maxWidthOfImageInRowHeaderCell, int maxHeightOfImageInRowHeaderCell, int maxWidthOfImageInTblRowPopUpWindow, int maxHeightOfImageInTblRowPopUpWindow, int widthOfTblRowPopUpWindow)
        {

            int theTblDimensionId = DataManipulation.AddTblDimensions(widthOfRowHeaderColumn, maxWidthOfImageInRowHeaderCell, maxHeightOfImageInRowHeaderCell, maxWidthOfImageInTblRowPopUpWindow, maxHeightOfImageInTblRowPopUpWindow, widthOfTblRowPopUpWindow);
            return theTblDimensionId;
        }

        public int ChangesGroupCreate(int? pointsManagerID, int? TblID, int? creator, int? makeChangeRatingID, int? rewardRatingID, DateTime? scheduleApprovalOrRejection, DateTime? scheduleImplementation)
        {
            return DataManipulation.AddChangesGroup(pointsManagerID, TblID, creator, makeChangeRatingID, rewardRatingID, StatusOfChanges.NotYetProposed, scheduleApprovalOrRejection, scheduleImplementation);
        }

        public void ResetDataContexts()
        {
            DataManipulation.ResetDataContexts();
        }

    }
}
