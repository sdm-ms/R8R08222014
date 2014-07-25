namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class partialmigrate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MyContainerContents", "MyContainerID", "dbo.MyContainers");
            DropIndex("dbo.MyContainerContents", new[] { "MyContainerID" });
            DropTable("dbo.MyContainerContents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MyContainerContents",
                c => new
                    {
                        MyContainerContentsID = c.Guid(nullable: false),
                        ContentsString = c.String(),
                        MyContainerID = c.Guid(),
                    })
                .PrimaryKey(t => t.MyContainerContentsID);
            
            CreateIndex("dbo.MyContainerContents", "MyContainerID");
            AddForeignKey("dbo.MyContainerContents", "MyContainerID", "dbo.MyContainers", "MyContainerID");
        }
    }
}
