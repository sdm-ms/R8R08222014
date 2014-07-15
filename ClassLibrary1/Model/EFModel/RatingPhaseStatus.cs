namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingPhaseStatus
    {
        public RatingPhaseStatus()
        {
            UserRatings = new HashSet<UserRating>();
        }

        [Key]
        public Guid RatingPhaseStatusID { get; set; }

        public Guid RatingGroupPhaseStatusID { get; set; }

        public Guid RatingID { get; set; }

        public decimal? ShortTermResolutionValue { get; set; }

        public int NumUserRatingsMadeDuringPhase { get; set; }

        public bool TriggerUserRatingsUpdate { get; set; }

        public virtual RatingGroupPhaseStatus RatingGroupPhaseStatus { get; set; }

        public virtual Rating Rating { get; set; }

        public virtual ICollection<UserRating> UserRatings { get; set; }
    }
}
