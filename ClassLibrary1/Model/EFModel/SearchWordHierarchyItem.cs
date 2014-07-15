namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWordHierarchyItem
    {
        public int SearchWordHierarchyItemID { get; set; }

        public int HierarchyItemID { get; set; }

        public int SearchWordID { get; set; }

        public virtual HierarchyItem HierarchyItem { get; set; }

        public virtual SearchWord SearchWord { get; set; }
    }
}
