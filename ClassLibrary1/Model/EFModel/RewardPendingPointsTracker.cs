namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RewardPendingPointsTracker
    {
        public RewardPendingPointsTracker()
        {
            UserRatings = new HashSet<UserRating>();
        }

        public Guid RewardPendingPointsTrackerID { get; set; }

        public decimal? PendingRating { get; set; }

        public DateTime? TimeOfPendingRating { get; set; }

        public Guid RewardTblRowID { get; set; }

        public Guid UserID { get; set; }

        public virtual User User { get; set; }

        public virtual TblRow TblRow { get; set; }

        public virtual ICollection<UserRating> UserRatings { get; set; }
    }
}
