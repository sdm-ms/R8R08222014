namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TblColumnFormatting")]
    public partial class TblColumnFormatting
    {
        public int TblColumnFormattingID { get; set; }

        public int TblColumnID { get; set; }

        [Required]
        [StringLength(10)]
        public string Prefix { get; set; }

        [Required]
        [StringLength(10)]
        public string Suffix { get; set; }

        public bool OmitLeadingZero { get; set; }

        public decimal? ExtraDecimalPlaceAbove { get; set; }

        public decimal? ExtraDecimalPlace2Above { get; set; }

        public decimal? ExtraDecimalPlace3Above { get; set; }

        [Required]
        public string SuppStylesHeader { get; set; }

        [Required]
        public string SuppStylesMain { get; set; }

        public byte Status { get; set; }

        public virtual TblColumn TblColumn { get; set; }
    }
}
