using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1.Misc;
using System.Data.Linq;
using System.Linq.Expressions;

namespace ClassLibrary1.Model
{

    public static class GetIRaterooDataContext
    {
        private static RaterooDataContext _UnderlyingSQLDataContextForInMemoryContext = 
            new RaterooDataContext("no real connection"); // RaterooSQLDataContext(true,true).GetRealDatabaseIfExists();

        public static bool UseRealDatabase = true;

        public static IRaterooDataContext New(bool doAllowChangeData, bool enableObjectTracking)
        {
            if (UseRealDatabase)
                return new RaterooSQLDataContext(doAllowChangeData, enableObjectTracking);
            else
                return new RaterooInMemoryDataContext();
        }
    }

    public interface IRaterooDataContext : IDataContextExtended
    {
        bool IsRealDatabase();

        RaterooDataContext GetRealDatabaseIfExists();

        System.IO.TextWriter Log { get; set; }

        System.Data.Common.DbCommand GetCommand(IQueryable query);

        bool ResolveConflictsIfPossible();

        void SetUserRatingAddOptions();

        void SetPageLoadOptions();

        void LoadStatsWithTrustTrackersAndUserInteractions();

        void SetUserRatingUpdatingLoadOptions();

        void SetUserRatingAddingLoadOptions();

