﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFExperimental
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Rateroo7Entities : DbContext
    {
        public Rateroo7Entities()
            : base("name=Rateroo7Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AddressField> AddressFields { get; set; }
        public DbSet<AdministrationRight> AdministrationRights { get; set; }
        public DbSet<AdministrationRightsGroup> AdministrationRightsGroups { get; set; }
        public DbSet<ChangesGroup> ChangesGroups { get; set; }
        public DbSet<ChangesStatusOfObject> ChangesStatusOfObjects { get; set; }
        public DbSet<ChoiceField> ChoiceFields { get; set; }
        public DbSet<ChoiceGroupFieldDefinition> ChoiceGroupFieldDefinitions { get; set; }
        public DbSet<ChoiceGroup> ChoiceGroups { get; set; }
        public DbSet<ChoiceInField> ChoiceInFields { get; set; }
        public DbSet<ChoiceInGroup> ChoiceInGroups { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<DatabaseStatu> DatabaseStatus { get; set; }
        public DbSet<DateTimeFieldDefinition> DateTimeFieldDefinitions { get; set; }
        public DbSet<DateTimeField> DateTimeFields { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<FieldDefinition> FieldDefinitions { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<HierarchyItem> HierarchyItems { get; set; }
        public DbSet<InsertableContent> InsertableContents { get; set; }
        public DbSet<InvitedUser> InvitedUsers { get; set; }
        public DbSet<LongProcess> LongProcesses { get; set; }
        public DbSet<NumberFieldDefinition> NumberFieldDefinitions { get; set; }
        public DbSet<NumberField> NumberFields { get; set; }
        public DbSet<OverrideCharacteristic> OverrideCharacteristics { get; set; }
        public DbSet<PointsAdjustment> PointsAdjustments { get; set; }
        public DbSet<PointsManager> PointsManagers { get; set; }
        public DbSet<PointsTotal> PointsTotals { get; set; }
        public DbSet<ProposalEvaluationRatingSetting> ProposalEvaluationRatingSettings { get; set; }
        public DbSet<ProposalSetting> ProposalSettings { get; set; }
        public DbSet<RatingCharacteristic> RatingCharacteristics { get; set; }
        public DbSet<RatingCondition> RatingConditions { get; set; }
        public DbSet<RatingGroupAttribute> RatingGroupAttributes { get; set; }
        public DbSet<RatingGroupPhaseStatu> RatingGroupPhaseStatus { get; set; }
        public DbSet<RatingGroupResolution> RatingGroupResolutions { get; set; }
        public DbSet<RatingGroup> RatingGroups { get; set; }
        public DbSet<RatingGroupStatusRecord> RatingGroupStatusRecords { get; set; }
        public DbSet<RatingPhaseGroup> RatingPhaseGroups { get; set; }
        public DbSet<RatingPhas> RatingPhases { get; set; }
        public DbSet<RatingPhaseStatu> RatingPhaseStatus { get; set; }
        public DbSet<RatingPlan> RatingPlans { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RewardPendingPointsTracker> RewardPendingPointsTrackers { get; set; }
        public DbSet<RewardRatingSetting> RewardRatingSettings { get; set; }
        public DbSet<RoleStatu> RoleStatus { get; set; }
        public DbSet<SearchWordChoice> SearchWordChoices { get; set; }
        public DbSet<SearchWordHierarchyItem> SearchWordHierarchyItems { get; set; }
        public DbSet<SearchWord> SearchWords { get; set; }
        public DbSet<SearchWordTblRowName> SearchWordTblRowNames { get; set; }
        public DbSet<SearchWordTextField> SearchWordTextFields { get; set; }
        public DbSet<SubsidyAdjustment> SubsidyAdjustments { get; set; }
        public DbSet<SubsidyDensityRangeGroup> SubsidyDensityRangeGroups { get; set; }
        public DbSet<SubsidyDensityRange> SubsidyDensityRanges { get; set; }
        public DbSet<TblColumnFormatting> TblColumnFormattings { get; set; }
        public DbSet<TblColumn> TblColumns { get; set; }
        public DbSet<TblDimension> TblDimensions { get; set; }
        public DbSet<TblRowFieldDisplay> TblRowFieldDisplays { get; set; }
        public DbSet<TblRow> TblRows { get; set; }
        public DbSet<TblRowStatusRecord> TblRowStatusRecords { get; set; }
        public DbSet<Tbl> Tbls { get; set; }
        public DbSet<TblTab> TblTabs { get; set; }
        public DbSet<TextFieldDefinition> TextFieldDefinitions { get; set; }
        public DbSet<TextField> TextFields { get; set; }
        public DbSet<TrustTrackerForChoiceInGroup> TrustTrackerForChoiceInGroups { get; set; }
        public DbSet<TrustTrackerForChoiceInGroupsUserRatingLink> TrustTrackerForChoiceInGroupsUserRatingLinks { get; set; }
        public DbSet<TrustTracker> TrustTrackers { get; set; }
        public DbSet<TrustTrackerStat> TrustTrackerStats { get; set; }
        public DbSet<TrustTrackerUnit> TrustTrackerUnits { get; set; }
        public DbSet<UserAction> UserActions { get; set; }
        public DbSet<UserCheckIn> UserCheckIns { get; set; }
        public DbSet<UserInfo> UserInfoes { get; set; }
        public DbSet<UserInteraction> UserInteractions { get; set; }
        public DbSet<UserInteractionStat> UserInteractionStats { get; set; }
        public DbSet<UserRatingGroup> UserRatingGroups { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<UserRatingsToAdd> UserRatingsToAdds { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UsersAdministrationRightsGroup> UsersAdministrationRightsGroups { get; set; }
        public DbSet<UsersRight> UsersRights { get; set; }
        public DbSet<VolatilityTblRowTracker> VolatilityTblRowTrackers { get; set; }
        public DbSet<VolatilityTracker> VolatilityTrackers { get; set; }
    }
}