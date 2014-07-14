namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserCheckIn
    {
        public int UserCheckInID { get; set; }

        public DateTime CheckInTime { get; set; }

        public int? UserID { get; set; }

        public virtual User User { get; set; }
    }
}
