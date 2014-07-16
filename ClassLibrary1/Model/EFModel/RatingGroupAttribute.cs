namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RatingGroupAttribute
    {
        public RatingGroupAttribute()
        {
            OverrideCharacteristics = new HashSet<OverrideCharacteristic>();
            ProposalEvaluationRatingSettings = new HashSet<ProposalEvaluationRatingSetting>();
            RatingGroups = new HashSet<RatingGroup>();
            RatingPlans = new HashSet<RatingPlan>();
            RatingPlans1 = new HashSet<RatingPlan>();
            RewardRatingSettings = new HashSet<RewardRatingSetting>();
            TblColumns = new HashSet<TblColumn>();
        }

        [Key]
        public Guid RatingGroupAttributesID { get; set; }

        public Guid RatingCharacteristicsID { get; set; }

        public Guid? RatingConditionID { get; set; }

        public Guid? PointsManagerID { get; set; }

        public decimal? ConstrainedSum { get; set; }

        public string Name { get; set; }

        public byte? TypeOfRatingGroup { get; set; }

        public string Description { get; set; }

        public bool RatingEndingTimeVaries { get; set; }

        public bool RatingsCanBeAutocalculated { get; set; }

        public decimal LongTermPointsWeight { get; set; }

        public int MinimumDaysToTrackLongTerm { get; set; }

        public int? Creator { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<OverrideCharacteristic> OverrideCharacteristics { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual ICollection<ProposalEvaluationRatingSetting> ProposalEvaluationRatingSettings { get; set; }

        public virtual RatingCharacteristic RatingCharacteristic { get; set; }

        public virtual RatingCondition RatingCondition { get; set; }

        public virtual ICollection<RatingGroup> RatingGroups { get; set; }

        public virtual ICollection<RatingPlan> RatingPlans { get; set; }

        public virtual ICollection<RatingPlan> RatingPlans1 { get; set; }

        public virtual ICollection<RewardRatingSetting> RewardRatingSettings { get; set; }

        public virtual ICollection<TblColumn> TblColumns { get; set; }
    }
}
