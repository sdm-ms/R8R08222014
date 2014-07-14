namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserRatingsToAdd")]
    public partial class UserRatingsToAdd
    {
        public int UserRatingsToAddID { get; set; }

        public int UserID { get; set; }

        public int TopRatingGroupID { get; set; }

        [Required]
        public byte[] UserRatingHierarchy { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual User User { get; set; }
    }
}
