namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyBackpacks",
                c => new
                    {
                        MyBackpackID = c.Guid(nullable: false),
                        ContainerInBackpack_MyContainerID = c.Guid(),
                    })
                .PrimaryKey(t => t.MyBackpackID)
                .ForeignKey("dbo.MyContainers", t => t.ContainerInBackpack_MyContainerID)
                .Index(t => t.ContainerInBackpack_MyContainerID);
            
            CreateTable(
                "dbo.MyContainers",
                c => new
                    {
                        MyContainerID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.MyContainerID);
            
            CreateTable(
                "dbo.MyContainerContents",
                c => new
                    {
                        MyContainerContentsID = c.Guid(nullable: false),
                        ContentsString = c.String(),
                        MyContainer_MyContainerID = c.Guid(),
                    })
                .PrimaryKey(t => t.MyContainerContentsID)
                .ForeignKey("dbo.MyContainers", t => t.MyContainer_MyContainerID)
                .Index(t => t.MyContainer_MyContainerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MyBackpacks", "ContainerInBackpack_MyContainerID", "dbo.MyContainers");
            DropForeignKey("dbo.MyContainerContents", "MyContainer_MyContainerID", "dbo.MyContainers");
            DropIndex("dbo.MyContainerContents", new[] { "MyContainer_MyContainerID" });
            DropIndex("dbo.MyBackpacks", new[] { "ContainerInBackpack_MyContainerID" });
            DropTable("dbo.MyContainerContents");
            DropTable("dbo.MyContainers");
            DropTable("dbo.MyBackpacks");
        }
    }
}
