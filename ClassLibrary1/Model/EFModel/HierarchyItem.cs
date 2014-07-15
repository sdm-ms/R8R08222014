namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HierarchyItem
    {
        public HierarchyItem()
        {
            HierarchyItems1 = new HashSet<HierarchyItem>();
            HierarchyItems11 = new HashSet<HierarchyItem>();
            SearchWordHierarchyItems = new HashSet<SearchWordHierarchyItem>();
        }

        public int HierarchyItemID { get; set; }

        public int? HigherHierarchyItemID { get; set; }

        public int? HigherHierarchyItemForRoutingID { get; set; }

        public int? TblID { get; set; }

        public string HierarchyItemName { get; set; }

        public string FullHierarchyWithHtml { get; set; }

        public string FullHierarchyNoHtml { get; set; }

        public string RouteToHere { get; set; }

        public bool IncludeInMenu { get; set; }

        public virtual ICollection<HierarchyItem> HierarchyItems1 { get; set; }

        public virtual HierarchyItem HierarchyItem1 { get; set; }

        public virtual ICollection<HierarchyItem> HierarchyItems11 { get; set; }

        public virtual HierarchyItem HierarchyItem2 { get; set; }

        public virtual Tbl Tbl { get; set; }

        public virtual ICollection<SearchWordHierarchyItem> SearchWordHierarchyItems { get; set; }
    }
}
