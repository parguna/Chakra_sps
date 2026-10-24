namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W_2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_DiscoveryJobConfiguration", "DiscoveryProcessId", "dbo.DI_M_DiscoveryProcess");
            DropForeignKey("dbo.DI_M_DiscoveryJobConfiguration", "EndpointObjectId", "dbo.DI_M_EndpointObjects");
            DropIndex("dbo.DI_M_DiscoveryJobConfiguration", new[] { "DiscoveryProcessId" });
            DropIndex("dbo.DI_M_DiscoveryJobConfiguration", new[] { "EndpointObjectId" });
            DropTable("dbo.DI_M_DiscoveryJobConfiguration");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DI_M_DiscoveryJobConfiguration",
                c => new
                    {
                        DiscoveryJobId = c.Int(nullable: false, identity: true),
                        DiscoveryProcessId = c.Int(nullable: false),
                        EndpointObjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiscoveryJobId);
            
            CreateIndex("dbo.DI_M_DiscoveryJobConfiguration", "EndpointObjectId");
            CreateIndex("dbo.DI_M_DiscoveryJobConfiguration", "DiscoveryProcessId");
            AddForeignKey("dbo.DI_M_DiscoveryJobConfiguration", "EndpointObjectId", "dbo.DI_M_EndpointObjects", "EndpointObjectId", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_DiscoveryJobConfiguration", "DiscoveryProcessId", "dbo.DI_M_DiscoveryProcess", "DiscoveryProcessId", cascadeDelete: true);
        }
    }
}
