namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplianceChanges_1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SODMitigationSODRiskCode", name: "SODMitigation_MitigationId", newName: "SODMitigation_Id");
            RenameColumn(table: "dbo.SODMitigationSODRiskCode", name: "SODRiskCode_RiskId", newName: "SODRiskCode_Id");
            RenameColumn(table: "dbo.SODPolicyExceutionDefinitionSODException", name: "SODPolicyExceutionDefinition_SODRPolicyExceutionId", newName: "SODPolicyExceutionDefinition_Id");
            RenameColumn(table: "dbo.SODPolicyExceutionDefinitionSODException", name: "SODException_ExceptionId", newName: "SODException_Id");
            RenameColumn(table: "dbo.SODFunctionSODConflictFunction", name: "SodFunction_SodFunctionId", newName: "SodFunction_Id");
            RenameColumn(table: "dbo.SODFunctionSODConflictFunction", name: "SODConflictFunction_SodConflictFunctionId", newName: "SODConflictFunction_Id");
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "SODConflictFunction_SodConflictFunctionId", newName: "SODConflictFunction_Id");
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "CompliancePolicy_CompliancePolicyId", newName: "CompliancePolicy_Id");
            RenameColumn(table: "dbo.DI_M_SODException", name: "SodFunction_SodFunctionId", newName: "SodFunction_Id");
            RenameColumn(table: "dbo.DI_M_SODException", name: "SODRunPolicy_SODRunPolicyId", newName: "SODRunPolicy_Id");
            RenameColumn(table: "dbo.DI_M_SODConflict", name: "SodRiskId_RiskId", newName: "SodRiskId_Id");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.SODMitigationSODRiskCode", name: "SODMitigation_Id", newName: "SODMitigation_MitigationId");
            RenameColumn(table: "dbo.SODMitigationSODRiskCode", name: "SODRiskCode_Id", newName: "SODRiskCode_RiskId");
            RenameColumn(table: "dbo.SODPolicyExceutionDefinitionSODException", name: "SODPolicyExceutionDefinition_Id", newName: "SODPolicyExceutionDefinition_SODRPolicyExceutionId");
            RenameColumn(table: "dbo.SODPolicyExceutionDefinitionSODException", name: "SODException_Id", newName: "SODException_ExceptionId");
            RenameColumn(table: "dbo.SodFunctionSODConflictFunction", name: "SodFunction_Id", newName: "SodFunction_SodFunctionId");
            RenameColumn(table: "dbo.SodFunctionSODConflictFunction", name: "SODConflictFunction_Id", newName: "SODConflictFunction_SodConflictFunctionId");
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "SODConflictFunction_Id", newName: "SODConflictFunction_SodConflictFunctionId");
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "CompliancePolicy_Id", newName: "CompliancePolicy_CompliancePolicyId");
            RenameColumn(table: "dbo.DI_M_SODException", name: "SodFunction_Id", newName: "SodFunction_SodFunctionId");
            RenameColumn(table: "dbo.DI_M_SODException", name: "SODRunPolicy_Id", newName: "SODRunPolicy_SODRunPolicyId");
            RenameColumn(table: "dbo.DI_M_SODConflict", name: "SodRiskId_Id", newName: "SodRiskId_RiskId");
        }
    }
}
