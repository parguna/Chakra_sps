namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProvisioningFormIdRename : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_ProvisioningForm", "ProvisioningFormId", "Id");
        }

        public override void Down()
        {
            RenameColumn("dbo.DI_M_ProvisioningForm", "Id", "ProvisioningFormId");
        }
    }
}
