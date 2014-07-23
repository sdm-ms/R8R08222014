namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdministrationRight
    {
        public Guid AdministrationRightID { get; set; }

        public Guid AdministrationRightsGroupID { get; set; }

        public Guid? UserActionID { get; set; }

        public bool AllowUserToMakeImmediateChanges { get; set; }

        public bool AllowUserToMakeProposals { get; set; }

        public bool AllowUserToSeekRewards { get; set; }

        public bool AllowUserNotToSeekRewards { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual AdministrationRightsGroup AdministrationRightsGroup { get; set; }

        public virtual UserAction UserAction { get; set; }
    }
}
