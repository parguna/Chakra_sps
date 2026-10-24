namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BulkRequestChanges1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DI_M_BulkRequestDetail", new[] { "BulkRequest_Id" });
            DropColumn("dbo.DI_M_BulkRequestDetail", "BulkRequestId");
            RenameColumn(table: "dbo.DI_M_BulkRequestDetail", name: "BulkRequest_Id", newName: "BulkRequestId");
            AddColumn("dbo.DI_M_Process", "DiscoveryInstance", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_BulkRequestDetail", "BulkRequestId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_BulkRequestDetail", "UserId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_BulkRequestDetail", "EndpointUserId", c => c.Long());
            AlterColumn("dbo.DI_M_BulkRequestDetail", "BulkRequestId", c => c.Long(nullable: false));
            CreateIndex("dbo.DI_M_BulkRequestDetail", "BulkRequestId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DI_M_BulkRequestDetail", new[] { "BulkRequestId" });
            AlterColumn("dbo.DI_M_BulkRequestDetail", "BulkRequestId", c => c.Long());
            AlterColumn("dbo.DI_M_BulkRequestDetail", "EndpointUserId", c => c.Int());
            AlterColumn("dbo.DI_M_BulkRequestDetail", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_BulkRequestDetail", "BulkRequestId", c => c.Int(nullable: false));
            DropColumn("dbo.DI_M_Process", "DiscoveryInstance");
            RenameColumn(table: "dbo.DI_M_BulkRequestDetail", name: "BulkRequestId", newName: "BulkRequest_Id");
            AddColumn("dbo.DI_M_BulkRequestDetail", "BulkRequestId", c => c.Int(nullable: false));
            CreateIndex("dbo.DI_M_BulkRequestDetail", "BulkRequest_Id");
        }
    }
}
