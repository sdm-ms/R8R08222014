//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClassLibrary1.ModelFromDatabase
{
    using System;
    using System.Collections.Generic;
    
    public partial class HierarchyItem
    {
        public HierarchyItem()
        {
            this.HierarchyItems1 = new HashSet<HierarchyItem>();
            this.HierarchyItems11 = new HashSet<HierarchyItem>();
            this.SearchWordHierarchyItems = new HashSet<SearchWordHierarchyItem>();
        }
    
        public int HierarchyItemID { get; set; }
        public Nullable<int> HigherHierarchyItemID { get; set; }
        public Nullable<int> HigherHierarchyItemForRoutingID { get; set; }
        public Nullable<int> TblID { get; set; }
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
