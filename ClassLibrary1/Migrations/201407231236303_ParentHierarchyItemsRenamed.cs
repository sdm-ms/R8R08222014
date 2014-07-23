namespace ClassLibrary1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParentHierarchyItemsRenamed : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.HierarchyItems", name: "HigherHierarchyItemID", newName: "ParentHierarchyItemID");
            RenameIndex(table: "dbo.HierarchyItems", name: "IX_HigherHierarchyItemID", newName: "IX_ParentHierarchyItemID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.HierarchyItems", name: "IX_ParentHierarchyItemID", newName: "IX_HigherHierarchyItemID");
            RenameColumn(table: "dbo.HierarchyItems", name: "ParentHierarchyItemID", newName: "HigherHierarchyItemID");
        }
    }
}
