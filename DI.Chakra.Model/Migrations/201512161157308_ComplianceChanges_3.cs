namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplianceChanges_3 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "CompliancePolicy_Id", newName: "CompliancePolicy_CompliancePolicyId");
            DropForeignKey("dbo.SODConflictFunctionCompliancePolicy", "CompliancePolicy_CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.DI_M_SODRunPolicy", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");           
            RenameColumn(table: "dbo.DI_M_CompliancePolicy", name: "CompliancePolicyId", newName: "Id");
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "CompliancePolicy_CompliancePolicyId", newName: "CompliancePolicy_Id");
            RenameIndex(table: "dbo.SODConflictFunctionCompliancePolicy", name: "IX_CompliancePolicy_CompliancePolicyId", newName: "IX_CompliancePolicy_Id");
            AddForeignKey("dbo.SODConflictFunctionCompliancePolicy", "CompliancePolicy_Id", "dbo.DI_M_CompliancePolicy", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy", "Id");
            AddForeignKey("dbo.DI_M_SODRunPolicy", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy", "Id");
            
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "CompliancePolicy_CompliancePolicyId", newName: "CompliancePolicy_Id");
            AddColumn("dbo.DI_M_CompliancePolicy", "CompliancePolicyId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.DI_M_SODRunPolicy", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.SODConflictFunctionCompliancePolicy", "CompliancePolicy_Id", "dbo.DI_M_CompliancePolicy");
            DropPrimaryKey("dbo.DI_M_CompliancePolicy");
            DropColumn("dbo.DI_M_CompliancePolicy", "Id");
            AddPrimaryKey("dbo.DI_M_CompliancePolicy", "CompliancePolicyId");
            RenameIndex(table: "dbo.SODConflictFunctionCompliancePolicy", name: "IX_CompliancePolicy_Id", newName: "IX_CompliancePolicy_CompliancePolicyId");
            RenameColumn(table: "dbo.SODConflictFunctionCompliancePolicy", name: "CompliancePolicy_Id", newName: "CompliancePolicy_CompliancePolicyId");
            AddForeignKey("dbo.DI_M_SODRunPolicy", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy", "CompliancePolicyId");
            AddForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy", "CompliancePolicyId");
            AddForeignKey("dbo.SODConflictFunctionCompliancePolicy", "CompliancePolicy_CompliancePolicyId", "dbo.DI_M_CompliancePolicy", "CompliancePolicyId", cascadeDelete: true);
           
        }
    }
}
