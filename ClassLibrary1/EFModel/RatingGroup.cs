namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingGroup
    {
        public RatingGroup()
        {
            RatingGroupPhaseStatus = new HashSet<RatingGroupPhaseStatus>();
            RatingGroupResolutions = new HashSet<RatingGroupResolution>();
            RatingGroupStatusRecords = new HashSet<RatingGroupStatusRecord>();
            UserRatingsToAdds = new HashSet<UserRatingsToAdd>();
            VolatilityTrackers = new HashSet<VolatilityTracker>();
            Ratings = new HashSet<Rating>();
            Ratings1 = new HashSet<Rating>();
            Ratings2 = new HashSet<Rating>();
            UserRatingGroups = new HashSet<UserRatingGroup>();
        }

        public int RatingGroupID { get; set; }

        public int RatingGroupAttributesID { get; set; }

        public int TblRowID { get; set; }

        public int TblColumnID { get; set; }

        public decimal? CurrentValueOfFirstRating { get; set; }

        public bool ValueRecentlyChanged { get; set; }

        public DateTime? ResolutionTime { get; set; }

        public byte TypeOfRatingGroup { get; set; }

        public byte Status { get; set; }

        public bool HighStakesKnown { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual ICollection<RatingGroupPhaseStatus> RatingGroupPhaseStatus { get; set; }

        public virtual ICollection<RatingGroupResolution> RatingGroupResolutions { get; set; }

        public virtual ICollection<RatingGroupStatusRecord> RatingGroupStatusRecords { get; set; }

        public virtual ICollection<UserRatingsToAdd> UserRatingsToAdds { get; set; }

        public virtual ICollection<VolatilityTracker> VolatilityTrackers { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Rating> Ratings1 { get; set; }

        public virtual ICollection<Rating> Ratings2 { get; set; }

        public virtual ICollection<UserRatingGroup> UserRatingGroups { get; set; }

        public virtual TblColumn TblColumn { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
