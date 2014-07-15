namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TrustTrackerForChoiceInGroup
    {
        public TrustTrackerForChoiceInGroup()
        {
            TrustTrackerForChoiceInGroupsUserRatingLinks = new HashSet<TrustTrackerForChoiceInGroupsUserRatingLink>();
        }

        public Guid TrustTrackerForChoiceInGroupID { get; set; }

        public Guid UserID { get; set; }

        public Guid ChoiceInGroupID { get; set; }

        public Guid TblID { get; set; }

        public float SumAdjustmentPctTimesRatingMagnitude { get; set; }

        public float SumRatingMagnitudes { get; set; }

        public float TrustLevelForChoice { get; set; }

        public virtual ChoiceInGroup ChoiceInGroup { get; set; }

        public virtual Tbl Tbl { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<TrustTrackerForChoiceInGroupsUserRatingLink> TrustTrackerForChoiceInGroupsUserRatingLinks { get; set; }
    }
}
