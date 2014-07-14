namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        public User()
        {
            ChangesGroups = new HashSet<ChangesGroup>();
            ChoiceGroups = new HashSet<ChoiceGroup>();
            Comments = new HashSet<Comment>();
            PointsAdjustments = new HashSet<PointsAdjustment>();
            PointsTotals = new HashSet<PointsTotal>();
            RatingCharacteristics = new HashSet<RatingCharacteristic>();
            RatingGroupResolutions = new HashSet<RatingGroupResolution>();
            RatingPhaseGroups = new HashSet<RatingPhaseGroup>();
            RatingPlans = new HashSet<RatingPlan>();
            Ratings = new HashSet<Rating>();
            RewardPendingPointsTrackers = new HashSet<RewardPendingPointsTracker>();
            SubsidyDensityRangeGroups = new HashSet<SubsidyDensityRangeGroup>();
            Tbls = new HashSet<Tbl>();
            TrustTrackerForChoiceInGroups = new HashSet<TrustTrackerForChoiceInGroup>();
            TrustTrackers = new HashSet<TrustTracker>();
            UserCheckIns = new HashSet<UserCheckIn>();
            UserInfoes = new HashSet<UserInfo>();
            UserInteractions = new HashSet<UserInteraction>();
            UserInteractions1 = new HashSet<UserInteraction>();
            UserRatings = new HashSet<UserRating>();
            UserRatingsToAdds = new HashSet<UserRatingsToAdd>();
            UsersAdministrationRightsGroups = new HashSet<UsersAdministrationRightsGroup>();
            UsersRights = new HashSet<UsersRight>();
        }

        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        public bool SuperUser { get; set; }

        public decimal TrustPointsRatioTotals { get; set; }

        public byte Status { get; set; }

        public virtual ICollection<ChangesGroup> ChangesGroups { get; set; }

        public virtual ICollection<ChoiceGroup> ChoiceGroups { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<PointsAdjustment> PointsAdjustments { get; set; }

        public virtual ICollection<PointsTotal> PointsTotals { get; set; }

        public virtual ICollection<RatingCharacteristic> RatingCharacteristics { get; set; }

        public virtual ICollection<RatingGroupResolution> RatingGroupResolutions { get; set; }

        public virtual ICollection<RatingPhaseGroup> RatingPhaseGroups { get; set; }

        public virtual ICollection<RatingPlan> RatingPlans { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<RewardPendingPointsTracker> RewardPendingPointsTrackers { get; set; }

        public virtual ICollection<SubsidyDensityRangeGroup> SubsidyDensityRangeGroups { get; set; }

        public virtual ICollection<Tbl> Tbls { get; set; }

        public virtual ICollection<TrustTrackerForChoiceInGroup> TrustTrackerForChoiceInGroups { get; set; }

        public virtual ICollection<TrustTracker> TrustTrackers { get; set; }

        public virtual ICollection<UserCheckIn> UserCheckIns { get; set; }

        public virtual ICollection<UserInfo> UserInfoes { get; set; }

        public virtual ICollection<UserInteraction> UserInteractions { get; set; }

        public virtual ICollection<UserInteraction> UserInteractions1 { get; set; }

        public virtual ICollection<UserRating> UserRatings { get; set; }

        public virtual ICollection<UserRatingsToAdd> UserRatingsToAdds { get; set; }

        public virtual ICollection<UsersAdministrationRightsGroup> UsersAdministrationRightsGroups { get; set; }

        public virtual ICollection<UsersRight> UsersRights { get; set; }
    }
}
