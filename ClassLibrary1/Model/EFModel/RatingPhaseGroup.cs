namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingPhaseGroup
    {
        public RatingPhaseGroup()
        {
            RatingCharacteristics = new HashSet<RatingCharacteristic>();
            RatingGroupPhaseStatuses = new HashSet<RatingGroupPhaseStatus>();
            RatingPhases = new HashSet<RatingPhase>();
        }

        public Guid RatingPhaseGroupID { get; set; }

        public int NumPhases { get; set; }

        public string Name { get; set; }

        public Guid? Creator { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual ICollection<RatingCharacteristic> RatingCharacteristics { get; set; }

        public virtual ICollection<RatingGroupPhaseStatus> RatingGroupPhaseStatuses { get; set; }

        public virtual ICollection<RatingPhase> RatingPhases { get; set; }

        public virtual User User { get; set; }
    }
}
