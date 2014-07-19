namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedWhenCreatedFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RatingGroups", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.RatingGroupPhaseStatus", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TblRows", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.RatingGroupResolutions", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.UserRatingsToAdd", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.UserRatingGroups", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.RoleStatus", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Ratings", "CreationTime");
            DropColumn("dbo.UserRatingGroups", "WhenMade");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserRatingGroups", "WhenMade", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ratings", "CreationTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.RoleStatus", "WhenCreated");
            DropColumn("dbo.UserRatingGroups", "WhenCreated");
            DropColumn("dbo.UserRatingsToAdd", "WhenCreated");
            DropColumn("dbo.RatingGroupResolutions", "WhenCreated");
            DropColumn("dbo.TblRows", "WhenCreated");
            DropColumn("dbo.RatingGroupPhaseStatus", "WhenCreated");
            DropColumn("dbo.RatingGroups", "WhenCreated");
        }
    }
}
