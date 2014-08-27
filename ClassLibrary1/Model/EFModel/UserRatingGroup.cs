namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserRatingGroup
    {
        public UserRatingGroup()
        {
            UserRatings = new HashSet<UserRating>();
        }

        public Guid UserRatingGroupID { get; set; }

        public Guid RatingGroupID { get; set; }

        public Guid RatingGroupPhaseStatusID { get; set; }

        public DateTime WhenCreated { get; set; }

        public virtual RatingGroupPhaseStatus RatingGroupPhaseStatus { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual ICollection<UserRating> UserRatings { get; set; }
    }
}
