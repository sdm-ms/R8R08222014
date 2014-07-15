namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingGroupResolution
    {
        public int RatingGroupResolutionID { get; set; }

        public int RatingGroupID { get; set; }

        public bool CancelPreviousResolutions { get; set; }

        public bool ResolveByUnwinding { get; set; }

        public DateTime EffectiveTime { get; set; }

        public DateTime? ExecutionTime { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual User User { get; set; }
    }
}
