namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProvConfiguration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_ProvisioningConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserNameSource = c.Byte(nullable: false),
                        UserNameGeneratorId = c.Int(nullable: false),
                        FormExecutorTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id", c => c.Int());
            CreateIndex("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id");
            AddForeignKey("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id", "dbo.DI_M_ProvisioningConfiguration", "Id");
            DropColumn("dbo.DI_M_ProvisioningForm", "FormExecutorTypeName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_ProvisioningForm", "FormExecutorTypeName", c => c.String());
            DropForeignKey("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id", "dbo.DI_M_ProvisioningConfiguration");
            DropIndex("dbo.DI_M_ProvisioningForm", new[] { "ProvisioningConfiguration_Id" });
            DropColumn("dbo.DI_M_ProvisioningForm", "ProvisioningConfiguration_Id");
            DropTable("dbo.DI_M_ProvisioningConfiguration");
        }
    }
}
