namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsersAdministrationRightsGroup
    {
        public Guid UsersAdministrationRightsGroupID { get; set; }

        public Guid? UserID { get; set; }

        public Guid PointsManagerID { get; set; }

        public Guid AdministrationRightsGroupID { get; set; }

        public byte Status { get; set; }

        public virtual AdministrationRightsGroup AdministrationRightsGroup { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual User User { get; set; }
    }
}
