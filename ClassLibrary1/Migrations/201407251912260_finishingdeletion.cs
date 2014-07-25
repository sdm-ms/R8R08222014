namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finishingdeletion : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MyBackpacks", "ContainerInBackpackID", "dbo.MyContainers");
            DropIndex("dbo.MyBackpacks", new[] { "ContainerInBackpackID" });
            DropTable("dbo.MyBackpacks");
            DropTable("dbo.MyContainers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MyContainers",
                c => new
                    {
                        MyContainerID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.MyContainerID);
            
            CreateTable(
                "dbo.MyBackpacks",
                c => new
                    {
                        MyBackpackID = c.Guid(nullable: false),
                        ContainerInBackpackID = c.Guid(),
                    })
                .PrimaryKey(t => t.MyBackpackID);
            
            CreateIndex("dbo.MyBackpacks", "ContainerInBackpackID");
            AddForeignKey("dbo.MyBackpacks", "ContainerInBackpackID", "dbo.MyContainers", "MyContainerID");
        }
    }
}
