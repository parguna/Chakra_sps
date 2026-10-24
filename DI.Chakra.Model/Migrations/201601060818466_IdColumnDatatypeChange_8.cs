namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId", "dbo.DI_M_ProvisioningRole");
            DropIndex("dbo.DI_M_ProvisioningRoleDetail", new[] { "ProvisioingRoleId" });
            DropPrimaryKey("dbo.DI_M_AdminModeItem");
            DropPrimaryKey("dbo.DI_M_ProvisioningRoleDetail");
            DropPrimaryKey("dbo.DI_M_ProvisioningRole");
            AlterColumn("dbo.DI_M_AdminModeItem", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_ProvisioningRoleDetail", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_ProvisioningRole", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_AdminModeItem", "Id");
            AddPrimaryKey("dbo.DI_M_ProvisioningRoleDetail", "Id");
            AddPrimaryKey("dbo.DI_M_ProvisioningRole", "Id");
            CreateIndex("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId");
            AddForeignKey("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId", "dbo.DI_M_ProvisioningRole", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId", "dbo.DI_M_ProvisioningRole");
            DropIndex("dbo.DI_M_ProvisioningRoleDetail", new[] { "ProvisioingRoleId" });
            DropPrimaryKey("dbo.DI_M_ProvisioningRole");
            DropPrimaryKey("dbo.DI_M_ProvisioningRoleDetail");
            DropPrimaryKey("dbo.DI_M_AdminModeItem");
            AlterColumn("dbo.DI_M_ProvisioningRole", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_ProvisioningRoleDetail", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AdminModeItem", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_ProvisioningRole", "Id");
            AddPrimaryKey("dbo.DI_M_ProvisioningRoleDetail", "Id");
            AddPrimaryKey("dbo.DI_M_AdminModeItem", "Id");
            CreateIndex("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId");
            AddForeignKey("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId", "dbo.DI_M_ProvisioningRole", "Id", cascadeDelete: true);
        }
    }
}
