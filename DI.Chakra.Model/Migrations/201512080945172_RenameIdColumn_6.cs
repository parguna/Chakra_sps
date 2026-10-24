namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_6 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_Log_Attestation", "AttestationLogId", "Id");
            RenameColumn("dbo.DI_Log_Discovery", "DiscoveryLogId", "Id");
            RenameColumn("dbo.DI_Log_Email", "EmailLogId", "Id");
            RenameColumn("dbo.DI_Log_Provisioning", "ProvisioningLogId", "Id");
            RenameColumn("dbo.DI_LOG_Scheduler", "SchedulerLogId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_Log_Attestation", "Id", "AttestationLogId");
            RenameColumn("dbo.DI_Log_Discovery", "Id", "DiscoveryLogId");
            RenameColumn("dbo.DI_Log_Email", "Id", "EmailLogId");
            RenameColumn("dbo.DI_Log_Provisioning", "Id", "ProvisioningLogId");
            RenameColumn("dbo.DI_LOG_Scheduler", "Id", "SchedulerLogId");
        }
    }
}
