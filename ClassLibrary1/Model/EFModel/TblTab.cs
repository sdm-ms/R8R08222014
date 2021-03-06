namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TblTab
    {
        public TblTab()
        {
            TblColumns = new HashSet<TblColumn>();
        }

        public Guid TblTabID { get; set; }

        public Guid TblID { get; set; }

        public int NumInTbl { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public Guid? DefaultSortTblColumnID { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<TblColumn> TblColumns { get; set; }

        public virtual Tbl Tbl { get; set; }
    }
}
