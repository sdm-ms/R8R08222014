namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RewardRatingSetting
    {
        [Key]
        public Guid RewardRatingSettingsID { get; set; }

        public Guid? PointsManagerID { get; set; }

        public Guid? UserActionID { get; set; }

        public Guid RatingGroupAttributesID { get; set; }

        public decimal ProbOfRewardEvaluation { get; set; }

        public decimal? Multiplier { get; set; }

        public string Name { get; set; }

        public Guid? Creator { get; set; }

        public ClassLibrary1.Model.StatusOfObject Status { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual UserAction UserAction { get; set; }
    }
}
