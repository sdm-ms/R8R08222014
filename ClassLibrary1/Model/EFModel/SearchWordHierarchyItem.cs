namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchWordHierarchyItem
    {
        public Guid SearchWordHierarchyItemID { get; set; }

        public Guid HierarchyItemID { get; set; }

        public Guid SearchWordID { get; set; }

        public virtual HierarchyItem HierarchyItem { get; set; }

        public virtual SearchWord SearchWord { get; set; }
    }
}
