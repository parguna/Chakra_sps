namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_4 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_SODReport", "SODReportId", "Id");
            RenameColumn("dbo.DI_M_SODConflict", "SodConflictId", "Id");
            //RenameColumn("dbo.DI_M_SODConflictFunctionMap", "SodConflictFunctionMapID", "Id");
            RenameColumn("dbo.DI_M_SODConflictFunctions", "SodConflictFunctionId", "Id");
            RenameColumn("dbo.DI_M_SODEndPointPolicySelection", "SODEndPointPolicySelectionId", "Id");
            RenameColumn("dbo.DI_M_SODException", "ExceptionId", "Id");
            RenameColumn("dbo.DI_M_SODExceptionItem", "ExceptionItemId", "Id");
            RenameColumn("dbo.DI_M_SODExecutionConfig", "SODExecutionConfigId", "Id");
            RenameColumn("dbo.DI_M_SODFunction", "SodFunctionId", "Id");
            RenameColumn("dbo.DI_M_SODFunctionItem", "SodFunctionItemId", "Id");
            RenameColumn("dbo.DI_M_SODMitigation", "MitigationId", "Id");
            RenameColumn("dbo.DI_M_SODPolicy", "SODPolicyID", "Id");
            RenameColumn("dbo.DI_M_SODPolicyExceutionDefinition", "SODRPolicyExceutionId", "Id");
            RenameColumn("dbo.DI_M_SODPolicyReport", "SODReportId", "Id");
            RenameColumn("dbo.DI_M_SODRiskCode", "RiskId", "Id");
            RenameColumn("dbo.DI_M_SODRiskLevel", "RiskLevelId", "Id");
            RenameColumn("dbo.DI_M_SODRiskScore", "RiskScoreId", "Id");
            RenameColumn("dbo.DI_M_SODRunPolicy", "SODRunPolicyId", "Id");
            RenameColumn("dbo.DI_M_SODRunPolicySelection", "SODRunPolicySelectionId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_SODReport", "Id", "SODReportId");
            RenameColumn("dbo.DI_M_SODConflict", "Id", "SodConflictId");
            //RenameColumn("dbo.DI_M_SODConflictFunctionMap", "Id", "SodConflictFunctionMapID");
            RenameColumn("dbo.DI_M_SODConflictFunctions", "Id", "SodConflictFunctionId");
            RenameColumn("dbo.DI_M_SODEndPointPolicySelection", "Id", "SODEndPointPolicySelectionId");
            RenameColumn("dbo.DI_M_SODException", "Id", "ExceptionId");
            RenameColumn("dbo.DI_M_SODExceptionItem", "Id", "ExceptionItemId");
            RenameColumn("dbo.DI_M_SODExecutionConfig", "Id", "SODExecutionConfigId");
            RenameColumn("dbo.DI_M_SODFunction", "Id", "SodFunctionId");
            RenameColumn("dbo.DI_M_SODFunctionItem", "Id", "SodFunctionItemId");
            RenameColumn("dbo.DI_M_SODMitigation", "Id", "MitigationId");
            RenameColumn("dbo.DI_M_SODPolicy", "Id", "SODPolicyID");
            RenameColumn("dbo.DI_M_SODPolicyExceutionDefinition", "Id", "SODRPolicyExceutionId");
            RenameColumn("dbo.DI_M_SODPolicyReport", "Id", "SODReportId");
            RenameColumn("dbo.DI_M_SODRiskCode", "Id", "RiskId");
            RenameColumn("dbo.DI_M_SODRiskLevel", "Id", "RiskLevelId");
            RenameColumn("dbo.DI_M_SODRiskScore", "Id", "RiskScoreId");
            RenameColumn("dbo.DI_M_SODRunPolicy", "Id", "SODRunPolicyId");
            RenameColumn("dbo.DI_M_SODRunPolicySelection", "Id", "SODRunPolicySelectionId");
        }
    }
}
