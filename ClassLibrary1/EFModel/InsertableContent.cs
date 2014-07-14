namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InsertableContent
    {
        public int InsertableContentID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int? DomainID { get; set; }

        public int? PointsManagerID { get; set; }

        public int? TblID { get; set; }

        [Required]
        public string Content { get; set; }

        public bool IsTextOnly { get; set; }

        public bool Overridable { get; set; }

        public short Location { get; set; }

        public byte Status { get; set; }

        public virtual Domain Domain { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual Tbl Tbl { get; set; }
    }
}
