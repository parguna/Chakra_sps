namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProvConfigChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_ProvisioningForm", "FormExecutorTypeName", c => c.String());
            DropColumn("dbo.DI_M_ProvisioningConfiguration", "FormExecutorTypeName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_ProvisioningConfiguration", "FormExecutorTypeName", c => c.String());
            DropColumn("dbo.DI_M_ProvisioningForm", "FormExecutorTypeName");
        }
    }
}
