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
    
    public partial class UserRating
    {
        public UserRating()
        {
            this.Ratings = new HashSet<Rating>();
            this.TrustTrackerForChoiceInGroupsUserRatingLinks = new HashSet<TrustTrackerForChoiceInGroupsUserRatingLink>();
            this.UserRatings1 = new HashSet<UserRating>();
        }
    
        public int UserRatingID { get; set; }
        public int UserRatingGroupID { get; set; }
        public int RatingID { get; set; }
        public int RatingPhaseStatusID { get; set; }
        public int UserID { get; set; }
        public Nullable<int> TrustTrackerUnitID { get; set; }
        public Nullable<int> RewardPendingPointsTrackerID { get; set; }
        public Nullable<int> MostRecentUserRatingID { get; set; }
        public decimal PreviousRatingOrVirtualRating { get; set; }
        public Nullable<decimal> PreviousDisplayedRating { get; set; }
        public decimal EnteredUserRating { get; set; }
        public decimal NewUserRating { get; set; }
        public decimal OriginalAdjustmentPct { get; set; }
        public decimal OriginalTrustLevel { get; set; }
        public decimal MaxGain { get; set; }
        public decimal MaxLoss { get; set; }
        public decimal PotentialPointsShortTerm { get; set; }
        public decimal PotentialPointsLongTerm { get; set; }
        public decimal PotentialPointsLongTermUnweighted { get; set; }
        public decimal LongTermPointsWeight { get; set; }
        public Nullable<decimal> PointsPumpingProportion { get; set; }
        public decimal PastPointsPumpingProportion { get; set; }
        public decimal PercentPreviousRatings { get; set; }
        public bool IsTrusted { get; set; }
        public bool MadeDirectly { get; set; }
        public bool LongTermResolutionReflected { get; set; }
        public bool ShortTermResolutionReflected { get; set; }
        public bool PointsHaveBecomePending { get; set; }
        public bool ForceRecalculate { get; set; }
        public bool HighStakesPreviouslySecret { get; set; }
        public bool HighStakesKnown { get; set; }
        public bool PreviouslyRated { get; set; }
        public bool SubsequentlyRated { get; set; }
        public bool IsMostRecent10Pct { get; set; }
        public bool IsMostRecent30Pct { get; set; }
        public bool IsMostRecent70Pct { get; set; }
        public bool IsMostRecent90Pct { get; set; }
        public bool IsUsersFirstWeek { get; set; }
        public Nullable<decimal> LogarithmicBase { get; set; }
        public Nullable<decimal> HighStakesMultiplierOverride { get; set; }
        public Nullable<System.DateTime> WhenPointsBecomePending { get; set; }
        public System.DateTime LastModifiedTime { get; set; }
        public byte VolatilityTrackingNextTimeFrameToRemove { get; set; }
        public decimal LastWeekDistanceFromStart { get; set; }
        public decimal LastWeekPushback { get; set; }
        public decimal LastYearPushback { get; set; }
        public int UserRatingNumberForUser { get; set; }
        public Nullable<int> NextRecencyUpdateAtUserRatingNum { get; set; }
    
        public virtual RatingPhaseStatu RatingPhaseStatu { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual Rating Rating { get; set; }
        public virtual RewardPendingPointsTracker RewardPendingPointsTracker { get; set; }
        public virtual ICollection<TrustTrackerForChoiceInGroupsUserRatingLink> TrustTrackerForChoiceInGroupsUserRatingLinks { get; set; }
        public virtual TrustTrackerUnit TrustTrackerUnit { get; set; }
        public virtual UserRatingGroup UserRatingGroup { get; set; }
        public virtual ICollection<UserRating> UserRatings1 { get; set; }
        public virtual UserRating UserRating1 { get; set; }
        public virtual User User { get; set; }
    }
}
