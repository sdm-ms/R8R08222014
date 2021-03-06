namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Rating
    {
        public Rating()
        {
            ChangesGroups = new HashSet<ChangesGroup>();
            RatingConditions = new HashSet<RatingCondition>();
            RatingPhaseStatus = new HashSet<RatingPhaseStatus>();
            UserRatings = new HashSet<UserRating>();
        }

        public Guid RatingID { get; set; }

        public Guid RatingGroupID { get; set; }

        public Guid RatingCharacteristicsID { get; set; }

        public Guid? OwnedRatingGroupID { get; set; }

        public Guid TopmostRatingGroupID { get; set; }

        public Guid? MostRecentUserRatingID { get; set; }

        public int NumInGroup { get; set; }

        public int TotalUserRatings { get; set; }

        public string Name { get; set; }

        public Guid? Creator { get; set; }

        public decimal? CurrentValue { get; set; }

        public decimal? LastTrustedValue { get; set; }

        public DateTime LastModifiedResolutionTimeOrCurrentValue { get; set; }

        public DateTime? ReviewRecentUserRatingsAfter { get; set; }

        public virtual ICollection<ChangesGroup> ChangesGroups { get; set; }

        public virtual RatingCharacteristic RatingCharacteristic { get; set; }

        public virtual ICollection<RatingCondition> RatingConditions { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual RatingGroup OwnedRatingGroup { get; set; }

        public virtual RatingGroup TopRatingGroup { get; set; }

        public virtual ICollection<RatingPhaseStatus> RatingPhaseStatus { get; set; }

        public virtual UserRating UserRating { get; set; }

        public virtual ICollection<UserRating> UserRatings { get; set; }

        public virtual User User { get; set; }
    }
}
