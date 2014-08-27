namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserWhenCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "WhenCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "WhenCreated");
        }
    }
}
