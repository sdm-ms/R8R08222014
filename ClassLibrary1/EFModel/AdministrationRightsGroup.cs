namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdministrationRightsGroup
    {
        public AdministrationRightsGroup()
        {
            AdministrationRights = new HashSet<AdministrationRight>();
            UsersAdministrationRightsGroups = new HashSet<UsersAdministrationRightsGroup>();
        }

        public int AdministrationRightsGroupID { get; set; }

        public int? PointsManagerID { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<AdministrationRight> AdministrationRights { get; set; }

        public virtual ICollection<UsersAdministrationRightsGroup> UsersAdministrationRightsGroups { get; set; }

        public virtual PointsManager PointsManager { get; set; }
    }
}
