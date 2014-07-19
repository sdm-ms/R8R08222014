namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingGroupPhaseStatus
    {
        public RatingGroupPhaseStatus()
        {
            RatingPhaseStatus = new HashSet<RatingPhaseStatus>();
            UserRatingGroups = new HashSet<UserRatingGroup>();
            SubsidyAdjustments = new HashSet<SubsidyAdjustment>();
        }

        [Key]
        public Guid RatingGroupPhaseStatusID { get; set; }

        public Guid RatingPhaseGroupID { get; set; }

        public Guid RatingPhaseID { get; set; }

        public Guid RatingGroupID { get; set; }

        public int RoundNum { get; set; }

        public int RoundNumThisPhase { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EarliestCompleteTime { get; set; }

        public DateTime ActualCompleteTime { get; set; }

        public DateTime ShortTermResolveTime { get; set; }

        public DateTime WhenCreated { get; set; }

        public bool HighStakesSecret { get; set; }

        public bool HighStakesKnown { get; set; }

        public bool HighStakesReflected { get; set; }

        public bool HighStakesNoviceUser { get; set; }

        public DateTime? HighStakesBecomeKnown { get; set; }

        public DateTime? HighStakesNoviceUserAfter { get; set; }

        public DateTime? DeletionTime { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual ICollection<RatingPhaseStatus> RatingPhaseStatus { get; set; }

        public virtual ICollection<UserRatingGroup> UserRatingGroups { get; set; }

        public virtual ICollection<SubsidyAdjustment> SubsidyAdjustments { get; set; }

        public virtual RatingPhase RatingPhase { get; set; }

        public virtual RatingPhaseGroup RatingPhaseGroup { get; set; }
    }
}