        IQueryable<TblRow> UDFNearestNeighborsForTbl(float? latitude, float? longitude, int? maxNumResults, int? tblID);
        IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance);
    }


    public static class RaterooDataContextExtensions
    {

        public static void PreSubmittingChangesTasks(this IRaterooDataContext theDataContext)
        {
            DatabaseAndAzureRoleStatus.CheckPreventChanges(theDataContext);
            FastAccessTablesMaintenance.PushRowsRequiringUpdateToAzureQueue(theDataContext);
            StatusRecords.RecordRememberedStatusRecordChanges(theDataContext);
            theDataContext.RegisteredToBeInserted = new List<object>();
        }


        public static void RepeatedlyAttemptToSubmitChanges(this IRaterooDataContext theDataContext, System.Data.Linq.ConflictMode failureMode)
        {
            PreSubmittingChangesTasks(theDataContext);
            int numTries = 0;
        TRYSTART:
            try
            {
                numTries++;
                theDataContext.CompleteSubmitChanges(failureMode);
            }
            catch (ChangeConflictException)
            {
                if (numTries <= 3)
                {
                    bool resolvable = theDataContext.ResolveConflictsIfPossible();
                    if (resolvable)
                        goto TRYSTART;
                    else
                        throw;
                }
                else
                    throw;
            }
        }
    }


    //public static class RaterooDataContextTableExtensions
    //{
    //    public static IRepository<AddressField> AddressFields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<AddressField>();
    //    }

    //    public static IRepository<AdministrationRight> AdministrationRights(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<AdministrationRight>();
    //    }
        
    //    public static IRepository<AdministrationRightsGroup> AdministrationRightsGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<AdministrationRightsGroup>();
    //    }

    //    public static IRepository<ChangesGroup> ChangesGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ChangesGroup>();
    //    }

    //    public static IRepository<ChangesStatusOfObject> ChangesStatusOfObject(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ChangesStatusOfObject>();
    //    }

    //    public static IRepository<ChoiceField> ChoiceFields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ChoiceField>();
    //    }
        
    //    public static IRepository<ChoiceGroupFieldDefinition> ChoiceGroupFieldDefinitions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ChoiceGroupFieldDefinition>();
    //    }

    //    public static IRepository<ChoiceGroup> ChoiceGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ChoiceGroup>();
    //    }

    //    public static IRepository<ChoiceInField> ChoiceInFields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ChoiceInField>();
    //    }

    //    public static IRepository<ChoiceInGroup> ChoiceInGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ChoiceInGroup>();
    //    }
        
    //    public static IRepository<Comment> Comments(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<Comment>();
    //    }

    //    public static IRepository<DatabaseStatus> DatabaseStatuses(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<DatabaseStatus>();
    //    }

    //    public static IRepository<DateTimeFieldDefinition> DateTimeFieldDefinitions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<DateTimeFieldDefinition>();
    //    }

    //    public static IRepository<DateTimeField> DateTimeFields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<DateTimeField>();
    //    }
        
    //    public static IRepository<Domain> Domains(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<Domain>();
    //    }

    //    public static IRepository<FieldDefinition> FieldDefinitions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<FieldDefinition>();
    //    }

    //    public static IRepository<Field> Fields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<Field>();
    //    }

    //    public static IRepository<HierarchyItem> HierarchyItem(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<HierarchyItem>();
    //    }
        
    //    public static IRepository<InsertableContent> InsertableContents(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<InsertableContent>();
    //    }

    //    public static IRepository<InvitedUser> InvitedUsers(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<InvitedUser>();
    //    }

    //    public static IRepository<LongProcess> LongProcesses(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<LongProcess>();
    //    }

    //    public static IRepository<NumberFieldDefinition> NumberFieldDefinitions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<NumberFieldDefinition>();
    //    }
        
    //    public static IRepository<NumberField> NumberFields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<NumberField>();
    //    }

    //    public static IRepository<OverrideCharacteristic> OverrideCharacteristics(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<OverrideCharacteristic>();
    //    }

    //    public static IRepository<PointsAdjustment> PointsAdjustments(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<PointsAdjustment>();
    //    }

    //    public static IRepository<PointsManager> PointsManager(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<PointsManager>();
    //    }
        
    //    public static IRepository<PointsTotal> PointsTotals(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<PointsTotal>();
    //    }

    //    public static IRepository<PointsTrustRule> PointsTrustRules(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<PointsTrustRule>();
    //    }

    //    public static IRepository<ProposalEvaluationRatingSetting> ProposalEvaluationRatingSettings(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ProposalEvaluationRatingSetting>();
    //    }

    //    public static IRepository<ProposalSetting> ProposalSettings(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<ProposalSetting>();
    //    }
        
    //    public static IRepository<RatingCharacteristic> RatingCharacteristics(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingCharacteristic>();
    //    }

    //    public static IRepository<RatingCondition> RatingConditions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingCondition>();
    //    }

    //    public static IRepository<RatingGroupAttribute> RatingGroupAttributes(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingGroupAttribute>();
    //    }

    //    public static IRepository<RatingGroupPhaseStatus> RatingGroupPhaseStatuses(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingGroupPhaseStatus>();
    //    }
        
    //    public static IRepository<RatingGroupResolution> RatingGroupResolutions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingGroupResolution>();
    //    }

    //    public static IRepository<RatingGroup> RatingGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingGroup>();
    //    }

    //    public static IRepository<RatingGroupStatusRecord> RatingGroupStatusRecords(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingGroupStatusRecord>();
    //    }

    //    public static IRepository<RatingPhaseGroup> RatingPhaseGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingPhaseGroup>();
    //    }
        
    //    public static IRepository<RatingPhase> RatingPhases(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingPhase>();
    //    }

    //    public static IRepository<RatingPhaseStatus> RatingPhaseStatuses(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingPhaseStatus>();
    //    }

    //    public static IRepository<RatingPlan> RatingPlans(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RatingPlan>();
    //    }

    //    public static IRepository<Rating> Ratings(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<Rating>();
    //    }
        
    //    public static IRepository<RewardPendingPointsTracker> RewardPendingPointsTrackers(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RewardPendingPointsTracker>();
    //    }

    //    public static IRepository<RewardRatingSetting> RewardRatingSettings(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RewardRatingSetting>();
    //    }


    //    public static IRepository<RoleStatus> RoleStatuses(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<RoleStatus>();
    //    }


    //    public static IRepository<SearchWordChoice> SearchWordChoicees(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SearchWordChoice>();
    //    }


    //    public static IRepository<SearchWordHierarchyItem> SearchWordHierarchyItems(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SearchWordHierarchyItem>();
    //    }

    //    public static IRepository<SearchWord> SearchWords(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SearchWord>();
    //    }


    //    public static IRepository<SearchWordTblRowName> SearchWordTblRowNames(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SearchWordTblRowName>();
    //    }

    //    public static IRepository<SearchWordTextField> SearchWordTextFields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SearchWordTextField>();
    //    }

    //    public static IRepository<SubsidyAdjustment> SubsidyAdjustments(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SubsidyAdjustment>();
    //    }

    //    public static IRepository<SubsidyDensityRangeGroup> SubsidyDensityRangeGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SubsidyDensityRangeGroup>();
    //    }

    //    public static IRepository<SubsidyDensityRange> SubsidyDensityRanges(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<SubsidyDensityRange>();
    //    }

    //    public static IRepository<TblColumnFormatting> TblColumnFormattings(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TblColumnFormatting>();
    //    }

    //    public static IRepository<TblColumn> TblColumns(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TblColumn>();
    //    }

    //    public static IRepository<TblDimension> TblDimensions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TblDimension>();
    //    }

    //    public static IRepository<TblRowFieldDisplay> TblRowFieldDisplays(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TblRowFieldDisplay>();
    //    }

    //    public static IRepository<TblRow> TblRows(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TblRow>();
    //    }

    //    public static IRepository<TblRowStatusRecord> TblRowStatusRecords(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TblRowStatusRecord>();
    //    }

    //    public static IRepository<Tbl> Tbls(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<Tbl>();
    //    }

    //    public static IRepository<TblTab> TblTabs(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TblTab>();
    //    }

    //    public static IRepository<TextFieldDefinition> TextFieldDefinitions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TextFieldDefinition>();
    //    }

    //    public static IRepository<TextField> TextFields(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TextField>();
    //    }

    //    public static IRepository<TrustTracker> TrustTrackers(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TrustTracker>();
    //    }

    //    public static IRepository<TrustTrackerStat> TrustTrackerStats(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TrustTrackerStat>();
    //    }

    //    public static IRepository<TrustTrackerUnit> TrustTrackerUnits(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<TrustTrackerUnit>();
    //    }

    //    public static IRepository<UserAction> UserActions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserAction>();
    //    }

    //    public static IRepository<UserCheckIn> UserCheckIns(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserCheckIn>();
    //    }

    //    public static IRepository<UserInfo> UserInfos(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserInfo>();
    //    }

    //    public static IRepository<UserInteraction> UserInteractions(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserInteraction>();
    //    }

    //    public static IRepository<UserInteractionStat> UserInteractionStat(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserInteractionStat>();
    //    }

    //    public static IRepository<UserRatingGroup> UserRatingGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserRatingGroup>();
    //    }

    //    public static IRepository<UserRating> UserRatings(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserRating>();
    //    }

    //    public static IRepository<UserRatingsToAdd> UserRatingsToAdds(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UserRatingsToAdd>();
    //    }

    //    public static IRepository<User> Users(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<User>();
    //    }

    //    public static IRepository<UsersAdministrationRightsGroup> UsersAdministrationRightsGroups(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UsersAdministrationRightsGroup>();
    //    }

    //    public static IRepository<UsersRight> UsersRights(this IRaterooDataContext theDataContext)
    //    {
    //        return theDataContext.GetTable<UsersRight>();
    //    }


    //}

}
