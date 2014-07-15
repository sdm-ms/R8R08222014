namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RoleStatus
    {
        [Key]
        public int RoleStatusID { get; set; }

        public string RoleID { get; set; }

        public DateTime? LastCheckIn { get; set; }

        public bool IsWorkerRole { get; set; }

        public bool IsBackgroundProcessing { get; set; }
    }
}
