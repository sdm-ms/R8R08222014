namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TblDimension
    {
        public TblDimension()
        {
            Domains = new HashSet<Domain>();
            Tbls = new HashSet<Tbl>();
        }

        [Key]
        public int TblDimensionsID { get; set; }

        public int MaxWidthOfImageInRowHeaderCell { get; set; }

        public int MaxHeightOfImageInRowHeaderCell { get; set; }

        public int MaxWidthOfImageInTblRowPopUpWindow { get; set; }

        public int MaxHeightOfImageInTblRowPopUpWindow { get; set; }

        public int WidthOfTblRowPopUpWindow { get; set; }

        public int? Creator { get; set; }

        public byte? Status { get; set; }

        public virtual ICollection<Domain> Domains { get; set; }

        public virtual ICollection<Tbl> Tbls { get; set; }
    }
}
