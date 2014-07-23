namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProposalEvaluationRatingSetting
    {
        [Key]
        public Guid ProposalEvaluationRatingSettingsID { get; set; }

        public Guid? PointsManagerID { get; set; }

        public Guid? UserActionID { get; set; }

        public Guid RatingGroupAttributesID { get; set; }

        public decimal MinValueToApprove { get; set; }

        public decimal MaxValueToReject { get; set; }

        public int TimeRequiredBeyondThreshold { get; set; }

        public decimal MinProportionOfThisTime { get; set; }

        public int HalfLifeForResolvingAtFinalValue { get; set; }

        public string Name { get; set; }

        public Guid? Creator { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual UserAction UserAction { get; set; }
    }
}
