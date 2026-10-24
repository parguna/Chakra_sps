namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_AccessRequest", "AccessRequestId", "Id");
            RenameColumn("dbo.DI_M_AccessRequestHistory", "AccessRequestHistoryId", "Id");
            RenameColumn("dbo.DI_M_AccountReconcilation", "AccountReconcilationId", "Id");
            RenameColumn("dbo.DI_M_EmployeeEventTrigger", "EmployeeEventTriggerId", "Id");
            RenameColumn("dbo.DI_M_EntitlementReconcilation", "EntitlementReconcilationId", "Id");
            RenameColumn("dbo.DI_M_OUChangeSnapshot", "SnapshotId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_AccessRequest", "Id", "AccessRequestId");
            RenameColumn("dbo.DI_M_AccessRequestHistory", "Id", "AccessRequestHistoryId");
            RenameColumn("dbo.AccountReconcilation", "Id", "AccountReconcilationId");
            RenameColumn("dbo.EmployeeEventTrigger", "Id", "EmployeeEventTriggerId");
            RenameColumn("dbo.DI_M_EntitlementReconcilation", "Id", "EntitlementReconcilationId");
            RenameColumn("dbo.DI_M_OUChangeSnapshot", "Id", "SnapshotId");
        }
    }
}
