namespace DI.Chakra.DBClient.Migrations
{
    using Infrastructure.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class AccessRequestChanges1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_AccessRequest", "ApplicationType", c => c.Byte(nullable: false, defaultValue: (byte)ApplicationType.None));
            DropColumn("dbo.DI_M_AccessRequest", "ProvisioningFormId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AccessRequest", "ProvisioningFormId", c => c.Long());
            DropColumn("dbo.DI_M_AccessRequest", "ApplicationType");
        }
    }
}
