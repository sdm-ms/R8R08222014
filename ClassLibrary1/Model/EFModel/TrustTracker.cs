namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TrustTracker
    {
        public TrustTracker()
        {
            TrustTrackerStats = new HashSet<TrustTrackerStat>();
        }

        public Guid TrustTrackerID { get; set; }

        public Guid TrustTrackerUnitID { get; set; }

        public Guid UserID { get; set; }

        public double OverallTrustLevel { get; set; }

        public double OverallTrustLevelAtLastReview { get; set; }

        public double DeltaOverallTrustLevel { get; set; }

        public double SkepticalTrustLevel { get; set; }

        public double SumUserInteractionWeights { get; set; }

        public double EgalitarianTrustLevel { get; set; }

        public double Egalitarian_Num { get; set; }

        public double Egalitarian_Denom { get; set; }

        public double? EgalitarianTrustLevelOverride { get; set; }

        public bool MustUpdateUserInteractionEgalitarianTrustLevel { get; set; }

        public virtual TrustTrackerUnit TrustTrackerUnit { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<TrustTrackerStat> TrustTrackerStats { get; set; }
    }
}
