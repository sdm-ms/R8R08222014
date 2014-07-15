namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SubsidyDensityRangeGroup
    {
        public SubsidyDensityRangeGroup()
        {
            RatingCharacteristics = new HashSet<RatingCharacteristic>();
            SubsidyDensityRanges = new HashSet<SubsidyDensityRange>();
        }

        public int SubsidyDensityRangeGroupID { get; set; }

        public decimal? UseLogarithmBase { get; set; }

        public decimal CumDensityTotal { get; set; }

        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<RatingCharacteristic> RatingCharacteristics { get; set; }

        public virtual ICollection<SubsidyDensityRange> SubsidyDensityRanges { get; set; }

        public virtual User User { get; set; }
    }
}
