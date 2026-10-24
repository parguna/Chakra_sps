namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_19 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DI_SAPEP_Configuration", name: "SapEpConfigurationId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAPEP_RoleInfoTemp", name: "RoleInfoId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAPEP_UserInfoHistory", name: "UserHistoryId", newName: "Id");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.DI_SAPEP_Configuration", name: "Id", newName: "SapEpConfigurationId");
            RenameColumn(table: "dbo.DI_SAPEP_RoleInfoTemp", name: "Id", newName: "RoleInfoId");
            RenameColumn(table: "dbo.DI_SAPEP_UserInfoHistory", name: "Id", newName: "UserHistoryId");
        }
    }
}
