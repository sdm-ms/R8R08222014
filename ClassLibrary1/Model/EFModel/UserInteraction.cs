namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserInteraction
    {
        public UserInteraction()
        {
            UserInteractionStats = new HashSet<UserInteractionStat>();
        }

        public Guid UserInteractionID { get; set; }

        public Guid TrustTrackerUnitID { get; set; }

        public Guid OrigRatingUserID { get; set; }

        public Guid LatestRatingUserID { get; set; }

        public int NumTransactions { get; set; }

        public double LatestUserEgalitarianTrust { get; set; }

        public double WeightInCalculatingTrustTotal { get; set; }

        public double? LatestUserEgalitarianTrustAtLastWeightUpdate { get; set; }

        public virtual TrustTrackerUnit TrustTrackerUnit { get; set; }

        public virtual User OriginalRatingUser { get; set; }

        public virtual User LatestRatingUser { get; set; }

        public virtual ICollection<UserInteractionStat> UserInteractionStats { get; set; }
    }
}
