namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChangesGroup")]
    public partial class ChangesGroup
    {
        public ChangesGroup()
        {
            ChangesStatusOfObjects = new HashSet<ChangesStatusOfObject>();
        }

        public int ChangesGroupID { get; set; }

        public int? PointsManagerID { get; set; }

        public int? TblID { get; set; }

        public int? Creator { get; set; }

        public int? MakeChangeRatingID { get; set; }

        public int? RewardRatingID { get; set; }

        public byte StatusOfChanges { get; set; }

        public DateTime? ScheduleApprovalOrRejection { get; set; }

        public DateTime? ScheduleImplementation { get; set; }

        public virtual ICollection<ChangesStatusOfObject> ChangesStatusOfObjects { get; set; }

        public virtual Rating Rating { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual Tbl Tbl { get; set; }

        public virtual User User { get; set; }
    }
}
