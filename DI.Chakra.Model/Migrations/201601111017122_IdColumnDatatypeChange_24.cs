namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_24 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_AccessRequestProgress");
            DropPrimaryKey("dbo.DI_M_AccessRequestSource");
            DropPrimaryKey("dbo.DI_M_AccessRequestStatus");
            DropPrimaryKey("dbo.DI_Log_UI");
            AlterColumn("dbo.DI_M_AccessRequestHistory", "RequestStatus", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestHistory", "ProgressStatus", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestHistory", "RequestSource", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestProgress", "Id", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequest", "RequestStatus", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequest", "ProgressStatus", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequest", "RequestSource", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestSource", "Id", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestStatus", "Id", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_RequestApprovalConfiguration", "RequestSource", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_Log_UI", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_AccessRequestProgress", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRequestSource", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRequestStatus", "Id");
            AddPrimaryKey("dbo.DI_Log_UI", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_Log_UI");
            DropPrimaryKey("dbo.DI_M_AccessRequestStatus");
            DropPrimaryKey("dbo.DI_M_AccessRequestSource");
            DropPrimaryKey("dbo.DI_M_AccessRequestProgress");
            AlterColumn("dbo.DI_Log_UI", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_RequestApprovalConfiguration", "RequestSource", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestStatus", "Id", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestSource", "Id", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequest", "RequestSource", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequest", "ProgressStatus", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequest", "RequestStatus", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestProgress", "Id", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestHistory", "RequestSource", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestHistory", "ProgressStatus", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestHistory", "RequestStatus", c => c.Byte(nullable: false));
            AddPrimaryKey("dbo.DI_Log_UI", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRequestStatus", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRequestSource", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRequestProgress", "Id");
        }
    }
}
