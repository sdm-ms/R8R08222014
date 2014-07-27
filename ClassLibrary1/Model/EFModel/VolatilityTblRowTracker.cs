namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VolatilityTblRowTracker
    {
        public VolatilityTblRowTracker()
        {
            VolatilityTrackers = new HashSet<VolatilityTracker>();
        }

        public Guid VolatilityTblRowTrackerID { get; set; }

        public Guid TblRowID { get; set; }

        public byte DurationType { get; set; }

        public decimal TotalMovement { get; set; }

        public decimal DistanceFromStart { get; set; }

        public decimal Pushback { get; set; }

        public decimal PushbackProportion { get; set; }

        public virtual TblRow TblRow { get; set; }

        public virtual ICollection<VolatilityTracker> VolatilityTrackers { get; set; }
    }
}
