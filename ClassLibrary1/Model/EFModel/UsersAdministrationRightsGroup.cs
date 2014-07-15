namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsersAdministrationRightsGroup
    {
        public int UsersAdministrationRightsGroupID { get; set; }

        public int? UserID { get; set; }

        public int PointsManagerID { get; set; }

        public int AdministrationRightsGroupID { get; set; }

        public byte Status { get; set; }

        public virtual AdministrationRightsGroup AdministrationRightsGroup { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual User User { get; set; }
    }
}
