namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingGroupStatusRecord
    {
        public Guid RatingGroupStatusRecordID { get; set; }

        public Guid RatingGroupID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? OldValueOfFirstRating { get; set; }

        public DateTime NewValueTime { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }
    }
}
