namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProvisioningLogChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_Log_Provisioning", "AccessRequestId", c => c.Long());
            AddColumn("dbo.DI_Log_Provisioning", "EndpointSystemTypeId", c => c.Long());
            AddColumn("dbo.DI_Log_Provisioning", "EndpointSystemTypeName", c => c.String());
            AddColumn("dbo.DI_Log_Provisioning", "EndpointSystemId", c => c.Long());
            DropColumn("dbo.DI_Log_Provisioning", "RequestId");
            DropColumn("dbo.DI_Log_Provisioning", "RequestUserId");
            DropColumn("dbo.DI_Log_Provisioning", "RequestDetailId");
            DropColumn("dbo.DI_Log_Provisioning", "ReferenceType");
            DropColumn("dbo.DI_Log_Provisioning", "Reference");
            DropColumn("dbo.DI_Log_Provisioning", "CreatedDate");
            DropColumn("dbo.DI_Log_Provisioning", "LastModifiedBy");
            DropColumn("dbo.DI_Log_Provisioning", "LastModifiedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_Log_Provisioning", "LastModifiedDate", c => c.DateTime());
            AddColumn("dbo.DI_Log_Provisioning", "LastModifiedBy", c => c.String());
            AddColumn("dbo.DI_Log_Provisioning", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DI_Log_Provisioning", "Reference", c => c.String());
            AddColumn("dbo.DI_Log_Provisioning", "ReferenceType", c => c.Int(nullable: false));
            AddColumn("dbo.DI_Log_Provisioning", "RequestDetailId", c => c.Long());
            AddColumn("dbo.DI_Log_Provisioning", "RequestUserId", c => c.Long());
            AddColumn("dbo.DI_Log_Provisioning", "RequestId", c => c.Long());
            DropColumn("dbo.DI_Log_Provisioning", "EndpointSystemId");
            DropColumn("dbo.DI_Log_Provisioning", "EndpointSystemTypeName");
            DropColumn("dbo.DI_Log_Provisioning", "EndpointSystemTypeId");
            DropColumn("dbo.DI_Log_Provisioning", "AccessRequestId");
        }
    }
}
