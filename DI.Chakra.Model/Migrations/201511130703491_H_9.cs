namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_ComplianceReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReportName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_ComplianceReports");
        }
    }
}
