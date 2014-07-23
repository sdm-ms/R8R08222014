namespace ClassLibrary1.EFModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.Core.Objects;
    using System.Collections.Generic;
using System.Data.Common;

    public partial class R8RContext : DbContext
    {
        public R8RContext()
            : base()
        {
        }

        public R8RContext(string connectionString)
            : base(connectionString)
        {
        }

        public R8RContext(DbConnection connection)
            : base(connection, true)
        {
        }

        public virtual DbSet<AddressField> AddressFields { get; set; }
        public virtual DbSet<AdministrationRight> AdministrationRights { get; set; }
        public virtual DbSet<AdministrationRightsGroup> AdministrationRightsGroups { get; set; }
        public virtual DbSet<ChangesGroup> ChangesGroups { get; set; }
        public virtual DbSet<ChangesStatusOfObject> ChangesStatusOfObjects { get; set; }
        public virtual DbSet<ChoiceField> ChoiceFields { get; set; }
        public virtual DbSet<ChoiceGroupFieldDefinition> ChoiceGroupFieldDefinitions { get; set; }
        public virtual DbSet<ChoiceGroup> ChoiceGroups { get; set; }
        public virtual DbSet<ChoiceInField> ChoiceInFields { get; set; }
        public virtual DbSet<ChoiceInGroup> ChoiceInGroups { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<DatabaseStatus> DatabaseStatus { get; set; }
        public virtual DbSet<DateTimeFieldDefinition> DateTimeFieldDefinitions { get; set; }
        public virtual DbSet<DateTimeField> DateTimeFields { get; set; }
        public virtual DbSet<Domain> Domains { get; set; }
        public virtual DbSet<FieldDefinition> FieldDefinitions { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<HierarchyItem> HierarchyItems { get; set; }
        public virtual DbSet<InsertableContent> InsertableContents { get; set; }
        public virtual DbSet<InvitedUser> InvitedUsers { get; set; }
        public virtual DbSet<LongProcess> LongProcesses { get; set; }
        public virtual DbSet<NumberFieldDefinition> NumberFieldDefinitions { get; set; }
        public virtual DbSet<NumberField> NumberFields { get; set; }
        public virtual DbSet<OverrideCharacteristic> OverrideCharacteristics { get; set; }
        public virtual DbSet<PointsAdjustment> PointsAdjustments { get; set; }
        public virtual DbSet<PointsManager> PointsManagers { get; set; }
        public virtual DbSet<PointsTotal> PointsTotals { get; set; }
        public virtual DbSet<ProposalEvaluationRatingSetting> ProposalEvaluationRatingSettings { get; set; }
        public virtual DbSet<ProposalSetting> ProposalSettings { get; set; }
        public virtual DbSet<RatingCharacteristic> RatingCharacteristics { get; set; }
        public virtual DbSet<RatingCondition> RatingConditions { get; set; }
        public virtual DbSet<RatingGroupAttribute> RatingGroupAttributes { get; set; }
        public virtual DbSet<RatingGroupPhaseStatus> RatingGroupPhaseStatus { get; set; }
        public virtual DbSet<RatingGroupResolution> RatingGroupResolutions { get; set; }
        public virtual DbSet<RatingGroup> RatingGroups { get; set; }
        public virtual DbSet<RatingGroupStatusRecord> RatingGroupStatusRecords { get; set; }
        public virtual DbSet<RatingPhaseGroup> RatingPhaseGroups { get; set; }
        public virtual DbSet<RatingPhase> RatingPhases { get; set; }
        public virtual DbSet<RatingPhaseStatus> RatingPhaseStatus { get; set; }
        public virtual DbSet<RatingPlan> RatingPlans { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<RewardPendingPointsTracker> RewardPendingPointsTrackers { get; set; }
        public virtual DbSet<RewardRatingSetting> RewardRatingSettings { get; set; }
        public virtual DbSet<RoleStatus> RoleStatus { get; set; }
        public virtual DbSet<SearchWordChoice> SearchWordChoices { get; set; }
        public virtual DbSet<SearchWordHierarchyItem> SearchWordHierarchyItems { get; set; }
        public virtual DbSet<SearchWord> SearchWords { get; set; }
        public virtual DbSet<SearchWordTblRowName> SearchWordTblRowNames { get; set; }
        public virtual DbSet<SearchWordTextField> SearchWordTextFields { get; set; }
        public virtual DbSet<SubsidyAdjustment> SubsidyAdjustments { get; set; }
        public virtual DbSet<SubsidyDensityRangeGroup> SubsidyDensityRangeGroups { get; set; }
        public virtual DbSet<SubsidyDensityRange> SubsidyDensityRanges { get; set; }
        public virtual DbSet<TblColumnFormatting> TblColumnFormattings { get; set; }
        public virtual DbSet<TblColumn> TblColumns { get; set; }
        public virtual DbSet<TblDimension> TblDimensions { get; set; }
        public virtual DbSet<TblRowFieldDisplay> TblRowFieldDisplays { get; set; }
        public virtual DbSet<TblRow> TblRows { get; set; }
        public virtual DbSet<TblRowStatusRecord> TblRowStatusRecords { get; set; }
        public virtual DbSet<Tbl> Tbls { get; set; }
        public virtual DbSet<TblTab> TblTabs { get; set; }
        public virtual DbSet<TextFieldDefinition> TextFieldDefinitions { get; set; }
        public virtual DbSet<TextField> TextFields { get; set; }
        public virtual DbSet<TrustTrackerForChoiceInGroup> TrustTrackerForChoiceInGroups { get; set; }
        public virtual DbSet<TrustTrackerForChoiceInGroupsUserRatingLink> TrustTrackerForChoiceInGroupsUserRatingLinks { get; set; }
        public virtual DbSet<TrustTracker> TrustTrackers { get; set; }
        public virtual DbSet<TrustTrackerStat> TrustTrackerStats { get; set; }
        public virtual DbSet<TrustTrackerUnit> TrustTrackerUnits { get; set; }
        public virtual DbSet<UniquenessLockReference> UniquenessLockReferences { get; set; }
        public virtual DbSet<UniquenessLock> UniquenessLocks { get; set; }
        public virtual DbSet<UserAction> UserActions { get; set; }
        public virtual DbSet<UserCheckIn> UserCheckIns { get; set; }
        public virtual DbSet<UserInfo> UserInfoes { get; set; }
        public virtual DbSet<UserInteraction> UserInteractions { get; set; }
        public virtual DbSet<UserInteractionStat> UserInteractionStats { get; set; }
        public virtual DbSet<UserRatingGroup> UserRatingGroups { get; set; }
        public virtual DbSet<UserRating> UserRatings { get; set; }
        public virtual DbSet<UserRatingsToAdd> UserRatingsToAdds { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersAdministrationRightsGroup> UsersAdministrationRightsGroups { get; set; }
        public virtual DbSet<UsersRight> UsersRights { get; set; }
        public virtual DbSet<VolatilityTblRowTracker> VolatilityTblRowTrackers { get; set; }
        public virtual DbSet<VolatilityTracker> VolatilityTrackers { get; set; }

        public virtual DbSet<ForumAdministrator> ForumAdministrators { get; set; }
        public virtual DbSet<ForumGroupPermission> ForumGroupPermissions { get; set; }
        public virtual DbSet<ForumGroup> ForumGroups { get; set; }
        public virtual DbSet<ForumMessageRating> ForumMessageRatings { get; set; }
        public virtual DbSet<ForumMessage> ForumMessages { get; set; }
        public virtual DbSet<ForumModerator> ForumModerators { get; set; }
        public virtual DbSet<ForumNewTopicSubscription> ForumNewTopicSubscriptions { get; set; }
        public virtual DbSet<ForumPersonalMessage> ForumPersonalMessages { get; set; }
        public virtual DbSet<ForumPollAnswer> ForumPollAnswers { get; set; }
        public virtual DbSet<ForumPollOption> ForumPollOptions { get; set; }
        public virtual DbSet<ForumPoll> ForumPolls { get; set; }
        public virtual DbSet<Forum> Forums { get; set; }
        public virtual DbSet<ForumSubforum> ForumSubforums { get; set; }
        public virtual DbSet<ForumSubscription> ForumSubscriptions { get; set; }
        public virtual DbSet<ForumTopic> ForumTopics { get; set; }
        public virtual DbSet<ForumUploadedFile> ForumUploadedFiles { get; set; }
        public virtual DbSet<ForumUploadedPersonalFile> ForumUploadedPersonalFiles { get; set; }
        public virtual DbSet<ForumUserGroup> ForumUserGroups { get; set; }
        public virtual DbSet<ForumUser> ForumUsers { get; set; }
        public virtual DbSet<ForumUsersInGroup> ForumUsersInGroups { get; set; }
        public virtual DbSet<ForumComplaint> ForumComplaints { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2")); 

            modelBuilder.Entity<AddressField>()
                .Property(e => e.Latitude)
                .HasPrecision(18, 8);

            modelBuilder.Entity<AddressField>()
                .Property(e => e.Longitude)
                .HasPrecision(18, 8);

            modelBuilder.Entity<AdministrationRightsGroup>()
                .HasMany(e => e.AdministrationRights)
                .WithRequired(e => e.AdministrationRightsGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AdministrationRightsGroup>()
                .HasMany(e => e.UsersAdministrationRightsGroups)
                .WithRequired(e => e.AdministrationRightsGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangesGroup>()
                .HasMany(e => e.ChangesStatusOfObjects)
                .WithRequired(e => e.ChangesGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangesStatusOfObject>()
                .Property(e => e.NewValueDecimal)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ChoiceField>()
                .HasMany(e => e.ChoiceInFields)
                .WithRequired(e => e.ChoiceField)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChoiceGroupFieldDefinition>()
                .HasMany(e => e.ChoiceGroupFieldDefinitions1)
                .WithOptional(e => e.ChoiceGroupFieldDefinition1)
                .HasForeignKey(e => e.DependentOnChoiceGroupFieldDefinitionID);

            modelBuilder.Entity<ChoiceGroup>()
                .HasMany(e => e.ChoiceGroupFieldDefinitions)
                .WithRequired(e => e.ChoiceGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChoiceGroup>()
                .HasMany(e => e.ChoiceInGroups)
                .WithRequired(e => e.ChoiceGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChoiceGroup>()
                .HasMany(e => e.ChoiceGroupsDependentOnThisChoiceGroup)
                .WithOptional(e => e.ChoiceGroupOnWhichThisChoiceGroupIsDependent)
                .HasForeignKey(e => e.DependentOnChoiceGroupID);

            modelBuilder.Entity<ChoiceInGroup>()
                .HasMany(e => e.ChoiceInFields)
                .WithRequired(e => e.ChoiceInGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChoiceInGroup>()
                .HasMany(e => e.ChoiceInGroupsDependentOnThisChoiceInGroup)
                .WithOptional(e => e.ChoiceInGroupOnWhichThisChoiceInGroupIsDependent)
                .HasForeignKey(e => e.ActiveOnDeterminingGroupChoiceInGroupID);

            modelBuilder.Entity<ChoiceInGroup>()
                .HasMany(e => e.SearchWordChoices)
                .WithRequired(e => e.ChoiceInGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChoiceInGroup>()
                .HasMany(e => e.TrustTrackerForChoiceInGroups)
                .WithRequired(e => e.ChoiceInGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Comment>()
                .Property(e => e.CommentTitle)
                .IsUnicode(true);

            modelBuilder.Entity<Comment>()
                .Property(e => e.CommentText)
                .IsUnicode(true);

            modelBuilder.Entity<Domain>()
                .HasMany(e => e.PointsManagers)
                .WithRequired(e => e.Domain)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FieldDefinition>()
                .HasMany(e => e.ChoiceGroupFieldDefinitions)
                .WithRequired(e => e.FieldDefinition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FieldDefinition>()
                .HasMany(e => e.DateTimeFieldDefinitions)
                .WithRequired(e => e.FieldDefinition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FieldDefinition>()
                .HasMany(e => e.Fields)
                .WithRequired(e => e.FieldDefinition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FieldDefinition>()
                .HasMany(e => e.NumberFieldDefinitions)
                .WithRequired(e => e.FieldDefinition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FieldDefinition>()
                .HasMany(e => e.TextFieldDefinitions)
                .WithRequired(e => e.FieldDefinition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Field>()
                .HasMany(e => e.AddressFields)
                .WithRequired(e => e.Field)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Field>()
                .HasMany(e => e.ChoiceFields)
                .WithRequired(e => e.Field)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Field>()
                .HasMany(e => e.DateTimeFields)
                .WithRequired(e => e.Field)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Field>()
                .HasMany(e => e.NumberFields)
                .WithRequired(e => e.Field)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Field>()
                .HasMany(e => e.TextFields)
                .WithRequired(e => e.Field)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HierarchyItem>()
                .HasMany(e => e.ChildHierarchyItems)
                .WithOptional(e => e.ParentHierarchyItem)
                .HasForeignKey(e => e.ParentHierarchyItemID);

            modelBuilder.Entity<HierarchyItem>()
                .HasMany(e => e.SearchWordHierarchyItems)
                .WithRequired(e => e.HierarchyItem)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InsertableContent>()
                .Property(e => e.Name)
                .IsUnicode(true);

            modelBuilder.Entity<InsertableContent>()
                .Property(e => e.Content)
                .IsUnicode(true);

            modelBuilder.Entity<NumberFieldDefinition>()
                .Property(e => e.Minimum)
                .HasPrecision(18, 4);

            modelBuilder.Entity<NumberFieldDefinition>()
                .Property(e => e.Maximum)
                .HasPrecision(18, 4);

            modelBuilder.Entity<NumberField>()
                .Property(e => e.Number)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsAdjustment>()
                .Property(e => e.TotalAdjustment)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsAdjustment>()
                .Property(e => e.CurrentAdjustment)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.TotalUserPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.CurrentUserPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.CurrentUserPendingPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.CurrentUserNotYetPendingPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.CurrentPointsToCount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.HighStakesProbability)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.HighStakesSecretMultiplier)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.HighStakesKnownMultiplier)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.DatabaseChangeSelectHighStakesNoviceNumPct)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.DollarValuePerPoint)
                .HasPrecision(18, 8);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.DiscountForGuarantees)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.MaximumTotalGuarantees)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.TotalUnconditionalGuaranteesEarnedEver)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.TotalConditionalGuaranteesEarnedEver)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .Property(e => e.TotalConditionalGuaranteesPending)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsManager>()
                .HasMany(e => e.ChoiceGroups)
                .WithRequired(e => e.PointsManager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PointsManager>()
                .HasMany(e => e.PointsAdjustments)
                .WithRequired(e => e.PointsManager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PointsManager>()
                .HasMany(e => e.PointsTotals)
                .WithRequired(e => e.PointsManager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PointsManager>()
                .HasMany(e => e.Tbls)
                .WithRequired(e => e.PointsManager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PointsManager>()
                .HasMany(e => e.UsersAdministrationRightsGroups)
                .WithRequired(e => e.PointsManager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.CurrentPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.TotalPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.PotentialMaxLossOnNotYetPending)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.PendingPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.NotYetPendingPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.TrustPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.TrustPointsRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.PointsPerRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.TotalTimeThisCheckInPeriod)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.TotalTimeThisRewardPeriod)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.TotalTimeEver)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.PointsPerHour)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.ProjectedPointsPerHour)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.PendingConditionalGuaranteeTotalHoursAtStart)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.PendingConditionalGuaranteeTotalHoursNeeded)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.TotalPointsOrPendingPointsLongTermUnweighted)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PointsTotal>()
                .Property(e => e.PointsPerRatingLongTerm)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalEvaluationRatingSetting>()
                .Property(e => e.MinValueToApprove)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalEvaluationRatingSetting>()
                .Property(e => e.MaxValueToReject)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalEvaluationRatingSetting>()
                .Property(e => e.MinProportionOfThisTime)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.MinValueToApprove)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.MaxValueToReject)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.MinProportionOfThisTime)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.MaxBonusForProposal)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.MaxPenaltyForRejection)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.SubsidyForApprovalRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.SubsidyForRewardRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ProposalSetting>()
                .Property(e => e.RequiredPointsToMakeProposal)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingCharacteristic>()
                .Property(e => e.MinimumUserRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingCharacteristic>()
                .Property(e => e.MaximumUserRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingCharacteristic>()
                .HasMany(e => e.Ratings)
                .WithRequired(e => e.RatingCharacteristic)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingCharacteristic>()
                .HasMany(e => e.RatingGroupAttributes)
                .WithRequired(e => e.RatingCharacteristic)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingCondition>()
                .Property(e => e.GreaterThan)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingCondition>()
                .Property(e => e.LessThan)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingGroupAttribute>()
                .Property(e => e.ConstrainedSum)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingGroupAttribute>()
                .Property(e => e.Description)
                .IsUnicode(true);

            modelBuilder.Entity<RatingGroupAttribute>()
                .Property(e => e.LongTermPointsWeight)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingGroupAttribute>()
                .HasMany(e => e.OverrideCharacteristics)
                .WithRequired(e => e.RatingGroupAttribute)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupAttribute>()
                .HasMany(e => e.ProposalEvaluationRatingSettings)
                .WithRequired(e => e.RatingGroupAttribute)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupAttribute>()
                .HasMany(e => e.RatingGroups)
                .WithRequired(e => e.RatingGroupAttribute)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupAttribute>()
                .HasMany(e => e.RatingPlans)
                .WithRequired(e => e.RatingGroupAttribute)
                .HasForeignKey(e => e.RatingGroupAttributesID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupAttribute>()
                .HasMany(e => e.RatingPlans1)
                .WithOptional(e => e.RatingGroupAttribute1)
                .HasForeignKey(e => e.OwnedRatingGroupAttributesID);

            modelBuilder.Entity<RatingGroupAttribute>()
                .HasMany(e => e.RewardRatingSettings)
                .WithRequired(e => e.RatingGroupAttribute)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupAttribute>()
                .HasMany(e => e.TblColumns)
                .WithRequired(e => e.RatingGroupAttribute)
                .HasForeignKey(e => e.DefaultRatingGroupAttributesID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupPhaseStatus>()
                .HasMany(e => e.RatingPhaseStatus)
                .WithRequired(e => e.RatingGroupPhaseStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupPhaseStatus>()
                .HasMany(e => e.UserRatingGroups)
                .WithRequired(e => e.RatingGroupPhaseStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupPhaseStatus>()
                .HasMany(e => e.SubsidyAdjustments)
                .WithRequired(e => e.RatingGroupPhaseStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroup>()
                .Property(e => e.CurrentValueOfFirstRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.RatingGroupPhaseStatuses)
                .WithRequired(e => e.RatingGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.RatingGroupResolutions)
                .WithRequired(e => e.RatingGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.RatingGroupStatusRecords)
                .WithRequired(e => e.RatingGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.UserRatingsToAdds)
                .WithRequired(e => e.RatingGroup)
                .HasForeignKey(e => e.TopRatingGroupID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.VolatilityTrackers)
                .WithRequired(e => e.RatingGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.Ratings)
                .WithRequired(e => e.RatingGroup)
                .HasForeignKey(e => e.RatingGroupID)
                .WillCascadeOnDelete(false);

            // TODO: Fix this. (We're not using this relationship now, but if we do we should fix it.)
            // Every Rating can have zero or 1 OwnedRatingGroup, using foreign key OwnedRatingGroupID.
            // But the RatingGroup can be owned by zero or 1 Rating, and there is no foreign key on the RatingGroup side of the relationship.
            // So, instead of RatingsAboveThisRatingGroupInHierarchy we should have RatingAboveThisRatingGroupInHierarchy, referring to a single one.
            // But I haven't been able to get that to work in a way that specifies the foreign key.
            // Using Map is a possibility but I would prefer to use HasKey.
            // See email with Leader/Follower example.
            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.RatingsAboveThisRatingGroupInHierarchy) 
                .WithOptional(e => e.OwnedRatingGroup)
                .HasForeignKey(e => e.OwnedRatingGroupID);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.RatingsWithinTopRatingGroupHierarchy)
                .WithRequired(e => e.TopRatingGroup)
                .HasForeignKey(e => e.TopmostRatingGroupID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroup>()
                .HasMany(e => e.UserRatingGroups)
                .WithRequired(e => e.RatingGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingGroupStatusRecord>()
                .Property(e => e.OldValueOfFirstRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingPhaseGroup>()
                .HasMany(e => e.RatingCharacteristics)
                .WithRequired(e => e.RatingPhaseGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingPhaseGroup>()
                .HasMany(e => e.RatingGroupPhaseStatuses)
                .WithRequired(e => e.RatingPhaseGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingPhaseGroup>()
                .HasMany(e => e.RatingPhases)
                .WithRequired(e => e.RatingPhaseGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingPhase>()
                .Property(e => e.SubsidyLevel)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingPhase>()
                .HasMany(e => e.RatingGroupPhaseStatus)
                .WithRequired(e => e.RatingPhase)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingPhaseStatus>()
                .Property(e => e.ShortTermResolutionValue)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingPhaseStatus>()
                .HasMany(e => e.UserRatings)
                .WithRequired(e => e.RatingPhaseStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatingPlan>()
                .Property(e => e.DefaultUserRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RatingPlan>()
                .Property(e => e.Description)
                .IsUnicode(true);

            modelBuilder.Entity<Rating>()
                .Property(e => e.CurrentValue)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Rating>()
                .Property(e => e.LastTrustedValue)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Rating>()
                .HasMany(e => e.ChangesGroups)
                .WithOptional(e => e.Rating)
                .HasForeignKey(e => e.RewardRatingID);

            modelBuilder.Entity<Rating>()
                .HasMany(e => e.RatingConditions)
                .WithOptional(e => e.Rating)
                .HasForeignKey(e => e.ConditionRatingID);

            modelBuilder.Entity<Rating>()
                .HasMany(e => e.RatingPhaseStatus)
                .WithRequired(e => e.Rating)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Rating>()
                .HasMany(e => e.UserRatings)
                .WithRequired(e => e.Rating)
                .HasForeignKey(e => e.RatingID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RewardPendingPointsTracker>()
                .Property(e => e.PendingRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RewardRatingSetting>()
                .Property(e => e.ProbOfRewardEvaluation)
                .HasPrecision(18, 4);

            modelBuilder.Entity<RewardRatingSetting>()
                .Property(e => e.Multiplier)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SearchWord>()
                .HasMany(e => e.SearchWordChoices)
                .WithRequired(e => e.SearchWord)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SearchWord>()
                .HasMany(e => e.SearchWordHierarchyItems)
                .WithRequired(e => e.SearchWord)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SearchWord>()
                .HasMany(e => e.SearchWordTblRowNames)
                .WithRequired(e => e.SearchWord)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SearchWord>()
                .HasMany(e => e.SearchWordTextFields)
                .WithRequired(e => e.SearchWord)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SubsidyAdjustment>()
                .Property(e => e.SubsidyAdjustmentFactor)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SubsidyDensityRangeGroup>()
                .Property(e => e.UseLogarithmBase)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SubsidyDensityRangeGroup>()
                .Property(e => e.CumDensityTotal)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SubsidyDensityRangeGroup>()
                .HasMany(e => e.SubsidyDensityRanges)
                .WithRequired(e => e.SubsidyDensityRangeGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SubsidyDensityRange>()
                .Property(e => e.RangeBottom)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SubsidyDensityRange>()
                .Property(e => e.RangeTop)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SubsidyDensityRange>()
                .Property(e => e.LiquidityFactor)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SubsidyDensityRange>()
                .Property(e => e.CumDensityBottom)
                .HasPrecision(18, 4);

            modelBuilder.Entity<SubsidyDensityRange>()
                .Property(e => e.CumDensityTop)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TblColumnFormatting>()
                .Property(e => e.ExtraDecimalPlaceAbove)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TblColumnFormatting>()
                .Property(e => e.ExtraDecimalPlace2Above)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TblColumnFormatting>()
                .Property(e => e.ExtraDecimalPlace3Above)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TblColumn>()
                .Property(e => e.ConditionGreaterThan)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TblColumn>()
                .Property(e => e.ConditionLessThan)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TblColumn>()
                .Property(e => e.Abbreviation)
                .IsFixedLength();

            modelBuilder.Entity<TblColumn>()
                .HasMany(e => e.OverrideCharacteristics)
                .WithRequired(e => e.TblColumn)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblColumn>()
                .HasMany(e => e.RatingGroups)
                .WithRequired(e => e.TblColumn)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblColumn>()
                .HasMany(e => e.TblColumnFormattings)
                .WithRequired(e => e.TblColumn)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblColumn>()
                .HasMany(e => e.TblColumnsConditionalOnThisColumn)
                .WithOptional(e => e.TblColumnOnWhichThisColumnIsConditional)
                .HasForeignKey(e => e.ConditionTblColumnID);

            modelBuilder.Entity<TblRowFieldDisplay>()
                .HasMany(e => e.TblRows)
                .WithRequired(e => e.TblRowFieldDisplay)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .Property(e => e.CountUserPoints)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.Comments)
                .WithRequired(e => e.TblRow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.Fields)
                .WithRequired(e => e.TblRow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.OverrideCharacteristics)
                .WithRequired(e => e.TblRow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.RatingGroups)
                .WithRequired(e => e.TblRow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.RewardPendingPointsTrackers)
                .WithRequired(e => e.TblRow)
                .HasForeignKey(e => e.RewardTblRowID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.SearchWordTblRowNames)
                .WithRequired(e => e.TblRow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.TblRowStatusRecords)
                .WithRequired(e => e.TblRow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TblRow>()
                .HasMany(e => e.VolatilityTblRowTrackers)
                .WithRequired(e => e.TblRow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tbl>()
                .Property(e => e.WordToDescribeGroupOfColumnsInThisTbl)
                .IsUnicode(true);

            modelBuilder.Entity<Tbl>()
                .Property(e => e.TypeOfTblRow)
                .IsUnicode(true);

            modelBuilder.Entity<Tbl>()
                .HasMany(e => e.FieldDefinitions)
                .WithRequired(e => e.Tbl)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tbl>()
                .HasMany(e => e.TblRows)
                .WithRequired(e => e.Tbl)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tbl>()
                .HasMany(e => e.TrustTrackerForChoiceInGroups)
                .WithRequired(e => e.Tbl)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TextField>()
                .HasMany(e => e.SearchWordTextFields)
                .WithRequired(e => e.TextField)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrustTrackerForChoiceInGroup>()
                .HasMany(e => e.TrustTrackerForChoiceInGroupsUserRatingLinks)
                .WithRequired(e => e.TrustTrackerForChoiceInGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrustTracker>()
                .HasMany(e => e.TrustTrackerStats)
                .WithRequired(e => e.TrustTracker)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrustTrackerStat>()
                .HasMany(e => e.UserInteractionStats)
                .WithRequired(e => e.TrustTrackerStat)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrustTrackerUnit>()
                .Property(e => e.ExtendIntervalWhenChangeIsLessThanThis)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TrustTrackerUnit>()
                .Property(e => e.ExtendIntervalMultiplier)
                .HasPrecision(18, 4);

            modelBuilder.Entity<TrustTrackerUnit>()
                .HasMany(e => e.TrustTrackers)
                .WithRequired(e => e.TrustTrackerUnit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrustTrackerUnit>()
                .HasMany(e => e.UserInteractions)
                .WithRequired(e => e.TrustTrackerUnit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserAction>()
                .Property(e => e.Text)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.FirstName)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.LastName)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.Email)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.Address1)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.Address2)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.WorkPhone)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.HomePhone)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.MobilePhone)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.ZipCode)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.City)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.State)
                .IsUnicode(true);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.Country)
                .IsUnicode(true);

            modelBuilder.Entity<UserRatingGroup>()
                .HasMany(e => e.UserRatings)
                .WithRequired(e => e.UserRatingGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PreviousRatingOrVirtualRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PreviousDisplayedRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.EnteredUserRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.NewUserRating)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.OriginalAdjustmentPct)
                .HasPrecision(7, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.OriginalTrustLevel)
                .HasPrecision(7, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.MaxGain)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.MaxLoss)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PotentialPointsShortTerm)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PotentialPointsLongTerm)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PotentialPointsLongTermUnweighted)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.LongTermPointsWeight)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PointsPumpingProportion)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PastPointsPumpingProportion)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.PercentPreviousRatings)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.LogarithmicBase)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.HighStakesMultiplierOverride)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.LastWeekDistanceFromStart)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.LastWeekPushback)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .Property(e => e.LastYearPushback)
                .HasPrecision(18, 4);

            modelBuilder.Entity<UserRating>()
                .HasMany(e => e.Ratings)
                .WithOptional(e => e.UserRating)
                .HasForeignKey(e => e.MostRecentUserRatingID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<UserRating>()
                .HasMany(e => e.TrustTrackerForChoiceInGroupsUserRatingLinks)
                .WithRequired(e => e.UserRating)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRating>()
                .HasMany(e => e.UserRatings1)
                .WithOptional(e => e.UserRating1)
                .HasForeignKey(e => e.MostRecentUserRatingID);

            modelBuilder.Entity<User>()
                .Property(e => e.TrustPointsRatioTotals)
                .HasPrecision(18, 4);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ChangesGroups)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ChoiceGroups)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Comments)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PointsAdjustments)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PointsTotals)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.RatingCharacteristics)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.RatingGroupResolutions)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.RatingPhaseGroups)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.RatingPlans)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Ratings)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.RewardPendingPointsTrackers)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SubsidyDensityRangeGroups)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Tbls)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Creator);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrustTrackerForChoiceInGroups)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrustTrackers)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.UserInfo)
                .WithRequired(e => e.User);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserInteractions)
                .WithRequired(e => e.OriginalRatingUser)
                .HasForeignKey(e => e.OrigRatingUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserInteractions1)
                .WithRequired(e => e.LatestRatingUser)
                .HasForeignKey(e => e.LatestRatingUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserRatings)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserRatingsToAdds)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VolatilityTblRowTracker>()
                .Property(e => e.TotalMovement)
                .HasPrecision(18, 4);

            modelBuilder.Entity<VolatilityTblRowTracker>()
                .Property(e => e.DistanceFromStart)
                .HasPrecision(18, 4);

            modelBuilder.Entity<VolatilityTblRowTracker>()
                .Property(e => e.Pushback)
                .HasPrecision(18, 4);

            modelBuilder.Entity<VolatilityTblRowTracker>()
                .Property(e => e.PushbackProportion)
                .HasPrecision(18, 4);

            modelBuilder.Entity<VolatilityTblRowTracker>()
                .HasMany(e => e.VolatilityTrackers)
                .WithRequired(e => e.VolatilityTblRowTracker)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VolatilityTracker>()
                .Property(e => e.TotalMovement)
                .HasPrecision(18, 4);

            modelBuilder.Entity<VolatilityTracker>()
                .Property(e => e.DistanceFromStart)
                .HasPrecision(18, 4);

            modelBuilder.Entity<VolatilityTracker>()
                .Property(e => e.Pushback)
                .HasPrecision(18, 4);

            modelBuilder.Entity<VolatilityTracker>()
                .Property(e => e.PushbackProportion)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ForumMessage>()
                .Property(e => e.IPAddress)
                .IsUnicode(false);
        }

    }
}
