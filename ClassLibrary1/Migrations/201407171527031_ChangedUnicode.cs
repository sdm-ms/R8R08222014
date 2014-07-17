namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedUnicode : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tbls", "WordToDescribeGroupOfColumnsInThisTbl", c => c.String(maxLength: 50));
            AlterColumn("dbo.Tbls", "TypeOfTblRow", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserActions", "Text", c => c.String());
            AlterColumn("dbo.RatingGroupAttributes", "Description", c => c.String());
            AlterColumn("dbo.Comments", "CommentTitle", c => c.String(nullable: false));
            AlterColumn("dbo.Comments", "CommentText", c => c.String(nullable: false));
            AlterColumn("dbo.RatingPlans", "Description", c => c.String());
            AlterColumn("dbo.UserInfo", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "Email", c => c.String(maxLength: 250));
            AlterColumn("dbo.UserInfo", "Address1", c => c.String(maxLength: 200));
            AlterColumn("dbo.UserInfo", "Address2", c => c.String(maxLength: 200));
            AlterColumn("dbo.UserInfo", "WorkPhone", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "HomePhone", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "MobilePhone", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "ZipCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "City", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "State", c => c.String(maxLength: 50));
            AlterColumn("dbo.UserInfo", "Country", c => c.String(maxLength: 50));
            AlterColumn("dbo.InsertableContents", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.InsertableContents", "Content", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InsertableContents", "Content", c => c.String(unicode: false));
            AlterColumn("dbo.InsertableContents", "Name", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "Country", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "State", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "City", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "ZipCode", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "MobilePhone", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "HomePhone", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "WorkPhone", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "Address2", c => c.String(maxLength: 200, unicode: false));
            AlterColumn("dbo.UserInfo", "Address1", c => c.String(maxLength: 200, unicode: false));
            AlterColumn("dbo.UserInfo", "Email", c => c.String(maxLength: 250, unicode: false));
            AlterColumn("dbo.UserInfo", "LastName", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.UserInfo", "FirstName", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.RatingPlans", "Description", c => c.String(unicode: false));
            AlterColumn("dbo.Comments", "CommentText", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Comments", "CommentTitle", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.RatingGroupAttributes", "Description", c => c.String(unicode: false));
            AlterColumn("dbo.UserActions", "Text", c => c.String(unicode: false));
            AlterColumn("dbo.Tbls", "TypeOfTblRow", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.Tbls", "WordToDescribeGroupOfColumnsInThisTbl", c => c.String(maxLength: 50, unicode: false));
        }
    }
}
