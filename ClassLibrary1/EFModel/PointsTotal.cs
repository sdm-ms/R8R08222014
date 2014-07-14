namespace ClassLibrary1.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PointsTotal
    {
        public int PointsTotalID { get; set; }

        public int UserID { get; set; }

        public int PointsManagerID { get; set; }

        public decimal CurrentPoints { get; set; }

        public decimal TotalPoints { get; set; }

        public decimal PotentialMaxLossOnNotYetPending { get; set; }

        public decimal PendingPoints { get; set; }

        public decimal NotYetPendingPoints { get; set; }

        public decimal TrustPoints { get; set; }

        public decimal TrustPointsRatio { get; set; }

        public int NumPendingOrFinalizedRatings { get; set; }

        public decimal PointsPerRating { get; set; }

        public DateTime? FirstUserRating { get; set; }

        public DateTime? LastCheckIn { get; set; }

        public DateTime? CurrentCheckInPeriodStart { get; set; }

        public decimal TotalTimeThisCheckInPeriod { get; set; }

        public decimal TotalTimeThisRewardPeriod { get; set; }

        public decimal TotalTimeEver { get; set; }

        public decimal? PointsPerHour { get; set; }

        public decimal? ProjectedPointsPerHour { get; set; }

        public decimal GuaranteedPaymentsEarnedThisRewardPeriod { get; set; }

        [StringLength(50)]
        public string PendingConditionalGuaranteeApplication { get; set; }

        public decimal? PendingConditionalGuaranteePayment { get; set; }

        public decimal? PendingConditionalGuaranteeTotalHoursAtStart { get; set; }

        public decimal? PendingConditionalGuaranteeTotalHoursNeeded { get; set; }

        public decimal? PendingConditionalGuaranteePaymentAlreadyMade { get; set; }

        public DateTime? RequestConditionalGuaranteeWhenAvailableTimeRequestMade { get; set; }

        public decimal TotalPointsOrPendingPointsLongTermUnweighted { get; set; }

        public decimal PointsPerRatingLongTerm { get; set; }

        public float PointsPumpingProportionAvg_Numer { get; set; }

        public float PointsPumpingProportionAvg_Denom { get; set; }

        public float PointsPumpingProportionAvg { get; set; }

        public int NumUserRatings { get; set; }

        public virtual PointsManager PointsManager { get; set; }

        public virtual User User { get; set; }
    }
}
