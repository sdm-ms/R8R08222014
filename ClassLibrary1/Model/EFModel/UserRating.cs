namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserRating
    {
        public UserRating()
        {
            Ratings = new HashSet<Rating>();
            TrustTrackerForChoiceInGroupsUserRatingLinks = new HashSet<TrustTrackerForChoiceInGroupsUserRatingLink>();
            UserRatings1 = new HashSet<UserRating>();
        }

        public int UserRatingID { get; set; }

        public int UserRatingGroupID { get; set; }

        public int RatingID { get; set; }

        public int RatingPhaseStatusID { get; set; }

        public int UserID { get; set; }

        public int? TrustTrackerUnitID { get; set; }

        public int? RewardPendingPointsTrackerID { get; set; }

        public int? MostRecentUserRatingID { get; set; }

        public decimal PreviousRatingOrVirtualRating { get; set; }

        public decimal? PreviousDisplayedRating { get; set; }

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

        public decimal? PointsPumpingProportion { get; set; }

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

        public decimal? LogarithmicBase { get; set; }

        public decimal? HighStakesMultiplierOverride { get; set; }

        public DateTime? WhenPointsBecomePending { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public byte VolatilityTrackingNextTimeFrameToRemove { get; set; }

        public decimal LastWeekDistanceFromStart { get; set; }

        public decimal LastWeekPushback { get; set; }

        public decimal LastYearPushback { get; set; }

        public int UserRatingNumberForUser { get; set; }

        public int? NextRecencyUpdateAtUserRatingNum { get; set; }

        public virtual RatingPhaseStatus RatingPhaseStatus { get; set; }

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
