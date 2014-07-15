namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingCharacteristic
    {
        public RatingCharacteristic()
        {
            Ratings = new HashSet<Rating>();
            RatingGroupAttributes = new HashSet<RatingGroupAttribute>();
        }

        [Key]
        public int RatingCharacteristicsID { get; set; }

        public int RatingPhaseGroupID { get; set; }

        public int? SubsidyDensityRangeGroupID { get; set; }

        public decimal MinimumUserRating { get; set; }

        public decimal MaximumUserRating { get; set; }

        public byte DecimalPlaces { get; set; }

        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<RatingGroupAttribute> RatingGroupAttributes { get; set; }

        public virtual RatingPhaseGroup RatingPhaseGroup { get; set; }

        public virtual SubsidyDensityRangeGroup SubsidyDensityRangeGroup { get; set; }

        public virtual User User { get; set; }
    }
}
