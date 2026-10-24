namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W12 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_ADS_GroupInfo", "EndPointSystemId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_LNS_Groups", "EndPointSystemId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_Oracle11gUM_RoleInfo", "EndPointSystemId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_SAP_GROUPINFO", "EndPointSystemId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_SAP_ROLEINFO", "EndPointSystemId", c => c.Int(nullable: false));
            CreateIndex("dbo.DI_M_AccountReconcilation", "EndpointSystemId");
            AddForeignKey("dbo.DI_M_AccountReconcilation", "EndpointSystemId", "dbo.DI_M_EndpointSystem", "EndpointSystemId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_AccountReconcilation", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_M_AccountReconcilation", new[] { "EndpointSystemId" });
            AlterColumn("dbo.DI_SAP_ROLEINFO", "EndPointSystemId", c => c.Int());
            AlterColumn("dbo.DI_SAP_GROUPINFO", "EndPointSystemId", c => c.Int());
            AlterColumn("dbo.DI_Oracle11gUM_RoleInfo", "EndPointSystemId", c => c.Int());
            AlterColumn("dbo.DI_LNS_Groups", "EndPointSystemId", c => c.Int());
            AlterColumn("dbo.DI_ADS_GroupInfo", "EndPointSystemId", c => c.Int());
        }
    }
}
