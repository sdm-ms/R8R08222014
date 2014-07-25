namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BackpackExampleChange : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MyContainerContents", name: "MyContainer_MyContainerID", newName: "MyContainerID");
            RenameIndex(table: "dbo.MyContainerContents", name: "IX_MyContainer_MyContainerID", newName: "IX_MyContainerID");
            AddColumn("dbo.MyBackpacks", "ContainerInBackpackID", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MyBackpacks", "ContainerInBackpackID");
            RenameIndex(table: "dbo.MyContainerContents", name: "IX_MyContainerID", newName: "IX_MyContainer_MyContainerID");
            RenameColumn(table: "dbo.MyContainerContents", name: "MyContainerID", newName: "MyContainer_MyContainerID");
        }
    }
}
