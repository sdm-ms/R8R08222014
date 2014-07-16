namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PointsManager
    {
        public PointsManager()
        {
            AdministrationRightsGroups = new HashSet<AdministrationRightsGroup>();
            ChangesGroups = new HashSet<ChangesGroup>();
            ChoiceGroups = new HashSet<ChoiceGroup>();
            InsertableContents = new HashSet<InsertableContent>();
            PointsAdjustments = new HashSet<PointsAdjustment>();
            PointsTotals = new HashSet<PointsTotal>();
            ProposalEvaluationRatingSettings = new HashSet<ProposalEvaluationRatingSetting>();
            ProposalSettings = new HashSet<ProposalSetting>();
            RatingGroupAttributes = new HashSet<RatingGroupAttribute>();
            RewardRatingSettings = new HashSet<RewardRatingSetting>();
            Tbls = new HashSet<Tbl>();
            UsersAdministrationRightsGroups = new HashSet<UsersAdministrationRightsGroup>();
            UsersRights = new HashSet<UsersRight>();
        }

        public Guid PointsManagerID { get; set; }

        public Guid DomainID { get; set; }

        public Guid? TrustTrackerUnitID { get; set; }

        public decimal CurrentPeriodDollarSubsidy { get; set; }

        public DateTime? EndOfDollarSubsidyPeriod { get; set; }

        public decimal? NextPeriodDollarSubsidy { get; set; }

        public int? NextPeriodLength { get; set; }

        public short NumPrizes { get; set; }

        public decimal? MinimumPayment { get; set; }

        public decimal TotalUserPoints { get; set; }

        public decimal CurrentUserPoints { get; set; }

        public decimal CurrentUserPendingPoints { get; set; }

        public decimal CurrentUserNotYetPendingPoints { get; set; }

        public decimal CurrentPointsToCount { get; set; }

        public int NumUsersMeetingUltimateStandard { get; set; }

        public int NumUsersMeetingCurrentStandard { get; set; }

        public decimal HighStakesProbability { get; set; }

        public decimal HighStakesSecretMultiplier { get; set; }

        public decimal? HighStakesKnownMultiplier { get; set; }

        public bool HighStakesNoviceOn { get; set; }

        public int HighStakesNoviceNumAutomatic { get; set; }

        public int HighStakesNoviceNumOneThird { get; set; }

        public int HighStakesNoviceNumOneTenth { get; set; }

        public decimal DatabaseChangeSelectHighStakesNoviceNumPct { get; set; }

        public int HighStakesNoviceNumActive { get; set; }

        public int HighStakesNoviceTargetNum { get; set; }

        public decimal DollarValuePerPoint { get; set; }

        public decimal DiscountForGuarantees { get; set; }

        public decimal MaximumTotalGuarantees { get; set; }

        public decimal MaximumGuaranteePaymentPerHour { get; set; }

        public decimal TotalUnconditionalGuaranteesEarnedEver { get; set; }

        public decimal TotalConditionalGuaranteesEarnedEver { get; set; }

        public decimal TotalConditionalGuaranteesPending { get; set; }

        public bool AllowApplicationsWhenNoConditionalGuaranteesAvailable { get; set; }

        public bool ConditionalGuaranteesAvailableForNewUsers { get; set; }

        public bool ConditionalGuaranteesAvailableForExistingUsers { get; set; }

        public int ConditionalGuaranteeTimeBlockInHours { get; set; }

        public int ConditionalGuaranteeApplicationsReceived { get; set; }

        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<AdministrationRightsGroup> AdministrationRightsGroups { get; set; }

        public virtual ICollection<ChangesGroup> ChangesGroups { get; set; }

        public virtual ICollection<ChoiceGroup> ChoiceGroups { get; set; }

        public virtual Domain Domain { get; set; }

        public virtual ICollection<InsertableContent> InsertableContents { get; set; }

        public virtual ICollection<PointsAdjustment> PointsAdjustments { get; set; }

        public virtual TrustTrackerUnit TrustTrackerUnit { get; set; }

        public virtual ICollection<PointsTotal> PointsTotals { get; set; }

        public virtual ICollection<ProposalEvaluationRatingSetting> ProposalEvaluationRatingSettings { get; set; }

        public virtual ICollection<ProposalSetting> ProposalSettings { get; set; }

        public virtual ICollection<RatingGroupAttribute> RatingGroupAttributes { get; set; }

        public virtual ICollection<RewardRatingSetting> RewardRatingSettings { get; set; }

        public virtual ICollection<Tbl> Tbls { get; set; }

        public virtual ICollection<UsersAdministrationRightsGroup> UsersAdministrationRightsGroups { get; set; }

        public virtual ICollection<UsersRight> UsersRights { get; set; }
    }
}
