namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdministrationRight
    {
        public int AdministrationRightID { get; set; }

        public int AdministrationRightsGroupID { get; set; }

        public int? UserActionID { get; set; }

        public bool AllowUserToMakeImmediateChanges { get; set; }

        public bool AllowUserToMakeProposals { get; set; }

        public bool AllowUserToSeekRewards { get; set; }

        public bool AllowUserNotToSeekRewards { get; set; }

        public byte Status { get; set; }

        public virtual AdministrationRightsGroup AdministrationRightsGroup { get; set; }

        public virtual UserAction UserAction { get; set; }
    }
}
