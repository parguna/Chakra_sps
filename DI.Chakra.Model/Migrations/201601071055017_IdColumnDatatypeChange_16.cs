namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_16 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_DefaultProvisioningFormGeneralData", "EndpointSystemTypeId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_DefaultProvisioningFormGeneralData", "EndpointSystemTypeId", c => c.Int(nullable: false));
        }
    }
}
