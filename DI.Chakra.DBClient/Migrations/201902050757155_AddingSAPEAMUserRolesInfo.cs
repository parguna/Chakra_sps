namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSAPEAMUserRolesInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "Company", c => c.String());
            AddColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "Section", c => c.String());
            AddColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "DepartmentBranch", c => c.String());
            AddColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "NovaPositionMatrixType", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "NovaPositionMatrixType");
            DropColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "DepartmentBranch");
            DropColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "Section");
            DropColumn("dbo.DI_SAP_EamNovaUserRoleInfo", "Company");
        }
    }
}
