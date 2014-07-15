namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tbl
    {
        public Tbl()
        {
            ChangesGroups = new HashSet<ChangesGroup>();
            FieldDefinitions = new HashSet<FieldDefinition>();
            HierarchyItems = new HashSet<HierarchyItem>();
            InsertableContents = new HashSet<InsertableContent>();
            ProposalSettings = new HashSet<ProposalSetting>();
            TblRows = new HashSet<TblRow>();
            TrustTrackerForChoiceInGroups = new HashSet<TrustTrackerForChoiceInGroup>();
            TblTabs = new HashSet<TblTab>();
        }

        public Guid TblID { get; set; }

        public Guid PointsManagerID { get; set; }

        public Guid? DefaultRatingGroupAttributesID { get; set; }

        [StringLength(50)]
        public string WordToDescribeGroupOfColumnsInThisTbl { get; set; }

        public string Name { get; set; }

        [StringLength(50)]
        public string TypeOfTblRow { get; set; }

        public Guid? TblDimensionsID { get; set; }

        public Guid? Creator { get; set; }

        public byte Status { get; set; }

        public bool AllowOverrideOfRatingGroupCharacterstics { get; set; }

        public bool AllowUsersToAddComments { get; set; }

        public bool LimitCommentsToUsersWhoCanMakeUserRatings { get; set; }

        public bool OneRatingPerRatingGroup { get; set; }

        public string TblRowAdditionCriteria { get; set; }

        public string SuppStylesHeader { get; set; }

        public string SuppStylesMain { get; set; }

        [StringLength(20)]
        public string WidthStyleEntityCol { get; set; }

        [StringLength(20)]
        public string WidthStyleNumCol { get; set; }

        public byte FastTableSyncStatus { get; set; }

        public int NumTblRowsActive { get; set; }

        public int NumTblRowsDeleted { get; set; }

        public virtual ICollection<ChangesGroup> ChangesGroups { get; set; }

        public virtual ICollection<FieldDefinition> FieldDefinitions { get; set; }

        public virtual ICollection<HierarchyItem> HierarchyItems { get; set; }

        public virtual ICollection<InsertableContent> InsertableContents { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual ICollection<ProposalSetting> ProposalSettings { get; set; }

        public virtual TblDimension TblDimension { get; set; }

        public virtual ICollection<TblRow> TblRows { get; set; }

        public virtual ICollection<TrustTrackerForChoiceInGroup> TrustTrackerForChoiceInGroups { get; set; }

        public virtual ICollection<TblTab> TblTabs { get; set; }

        public virtual User User { get; set; }
    }
}
