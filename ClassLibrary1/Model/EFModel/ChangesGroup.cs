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

        public Guid ChangesGroupID { get; set; }

        public Guid? PointsManagerID { get; set; }

        public Guid? TblID { get; set; }

        public int? Creator { get; set; }

        public Guid? MakeChangeRatingID { get; set; }

        public Guid? RewardRatingID { get; set; }

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
