namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Office365LicenseConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_Office365_LicenseConfig",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        AccountSkuId = c.String(),
                        LicenseDescription = c.String(),
                        ServicePlan = c.String(),
                        ServicePlanDescription = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_Office365_LicenseConfig", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_Office365_LicenseConfig", new[] { "EndPointSystemId" });
            DropTable("dbo.DI_Office365_LicenseConfig");
        }
    }
}
