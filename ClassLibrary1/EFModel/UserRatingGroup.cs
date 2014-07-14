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

        public int UserRatingGroupID { get; set; }

        public int RatingGroupID { get; set; }

        public int RatingGroupPhaseStatusID { get; set; }

        public DateTime WhenMade { get; set; }

        public virtual RatingGroupPhaseStatus RatingGroupPhaseStatus { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual ICollection<UserRating> UserRatings { get; set; }
    }
}
