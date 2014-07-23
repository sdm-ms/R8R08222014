namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InsertableContent
    {
        public Guid InsertableContentID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public Guid? DomainID { get; set; }

        public Guid? PointsManagerID { get; set; }

        public Guid? TblID { get; set; }

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
