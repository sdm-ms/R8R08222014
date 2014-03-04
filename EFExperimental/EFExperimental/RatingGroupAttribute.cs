//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFExperimental
{
    using System;
    using System.Collections.Generic;
    
    public partial class RatingGroupAttribute
    {
        public RatingGroupAttribute()
        {
            this.OverrideCharacteristics = new HashSet<OverrideCharacteristic>();
            this.ProposalEvaluationRatingSettings = new HashSet<ProposalEvaluationRatingSetting>();
            this.RatingGroups = new HashSet<RatingGroup>();
            this.RatingPlans = new HashSet<RatingPlan>();
            this.RatingPlans1 = new HashSet<RatingPlan>();
            this.RewardRatingSettings = new HashSet<RewardRatingSetting>();
            this.TblColumns = new HashSet<TblColumn>();
        }
    
        public int RatingGroupAttributesID { get; set; }
        public int RatingCharacteristicsID { get; set; }
        public Nullable<int> RatingConditionID { get; set; }
        public Nullable<int> PointsManagerID { get; set; }
        public Nullable<decimal> ConstrainedSum { get; set; }
        public string Name { get; set; }
        public Nullable<byte> TypeOfRatingGroup { get; set; }
        public string Description { get; set; }
        public bool RatingEndingTimeVaries { get; set; }
        public bool RatingsCanBeAutocalculated { get; set; }
        public decimal LongTermPointsWeight { get; set; }
        public int MinimumDaysToTrackLongTerm { get; set; }
        public Nullable<int> Creator { get; set; }
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
