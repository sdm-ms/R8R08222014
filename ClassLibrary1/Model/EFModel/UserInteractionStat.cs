namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserInteractionStat
    {
        public int UserInteractionStatID { get; set; }

        public int UserInteractionID { get; set; }

        public int TrustTrackerStatID { get; set; }

        public short StatNum { get; set; }

        public double SumAdjustPctTimesWeight { get; set; }

        public double SumWeights { get; set; }

        public double AvgAdjustmentPctWeighted { get; set; }

        public virtual TrustTrackerStat TrustTrackerStat { get; set; }

        public virtual UserInteraction UserInteraction { get; set; }
    }
}
