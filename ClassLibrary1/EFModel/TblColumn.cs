namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TblColumn
    {
        public TblColumn()
        {
            OverrideCharacteristics = new HashSet<OverrideCharacteristic>();
            RatingGroups = new HashSet<RatingGroup>();
            TblColumnFormattings = new HashSet<TblColumnFormatting>();
        }

        public int TblColumnID { get; set; }

        public int TblTabID { get; set; }

        public int DefaultRatingGroupAttributesID { get; set; }

        public int? ConditionTblColumnID { get; set; }

        public int? TrustTrackerUnitID { get; set; }

        public decimal? ConditionGreaterThan { get; set; }

        public decimal? ConditionLessThan { get; set; }

        public int CategoryNum { get; set; }

        [Required]
        [StringLength(20)]
        public string Abbreviation { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Explanation { get; set; }

        [Required]
        [StringLength(20)]
        public string WidthStyle { get; set; }

        public int NumNonNull { get; set; }

        public double ProportionNonNull { get; set; }

        public bool UsingNonSparseColumn { get; set; }

        public bool ShouldUseNonSparseColumn { get; set; }

        public bool UseAsFilter { get; set; }

        public bool Sortable { get; set; }

        public bool DefaultSortOrderAscending { get; set; }

        public bool AutomaticallyCreateMissingRatings { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<OverrideCharacteristic> OverrideCharacteristics { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual ICollection<RatingGroup> RatingGroups { get; set; }

        public virtual ICollection<TblColumnFormatting> TblColumnFormattings { get; set; }

        public virtual TblColumn TblColumns1 { get; set; }

        public virtual TblColumn TblColumn1 { get; set; }

        public virtual TrustTrackerUnit TrustTrackerUnit { get; set; }

        public virtual TblTab TblTab { get; set; }
    }
}
