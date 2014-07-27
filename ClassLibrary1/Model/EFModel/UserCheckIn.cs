namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserCheckIn
    {
        public Guid UserCheckInID { get; set; }

        public DateTime CheckInTime { get; set; }

        public Guid? UserID { get; set; }

        public virtual User User { get; set; }
    }
}
