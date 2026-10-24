namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_5 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_ConfigurationMaster", "ConfigId", "Id");
            RenameColumn("dbo.DI_M_ConfigurationMasterHistory", "ConfigHistoryId", "Id");
            RenameColumn("dbo.DI_M_PasswordPolicy", "PolicyId", "Id");
            RenameColumn("dbo.DI_M_PasswordSyncConfiguration", "ConfigId", "Id");
            RenameColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigDetailId", "Id");
            RenameColumn("dbo.DI_M_RequestApprovalConfiguration", "ConfigId", "Id");

            RenameColumn("dbo.DI_M_DiscoveryProcess", "DiscoveryProcessId", "Id");
            RenameColumn("dbo.DI_M_DiscoveryProcessHistory", "DiscoveryProcessHistoryId", "Id");

            RenameColumn("dbo.DI_M_MailServerConfiguration", "MailServerId", "Id");
            RenameColumn("dbo.DI_M_MailTemplate", "TemplateId", "Id");
            RenameColumn("dbo.DI_M_MailTemplateVariable", "VariableId", "Id");

            RenameColumn("dbo.DI_M_AttributeMapping", "Rowid", "Id");
            RenameColumn("dbo.DI_M_EndpointSystem", "EndpointSystemId", "Id");
            RenameColumn("dbo.DI_M_EndpointFunctionType", "EndpointFunctionTypeId", "Id");
            RenameColumn("dbo.DI_M_EndpointObjects", "EndpointObjectId", "Id");
            RenameColumn("dbo.DI_M_EndpointSystemType", "EndpointSystemTypeId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_ConfigurationMaster", "Id", "ConfigId");
            RenameColumn("dbo.DI_M_ConfigurationMasterHistory", "Id", "ConfigHistoryId");
            RenameColumn("dbo.DI_M_PasswordPolicy", "Id", "PolicyId");
            RenameColumn("dbo.DI_M_PasswordSyncConfiguration", "Id", "ConfigId");
            RenameColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "Id", "ConfigDetailId");
            RenameColumn("dbo.DI_M_RequestApprovalConfiguration", "Id", "ConfigId");

            RenameColumn("dbo.DI_M_DiscoveryProcess", "Id", "DiscoveryProcessId");
            RenameColumn("dbo.DI_M_DiscoveryProcessHistory", "Id", "DiscoveryProcessHistoryId");

            RenameColumn("dbo.DI_M_MailServerConfiguration", "Id", "MailServerId");
            RenameColumn("dbo.DI_M_MailTemplate", "Id", "TemplateId");
            RenameColumn("dbo.DI_M_MailTemplateVariable", "Id", "VariableId");

            RenameColumn("dbo.DI_M_AttributeMapping", "Id", "Rowid");
            RenameColumn("dbo.DI_M_EndpointSystem", "Id", "EndpointSystemId");
            RenameColumn("dbo.DI_M_EndpointFunctionType", "Id", "EndpointFunctionTypeId");
            RenameColumn("dbo.DI_M_EndpointObjects", "Id", "EndpointObjectId");
            RenameColumn("dbo.DI_M_EndpointSystemType", "Id", "EndpointSystemTypeId");
        }
    }
}
