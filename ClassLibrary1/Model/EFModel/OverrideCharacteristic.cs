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
        public Guid OverrideCharacteristicsID { get; set; }

        public Guid RatingGroupAttributesID { get; set; }

        public Guid TblRowID { get; set; }

        public Guid TblColumnID { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual TblColumn TblColumn { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
