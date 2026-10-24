namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W_4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_ProvisioningRoleDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvisioingRoleId = c.Int(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        Entitlements = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningRole", t => t.ProvisioingRoleId, cascadeDelete: true)
                .Index(t => t.ProvisioingRoleId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_ProvisioningRoleDetail", "ProvisioingRoleId", "dbo.DI_M_ProvisioningRole");
            DropIndex("dbo.DI_M_ProvisioningRoleDetail", new[] { "ProvisioingRoleId" });
            DropTable("dbo.DI_M_ProvisioningRole");
            DropTable("dbo.DI_M_ProvisioningRoleDetail");
        }
    }
}
