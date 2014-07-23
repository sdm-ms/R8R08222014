namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingCondition
    {
        public RatingCondition()
        {
            RatingGroupAttributes = new HashSet<RatingGroupAttribute>();
        }

        public Guid RatingConditionID { get; set; }

        public Guid? ConditionRatingID { get; set; }

        public decimal? GreaterThan { get; set; }

        public decimal? LessThan { get; set; }

        public byte Status { get; set; }

        public virtual Rating Rating { get; set; }

        public virtual ICollection<RatingGroupAttribute> RatingGroupAttributes { get; set; }
    }
}
