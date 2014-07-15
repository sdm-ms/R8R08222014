namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VolatilityTracker
    {
        public Guid VolatilityTrackerID { get; set; }

        public Guid RatingGroupID { get; set; }

        public Guid VolatilityTblRowTrackerID { get; set; }

        public byte DurationType { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal TotalMovement { get; set; }

        public decimal DistanceFromStart { get; set; }

        public decimal Pushback { get; set; }

        public decimal PushbackProportion { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual VolatilityTblRowTracker VolatilityTblRowTracker { get; set; }
    }
}
