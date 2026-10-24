namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INHousePermissionModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_INH_Permission", "ADGroups", c => c.String());
            AddColumn("dbo.DI_INH_Permission", "EPGroups", c => c.String());
            AddColumn("dbo.DI_INH_Permission", "EPRoles", c => c.String());
            AddColumn("dbo.DI_INH_Permission", "CEGroups", c => c.String());
            AddColumn("dbo.DI_INH_Permission", "CERoles", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_INH_Permission", "CERoles");
            DropColumn("dbo.DI_INH_Permission", "CEGroups");
            DropColumn("dbo.DI_INH_Permission", "EPRoles");
            DropColumn("dbo.DI_INH_Permission", "EPGroups");
            DropColumn("dbo.DI_INH_Permission", "ADGroups");
        }
    }
}
