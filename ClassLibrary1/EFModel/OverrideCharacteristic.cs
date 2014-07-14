namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OverrideCharacteristic
    {
        [Key]
        public int OverrideCharacteristicsID { get; set; }

        public int RatingGroupAttributesID { get; set; }

        public int TblRowID { get; set; }

        public int TblColumnID { get; set; }

        public byte Status { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual TblColumn TblColumn { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
