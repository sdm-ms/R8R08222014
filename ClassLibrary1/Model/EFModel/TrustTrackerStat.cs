namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TrustTrackerStat
    {
        public TrustTrackerStat()
        {
            UserInteractionStats = new HashSet<UserInteractionStat>();
        }

        public Guid TrustTrackerStatID { get; set; }

        public Guid TrustTrackerID { get; set; }

        public short StatNum { get; set; }

        public double TrustValue { get; set; }

        public double Trust_Numer { get; set; }

        public double Trust_Denom { get; set; }

        public double SumUserInteractionStatWeights { get; set; }

        public virtual TrustTracker TrustTracker { get; set; }

        public virtual ICollection<UserInteractionStat> UserInteractionStats { get; set; }
    }
}
