namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RatingPhases")]
    public partial class RatingPhase
    {
        public RatingPhase()
        {
            RatingGroupPhaseStatus = new HashSet<RatingGroupPhaseStatus>();
        }

        [Key]
        public int RatingPhaseID { get; set; }

        public int RatingPhaseGroupID { get; set; }

        public int NumberInGroup { get; set; }

        public decimal SubsidyLevel { get; set; }

        public short ScoringRule { get; set; }

        public bool Timed { get; set; }

        public bool BaseTimingOnSpecificTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? RunTime { get; set; }

        public int HalfLifeForResolution { get; set; }

        public bool RepeatIndefinitely { get; set; }

        public int? RepeatNTimes { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<RatingGroupPhaseStatus> RatingGroupPhaseStatus { get; set; }

        public virtual RatingPhaseGroup RatingPhaseGroup { get; set; }
    }
}
