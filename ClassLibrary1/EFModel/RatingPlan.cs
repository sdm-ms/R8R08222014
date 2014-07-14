namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingPlan
    {
        [Key]
        public int RatingPlansID { get; set; }

        public int RatingGroupAttributesID { get; set; }

        public int NumInGroup { get; set; }

        public int? OwnedRatingGroupAttributesID { get; set; }

        public decimal? DefaultUserRating { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute1 { get; set; }

        public virtual User User { get; set; }
    }
}
