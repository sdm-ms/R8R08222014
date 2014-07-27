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
            ChildHierarchyItems = new HashSet<HierarchyItem>();
            SearchWordHierarchyItems = new HashSet<SearchWordHierarchyItem>();
        }

        public Guid HierarchyItemID { get; set; }

        public Guid? ParentHierarchyItemID { get; set; }

        public Guid? TblID { get; set; }

        public string HierarchyItemName { get; set; }

        public string FullHierarchyWithHtml { get; set; }

        public string FullHierarchyNoHtml { get; set; }

        public string RouteToHere { get; set; }

        public bool IncludeInMenu { get; set; }

        public virtual ICollection<HierarchyItem> ChildHierarchyItems { get; set; }

        public virtual HierarchyItem ParentHierarchyItem { get; set; }

        public virtual Tbl Tbl { get; set; }

        public virtual ICollection<SearchWordHierarchyItem> SearchWordHierarchyItems { get; set; }
    }
}
