namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProposalSetting
    {
        [Key]
        public Guid ProposalSettingsID { get; set; }

        public Guid? PointsManagerID { get; set; }

        public Guid? TblID { get; set; }

        public bool UsersMayProposeAddingTbls { get; set; }

        public bool UsersMayProposeResolvingRatings { get; set; }

        public bool UsersMayProposeChangingTblRows { get; set; }

        public bool UsersMayProposeChangingChoiceGroups { get; set; }

        public bool UsersMayProposeChangingCharacteristics { get; set; }

        public bool UsersMayProposeChangingColumns { get; set; }

        public bool UsersMayProposeChangingUsersRights { get; set; }

        public bool UsersMayProposeAdjustingPoints { get; set; }

        public bool UsersMayProposeChangingProposalSettings { get; set; }

        public decimal MinValueToApprove { get; set; }

        public decimal MaxValueToReject { get; set; }

        public int MinTimePastThreshold { get; set; }

        public decimal MinProportionOfThisTime { get; set; }

        public int MinAdditionalTimeForRewardRating { get; set; }

        public int HalfLifeForRewardRating { get; set; }

        public decimal MaxBonusForProposal { get; set; }

        public decimal MaxPenaltyForRejection { get; set; }

        public decimal SubsidyForApprovalRating { get; set; }

        public decimal SubsidyForRewardRating { get; set; }

        public int HalfLifeForResolvingAtFinalValue { get; set; }

        public decimal RequiredPointsToMakeProposal { get; set; }

        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual Tbl Tbl { get; set; }
    }
}
