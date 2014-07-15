namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChangesStatusOfObject")]
    public partial class ChangesStatusOfObject
    {
        public int ChangesStatusOfObjectID { get; set; }

        public int ChangesGroupID { get; set; }

        public byte ObjectType { get; set; }

        public bool AddObject { get; set; }

        public bool DeleteObject { get; set; }

        public bool ReplaceObject { get; set; }

        public bool ChangeName { get; set; }

        public bool ChangeOther { get; set; }

        public bool ChangeSetting1 { get; set; }

        public bool ChangeSetting2 { get; set; }

        public bool MayAffectRunningRating { get; set; }

        [StringLength(50)]
        public string NewName { get; set; }

        public int? NewObject { get; set; }

        public int? ExistingObject { get; set; }

        public bool? NewValueBoolean { get; set; }

        public int? NewValueInteger { get; set; }

        public decimal? NewValueDecimal { get; set; }

        public string NewValueText { get; set; }

        public DateTime? NewValueDateTime { get; set; }

        public string ChangeDescription { get; set; }

        public virtual ChangesGroup ChangesGroup { get; set; }
    }
}
