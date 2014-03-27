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
    
    public partial class TblRow
    {
        public TblRow()
        {
            this.Comments = new HashSet<Comment>();
            this.Fields = new HashSet<Field>();
            this.OverrideCharacteristics = new HashSet<OverrideCharacteristic>();
            this.RatingGroups = new HashSet<RatingGroup>();
            this.RewardPendingPointsTrackers = new HashSet<RewardPendingPointsTracker>();
            this.SearchWordTblRowNames = new HashSet<SearchWordTblRowName>();
            this.TblRowStatusRecords = new HashSet<TblRowStatusRecord>();
            this.VolatilityTblRowTrackers = new HashSet<VolatilityTblRowTracker>();
        }
    
        public int TblRowID { get; set; }
        public int TblID { get; set; }
        public int TblRowFieldDisplayID { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public bool StatusRecentlyChanged { get; set; }
        public int CountHighStakesNow { get; set; }
        public int CountNullEntries { get; set; }
        public decimal CountUserPoints { get; set; }
        public bool ElevateOnMostNeedsRating { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Field> Fields { get; set; }
        public virtual ICollection<OverrideCharacteristic> OverrideCharacteristics { get; set; }
        public virtual ICollection<RatingGroup> RatingGroups { get; set; }
        public virtual ICollection<RewardPendingPointsTracker> RewardPendingPointsTrackers { get; set; }
        public virtual ICollection<SearchWordTblRowName> SearchWordTblRowNames { get; set; }
        public virtual TblRowFieldDisplay TblRowFieldDisplay { get; set; }
        public virtual ICollection<TblRowStatusRecord> TblRowStatusRecords { get; set; }
        public virtual ICollection<VolatilityTblRowTracker> VolatilityTblRowTrackers { get; set; }
        public virtual Tbl Tbl { get; set; }
    }
}