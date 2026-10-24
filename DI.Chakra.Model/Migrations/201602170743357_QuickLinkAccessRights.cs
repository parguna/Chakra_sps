namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuickLinkAccessRights : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DI_M_QckLinkAccessRights", newName: "DI_M_QuickLinkAccessRights");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DI_M_QuickLinkAccessRights", newName: "DI_M_QckLinkAccessRights");
        }
    }
}
