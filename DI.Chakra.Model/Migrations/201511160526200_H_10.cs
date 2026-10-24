namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_ComplianceProcess",
                c => new
                    {
                        ComplianceProcessId = c.Long(nullable: false, identity: true),
                        ProcessName = c.String(),
                        Description = c.String(),
                        PolicyExecutionDefinitionId = c.Int(nullable: false),
                        Default = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ComplianceProcessId)
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.PolicyExecutionDefinitionId, cascadeDelete: true)
                .Index(t => t.PolicyExecutionDefinitionId);
            
            CreateTable(
                "dbo.DI_M_ComplianceProcessHistory",
                c => new
                    {
                        ComplianceProcessHistoryId = c.Long(nullable: false, identity: true),
                        ProcessId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        Message = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        RecordStatus = c.String(),
                    })
                .PrimaryKey(t => t.ComplianceProcessHistoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_ComplianceProcess", "PolicyExecutionDefinitionId", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropIndex("dbo.DI_M_ComplianceProcess", new[] { "PolicyExecutionDefinitionId" });
            DropTable("dbo.DI_M_ComplianceProcessHistory");
            DropTable("dbo.DI_M_ComplianceProcess");
        }
    }
}
