namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EliminatedRoutingHierarchyItems : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HierarchyItems", "HigherHierarchyItemForRoutingID", "dbo.HierarchyItems");
            DropIndex("dbo.HierarchyItems", new[] { "HigherHierarchyItemForRoutingID" });
            DropColumn("dbo.HierarchyItems", "HigherHierarchyItemForRoutingID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HierarchyItems", "HigherHierarchyItemForRoutingID", c => c.Guid());
            CreateIndex("dbo.HierarchyItems", "HigherHierarchyItemForRoutingID");
            AddForeignKey("dbo.HierarchyItems", "HigherHierarchyItemForRoutingID", "dbo.HierarchyItems", "HierarchyItemID");
        }
    }
}
