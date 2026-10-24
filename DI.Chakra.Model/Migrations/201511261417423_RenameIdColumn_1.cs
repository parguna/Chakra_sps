namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_AccessRightHistory", "AccessRightHistoryId", "Id");
            RenameColumn("dbo.DI_M_AccessRights", "AccessRightsId", "Id");
            RenameColumn("dbo.DI_M_RoleHistory", "RoleHistoryId", "Id"); 
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_AccessRightHistory", "Id", "AccessRightHistoryId");
            RenameColumn("dbo.DI_M_AccessRights", "Id", "AccessRightsId");
            RenameColumn("dbo.DI_M_RoleHistory", "Id", "RoleHistoryId"); 
        }
    }
}
