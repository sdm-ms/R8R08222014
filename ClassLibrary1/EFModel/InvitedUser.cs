namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InvitedUser")]
    public partial class InvitedUser
    {
        [Key]
        public int ActivationNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailId { get; set; }

        public bool MayView { get; set; }

        public bool MayPredict { get; set; }

        public bool MayAddTbls { get; set; }

        public bool MayResolveRatings { get; set; }

        public bool MayChangeTblRows { get; set; }

        public bool MayChangeChoiceGroups { get; set; }

        public bool MayChangeCharacteristics { get; set; }

        public bool MayChangeColumns { get; set; }

        public bool MayChangeUsersRights { get; set; }

        public bool MayAdjustPoints { get; set; }

        public bool MayChangeProposalSettings { get; set; }

        public bool IsRegistered { get; set; }
    }
}
