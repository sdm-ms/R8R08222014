namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TrustTrackerUnit
    {
        public TrustTrackerUnit()
        {
            PointsManagers = new HashSet<PointsManager>();
            TblColumns = new HashSet<TblColumn>();
            TrustTrackers = new HashSet<TrustTracker>();
            UserInteractions = new HashSet<UserInteraction>();
            UserRatings = new HashSet<UserRating>();
        }

        public Guid TrustTrackerUnitID { get; set; }

        public short SkepticalTrustThreshhold { get; set; }

        public short LastSkepticalTrustThreshhold { get; set; }

        public int MinUpdateIntervalSeconds { get; set; }

        public int MaxUpdateIntervalSeconds { get; set; }

        public decimal ExtendIntervalWhenChangeIsLessThanThis { get; set; }

        public decimal ExtendIntervalMultiplier { get; set; }

        public virtual ICollection<PointsManager> PointsManagers { get; set; }

        public virtual ICollection<TblColumn> TblColumns { get; set; }

        public virtual ICollection<TrustTracker> TrustTrackers { get; set; }

        public virtual ICollection<UserInteraction> UserInteractions { get; set; }

        public virtual ICollection<UserRating> UserRatings { get; set; }
    }
}
