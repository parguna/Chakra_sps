namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomReport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_CustomReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UniqueName = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ReportQuery = c.String(),
                        Status = c.String(),
                        LastRunOn = c.DateTime(),
                        ContentType = c.String(),
                        FileName = c.String(),
                        Attachment = c.Binary(),
                        IsLinkedToMenu = c.Boolean(nullable: false),
                        LinkedToMenu = c.Long(),
                        PageId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_CustomReportParameter",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomReportId = c.Long(nullable: false),
                        ParameterName = c.String(),
                        Description = c.String(),
                        ParameterVariant = c.Int(nullable: false),
                        ParameterType = c.Int(nullable: false),
                        Sequence = c.Long(),
                        DefaultValue = c.String(),
                        DataFormatString = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_CustomReport", t => t.CustomReportId)
                .Index(t => t.CustomReportId);
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_CustomReportParameter", "CustomReportId", "dbo.DI_M_CustomReport");
            DropIndex("dbo.DI_M_CustomReportParameter", new[] { "CustomReportId" }); 
            DropTable("dbo.DI_M_CustomReportParameter");
            DropTable("dbo.DI_M_CustomReport");
        }
    }
}
