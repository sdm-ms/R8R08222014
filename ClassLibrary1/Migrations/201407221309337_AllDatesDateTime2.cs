namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllDatesDateTime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AddressFields", "LastGeocode", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ChangesGroup", "ScheduleApprovalOrRejection", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ChangesGroup", "ScheduleImplementation", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ChangesStatusOfObject", "NewValueDateTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PointsManagers", "EndOfDollarSubsidyPeriod", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroups", "ResolutionTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupPhaseStatus", "StartTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupPhaseStatus", "EarliestCompleteTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupPhaseStatus", "ActualCompleteTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupPhaseStatus", "ShortTermResolveTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupPhaseStatus", "HighStakesBecomeKnown", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupPhaseStatus", "HighStakesNoviceUserAfter", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupPhaseStatus", "DeletionTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingPhases", "EndTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Ratings", "LastModifiedResolutionTimeOrCurrentValue", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Ratings", "ReviewRecentUserRatingsAfter", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.UserRatings", "WhenPointsBecomePending", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.UserRatings", "LastModifiedTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RewardPendingPointsTrackers", "TimeOfPendingRating", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Comments", "DateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Comments", "LastDeletedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PointsAdjustments", "WhenMade", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PointsTotals", "FirstUserRating", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PointsTotals", "LastCheckIn", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PointsTotals", "CurrentCheckInPeriodStart", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PointsTotals", "RequestConditionalGuaranteeWhenAvailableTimeRequestMade", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupResolutions", "EffectiveTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupResolutions", "ExecutionTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.UserCheckIns", "CheckInTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TblRowStatusRecord", "TimeChanged", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.VolatilityTrackers", "StartTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.VolatilityTrackers", "EndTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SubsidyAdjustments", "EffectiveTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SubsidyAdjustments", "EndingTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RatingGroupStatusRecords", "NewValueTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DateTimeFields", "DateTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ForumMessages", "CreationDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ForumPersonalMessages", "CreationDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ForumUsers", "RegistrationDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ForumUsers", "LastLogonDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.LongProcesses", "EarliestRestart", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RoleStatus", "LastCheckIn", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.UniquenessLocks", "DeletionTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UniquenessLocks", "DeletionTime", c => c.DateTime());
            AlterColumn("dbo.RoleStatus", "LastCheckIn", c => c.DateTime());
            AlterColumn("dbo.LongProcesses", "EarliestRestart", c => c.DateTime());
            AlterColumn("dbo.ForumUsers", "LastLogonDate", c => c.DateTime());
            AlterColumn("dbo.ForumUsers", "RegistrationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ForumPersonalMessages", "CreationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ForumMessages", "CreationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DateTimeFields", "DateTime", c => c.DateTime());
            AlterColumn("dbo.RatingGroupStatusRecords", "NewValueTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SubsidyAdjustments", "EndingTime", c => c.DateTime());
            AlterColumn("dbo.SubsidyAdjustments", "EffectiveTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.VolatilityTrackers", "EndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.VolatilityTrackers", "StartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TblRowStatusRecord", "TimeChanged", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserCheckIns", "CheckInTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RatingGroupResolutions", "ExecutionTime", c => c.DateTime());
            AlterColumn("dbo.RatingGroupResolutions", "EffectiveTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PointsTotals", "RequestConditionalGuaranteeWhenAvailableTimeRequestMade", c => c.DateTime());
            AlterColumn("dbo.PointsTotals", "CurrentCheckInPeriodStart", c => c.DateTime());
            AlterColumn("dbo.PointsTotals", "LastCheckIn", c => c.DateTime());
            AlterColumn("dbo.PointsTotals", "FirstUserRating", c => c.DateTime());
            AlterColumn("dbo.PointsAdjustments", "WhenMade", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Comments", "LastDeletedDate", c => c.DateTime());
            AlterColumn("dbo.Comments", "DateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RewardPendingPointsTrackers", "TimeOfPendingRating", c => c.DateTime());
            AlterColumn("dbo.UserRatings", "LastModifiedTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserRatings", "WhenPointsBecomePending", c => c.DateTime());
            AlterColumn("dbo.Ratings", "ReviewRecentUserRatingsAfter", c => c.DateTime());
            AlterColumn("dbo.Ratings", "LastModifiedResolutionTimeOrCurrentValue", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RatingPhases", "EndTime", c => c.DateTime());
            AlterColumn("dbo.RatingGroupPhaseStatus", "DeletionTime", c => c.DateTime());
            AlterColumn("dbo.RatingGroupPhaseStatus", "HighStakesNoviceUserAfter", c => c.DateTime());
            AlterColumn("dbo.RatingGroupPhaseStatus", "HighStakesBecomeKnown", c => c.DateTime());
            AlterColumn("dbo.RatingGroupPhaseStatus", "ShortTermResolveTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RatingGroupPhaseStatus", "ActualCompleteTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RatingGroupPhaseStatus", "EarliestCompleteTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RatingGroupPhaseStatus", "StartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RatingGroups", "ResolutionTime", c => c.DateTime());
            AlterColumn("dbo.PointsManagers", "EndOfDollarSubsidyPeriod", c => c.DateTime());
            AlterColumn("dbo.ChangesStatusOfObject", "NewValueDateTime", c => c.DateTime());
            AlterColumn("dbo.ChangesGroup", "ScheduleImplementation", c => c.DateTime());
            AlterColumn("dbo.ChangesGroup", "ScheduleApprovalOrRejection", c => c.DateTime());
            AlterColumn("dbo.AddressFields", "LastGeocode", c => c.DateTime());
        }
    }
}
