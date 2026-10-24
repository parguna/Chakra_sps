namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_SODPolicyExceutionDefinition",
                c => new
                    {
                        SODRPolicyExceutionId = c.Int(nullable: false, identity: true),
                        CompliancePolicyId = c.Int(),
                        UniqueRunCode = c.String(),
                        OutputType = c.Int(nullable: false),
                        RunDate = c.DateTime(nullable: false),
                        RunAtObjectLevel = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SODRPolicyExceutionId)
                .ForeignKey("dbo.DI_M_CompliancePolicy", t => t.CompliancePolicyId)
                .Index(t => t.CompliancePolicyId);
            
            CreateTable(
                "dbo.DI_M_SODEndPointPolicySelection",
                c => new
                    {
                        SODEndPointPolicySelectionId = c.Int(nullable: false, identity: true),
                        SODPolicyExceutionId = c.Int(nullable: false),
                        EndpointTypeName = c.String(),
                        LevelName = c.String(),
                        LevelValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SODEndPointPolicySelectionId)
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.SODPolicyExceutionId, cascadeDelete: true)
                .Index(t => t.SODPolicyExceutionId);
            
            CreateTable(
                "dbo.SODPolicyExceutionDefinitionSODException",
                c => new
                    {
                        SODPolicyExceutionDefinition_SODRPolicyExceutionId = c.Int(nullable: false),
                        SODException_ExceptionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SODPolicyExceutionDefinition_SODRPolicyExceutionId, t.SODException_ExceptionId })
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.SODPolicyExceutionDefinition_SODRPolicyExceutionId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_SODException", t => t.SODException_ExceptionId, cascadeDelete: true)
                .Index(t => t.SODPolicyExceutionDefinition_SODRPolicyExceutionId)
                .Index(t => t.SODException_ExceptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SODPolicyExceutionDefinitionSODException", "SODException_ExceptionId", "dbo.DI_M_SODException");
            DropForeignKey("dbo.SODPolicyExceutionDefinitionSODException", "SODPolicyExceutionDefinition_SODRPolicyExceutionId", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropForeignKey("dbo.DI_M_SODEndPointPolicySelection", "SODPolicyExceutionId", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropIndex("dbo.SODPolicyExceutionDefinitionSODException", new[] { "SODException_ExceptionId" });
            DropIndex("dbo.SODPolicyExceutionDefinitionSODException", new[] { "SODPolicyExceutionDefinition_SODRPolicyExceutionId" });
            DropIndex("dbo.DI_M_SODEndPointPolicySelection", new[] { "SODPolicyExceutionId" });
            DropIndex("dbo.DI_M_SODPolicyExceutionDefinition", new[] { "CompliancePolicyId" });
            DropTable("dbo.SODPolicyExceutionDefinitionSODException");
            DropTable("dbo.DI_M_SODEndPointPolicySelection");
            DropTable("dbo.DI_M_SODPolicyExceutionDefinition");
        }
    }
}
