namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingGroupResolution
    {
        public Guid RatingGroupResolutionID { get; set; }

        public Guid RatingGroupID { get; set; }

        public bool CancelPreviousResolutions { get; set; }

        public bool ResolveByUnwinding { get; set; }

        public DateTime EffectiveTime { get; set; }

        public DateTime? ExecutionTime { get; set; }

        public DateTime WhenCreated { get; set; }

        public Guid? Creator { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual RatingGroup RatingGroup { get; set; }

        public virtual User User { get; set; }
    }
}
