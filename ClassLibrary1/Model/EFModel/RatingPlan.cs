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
        public Guid RatingPlansID { get; set; }

        public Guid RatingGroupAttributesID { get; set; }

        public int NumInGroup { get; set; }

        public Guid? OwnedRatingGroupAttributesID { get; set; }

        public decimal? DefaultUserRating { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? Creator { get; set; }

        public byte Status { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute1 { get; set; }

        public virtual User User { get; set; }
    }
}
