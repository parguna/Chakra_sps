namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_SODPolicyReport",
                c => new
                    {
                        SODReportId = c.Int(nullable: false, identity: true),
                        PolicyExecutionDefinitionId = c.Int(),
                        UniqueRunCode = c.String(),
                        FunctionId1 = c.Int(),
                        Role1 = c.String(),
                        FunctionItem1 = c.Int(),
                        FunctionType1 = c.String(),
                        FunctionId2 = c.Int(),
                        Role2 = c.String(),
                        FunctionItem2 = c.Int(),
                        FunctionType2 = c.String(),
                        AuthorizationCode1 = c.String(),
                        AuthorizationCode2 = c.String(),
                        ProfileCode1 = c.String(),
                        ProfileCode2 = c.String(),
                        RoleCode1 = c.String(),
                        RoleCode2 = c.String(),
                        ConflictId = c.Int(),
                        ConflictCode = c.String(),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(),
                        RunCode = c.String(),
                        PassedObjectLevelLeft = c.Boolean(nullable: false),
                        PassedObjectLevelRight = c.Boolean(nullable: false),
                        TotalMatching = c.Int(nullable: false),
                        MitigationCode = c.String(),
                        SODRunPolicyId = c.Int(),
                        ExceptionId = c.Int(),
                    })
                .PrimaryKey(t => t.SODReportId)
                .ForeignKey("dbo.DI_M_SODConflictFunctions", t => t.ConflictId)
                .ForeignKey("dbo.DI_M_SODException", t => t.ExceptionId)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId1)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId2)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItem1)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItem2)
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.PolicyExecutionDefinitionId)
                .Index(t => t.PolicyExecutionDefinitionId)
                .Index(t => t.FunctionId1)
                .Index(t => t.FunctionItem1)
                .Index(t => t.FunctionId2)
                .Index(t => t.FunctionItem2)
                .Index(t => t.ConflictId)
                .Index(t => t.ExceptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_SODPolicyReport", "PolicyExecutionDefinitionId", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionItem2", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionItem1", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "ExceptionId", "dbo.DI_M_SODException");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "ConflictId", "dbo.DI_M_SODConflictFunctions");
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "ExceptionId" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "ConflictId" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionItem2" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionId2" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionItem1" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionId1" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "PolicyExecutionDefinitionId" });
            DropTable("dbo.DI_M_SODPolicyReport");
        }
    }
}
