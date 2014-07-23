namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SubsidyAdjustment
    {
        public Guid SubsidyAdjustmentID { get; set; }

        public Guid RatingGroupPhaseStatusID { get; set; }

        public decimal SubsidyAdjustmentFactor { get; set; }

        public DateTime EffectiveTime { get; set; }

        public DateTime? EndingTime { get; set; }

        public int? EndingTimeHalfLife { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual RatingGroupPhaseStatus RatingGroupPhaseStatus { get; set; }
    }
}
