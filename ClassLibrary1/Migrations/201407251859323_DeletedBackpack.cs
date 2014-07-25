namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedBackpack : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MyContainerContents", "MyContainerID", "dbo.MyContainers");
            DropForeignKey("dbo.MyBackpacks", "ContainerInBackpackID", "dbo.MyContainers");
            DropIndex("dbo.MyBackpacks", new[] { "ContainerInBackpackID" });
            DropIndex("dbo.MyContainerContents", new[] { "MyContainerID" });
            DropTable("dbo.MyBackpacks");
            DropTable("dbo.MyContainers");
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
            
            CreateIndex("dbo.MyContainerContents", "MyContainerID");
            CreateIndex("dbo.MyBackpacks", "ContainerInBackpackID");
            AddForeignKey("dbo.MyBackpacks", "ContainerInBackpackID", "dbo.MyContainers", "MyContainerID");
            AddForeignKey("dbo.MyContainerContents", "MyContainerID", "dbo.MyContainers", "MyContainerID");
        }
    }
}
