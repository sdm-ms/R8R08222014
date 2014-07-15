namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PointsAdjustment
    {
        public int PointsAdjustmentID { get; set; }

        public int UserID { get; set; }

        public int PointsManagerID { get; set; }

        public int Reason { get; set; }

        public decimal TotalAdjustment { get; set; }

        public decimal CurrentAdjustment { get; set; }

        public decimal? CashValue { get; set; }

        public DateTime WhenMade { get; set; }

        public byte Status { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual User User { get; set; }
    }
}
