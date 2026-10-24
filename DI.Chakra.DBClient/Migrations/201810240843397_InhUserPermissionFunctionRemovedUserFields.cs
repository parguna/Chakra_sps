namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InhUserPermissionFunctionRemovedUserFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_INH_UserPermissionFunction", "FunctionId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_INH_UserPermissionFunction", "PermissionId", c => c.Long(nullable: false));
            DropColumn("dbo.DI_INH_UserPermissionFunction", "EmployeeId");
            DropColumn("dbo.DI_INH_UserPermissionFunction", "NRIC");
            DropColumn("dbo.DI_INH_UserPermissionFunction", "UserName");
            DropColumn("dbo.DI_INH_UserPermissionFunction", "Permission");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_INH_UserPermissionFunction", "Permission", c => c.String());
            AddColumn("dbo.DI_INH_UserPermissionFunction", "UserName", c => c.String());
            AddColumn("dbo.DI_INH_UserPermissionFunction", "NRIC", c => c.String());
            AddColumn("dbo.DI_INH_UserPermissionFunction", "EmployeeId", c => c.String());
            AlterColumn("dbo.DI_INH_UserPermissionFunction", "PermissionId", c => c.String());
            DropColumn("dbo.DI_INH_UserPermissionFunction", "FunctionId");
        }
    }
}
