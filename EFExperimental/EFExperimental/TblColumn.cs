//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFExperimental
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblColumn
    {
        public TblColumn()
        {
            this.OverrideCharacteristics = new HashSet<OverrideCharacteristic>();
            this.RatingGroups = new HashSet<RatingGroup>();
            this.TblColumnFormattings = new HashSet<TblColumnFormatting>();
        }
    
        public int TblColumnID { get; set; }
        public int TblTabID { get; set; }
        public int DefaultRatingGroupAttributesID { get; set; }
        public Nullable<int> ConditionTblColumnID { get; set; }
        public Nullable<int> TrustTrackerUnitID { get; set; }
        public Nullable<decimal> ConditionGreaterThan { get; set; }
        public Nullable<decimal> ConditionLessThan { get; set; }
        public int CategoryNum { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string Explanation { get; set; }
        public string WidthStyle { get; set; }
        public bool UseAsFilter { get; set; }
        public bool Sortable { get; set; }
        public bool DefaultSortOrderAscending { get; set; }
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
