namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeyOnBackpackModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MyBackpacks", "ContainerInBackpackID");
            RenameColumn(table: "dbo.MyBackpacks", name: "ContainerInBackpack_MyContainerID", newName: "ContainerInBackpackID");
            RenameIndex(table: "dbo.MyBackpacks", name: "IX_ContainerInBackpack_MyContainerID", newName: "IX_ContainerInBackpackID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.MyBackpacks", name: "IX_ContainerInBackpackID", newName: "IX_ContainerInBackpack_MyContainerID");
            RenameColumn(table: "dbo.MyBackpacks", name: "ContainerInBackpackID", newName: "ContainerInBackpack_MyContainerID");
            AddColumn("dbo.MyBackpacks", "ContainerInBackpackID", c => c.Guid());
        }
    }
}
