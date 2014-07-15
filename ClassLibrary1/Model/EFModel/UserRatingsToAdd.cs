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
        public Guid UserRatingsToAddID { get; set; }

        public Guid UserID { get; set; }

        public Guid TopRatingGroupID { get; set; }

        public byte[] UserRatingHierarchy { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual User User { get; set; }
    }
}
