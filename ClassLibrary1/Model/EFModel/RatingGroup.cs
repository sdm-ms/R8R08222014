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
            RatingGroupPhaseStatuses = new HashSet<RatingGroupPhaseStatus>();
            RatingGroupResolutions = new HashSet<RatingGroupResolution>();
            RatingGroupStatusRecords = new HashSet<RatingGroupStatusRecord>();
            UserRatingsToAdds = new HashSet<UserRatingsToAdd>();
            VolatilityTrackers = new HashSet<VolatilityTracker>();
            Ratings = new HashSet<Rating>();
            RatingsAboveThisRatingGroupInHierarchy = new HashSet<Rating>();
            RatingsWithinTopRatingGroupHierarchy = new HashSet<Rating>();
            UserRatingGroups = new HashSet<UserRatingGroup>();
        }

        public Guid RatingGroupID { get; set; }

        public Guid RatingGroupAttributesID { get; set; }

        public Guid TblRowID { get; set; }

        public Guid TblColumnID { get; set; }

        public decimal? CurrentValueOfFirstRating { get; set; }

        public bool ValueRecentlyChanged { get; set; }

        public DateTime? ResolutionTime { get; set; }

        public DateTime WhenCreated { get; set; }

        public byte TypeOfRatingGroup { get; set; }

        public byte Status { get; set; }

        public bool HighStakesKnown { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual ICollection<RatingGroupPhaseStatus> RatingGroupPhaseStatuses { get; set; }

        public virtual ICollection<RatingGroupResolution> RatingGroupResolutions { get; set; }

        public virtual ICollection<RatingGroupStatusRecord> RatingGroupStatusRecords { get; set; }

        public virtual ICollection<UserRatingsToAdd> UserRatingsToAdds { get; set; }

        public virtual ICollection<VolatilityTracker> VolatilityTrackers { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Rating> RatingsAboveThisRatingGroupInHierarchy { get; set; }

        public virtual ICollection<Rating> RatingsWithinTopRatingGroupHierarchy { get; set; }

        public virtual ICollection<UserRatingGroup> UserRatingGroups { get; set; }

        public virtual TblColumn TblColumn { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
