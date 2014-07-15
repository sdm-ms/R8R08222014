namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsersRight
    {
        [Key]
        public int UsersRightsID { get; set; }

        public int? UserID { get; set; }

        public int? PointsManagerID { get; set; }

        public bool MayView { get; set; }

        public bool MayPredict { get; set; }

        public bool MayAddTbls { get; set; }

        public bool MayResolveRatings { get; set; }

        public bool MayChangeTblRows { get; set; }

        public bool MayChangeChoiceGroups { get; set; }

        public bool MayChangeCharacteristics { get; set; }

        public bool MayChangeCategories { get; set; }

        public bool MayChangeUsersRights { get; set; }

        public bool MayAdjustPoints { get; set; }

        public bool MayChangeProposalSettings { get; set; }

        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual User User { get; set; }
    }
}
