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
        public int RewardRatingSettingsID { get; set; }

        public int? PointsManagerID { get; set; }

        public int? UserActionID { get; set; }

        public int RatingGroupAttributesID { get; set; }

        public decimal ProbOfRewardEvaluation { get; set; }

        public decimal? Multiplier { get; set; }

        public string Name { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual RatingGroupAttribute RatingGroupAttribute { get; set; }

        public virtual UserAction UserAction { get; set; }
    }
}
