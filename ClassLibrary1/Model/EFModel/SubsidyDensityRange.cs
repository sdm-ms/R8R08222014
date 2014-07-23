namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SubsidyDensityRange
    {
        [Key]
        public Guid SubsidyDensityRangeID { get; set; }

        public Guid SubsidyDensityRangeGroupID { get; set; }

        public decimal RangeBottom { get; set; }

        public decimal RangeTop { get; set; }

        public decimal LiquidityFactor { get; set; }

        public decimal CumDensityBottom { get; set; }

        public decimal CumDensityTop { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual SubsidyDensityRangeGroup SubsidyDensityRangeGroup { get; set; }
    }
}
