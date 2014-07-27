namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TblRowStatusRecord")]
    public partial class TblRowStatusRecord
    {
        [Key]
        public Guid TblRowStatusRecordID { get; set; }

        public Guid TblRowId { get; set; }

        public DateTime TimeChanged { get; set; }

        public bool Adding { get; set; }

        public bool Deleting { get; set; }

        public virtual TblRow TblRow { get; set; }
    }
}
