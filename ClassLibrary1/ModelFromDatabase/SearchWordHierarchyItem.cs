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
    
    public partial class SearchWordHierarchyItem
    {
        public int SearchWordHierarchyItemID { get; set; }
        public int HierarchyItemID { get; set; }
        public int SearchWordID { get; set; }
    
        public virtual HierarchyItem HierarchyItem { get; set; }
        public virtual SearchWord SearchWord { get; set; }
    }
}
