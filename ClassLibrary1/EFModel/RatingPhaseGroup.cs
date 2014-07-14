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
            RatingGroupPhaseStatus = new HashSet<RatingGroupPhaseStatus>();
            RatingPhases = new HashSet<RatingPhase>();
        }

        public int RatingPhaseGroupID { get; set; }

        public int NumPhases { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<RatingCharacteristic> RatingCharacteristics { get; set; }

        public virtual ICollection<RatingGroupPhaseStatus> RatingGroupPhaseStatus { get; set; }

        public virtual ICollection<RatingPhase> RatingPhases { get; set; }

        public virtual User User { get; set; }
    }
}
