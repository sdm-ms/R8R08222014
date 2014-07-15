namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TblRow
    {
        public TblRow()
        {
            Comments = new HashSet<Comment>();
            Fields = new HashSet<Field>();
            OverrideCharacteristics = new HashSet<OverrideCharacteristic>();
            RatingGroups = new HashSet<RatingGroup>();
            RewardPendingPointsTrackers = new HashSet<RewardPendingPointsTracker>();
            SearchWordTblRowNames = new HashSet<SearchWordTblRowName>();
            TblRowStatusRecords = new HashSet<TblRowStatusRecord>();
            VolatilityTblRowTrackers = new HashSet<VolatilityTblRowTracker>();
        }

        public int TblRowID { get; set; }

        public int TblID { get; set; }

        public int TblRowFieldDisplayID { get; set; }

        public string Name { get; set; }

        public byte Status { get; set; }

        public bool StatusRecentlyChanged { get; set; }

        public int CountHighStakesNow { get; set; }

        public int CountNonnullEntries { get; set; }

        public decimal CountUserPoints { get; set; }

        public bool ElevateOnMostNeedsRating { get; set; }

        public bool InitialFieldsDisplaySet { get; set; }

        public bool FastAccessInitialCopy { get; set; }

        public bool FastAccessDeleteThenRecopy { get; set; }

        public bool FastAccessUpdateFields { get; set; }

        public bool FastAccessUpdateRatings { get; set; }

        public bool FastAccessUpdateSpecified { get; set; }

        public byte[] FastAccessUpdated { get; set; }

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
